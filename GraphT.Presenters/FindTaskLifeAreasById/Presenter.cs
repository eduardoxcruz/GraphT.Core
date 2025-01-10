using GraphT.UseCases.FindTaskLifeAreasById;

using SeedWork;

namespace GraphT.Presenters.FindTaskLifeAreasById;

public class Presenter : IPresenter<OutputDto>, IOutputPort
{
	public OutputDto Content { get; private set; }

	public ValueTask Handle(OutputDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}

