using GraphT.Controllers.FindFinishedTasks;
using GraphT.UseCases.FindFinishedTasks;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStatuses;

[ApiController]
[Route("api/tasks/statuses/finished")]
public class FinishedTasksController : ControllerBase
{
    private readonly IFindFinishedTasksController _controller;

    public FinishedTasksController(IFindFinishedTasksController controller)
    {
        _controller = controller;
    }

    [HttpGet]
    public async Task<ActionResult<OutputDto>> GetFinishedTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}
