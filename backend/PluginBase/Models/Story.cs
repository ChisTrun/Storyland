namespace PluginBase.Models
{
    public class Story : Representative
    {
        public string? ImageUrl { get; set; }
        public Story(string name, string url, string? imageUrl = null) : base(name, url)
        {
            ImageUrl = imageUrl;
        }

    }
}
