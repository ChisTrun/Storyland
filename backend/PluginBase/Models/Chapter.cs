namespace PluginBase.Models
{
    public class Chapter : Representative
    {
        public Story Belong { get; }
        public int Index { get; }

        public Chapter(string name, string id, Story belong, int index = -1) : base(name, id)
        {
            Belong = belong;
            Index = index;
        }
    }
}
