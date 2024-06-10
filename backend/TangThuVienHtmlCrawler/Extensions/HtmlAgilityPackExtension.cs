using HtmlAgilityPack;
using System.Net;

namespace TangThuVienHtmlCrawler.Extensions;

public static class HtmlAgilityPackExtension
{
    public static string GetDirectInnerTextDecoded(this HtmlNode node)
    {
        return WebUtility.HtmlDecode(node.GetDirectInnerText());
    }

}
