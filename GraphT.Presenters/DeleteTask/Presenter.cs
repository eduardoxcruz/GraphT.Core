using GraphT.UseCases.DeleteTask;

namespace GraphT.Presenters.DeleteTask;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}

