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
        public void FilterByType(List<string>? skipPaths = null)
        {
            FileGroups.Clear();
            List<string> allFiles = GetFiles();

            HashSet<string> normalizedSkips = skipPaths != null
                ? new HashSet<string>(skipPaths.Select(p => Path.GetFullPath(p).ToLower()))
                : new HashSet<string>();

            foreach (var file in allFiles)
            {
                string normalized = Path.GetFullPath(file).ToLower();
                if (normalizedSkips.Contains(normalized))
                {
                    continue;
                }
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
        public List<string> DeleteOldInstallers(int numDays = 30)
        {
            List<string> deletedFiles = new();
            List<string> allFiles = GetFiles();

            //checks if the source is the dowloads folder before deleting .exe
            //if not logs in the dataGridViewer1 with messages saying they were skipped
            string safePath = SourceDirectory.ToLower();
            if (!safePath.Contains("downloads"))
            {
                return deletedFiles;
            }

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
                            deletedFiles.Add(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delte {file}: {ex.Message}");
                        }
                    }
                }
            }
            return deletedFiles;
        }
    }


}
