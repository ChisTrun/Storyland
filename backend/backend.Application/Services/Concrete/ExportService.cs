using backend.Application.Commands.Concrete;
using backend.Application.DLLScanner.Contract;
using backend.Application.DTO;
using backend.Application.Queries.Concrete;
using backend.Application.Services.Abstract;
using backend.Domain.Contract;

namespace backend.Application.Services.Concrete;

public class ExportService : IExportService
{
    private readonly IPluginsScannerService _pluginsScannerService;

    public ExportService(IPluginsScannerService pluginsScannerService)
    {
        _pluginsScannerService = pluginsScannerService;
    }

    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId)
    {
        var crawler = _pluginsScannerService.GetCrawlerScanner().UsePlugin(serverId);
        var exporter = _pluginsScannerService.GetExporterScanner().UsePlugin(fileTypeId);
        var exportCommand = new ExportCommand(exporter);
        var chapterQuery = new ChapterQuery(crawler);
        var chapterContents = chapterQuery.GetChapterContents(storyId);
        var storyDetails = crawler.GetStoryDetail(storyId);
        var file = exportCommand.CreateFile(storyDetails, chapterContents);
        return file;
    }
}
