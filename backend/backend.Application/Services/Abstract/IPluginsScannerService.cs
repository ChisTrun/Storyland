using Backend.Application.DLLScanner.Contract;
using Backend.Application.DTO;
using Backend.Domain.Contract;

namespace Backend.Application.Services.Abstract;

public interface IPluginsScannerService
{
    List<PluginInfoDTO> GetCrawlerPluginInfos();
    IScanner<ICrawler> GetCrawlerScanner();
    List<PluginInfoDTO> GetExporterPluginInfos();
    IScanner<IExporter> GetExporterScanner();
}
