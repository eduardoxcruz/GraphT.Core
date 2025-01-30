using GraphT.Controllers.FindTaskLifeAreasById;
using GraphT.UseCases.FindTaskLifeAreasById;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStreams;

[ApiController]
[Route("api/tasks/streams/life-areas")]
public class TaskLifeAreasController : ControllerBase
{
    private readonly IFindTaskLifeAreasByIdController _controller;

    public TaskLifeAreasController(IFindTaskLifeAreasByIdController controller)
    {
        _controller = controller;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutputDto>> GetLifeAreas(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            Id = id,
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}