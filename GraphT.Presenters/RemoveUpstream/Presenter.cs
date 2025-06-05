using GraphT.UseCases.RemoveUpstream;

namespace GraphT.Presenters.RemoveUpstream;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}

