using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace IcyDragonsHelper.DirectoryTools
{
    internal class DirectorySize
    {
        private readonly static IEnumerable<string> _disallowedDirectories = new List<string>(["System Volume Information".ToLowerInvariant(), "$Recycle.Bin".ToLowerInvariant()]).ToFrozenSet();

        public static void PrintDirectorySize(string strDirectory)
        {
            if (Directory.GetLogicalDrives().Any(logicalDrive => logicalDrive == strDirectory))
            {
                foreach (string mainFolder in Directory.GetDirectories(strDirectory))
                {
                    PrintDirectorySize(mainFolder);
                }
            }
            else if (!Directory.Exists(strDirectory))
            {
                Console.WriteLine($"The directory '{strDirectory}' does not exist.");
                return;
            }
            else
            {
                long totalSize = GetDirectorySize(strDirectory);
                if (totalSize == 0)
                {
                    // If the size is 0, it could be due to access restrictions or the directory being empty output is in GetDirectorySize
                }
                else
                {
                    Console.WriteLine($"Total size of '{strDirectory}': {FormatSize(totalSize)}");
                }
            }
        }

        internal static long GetDirectorySize(string directoryPath)
        {
            if (_disallowedDirectories.Contains(new DirectoryInfo(directoryPath).Name.ToLowerInvariant()))
            {
                Console.WriteLine($"Skipping disallowed directory: {directoryPath}");
                return 0;
            }
            long size = 0;
            try
            {
                // Get all files in the directory and its subdirectories
                string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    size += fileInfo.Length;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to some files or directories: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while calculating the directory size: {ex.Message}");
            }
            return size;
        }

        internal static string FormatSize(long sizeInBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = sizeInBytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
