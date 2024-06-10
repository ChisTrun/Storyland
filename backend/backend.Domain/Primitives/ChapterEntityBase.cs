namespace backend.Domain.Primitives;

public class ChapterEntityBase(string storyId, int index) : EntityBase(storyId)
{
    public int Index { get; } = index;

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType() || obj is not ChapterEntityBase chapterEntityBase)
        {
            return false;
        }
        return chapterEntityBase.ID == ID && chapterEntityBase.Index == Index;
    }

    public override int GetHashCode()
    {
        return $"{ID}{Index}".GetHashCode();
    }
}
