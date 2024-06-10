namespace backend.Domain.Entities;

public class Chapter(string name, string storyId, int index)
{
    public string StoryID { get; } = storyId;
    public int Index { get; } = index;
    public string Name { get; } = name;
}
