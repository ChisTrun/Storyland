using Backend.Application.DLLScanner.Contract;
using Backend.Application.DLLScanner.Utilis;
using Backend.Domain.Contract;
using System.Reflection;

namespace Backend.Application.DLLScanner.Concrete;

internal class CrawlerScanner : ScannerBase<ICrawler>
{
    private readonly string _exePath;
    private readonly string _folder;
    private readonly object _commandsLock = new();

    private FileInfo[] _pluginPaths;
    private readonly Dictionary<string, PluginInfo<ICrawler>> _plugins;
    public override string PluginsFolder => "./plugins/storySource/";

    private static readonly Lazy<CrawlerScanner> _lazy = new(() => new CrawlerScanner());
    public static CrawlerScanner Instance => _lazy.Value;

    private CrawlerScanner()
    {
        _plugins = [];
        _exePath = Assembly.GetExecutingAssembly().Location;
        _folder = Path.GetDirectoryName(_exePath)!;
        _pluginPaths = [];
    }

    protected override Dictionary<string, PluginInfo<ICrawler>> Plugins => _plugins;

    public override void ScanDLLFiles()
    {
        if (!Directory.Exists(PluginsFolder))
        {
            Directory.CreateDirectory(PluginsFolder);
        }
        var scanAgain = new DirectoryInfo(_folder).GetFiles($"{PluginsFolder}*.dll");
        var newPlugins = scanAgain.Where(x => !_pluginPaths.Any(p => p.FullName == x.FullName)).ToArray();
        if (newPlugins.Length != 0)
        {
            _pluginPaths = scanAgain.ToArray();
            var newCommands = newPlugins.SelectMany(pluginPath =>
                {
                    var pluginPathString = pluginPath.FullName;
                    var pluginAssembly = PluginLoader.LoadPlugin(pluginPathString);
                    return PluginLoader.CreateCommands<ICrawler>(pluginAssembly);
                }).ToList();
            lock (_commandsLock)
            {
                newCommands.ForEach(command => _plugins.Add(UUID.GenerateUUID(), new PluginInfo<ICrawler>(command, PluginStatus.Used)));
            }
        }
    }
}
