namespace GraphT.Model.ValueObjects;

public struct DateTimeInfo
{
	public DateTimeOffset CreationDateTime { get; private set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public readonly string TimeSpend => GetTimeSpend();
	public readonly string Punctuality => GetPunctuality();

	public DateTimeInfo()
	{
		CreationDateTime = DateTimeOffset.Now;
	}

	private readonly string GetPunctuality()
	{
		DateTimeOffset now = DateTimeOffset.Now;

		if (!LimitDateTime.HasValue) return "⚠ No Target";

		if (FinishDateTime.HasValue)
		{
			if ((FinishDateTime.Value.Date - LimitDateTime.Value.Date).TotalDays == 0) return "✔️ On Time!";
			
			int daysDifference = Math.Abs((LimitDateTime.Value.Date - FinishDateTime.Value.Date).Days);
			
			return FinishDateTime.Value.Date > LimitDateTime.Value.Date ? 
						$"🚨 Late {daysDifference} day(s)!" : 
						$"⭐ Early {daysDifference} day(s)!";
		}
		
		if ((LimitDateTime.Value.Date - now.Date).TotalDays == 0) return "⚠ Finish Today!";
		
		if (now > LimitDateTime)
		{
			int daysLate = Math.Abs((LimitDateTime.Value.Date - now.Date).Days);
			return $"🚨 Late {daysLate} day(s)!";
		}
		
		int daysToGo = (LimitDateTime.Value.Date - now.Date).Days + 1;
		return $"⏱ {daysToGo} day(s) to go!";
	}

	private readonly string GetTimeSpend()
	{
		if (!StartDateTime.HasValue)
		{
			return "⏰0 month(s) - 0 day(s) - 0 hours - 0 minutes";
		}

		DateTimeOffset finishDate = FinishDateTime ?? DateTimeOffset.Now;
		TimeSpan duration = finishDate - StartDateTime.Value;
		int months = (finishDate.Year - StartDateTime.Value.Year) * 12 + finishDate.Month - StartDateTime.Value.Month;
		
		if (finishDate.Day < StartDateTime.Value.Day)
		{
			months--;
		}
		
		int days = (finishDate - StartDateTime.Value.AddMonths(months)).Days;
		int hours = duration.Hours;
		int minutes = duration.Minutes;
		string emoji = duration.TotalHours > 1 ? "⏰" : "⚡️";

		return $"{emoji}{months} month(s) - {days} day(s) - {hours} hours - {minutes} minutes";
	}
}
