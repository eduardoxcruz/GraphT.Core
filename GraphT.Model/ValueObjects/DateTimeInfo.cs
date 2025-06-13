namespace GraphT.Model.ValueObjects;

public struct DateTimeInfo
{
	public DateTimeOffset CreationDateTime { get; private set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public string TimeSpend { get; set; }
	public readonly string Punctuality => GetPunctuality();

	public DateTimeInfo()
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
			if ((FinishDateTime.Value - LimitDateTime.Value).TotalDays == 0) return "\u2705 On Time!";
			
			int daysDifference = Math.Abs((LimitDateTime.Value - FinishDateTime.Value).Days);
			
			return FinishDateTime.Value > LimitDateTime.Value ? 
				$"\ud83d\udea8 Late {daysDifference} day(s)!" : 
				$"\u2b50 Early {daysDifference} day(s)!";
		}
		
		if ((LimitDateTime.Value - now).TotalHours < 24) return "\u26a0 Finish Today!";
		
		if (now > LimitDateTime)
		{
			int daysLate = Math.Abs((LimitDateTime.Value - now).Days);
			return $"\ud83d\udea8 Late {daysLate} day(s)!";
		}
		
		int daysToGo = (LimitDateTime.Value - now).Days;
		return $"\u23f1 {daysToGo} day(s) to go!";
	}
}
