using System.Text.RegularExpressions;
using System.Text;

namespace PluginBase.Utils
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

            standardString = standardString.ToLower();

            return standardString;
        }
    }
}
