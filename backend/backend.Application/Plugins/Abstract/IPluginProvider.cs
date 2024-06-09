using backend.Domain.Contract;
using backend.Domain.Mics;

namespace backend.Application.Plugins.Abstract;

public interface IPluginProvider
{
    public ICrawler GetCrawlerPlugin(string id);
    public IExporter GetExporterPlugin(string id);

    public List<PluginInfo> GetExporters();
    public List<PluginInfo> GetCrawlers();
}
