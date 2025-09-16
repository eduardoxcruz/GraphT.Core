namespace GraphT.Model.ValueObjects;

public static class ElapsedTimeExtension
{
	public static string ToElapsedTime(this TimeSpan timeSpan)
	{
		return
			$"\u23f0 {timeSpan.Days} day(s) - {timeSpan.Hours} hour(s) - {timeSpan.Minutes} minute(s) - {timeSpan.Seconds} second(s)";
	}
}
