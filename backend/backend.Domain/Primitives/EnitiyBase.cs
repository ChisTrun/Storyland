namespace backend.Domain.Primitives;

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

        return obj == null || obj.GetType() != GetType() || obj is not EntityBase entityBase ? false : entityBase.Identity == Identity;
    }

    public bool Equals(EntityBase? other)
    {
        return other == null || other.GetType() != GetType() ? false : other.Identity == Identity;
    }

    public override int GetHashCode()
    {
        return Identity.GetHashCode();
    }
}
