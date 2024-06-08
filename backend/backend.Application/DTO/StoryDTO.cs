namespace backend.Application.DTO;

public class StoryDTO(string id, string name, string? imageUrl, string? authorName)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string ImageUrl { get; } = imageUrl ?? "https://birkhauser.de/product-not-found.png";
    public string? AuthorName { get; } = authorName;
}
