namespace PluginBase.Models
{
    public class Story : Representative
    {
        public string? ImageUrl { get; set; }
        public Story(string name, string id, string? imageUrl = null) : base(name, id)
        {
            ImageUrl = imageUrl;
        }

    }
}
