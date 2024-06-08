using backend.Application.Objects;
using backend.Application.Plugins.Abstract;
using backend.Application.Plugins.Contracts;

namespace backend.Infrastructure.Concrete;

public class PluginProvider : IPluginProvider
{
    public ICrawler GetCrawlerPlugin(string id)
    {
        throw new NotImplementedException();
    }

    public List<PluginInfo> GetCrawlers()
    {
        throw new NotImplementedException();
    }

    public IExporter GetExporterPlugin(string id)
    {
        throw new NotImplementedException();
    }

    public List<PluginInfo> GetExporters()
    {
        throw new NotImplementedException();
    }

    ICrawler IPluginProvider.GetCrawlerPlugin(string id)
    {
        throw new NotImplementedException();
    }

    IExporter IPluginProvider.GetExporterPlugin(string id)
    {
        throw new NotImplementedException();
    }
}
