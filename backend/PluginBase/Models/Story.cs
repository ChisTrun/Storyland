namespace PluginBase.Models
{
    public class Story : Representative
    {
        private readonly string? _imageUrl;
        public Story(string name, string url, string? imageUrl = null) : base(name, url)
        {
            _imageUrl = imageUrl;
        }
    }
}
