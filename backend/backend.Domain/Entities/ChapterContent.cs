namespace backend.Domain.Entities;

public class ChapterContent(Story belong, int index, string name, string content)
{
    public ChapterContent(Chapter chapter, string content) : this(chapter.Belong, chapter.Index, chapter.Name, content)
    {
    }

    public Story Belong { get; } = belong;
    public int Index { get; } = index;
    public string Name { get; } = name;
    public string Content { get; } = content;

    public Chapter ToChapter() => new(Belong, Index, Name);
}
