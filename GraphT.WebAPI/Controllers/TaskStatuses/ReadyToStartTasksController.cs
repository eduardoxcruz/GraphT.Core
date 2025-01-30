using GraphT.Controllers.FindReadyToStartTasks;
using GraphT.UseCases.FindReadyToStartTasks;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStatuses;

[ApiController]
[Route("api/tasks/statuses/ready-to-start")]
public class ReadyToStartTasksController : ControllerBase
{
    private readonly IFindReadyToStartTasksController _controller;

    public ReadyToStartTasksController(IFindReadyToStartTasksController controller)
    {
        _controller = controller;
    }

    [HttpGet]
    public async Task<ActionResult<OutputDto>> GetReadyToStartTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}