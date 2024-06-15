using Backend.Application.DLLScanner.Contract;
using Backend.Application.DLLScanner.Utilis;
using Backend.Domain.Contract;
using System.Reflection;

namespace Backend.Application.DLLScanner.Concrete;

internal class ExporterScanner : ScannerBase<IExporter>
{
    private readonly string _exePath;
    private readonly string _folder;
    private readonly object _commandsLock = new();

    private FileInfo[] _pluginPaths;
    private readonly Dictionary<string, PluginInfo<IExporter>> _plugins;
    public override string PluginsFolder => "./plugins/exporter/";

    private static readonly Lazy<ExporterScanner> _lazy = new(() => new ExporterScanner());
    public static ExporterScanner Instance => _lazy.Value;

    private ExporterScanner()
    {
        _plugins = [];
        _exePath = Assembly.GetExecutingAssembly().Location;
        _folder = Path.GetDirectoryName(_exePath)!;
        _pluginPaths = [];
    }

    protected override Dictionary<string, PluginInfo<IExporter>> Plugins => _plugins;

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
                return PluginLoader.CreateCommands<IExporter>(pluginAssembly);
            }).ToList();
            lock (_commandsLock)
            {
                newCommands.ForEach(command => Plugins.Add(UUID.GenerateUUID(), new PluginInfo<IExporter>(command, PluginStatus.Used)));
            }
        }
    }
}
