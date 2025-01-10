using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.FindReadyToStartTasks;
using GraphT.Controllers.FindTaskById;
using GraphT.Controllers.UpdateTask;
using GraphT.Controllers.FindTaskUpstreamsById;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Controllers;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTControllers(this IServiceCollection services)
	{
		services.AddScoped<IUpdateTaskController, UpdateTask.Controller>();
		services.AddScoped<IAddNewTaskController, AddNewTask.Controller>();
		services.AddScoped<IFindTaskByIdController, FindTaskById.Controller>();
		services.AddScoped<IFindTaskUpstreamsByIdController, FindTaskUpstreamsById.Controller>();
		services.AddScoped<IFindReadyToStartTasksController, FindReadyToStartTasks.Controller>();

		return services;
	}
}
