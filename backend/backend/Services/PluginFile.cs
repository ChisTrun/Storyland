using backend.Application.DLLScanner.Contract;
using backend.Application.Services.Abstract;
using backend.Domain.Contract;
using System.Reflection;

namespace backend.Services;

public class PluginFile
{
    public static string UploadFiles<T>(IScanner<T> scanner, List<IFormFile> files) where T : IPlugin
    {
        string message = string.Empty;
        int numberSuccess = 0;
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
