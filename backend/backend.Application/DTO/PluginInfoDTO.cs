namespace Backend.Application.DTO;

public class PluginInfoDTO(string iD, string name)
{
    public string ID { get; } = iD;
    public string Name { get; } = name;
}
