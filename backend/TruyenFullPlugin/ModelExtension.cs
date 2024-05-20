using PluginBase.Models;
using System.Text.RegularExpressions;

namespace TruyenFullPlugin
{
    public enum ModelType
    {
        Story,
        Author,
        Category,
        Chapter,
    }

    public static class ModelExtension
    {
        public static string GetIDFromUrl(ModelType type, string url)
        {
            switch (type)
            {
                case ModelType.Story:
                    return Regex.Replace(url, TruyenFullCommand.DomainDocTruyen, "");
                case ModelType.Author:
                    return Regex.Replace(url, TruyenFullCommand.DomainTacGia, "");
                case ModelType.Category:
                    return Regex.Replace(url, TruyenFullCommand.Domain, "");
                case ModelType.Chapter:
                    return Regex.Replace(url, TruyenFullCommand.DomainDocTruyen, "");
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public static string GetUrlFromID(ModelType type, string id)
        {
            switch (type)
            {
                case ModelType.Story:
                    return $"{TruyenFullCommand.DomainDocTruyen}{id}";
                case ModelType.Author:
                    return $"{TruyenFullCommand.DomainTacGia}{id}";
                case ModelType.Category:
                    return $"{TruyenFullCommand.Domain}{id}";
                case ModelType.Chapter:
                    return $"{TruyenFullCommand.DomainDocTruyen}{id}";
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public static string GetUrl(this Story representative)
        {
            return $"{TruyenFullCommand.DomainDocTruyen}{representative.Id}";
        }
        public static string GetUrl(this Author representative)
        {
            return $"{TruyenFullCommand.DomainTacGia}{representative.Id}";
        }
        public static string GetUrl(this Category representative)
        {
            return $"{TruyenFullCommand.Domain}{representative.Id}";
        }
        public static string GetUrl(this Chapter representative)
        {
            return $"{TruyenFullCommand.DomainDocTruyen}{representative.Id}";
        }




    }
}
