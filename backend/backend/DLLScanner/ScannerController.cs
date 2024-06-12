using backend.DLLScanner.Concrete;
using backend.DLLScanner.Contract;
using PluginBase.Contract;
using System.Reflection;

namespace backend.DLLScanner
{
    public class ScannerController
    {
        public IScanner<ICrawler> sourceScanner;
        public IScanner<IExporter> exporterScanner;
        private static readonly Lazy<ScannerController> _lazy = new(() => new ScannerController());
        public static ScannerController Instance => _lazy.Value;

        private ScannerController()
        {
            sourceScanner = StorySourceScanner.Instance;
            exporterScanner = ExporterScanner.Instance;

            StartToScan();
        }

        public void StartToScan()
        {
            sourceScanner.ScanDLLFiles();
            exporterScanner.ScanDLLFiles();
        }
    }
}
