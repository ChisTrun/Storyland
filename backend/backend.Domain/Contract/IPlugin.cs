using backend.Domain.Mics;

namespace backend.Domain.Contract;

public interface IPlugin
{
    public PluginInfo Info { get; }
}
