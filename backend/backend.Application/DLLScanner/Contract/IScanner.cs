using backend.Application.DLLScanner.Utilis;
using backend.Domain.Contract;

namespace backend.Application.DLLScanner.Contract;

public interface IScanner<T> where T : IPlugin
{
    string PluginsFolder { get; }

    T UsePlugin(string uuid);
    Dictionary<string, PluginInfo<T>> GetUsedPlugins();
    Dictionary<string, PluginInfo<T>> GetAllPlugins();

    void ScanDLLFiles();
    void ChangeStatus(string key);
}