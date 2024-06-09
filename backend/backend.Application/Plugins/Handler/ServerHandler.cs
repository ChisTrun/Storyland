using backend.Application.Plugins.DLLScanner.Concrete;
using System.Collections.Generic;

namespace backend.Handler
{
    public class ServerHandler
    {
        public static bool CheckServerIndex(int index)
        {
            return CrawlerScanner.Instance.Commands.ElementAtOrDefault(index) != null;
        }
    }
}
