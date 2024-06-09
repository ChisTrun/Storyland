using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Mapper;

public static class ChapterMapper
{
    public static ChapterDTO ToDTO(this Chapter chapter)
    {
        return new ChapterDTO(chapter.Belong.ID, chapter.Index, chapter.Name);
    }
}
