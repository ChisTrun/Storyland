using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class ChapterContentMapper
{
    public static ChapterContentDTO ToDTO(this ChapterContent chapterContent)
    {
        return new ChapterContentDTO(chapterContent.StoryID, chapterContent.Index, chapterContent.Name, chapterContent.Content);
    }
}
