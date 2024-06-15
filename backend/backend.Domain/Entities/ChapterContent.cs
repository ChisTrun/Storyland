using Backend.Domain.Primitives;

namespace Backend.Domain.Entities;

public class ChapterContent(string content, string name, int index, string storyId) : ChapterEntityBase(storyId, index)
{
    public string Name { get; } = name;
    public string Content { get; } = content;

    public Chapter ToChapter() => new(Name, StoryID, Index);
}
