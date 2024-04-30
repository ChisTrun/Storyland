using PluginBase;
using System.Reflection;
using Test;

string exePath = Assembly.GetExecutingAssembly().Location;
string folder = Path.GetDirectoryName(exePath);
FileInfo[] pluginPaths = new DirectoryInfo(folder).GetFiles("*.dll");

while (true)
{
    FileInfo[] scanAgain = new DirectoryInfo(folder).GetFiles("*.dll");
    FileInfo[] newPlugins = scanAgain.Where(x => !pluginPaths.Contains(x)).ToArray();
    if (newPlugins.Length != 0)
    {
        pluginPaths = scanAgain.ToArray();
    }
    
    IEnumerable<IStorySourcePlugin> commands = (IEnumerable<IStorySourcePlugin>)newPlugins.SelectMany(pluginPath =>
    {
        string pluginPathString = pluginPath.FullName;
        Assembly pluginAssembly = LoadPlugin(pluginPathString);
        return CreateCommands(pluginAssembly);
    }).ToList();

    if (commands.Count()  > 0)
    {
        foreach (var command in commands)
        {
            Console.WriteLine(command.Name + "\n");
        }
    }
    else
    {
        Console.WriteLine("no plugin detected yet \n");
    }

    Thread.Sleep(2000);

    static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root
        string root = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }

    static IEnumerable<IStorySourcePlugin> CreateCommands(Assembly assembly)
    {
        int count = 0;

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