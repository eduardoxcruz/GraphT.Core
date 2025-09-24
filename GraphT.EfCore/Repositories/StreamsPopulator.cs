using System.Reflection;

using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Repositories;

public static class StreamsPopulator
{
	public static async Task PopulateStreamCountsAsync(OldTodoTask task, EfDbContext context)
    {
        int upstreamsCount = await context.TaskStreams
            .Where(ts => ts.DownstreamId == task.Id)
            .CountAsync();
            
        int downstreamsCount = await context.TaskStreams
            .Where(ts => ts.UpstreamId == task.Id)
            .CountAsync();

        // TODO: Implement this
        /*int lifeAreasCount = await _context.TaskLifeAreas
            .Where(tla => tla.TaskId == task.Id)
            .CountAsync();*/

        FieldInfo? upstreamsField = typeof(OldTodoTask).GetField("_upstreamsCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        FieldInfo? downstreamsField = typeof(OldTodoTask).GetField("_downstreamsCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        /*FieldInfo? lifeAreasField = typeof(TodoTask).GetField("_lifeAreasCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);*/

        upstreamsField?.SetValue(task, (uint)upstreamsCount);
        downstreamsField?.SetValue(task, (uint)downstreamsCount);
        //lifeAreasField?.SetValue(task, (uint)lifeAreasCount);
    }

    public static async Task PopulateStreamCountsAsync(List<OldTodoTask> tasks, EfDbContext context)
    {
        if (!tasks.Any()) return;

        List<Guid> taskIds = tasks.Select(t => t.Id).ToList();
        
        Dictionary<Guid, int> upstreamsCounts = await context.TaskStreams
            .Where(ts => taskIds.Contains(ts.DownstreamId))
            .GroupBy(ts => ts.DownstreamId)
            .Select(g => new { TaskId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TaskId, x => x.Count);

        Dictionary<Guid, int> downstreamsCounts = await context.TaskStreams
            .Where(ts => taskIds.Contains(ts.UpstreamId))
            .GroupBy(ts => ts.UpstreamId)
            .Select(g => new { TaskId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TaskId, x => x.Count);

        // TODO: Implement this
        /*Dictionary<Guid, int> lifeAreasCounts = await _context.TaskLifeAreas
            .Where(tla => taskIds.Contains(tla.TaskId))
            .GroupBy(tla => tla.TaskId)
            .Select(g => new { TaskId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TaskId, x => x.Count);*/

        FieldInfo? upstreamsField = typeof(OldTodoTask).GetField("_upstreamsCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        FieldInfo? downstreamsField = typeof(OldTodoTask).GetField("_downstreamsCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // TODO: Implement this
        /*FieldInfo? lifeAreasField = typeof(TodoTask).GetField("_lifeAreasCount", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);*/
        
        foreach (OldTodoTask task in tasks)
        {
            upstreamsField?.SetValue(task, (uint)(upstreamsCounts.GetValueOrDefault(task.Id, 0)));
            downstreamsField?.SetValue(task, (uint)(downstreamsCounts.GetValueOrDefault(task.Id, 0)));
            //lifeAreasField?.SetValue(task, (uint)(lifeAreasCounts.GetValueOrDefault(task.Id, 0)));
        }
    }
}
