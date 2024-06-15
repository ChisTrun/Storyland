namespace Backend.Domain.Primitives;

public class ChapterEntityBase(string storyId, int index) : EntityBase
{
    public string StoryID { get; } = storyId;
    public int Index { get; } = index;

    public override string Identity => $"{StoryID}{Index}";
}
