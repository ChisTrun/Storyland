using backend.Application.Plugins.DLLScanner.Concrete;

namespace backend.Application.Plugins.Handler;

public class ServerHandler
{
    public static bool CheckServerID(string id)
    {
        return StorySourceScanner.Instance.Commands[id] != null;
    }
    
    public static bool CheckExporterID(string id)
    {
        return ExporterScanner.Instance.Commands[id] != null;
    }
}
