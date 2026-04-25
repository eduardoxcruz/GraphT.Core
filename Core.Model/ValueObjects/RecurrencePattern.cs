using Core.Model.Enums.Recurrence;

namespace Core.Model.ValueObjects;

public struct RecurrencePattern
{
	public RecurrenceType Type { get; init; }
	public int Interval { get; init; }
	public RecurrenceUnit? Unit { get; init; }
	public DayOfWeek[]? DaysOfWeek { get; init; }
	public int? DayOfMonth { get; init; }
	public WeekOccurrence? WeekOccurrence { get; init; }
	public DayOfWeek? DayOfWeekInMonth { get; init; }

	public RecurrencePattern(
		RecurrenceType type,
		int interval = 1,
		RecurrenceUnit? unit = null,
		DayOfWeek[]? daysOfWeek = null,
		int? dayOfMonth = null,
		WeekOccurrence? weekOccurrence = null,
		DayOfWeek? dayOfWeekInMonth = null)
	{
		if (interval <= 0)
		{
			throw new ArgumentException("Interval must be greater than 0", nameof(interval));
		}

		Type = type;
		Interval = interval;
		Unit = unit;
		DaysOfWeek = daysOfWeek;
		DayOfMonth = dayOfMonth;
		WeekOccurrence = weekOccurrence;
		DayOfWeekInMonth = dayOfWeekInMonth;

		Validate();
	}

	private void Validate()
	{
		switch (Type)
		{
			case RecurrenceType.Specific:
				if (Unit == null)
				{
					throw new ArgumentException("Unit is required for specific recurrence");
				}

				break;
			case RecurrenceType.DaysOfWeek:
				if (DaysOfWeek == null || DaysOfWeek.Length == 0)
				{
					throw new ArgumentException("DaysOfWeek is required for weekly recurrence");
				}

				break;
			case RecurrenceType.DayOfMonth:
				if (DayOfMonth == null || DayOfMonth < 1 || DayOfMonth > 31)
				{
					throw new ArgumentException("DayOfMonth must be between 1 and 31");
				}

				break;
			case RecurrenceType.WeekdayOfMonth:
				if (WeekOccurrence == null || DayOfWeekInMonth == null)
				{
					throw new ArgumentException("WeekOccurrence and DayOfWeekInMonth are required");
				}

				break;
		}
	}

	public DateTimeOffset CalculateNextLimitDate(DateTimeOffset referenceDate)
	{
		switch (Type)
		{
			case RecurrenceType.Specific:
				return CalculateSpecificRecurrence(referenceDate);
			case RecurrenceType.DaysOfWeek:
				return CalculateDaysOfWeekRecurrence(referenceDate);
			case RecurrenceType.DayOfMonth:
				return CalculateDayOfMonthRecurrence(referenceDate);
			case RecurrenceType.WeekdayOfMonth:
				return CalculateWeekdayOfMonthRecurrence(referenceDate);
			default:
				throw new InvalidOperationException($"Unknown recurrence type: {Type}");
		}
	}

	private DateTimeOffset CalculateSpecificRecurrence(DateTimeOffset referenceDate)
	{
		switch (Unit)
		{
			case RecurrenceUnit.Seconds:
				return referenceDate.AddSeconds(Interval);
			case RecurrenceUnit.Minutes:
				return referenceDate.AddMinutes(Interval);
			case RecurrenceUnit.Hours:
				return referenceDate.AddHours(Interval);
			case RecurrenceUnit.Days:
				return referenceDate.AddDays(Interval);
			case RecurrenceUnit.Weeks:
				return referenceDate.AddDays(Interval * 7);
			case RecurrenceUnit.Months:
				return referenceDate.AddMonths(Interval);
			case RecurrenceUnit.Years:
				return referenceDate.AddYears(Interval);
			default:
				throw new InvalidOperationException($"Unknown recurrence unit: {Unit}");
		}
	}

	private DateTimeOffset CalculateDaysOfWeekRecurrence(DateTimeOffset referenceDate)
	{
		DayOfWeek[] sortedDays = DaysOfWeek!.OrderBy(d => d).ToArray();
		DayOfWeek currentDayOfWeek = referenceDate.DayOfWeek;

		foreach (DayOfWeek day in sortedDays)
		{
			if (day <= currentDayOfWeek)
			{
				continue;
			}

			int daysToAdd = day - currentDayOfWeek;
			return referenceDate.AddDays(daysToAdd);
		}

		int daysUntilNextWeek = 7 - (int)currentDayOfWeek + (int)sortedDays[0];
		return referenceDate.AddDays(daysUntilNextWeek);
	}

	private DateTimeOffset CalculateDayOfMonthRecurrence(DateTimeOffset referenceDate)
	{
		DateTimeOffset nextDate = referenceDate.AddMonths(Interval);
		int daysInMonth = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
		int targetDay = Math.Min(DayOfMonth!.Value, daysInMonth);

		return new DateTimeOffset(
			nextDate.Year,
			nextDate.Month,
			targetDay,
			referenceDate.Hour,
			referenceDate.Minute,
			referenceDate.Second,
			referenceDate.Offset);
	}

	private DateTimeOffset CalculateWeekdayOfMonthRecurrence(DateTimeOffset referenceDate)
	{
		DateTimeOffset nextMonth = referenceDate.AddMonths(Interval);
		DateTimeOffset firstDayOfMonth = new(
			nextMonth.Year,
			nextMonth.Month,
			1,
			referenceDate.Hour,
			referenceDate.Minute,
			referenceDate.Second,
			referenceDate.Offset);

		int daysUntilTargetDay = ((int)DayOfWeekInMonth! - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
		DateTimeOffset firstOccurrence = firstDayOfMonth.AddDays(daysUntilTargetDay);

		int weeksToAdd;
		switch (WeekOccurrence)
		{
			case Enums.Recurrence.WeekOccurrence.First:
				weeksToAdd = 0;
				break;
			case Enums.Recurrence.WeekOccurrence.Second:
				weeksToAdd = 1;
				break;
			case Enums.Recurrence.WeekOccurrence.Third:
				weeksToAdd = 2;
				break;
			case Enums.Recurrence.WeekOccurrence.Fourth:
				weeksToAdd = 3;
				break;
			case Enums.Recurrence.WeekOccurrence.Last:
				weeksToAdd = GetLastOccurrenceOffset(firstOccurrence);
				break;
			default:
				throw new InvalidOperationException($"Unknown week occurrence: {WeekOccurrence}");
		}

		return firstOccurrence.AddDays(weeksToAdd * 7);
	}

	private int GetLastOccurrenceOffset(DateTimeOffset firstOccurrence)
	{
		int lastDayOfMonth = DateTime.DaysInMonth(firstOccurrence.Year, firstOccurrence.Month);
		int daysRemaining = lastDayOfMonth - firstOccurrence.Day;
		return daysRemaining / 7 * 7;
	}
}
