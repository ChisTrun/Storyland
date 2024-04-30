using PluginBase;
using PluginBase.Models;
using HtmlAgilityPack;
using PluginBase.Utils;

namespace TruyenFullPlugin;

public class TruyenFullCommand : IStorySourcePlugin
{
    public string Name =>  "truyenfull.com";
    public string Description => "Plugin de lay data tu trang web truyenfull.com";
    private static readonly string Domain = "https://truyenfull.com/";
    private static readonly string SearchStoryURL = "https://truyenfull.com/tim-kiem/?tukhoa=";
    private static readonly string SearchAuthorURL = "https://truyenfull.com/tac-gia/";

    public HtmlDocument GetWebPageDocument(string url)
    {
        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        var document = web.Load(url);
        return document;
    }

    public IEnumerable<Categories> GetCategories()
    {
        var document = GetWebPageDocument("https://truyenfull.com");

        var liTags = document.DocumentNode.QuerySelectorAll("ul.control.navbar-nav > li")[1];
        var lists = liTags.QuerySelectorAll("a");

        List<Categories> listOfCategories = new List<Categories>();

        foreach (var list in lists)
        {
            var url = list.Attributes["href"].Value;
            var name = list.InnerText;
            listOfCategories.Add(new Categories(name, url));
        }

        return listOfCategories;
    }

    public IEnumerable<StoryInfo> GetStoryInfoOfCategory(string category)
    {
        return GetAllStoriesWithPagination($"{Domain}{category}");
    }


    /// <summary>
    /// get stories from 1 page with HTML Node
    /// </summary>
    /// <param name="document">the HtmlDocument from HtmlAgilityPack</param>
    /// <returns>All stoties</returns>
    public static List<StoryInfo> GetListOfStoriesFromHTMLNode(HtmlDocument document)
    {
        var main = document.DocumentNode.QuerySelectorAll(".container .col-truyen-main .list.list-truyen");

        var rows = main.QuerySelectorAll(".row");

        List<StoryInfo> listOfStories = new List<StoryInfo>();

        foreach (var row in rows)
        {
            var url = HtmlEntity.DeEntitize(row.QuerySelector("a").Attributes["href"].Value);
            var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);

            var img = row.SelectSingleNode(".//div[@data-desk-image]")?.Attributes["data-desk-image"]?.Value;

            listOfStories.Add(new StoryInfo(name, url, img));
        }

        return listOfStories;
    }

    //Scrawling stories
    public  List<StoryInfo> GetAllStoriesWithPagination(string Url)
    {
        var pageDiscoverd = new List<string>
            {
                Url // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue(Url);

        int i = 0;
        int limit = 100;

        List<StoryInfo> listOfStories = new List<StoryInfo>();

        while (pageToScrape.Count > 0 && i < limit)
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
                i++;
            }
            catch (Exception)
            {
            }
        }

        return listOfStories;
    }

    public  List<StoryInfo> GetStoriesFromSearchingName(string searchWord)
    {
        return GetAllStoriesWithPagination($"{SearchStoryURL}{searchWord}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchWord">author name, which needs - between the words</param>
    /// <returns>list of stories</returns>
    public  List<StoryInfo> GetStoriesFromSearchingAuthor(string searchWord)
    {
        searchWord = searchWord.Replace(' ', '-');

        List<StoryInfo> listOfStories = new List<StoryInfo>();
        try
        {
   
            var document = GetWebPageDocument($"{SearchAuthorURL}{searchWord}");
            var searchRes = StringProblem.ConvertVietnameseToNormalizationForm(document.DocumentNode.QuerySelector(".breadcrumb-container h1 a").Attributes["title"].Value).Replace(' ', '-');
            var res = searchRes.Equals(searchWord);

            if (res)
            {
                listOfStories = GetAllStoriesWithPagination($"{SearchAuthorURL}{searchWord}");
            }
        }
        catch (Exception)
        {
        }
        return listOfStories;
    }
}