using backend.Application.DLLScanner.Utilis;
using backend.Application.Services.Abstract;
using backend.Domain.Contract;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages;

public class IndexModel : PageModel
{
    private readonly IPluginsScannerService _pluginsScannerService;

    public Dictionary<string, PluginInfo<ICrawler>> StoryPlugin => _pluginsScannerService.GetCrawlerScanner().GetAllPlugins();
    public Dictionary<string, PluginInfo<IExporter>> ExporterPlugin => _pluginsScannerService.GetExporterScanner().GetAllPlugins();

    public IndexModel(IPluginsScannerService pluginsScannerService)
    {
        _pluginsScannerService = pluginsScannerService;
    }

    public void OnGet()
    {
    }
}
