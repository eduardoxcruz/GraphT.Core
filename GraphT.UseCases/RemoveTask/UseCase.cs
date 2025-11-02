using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.RemoveTask;

public class UseCase : IPortWithInput<InputDto>
{
	private readonly IRemoveTaskPort _removeTaskPort;
	private readonly IContainsTaskPort _containsTaskPort;

	public UseCase(IRemoveTaskPort removeTaskPort, IContainsTaskPort containsTaskPort)
	{
		_removeTaskPort = removeTaskPort;
		_containsTaskPort = containsTaskPort;
	}

	public async ValueTask HandleAsync(InputDto input)
	{
		if (!await _containsTaskPort.HandleAsync(input.Id)) 
			throw new TaskNotFoundException("Task not found.", input.Id);
		
		await _removeTaskPort.HandleAsync(input.Id);
	}
}

public record struct InputDto
{
	public Guid Id { get; set; }
}

