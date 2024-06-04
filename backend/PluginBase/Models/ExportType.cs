namespace PluginBase.Models
{
    public class ExportType(int index, string name)
    {
        public int Index { get; set; } = index;
        public string Name { get; set; } = name;
    }
}
