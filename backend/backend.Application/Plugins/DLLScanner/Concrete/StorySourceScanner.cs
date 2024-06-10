﻿using backend.Application.Plugins.DLLScanner.Contract;
using backend.Application.Plugins.DLLScanner.Utilis;
using backend.Domain.Contract;
using System.Reflection;

namespace backend.Application.Plugins.DLLScanner.Concrete;

public class StorySourceScanner : IScanner<ICrawler>
{
    private readonly string _exePath;
    private readonly string _folder;
    private FileInfo[] _pluginPaths;
    private string _pluginsFolder = "./plugins/storySource/";

    private readonly object _commandsLock = new();
    public Dictionary<string, ICrawler> Commands { get; private set; }
    private bool _isCalled = false;

    public ICrawler? GetCurrentCrawler(int index) => Commands.Count > 0 ? Commands.ElementAt(index).Value : null;

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
                        var pluginAssembly = PluginLoader.LoadPlugin(pluginPathString);
                        return CreateCommands(pluginAssembly);
                    }).ToList();
                lock (_commandsLock)
                {
                    newCommands.ForEach(command => Commands.Add(UUID.GenerateUUID(), command));
                }
            }
            Thread.Sleep(5000);
        }
    }

    static IEnumerable<ICrawler> CreateCommands(Assembly assembly)
    {
        int count = 0;
        var t1 = typeof(ICrawler).FullName;
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(ICrawler).IsAssignableFrom(type))
            {
                if (Activator.CreateInstance(type) is ICrawler result)
                {
                    count++;
                    yield return result;
                }
            }
        }
    }
}
