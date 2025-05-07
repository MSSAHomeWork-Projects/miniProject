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
                {".rar", "Archives" }
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
                string ext = Path.GetExtension(file);
                if (FileExtension.TryGetValue(ext, out string category))
                {
                    if (!FileGroups.ContainsKey(category))
                        FileGroups[category] = new List<string>();

                    FileGroups[category].Add(file);
                }
            }
        }
    }


}
