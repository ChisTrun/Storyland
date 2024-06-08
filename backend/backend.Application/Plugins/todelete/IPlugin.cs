using backend.Application.Objects;

namespace backend.Application.Plugins.Contracts;

public interface IPlugin
{
    public PluginInfo Info { get; }
}
