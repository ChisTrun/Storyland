using System.Reflection;

namespace backend.DLLScanner.Utilis
{
    public class PluginLoader
    {
        static public Assembly LoadPlugin(string absolutePath)
        {
            PluginLoadContext loadContext = new PluginLoadContext(absolutePath);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(absolutePath)));
        }
    }
}