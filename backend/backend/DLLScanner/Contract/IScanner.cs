using PluginBase.Contract;

namespace backend.DLLScanner.Contract
{
    public interface IScanner<T> where T : class
    {
        public Dictionary<string, T> Commands { get; }
        public void StartScanThread();
        public string PluginsFolder { get; }
    }
}
