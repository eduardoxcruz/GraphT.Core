using System.Text.Json;

using GraphT.Controllers.AddDownstream;
using GraphT.Controllers.AddUpstream;
using GraphT.Controllers.FindTasksWithoutUpstreams;
using GraphT.Controllers.FindTaskUpstreamsById;
using GraphT.Controllers.RemoveUpstream;

using Microsoft.AspNetCore.Mvc;

using SeedWork;

namespace GraphT.WebAPI.Controllers.TaskStreams;

[ApiController]
[Route("api/tasks/streams/upstreams")]
[Produces("application/json")]
public class TaskUpstreamsController : ControllerBase
{
	private readonly IFindTaskUpstreamsByIdController _findTaskUpstreamsByIdController;
	private readonly IFindTasksWithoutUpstreamsController _findTasksWithoutUpstreamsController;
	private readonly IAddUpstreamController _addUpstreamController;
	private readonly IRemoveUpstreamController _removeUpstreamController;

	public TaskUpstreamsController(
		IFindTaskUpstreamsByIdController findTaskUpstreamsByIdController, 
		IFindTasksWithoutUpstreamsController findTasksWithoutUpstreamsController,
		IAddUpstreamController addUpstreamController,
		IRemoveUpstreamController removeUpstreamController)
	{
		_findTaskUpstreamsByIdController = findTaskUpstreamsByIdController;
		_findTasksWithoutUpstreamsController = findTasksWithoutUpstreamsController;
		_addUpstreamController = addUpstreamController;
		_removeUpstreamController = removeUpstreamController;  
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UseCases.FindTaskUpstreamsById.OutputDto>> GetUpstreams(Guid id, [FromQuery] int pageNumber = 1
		, [FromQuery] int pageSize = 10)
	{
		UseCases.FindTaskUpstreamsById.OutputDto result = await _findTaskUpstreamsByIdController.RunUseCase(new UseCases.FindTaskUpstreamsById.InputDto
		{
			Id = id, PagingParams = new PagingParams { PageNumber = pageNumber, PageSize = pageSize }
		});
		var metadata = new
		{
			result.Upstreams.TotalCount
			, result.Upstreams.PageSize
			, result.Upstreams.CurrentPage
			, result.Upstreams.TotalPages
			, result.Upstreams.HasNext
			, result.Upstreams.HasPrevious
		};
		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

		return Ok(result);
	}

	[HttpGet("tasks-without-upstreams")]
	public async Task<ActionResult<UseCases.FindTasksWithoutUpstreams.OutputDto>> GetTasksWithoutUpstreams(
		[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
	{
		UseCases.FindTasksWithoutUpstreams.OutputDto result =
			await _findTasksWithoutUpstreamsController.RunUseCase(new UseCases.FindTasksWithoutUpstreams.InputDto
			{
				PagingParams = new PagingParams { PageNumber = pageNumber, PageSize = pageSize }
			});
		var metadata = new
		{
			result.Tasks.TotalCount
			, result.Tasks.PageSize
			, result.Tasks.CurrentPage
			, result.Tasks.TotalPages
			, result.Tasks.HasNext
			, result.Tasks.HasPrevious
		};
		Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

		return Ok(result);
	}
	
	[HttpPut("{id:guid}/add")]
	public async Task<IActionResult> AddUpstream(Guid id, [FromBody] Guid upstreamId)
	{
		UseCases.AddUpstream.InputDto input = new() { TaskId = id, UpstreamId = upstreamId};
		await _addUpstreamController.RunUseCase(input);
		return NoContent();
	}
	
	[HttpPut("{id:guid}/remove")]
	public async Task<IActionResult> RemoveUpstream(Guid id, [FromBody] Guid upstreamId)
	{
		UseCases.RemoveUpstream.InputDto input = new() { TaskId = id, UpstreamId = upstreamId};
		await _removeUpstreamController.RunUseCase(input);
		return NoContent();
	}
}
