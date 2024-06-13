namespace backend.Application.DTO;

public class StoryDTO(string id, string name, string imageUrl, string authorName, int numberOfChapter)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string ImageUrl { get; } = imageUrl;
    public string AuthorName { get; } = authorName;
    public int NumberOfChapter { get; } = numberOfChapter;
}
