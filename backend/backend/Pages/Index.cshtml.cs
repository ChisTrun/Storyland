using Backend.Application.DLLScanner.Utilis;
using Backend.Application.Services.Abstract;
using Backend.Domain.Contract;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public class IndexModel : PageModel
{
    private readonly IPluginsScannerService _pluginsScannerService;

    public Dictionary<string, PluginInfo<ICrawler>> StoryPlugin => _pluginsScannerService.GetCrawlerScanner().GetAllPlugins();
    public Dictionary<string, PluginInfo<IExporter>> ExporterPlugin => _pluginsScannerService.GetExporterScanner().GetAllPlugins();

    public IndexModel(IPluginsScannerService pluginsScannerService) => _pluginsScannerService = pluginsScannerService;

    public void OnGet()
    {
    }
}
