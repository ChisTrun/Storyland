namespace ExporterEPUB.Helpers;

public class FileGenerator
{
    public static string SaveImage(string uri, FolderStructure structure)
    {
        using var client = new HttpClient();
        using var s = client.GetStreamAsync(uri);
        var guidString = Guid.NewGuid().ToString();
        var path = Path.Combine(structure.ABS_OEBPS_IMAGES, $"{guidString}-IMG-{DateTime.Now:dd-MM-yyyy}.jpg");
        using var fs = new FileStream(path, FileMode.Create);
        s.Result.CopyTo(fs);
        return path;
    }

    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }
        var dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }
        if (recursive)
        {
            foreach (var subDir in dirs)
            {
                var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}


