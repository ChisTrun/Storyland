namespace backend.Application.Plugins.DLLScanner.Contract;

public interface IScanner<T> where T : class
{
    public List<T> Commands { get; }
    public void StartScanThread();
}
