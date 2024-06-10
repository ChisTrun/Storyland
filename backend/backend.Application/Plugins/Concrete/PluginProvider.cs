using backend.Application.Plugins.Contract;
using backend.Application.Plugins.DLLScanner.Concrete;
using backend.Application.Plugins.Handler;
using backend.Domain.Contract;
using backend.Domain.Objects;

namespace backend.Application.Plugins.Concrete;

public class PluginProvider : IPluginProvider
{
    public ICrawler GetCrawlerPlugin(string id)
    {
        if (ServerHandler.CheckServerID(id) == false)
        {
            throw new InvalidOperationException("Invalid Plugin ID");
        }
        return StorySourceScanner.Instance.Commands[id];
    }

    public List<PluginInfo> GetCrawlers()
    {
        return StorySourceScanner.Instance.Commands.Select(x => new PluginInfo(x.Key, x.Value.Name)).ToList();
    }

    public IExporter GetExporterPlugin(string id)
    {
        if (ServerHandler.CheckExporterID(id) == false)
        {
            throw new InvalidOperationException("Invalid Plugin ID");
        }
        return ExporterScanner.Instance.Commands[id];
    }

    public List<PluginInfo> GetExporters()
    {
        return ExporterScanner.Instance.Commands.Select(x => new PluginInfo(x.Key, x.Value.Name)).ToList();
    }
}
