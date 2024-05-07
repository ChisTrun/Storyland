namespace PluginBase.Models
{
    public class Author
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Author(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
