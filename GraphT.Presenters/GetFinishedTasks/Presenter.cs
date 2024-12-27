using GraphT.UseCases.GetFinishedTasks;

using SeedWork;

namespace GraphT.Presenters.GetFinishedTasks;

public class Presenter : IPresenter<OutputDto>, IOutputPort
{
	public OutputDto Content { get; private set; }
	
	public ValueTask Handle(OutputDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
