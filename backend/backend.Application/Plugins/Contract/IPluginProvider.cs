using backend.Domain.Contract;
using backend.Domain.Objects;

namespace backend.Application.Plugins.Contract;

public interface IPluginProvider
{
    public ICrawler GetCrawlerPlugin(string id);
    public IExporter GetExporterPlugin(string id);

    public List<PluginInfo> GetExporters();
    public List<PluginInfo> GetCrawlers();
}
