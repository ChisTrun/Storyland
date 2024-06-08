using backend.Application.DTO;

namespace backend.Application.Plugins.Contracts;

public interface IExporter : IPlugin
{
    public string Extension { get; }
    public byte[] ExportStory(StoryDetailDTO storyDetail, IEnumerable<ChapterContentDTO> chapterContents);
}
