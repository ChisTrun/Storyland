using backend.Application.Plugins.DLLScanner.Concrete;
using backend.Application.Plugins.DLLScanner.Contract;
using backend.Domain.Contract;

namespace backend.Application.Plugins.DLLScanner;

public class ScannerController
{
    public IScanner<ICrawler> crawlerScanner;
    public IScanner<IExporter> exporterScanner;
    private static readonly Lazy<ScannerController> _lazy = new(() => new ScannerController());
    public static ScannerController Instance => _lazy.Value;

    private ScannerController()
    {
        crawlerScanner = StorySourceScanner.Instance;
        exporterScanner = ExporterScanner.Instance;

        StartToScan();
    }

    public void StartToScan()
    {
        crawlerScanner.StartScanThread();
        exporterScanner.StartScanThread();
    }
}
