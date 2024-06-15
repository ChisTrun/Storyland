using Backend.Application.DLLScanner.Concrete;
using Backend.Application.DLLScanner.Contract;
using Backend.Application.DTO;
using Backend.Application.Services.Abstract;
using Backend.Domain.Contract;

namespace Backend.Application.Services.Concrete;

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
