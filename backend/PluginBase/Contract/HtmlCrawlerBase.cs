using HtmlAgilityPack;
using PluginBase.Models;
using System.Web;

namespace PluginBase.Contract
{
    public abstract class HtmlCrawlerBase : ICrawler
    {
        protected static HtmlDocument GetWebPageDocument(string sourceURL)
        {
            sourceURL = HttpUtility.UrlDecode(sourceURL);

            var web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
            var document = web.Load(sourceURL);
            return document;
        }

        public virtual string Name => "";
        public virtual string Description => "";

        public abstract IEnumerable<Category> GetCategories();
        public abstract ChapterContent GetChapterContent(string sourceURL);
        public abstract List<Chapter> GetChaptersOfStory(string sourceURL);
        public abstract IEnumerable<Story> GetStoriesOfAuthor(string searchWord);
        public abstract IEnumerable<Story> GetStoriesBySearchName(string searchWord);
        public abstract IEnumerable<Story> GetStoriesOfCategory(string categoryName);
    }
}
