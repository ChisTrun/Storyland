using backend.DLLScanner.Concrete;
using System.Collections.Generic;

namespace backend.Handler
{
    public class ServerHandler
    {
 
        public static bool CheckServerIndex(int index)
        {
            return StorySourceScanner.Instance.Commands.ElementAtOrDefault(index) != null;
        }
    }
}
