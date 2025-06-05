using System.Text.Json;

using GraphT.Controllers.AddDownstream;
using GraphT.Controllers.FindTaskDownstreamsById;
using GraphT.Controllers.RemoveDownstream;
using GraphT.UseCases.FindTaskDownstreamsById;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStreams;

[ApiController]
[Route("api/tasks/streams/downstreams")]
[Produces("application/json")]
public class TaskDownstreamsController : ControllerBase
{
    private readonly IFindTaskDownstreamsByIdController _findTaskDownstreamsByIdController;
    private readonly IAddDownstreamController _addDownstreamController;
    private readonly IRemoveDownstreamController _removeDownstreamController;

    public TaskDownstreamsController(
	    IFindTaskDownstreamsByIdController findTaskDownstreamsByIdController,
	    IAddDownstreamController addDownstreamController,
	    IRemoveDownstreamController removeDownstreamController)
    {
        _findTaskDownstreamsByIdController = findTaskDownstreamsByIdController;
        _addDownstreamController = addDownstreamController;
        _removeDownstreamController = removeDownstreamController;   
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutputDto>> GetDownstreams(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _findTaskDownstreamsByIdController.RunUseCase(new InputDto
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
    
    [HttpPut("{id:guid}/add")]
    public async Task<IActionResult> AddDownstream(Guid id, [FromBody] Guid downstreamId)
    {
	    UseCases.AddDownstream.InputDto input = new() { TaskId = id, DownstreamId = downstreamId};
	    await _addDownstreamController.RunUseCase(input);
	    return NoContent();
    }
    
    [HttpPut("{id:guid}/remove")]
    public async Task<IActionResult> RemoveDownstream(Guid id, [FromBody] Guid downstreamId)
    {
	    UseCases.RemoveDownstream.InputDto input = new() { TaskId = id, DownstreamId = downstreamId};
	    await _removeDownstreamController.RunUseCase(input);
	    return NoContent();
    }
}
