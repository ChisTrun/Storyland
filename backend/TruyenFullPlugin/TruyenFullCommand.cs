using PluginBase.Models;
using HtmlAgilityPack;
using PluginBase.Utils;
using System.Web;
using PluginBase.Contract;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Xml.Linq;

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
        return GetStoryWithPageAndLimit(ModelType.Category, ModelExtension.GetUrlFromID(ModelType.Category, categoryId), -1, -1).Item2;
    }

    PagingRepresentative<Story> ICrawler.GetStoriesOfCategory(string categoryId, int page, int limit)
    {
        var res = GetStoryWithPageAndLimit(ModelType.Category, ModelExtension.GetUrlFromID(ModelType.Category, categoryId), page, limit);
        return new PagingRepresentative<Story>(page, limit, res.Item1, res.Item2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelType"></param>
    /// <param name="firstURL"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns>Total Page and List of Story</returns>
    private Tuple<int, List<Story>> GetStoryWithPageAndLimit(ModelType modelType, string firstURL, int page, int limit)
    {
        if (page < 0 || limit < 0)
        {
            return new Tuple<int, List<Story>>(-1, GetAllStoriesWithOffsetAndLimit(firstURL, 0, limit));
        }
        else
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
                    return new Tuple<int, List<Story>>(0, new List<Story>());
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

            return new Tuple<int, List<Story>>(totalPage, GetAllStoriesWithOffsetAndLimit($"{firstURL}/{(systemPageStart > 1 ? ModelExtension.PagingType(modelType) + systemPageStart : "")}", systemOffsetStart, limit));
        }
    }
    private List<Story> GetListOfStoriesFromHTMLNode(HtmlDocument document, ref int? needRemain, int offset)
    {
        var main = document.DocumentNode.QuerySelectorAll(".container .col-truyen-main .list.list-truyen");

        var rows = main.QuerySelectorAll(".row");

        List<Story> listOfStories = new List<Story>();

        foreach (var row in rows)
        {
            if (needRemain == 0)
            {
                break;
            }
            if (offset > 0)
            {
                offset--;
                continue;
            }
            var url = ModelExtension.GetIDFromUrl(ModelType.Story, HtmlEntity.DeEntitize(row.QuerySelector("a").Attributes["href"].Value));
            var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);
            var img = row.SelectSingleNode(".//div[@data-desk-image]")?.Attributes["data-desk-image"]?.Value;

            listOfStories.Add(new Story(name, url, img));
            needRemain--;
        }

        return listOfStories;
    }

    //Scrawling stories
    //private List<Story> GetAllStoriesWithPagination(string sourceURL)
    //{
    //    var pageDiscoverd = new List<string>
    //        {
    //            sourceURL // first page to scrape
    //        };

    //    var pageToScrape = new Queue<string>();
    //    pageToScrape.Enqueue(sourceURL);

    //    List<Story> listOfStories = new List<Story>();

    //    while (pageToScrape.Count > 0)
    //    {
    //        try
    //        {
    //            var currentPage = pageToScrape.Dequeue();
    //            var currentDocument = GetWebPageDocument(currentPage);

    //            var paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll(".pagination li a");
    //            foreach (var paginationHTMLElement in paginationHTMLElements)
    //            {
    //                var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;
    //                if (!pageDiscoverd.Contains(newPaginationLink))
    //                {
    //                    if (!pageToScrape.Contains(newPaginationLink))
    //                    {
    //                        pageToScrape.Enqueue(newPaginationLink);
    //                    }
    //                    pageDiscoverd.Add(newPaginationLink);
    //                }
    //            }
    //            int? tmp = int.MaxValue;
    //            listOfStories.AddRange((GetListOfStoriesFromHTMLNode(currentDocument, ref tmp, 0)));
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }

    //    return listOfStories;
    //}

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
        while (pageToScrape.Count > 0 && needRemain != 0)
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
        return GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTimKiem}{WebUtility.UrlEncode(storyName)}", -1, -1).Item2;
    }

    PagingRepresentative<Story> ICrawler.GetStoriesBySearchName(string storyName, int page, int limit)
    {
        var res = GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTimKiem}{WebUtility.UrlEncode(storyName)}", page, limit);
        return new PagingRepresentative<Story>(page, limit, res.Item1, res.Item2);
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
                listOfStories = GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTacGia}/{authorId}", -1, -1).Item2;
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
                var stories = GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTacGia}/{authorId}", page, limit);
                return new PagingRepresentative<Story>(page, limit, stories.Item1, stories.Item2);
            }
        }
        catch (Exception)
        {
        }
        return new PagingRepresentative<Story>(page, limit, 0, new List<Story>());
    }

    IEnumerable<Chapter> ICrawler.GetChaptersOfStory(string storyId)
    {
        return GetChapterWithPageAndLimit(storyId, -1, -1).Item2;
    }

    private Tuple<int, List<Chapter>> GetChapterWithPageAndLimit(string storyId, int limit, int page)
    {
        var story = GetExactStory(storyId);

        int systemLimit;
        int totalPage = 0;
        int totalRecord;
        var systemPageStart = 1;
        var systemOffsetStart = 0;
        var systemTotalPage = 0;
        if (limit >= 0 && page >= 0)
        {
            {
                var documentFirstPage = GetWebPageDocument($"{Domain}/{storyId}");
                systemLimit = documentFirstPage.QuerySelectorAll("#list-chapter .row ul.list-chapter li a").Count;
                if (systemLimit == 0)
                {
#pragma warning disable IDE0028 // Simplify collection initialization
                    return new Tuple<int, List<Chapter>>(0, new List<Chapter>());
#pragma warning restore IDE0028 // Simplify collection initialization
                }
                var lastLiPagingTag = documentFirstPage.QuerySelector("ul.pagination  li:last-child");
                if (lastLiPagingTag == null)
                {
                    totalRecord = systemLimit;
                }
                else
                {
                    var aTag = lastLiPagingTag.PreviousSiblingElement().GetChildElements().First();
                    var lastURL = aTag.Attributes["href"].Value;

                    var regex = new Regex(@"trang-(\d+)");
                    var match = regex.Match(lastURL);
                    if (match.Success)
                    {
                        systemTotalPage = int.Parse(match.Groups[1].Value);
                    }
                    var documentLastPage = GetWebPageDocument(lastURL);
                    var systemLastPageCount = documentLastPage.QuerySelectorAll("#list-chapter .row ul.list-chapter li a").Count;
                    totalRecord = systemLimit * (systemTotalPage - 1) + systemLastPageCount;
                }
                totalPage = (totalRecord / limit) + (totalRecord % limit == 0 ? 0 : 1);
            }
            var pageToGet = CalculatePageAndLimit(systemLimit, page, limit);
            systemPageStart = pageToGet.Item1;
            systemOffsetStart = pageToGet.Item2;
        }
        // another
        var pageDiscoverd = new List<string>
        {
         $"{Domain}/{storyId}/{(systemPageStart > 1 ? "trang-" + systemPageStart +"/#chapter-list" : "")}" // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue($"{Domain}/{storyId}");

        List<Chapter> listOfChapter = new List<Chapter>();

        int index = 0;
        while (pageToScrape.Count > 0 && limit != 0)
        {
            try
            {
                var currentPage = pageToScrape.Dequeue();
                var currentDocument = GetWebPageDocument(currentPage);

                var listsChapters = currentDocument.QuerySelectorAll(".list-chapter");
                foreach (var listChapter in listsChapters)
                {
                    if (limit == 0)
                    {
                        break;
                    }
                    var aTags = listChapter.QuerySelectorAll("a");
                    foreach (var aTag in aTags)
                    {
                        if (limit == 0)
                        {
                            break;
                        }
                        if (systemOffsetStart > 0)
                        {
                            systemOffsetStart--;
                            continue;
                        }
                        var href = ModelExtension.GetIDFromUrl(ModelType.Chapter, aTag.Attributes["href"].Value);
                        var title = (aTag.Attributes["title"].Value);
                        var name = title.Substring(title.IndexOf('-') + 2);
                        listOfChapter.Add(new Chapter(name, href, story, index));
                        index++;
                        limit--;
                    }
                }

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

        return new Tuple<int, List<Chapter>>(totalPage, listOfChapter);
    }

    PagingRepresentative<Chapter> ICrawler.GetChaptersOfStory(string storyId, int page, int limit)
    {
        var res = GetChapterWithPageAndLimit(storyId, limit, page);
        return new PagingRepresentative<Chapter>(page, limit, res.Item1, res.Item2);
    }

    public ChapterContent GetChapterContent(string storyId, int chapterIndex)
    {
        var text = "";
        var pre = "";
        var next = "";
        try
        {
            var storySubId = storyId.Substring(0, storyId.LastIndexOf("."));
            var path = $"{Domain}/{storySubId}/chuong-{chapterIndex}.html";
            var document = GetWebPageDocument(path);
            var mainContent = document.QuerySelector("#chapter-c");

            //storyId co the la "cuc-pham-o-re.29119/" hoac "con-duong-ba-chu-f24.20355/" 
            if (mainContent == null)
            {
                storySubId = storyId.Substring(0, storyId.LastIndexOf('-'));
                path = $"{Domain}/{storySubId}/chuong-{chapterIndex}.html";
                document = GetWebPageDocument(path);
                mainContent = document.QuerySelector("#chapter-c");
            }

            mainContent.SelectNodes("//div[contains(@class, 'ads')]")?.ToList().ForEach(n => n.Remove());
            text = mainContent.InnerHtml;
            text = text.Replace("<br>", "\n");

            next = document.QuerySelector("#next_chap").GetAttributeValue("href", null);
            if (next.Contains(Domain))
            {
                next = next.Replace(Domain, "");
            }
            else
            {
                next = "";
            }
            pre = document.QuerySelector("#prev_chap").GetAttributeValue("href", null);
            if (pre.Contains(Domain))
            {
                pre = pre.Replace(Domain, "");
            }
            else
            {
                pre = "";
            }
        }
        catch { }
        return new ChapterContent(text, next, pre);
    }

    private Story GetExactStory(string id)
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Story, id));
        var name = doc.QuerySelector(".col-info-desc h3.title").GetDirectInnerTextDecoded();
        var imgUrl = doc.QuerySelector(".books > .book > img").GetAttributeValue("src", null);
        return new Story(name, id, imgUrl);
    }

    private Tuple<string, string> GetNameUrlFromATag(HtmlNode aTag)
    {
        var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
        var name = aTag.GetDirectInnerTextDecoded();
        return Tuple.Create(url, name);
    }

    private Category GetCategoryFromSubURL(string subUrl)
    {
        var doc = GetWebPageDocument(subUrl);
        var name = doc.QuerySelector(".list.list-truyen .cat-title h2 ").GetDirectInnerTextDecoded();
        //var urlRaw = doc.QuerySelector("#update-tab > a").GetAttributeValue("href", null);
        //int startIndex = urlRaw.IndexOf("ctg=");
        //string ctgSubstring = urlRaw.Substring(startIndex);
        //int endIndex = ctgSubstring.IndexOf('&');
        //if (endIndex == -1)
        //{
        //    endIndex = ctgSubstring.Length;
        //}
        string ctgValue = subUrl.Replace(Domain, "");
        return new Category(name, ctgValue);
    }

    public StoryDetail GetStoryDetail(string storyName)
    {
        var story = GetExactStory(storyName);
        var url = story.GetUrl();
        var document = GetWebPageDocument(url);
        var authorATag = document.QuerySelectorAll(".col-info-desc .info div ")[0];
        authorATag = authorATag.QuerySelector("a");
        var tuple = GetNameUrlFromATag(authorATag);
        var author = new Author(tuple.Item2, ModelExtension.GetIDFromUrl(ModelType.Author, tuple.Item1));

        var t = document.QuerySelectorAll(".col-info-desc  .info  div");
        var statusSpan = document.QuerySelector(".col-info-desc  .info").ChildNodes.QuerySelectorAll("div")[2];
        var tmp = statusSpan.QuerySelector("span");
        var status = tmp.GetDirectInnerTextDecoded();
        var categoryTags = document.QuerySelector(".col-info-desc  .info").ChildNodes.QuerySelectorAll("div")[1].QuerySelectorAll("a");

        var categories = new List<Category>();
        foreach (var categoryTag in categoryTags)
        {
            var categorySubUrl = categoryTag.GetAttributeValue("href", null);
            var category = GetCategoryFromSubURL(categorySubUrl);
            categories.Add(category);
        }

        var descriptionPTag = document.QuerySelector(".col-info-desc .desc-text.desc-text-full p");
        var description = descriptionPTag.GetDirectInnerTextDecoded();
        return new StoryDetail(story, author, status, categories.ToArray(), description);
    }

    public IEnumerable<Story> GetStoriesOfAuthor(string authorId, int page, int limit)
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