using GraphT.EfCore.Repositories;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Tests;

public class UnitOfWorkTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public UnitOfWorkTests(TestDatabaseFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task SaveChangesAsync_PersistsChangesAcrossRepositoriesToDatabase()
	{
		EfDbContext context = _fixture.CreateContext();
		UnitOfWork unitOfWork = new(context);
		OldTodoTask task = new("Test Task");
		OldTaskLog log = new(task.Id, DateTimeOffset.UtcNow, OldStatus.Created);
		TodoTaskRepository todoTaskRepository = new(context);
		TaskLogRepository taskLogRepository = new(context);

		await todoTaskRepository.AddAsync(task);
		await taskLogRepository.AddAsync(log);
		int saveResult = await unitOfWork.SaveChangesAsync();
		bool taskExist = await context.TodoTasks.ContainsAsync(task);
		bool logExist = await context.TaskLogs.ContainsAsync(log);

		Assert.Equal(2, saveResult);
		Assert.True(taskExist);
		Assert.True(logExist);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
}
