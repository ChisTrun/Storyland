using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Commands.Abstract;

public interface IExportCommand
{
    public FileBytesDTO CreateFile(StoryDetail storyDetail, IEnumerable<ChapterContent> chapterContents);
}
