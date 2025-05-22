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
            FileScanner scanner = new FileScanner(selectedFolderPath);
            dataGridView1.Rows.Clear();
            //zip files population on dataGridViewer1
            var deletedFiles = scanner.DeleteOldInstallers();
            foreach (var deleted in deletedFiles)
            {
                int row = dataGridView1.Rows.Add(deleted.fileName, deleted.category, deleted.destination);
                dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
            }
            scanner.FilterByType();

            int totalFiles = scanner.FileGroups.Sum(group => group.Value.Count);
            int processedFiles = 0;

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
                            string zipFileName = Path.GetFileName(zipPath);
                            string? extractedPath = mover.ExtractZipFiles(zipPath);

                            if (extractedPath != null)
                            {
                                int row = dataGridView1.Rows.Add(zipFileName, "Zip File", $"Extracted to {extractedPath}");
                                dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.OrangeRed;
                            }
                        }
                    }
                    continue;
                }

                foreach (string file in category.Value)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationRoot = (category.Key == "Archives" && !chkExtractZips.Checked)
                        ? selectedFolderPath : mover.GetCategoryDestination(category.Key);
                    string destPath = Path.Combine(destinationRoot, fileName);
                    mover.MoveFile(file, destPath);
                    string destinationFolder = Path.GetDirectoryName(destPath);
                    dataGridView1.Rows.Add(fileName, category.Key, destinationFolder);

                    processedFiles++;
                    progressBar1.Value = (int)((processedFiles / (double)totalFiles) * 100);
                    progressBar1.Refresh();
                }
            }
            progressBar1.Value = 100;
            progressBar1.Refresh();
            MessageBox.Show("Files sorted.");
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
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
                ".mp4", ".mov", ".avi", ".mkv",
                ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                ".pdf", ".txt", ".csv", ".rtf"
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
        }
    }
}
