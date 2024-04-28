using PluginBase;
using PluginBase.Models;
using HtmlAgilityPack;

namespace TruyenFullPlugin;

public class TruyenFullCommand : IStorySourcePlugin
{
    public string Name =>  "truyenfull.com";

    public string Description => "Plugin de lay data tu trang web truyenfull.com";

    public IEnumerable<Categories> GetCategories()
    {
        var web = new HtmlWeb();

        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

        var document = web.Load("https://truyenfull.com");

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

    public IEnumerable<StoryInfo> GetStoryInfos(string sourceURL)
    {
        var web = new HtmlWeb();

        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

        var document = web.Load(sourceURL);

        var main = document.DocumentNode.QuerySelectorAll(".container .col-truyen-main");

        var rows = main.QuerySelectorAll(".row");

        List<StoryInfo> storiesInfo = new List<StoryInfo>();

        foreach (var row in rows)
        {
            var url = HtmlEntity.DeEntitize(row.QuerySelector("a").Attributes["href"].Value);
            var name = HtmlEntity.DeEntitize(row.QuerySelector("a").InnerText);

            var img = row.SelectSingleNode(".//div[@data-desk-image]")?.Attributes["data-desk-image"]?.Value;

            storiesInfo.Add(new StoryInfo(name, url, img));
        }

        return storiesInfo;
    }
}
