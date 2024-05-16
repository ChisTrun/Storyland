using PluginBase.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;
using PluginBase.Utils;
using System.Web;
using PluginBase.Contract;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace TruyenFullPlugin;

public class TruyenFullCommand : ICrawler
{
    public string Name =>  "truyenfull.com";
    public string Description => "Plugin de lay data tu trang web truyenfull.com";

    private static readonly string _domain = "https://truyenfull.com/";
    private static readonly string _searchStoryURL = "https://truyenfull.com/tim-kiem/?tukhoa=";
    private static readonly string _searchAuthorURL = "https://truyenfull.com/tac-gia/";

    private HtmlDocument GetWebPageDocument(string sourceURL)
    {
        sourceURL = HttpUtility.UrlDecode(sourceURL);

        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        var document = web.Load(sourceURL);
        return document;
    }

    public IEnumerable<Category> GetCategories()
    {
        var document = GetWebPageDocument(_domain);

        var liTags = document.DocumentNode.QuerySelectorAll("ul.control.navbar-nav > li")[1];
        var lists = liTags.QuerySelectorAll("a");

        List<Category> listOfCategories = new List<Category>();

        foreach (var list in lists)
        {
            var url = list.Attributes["href"].Value;
            var name = list.InnerText;
            listOfCategories.Add(new Category(name, url));
        }

        return listOfCategories;
    }

    public IEnumerable<Story> GetStoriesOfCategory(string categoryName)
    {
        return GetAllStoriesWithPagination($"{_domain}categoryName");
    }

    private  List<Story> GetListOfStoriesFromHTMLNode(HtmlDocument document)
    {
        var main = document.DocumentNode.QuerySelectorAll(".container .col-truyen-main .list.list-truyen");

        var rows = main.QuerySelectorAll(".row");

        List<Story> listOfStories = new List<Story>();

        foreach (var row in rows)
        {
            var url = HtmlEntity.DeEntitize(row.QuerySelector("a").Attributes["href"].Value);
            var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);

            var img = row.SelectSingleNode(".//div[@data-desk-image]")?.Attributes["data-desk-image"]?.Value;

            listOfStories.Add(new Story(name, url));
        }

        return listOfStories;
    }

    //Scrawling stories
    private  List<Story> GetAllStoriesWithPagination(string sourceURL)
    {
        var pageDiscoverd = new List<string>
            {
                sourceURL // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue(sourceURL);

        List<Story> listOfStories = new List<Story>();

        while (pageToScrape.Count > 0)
        {
            try
            {
                var currentPage = pageToScrape.Dequeue();
                var currentDocument =  GetWebPageDocument(currentPage);

                var paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll(".pagination li a");
                foreach (var paginationHTMLElement in paginationHTMLElements)
                {
                    var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;
                    if (!pageDiscoverd.Contains(newPaginationLink))
                    {
                        if (!pageToScrape.Contains(newPaginationLink))
                        {
                            pageToScrape.Enqueue(newPaginationLink);
                        }
                        pageDiscoverd.Add(newPaginationLink);
                    }
                }
                listOfStories.AddRange((GetListOfStoriesFromHTMLNode(currentDocument)));  
            }
            catch (Exception)
            {
            }
        }

        return listOfStories;
    }

    public IEnumerable<Story> GetStoriesBySearchName(string searchWord)
    {
        return GetAllStoriesWithPagination($"{_searchStoryURL}{searchWord}");
    }

    public IEnumerable<Story> GetStoriesOfAuthor(string searchWord)
    {
        searchWord = searchWord.Replace(' ', '-');
        searchWord = StringProblem.ConvertVietnameseToNormalizationForm(searchWord);
        List <Story> listOfStories = new List<Story>();
        try
        {
   
            var document = GetWebPageDocument($"{_searchAuthorURL}{searchWord}");
            var searchRes = StringProblem.ConvertVietnameseToNormalizationForm(document.DocumentNode.QuerySelector(".breadcrumb-container h1 a").Attributes["title"].Value).Replace(' ', '-');
            var res = searchRes.Equals(searchWord);

            if (res)
            {
                listOfStories = GetAllStoriesWithPagination($"{_searchAuthorURL}{searchWord}");
            }
        }
        catch (Exception)
        {
        }
        return listOfStories;
    }

    public  List<Chapter> GetChaptersOfStory(string path)
    {
        path = $"{_domain}{path}";
        var pageDiscoverd = new List<string>
        {
           path // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue(path);

        List<Chapter> listOfChapter = new List<Chapter>();

        while (pageToScrape.Count > 0)
        {
            try
            {
                var currentPage = pageToScrape.Dequeue();
                var currentDocument = GetWebPageDocument(currentPage);

                var paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll(".pagination li a");
                foreach (var paginationHTMLElement in paginationHTMLElements)
                {
                    var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;
                    if (!pageDiscoverd.Contains(newPaginationLink))
                    {
                        if (!pageToScrape.Contains(newPaginationLink))
                        {
                            pageToScrape.Enqueue(newPaginationLink);
                        }
                        pageDiscoverd.Add(newPaginationLink);
                    }
                }
                var listsChapters = currentDocument.QuerySelectorAll(".list-chapter");
                foreach (var listChapter in listsChapters)
                {
                    var aTags = listChapter.QuerySelectorAll("a");
                    foreach (var aTag in aTags)
                    {
                        var href = aTag.Attributes["href"].Value;
                        var title = (aTag.Attributes["title"].Value);
                        var name = title.Substring(title.IndexOf('-') + 2);
                        listOfChapter.Add(new Chapter(name, href));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        return listOfChapter;
    }
    public  ChapterContent GetChapterContent(string path, int chapterIndex)
    {
        path = $"{_domain}{path}";
        var document = GetWebPageDocument(path);

        HtmlNode mainContent = document.DocumentNode.QuerySelector(".chapter-c");
        mainContent.SelectNodes("//div[contains(@class, 'ads')]")?.ToList().ForEach(n => n.Remove());
        var text = mainContent.InnerHtml;
        text = text.Replace("<br>", "\n");
        return new ChapterContent(text);
    }

    public StoryDetail GetStoryDetail(string storyName)
    {
        throw new NotImplementedException();
    }
}