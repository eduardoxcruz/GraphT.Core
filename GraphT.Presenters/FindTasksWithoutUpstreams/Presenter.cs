using GraphT.UseCases.FindTasksWithoutUpstreams;

using SeedWork;

namespace GraphT.Presenters.FindTasksWithoutUpstreams;

public class Presenter : IPresenter<OutputDto>, IOutputPort
{
	public OutputDto Content { get; private set; }

	public ValueTask Handle(OutputDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}

