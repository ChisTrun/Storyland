using backend.Domain.Primitives;

namespace backend.Domain.Entities;

public class Chapter(string iD, string name, int index, Story belong) : EntityBase(iD)
{
    public string Name { get; } = name;
    public int Index { get; } = index;
    public Story Belong { get; } = belong;
}
