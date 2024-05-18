namespace PluginBase.Models
{
    public class Server(int index, string name)
    {
        public int Index { get; set; } = index;
        public string Name { get; set; } = name;
    }
}
