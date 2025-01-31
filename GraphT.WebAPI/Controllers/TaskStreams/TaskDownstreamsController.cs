using System.Text.Json;

using GraphT.Controllers.FindTaskDownstreamsById;
using GraphT.UseCases.FindTaskDownstreamsById;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStreams;

[ApiController]
[Route("api/tasks/streams/downstreams")]
public class TaskDownstreamsController : ControllerBase
{
    private readonly IFindTaskDownstreamsByIdController _controller;

    public TaskDownstreamsController(IFindTaskDownstreamsByIdController controller)
    {
        _controller = controller;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutputDto>> GetDownstreams(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            Id = id,
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        var metadata = new
	    {
		    result.Downstreams.TotalCount,
		    result.Downstreams.PageSize,
		    result.Downstreams.CurrentPage,
		    result.Downstreams.TotalPages,
		    result.Downstreams.HasNext,
		    result.Downstreams.HasPrevious
	    };
	    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(result);
    }
}