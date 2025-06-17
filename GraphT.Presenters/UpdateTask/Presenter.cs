using GraphT.UseCases.UpdateTask;

namespace GraphT.Presenters.UpdateTask;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}
