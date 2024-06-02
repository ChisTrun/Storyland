using backend.DLLScanner.Contract;
using backend.DLLScanner.Utilis;
using PluginBase.Contract;
using System.Reflection;

namespace backend.DLLScanner.Concrete
{
    public class ExporterScanner : PluginLoader, IScanner<IExporter>
    {

        private readonly string _exePath;
        private readonly string _folder;
        private FileInfo[] _pluginPaths;
        private string _pluginsFolder = "./plugins/exporter/";

        private readonly object _commandsLock = new();
        public List<IExporter> Commands { get; private set; }
        private bool _isCalled = false;

        public IExporter? GetCurrentCrawler(int index) => Commands.Count > 0 ? Commands[index] : null;

        private static readonly Lazy<ExporterScanner> _lazy = new(() => new ExporterScanner());
        public static ExporterScanner Instance => _lazy.Value;

        private ExporterScanner()
        {
            Commands = [];
            _exePath = Assembly.GetExecutingAssembly().Location;
            _folder = Path.GetDirectoryName(_exePath)!;
            _pluginPaths = [];
        }

        public void StartScanThread()
        {
            if (_isCalled)
                return;
            _isCalled = true;
            Thread ScanDLL = new Thread(new ThreadStart(Instance.ScanDLLFiles));
            ScanDLL.Start();
        }

        public void ScanDLLFiles()
        {
            while (true)
            {
                if (!Directory.Exists(_pluginsFolder))
                {
                    Directory.CreateDirectory(_pluginsFolder);
                }
                var scanAgain = new DirectoryInfo(_folder).GetFiles($"{_pluginsFolder}*.dll");
                var newPlugins = scanAgain.Where(x => !_pluginPaths.Any(p => p.FullName == x.FullName)).ToArray();
                if (newPlugins.Length != 0)
                {
                    _pluginPaths = scanAgain.ToArray();
                    var newCommands = newPlugins.SelectMany(pluginPath =>
                    {
                        string pluginPathString = pluginPath.FullName;
                        var pluginAssembly = LoadPlugin(pluginPathString);
                        return CreateCommands(pluginAssembly);
                    }).ToList();
                    lock (_commandsLock)
                    {
                        Commands.AddRange(newCommands);
                    }
                }
                Thread.Sleep(5000);
            }
        }

        static IEnumerable<IExporter> CreateCommands(Assembly assembly)
        {
            int count = 0;
            var t1 = typeof(IExporter).FullName;
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IExporter).IsAssignableFrom(type))
                {
                    IExporter? result = Activator.CreateInstance(type) as IExporter;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }
        }

    }
}
