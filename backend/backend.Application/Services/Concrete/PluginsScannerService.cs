using backend.Application.DLLScanner.Concrete;
using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Application.Services.Abstract;
using backend.Domain.Contract;

namespace backend.Application.Services.Concrete;

public class PluginsScannerService : IPluginsScannerService
{
    private readonly IScanner<ICrawler> _crawlerScanner;
    private readonly IScanner<IExporter> _exporterScanner;

    public PluginsScannerService()
    {
        _crawlerScanner = CrawlerScanner.Instance;
        _exporterScanner = ExporterScanner.Instance;
        _crawlerScanner.ScanDLLFiles();
        _exporterScanner.ScanDLLFiles();
    }

    public IScanner<ICrawler> GetCrawlerScanner() => _crawlerScanner;

    public IScanner<IExporter> GetExporterScanner() => _exporterScanner;

    public List<PluginInfoDTO> GetCrawlerPluginInfos() => _crawlerScanner.GetUsedPlugins().Select(x => new PluginInfoDTO(x.Key, x.Value.Plugin.Name)).ToList();

    public List<PluginInfoDTO> GetExporterPluginInfos() => _exporterScanner.GetUsedPlugins().Select(x => new PluginInfoDTO(x.Key, x.Value.Plugin.Name)).ToList();
}
