namespace backend.Domain.Primitives;

public abstract class EntityBase(string id) : IEquatable<EntityBase>
{
    public string ID { get; } = id;

    public static bool operator ==(EntityBase? first, EntityBase? second)
    {
        return first is not null && second is not null && first.Equals(second);
    }

    public static bool operator !=(EntityBase? first, EntityBase? second)
    {
        return !(first == second);
    }

    public override bool Equals(object? obj)
    {

        if (obj == null || obj.GetType() != GetType() || obj is not EntityBase entityBase)
        {
            return false;
        }
        return entityBase.ID == ID;
    }

    public bool Equals(EntityBase? other)
    {
        if (other == null || other.GetType() != GetType())
        {
            return false;
        }
        return other.ID == ID;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}
