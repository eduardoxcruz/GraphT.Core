using GraphT.UseCases.AddUpstream;

namespace GraphT.Presenters.AddUpstream;

public class Presenter : IOutputPort
{
	public ValueTask Handle()
	{
		return ValueTask.CompletedTask;
	}
}

