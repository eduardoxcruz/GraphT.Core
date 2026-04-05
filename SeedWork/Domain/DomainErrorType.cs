namespace SeedWork.Domain;

public enum DomainErrorType
{
	TaskIsNotRecurring,
	RecurringTaskDoesNotNeedReset,
	UserNotFound,
	LifeAreaNotFound,
	UsernameCannotBeNullOrEmpty,
	EmailCannotBeNullOrEmpty,
	PasswordCannotBeNullOrEmpty,
	UsernameAlreadyInUse,
	EmailAlreadyInUse,
	InvalidUsername,
	InvalidPassword,
	TaskNotFound,
	CreatorNotFound,
	NoPermissionToViewTask,
	NoPermissionToDeleteTask,
	NoPermissionToUpdateBasicProperties,
	NoPermissionToSetStatus,
	NoPermissionToUpdateDateTimes,
	NoPermissionToUpdateRecurrence,
	NoPermissionToRemoveParticipants,
	FilterAndPagingOptionsCannotBeBothNull
}
