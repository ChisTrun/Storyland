using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Queries.Abstract;

public interface IChapterQuery
{
    public List<ChapterContent> GetChapterContents(string storyId);
}
