using HtmlAgilityPack;
using System.Net;

namespace TruyenFullPlugin.Extensions;

public static class HtmlAgilityPackExtension
{
    public static string GetDirectInnerTextDecoded(this HtmlNode node)
    {
        try
        {
            return WebUtility.HtmlDecode(node.InnerText);
        }
        catch
        {
            return "";
        }
    }
}
