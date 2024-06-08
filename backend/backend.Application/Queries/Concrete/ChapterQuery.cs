using backend.Application.DTO;
using backend.Application.Plugins.Contracts;
using backend.Application.Queries.Abstract;

namespace backend.Application.Queries.Concrete;

public class ChapterQuery : IChapterQuery
{
    private readonly ICrawler _crawler;

    public ChapterQuery(ICrawler crawler)
    {
        _crawler = crawler;
    }

    public List<ChapterContentDTO> GetChapterContents(string storyId)
    {
        var chapters = _crawler.GetChaptersOfStory(storyId);
        var chapterContents = new List<ChapterContentDTO>();
        foreach (var chapter in chapters)
        {
            var chapterContent = _crawler.GetChapterContent(chapter.Name, chapter.Index);
            chapterContents.Add(chapterContent);
        }
        return chapterContents;
    }
}
