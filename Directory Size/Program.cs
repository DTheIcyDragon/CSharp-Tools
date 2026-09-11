using System.Collections.Frozen;

namespace IcyDragonsHelper;

class Program
{
    private readonly static IEnumerable<string> _disallowedDirectories = new List<string>(["System Volume Information".ToLowerInvariant(), "$Recycle.Bin".ToLowerInvariant()]).ToFrozenSet();

    static void Main(string[] args)
    {
        string workingDirectory;

        if (args.Length == 0)
        {
            workingDirectory = Directory.GetCurrentDirectory();
        }
        else
        {
            workingDirectory = args[0];
        }
        PrintDirectorySize(workingDirectory);
    }


    internal static void PrintDirectorySize(string directoryPath)
    {
        if (Directory.GetLogicalDrives().Any(logicalDrive => logicalDrive == directoryPath))
        {
            foreach (string mainFolder in Directory.GetDirectories(directoryPath))
            {
                PrintDirectorySize(mainFolder);
            }
        }
        else if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"The directory '{directoryPath}' does not exist.");
            return;
        }
        else
        {
            long totalSize = GetDirectorySize(directoryPath);
            if (totalSize == 0)
            {
                // If the size is 0, it could be due to access restrictions or the directory being empty output is in GetDirectorySize
            }
            else
            {
                Console.WriteLine($"Total size of '{directoryPath}': {FormatSize(totalSize)}");
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