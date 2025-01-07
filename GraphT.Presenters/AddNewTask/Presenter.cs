using GraphT.UseCases.AddNewTask;

using SeedWork;

namespace GraphT.Presenters.AddNewTask;

public class Presenter : IPresenter<OutputDto>, IOutputPort
{
	public OutputDto Content { get; private set; }

	public ValueTask Handle(OutputDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
