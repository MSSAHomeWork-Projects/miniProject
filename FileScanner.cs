namespace MiniProject
{
    public class FileScanner
    {
        public string SourceDirectory { get; set; }
        public Dictionary<string, List<string>> FileGroups { get; set; } = new();
        public Dictionary<string, string> FileExtension { get; set; } = new();

        public FileScanner(string sourceDirectory)
        {
            SourceDirectory = sourceDirectory;
            FileGroups = new Dictionary<string, List<string>>();
            FileExtension = new(StringComparer.OrdinalIgnoreCase)
            {
                {".jpg", "Images" },
                {".jpeg", "Images" },
                {".png", "Images" },
                {".gif", "Images" },
                {".webp", "Images" },
                {".mp3", "Music" },
                {".wav", "Music" },
                {".mp4", "Videos" },
                {".mov", "Videos" },
                {".docx", "Documents" },
                {".pdf", "Documents" },
                {".txt", "Documents" },
                {".xlsx", "Documents" },
                {".csv", "Documents" },
                {".pptx", "Documents" },
                {".rtf", "Documents" },
                {".zip", "Archives" },
                {".rar", "Archives" },
                {".exe", "Installers"}
            };
        }
        

        //Get all of the files and return a list
        public List<string> GetFiles()
        {
            if (!Directory.Exists(SourceDirectory))
            {
                throw new DirectoryNotFoundException("Source directory does not exist.");
            }
            return new List<string>(Directory.GetFiles(SourceDirectory, "*", SearchOption.TopDirectoryOnly));
        }

        //Filters and places in the FileGroups Dictionary
        public void FilterByType()
        {
            FileGroups.Clear();
            List<string> allFiles = GetFiles();

            foreach (var file in allFiles)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (FileExtension.TryGetValue(ext, out string category))
                {
                    if (!FileGroups.ContainsKey(category))
                        FileGroups[category] = new List<string>();

                    FileGroups[category].Add(file);
                }
            }
        }

        //filters through installer files for Apps and deletes any older 30 days
        public List<(string fileName, string category, string destination)> DeleteOldInstallers(int numDays = 30)
        {
            List<(string fileName, string category, string destination)> deletedFiles = new();
            List<string> allFiles = GetFiles();

            foreach( var file in allFiles)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".exe")
                {
                    DateTime lastModified = File.GetLastWriteTime(file);
                    if (lastModified < DateTime.Now.AddDays(-numDays))
                    {
                        try
                        {
                            File.Delete(file);
                            deletedFiles.Add((Path.GetFileName(file), "Installers", "Deleted"));
                        }
                        catch (Exception ex)
                        {
                            deletedFiles.Add((Path.GetFileName(file),"Installers", $"Deleted failed: {ex.Message}"));
                        }
                    }
                }
            }
            return deletedFiles;
        }
    }


}
