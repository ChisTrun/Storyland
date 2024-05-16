namespace PluginBase.Models
{
    public class Author : Representative
    {
        public Author(string name, string url) : base(name, url)
        {
        }
        public Author(Representative representative) : this(representative.Name, representative.Url)
        {
        }
    }
}
