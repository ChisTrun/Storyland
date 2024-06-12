using backend.Domain.Entities;

namespace backend.Domain.Contract;

public interface IExporter : IPlugin
{
    public string Extension { get; }

    public byte[] ExportStory(StoryDetail story, IEnumerable<ChapterContent> chapteres);
}
