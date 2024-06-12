using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Domain.Contract;

namespace backend.Application.Services.Abstract;

public interface IPluginsScannerService
{
    List<PluginInfoDTO> GetCrawlerPluginInfos();
    IScanner<ICrawler> GetCrawlerScanner();
    List<PluginInfoDTO> GetExporterPluginInfos();
    IScanner<IExporter> GetExporterScanner();
}
