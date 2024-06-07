using PluginBase.Contract;
using PluginBase.Models;

namespace backend.Model
{
    public class CrawlerModel
    {
        public static List<ChapterContent> GetAllChapterContents(ICrawler command, string storyId)
        {
            var chapters = command.GetChaptersOfStory(storyId).ToList();
            var chapterContents = new List<ChapterContent>();
            foreach (var chapter in chapters)
            {
                var chapterContent = command.GetChapterContent(storyId, chapter.Index + 1);
                chapterContents.Add(chapterContent);
            }
            return chapterContents;
        }

        public static List<ChapterContent> AsyncGetAllChapterContents(ICrawler command, string storyId)
        {
            var storyDetail = command.GetStoryDetail(storyId);
            var chapters = command.GetChaptersOfStory(storyId);
            List<Task<ChapterContent>> tasks = new();
            foreach (var chapter in chapters)
            {
                tasks.Add(Task.Run(() =>
                {
                    var content = command.GetChapterContent(storyId, chapter.Index);
                    content.ChapterName = chapter.Name;
                    content.ChapterID = chapter.Id;
                    content.ChapterIndex = chapter.Index;
                    return content;
                }));
            }
            var chapterContents = Task.WhenAll(tasks).Result.ToList();
            return chapterContents;
        }
    }
}
