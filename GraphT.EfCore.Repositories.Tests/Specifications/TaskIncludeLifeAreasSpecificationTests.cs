using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Services.Specifications;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TaskIncludeLifeAreasSpecificationTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TaskIncludeLifeAreasSpecificationTests(TestDatabaseFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Find_ReturnsTaskWithLifeAreas()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate mainTask = new("Main Task");
		LifeArea lifeArea = new("Name 1");
		LifeArea lifeArea2 = new("Name 2");
		TaskIncludeLifeAreasSpecification spec = new(mainTask.Id);

		// Act
		mainTask.AddLifeAreas([lifeArea, lifeArea2]);
		await context.LifeAreas.AddRangeAsync(lifeArea, lifeArea2);
		await context.TaskAggregates.AddAsync(mainTask);
		await context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await repository.FindAsync(spec);
		TaskAggregate resultTask = results.First();

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal(mainTask.Id, resultTask.Id);
		Assert.Equal(2, resultTask.LifeAreas.Count);
		Assert.Contains(resultTask.LifeAreas, lf => lf.Id == lifeArea.Id);
		Assert.Contains(resultTask.LifeAreas, lf => lf.Id == lifeArea2.Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task Find_WhenTaskHasNoLifeAreas_ReturnsTaskWithEmptyLifeAreas()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate mainTask = new("Main Task");
		TaskIncludeLifeAreasSpecification spec = new(mainTask.Id);

		// Act
		await context.TaskAggregates.AddAsync(mainTask);
		await context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await repository.FindAsync(spec);
		TaskAggregate resultTask = results.First();

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal(mainTask.Id, resultTask.Id);
		Assert.Empty(resultTask.LifeAreas);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task Find_WhenTaskDoesNotExist_ReturnsEmptyList()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Guid nonExistentId = Guid.NewGuid();
		TaskIncludeLifeAreasSpecification spec = new(nonExistentId);

		// Act
		PagedList<TaskAggregate> results = await repository.FindAsync(spec);

		// Assert
		Assert.NotNull(results);
		Assert.Empty(results);
		Assert.Equal(0, results.TotalCount);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
}
