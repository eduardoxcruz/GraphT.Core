using SeedWork;

namespace Core.Model.Enums;

public static class CoreDomainErrors
{
	public static DomainError TaskIsNotRecurring => new((int)CoreDomainErrorType.TaskIsNotRecurring, "Task is not recurring");
	public static DomainError RecurringTaskDoesNotNeedReset => new((int)CoreDomainErrorType.RecurringTaskDoesNotNeedReset, "Task does not need to be reset yet");
}
