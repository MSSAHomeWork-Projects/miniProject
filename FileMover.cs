using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;

namespace MiniProject
{
    public class FileMover
    {
        public string ZipExtractToDirectory { get; set; }
        public int FilesMoved { get; set; }

        public FileMover()
        {
            ZipExtractToDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }

        public string? ExtractZipFiles(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                return null;
            }

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(zipFilePath);
                string extractFolder = Path.Combine(ZipExtractToDirectory, fileName);

                Directory.CreateDirectory(extractFolder);
                ZipFile.ExtractToDirectory(zipFilePath, extractFolder, true);
                FilesMoved++;

                FileSystem.DeleteFile(zipFilePath);
                return extractFolder;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to extract {zipFilePath}: {e.Message}");
                return null;
            }
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }
            string destDirectory = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            if (!File.Exists(destinationPath))
            {
                File.Move(sourcePath, destinationPath);
            }
        }

        public string GetCategoryDestination(string category)
        {
            return category switch
            {
                "Images" => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "Music" => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                "Videos" => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "Documents" => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Archives" => ZipExtractToDirectory,
                "Code" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Code"),
                "Fonts" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts"),
                "System" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SystemFiles"),
                "Downloads" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                "Installers" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Installers"),
                "Miscellaneous" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Miscellaneous"),
                _ => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            };
        }
    }
}
