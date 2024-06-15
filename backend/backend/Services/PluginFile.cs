using Backend.Application.DLLScanner.Contract;
using Backend.Domain.Contract;
using System.Reflection;

namespace Backend.Services;

public class PluginFile
{
    public static string UploadFiles<T>(IScanner<T> scanner, List<IFormFile> files) where T : IPlugin
    {
        var message = string.Empty;
        var numberSuccess = 0;
        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                try
                {
                    var pluginFolderPath = scanner.PluginsFolder;
                    var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception(), pluginFolderPath, formFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    numberSuccess++;
                }
                catch (Exception ex)
                {
                    message += ex.Message + "\n";
                }
            }
        }
        message += $"\nLoaded {numberSuccess} file(s).";
        scanner.ScanDLLFiles();
        return message;
    }
}
