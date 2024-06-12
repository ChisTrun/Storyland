namespace backend.DLLScanner.Contract
{
    public interface IScanner<T> where T : class
    {
        public Dictionary<string, Tuple<T,Utilis.PluginStatus>> Commands { get; }
        public void ScanDLLFiles();
        public string PluginsFolder { get; }
        public void ChangeStatus(string key);
    }
}
