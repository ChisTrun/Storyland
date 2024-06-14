using backend.Application.DLLScanner.Utilis;
using backend.Application.Exceptions;
using backend.Domain.Contract;

namespace backend.Application.DLLScanner.Contract;

public abstract class ScannerBase<T> : IScanner<T> where T : IPlugin
{
    public abstract string PluginsFolder { get; }
    protected abstract Dictionary<string, PluginInfo<T>> Plugins { get; }
    public abstract void ScanDLLFiles();

    public T UsePlugin(string uuid)
    {
        var pluginInfo = Plugins[uuid];
        return pluginInfo == null || pluginInfo.Status == PluginStatus.Removed ? throw new PluginNotFoundException() : pluginInfo.Plugin;
    }

    public Dictionary<string, PluginInfo<T>> GetUsedPlugins() => Plugins.Where(x => x.Value.Status == PluginStatus.Used).ToDictionary();

    public Dictionary<string, PluginInfo<T>> GetAllPlugins() => Plugins;

    public void ChangeStatus(string key)
    {
        var plugin = Plugins[key].Plugin;
        var status = Plugins[key].Status;
        Plugins[key] = new PluginInfo<T>(plugin, status.ChangeStatus());
    }
}
