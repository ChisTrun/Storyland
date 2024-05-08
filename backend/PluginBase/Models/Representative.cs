namespace PluginBase.Models
{
    public abstract class Representative
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Representative(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
