using PluginBase;
using System.Reflection;

namespace backend.DLLScanner
{
    public class StorySourceScanner
    {
        string exePath;
        string folder;
        FileInfo[] pluginPaths;

        private object commandsLock = new object();
        public List<IStorySourcePlugin> commands { get; private set; }

        private static readonly Lazy<StorySourceScanner> lazy =
        new Lazy<StorySourceScanner>(() => new StorySourceScanner());

        public static StorySourceScanner Instance { get { return lazy.Value; } }

        private StorySourceScanner()
        {
            commands = new List<IStorySourcePlugin>();
            exePath = Assembly.GetExecutingAssembly().Location;
            folder = Path.GetDirectoryName(exePath);
            pluginPaths = new FileInfo[0];
        }

        public void StartScanThread()
        {
            Thread ScanDLL = new Thread(new ThreadStart(Instance.ScanDLLFiles));
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
                        commands.AddRange(newCommands);
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

        static IEnumerable<IStorySourcePlugin> CreateCommands(Assembly assembly)
        {
            int count = 0;

            var t1 = typeof(IStorySourcePlugin).FullName;

            foreach (Type type in assembly.GetTypes())
            {
                if (type.FullName.Contains("TruyenFull"))
                {
                    var s = new string[]
                    {
                        type.FullName,
                        typeof(IStorySourcePlugin).FullName,
                        type.Namespace,
                        type.Name

                    };
                }
                if (typeof(IStorySourcePlugin).IsAssignableFrom(type))
                {
                    IStorySourcePlugin result = Activator.CreateInstance(type) as IStorySourcePlugin;
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
