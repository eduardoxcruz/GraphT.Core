using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Aggregates;

public class TaskAggregateTests
{
	[Fact]
	public void Constructor_UseDefaultOptionalArguments()
	{
		string taskName = "Test Task";
		TaskAggregate task = new(taskName);

		Assert.Equal(taskName, task.Name);
		Assert.False(task.IsFun);
		Assert.False(task.IsProductive);
		Assert.Equal(Complexity.Indefinite, task.Complexity);
		Assert.Equal(Priority.MentalClutter, task.Priority);
		Assert.Equal(Status.Backlog, task.Status);
		Assert.Equal(Relevance.Superfluous, task.Relevance);
	}

	[Fact]
	public void IsFunAndIsProductive_UpdatesRelevance()
	{
		TaskAggregate taskSuperfluous = new("Task Superfluous");
		TaskAggregate taskEntertaining = new("Task Entertaining", isFun:true);
		TaskAggregate taskNecessary = new("Task Necessary", isFun:false, isProductive:true);
		TaskAggregate taskPurposeful = new("TaskPurposeful", isFun:true, isProductive:true);

		Assert.Equal(Relevance.Superfluous, taskSuperfluous.Relevance);
		Assert.Equal(Relevance.Entertaining, taskEntertaining.Relevance);
		Assert.Equal(Relevance.Necessary, taskNecessary.Relevance);
		Assert.Equal(Relevance.Purposeful, taskPurposeful.Relevance);
	}

	[Fact]
	public void AddUpstream_AddsTaskToUpstreams()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask = new("Upstream Task");

		task.AddUpstream(upstreamTask);

		Assert.Contains(upstreamTask, task.Upstreams);
	}

	[Fact]
	public void RemoveUpstream_RemovesTaskFromUpstreams()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask = new("Upstream Task");
		TaskAggregate upstreamTask2 = new("Upstream Task 2");

		task.AddUpstream(upstreamTask);
		task.AddUpstream(upstreamTask2);
		task.RemoveUpstream(upstreamTask2);

		Assert.Contains(upstreamTask, task.Upstreams);
		Assert.DoesNotContain(upstreamTask2, task.Upstreams);
	}

	[Fact]
	public void AddUpstreams_AddsUpstreamsToCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask1 = new("Upstream Task 1");
		TaskAggregate upstreamTask2 = new("Upstream Task 2");
		HashSet<TaskAggregate> newUpstreams = new() { upstreamTask1, upstreamTask2 };

		task.AddUpstreams(newUpstreams);

		Assert.Contains(upstreamTask1, task.Upstreams);
		Assert.Contains(upstreamTask2, task.Upstreams);
	}

	[Fact]
	public void AddUpstreams_ThrowsExceptionIfNewCollectionAreNull()
	{
		TaskAggregate task = new("Test Task");

		Assert.Throws<ArgumentException>(() => task.AddUpstreams(null));
	}

	[Fact]
	public void AddUpstreams_ThrowsExceptionIfNewCollectionAreEmpty()
	{
		TaskAggregate task = new("Test Task");

		Assert.Throws<ArgumentException>(() => task.AddUpstreams(new HashSet<TaskAggregate>()));
	}

	[Fact]
	public void RemoveUpstreams_RemovesUpstreamsFromCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask1 = new("Upstream Task 1");
		TaskAggregate upstreamTask2 = new("Upstream Task 2");
		TaskAggregate upstreamTask3 = new("Upstream Task 3");
		HashSet<TaskAggregate> newUpstreams = new() { upstreamTask2, upstreamTask3 };

		task.AddUpstream(upstreamTask1);
		task.AddUpstreams(newUpstreams);
		task.RemoveUpstreams(newUpstreams);

		Assert.Contains(upstreamTask1, task.Upstreams);
		Assert.DoesNotContain(upstreamTask2, task.Upstreams);
		Assert.DoesNotContain(upstreamTask3, task.Upstreams);
	}

	[Fact]
	public void ReplaceUpstreams_ReplacesUpstreamCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask1 = new("Upstream Task 1");
		TaskAggregate upstreamTask2 = new("Upstream Task 2");
		TaskAggregate upstreamTask3 = new("Upstream Task 3");
		TaskAggregate upstreamTask4 = new("Upstream Task 4");
		HashSet<TaskAggregate> originalUpstreams = new() { upstreamTask1, upstreamTask2 };
		HashSet<TaskAggregate> newUpstreams = new() { upstreamTask3, upstreamTask4 };

		task.AddUpstreams(originalUpstreams);
		task.ReplaceUpstreams(newUpstreams);

		Assert.DoesNotContain(upstreamTask1, task.Upstreams);
		Assert.DoesNotContain(upstreamTask2, task.Upstreams);
		Assert.Contains(upstreamTask3, task.Upstreams);
		Assert.Contains(upstreamTask4, task.Upstreams);
	}

	[Fact]
	public void ClearUpstreams_ClearsCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate upstreamTask1 = new("Upstream Task 1");
		TaskAggregate upstreamTask2 = new("Upstream Task 2");
		HashSet<TaskAggregate> upstreams = new() { upstreamTask1, upstreamTask2 };

		task.AddUpstreams(upstreams);
		task.ClearUpstreams();

		Assert.Empty(task.Upstreams);
	}

	[Fact]
	public void AddDownstream_AddsTaskToDownstreams()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream = new("Upstream Task");

		task.AddDownstream(downstream);

		Assert.Contains(downstream, task.Downstreams);
	}

	[Fact]
	public void RemoveDownstream_RemovesTaskFromDownstreams()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream = new("Task");
		TaskAggregate downstream2 = new("Task 2");

		task.AddDownstream(downstream);
		task.AddDownstream(downstream2);
		task.RemoveDownstream(downstream2);

		Assert.Contains(downstream, task.Downstreams);
		Assert.DoesNotContain(downstream2, task.Downstreams);
	}

	[Fact]
	public void AddDownstreams_AddsDownstreamsToCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream1 = new("Upstream Task 1");
		TaskAggregate downstream2 = new("Upstream Task 2");
		HashSet<TaskAggregate> newDownstreams = new() { downstream1, downstream2 };

		task.AddDownstreams(newDownstreams);

		Assert.Contains(downstream1, task.Downstreams);
		Assert.Contains(downstream2, task.Downstreams);
	}

	[Fact]
	public void AddDownstreams_ThrowsExceptionIfNewCollectionAreNull()
	{
		TaskAggregate task = new("Test Task");

		Assert.Throws<ArgumentException>(() => task.AddDownstreams(null));
	}

	[Fact]
	public void AddDownstreams_ThrowsExceptionIfNewCollectionAreEmpty()
	{
		TaskAggregate task = new("Test Task");

		Assert.Throws<ArgumentException>(() => task.AddDownstreams(new HashSet<TaskAggregate>()));
	}

	[Fact]
	public void RemoveDownstreams_RemovesDownstreamsFromCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream1 = new("Task 1");
		TaskAggregate downstream2 = new("Task 2");
		TaskAggregate downstream3 = new("Task 3");
		HashSet<TaskAggregate> newDownstreams = new() { downstream2, downstream3 };

		task.AddDownstream(downstream1);
		task.AddDownstreams(newDownstreams);
		task.RemoveDownstreams(newDownstreams);

		Assert.Contains(downstream1, task.Downstreams);
		Assert.DoesNotContain(downstream2, task.Downstreams);
		Assert.DoesNotContain(downstream3, task.Downstreams);
	}

	[Fact]
	public void ReplaceDownstreams_ReplacesDownstreamCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream1 = new("Task 1");
		TaskAggregate downstream2 = new("Task 2");
		TaskAggregate downstream3 = new("Task 3");
		TaskAggregate downstream4 = new("Task 4");
		HashSet<TaskAggregate> originalDownstreams = new() { downstream1, downstream2 };
		HashSet<TaskAggregate> newDownstreams = new() { downstream3, downstream4 };

		task.AddDownstreams(originalDownstreams);
		task.ReplaceDownstreams(newDownstreams);

		Assert.DoesNotContain(downstream1, task.Downstreams);
		Assert.DoesNotContain(downstream2, task.Downstreams);
		Assert.Contains(downstream3, task.Downstreams);
		Assert.Contains(downstream4, task.Downstreams);
	}

	[Fact]
	public void ClearDownstreams_ClearsCollection()
	{
		TaskAggregate task = new("Test Task");
		TaskAggregate downstream1 = new("Task 1");
		TaskAggregate downstream2 = new("Task 2");
		HashSet<TaskAggregate> downstreams = new() { downstream1, downstream2 };

		task.AddDownstreams(downstreams);
		task.ClearDownstreams();

		Assert.Empty(task.Downstreams);
	}

	[Fact]
	public void SetStartDate_UpdatesStartDate()
	{
		TaskAggregate task = new("Test Task");
		DateTimeOffset startDateTime = new(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);

		task.SetStartDate(startDateTime);

		Assert.Equal(startDateTime, task.DateTimeInfo.StartDateTime);
	}

	[Fact]
	public void SetStartDate_ThrowsExceptionIfStartDateAfterFinishDate()
	{
		TaskAggregate task = new("Test Task");
		DateTimeOffset finishDate = DateTimeOffset.Now;

		task.SetFinishDate(finishDate);

		Assert.Throws<ArgumentException>(() => task.SetStartDate(finishDate.AddDays(1)));
	}

	[Fact]
	public void SetFinishDate_UpdatesFinishDate()
	{
		TaskAggregate task = new("Test Task");
		DateTimeOffset finishDateTime = new(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);

		task.SetFinishDate(finishDateTime);

		Assert.Equal(finishDateTime, task.DateTimeInfo.FinishDateTime);
	}

	[Fact]
	public void SetFinishDate_ThrowsExceptionIfFinishDateBeforeStartDate()
	{
		TaskAggregate task = new("Test Task");
		DateTimeOffset startDate = DateTimeOffset.Now;

		task.SetStartDate(startDate);

		Assert.Throws<ArgumentException>(() => task.SetFinishDate(startDate.AddDays(-1)));
	}

	[Fact]
	public void SetLimitDate_UpdatesLimitDate()
	{
		TaskAggregate task = new("Test Task");
		DateTimeOffset limitDate = DateTimeOffset.Now.AddDays(1);

		task.SetLimitDate(limitDate);

		Assert.Equal(limitDate, task.DateTimeInfo.LimitDateTime);
	}

	[Fact]
	public void SetPriority_UpdatesPriority()
	{
		TaskAggregate task = new("Test Task");

		task.Priority = Priority.DoItNow;

		Assert.Equal(Priority.DoItNow, task.Priority);
	}

	[Fact]
	public void SetComplexity_UpdatesComplexity()
	{
		TaskAggregate task = new("Test Task");

		task.Complexity = Complexity.Indefinite;

		Assert.Equal(Complexity.Indefinite, task.Complexity);
	}

	[Fact]
	public void SetStatus_UpdatesStatus()
	{
		TaskAggregate task = new("Test Task");

		task.Status = Status.Completed;

		Assert.Equal(Status.Completed, task.Status);
	}
	
	[Fact]
	public void Progress_ReturnsZero_WhenNoDownstreamTasksAndParentIsNotCompleted()
	{
		TaskAggregate task = new("Parent Task", status: Status.Backlog);
		
		Assert.Equal(0, task.Progress);
		Assert.NotEqual(Status.Completed, task.Status);
	}
	
	[Fact]
	public void Progress_ReturnsHundred_WhenNoDownstreamTasksAndParentIsCompleted()
	{
		TaskAggregate task = new("Parent Task", status: Status.Completed);
		
		Assert.Equal(100, task.Progress);
		Assert.Equal(Status.Completed, task.Status);
	}
	
	[Fact]
	public void Progress_ReturnsHundred_WhenAllDownstreamTasksAreCompletedOrDropped()
	{
		TaskAggregate task = new("Parent Task");
		
		task.AddDownstreams(new HashSet<TaskAggregate>
		{
			new("Downstream 1", status: Status.Completed), 
			new("Downstream 2", status: Status.Completed),
			new("Downstream 3", status: Status.Dropped)
		});

		Assert.Equal(100, task.Progress);
	}

	[Fact]
	public void Progress_ReturnsZero_WhenAllDownstreamTasksAreInBacklog()
	{
		TaskAggregate task = new("Parent Task");
		
		task.AddDownstreams(new HashSet<TaskAggregate>
		{
			new("Downstream 1", status: Status.Backlog), 
			new("Downstream 2", status: Status.Backlog)
		});

		Assert.Equal(0, task.Progress);
	}
	
	[Fact]
	public void Progress_ReturnsCorrectProgress()
	{
		TaskAggregate task = new("Parent Task");
		
		task.AddDownstreams(new HashSet<TaskAggregate>
		{
			new("Downstream 1", status: Status.Backlog), 
			new("Downstream 2", status: Status.ReadyToStart),
			new("Downstream 3", status: Status.InProgress),
			new("Downstream 4", status: Status.Paused),
			new("Downstream 5", status: Status.Dropped),
			new("Downstream 6", status: Status.Completed),
		});

		float expectedProgress = 28;
		float tolerance = 1;

		Assert.True(Math.Abs(task.Progress - expectedProgress) < tolerance);
	}
}
