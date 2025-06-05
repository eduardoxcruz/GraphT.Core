using GraphT.UseCases.RemoveDownstream;

namespace GraphT.Presenters.RemoveDownstream;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}

