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
            var deletedFiles = scanner.DeleteOldInstallers();
            foreach (var deleted in  deletedFiles)
            {
                int row = dataGridView1.Rows.Add(deleted.fileName, deleted.category, deleted.destination);
                dataGridView1.Rows[row].DefaultCellStyle.ForeColor = Color.Red;
            }
            scanner.FilterByType();

            int totalFiles = scanner.FileGroups.Sum(group => group.Value.Count);
            int processedFiles = 0;

            FileMover mover = new FileMover();

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
                                int row = dataGridView1.Rows.Add(zipFileName, "Download", $"Extracted to {extractedPath}");
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

    }
}
