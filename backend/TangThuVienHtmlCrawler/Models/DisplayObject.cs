using backend.Domain.Entities;

namespace TangThuVienHtmlCrawler.Models;

public class DisplayObject(string iD, string name)
{
    public string ID { get; } = iD;
    public string Name { get; } = name;

    public static implicit operator Story(DisplayObject disObj)
    {
        var story = new Story(disObj.ID, disObj.Name);
        return story;
    }
}
