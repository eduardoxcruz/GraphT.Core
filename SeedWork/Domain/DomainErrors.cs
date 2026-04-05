namespace SeedWork.Domain;

public static class DomainErrors
{
	public static DomainError TaskIsNotRecurring => new(DomainErrorType.TaskIsNotRecurring, "Task is not recurring");
	public static DomainError RecurringTaskDoesNotNeedReset => new(DomainErrorType.RecurringTaskDoesNotNeedReset, "Task does not need to be reset yet");
	public static DomainError UserNotFound(Guid id) => new(DomainErrorType.UserNotFound, "User with specified id not found", id.ToString());
	public static DomainError LifeAreaNotFound(Guid id) => new(DomainErrorType.LifeAreaNotFound, "LifeArea with specified id not found", id.ToString());
	public static DomainError UsernameCannotBeNullOrEmpty => new(DomainErrorType.UsernameCannotBeNullOrEmpty, "Username cannot be null, empty, or whitespace");
	public static DomainError EmailCannotBeNullOrEmpty => new(DomainErrorType.EmailCannotBeNullOrEmpty, "Email cannot be null, empty, or whitespace");
	public static DomainError PasswordCannotBeNullOrEmpty => new(DomainErrorType.PasswordCannotBeNullOrEmpty, "Password cannot be null, empty, or whitespace");
	public static DomainError UsernameAlreadyInUse(string username) => new(DomainErrorType.UsernameAlreadyInUse, $"Username '{username}' is already in use", username);
	public static DomainError EmailAlreadyInUse(string email) => new(DomainErrorType.EmailAlreadyInUse, $"Email '{email}' is already in use", email);
	public static DomainError InvalidUsername => new(DomainErrorType.InvalidUsername, "Username must be provided");
	public static DomainError InvalidPassword => new(DomainErrorType.InvalidPassword, "Invalid password");
	public static DomainError TaskNotFound(Guid id) => new(DomainErrorType.TaskNotFound, $"Task with id {id} not found", id.ToString());
	public static DomainError CreatorNotFound(Guid id) => new(DomainErrorType.CreatorNotFound, $"Creator with id {id} not found", id.ToString());
	public static DomainError NoPermissionToViewTask => new(DomainErrorType.NoPermissionToViewTask, "You do not have permission to view this task");
	public static DomainError NoPermissionToDeleteTask => new(DomainErrorType.NoPermissionToDeleteTask, "You do not have permission to delete this task");
	public static DomainError NoPermissionToUpdateBasicProperties => new(DomainErrorType.NoPermissionToUpdateBasicProperties, "You don't have permission to update basic properties");
	public static DomainError NoPermissionToSetStatus => new(DomainErrorType.NoPermissionToSetStatus, "You don't have permission to set this status");
	public static DomainError NoPermissionToUpdateDateTimes => new(DomainErrorType.NoPermissionToUpdateDateTimes, "You don't have permission to update datetimes");
	public static DomainError NoPermissionToUpdateRecurrence => new(DomainErrorType.NoPermissionToUpdateRecurrence, "You don't have permission to update recurrence");
	public static DomainError NoPermissionToRemoveParticipants => new(DomainErrorType.NoPermissionToRemoveParticipants, "You don't have permission to remove participants");
	public static DomainError FilterAndPagingOptionsCannotBeBothNull => new(DomainErrorType.FilterAndPagingOptionsCannotBeBothNull, "Filter and paging options cannot be both null");
}
