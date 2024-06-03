namespace PluginBase.Models
{
    public class ChapterContent
    {
        public string Content { get; set; }
        public string? NextChapID { get; set; }
        public string? PrevChapID { get; set; }
        public string ChapterName { get; set; }
        public string ChapterID { get; set; }
        public int? ChapterIndex { get; set; }
        public Story? Belong { get; set; }

        public ChapterContent(string content, string? next, string? pre, string name, string chapterID, int? chapterIndex = null, Story? belong = null)
        {
            Content = content;
            NextChapID = next;
            PrevChapID = pre;
            ChapterName = name;
            ChapterID = chapterID;
            ChapterIndex = chapterIndex;
            Belong = belong;
        }
    }
}
