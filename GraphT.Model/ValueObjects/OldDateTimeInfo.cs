namespace GraphT.Model.ValueObjects;

public struct OldDateTimeInfo
{
	public DateTimeOffset CreationDateTime { get; private set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public string TimeSpend { get; set; }
	public readonly string Punctuality => GetPunctuality();

	public OldDateTimeInfo()
	{
		CreationDateTime = DateTimeOffset.Now;
		TimeSpend = $"\u23f0 0 day(s) - 0 hours - 0 minutes";
	}

	private readonly string GetPunctuality()
	{
		DateTimeOffset now = DateTimeOffset.Now;

		if (!LimitDateTime.HasValue) return "\u26a0 No Target";

		if (FinishDateTime.HasValue)
		{
			if ((((LimitDateTime.Value - FinishDateTime.Value).TotalHours >= 0) && (LimitDateTime.Value - FinishDateTime.Value).TotalHours < 24)) return "\u2705 On Time!";
			
			double daysDifference = Math.Abs(Math.Round((LimitDateTime.Value - FinishDateTime.Value).TotalDays, 1, MidpointRounding.ToZero));
			
			return FinishDateTime.Value > LimitDateTime.Value ? 
				$"\ud83d\udea8 Late {daysDifference} day(s)!" : 
				$"\u2b50 Early {daysDifference} day(s)!";
		}
		
		if (((LimitDateTime.Value - now).TotalHours >= 0) && ((LimitDateTime.Value - now).TotalHours < 24))
		{
			return "\u26a0 Finish Today!";
		}
		
		if (now > LimitDateTime)
		{
			double daysLate = Math.Round((LimitDateTime.Value - now).TotalDays, 1, MidpointRounding.ToZero);
			return $"\ud83d\udea8 Late {daysLate} day(s)!";
		}
		
		double daysToGo = Math.Round((LimitDateTime.Value - now).TotalDays, 1, MidpointRounding.ToZero);
		return $"\u23f1 {daysToGo} day(s) to go!";
	}
}
