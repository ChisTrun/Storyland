using PluginBase.Contract;
using System.Reflection;

namespace backend.DLLScanner
{
    public class StorySourceScanner
    {
        private readonly string _exePath;
        private readonly string _folder;
        private FileInfo[] _pluginPaths;

        private readonly object _commandsLock = new();
        public List<ICrawler> Commands { get; private set; }
        private bool _isCalled = false;

        public ICrawler? GetCurrentCrawler(int index) => Commands.Count > 0 ? Commands[index] : null;

        private static readonly Lazy<StorySourceScanner> _lazy = new(() => new StorySourceScanner());
        public static StorySourceScanner Instance => _lazy.Value;

        private StorySourceScanner()
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
                string pluginsFolder = "./plugins";
                if (!Directory.Exists(pluginsFolder))
                {
                    Directory.CreateDirectory(pluginsFolder);
                }
                FileInfo[] scanAgain = new DirectoryInfo(_folder).GetFiles("./plugins/*.dll");
                FileInfo[] newPlugins = scanAgain.Where(x => !_pluginPaths.Any(p => p.FullName == x.FullName)).ToArray();
                if (newPlugins.Length != 0)
                {
                    _pluginPaths = scanAgain.ToArray();
                    var newCommands = newPlugins.SelectMany(pluginPath =>
                        {
                            string pluginPathString = pluginPath.FullName;
                            Assembly pluginAssembly = LoadPlugin(pluginPathString);
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

        static Assembly LoadPlugin(string absolutePath)
        {
            //            
            // Navigate up to the solution root
            //#pragma warning disable CS8604 // Possible null reference argument.
            //            string root = Path.GetFullPath(Path.Combine(
            //                Path.GetDirectoryName(
            //                    Path.GetDirectoryName(
            //                        Path.GetDirectoryName(
            //                            Path.GetDirectoryName(
            //                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));
            //#pragma warning restore CS8604 // Possible null reference argument.

            //            string pluginLocation = Path.GetFullPath(Path.Combine(root, absolutePath.Replace('\\', Path.DirectorySeparatorChar)));
            //            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            //            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));

            PluginLoadContext loadContext = new PluginLoadContext(absolutePath);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(absolutePath)));
        }

        static IEnumerable<ICrawler> CreateCommands(Assembly assembly)
        {
            int count = 0;
            var t1 = typeof(ICrawler).FullName;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICrawler).IsAssignableFrom(type))
                {
                    ICrawler result = Activator.CreateInstance(type) as ICrawler;
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
