using PluginBase.Models;

namespace PluginBase.Contract
{
    public interface IExporter
    {
        public string Ext { get; }
        byte[] ExportStory(StoryDetail storyDetail, List<ChapterContent> chapterContents);
        byte[] ExportChapter(StoryDetail storyDetail, ChapterContent chapterContent);
    }
}
