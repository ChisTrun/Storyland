using System.Text.RegularExpressions;
using System.Text;

namespace backend.Domain.Utils
{
    public static class StringProblem
    {
        /// <summary>
        /// from Con Mèo -> con meo
        /// </summary>
        public static string ConvertVietnameseToNormalizationForm(string vietnameseString)
        {
            string normalizedString = vietnameseString.Normalize(NormalizationForm.FormD);

            var regex = new Regex(@"\p{Mn}", RegexOptions.Compiled);
            string standardString = regex.Replace(normalizedString, string.Empty);
            standardString = standardString.Replace("đ", "d");
            standardString = standardString.Replace("Đ", "D");
            standardString = standardString.ToLower();
            standardString = standardString.Trim();
            standardString = Regex.Replace(standardString, @"[^A-Za-z\d\s]", " ").Replace("  ", " ");

            return standardString;
        }

        public static string GetChapterNumber(string inputString)
        {
            var match = Regex.Match(inputString, @"Chương (\d+)");
            return match.Success ? match.Groups[1].Value : "-1";
        }
    }
}
