namespace PluginBase.Models
{
    public class Author : Representative
    {
        /// <param name="name"></param>
        /// <param name="id"></param>
        public Author(string name = "", string id = "") : base(name, id)
        {
        }
        public Author(Representative representative) : this(representative.Name, representative.Id)
        {
        }
    }
}
