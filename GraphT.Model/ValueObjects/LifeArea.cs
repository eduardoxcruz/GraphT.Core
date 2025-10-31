namespace GraphT.Model.ValueObjects;

public readonly struct LifeArea : IEquatable<LifeArea>
{
	public string Name { get; }

	public LifeArea()
	{
		throw new ArgumentNullException(nameof(Name),
			"Can not create Life Area without name. Use constructor with param instead.");
	}

	public LifeArea(string name)
	{
		Name = name;
	}

	public bool Equals(LifeArea other)
	{
		return Name == other.Name;
	}

	public override bool Equals(object? obj)
	{
		return obj is LifeArea other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
	public static bool operator ==(LifeArea left, LifeArea right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(LifeArea left, LifeArea right)
	{
		return !(left == right);
	}
}
