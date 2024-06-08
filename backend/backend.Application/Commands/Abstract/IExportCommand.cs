using backend.Application.DTO;
using backend.Application.Objects;
using backend.Application.Plugins;

namespace backend.Application.Commands.Abstract;

public interface IExportCommand
{
    public FileBytesDTO CreateFile(StoryDetailDTO storyDetail, IEnumerable<ChapterContentDTO> chapterContents);
}
