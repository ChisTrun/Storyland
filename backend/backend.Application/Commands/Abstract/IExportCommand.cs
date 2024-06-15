using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Commands.Abstract;

public interface IExportCommand
{
    public FileBytesDTO CreateFile(StoryDetail storyDetail, IEnumerable<ChapterContent> chapterContents);
}
