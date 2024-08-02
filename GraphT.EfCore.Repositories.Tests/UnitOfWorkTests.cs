using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

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
		TaskAggregate task = new("Test Task");
		
		await _unitOfWork.Repository<TaskAggregate>().AddAsync(task);
		int saveResult = await _unitOfWork.SaveChangesAsync();
		TaskAggregate? savedTask = await _context.TaskAggregates.FindAsync(task.Id);

		Assert.Equal(1, saveResult);
		Assert.NotNull(savedTask);
		Assert.Equal(task.Id, savedTask.Id);
	}

	[Fact]
	public void Repository_ReturnsSameInstanceForSameType()
	{
		IRepository<TaskAggregate> repo1 = _unitOfWork.Repository<TaskAggregate>();
		IRepository<TaskAggregate> repo2 = _unitOfWork.Repository<TaskAggregate>();

		Assert.Same(repo1, repo2);
	}

	[Fact]
	public void Repository_ReturnsDifferentInstancesForDifferentTypes()
	{
		IRepository<TaskAggregate> repo1 = _unitOfWork.Repository<TaskAggregate>();
		IRepository<TaskLog> repo2 = _unitOfWork.Repository<TaskLog>();

		Assert.NotSame(repo1, repo2);
	}

	[Fact]
	public void Dispose_DisposesContext()
	{
		_unitOfWork.Dispose();

		Assert.Throws<ObjectDisposedException>(() => _context.TaskAggregates.ToList());
	}
}
