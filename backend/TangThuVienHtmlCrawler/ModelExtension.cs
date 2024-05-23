using PluginBase.Models;
using System.Text.RegularExpressions;

namespace TangThuVien
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
                    return Regex.Replace(url, $"{TangThuVienCrawler.DomainDocTruyen}/", "");
                case ModelType.Author:
                    return Regex.Replace(url, TangThuVienCrawler.DomainTacGia, "");
                case ModelType.Category:
                    return Regex.Replace(url, TangThuVienCrawler.DomainTongHop, "");
                case ModelType.Chapter:
                    return Regex.Replace(url, $"{TangThuVienCrawler.DomainDocTruyen}/", "");
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
                    return $"{TangThuVienCrawler.DomainDocTruyen}/{id}";
                case ModelType.Author:
                    return $"{TangThuVienCrawler.DomainTacGia}{id}";
                case ModelType.Category:
                    return $"{TangThuVienCrawler.DomainTongHop}{id}";
                case ModelType.Chapter:
                    return $"{TangThuVienCrawler.DomainDocTruyen}/{id}";
                default:
                    break;
            }
            throw new NotImplementedException();
        }
    }

    public static class StringExtension
    {
        public static string TakeLastParamURL(this string self)
        {
            return self.Substring(self.LastIndexOf('/') + 1);
        }
    }

}
