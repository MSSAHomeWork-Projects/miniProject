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
                // Images
                {".jpg", "Images" },
                {".jpeg", "Images" },
                {".png", "Images" },
                {".gif", "Images" },
                {".webp", "Images" },
                {".bmp", "Images" },
                {".tiff", "Images" },
                {".tif", "Images" },
                {".svg", "Images" },
                {".ico", "Images" },
                {".raw", "Images" },
                {".cr2", "Images" },
                {".nef", "Images" },
                {".dng", "Images" },
                
                // Audio/Music
                {".mp3", "Music" },
                {".wav", "Music" },
                {".flac", "Music" },
                {".aac", "Music" },
                {".ogg", "Music" },
                {".wma", "Music" },
                {".m4a", "Music" },
                {".opus", "Music" },
                
                // Videos
                {".mp4", "Videos" },
                {".mov", "Videos" },
                {".avi", "Videos" },
                {".mkv", "Videos" },
                {".wmv", "Videos" },
                {".flv", "Videos" },
                {".webm", "Videos" },
                {".m4v", "Videos" },
                {".3gp", "Videos" },
                {".3g2", "Videos" },
                {".f4v", "Videos" },
                {".asf", "Videos" },
                
                // Documents
                {".docx", "Documents" },
                {".doc", "Documents" },
                {".pdf", "Documents" },
                {".txt", "Documents" },
                {".xlsx", "Documents" },
                {".xls", "Documents" },
                {".csv", "Documents" },
                {".pptx", "Documents" },
                {".ppt", "Documents" },
                {".rtf", "Documents" },
                {".odt", "Documents" },
                {".ods", "Documents" },
                {".odp", "Documents" },
                {".pages", "Documents" },
                {".numbers", "Documents" },
                {".keynote", "Documents" },
                
                // Archives
                {".zip", "Archives" },
                {".rar", "Archives" },
                {".7z", "Archives" },
                {".tar", "Archives" },
                {".gz", "Archives" },
                {".bz2", "Archives" },
                {".xz", "Archives" },
                {".tar.gz", "Archives" },
                {".tar.bz2", "Archives" },
                {".tar.xz", "Archives" },
                
                // Installers
                {".exe", "Installers"},
                {".msi", "Installers"},
                {".dmg", "Installers"},
                {".pkg", "Installers"},
                {".deb", "Installers"},
                {".rpm", "Installers"},
                {".appx", "Installers"},
                {".appxbundle", "Installers"},
                
                // Code/Development files
                {".js", "Code" },
                {".html", "Code" },
                {".htm", "Code" },
                {".css", "Code" },
                {".json", "Code" },
                {".xml", "Code" },
                {".py", "Code" },
                {".cs", "Code" },
                {".java", "Code" },
                {".cpp", "Code" },
                {".c", "Code" },
                {".h", "Code" },
                {".hpp", "Code" },
                {".php", "Code" },
                {".rb", "Code" },
                {".go", "Code" },
                {".rs", "Code" },
                {".swift", "Code" },
                {".kt", "Code" },
                {".scala", "Code" },
                {".sql", "Code" },
                {".sh", "Code" },
                {".bat", "Code" },
                {".ps1", "Code" },
                
                // Fonts
                {".ttf", "Fonts" },
                {".otf", "Fonts" },
                {".woff", "Fonts" },
                {".woff2", "Fonts" },
                {".eot", "Fonts" },
                
                // Disk Images and System files
                {".iso", "System" },
                {".img", "System" },
                {".vhd", "System" },
                {".vmdk", "System" },
                
                // Torrents and Downloads
                {".torrent", "Downloads" },
                {".crdownload", "Downloads" },
                {".download", "Downloads" },
                {".part", "Downloads" },
                {".tmp", "Downloads" }
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
                try
                {
                    string normalized = Path.GetFullPath(file).ToLower();
                    if (normalizedSkips.Contains(normalized))
                    {
                        continue;
                    }
                    
                    string ext = Path.GetExtension(file).ToLower();
                    string category;
                    
                    // Check if we have a known extension
                    if (FileExtension.TryGetValue(ext, out category))
                    {
                        if (!FileGroups.ContainsKey(category))
                            FileGroups[category] = new List<string>();

                        FileGroups[category].Add(file);
                    }
                    else
                    {
                        // Handle unknown file extensions - put them in "Miscellaneous" category
                        category = "Miscellaneous";
                        if (!FileGroups.ContainsKey(category))
                            FileGroups[category] = new List<string>();
                            
                        FileGroups[category].Add(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                    // Continue processing other files instead of failing completely
                }
            }
        }

        //filters through installer files for Apps and deletes any older 30 days
        public List<string> DeleteOldInstallers(int numDays = 30)
        {
            List<string> deletedFiles = new();
            
            try
            {
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
                    try
                    {
                        string ext = Path.GetExtension(file).ToLower();
                        if (ext == ".exe" || ext == ".msi")
                        {
                            DateTime lastModified = File.GetLastWriteTime(file);
                            if (lastModified < DateTime.Now.AddDays(-numDays))
                            {
                                File.Delete(file);
                                deletedFiles.Add(file);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteOldInstallers: {ex.Message}");
            }
            
            return deletedFiles;
        }
    }
}
