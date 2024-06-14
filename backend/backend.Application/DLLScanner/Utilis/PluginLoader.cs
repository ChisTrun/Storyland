using backend.Domain.Contract;
using System.Reflection;

namespace backend.Application.DLLScanner.Utilis;

internal class PluginLoader
{
    static public Assembly LoadPlugin(string absolutePath)
    {
        var loadContext = new PluginLoadContext(absolutePath);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(absolutePath)));
    }

    public static IEnumerable<T> CreateCommands<T>(Assembly assembly) where T : IPlugin
    {
        var count = 0;
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                if (Activator.CreateInstance(type) is T result)
                {
                    count++;
                    yield return result;
                }
            }
        }
    }
}