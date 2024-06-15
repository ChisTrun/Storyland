using Backend.Domain.Primitives;

namespace Backend.Domain.Entities;

public sealed class Story(string name, string iD, string? imageURL = null, string? authorName = null, int numberOfChapter = 0) : IDEntityBase(iD)
{
    public string Name { get; } = name;
    public string ImageURL { get; } = imageURL ?? "https://birkhauser.de/product-not-found.png";
    public string? AuthorName { get; } = authorName;
    public int NumberOfChapter { get; } = numberOfChapter;
}
