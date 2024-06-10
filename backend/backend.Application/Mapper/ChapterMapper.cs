using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class ChapterMapper
{
    public static ChapterDTO ToDTO(this Chapter chapter)
    {
        return new ChapterDTO(chapter.StoryID, chapter.Index, chapter.Name);
    }
}
