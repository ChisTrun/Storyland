namespace backend.Application.Exceptions;

public class PluginNotFoundException : Exception
{
    public PluginNotFoundException() : base("Plugin not found")
    {
    }

    public PluginNotFoundException(string? message) : base(message)
    {
    }

    public PluginNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
