using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTaskById;

public class UseCase : IFullPort<InputDto, OutputDto>
{
	private readonly IFindTaskByIdPort _findTaskByIdPort;
	private readonly IContainsTaskPort _containsTaskPort;

	public UseCase(IFindTaskByIdPort findTaskByIdPort, IContainsTaskPort containsTaskPort)
	{
		_findTaskByIdPort = findTaskByIdPort;
		_containsTaskPort = containsTaskPort;
	}

	public async ValueTask<OutputDto> HandleAsync(InputDto input)
	{
		if (!await _containsTaskPort.HandleAsync(input.Id))
			throw new TaskNotFoundException("Task not found.", input.Id);
		
		return new OutputDto(await _findTaskByIdPort.HandleAsync(input.Id));
	}
}

public record struct InputDto(Guid Id);

public record struct OutputDto(TodoTask Task);
