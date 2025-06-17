using System.Reflection;

using GraphT.EfCore.Models;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public class TodoTaskRepository : ITodoTaskRepository
{
	private readonly EfDbContext _context;

	public TodoTaskRepository(EfDbContext context)
	{
		_context = context;
	}

	public async ValueTask<TodoTask?> FindByIdAsync(Guid id)
	{
		TodoTask? task = await _context.TodoTasks.FirstOrDefaultAsync(t => t.Id.Equals(id));
        
		if (task != null)
		{
			await StreamsPopulator.PopulateStreamCountsAsync(task, _context);
		}
        
		return task;
	}

	public async ValueTask<PagedList<TodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams)
    {
       IQueryable<TodoTask> query = _context.TodoTasks
          .Where(task => task.Status == Status.Completed || task.Status == Status.Dropped)
          .OrderByDescending(task => task.DateTimeInfo.FinishDateTime)
          .AsNoTracking();

       int totalCount = await query.CountAsync();
       
       List<TodoTask> results = await query
          .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
          .Take(pagingParams.PageSize)
          .ToListAsync();

       return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<TodoTask>> FindTasksInProgress(PagingParams pagingParams)
    {
       IQueryable<TodoTask> query = _context.TodoTasks
          .Where(task => task.Status == Status.Doing)
          .OrderByDescending(task => task.Priority)
          .ThenBy(task => task.DateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue)
          .AsNoTracking();

       int totalCount = await query.CountAsync();
       
       List<TodoTask> results = await query
          .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
          .Take(pagingParams.PageSize)
          .ToListAsync();

       return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<TodoTask>> FindTasksReadyToStart(PagingParams pagingParams)
    {
	    IQueryable<TodoTask> query = _context.TodoTasks
		    .Where(task => 
			    (task.Status == Status.Ready || task.Status == Status.Paused) &&
			    (!_context.TaskStreams.Any(ts => ts.UpstreamId == task.Id) || task.Progress >= 99))
		    .OrderBy(task => task.DateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue)
		    .ThenByDescending(task => task.Priority)
		    .AsNoTracking();

	    int totalCount = await query.CountAsync();
        
	    List<TodoTask> results = await query
		    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
		    .Take(pagingParams.PageSize)
		    .ToListAsync();

	    await StreamsPopulator.PopulateStreamCountsAsync(results, _context);

	    return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<TodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams)
    {
	    IQueryable<TodoTask> query = _context.TodoTasks
		    .OrderByDescending(task => task.DateTimeInfo.CreationDateTime)
		    .AsNoTracking();
            
	    int totalCount = await query.CountAsync();
        
	    List<TodoTask> results = await query
		    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
		    .Take(pagingParams.PageSize)
		    .ToListAsync();

	    await StreamsPopulator.PopulateStreamCountsAsync(results, _context);
        
	    return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
    
    public async ValueTask AddAsync(TodoTask task)
	{
		await _context.TodoTasks.AddAsync(task);
	}

	public async ValueTask AddRangeAsync(IEnumerable<TodoTask> tasks)
	{
		await _context.TodoTasks.AddRangeAsync(tasks);
	}

	public async ValueTask RemoveAsync(TodoTask task)
	{
		_context.TaskStreams.RemoveRange(_context.TaskStreams.Where(ts => ts.UpstreamId == task.Id));
		_context.TaskStreams.RemoveRange(_context.TaskStreams.Where(ts => ts.DownstreamId == task.Id));
		_context.TodoTasks.Remove(task);
		await Task.CompletedTask;
	}

	public async ValueTask RemoveRangeAsync(IEnumerable<TodoTask> tasks)
	{
		IEnumerable<TodoTask> todoTasks = tasks as TodoTask[] ?? tasks.ToArray();
		HashSet<Guid> taskIds = todoTasks.Select(t => t.Id).ToHashSet(); // HashSet para mejor rendimiento en Contains
    
		IQueryable<TaskStream> relatedTaskStreams = _context.TaskStreams
			.Where(ts => taskIds.Contains(ts.UpstreamId) || taskIds.Contains(ts.DownstreamId));
    
		_context.TaskStreams.RemoveRange(relatedTaskStreams);
		_context.TodoTasks.RemoveRange(todoTasks);
    
		await Task.CompletedTask;
	}

	public ValueTask UpdateAsync(TodoTask TodoTask)
	{
		_context.TodoTasks.Attach(TodoTask);
		_context.Entry(TodoTask).State = EntityState.Modified;
		
		return ValueTask.CompletedTask;
	}

	public async ValueTask UpdateRangeAsync(IEnumerable<TodoTask> tasks)
	{
		_context.TodoTasks.UpdateRange(tasks);
		await Task.CompletedTask;
	}

	public async ValueTask<bool> ContainsAsync(Guid id)
	{
		return await _context.TodoTasks.AnyAsync(t => t.Id.Equals(id));
	}
}
