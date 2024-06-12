using backend.DLLScanner;
using backend.DLLScanner.Utilis;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Plugins;
using PluginBase.Contract;

namespace backend.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, Tuple<ICrawler, PluginStatus>> StoryPlugin;
        public Dictionary<string, Tuple<IExporter, PluginStatus>> ExporterPlugin;


        public IndexModel()
        {
            StoryPlugin = ScannerController.Instance.sourceScanner.Commands;
            ExporterPlugin = ScannerController.Instance.exporterScanner.Commands;
        }

        public void OnGet()
        {
        }
    }
}
