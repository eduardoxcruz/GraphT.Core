using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.DeleteTask;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(InputDto dto)
	{
		bool taskExist = await _unitOfWork.Repository<TodoTask>().ContainsAsync(task => task.Id.Equals(dto.Id));
		
		if (!taskExist)
		{
			throw new TaskNotFoundException("Task not found.", dto.Id);
		}

		TaskIncludeDownstreamsSpecification specification = new(dto.Id);
		TodoTask? task = (await _unitOfWork.Repository<TodoTask>().FindAsync(specification)).FirstOrDefault();
		
		foreach (TodoTask downstream in task.Downstreams)
		{
			downstream.RemoveUpstream(task);
			await _unitOfWork.Repository<TodoTask>().UpdateAsync(downstream);
		}
		
		await _unitOfWork.Repository<TodoTask>().RemoveAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle();
	}
}

public class InputDto
{
	public Guid Id { get; set; }
}

