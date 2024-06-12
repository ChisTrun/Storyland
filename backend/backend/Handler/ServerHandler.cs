using backend.DLLScanner;
using backend.DLLScanner.Concrete;
using System.Collections.Generic;
using System.Diagnostics;

namespace backend.Handler
{
    public class ServerHandler
    {
 
        public static bool CheckServerID(string id)
        {
            var plugin = ScannerController.Instance.sourceScanner.Commands[id];
            return plugin != null && plugin.Item2 != DLLScanner.Utilis.PluginStatus.Removed;
        }
    }
}
