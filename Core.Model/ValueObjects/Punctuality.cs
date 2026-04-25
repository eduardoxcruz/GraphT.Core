using Core.Model.Enums;

namespace Core.Model.ValueObjects;

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
