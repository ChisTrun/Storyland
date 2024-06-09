using backend.Application.Queries.Abstract;
using backend.Domain.Entities;

namespace backend.Application.Queries.Concrete;

public class ChapterQueryAsync : IChapterQuery
{
    public List<ChapterContent> GetChapterContents(string storyId)
    {
        throw new NotImplementedException();
    }
}
