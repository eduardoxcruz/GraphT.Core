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
		try
		{
			await _removeTaskPort.HandleAsync(input.Id);
		}
		catch (Exception ex)
		{
			throw new ExternalRepositoryException("Error removing task from repository.", ex);
		}
	}
}

public record struct InputDto
{
	public Guid Id { get; set; }
}

