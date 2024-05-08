using HtmlAgilityPack;
using PluginBase.Models;
using System.Web;

namespace PluginBase.Contract
{
    public abstract class HtmlCrawlerBase : ICrawler
    {


        public virtual string Name => "";
        public virtual string Description => "";

        public abstract IEnumerable<Category> GetCategories();
        public abstract IEnumerable<Story> GetStoriesOfCategory(string categoryName);
        public abstract IEnumerable<Story> GetStoriesBySearchName(string searchWord);
        public abstract IEnumerable<Story> GetStoriesOfAuthor(string searchWord);
        public abstract List<Chapter> GetChaptersOfStory(string sourceURL);
        public abstract ChapterContent GetChapterContent(string storyName, int chapterIndex);
    }
}
