namespace backend.Application.Objects;

public class PluginInfo(string iD, string name)
{
    public string ID { get; } = iD;
    public string Name { get; } = name;
}
