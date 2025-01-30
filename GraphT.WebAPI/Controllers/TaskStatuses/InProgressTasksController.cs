using GraphT.Controllers.FindInProgressTasks;
using GraphT.UseCases.FindInProgressTasks;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStatuses;

[ApiController]
[Route("api/tasks/statuses/in-progress")]
public class InProgressTasksController : ControllerBase
{
    private readonly IFindInProgressTasksController _controller;

    public InProgressTasksController(IFindInProgressTasksController controller)
    {
        _controller = controller;
    }

    [HttpGet]
    public async Task<ActionResult<OutputDto>> GetInProgressTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}