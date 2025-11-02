using GraphT.Model.Aggregates;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTaskById;

public class UseCase : IFullPort<InputDto, OutputDto>
{
	private readonly IFindTaskByIdPort _findTaskByIdPort;

	public UseCase(IFindTaskByIdPort findTaskByIdPort)
	{
		_findTaskByIdPort = findTaskByIdPort;
	}

	public async ValueTask<OutputDto> HandleAsync(InputDto input)
	{
		TodoTask task = await _findTaskByIdPort.HandleAsync(input.Id);
		
		return new OutputDto(task);
	}
}

public record struct InputDto(Guid Id);

public record struct OutputDto(TodoTask Task);
