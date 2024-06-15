using Backend.Application.DTO;
using Backend.Domain.Entities;

namespace Backend.Application.Mapper;

public static class ChapterContentMapper
{
    public static ChapterContentDTO ToDTO(this ChapterContent chapterContent)
    {
        return new ChapterContentDTO(chapterContent.StoryID, chapterContent.Index, chapterContent.Name, chapterContent.Content);
    }
}
