using System;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
namespace MiniProject
{
    public partial class Form1 : Form
    {
        private bool extractZips;
        private string selectedFolderPath;
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // Show welcome/how-to-use dialog
            ShowWelcomeDialog();

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("FileName", "File Name");
                dataGridView1.Columns.Add("Category", "Category");
                dataGridView1.Columns.Add("Destination", "Destination");
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns["FileName"].FillWeight = 35;
            dataGridView1.Columns["Category"].FillWeight = 15;
            dataGridView1.Columns["Destination"].FillWeight = 50;


        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFolderPath = folderBrowserDialog1.SelectedPath;
                lblSelectedPath.Text = selectedFolderPath;
                MessageBox.Show($"Folder Path: {selectedFolderPath}");
            }
        }

        private void BtnSortFiles_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                MessageBox.Show("No folder selected. Select a folder to sort.");
                return;
            }
            
            try
            {
                FileScanner scanner = new FileScanner(selectedFolderPath);
                dataGridView1.Rows.Clear();
                
                //zip files population on dataGridViewer1
                var deletedFiles = scanner.DeleteOldInstallers();
                foreach (var deleted in deletedFiles)
                {
                    int row = dataGridView1.Rows.Add(Path.GetFileName(deleted), "Installers", "Deleted");
                    dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                }
                scanner.FilterByType(deletedFiles);

                int totalFiles = scanner.FileGroups.Sum(group => group.Value.Count);
                int processedFiles = 0;

                // Prevent division by zero
                if (totalFiles == 0)
                {
                    MessageBox.Show("No files to sort in the selected folder.");
                    return;
                }

                FileMover mover = new FileMover();
                //
                foreach (var category in scanner.FileGroups)
                {
                    string destination = mover.GetCategoryDestination(category.Key);
                    if (category.Key == "Archives")
                    {
                        if (chkExtractZips.Checked)
                        {
                            foreach (string zipPath in category.Value)
                            {
                                try
                                {
                                    string zipFileName = Path.GetFileName(zipPath);
                                    string? extractedPath = mover.ExtractZipFiles(zipPath);

                                    if (extractedPath != null)
                                    {
                                        int row = dataGridView1.Rows.Add(zipFileName, "Zip File", $"Extracted to {extractedPath}");
                                        dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.OrangeRed;
                                    }
                                    else
                                    {
                                        int row = dataGridView1.Rows.Add(zipFileName, "Zip File", "Extraction failed");
                                        dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    int row = dataGridView1.Rows.Add(Path.GetFileName(zipPath), "Zip File", $"Error: {ex.Message}");
                                    dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                                }
                            }
                        }
                        continue;
                    }

                    foreach (string file in category.Value)
                    {
                        try
                        {
                            string fileName = Path.GetFileName(file);
                            
                            //Special handling for installers
                            if (category.Key == "Installers")
                            {
                                // If installer is not from downloads folder, skip it
                                if (!file.ToLower().Contains("downloads"))
                                {
                                    dataGridView1.Rows.Add(fileName, "Installers", "Skipped (Not from Downloads folder)");
                                    processedFiles++;
                                    continue;
                                }
                                // If installer is from downloads folder, it should stay there (newer ones)
                                // Older ones were already deleted by DeleteOldInstallers
                                else
                                {
                                    dataGridView1.Rows.Add(fileName, "Installers", "Kept in Downloads (Recent installer)");
                                    processedFiles++;
                                    continue;
                                }
                            }
                            
                            string destinationRoot;

                            if (category.Key == "Archives" && !chkExtractZips.Checked)
                            {
                                destinationRoot = selectedFolderPath;
                            }
                            else
                            {
                                destinationRoot = mover.GetCategoryDestination(category.Key);
                            }
                            string destPath = Path.Combine(destinationRoot, fileName);
                            
                            //calling Movefile from FileMover Class
                            mover.MoveFile(file, destPath);

                            string destinationFolder = Path.GetDirectoryName(destPath);
                            dataGridView1.Rows.Add(fileName, category.Key, destinationFolder);
                        }
                        catch (Exception ex)
                        {
                            // Add error row instead of crashing
                            int row = dataGridView1.Rows.Add(Path.GetFileName(file), category.Key, $"Error: {ex.Message}");
                            dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
                        }
                        finally
                        {
                            processedFiles++;
                            // Update progress bar safely
                            int progressValue = Math.Min(100, (int)((processedFiles / (double)totalFiles) * 100));
                            progressBar1.Value = progressValue;
                            progressBar1.Refresh();
                        }
                    }
                }
                progressBar1.Value = 100;
                progressBar1.Refresh();
                MessageBox.Show($"Files sorted successfully. Processed {processedFiles} files.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while sorting files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkExtractZips_CheckedChanged(object sender, EventArgs e)
        {
            extractZips = chkExtractZips.Checked;
        }

        private void BtnDeleteDuplicates_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }

            //Prevent scanning critical system folders
            string safePath = Path.GetFullPath(selectedFolderPath).ToLower();
            if (safePath.StartsWith(@"c:\windows") ||
                safePath.StartsWith(@"c:\program files") ||
                safePath.StartsWith(@"c:\program files (x86)") ||
                safePath.StartsWith(@"c:\programdata") ||
                safePath.StartsWith(@"c:\users\default") ||
                safePath.StartsWith(@"c:\system32"))
            {
                MessageBox.Show("This folder is protected and cannot be scanned for duplicates.\n" + 
                    "Please choose a user folder such as Downloads, Documents, or Pictures.",
                    "Folder Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Folder protection Doesn't allow entry into windows, program files and only looks through certain extensions for duplicates
            string[] allowedExtensions =
            {
                // Images
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg", ".ico",
                // Videos
                ".mp4", ".mov", ".avi", ".mkv", ".wmv", ".flv", ".webm", ".m4v", ".3gp",
                // Audio
                ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a",
                // Documents
                ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                ".pdf", ".txt", ".csv", ".rtf", ".odt", ".ods", ".odp",
                // Archives (for duplicate detection)
                ".zip", ".rar", ".7z"
            };

            Dictionary<string, List<string>> hashToFiles = new Dictionary<string, List<string>>();
            var files = Directory.GetFiles(selectedFolderPath, "*", SearchOption.AllDirectories)
                .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower())).ToArray();

            using (var sha256 = SHA256.Create())
            {
                foreach (var file in files)
                {
                    try
                    {
                        using (var stream = File.OpenRead(file))
                        {
                            var hashBytes = sha256.ComputeHash(stream);
                            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                            if(!hashToFiles.ContainsKey(hash))
                            {
                                hashToFiles[hash] = new List<string>();
                            }
                            hashToFiles[hash].Add(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to hash file {file}: {ex.Message}");
                    }
                }
            }

            int duplicatesDeleted = 0;
            foreach (var kvp in hashToFiles)
            {
                var duplicates = kvp.Value;
                if (duplicates.Count > 1)
                {
                    for (int i = 1; i < duplicates.Count; i++)
                    {
                        try
                        {
                            File.Delete(duplicates[i]);
                            duplicatesDeleted++;

                            int row = dataGridView1.Rows.Add(Path.GetFileName(duplicates[i]), "Duplicate", "Deleted");
                            dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Orange;
                        }

                        catch (Exception ex)
                        {
                            int row = dataGridView1.Rows.Add(Path.GetFileName(duplicates[i]), "Error", $"Deleted failed {ex.Message}");
                            dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.OrangeRed;
                        }

                    }
                }
            }
            if (duplicatesDeleted == 0)
            {
                MessageBox.Show("No duplicates Found.");
            }
            else
            {
                MessageBox.Show($"{duplicatesDeleted} duplicates deleted");
            }
        }

        private void ShowWelcomeDialog()
        {
            // Create a custom dialog form for better readability
            Form welcomeForm = new Form()
            {
                Text = "How to Use File Organizer",
                Size = new Size(600, 700),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            // Create a RichTextBox for better text formatting
            RichTextBox textBox = new RichTextBox()
            {
                Location = new Point(20, 20),
                Size = new Size(540, 580),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = welcomeForm.BackColor,
                Font = new Font("Segoe UI", 10F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            string welcomeMessage = "Welcome to File Organizer!\n\n" +
                                   "Here's how this application works:\n\n" +
                                   "📁 FILE SORTING:\n" +
                                   "Files will be automatically sorted into folders based on their type:\n" +
                                   "   • Images → Images folder\n" +
                                   "   • Videos → Videos folder\n" +
                                   "   • Music → Music folder\n" +
                                   "   • Documents → Documents folder\n" +
                                   "   • Archives → Archives folder\n" +
                                   "   • Code → Code folder\n" +
                                   "   • Fonts → Fonts folder\n" +
                                   "   • And other categories...\n\n" +
                                   "📦 UNZIP FILES:\n" +
                                   "When you check 'Unzip Files':\n" +
                                   "   • Zip files will be extracted\n" +
                                   "   • The unzipped folder will be placed in Downloads\n" +
                                   "   • The original zip file will be deleted\n\n" +
                                   "🗑️ INSTALLER CLEANUP:\n" +
                                   "Installer files (.exe, .msi) older than 30 days will be automatically deleted.\n" +
                                   "⚠️ This only applies to files in the Downloads folder for safety.\n\n" +
                                   "🔄 DUPLICATE REMOVAL:\n" +
                                   "Duplicates can only be deleted from:\n" +
                                   "   • Images folders\n" +
                                   "   • Videos folders\n" +
                                   "   • Music folders\n" +
                                   "   • Documents folders\n" +
                                   "This ensures system files remain safe.\n\n" +
                                   "Click OK to continue!";

            textBox.Text = welcomeMessage;

            // Create OK button
            Button okButton = new Button()
            {
                Text = "OK",
                Size = new Size(100, 35),
                Location = new Point(250, 620),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9F)
            };

            // Add controls to form
            welcomeForm.Controls.Add(textBox);
            welcomeForm.Controls.Add(okButton);
            welcomeForm.AcceptButton = okButton;

            // Show the dialog
            welcomeForm.ShowDialog(this);
            welcomeForm.Dispose();
        }
    }
}
