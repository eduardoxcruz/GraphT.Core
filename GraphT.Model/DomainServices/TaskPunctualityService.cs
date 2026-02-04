using GraphT.Model.Enums;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.DomainServices;

public static class TaskPunctualityService
{
	public static Punctuality Calculate(DateTimeOffset? limitDate, DateTimeOffset? finishDate)
	{
		const int totalMillisecondsInADay = 86_400_000;
		
		if (limitDate is null) return new Punctuality(TimeSpan.Zero, PunctualityType.NoLimit);

		TimeSpan timeDifference;
		
		if (finishDate is not null)
		{
			timeDifference = finishDate.Value - limitDate.Value;

			switch (timeDifference.TotalMilliseconds)
			{
				case 0: return new Punctuality(timeDifference, PunctualityType.OnTime);
				case < 0: return timeDifference.TotalMilliseconds < -totalMillisecondsInADay ? 
					new Punctuality(timeDifference, PunctualityType.Early): 
					new Punctuality(timeDifference, PunctualityType.OnTime);
				case > 0: return timeDifference.TotalSeconds < 1 ? 
					new Punctuality(timeDifference, PunctualityType.OnTime) : 
					new Punctuality(timeDifference, PunctualityType.Late);
			}
		}

		DateTimeOffset now = DateTimeOffset.Now;
		timeDifference = (limitDate.Value - now);
		const int marginOfError = 30;
		
		if (timeDifference.Seconds > 0) timeDifference = timeDifference.Add(TimeSpan.FromMilliseconds(marginOfError));
		
		if (timeDifference.TotalMilliseconds < (totalMillisecondsInADay + marginOfError))
		{
			return timeDifference.TotalMilliseconds >= -marginOfError ? 
				new Punctuality(timeDifference, PunctualityType.FinishToday) : 
				new Punctuality(timeDifference, PunctualityType.Late);
		}
		
		return new Punctuality(timeDifference, PunctualityType.TimeRemaining);
	}
}
