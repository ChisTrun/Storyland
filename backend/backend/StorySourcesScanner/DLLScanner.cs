using PluginBase;
using System.Reflection;

namespace backend.StorySourcesScanner
{
    public class DLLScanner
    {
        string exePath;
        string folder;
        FileInfo[] pluginPaths;

        private object commandsLock = new object();
        public List<Icommand> commands { get; private set; }

        private static readonly Lazy<DLLScanner> lazy =
        new Lazy<DLLScanner>(() => new DLLScanner());

        public static DLLScanner Instance { get { return lazy.Value; } }

        private DLLScanner()
        {
            commands = new List<Icommand>();
            exePath = Assembly.GetExecutingAssembly().Location;
            folder = Path.GetDirectoryName(exePath);
            pluginPaths = new FileInfo[0];
        }

        public void startScanThread()
        {
            Thread ScanDLL = new Thread(new ThreadStart(DLLScanner.Instance.ScanDLLFiles));
            ScanDLL.Start();
        }

        public void ScanDLLFiles()
        {
            while (true)
            {

                FileInfo[] scanAgain = new DirectoryInfo(folder).GetFiles("./plugins/*.dll");
                FileInfo[] newPlugins = scanAgain.Where(x => !pluginPaths.Any(p => p.FullName == x.FullName)).ToArray();

                if (newPlugins.Length != 0)
                {
                    pluginPaths = scanAgain.ToArray();

                    var newCommands = newPlugins.SelectMany(pluginPath =>
                        {
                            string pluginPathString = pluginPath.FullName;
                            Assembly pluginAssembly = LoadPlugin(pluginPathString);
                            return CreateCommands(pluginAssembly);
                        }).ToList();

                    lock (commandsLock)
                    {
                        ((List<Icommand>)commands).AddRange(newCommands);
                    }
                }

                Thread.Sleep(5000);
            }
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
#pragma warning disable CS8604 // Possible null reference argument.
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));
#pragma warning restore CS8604 // Possible null reference argument.

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<Icommand> CreateCommands(Assembly assembly)
        {
            int count = 0;

            var t1 = typeof(Icommand).FullName;

            foreach (Type type in assembly.GetTypes())
            {
                if (type.FullName.Contains("TruyenFull"))
                {
                    var s = new string[]
                    {
                        type.FullName,
                        typeof(Icommand).FullName,
                        type.Namespace,
                        type.Name

                    };
                }
                if (typeof(Icommand).IsAssignableFrom(type))
                {
                    Icommand result = Activator.CreateInstance(type) as Icommand;
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
