using GraphT.UseCases.FindTaskDownstreamsById;

using SeedWork;

namespace GraphT.Presenters.FindTaskDownstreamsById;

public class Presenter : IPresenter<OutputDto>, IOutputPort
{
	public OutputDto Content { get; private set; }

	public ValueTask Handle(OutputDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}

