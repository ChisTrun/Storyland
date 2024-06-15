using Backend.Domain.Primitives;

namespace Backend.Domain.Entities;

public class Chapter(string name, string storyId, int index) : ChapterEntityBase(storyId, index)
{
    public string Name { get; } = name;
}
