namespace Core.Model.ValueObjects;

public class RecurrenceInfo
{
	public bool IsRecurring { get; set; }
	public RecurrencePattern? RecurrencePattern { get; set; }
	public DateTimeOffset? LastRecurrenceReset { get; set; }

	public RecurrenceInfo(bool isRecurring, RecurrencePattern? recurrencePattern, DateTimeOffset? lastRecurrenceReset)
	{
		IsRecurring = isRecurring;
		RecurrencePattern = recurrencePattern;
		LastRecurrenceReset = lastRecurrenceReset;
	}
}
