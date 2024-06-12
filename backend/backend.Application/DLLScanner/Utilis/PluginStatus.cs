namespace backend.Application.DLLScanner.Utilis;

public enum PluginStatus
{
    Used,
    Removed
}

public static class PluginStatusExtension
{
    public static string ToCustomString(this PluginStatus status)
    {
        return status switch
        {
            PluginStatus.Used => "used",
            PluginStatus.Removed => "removed",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static PluginStatus ChangeStatus(this PluginStatus status)
    {
        return status switch
        {
            PluginStatus.Used => PluginStatus.Removed,
            PluginStatus.Removed => PluginStatus.Used,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
