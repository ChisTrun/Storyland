using PluginBase.Contract;

namespace backend.DLLScanner.Contract
{
    public interface IScanner<T> where T : class
    {
        public List<T> Commands { get; }
        public void StartScanThread();
    }
}
