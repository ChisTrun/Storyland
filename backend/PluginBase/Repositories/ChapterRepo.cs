using PluginBase.Contract;
using PluginBase.Exceptions;
using PluginBase.Models;

namespace PluginBase.Repositories;

public class ChapterRepo
{
    public static List<ChapterContent> GetAllChapterContents(ICrawler command, string storyId)
    {
        var chapters = command.GetChaptersOfStory(storyId).ToList();
        var chapterContents = new List<ChapterContent>();
        foreach (var chapter in chapters)
        {
            var chapterContent = command.GetChapterContent(storyId, chapter.Index);
            chapterContents.Add(chapterContent);
        }
        return chapterContents;
    }

    // Do not use
    public static List<ChapterContent> AsyncGetAllChapterContents(ICrawler command, string storyId)
    {
        var storyDetail = command.GetStoryDetail(storyId);
        var chapters = command.GetChaptersOfStory(storyId);
        List<Task<ChapterContent>> tasks = new();
        foreach (var chapter in chapters)
        {
            tasks.Add(Task.Run(() =>
            {
                ChapterContent content;
                int count = 0;
                int limit = 10000;
                while (count < limit)
                {
                    try
                    {
                        var _obj = new object();
                        content = command.GetChapterContent(storyId, chapter.Index);
                        content.ChapterName = chapter.Name;
                        content.ChapterID = chapter.Id;
                        content.ChapterIndex = chapter.Index;
                        return content;
                    }
                    catch (CrawlerDocumentException ex)
                    {
                        count++;
                    }
                }
                throw new Exception();
            }));
        }
        var chapterContents = Task.WhenAll(tasks).Result.ToList();
        return chapterContents;
    }
}
