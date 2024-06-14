using backend.Application.DLLScanner.Contract;
using backend.Domain.Contract;
using System.Text;
using System.Text.RegularExpressions;

namespace backend.Application.Utils;

public class Algorithm
{
    public static List<T> PriorityMergeLists<T, TId>(List<List<T>> storiesList, Func<T, TId> GetID) where T : class where TId : IComparable
    {
        var storiesDto = new List<T>();
        while (true)
        {
            var firstStories = storiesList.Where(x => x.FirstOrDefault() != null).Select(x => x.First());
            if (firstStories == null)
            {
                break;
            }
            var minStory = firstStories.MinBy(GetID);
            if (minStory == null)
            {
                break;
            }
            storiesList.ForEach(x =>
            {
                var first = x.FirstOrDefault(s => GetID.Invoke(s).Equals(GetID.Invoke(minStory)));
                if (first != null)
                {
                    x.Remove(first);
                }
            });
            storiesDto.Add(minStory);
        }
        return storiesDto;
    }


    public static List<List<T>> CrawlMultipleSources<T>(IScanner<ICrawler> scanner, IEnumerable<string> sourceIDs, Func<ICrawler, List<T>> GetFromASource) where T : class
    {
        var tasks = new List<Task<List<T>>>();
        foreach (var id in sourceIDs)
        {
            var plugin = scanner.GetAllPlugins()[id].Plugin;
            tasks.Add(Task.Run(() => GetFromASource.Invoke(plugin)));
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
