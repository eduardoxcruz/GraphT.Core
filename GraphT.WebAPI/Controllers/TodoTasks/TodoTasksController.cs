using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.DeleteTask;
using GraphT.Controllers.FindTaskById;
using GraphT.Controllers.GetTaskEnumsItems;
using GraphT.Controllers.UpdateTask;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TodoTasks;

[ApiController]
[Route("api/tasks")]
public class TodoTasksController : ControllerBase
{
    private readonly IAddNewTaskController _addNewTaskController;
    private readonly IUpdateTaskController _updateTaskController;
    private readonly IFindTaskByIdController _findTaskByIdController;
    private readonly IDeleteTaskController _deleteTaskController;
    private readonly IGetTaskEnumsItemsController _getTaskEnumsItemsController;

    public TodoTasksController(
        IAddNewTaskController addNewTaskController,
        IUpdateTaskController updateTaskController,
        IFindTaskByIdController findTaskByIdController,
	    IDeleteTaskController deleteTaskController,
	    IGetTaskEnumsItemsController getTaskEnumsItemsController)
    {
        _addNewTaskController = addNewTaskController;
        _updateTaskController = updateTaskController;
        _findTaskByIdController = findTaskByIdController;
        _deleteTaskController = deleteTaskController;
	    _getTaskEnumsItemsController = getTaskEnumsItemsController;
    }

    [HttpGet("enums")]
    public async Task<ActionResult<UseCases.GetTaskEnumsItems.OutputDto>> GetEnums()
    {
        UseCases.GetTaskEnumsItems.OutputDto result = await _getTaskEnumsItemsController.RunUseCase();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UseCases.FindTaskById.OutputDto>> GetTask(Guid id)
    {
	    UseCases.FindTaskById.OutputDto result = await _findTaskByIdController.RunUseCase(new UseCases.FindTaskById.InputDto { Id = id });
	    return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<UseCases.AddNewTask.OutputDto>> CreateTask([FromBody] UseCases.AddNewTask.InputDto input)
    {
        UseCases.AddNewTask.OutputDto result = await _addNewTaskController.RunUseCase(input);
        return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UseCases.UpdateTask.InputDto input)
    {
        input.Id = id;
        await _updateTaskController.RunUseCase(input);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        await _deleteTaskController.RunUseCase(new UseCases.DeleteTask.InputDto { Id = id });
        return NoContent();
    }
}
