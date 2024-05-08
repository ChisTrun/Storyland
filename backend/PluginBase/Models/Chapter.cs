namespace PluginBase.Models
{
    public class Chapter : Representative
    {
        public Chapter(string name, string url, int index = -1) : base(name, url)
        {
            Index = index;
        }

        public int Index { get; }
    }
}
