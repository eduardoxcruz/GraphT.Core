using SeedWork;

namespace GraphT.Model.Aggregates;

public class LifeArea : IEntity<Guid>, IEquatable<LifeArea>
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }

    private LifeArea() { }
	
    public static LifeArea Create(string name)
    {
	    return string.IsNullOrWhiteSpace(name) ? 
		    throw new ArgumentException("Life area name cannot be empty.", nameof(name)) : 
		    new LifeArea { Id = Guid.NewGuid(), Name = name };
    }

    public static LifeArea Rehydrate(Guid id, string name)
    {
	    return new LifeArea { Id = id, Name = name };
    }

    public bool Equals(LifeArea? other)
    {
	    if (other is null)
	    {
		    return false;
	    }

	    if (ReferenceEquals(this, other))
	    {
		    return true;
	    }

	    return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
	    if (obj is null)
	    {
		    return false;
	    }

	    if (ReferenceEquals(this, obj))
	    {
		    return true;
	    }

	    if (obj.GetType() != GetType())
	    {
		    return false;
	    }

	    return Equals((LifeArea)obj);
    }

    public override int GetHashCode()
    {
	    return Id.GetHashCode();
    }

    public static bool operator ==(LifeArea? left, LifeArea? right)
    {
	    return Equals(left, right);
    }

    public static bool operator !=(LifeArea? left, LifeArea? right)
    {
	    return !Equals(left, right);
    }
}
