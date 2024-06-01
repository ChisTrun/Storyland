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
using System.Reflection.Metadata;

namespace TruyenFullPlugin;

public class TruyenFullCommand : ICrawler
{
    public string Name => "Truyện Full";
    public string Description => "Plugin de lay data tu trang web truyenfull.vn";

    public static string Domain => "https://truyenfull.vn/";
    public static string DomainTheLoai => $"{Domain}the-loai/";
    public static string DomainTimKiem => "https://truyenfull.vn/tim-kiem/?tukhoa=";
    //public static string DomainKetQuaTimKiem => $"{Domain}/ket-qua-tim-kiem";
    public static string DomainDocTruyen => $"{Domain}doc-truyen/";
    public static string DomainTacGia => $"{Domain}tac-gia/";

    public IEnumerable<Category> GetCategories() //updated
    {
        var document = GetWebPageDocument(Domain);

        var liTags = document.DocumentNode.QuerySelectorAll("ul.control.navbar-nav > li")[1];
        var lists = liTags.QuerySelectorAll("div a");

        List<Category> listOfCategories = new List<Category>();

        foreach (var list in lists)
        {
            var url = list.Attributes["href"].Value;
            var name = list.InnerText;
            listOfCategories.Add(new Category(name, ModelExtension.GetIDFromUrl(ModelType.Category, url)));
        }

        return listOfCategories;
    }

    public IEnumerable<Story> GetStoriesOfCategory(string categoryId) //updated
    {
        return GetStoryWithPageAndLimit(ModelType.Category, ModelExtension.GetUrlFromID(ModelType.Category, categoryId), -1, -1).Item2;
    }

    PagingRepresentative<Story> ICrawler.GetStoriesOfCategory(string categoryId, int page, int limit) //updated
    {
        var res = GetStoryWithPageAndLimit(ModelType.Category, ModelExtension.GetUrlFromID(ModelType.Category, categoryId), page, limit);
        return new PagingRepresentative<Story>(page, limit, res.Item1, res.Item2);
    }

    public IEnumerable<Story> GetStoriesBySearchName(string storyName)
    {
        var storySearched = GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTimKiem}{WebUtility.UrlEncode(storyName)}", -1, -1).Item2;

        var searchNameNorm = StringProblem.ConvertVietnameseToNormalizationForm(storyName);

        List<Story> res = storySearched.Where(story =>
        {
            var storyNameNorm = StringProblem.ConvertVietnameseToNormalizationForm(story.Name);
            storyNameNorm = Regex.Replace(storyNameNorm, @"[^A-Za-z\d\s]", "");

            var boolres = storyNameNorm.Contains(searchNameNorm);
            return boolres;
        }
        ).ToList();

        return res;
    }

    PagingRepresentative<Story> ICrawler.GetStoriesBySearchName(string storyName, int page, int limit)
    {
        var storySearched = GetStoriesBySearchName(storyName).ToList();
        var storySeatchedCount = storySearched.Count();
        var totalPage = (storySeatchedCount / limit) + (storySeatchedCount % limit == 0 ? 0 : 1);
        if (page > totalPage)
        {
            return new PagingRepresentative<Story>(page, limit, -1, new List<Story>());
        }

        List<Story> res = new List<Story>();
        try
        {
            res = storySearched.GetRange((page - 1) * limit, page != totalPage ? limit : storySeatchedCount % limit);
        }
        catch (Exception ex)
        {
        }

        return new PagingRepresentative<Story>(page, limit, totalPage, res);
    }

    public IEnumerable<Story> GetStoriesOfAuthor(string authorId)
    {
        var storySearched = GetStoryWithPageAndLimit(ModelType.Story, $"{DomainTimKiem}{WebUtility.UrlEncode(authorId)}", -1, -1).Item2;

        var searchNameNorm = StringProblem.ConvertVietnameseToNormalizationForm(authorId);

        List<Story> res = storySearched.Where(story =>
        {
            var authorNameNorm = StringProblem.ConvertVietnameseToNormalizationForm(story.Author!);
            authorNameNorm = Regex.Replace(authorNameNorm, @"[^A-Za-z\d\s]", "");
            var boolres = authorNameNorm.Contains(searchNameNorm);
            return boolres;
        }
        ).ToList();

        return res;
    }

    PagingRepresentative<Story> ICrawler.GetStoriesOfAuthor(string authorId, int page, int limit)
    {
        var storySearched = GetStoriesOfAuthor(authorId).ToList();
        var storySeatchedCount = storySearched.Count();
        var totalPage = (storySeatchedCount / limit) + (storySeatchedCount % limit == 0 ? 0 : 1);
        if (page > totalPage)
        {
            return new PagingRepresentative<Story>(page, limit, totalPage, new List<Story>());
        }

        List<Story> res = new List<Story>();
        try
        {
            res = storySearched.GetRange((page - 1) * limit, page != totalPage ? limit : (storySeatchedCount % limit == 0 ? limit : storySeatchedCount % limit));
        }
        catch (Exception ex)
        {
        }

        return new PagingRepresentative<Story>(page, limit, totalPage, res);
    }

    IEnumerable<Chapter> ICrawler.GetChaptersOfStory(string storyId)//updated
    {
        return GetChapterWithPageAndLimit(storyId, -1, -1).Item2;
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
        var chapterName = "";
        var chapterID = "";
        try
        {
            chapterID = $"{storyId}chuong-{chapterIndex}";
            var path = $"{Domain}{chapterID}";
            var document = GetWebPageDocument(path);
            var mainContent = document.QuerySelector("#chapter-c");

            //storyId co the la "cuc-pham-o-re.29119/" hoac "con-duong-ba-chu-f24.20355/" 
            //if (mainContent == null)
            //{
            //    storySubId = storyId.Substring(0, storyId.LastIndexOf('-'));
            //    path = $"{Domain}/{storySubId}/chuong-{chapterIndex}.html";
            //    document = GetWebPageDocument(path);
            //    mainContent = document.QuerySelector("#chapter-c");
            //}

            var chapterNameTag = document.QuerySelector("#chapter-big-container .chapter-title");
            chapterName = chapterNameTag.InnerText;

            mainContent.SelectNodes("//div[contains(@class, 'ads')]")?.ToList().ForEach(n => n.Remove());
            text = mainContent.InnerHtml;
            text = text.Replace("<br>", "\n");
            text = Regex.Replace(text, @"<.*?>", string.Empty);
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
        return new ChapterContent(text, next, pre, chapterName, chapterID, chapterIndex);
    }

    //==========================================
    //=============Private Function================
    //==========================================


    private Tuple<string, string> GetNameUrlFromATag(HtmlNode aTag)
    {
        var url = aTag.GetAttributeValue("href", null) ?? throw new Exception();
        var name = aTag.GetDirectInnerTextDecoded();
        return Tuple.Create(url, name);
    }

    private Category GetCategoryFromSubURL(string subUrl)
    {
        var doc = GetWebPageDocument(subUrl);
        var name = doc.QuerySelector(".list.list-truyen .title-list h2 ").GetDirectInnerTextDecoded();
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

        var descriptionPTag = document.QuerySelector(".col-info-desc .desc-text");
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
    private Tuple<int, int> CalculatePageAndLimit(int systemLimit, int page, int limit) //updated
    {
        var offsetStart = (page - 1) * limit;
        int systemPageStart = offsetStart / systemLimit + 1;
        int systemOffsetStart = offsetStart % systemLimit;

        return new Tuple<int, int>(systemPageStart, systemOffsetStart);
    }

    private HtmlDocument GetWebPageDocument(string sourceURL)
    {
        sourceURL = HttpUtility.UrlDecode(sourceURL);

        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
        var document = web.Load(sourceURL);
        return document;
    }

    private Tuple<int, List<Story>> GetStoryWithPageAndLimit(ModelType modelType, string firstURL, int page, int limit) //updated
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
                systemLimit = documentFirstPage.QuerySelectorAll("#list-page  .list.list-truyen  .row .lazyimg").Count;
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

                    var systemTotalPage = 0;
                    var regex = ModelExtension.PageNumberRegrex(modelType);
                    var match = regex.Match(lastURL);
                    if (match.Success)
                    {
                        systemTotalPage = int.Parse(match.Groups[1].Value);
                    }

                    var documentLastPage = GetWebPageDocument(lastURL);
                    var systemLastPageCount = documentLastPage.QuerySelectorAll("#list-page  .list.list-truyen  .row .lazyimg").Count;
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

    private List<Story> GetListOfStoriesFromHTMLNode(HtmlDocument document, ref int? needRemain, int offset) //updated
    {
        var main = document.DocumentNode.QuerySelectorAll("#list-page .col-truyen-main .list.list-truyen");

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
            try
            {
                var aTag = row.QuerySelector("a");
                var href = aTag.GetAttributeValue("href", null);
                var url = ModelExtension.GetIDFromUrl(ModelType.Story, HtmlEntity.DeEntitize(href));
                var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);
                var imgTag = row.QuerySelector("img");
                var img = "";
                if (imgTag == null)
                {
                    imgTag = row.QuerySelector(".lazyimg");
                    img = imgTag.GetDataAttribute("image").Value;
                }
                else
                {
                    img = imgTag.GetAttributeValue("src", null);
                }
                var author = row.QuerySelector(".author").InnerText;

                listOfStories.Add(new Story(name, url, img, author));
                needRemain--;
            }
            catch (Exception)
            {
            }
        }

        return listOfStories;
    }


    private List<Story> GetAllStoriesWithOffsetAndLimit(string sourceURL, int firstOffset, int limit) //updated
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

    private Tuple<int, List<Chapter>> GetChapterWithPageAndLimit(string storyId, int limit, int page) //updated
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

                    var regex = ModelExtension.PageNumberRegrex(ModelType.Chapter);
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
         $"{Domain}/{storyId}/{(systemPageStart > 1 ?ModelExtension.PagingType(ModelType.Chapter) + systemPageStart +"/#chapter-list" : "")}" // first page to scrape
            };

        var pageToScrape = new Queue<string>();
        pageToScrape.Enqueue(ModelExtension.GetUrlFromID(ModelType.Story, storyId));

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

    public Story GetExactStory(string id) //update
    {
        var doc = GetWebPageDocument(ModelExtension.GetUrlFromID(ModelType.Story, id));
        var name = doc.QuerySelector(".col-info-desc h3.title").GetDirectInnerTextDecoded();
        var imgUrl = doc.QuerySelector(".books > .book > img").GetAttributeValue("src", null);
        var authorATag = doc.QuerySelectorAll(".col-info-desc .info div ")[0];
        authorATag = authorATag.QuerySelector("a");
        var tuple = GetNameUrlFromATag(authorATag);

        return new Story(name, id, imgUrl, tuple.Item2);
    }
}