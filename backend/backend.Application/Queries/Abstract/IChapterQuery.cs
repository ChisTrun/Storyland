using backend.Application.DTO;
using backend.Application.Plugins;

namespace backend.Application.Queries.Abstract;

public interface IChapterQuery
{
    public List<ChapterContentDTO> GetChapterContents(string storyId);
}
