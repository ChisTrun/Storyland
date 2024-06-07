using backend.DLLScanner.Concrete;
using System.Collections.Generic;
using System.Diagnostics;

namespace backend.Handler
{
    public class ServerHandler
    {
 
        public static bool CheckServerID(string id)
        {
            return StorySourceScanner.Instance.Commands[id] != null;
        }
    }
}
