using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class LifeAreasRepositoryTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public LifeAreasRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;
	
	public async Task FindTaskLifeAreasById_ReturnsLifeAreas()
	{
		// TODO: Implement this
		/*// Arrange
		EfDbContext context = _fixture.CreateContext();
		LifeAreasRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		LifeArea lifeArea = new("Name 1");
		LifeArea lifeArea2 = new("Name 2");

		// Act
		mainTask.AddLifeAreas([lifeArea, lifeArea2]);
		await context.LifeAreas.AddRangeAsync(lifeArea, lifeArea2);
		await context.TodoTasks.AddAsync(mainTask);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync(spec);
		TodoTask resultTask = results.First();

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal(mainTask.Id, resultTask.Id);
		Assert.Equal(2, resultTask.LifeAreas.Count);
		Assert.Contains(resultTask.LifeAreas, lf => lf.Id == lifeArea.Id);
		Assert.Contains(resultTask.LifeAreas, lf => lf.Id == lifeArea2.Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");*/
	}

	public async Task FindTaskLifeAreasById_WhenTaskHasNoLifeAreas_ReturnsEmptyPagedList()
	{
		// TODO: Implement this
		/*// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TaskIncludeLifeAreasSpecification spec = new(mainTask.Id);

		// Act
		await context.TodoTasks.AddAsync(mainTask);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync(spec);
		TodoTask resultTask = results.First();

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal(mainTask.Id, resultTask.Id);
		Assert.Empty(resultTask.LifeAreas);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");*/
	}

	public async Task FindTaskLifeAreasById_WhenTaskDoesNotExist_ReturnsEmptyPagedList()
	{
		// TODO: Implement this
		/*// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		Guid nonExistentId = Guid.NewGuid();
		TaskIncludeLifeAreasSpecification spec = new(nonExistentId);

		// Act
		PagedList<TodoTask> results = await repository.FindAsync(spec);

		// Assert
		Assert.NotNull(results);
		Assert.Empty(results);
		Assert.Equal(0, results.TotalCount);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");*/
	}
}
