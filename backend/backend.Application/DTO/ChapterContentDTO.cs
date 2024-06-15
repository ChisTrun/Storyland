namespace Backend.Application.DTO;

public class ChapterContentDTO(string storyID, int index, string name, string content)
{
    public string StoryID { get; } = storyID;
    public int Index { get; } = index;
    public string Name { get; } = name;
    public string Content { get; } = content;
}
