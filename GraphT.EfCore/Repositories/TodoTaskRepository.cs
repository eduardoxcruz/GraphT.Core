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

	public async ValueTask<OldTodoTask?> FindByIdAsync(Guid id)
	{
		OldTodoTask? task = await _context.TodoTasks.FirstOrDefaultAsync(t => t.Id.Equals(id));
        
		if (task != null)
		{
			await StreamsPopulator.PopulateStreamCountsAsync(task, _context);
		}
        
		return task;
	}

	public async ValueTask<PagedList<OldTodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams)
    {
       IQueryable<OldTodoTask> query = _context.TodoTasks
          .Where(task => task.OldStatus == OldStatus.Completed || task.OldStatus == OldStatus.Dropped)
          .OrderByDescending(task => task.OldDateTimeInfo.FinishDateTime)
          .AsNoTracking();

       int totalCount = await query.CountAsync();
       
       List<OldTodoTask> results = await query
          .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
          .Take(pagingParams.PageSize)
          .ToListAsync();

       return new PagedList<OldTodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<OldTodoTask>> FindTasksInProgress(PagingParams pagingParams)
    {
       IQueryable<OldTodoTask> query = _context.TodoTasks
          .Where(task => task.OldStatus == OldStatus.Doing)
          .OrderByDescending(task => task.Priority)
          .ThenBy(task => task.OldDateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue)
          .AsNoTracking();

       int totalCount = await query.CountAsync();
       
       List<OldTodoTask> results = await query
          .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
          .Take(pagingParams.PageSize)
          .ToListAsync();

       return new PagedList<OldTodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<OldTodoTask>> FindTasksReadyToStart(PagingParams pagingParams)
    {
	    IQueryable<OldTodoTask> query = _context.TodoTasks
		    .Where(task => 
			    (task.OldStatus == OldStatus.Ready || task.OldStatus == OldStatus.Paused) &&
			    (!_context.TaskStreams.Any(ts => ts.UpstreamId == task.Id) || task.Progress >= 99))
		    .OrderBy(task => task.OldDateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue)
		    .ThenByDescending(task => task.Priority)
		    .AsNoTracking();

	    int totalCount = await query.CountAsync();
        
	    List<OldTodoTask> results = await query
		    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
		    .Take(pagingParams.PageSize)
		    .ToListAsync();

	    await StreamsPopulator.PopulateStreamCountsAsync(results, _context);

	    return new PagedList<OldTodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }

    public async ValueTask<PagedList<OldTodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams)
    {
	    IQueryable<OldTodoTask> query = _context.TodoTasks
		    .OrderByDescending(task => task.OldDateTimeInfo.CreationDateTime)
		    .AsNoTracking();
            
	    int totalCount = await query.CountAsync();
        
	    List<OldTodoTask> results = await query
		    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
		    .Take(pagingParams.PageSize)
		    .ToListAsync();

	    await StreamsPopulator.PopulateStreamCountsAsync(results, _context);
        
	    return new PagedList<OldTodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
    }
    
    public async ValueTask AddAsync(OldTodoTask task)
	{
		await _context.TodoTasks.AddAsync(task);
	}

	public async ValueTask AddRangeAsync(IEnumerable<OldTodoTask> tasks)
	{
		await _context.TodoTasks.AddRangeAsync(tasks);
	}

	public async ValueTask RemoveAsync(OldTodoTask task)
	{
		_context.TaskStreams.RemoveRange(_context.TaskStreams.Where(ts => ts.UpstreamId == task.Id));
		_context.TaskStreams.RemoveRange(_context.TaskStreams.Where(ts => ts.DownstreamId == task.Id));
		_context.TodoTasks.Remove(task);
		await Task.CompletedTask;
	}

	public async ValueTask RemoveRangeAsync(IEnumerable<OldTodoTask> tasks)
	{
		IEnumerable<OldTodoTask> todoTasks = tasks as OldTodoTask[] ?? tasks.ToArray();
		HashSet<Guid> taskIds = todoTasks.Select(t => t.Id).ToHashSet(); // HashSet para mejor rendimiento en Contains
    
		IQueryable<TaskStream> relatedTaskStreams = _context.TaskStreams
			.Where(ts => taskIds.Contains(ts.UpstreamId) || taskIds.Contains(ts.DownstreamId));
    
		_context.TaskStreams.RemoveRange(relatedTaskStreams);
		_context.TodoTasks.RemoveRange(todoTasks);
    
		await Task.CompletedTask;
	}

	public ValueTask UpdateAsync(OldTodoTask oldTodoTask)
	{
		_context.TodoTasks.Attach(oldTodoTask);
		_context.Entry(oldTodoTask).State = EntityState.Modified;
		
		return ValueTask.CompletedTask;
	}

	public async ValueTask UpdateRangeAsync(IEnumerable<OldTodoTask> tasks)
	{
		_context.TodoTasks.UpdateRange(tasks);
		await Task.CompletedTask;
	}

	public async ValueTask<bool> ContainsAsync(Guid id)
	{
		return await _context.TodoTasks.AnyAsync(t => t.Id.Equals(id));
	}
}
