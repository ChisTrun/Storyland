using HtmlAgilityPack;
using System.Net;

namespace PluginBase.Utils
{
    public static class HtmlAgilityPackExtension
    {
        public static string GetDirectInnerTextDecoded(this HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.GetDirectInnerText());
        }

    }
}
