using backend.Domain.Contract;

namespace backend.Application.DLLScanner.Utilis;

public class PluginInfo<T>(T plugin, PluginStatus status) where T : IPlugin
{
    public T Plugin { get; } = plugin;
    public PluginStatus Status { get; } = status;
}
