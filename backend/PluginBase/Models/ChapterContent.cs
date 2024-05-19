namespace PluginBase.Models
{
    public  class ChapterContent
    {
        public string Content { get; set; }
        public string? NextChapID { get; set; }
        public string? PrevChapID { get; set; }

        public ChapterContent(string content, string? next = null, string? pre = null)
        {
            Content = content;
            NextChapID = next;
            PrevChapID = pre;
        }
    }
}
