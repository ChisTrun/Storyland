using backend.Domain.Contract;
using backend.Domain.Entities;

namespace backend.Application.Queries;

public class ChapterQuery
{
    private readonly ICrawler _crawler;

    public ChapterQuery(ICrawler crawler)
    {
        _crawler = crawler;
    }

    public List<ChapterContent> GetChapterContents(string storyId)
    {
        var chapters = _crawler.GetChaptersOfStory(storyId);
        var chapterContents = new List<ChapterContent>();
        foreach (var chapter in chapters)
        {
            var chapterContent = _crawler.GetChapterContent(chapter.StoryID, chapter.Index);
            chapterContents.Add(chapterContent);
        }
        return chapterContents;
    }
}
