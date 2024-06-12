using backend.DLLScanner;
using backend.DLLScanner.Concrete;

namespace backend.Handler
{
    public class ExporterHandler
    {
        public static bool CheckExporterID(string id)
        {
            var plugin = ScannerController.Instance.exporterScanner.Commands[id];
            return plugin != null && plugin.Item2 != DLLScanner.Utilis.PluginStatus.Removed;
        }
    }
}
