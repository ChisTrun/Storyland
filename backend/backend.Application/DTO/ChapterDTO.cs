namespace Backend.Application.DTO;

public class ChapterDTO(string storyID, int index, string name)
{
    public int Index { get; } = index;
    public string StoryID { get; } = storyID;
    public string Name { get; } = name;
}
