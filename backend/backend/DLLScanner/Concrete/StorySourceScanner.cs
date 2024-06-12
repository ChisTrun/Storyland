﻿using backend.DLLScanner.Contract;
using backend.DLLScanner.Utilis;
using NuGet.Packaging;
using PluginBase.Contract;
using System.Reflection;
namespace backend.DLLScanner.Concrete
{
    public class StorySourceScanner : IScanner<ICrawler>
    {
        private readonly string _exePath;
        private readonly string _folder;
        private FileInfo[] _pluginPaths;
        public string PluginsFolder => "./plugins/storySource/";

        private readonly object _commandsLock = new();
        public Dictionary<string, Tuple<ICrawler, PluginStatus>> Commands { get; private set; }
        private bool _isCalled = false;

        public ICrawler? GetCurrentCrawler(int index) => Commands.Count > 0 ? Commands.ElementAt(index).Value.Item1 : null;

        private static readonly Lazy<StorySourceScanner> _lazy = new(() => new StorySourceScanner());
        public static StorySourceScanner Instance => _lazy.Value;


        private StorySourceScanner()
        {
            Commands = [];
            _exePath = Assembly.GetExecutingAssembly().Location;
            _folder = Path.GetDirectoryName(_exePath)!;
            _pluginPaths = [];
        }

        //public void StartScanThread()
        //{
        //    if (_isCalled)
        //        return;
        //    _isCalled = true;
        //    Thread ScanDLL = new Thread(new ThreadStart(Instance.ScanDLLFiles));
        //    ScanDLL.Start();
        //}

        public void ScanDLLFiles()
        {
            //while (true)
            //{
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
                            string pluginPathString = pluginPath.FullName;
                            var pluginAssembly = PluginLoader.LoadPlugin(pluginPathString);
                            return CreateCommands(pluginAssembly);
                        }).ToList();
                    lock (_commandsLock)
                    {
                        newCommands.ForEach(command => Commands.Add(UUID.GenerateUUID(), new Tuple<ICrawler, PluginStatus>(command, PluginStatus.Used)));
                    }
                }
            //    Thread.Sleep(5000);
            //}
        }

        static IEnumerable<ICrawler> CreateCommands(Assembly assembly)
        {
            int count = 0;
            var t1 = typeof(ICrawler).FullName;
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ICrawler).IsAssignableFrom(type))
                {
                    ICrawler? result = Activator.CreateInstance(type) as ICrawler;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }
            if (count == 0)
            {
            }
        }

        public void ChangeStatus(string key)
        {
            var plugin = Commands[key].Item1;
            var status = Commands[key].Item2;

            Commands[key] = new Tuple<ICrawler, PluginStatus>(plugin, status.ChangeStatus());
        }
    }
}
