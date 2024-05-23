namespace PluginBase.Models
{
    public class Chapter : Representative
    {
        public Story Belong { get; }
        public int Index { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="storyid">Is ID of story</param>
        /// <param name="belong"></param>
        /// <param name="index"></param>
        public Chapter(string name, string storyid, Story belong, int index = -1) : base(name, storyid)
        {
            Belong = belong;
            Index = index;
        }
    }
}
