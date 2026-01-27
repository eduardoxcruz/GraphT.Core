using GraphT.Model.Enums;

namespace GraphT.Model.ValueObjects;

public class Punctuality
{
	public TimeSpan TimeDifference { get; }
	public PunctualityType Type { get; }

	public Punctuality(TimeSpan timeDifference, PunctualityType type)
	{
		TimeDifference = timeDifference;
		Type = type;
	}
}
