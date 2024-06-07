namespace PluginBase.Models
{
    public class ExportType(string id, string name)
    {
        public string ID { get; set; } = id ;
        public string Name { get; set; } = name;
    }
}
