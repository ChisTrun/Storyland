using backend.DLLScanner;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PluginBase.Contract;

namespace backend.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, ICrawler> StoryPlugin;
        public Dictionary<string, IExporter> ExporterPlugin;


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
