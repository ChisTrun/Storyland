using backend.Application.Commands.Concrete;
using backend.Application.DTO;
using backend.Application.Plugins.Abstract;
using backend.Application.Queries.Concrete;
using backend.Application.Services.Abstract;
using backend.Domain.Mics;

namespace backend.Application.Services.Concrete;

public class ExportService : IExportService
{
    private readonly IPluginProvider _pluginProvider;

    public ExportService(IPluginProvider pluginProvider)
    {
        _pluginProvider = pluginProvider;
    }

    public List<PluginInfo> GetExportFormats()
    {
        return _pluginProvider.GetExporters();
    }

    public FileBytesDTO CreateFile(string serverId, string fileTypeId, string storyId)
    {
        var crawler = _pluginProvider.GetCrawlerPlugin(serverId);
        var exporter = _pluginProvider.GetExporterPlugin(fileTypeId);
        var exportCommand = new ExportCommand(exporter);
        var chapterQuery = new ChapterQuery(crawler);
        var chapterContents = chapterQuery.GetChapterContents(storyId);
        var storyDetails = crawler.GetStoryDetail(storyId);
        var file = exportCommand.CreateFile(storyDetails, chapterContents);
        return file;
    }
}
