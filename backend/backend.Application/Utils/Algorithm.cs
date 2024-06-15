using backend.Application.DLLScanner.Contract;
using backend.Domain.Contract;
using System.Text;
using System.Text.RegularExpressions;

namespace backend.Application.Utils;

public class Algorithm
{
    public static List<T> PriorityMergeLists<T, TId>(List<List<T>> listList, Func<T, TId> getID) where T : class where TId : IComparable
    {
        var list = new List<T>();
        while (true)
        {
            var firstStories = listList.Where(x => x.FirstOrDefault() != null).Select(x => x.First());
            if (firstStories == null)
            {
                break;
            }
            var minItem = firstStories.MinBy(getID);
            if (minItem == null)
            {
                break;
            }
            listList.ForEach(x =>
            {
                var first = x.FirstOrDefault(s => getID.Invoke(s).Equals(getID.Invoke(minItem)));
                if (first != null)
                {
                    x.Remove(first);
                }
            });
            list.Add(minItem);
        }
        return list;
    }

    public static List<T> PriorityMergeLists<T, TId>(List<List<T>> listList, Func<T, TId> getID, Action<List<List<T>>, T, List<T>> popRule) where T : class where TId : IComparable
    {
        var list = new List<T>();
        while (true)
        {
            var firstStories = listList.Where(x => x.FirstOrDefault() != null).Select(x => x.First());
            if (firstStories == null)
            {
                break;
            }
            var minItem = firstStories.MinBy(getID);
            if (minItem == null)
            {
                break;
            }
            popRule.Invoke(listList, minItem, list);
        }
        return list;
    }

    public static List<List<T>> CrawlMultipleSources<T>(IScanner<ICrawler> scanner, IEnumerable<string> sourceIDs, Func<ICrawler, List<T>> getFromASource) where T : class
    {
        var tasks = new List<Task<List<T>>>();
        foreach (var id in sourceIDs)
        {
            var plugin = scanner.GetAllPlugins()[id].Plugin;
            tasks.Add(Task.Run(() => getFromASource.Invoke(plugin)));
        }
        var superList = Task.WhenAll(tasks).Result.ToList();
        return superList;
    }

    public static string ConvertToUnsign(string str)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var temp = str.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, string.Empty)
                    .Replace('\u0111', 'd').Replace('\u0110', 'D');
    }
}
