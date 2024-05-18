namespace PluginBase.Models
{
    public class Representative
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public Representative(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
