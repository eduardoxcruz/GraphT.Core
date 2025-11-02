using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.RemoveTask;

public class UseCase : IPortWithInput<InputDto>
{
	private readonly IRemoveTaskPort _removeTaskPort;

	public UseCase(IRemoveTaskPort removeTaskPort)
	{
		_removeTaskPort = removeTaskPort;
	}

	public async ValueTask HandleAsync(InputDto input)
	{
		await _removeTaskPort.HandleAsync(input.Id);
	}
}

public record struct InputDto
{
	public Guid Id { get; set; }
}

