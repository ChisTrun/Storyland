using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public class Chapter(string name, string storyId, int index) : ChapterEntityBase(storyId, index)
{
    public string Name { get; } = name;
}
