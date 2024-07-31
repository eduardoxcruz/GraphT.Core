using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests;

public class UnitOfWorkTests : TestBase
{
	private readonly UnitOfWork _unitOfWork;

	public UnitOfWorkTests()
	{
		_unitOfWork = new UnitOfWork(_context);
	}

	[Fact]
	public async Task SaveChangesAsync_PersistsChangesToDatabase()
	{
		TodoTask task = new("Test Task");
		
		await _unitOfWork.Repository<TodoTask>().AddAsync(task);
		int saveResult = await _unitOfWork.SaveChangesAsync();
		TodoTask? savedTask = await _context.TodoTasks.FindAsync(task.Id);

		Assert.Equal(1, saveResult);
		Assert.NotNull(savedTask);
		Assert.Equal(task.Id, savedTask.Id);
	}

	[Fact]
	public void Repository_ReturnsSameInstanceForSameType()
	{
		IRepository<TodoTask> repo1 = _unitOfWork.Repository<TodoTask>();
		IRepository<TodoTask> repo2 = _unitOfWork.Repository<TodoTask>();

		Assert.Same(repo1, repo2);
	}

	[Fact]
	public void Repository_ReturnsDifferentInstancesForDifferentTypes()
	{
		IRepository<TodoTask> repo1 = _unitOfWork.Repository<TodoTask>();
		IRepository<TaskAggregate> repo2 = _unitOfWork.Repository<TaskAggregate>();

		Assert.NotSame(repo1, repo2);
	}

	[Fact]
	public void Dispose_DisposesContext()
	{
		_unitOfWork.Dispose();

		Assert.Throws<ObjectDisposedException>(() => _context.TodoTasks.ToList());
	}
}
