namespace Backend.Domain.Primitives;

public abstract class EntityBase : IEquatable<EntityBase>
{
    public abstract string Identity { get; }

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
        return entityBase.Identity == Identity;
    }

    public bool Equals(EntityBase? other)
    {
        if (other == null || other.GetType() != GetType())
        {
            return false;
        }
        return other.Identity == Identity;
    }

    public override int GetHashCode()
    {
        return Identity.GetHashCode();
    }
}
