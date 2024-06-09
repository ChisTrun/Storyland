namespace backend.Domain.Entities;

public class Chapter(Story belong, int index, string name)
{
    public Story Belong { get; } = belong;
    public int Index { get; } = index;
    public string Name { get; } = name;
}
