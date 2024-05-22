using PluginBase.Models;
using HtmlAgilityPack;
using PluginBase.Utils;
using System.Web;
using PluginBase.Contract;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace TruyenFullPlugin;

public class TruyenFullCommand : ICrawler
{
    public string Name => "Truyện Full";
    public string Description => "Plugin de lay data tu trang web truyenfull.com";

    public static string Domain => "https://truyenfull.com/";
    //public static string DomainTongHop => $"{Domain}/tong-hop";
    public static string DomainTimKiem => "https://truyenfull.com/tim-kiem/?tukhoa=";
    //public static string DomainKetQuaTimKiem => $"{Domain}/ket-qua-tim-kiem";
    public static string DomainDocTruyen => $"{Domain}/doc-truyen";
    public static string DomainTacGia => $"{Domain}/tac-gia";

    //private static readonly string Domain = "https://truyenfull.com/";
    //private static readonly string DomainTimKiem = "https://truyenfull.com/tim-kiem/?tukhoa=";
    //private static readonly string DomainTacGia = "https://truyenfull.com/tac-gia/";

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
        var document = GetWebPageDocument(Domain);

        var liTags = document.DocumentNode.QuerySelectorAll("ul.control.navbar-nav > li")[1];
        var lists = liTags.QuerySelectorAll("a");

        List<Category> listOfCategories = new List<Category>();

        foreach (var list in lists)
        {
            var url = list.Attributes["href"].Value;
            var name = list.InnerText;
            listOfCategories.Add(new Category(name, ModelExtension.GetIDFromUrl(ModelType.Category, url)));
        }

        return listOfCategories;
    }

    public IEnumerable<Story> GetStoriesOfCategory(string categoryId)
    {
        return GetAllStoriesWithPagination(ModelExtension.GetUrlFromID(ModelType.Category, categoryId));
    }

    PagingRepresentative<Story> ICrawler.GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        return GetStoryWithPageAndLimit(ModelType.Category, ModelExtension.GetUrlFromID(ModelType.Category, categoryId), page, limit);
    }

    private PagingRepresentative<Story> GetStoryWithPageAndLimit(ModelType modelType, string firstURL, int page, int limit)
    {
        int systemLimit;
        int totalPage;
        int totalRecord;
        {
            var documentFirstPage = GetWebPageDocument(firstURL);
            systemLimit = documentFirstPage.QuerySelectorAll("#list-page  .list.list-truyen  .row .col-list-image").Count;
            if (systemLimit == 0)
            {
#pragma warning disable IDE0028 // Simplify collection initialization
                return new PagingRepresentative<Story>(page, limit, 0, new List<Story>());
#pragma warning restore IDE0028 // Simplify collection initialization
            }
            var lastLiPagingTag = documentFirstPage.QuerySelector(".pagination-container  ul  li:last-child");
            if (lastLiPagingTag == null)
            {
                totalRecord = systemLimit;
            }
            else
            {
                var aTag = lastLiPagingTag.PreviousSiblingElement().GetChildElements().First();
                var lastURL = aTag.Attributes["href"].Value;

                var systemTotalPage = int.Parse((new Regex(@"[^\d]")).Replace(lastURL, ""));

                var documentLastPage = GetWebPageDocument(lastURL);
                var systemLastPageCount = documentLastPage.QuerySelectorAll("#list-page  .list.list-truyen  .row .col-list-image").Count;
                totalRecord = systemLimit * (systemTotalPage - 1) + systemLastPageCount;
            }
            totalPage = (totalRecord / limit) + (totalRecord % limit == 0 ? 0 : 1);
        }
        var pageToGet = CalculatePageAndLimit(systemLimit, page, limit);
        var systemPageStart = pageToGet.Item1;
        var systemOffsetStart = pageToGet.Item2;

        return new PagingRepresentative<Story>(page, limit, totalPage, GetAllStoriesWithOffsetAndLimit($"{firstURL}/{(systemPageStart > 1 ? ModelExtension.PagingType(modelType) + systemPageStart : "")}", systemOffsetStart, limit));
    }

    private List<Story> GetListOfStoriesFromHTMLNode(HtmlDocument document, ref int? needRemain, int offset)
    {
        var main = document.DocumentNode.QuerySelectorAll(".container .col-truyen-main .list.list-truyen");

        var rows = main.QuerySelectorAll(".row");

        List<Story> listOfStories = new List<Story>();

        foreach (var row in rows)
        {
            if (needRemain <= 0)
            {
                break;
            }
            if (offset >0 )
            {
                offset--;
                continue;
            }
            var url = HtmlEntity.DeEntitize(row.QuerySelector("a").Attributes["href"].Value);
            var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);
            var img = row.SelectSingleNode(".//div[@data-desk-image]")?.Attributes["data-desk-image"]?.Value;

            listOfStories.Add(new Story(name, url ,img));
            needRemain--;
        }

        return listOfStories;
    }

    //Scrawling stories
    private List<Story> GetAllStoriesWithPagination(string sourceURL)
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
                int? tmp = int.MaxValue;
                listOfStories.AddRange((GetListOfStoriesFromHTMLNode(currentDocument, ref tmp , 0)));
            }
            catch (Exception)
            {
            }
        }

        return listOfStories;
    }

    private List<Story> GetAllStoriesWithOffsetAndLimit(string sourceURL, int firstOffset, int limit)
    {
        var pageDiscoverd = new List<string>
            {
                sourceURL // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue(sourceURL);

        List<Story> listOfStories = new List<Story>();

        int? needRemain = limit;
        while (pageToScrape.Count > 0 && needRemain > 0)
        {
            try
            {
                var currentPage = pageToScrape.Dequeue();
                var currentDocument = GetWebPageDocument(currentPage);

                listOfStories.AddRange((GetListOfStoriesFromHTMLNode(currentDocument, ref needRemain, currentPage.Equals(sourceURL) ? firstOffset : 0)));


                var curpaginationHTMLElement = currentDocument.DocumentNode.QuerySelector(".pagination li.active");
                var nextpaginationElement = curpaginationHTMLElement.NextSiblingElement().QuerySelector("a");
                var newPaginationLink = nextpaginationElement.Attributes["href"].Value;
                if (!pageDiscoverd.Contains(newPaginationLink))
                {
                    if (!pageToScrape.Contains(newPaginationLink))
                    {
                        pageToScrape.Enqueue(newPaginationLink);
                    }
                    pageDiscoverd.Add(newPaginationLink);
                }
            }
            catch (Exception)
            {
            }
        }

        return listOfStories;
    }

    public IEnumerable<Story> GetStoriesBySearchName(string storyName)
    {
        return GetAllStoriesWithPagination($"{DomainTimKiem}{WebUtility.UrlEncode(storyName)}");
    }

    PagingRepresentative<Story> ICrawler.GetStoriesBySearchName(string storyName, int page, int limit)
    {
        return GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTimKiem}{WebUtility.UrlEncode(storyName)}", page, limit);
    }

    public IEnumerable<Story> GetStoriesOfAuthor(string authorId)
    {
        authorId = WebUtility.UrlEncode(authorId);
        List<Story> listOfStories = new List<Story>();
        try
        {

            var document = GetWebPageDocument($"{DomainTacGia}/{authorId}");
            var searchRes = StringProblem.ConvertVietnameseToNormalizationForm(document.DocumentNode.QuerySelector(".breadcrumb-container h1 a").Attributes["title"].Value).Replace(' ', '-');
            var res = searchRes.Equals(authorId);

            if (res)
            {
                listOfStories = GetAllStoriesWithPagination($"{DomainTacGia}/{authorId}");
            }
        }
        catch (Exception)
        {
        }
        return listOfStories;
    }
    PagingRepresentative<Story> ICrawler.GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        authorId = WebUtility.UrlEncode(authorId);
        try
        {

            var document = GetWebPageDocument($"{DomainTacGia}/{authorId}");
            var searchRes = StringProblem.ConvertVietnameseToNormalizationForm(document.DocumentNode.QuerySelector(".breadcrumb-container h1 a").Attributes["title"].Value).Replace(' ', '-');
            var res = searchRes.Equals(authorId);

            if (res)
            {
                return GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTacGia}/{authorId}", page, limit);
            }
        }
        catch (Exception)
        {
        }
        return  new PagingRepresentative<Story>(page, limit, 0, new List<Story>());
    }


    public List<Chapter> GetChaptersOfStory(string path)
    {
        path = $"{Domain}{path}";
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
                        listOfChapter.Add(new Chapter(name, href, null));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        return listOfChapter;
    }
    public ChapterContent GetChapterContent(string path, int chapterIndex)
    {
        path = $"{Domain}{path}";
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



    public IEnumerable<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        throw new NotImplementedException();
    }

    public List<Chapter> GetChaptersOfStory(string storyId, int page, int limit)
    {
        throw new NotImplementedException();
    }


    PagingRepresentative<Chapter> ICrawler.GetChaptersOfStory(string storyId, int page, int limit)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Chapter> ICrawler.GetChaptersOfStory(string storyId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="systemLimit"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>Tuple of systemPageStart and systemOffsetStart</returns>
    private Tuple<int, int> CalculatePageAndLimit(int systemLimit, int page, int limit)
    {
        //int systemPageEnd = 0;
        int systemOffsetStart = 0;
        //int systemOffsetEnd = 0;

        var offsetStart = (page - 1) * limit;
        //var offsetEnd = Math.Min((page * limit) - 1, totalRecord - 1);
        int systemPageStart = offsetStart / systemLimit + 1;
        systemOffsetStart = offsetStart % systemLimit;
        //systemPageEnd = (offsetEnd / systemLimit) + 1;
        //systemOffsetEnd = offsetEnd % systemLimit;

        //int systemCurrentPage = systemPageStart;
        //while (systemCurrentPage <= systemPageEnd)
        //{
        //    var accessUrl = createURLFromPage.Invoke(systemCurrentPage);
        //    var document = GetWebPageDocument(accessUrl);
        //    var pageResults = crawlRepresentativesFromAPage.Invoke(document);
        //    var offsetStart = systemCurrentPage == systemPageStart ? systemOffsetStart : 0;
        //    var offsetEnd = systemCurrentPage == systemPageEnd ? systemOffsetEnd : systemLimit - 1;
        //    results.AddRange(pageResults.Skip(offsetStart).Take(offsetEnd - offsetStart + 1));
        //    if (nextPageAvailible.Invoke(document) == true)
        //    {
        //        break;
        //    }
        //    systemCurrentPage += 1;
        //}
        //return new PagingRepresentative<T>(page, limit, totalPage, results);

        return new Tuple<int, int>(systemPageStart, systemOffsetStart);
    }
}