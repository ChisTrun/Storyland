using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper;

public static class ChapterMapper
{
    public static ChapterDTO ToDTO(this Chapter chapter)
    {
        return new ChapterDTO(chapter.StoryID, chapter.Index, chapter.Name);
    }
}
