using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class LifeAreaRepository: IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public LifeAreaRepository(TestDatabaseFixture fixture) => Fixture = fixture;

	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		LifeArea lifeArea = new("Life Area");
		
		await context.LifeAreas.AddAsync(lifeArea);
		await context.SaveChangesAsync();
		LifeArea? result = await repository.FindByIdAsync(lifeArea.Id);

		Assert.NotNull(result);
		Assert.Equal(lifeArea.Id, result.Id);
		Assert.Equal("Life Area", result.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		string expectedName = "Expected Name";
		List<LifeArea> lifeAreas =
		[
			new("Name"), new(expectedName), new("Name 2")
		];

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		PagedList<LifeArea> results = await repository.FindAsync(new BaseSpecification<LifeArea>(l => l.Name.Equals(expectedName)));

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.All(results, lifeArea => Assert.Equal(expectedName, lifeArea.Name));
		Assert.Equal(1, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task Find_ReturnsAllEntities()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		List<LifeArea> lifeAreas =
		[
			new("Name"), new("Name 2"), new("Name 3")
		];

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		PagedList<LifeArea> results = await repository.FindAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(3, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		LifeArea lifeArea = new("Name");

		await repository.AddAsync(lifeArea);
		await context.SaveChangesAsync();
		LifeArea? addedLifeArea = await context.LifeAreas.FindAsync(lifeArea.Id);

		Assert.NotNull(addedLifeArea);
		Assert.Equal(lifeArea.Id, addedLifeArea.Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		List<LifeArea> lifeAreas =
		[
			new("Name 1"), new("Name 2"), new("Name 3")
		];

		await repository.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();

		foreach (LifeArea lifeArea in lifeAreas)
		{
			LifeArea? addedLifeArea = await context.LifeAreas.FindAsync(lifeArea.Id);
			
			Assert.NotNull(addedLifeArea);
			Assert.Equal(lifeArea.Id, addedLifeArea.Id);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		LifeArea lifeArea = new("Name");

		await context.LifeAreas.AddAsync(lifeArea);
		await repository.RemoveAsync(lifeArea);
		await context.SaveChangesAsync();

		LifeArea? removedLifeArea = await context.LifeAreas.FindAsync(lifeArea.Id);

		Assert.Null(removedLifeArea);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		List<LifeArea> lifeAreas =
		[
			new("Name 1"), new("Name 2"), new("Name 3")
		];

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(lifeAreas);
		await context.SaveChangesAsync();

		foreach (LifeArea lifeArea in lifeAreas)
		{
			LifeArea? removedLifeArea = await context.LifeAreas.FindAsync(lifeArea.Id);
			
			Assert.Null(removedLifeArea);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		LifeArea lifeArea = new("Life Area");
		string newName = "New name";

		await context.LifeAreas.AddAsync(lifeArea);
		await context.SaveChangesAsync();
		lifeArea.Name = newName;
		await repository.UpdateAsync(lifeArea);
		await context.SaveChangesAsync();
		LifeArea? updatedLifeArea = await context.LifeAreas.FindAsync(lifeArea.Id);

		Assert.NotNull(updatedLifeArea);
		Assert.Equal(newName, updatedLifeArea.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		List<LifeArea> lifeAreas = new()
		{
			new LifeArea("Name 1"), 
			new LifeArea("Name 2"), 
			new LifeArea("Name 3")
		};

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		lifeAreas[0].Name = "Name 4";
		lifeAreas[1].Name = "Name 5";
		lifeAreas[2].Name = "Name 6";
		await repository.UpdateRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		LifeArea? firstUpdated = await context.LifeAreas.FindAsync(lifeAreas[0].Id);
		LifeArea? secondUpdated = await context.LifeAreas.FindAsync(lifeAreas[1].Id);
		LifeArea? thirdUpdated = await context.LifeAreas.FindAsync(lifeAreas[2].Id);

		Assert.NotNull(firstUpdated);
		Assert.NotNull(secondUpdated);
		Assert.NotNull(thirdUpdated);
		Assert.Equal("Name 4", firstUpdated.Name);
		Assert.Equal("Name 5", secondUpdated.Name);
		Assert.Equal("Name 6", thirdUpdated.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task ContainsAsync_WithSpecification_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		string expectedName = "Strange Name for Specification";
		LifeArea lifeArea = new(expectedName);

		await context.LifeAreas.AddAsync(lifeArea);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(new BaseSpecification<LifeArea>(t => t.Name.Equals(expectedName)));

		Assert.True(result);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		string expectedName = "Strange Name for Predicate";
		LifeArea lifeArea = new(expectedName);

		await context.LifeAreas.AddAsync(lifeArea);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(t => t.Name.Equals(expectedName));

		Assert.True(result);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		string expectedName = "Strange Name for Count";
		List<LifeArea> lifeAreas = new()
		{
			new LifeArea("Name 1"), 
			new LifeArea("Name 2"), 
			new LifeArea(expectedName)
		};

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(new BaseSpecification<LifeArea>(t => t.Name.Equals(expectedName)));

		Assert.Equal(1, count);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<LifeArea> repository = new(context);
		string expectedName = "Strange Name for Count";
		List<LifeArea> lifeAreas = new()
		{
			new LifeArea("Name 1"), 
			new LifeArea("Name 2"), 
			new LifeArea(expectedName)
		};

		await context.LifeAreas.AddRangeAsync(lifeAreas);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(t => t.Name.Equals(expectedName));

		Assert.Equal(1, count);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [LifeAreas]");
	}
}
