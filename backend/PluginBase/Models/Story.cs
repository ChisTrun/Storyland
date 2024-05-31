namespace PluginBase.Models
{
    public class Story : Representative
    {
        public string? ImageUrl { get; set; }
        public string? Author { get; set; }
        public Story(string name, string id, string? imageUrl = null, string? author = null) : base(name, id)
        {
            ImageUrl = imageUrl;
            Author = author;
        }

    }
}
