namespace Backend.Domain.Primitives;

public abstract class IDEntityBase(string id) : EntityBase
{
    public string ID { get; } = id;

    public override string Identity => ID;
}
