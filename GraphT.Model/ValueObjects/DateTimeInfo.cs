namespace GraphT.Model.ValueObjects;

public struct DateTimeInfo
{
	public DateTimeOffset CreationDateTime { get; private set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public string TimeSpend { get; private set; }
	public readonly string Punctuality => GetPunctuality();

	public DateTimeInfo()
	{
		CreationDateTime = DateTimeOffset.Now;
		TimeSpend = $"\u23f0 0 day(s) - 0 hours - 0 minutes";
	}

	public void UpdateTimeSpend(TimeSpan timeSpend)
	{
		if (timeSpend.Ticks < 1)
		{
			TimeSpend = $"\u23f0 0 day(s) - 0 hours - 0 minutes";
			return;
		}
		
		string emoji = timeSpend.TotalHours > 1 ? "\u23f0" : "\u26a1";
		TimeSpend = $"{emoji} {timeSpend.Days} day(s) - {timeSpend.Hours} hours - {timeSpend.Minutes} minutes";
	}
	
	private readonly string GetPunctuality()
	{
		DateTimeOffset now = DateTimeOffset.Now;

		if (!LimitDateTime.HasValue) return "\u26a0 No Target";

		if (FinishDateTime.HasValue)
		{
			if ((FinishDateTime.Value.Date - LimitDateTime.Value.Date).TotalDays == 0) return "\u2705 On Time!";
			
			int daysDifference = Math.Abs((LimitDateTime.Value.Date - FinishDateTime.Value.Date).Days);
			
			return FinishDateTime.Value.Date > LimitDateTime.Value.Date ? 
				$"\ud83d\udea8 Late {daysDifference} day(s)!" : 
				$"\u2b50 Early {daysDifference} day(s)!";
		}
		
		if ((LimitDateTime.Value.Date - now.Date).TotalDays == 0) return "\u26a0 Finish Today!";
		
		if (now > LimitDateTime)
		{
			int daysLate = Math.Abs((LimitDateTime.Value.Date - now.Date).Days);
			return $"\ud83d\udea8 Late {daysLate} day(s)!";
		}
		
		int daysToGo = (LimitDateTime.Value.Date - now.Date).Days + 1;
		return $"\u23f1 {daysToGo} day(s) to go!";
	}
}
