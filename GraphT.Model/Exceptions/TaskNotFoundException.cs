namespace GraphT.Model.Exceptions;

public class TaskNotFoundException : Exception
{
	public Guid Id { get; }
	
	public TaskNotFoundException(Guid id)
	{
		Id = id;
	}
	
	public TaskNotFoundException(string message, Guid id) : base(message)
	{
		Id = id;
	}

	public TaskNotFoundException(string message, Exception inner, Guid id)
		: base(message, inner)
	{
		Id = id;
	}
}
