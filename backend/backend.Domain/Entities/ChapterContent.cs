namespace backend.Domain.Entities;

public class ChapterContent(string content, string name, int index, string storyId)
{
    public string StoryID { get; } = storyId;
    public int Index { get; } = index;
    public string Name { get; } = name;
    public string Content { get; } = content;

    public Chapter ToChapter() => new(Name, StoryID, Index);
}
