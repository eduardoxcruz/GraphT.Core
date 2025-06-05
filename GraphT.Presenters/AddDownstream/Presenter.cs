using GraphT.UseCases.AddDownstream;

namespace GraphT.Presenters.AddDownstream;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}

