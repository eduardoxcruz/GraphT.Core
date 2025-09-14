using GraphT.Model.Entities;
using GraphT.Model.Services;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Services;

public class TaskProgressCalculatorServiceTests
{
	 [Fact]
    public void GetProgress_NoDownstreams_ReturnsCorrectProgress()
    {
        // Arrange
        bool finishCurrentTask = false;
        float expectedProgress = 0f;

        // Act

        float result = TaskProgressCalculatorService.GetProgress([], finishCurrentTask);
        // Assert
        Assert.Equal(expectedProgress, result);
    }

    [Fact]
    public void GetProgress_NoDownstreamsAndFinishCurrentTask_Returns100()
    {
        // Arrange
        bool finishCurrentTask = true;
        float expectedProgress = 100f;

        // Act
        float result = TaskProgressCalculatorService.GetProgress([], finishCurrentTask);

        // Assert
        Assert.Equal(expectedProgress, result);
    }

    [Fact]
    public void GetProgress_AllTasksInBacklog_ReturnsZero()
    {
        // Arrange
        HashSet<TodoTask> downstreams =
        [
	        new("Task 1", status: OldStatus.Backlog), 
	        new("Task 2", status: OldStatus.Backlog), 
	        new("Task 3", status: OldStatus.Backlog)
        ];
        bool finishCurrentTask = false;
        float expectedProgress = 0f;

        // Act
        float result = TaskProgressCalculatorService.GetProgress(downstreams, finishCurrentTask);

        // Assert
        Assert.Equal(expectedProgress, result);
    }

    [Fact]
    public void GetProgress_AllTasksCompletedOrDropped_Returns99()
    {
        // Arrange
        HashSet<TodoTask> downstreams =
        [
	        new("Task 1", status: OldStatus.Completed), 
	        new("Task 2", status: OldStatus.Dropped), 
	        new("Task 3", status: OldStatus.Completed)
        ];
        bool finishCurrentTask = false;
        float expectedProgress = 99f;

        // Act
        float result = TaskProgressCalculatorService.GetProgress(downstreams, finishCurrentTask);

        // Assert
        Assert.Equal(expectedProgress, result);
    }

    [Fact]
    public void GetProgress_AllTasksCompletedOrDroppedAndFinishCurrentTask_Returns100()
    {
        // Arrange
        HashSet<TodoTask> downstreams =
        [
	        new("Task 1", status: OldStatus.Completed), 
	        new("Task 2", status: OldStatus.Dropped), 
	        new("Task 3", status: OldStatus.Completed)
        ];
        bool finishCurrentTask = true;
        float expectedProgress = 100f;

        // Act
        float result = TaskProgressCalculatorService.GetProgress(downstreams, finishCurrentTask);

        // Assert
        Assert.Equal(expectedProgress, result);
    }

    [Theory]
    [InlineData(2, 1, 50f)]
    [InlineData(3, 2, 66.67f)]
    [InlineData(4, 1, 25f)]
    public void GetProgress_MixedTaskStates_ReturnsCorrectPercentage(
        int totalTasks, 
        int completedTasks, 
        float expectedProgress)
    {
        // Arrange
        HashSet<TodoTask> downstreams = [];
        
        // Agregar tareas completadas
        for (int i = 0; i < completedTasks; i++)
        {
            downstreams.Add(new TodoTask($"Completed Task {i}", status: OldStatus.Completed));
        }
        
        // Agregar tareas en progreso
        for (int i = 0; i < (totalTasks - completedTasks); i++)
        {
            downstreams.Add(new TodoTask($"In Progress Task {i}", status: OldStatus.Doing));
        }

        bool finishCurrentTask = false;
        float tolerance = 0.01f;

        // Act
        float result = TaskProgressCalculatorService.GetProgress(downstreams, finishCurrentTask);

        // Assert
        Assert.True(Math.Abs(expectedProgress - result) < tolerance);
    }
}
