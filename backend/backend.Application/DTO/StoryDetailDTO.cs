namespace backend.Application.DTO;

public class StoryDetailDTO(string id, string name, string imageUrl, DisplayDTO author, string status, IEnumerable<DisplayDTO> categories, string description)
{
    public DisplayDTO Author { get; } = author;
    public string Status { get; } = status;
    public IEnumerable<DisplayDTO> Categories { get; } = categories;
    public string Description { get; } = description;
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string ImageUrl { get; } = imageUrl;
}
