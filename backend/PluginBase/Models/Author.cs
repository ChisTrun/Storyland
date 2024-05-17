namespace PluginBase.Models
{
    public class Author : Representative
    {
        public Author(string name, string id) : base(name, id)
        {
        }
        public Author(Representative representative) : this(representative.Name, representative.Id)
        {
        }
    }
}
