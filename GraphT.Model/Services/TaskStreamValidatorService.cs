namespace GraphT.Model.Services;

public static class TaskStreamValidatorService
{
	public static bool IsStreamValid(Guid streamId)
	{
		if (streamId.Equals(Guid.Empty)) throw new ArgumentException("Stream id cannot be empty");

		return true;
	}

	public static bool IsStreamCollectionValid(ICollection<Guid> streamIdCollection)
	{
		if (streamIdCollection is null) throw new ArgumentException("Task collection cannot be null");

		if (streamIdCollection.Count == 0) throw new ArgumentException("Task collection cannot be empty");

		if (streamIdCollection.Any(taskId => taskId.Equals(Guid.Empty)))
			throw new ArgumentException("Task collection cannot contain tasks with empty Id");

		return true;
	}
}
