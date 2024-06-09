using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class ChapterContentMapper
{
    public static ChapterContentDTO ToDTO(this ChapterContent chapterContent)
    {
        return new ChapterContentDTO(chapterContent.Belong.ID, chapterContent.Index, chapterContent.Name, chapterContent.Content);
    }
}
