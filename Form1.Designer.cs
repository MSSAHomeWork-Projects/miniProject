namespace MiniProject
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            btnSelectFolder = new Button();
            lblSourceTitle = new Label();
            BtnSortFiles = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            lblSelectedPath = new Label();
            chkExtractZips = new CheckBox();
            progressBar1 = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.FromArgb(216, 220, 223);
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(30, 127, 176);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = Color.LightSteelBlue;
            dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.Location = new Point(41, 124);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 28;
            dataGridView1.Size = new Size(808, 272);
            dataGridView1.TabIndex = 0;
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.BackColor = Color.FromArgb(152, 212, 34);
            btnSelectFolder.FlatStyle = FlatStyle.Flat;
            btnSelectFolder.ForeColor = Color.Black;
            btnSelectFolder.Location = new Point(41, 71);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(86, 23);
            btnSelectFolder.TabIndex = 1;
            btnSelectFolder.Text = "Select Folder";
            btnSelectFolder.UseVisualStyleBackColor = false;
            btnSelectFolder.Click += BtnSelectFolder_Click;
            // 
            // lblSourceTitle
            // 
            lblSourceTitle.AutoSize = true;
            lblSourceTitle.ForeColor = Color.Black;
            lblSourceTitle.Location = new Point(41, 30);
            lblSourceTitle.Name = "lblSourceTitle";
            lblSourceTitle.Size = new Size(46, 15);
            lblSourceTitle.TabIndex = 2;
            lblSourceTitle.Text = "Source:";
            // 
            // BtnSortFiles
            // 
            BtnSortFiles.BackColor = Color.FromArgb(152, 212, 34);
            BtnSortFiles.FlatStyle = FlatStyle.Flat;
            BtnSortFiles.ForeColor = Color.Black;
            BtnSortFiles.Location = new Point(150, 71);
            BtnSortFiles.Name = "BtnSortFiles";
            BtnSortFiles.Size = new Size(75, 23);
            BtnSortFiles.TabIndex = 3;
            BtnSortFiles.Text = "Sort Files";
            BtnSortFiles.UseVisualStyleBackColor = false;
            BtnSortFiles.Click += BtnSortFiles_Click;
            // 
            // lblSelectedPath
            // 
            lblSelectedPath.AutoSize = true;
            lblSelectedPath.ForeColor = Color.Black;
            lblSelectedPath.Location = new Point(150, 30);
            lblSelectedPath.Name = "lblSelectedPath";
            lblSelectedPath.Size = new Size(36, 15);
            lblSelectedPath.TabIndex = 0;
            lblSelectedPath.Text = "None";
            // 
            // chkExtractZips
            // 
            chkExtractZips.AutoSize = true;
            chkExtractZips.Location = new Point(260, 75);
            chkExtractZips.Name = "chkExtractZips";
            chkExtractZips.Size = new Size(107, 19);
            chkExtractZips.TabIndex = 4;
            chkExtractZips.Text = "Extract Zip Files";
            chkExtractZips.UseVisualStyleBackColor = true;
            chkExtractZips.CheckedChanged += ChkExtractZips_CheckedChanged;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(439, 71);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(410, 23);
            progressBar1.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(137, 197, 245);
            ClientSize = new Size(884, 450);
            Controls.Add(progressBar1);
            Controls.Add(chkExtractZips);
            Controls.Add(lblSelectedPath);
            Controls.Add(BtnSortFiles);
            Controls.Add(lblSourceTitle);
            Controls.Add(btnSelectFolder);
            Controls.Add(dataGridView1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button btnSelectFolder;
        private Label lblSourceTitle;
        private Button BtnSortFiles;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label lblSelectedPath;
        private CheckBox chkExtractZips;
        private ProgressBar progressBar1;
    }
}
