using backend.Application.DTO;
using backend.Application.Queries.Abstract;

namespace backend.Application.Queries.Concrete;

public class ChapterQueryAsync : IChapterQuery
{
    public List<ChapterContentDTO> GetChapterContents(string storyId)
    {
        throw new NotImplementedException();
    }
}
