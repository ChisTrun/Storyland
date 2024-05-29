using PluginBase.Models;

namespace PluginBase.Contract;

public interface IExport
{
    public byte[] ExportStory(StoryDetail story, List<ChapterContent> chapters);
    public byte[] ExportChapter(StoryDetail story, ChapterContent chapters);
}
