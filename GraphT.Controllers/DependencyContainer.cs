using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.DeleteTask;
using GraphT.Controllers.FindFinishedTasks;
using GraphT.Controllers.FindInProgressTasks;
using GraphT.Controllers.FindReadyToStartTasks;
using GraphT.Controllers.FindTaskById;
using GraphT.Controllers.FindTaskDownstreamsById;
using GraphT.Controllers.FindTaskLifeAreasById;
using GraphT.Controllers.FindTasksWithoutUpstreams;
using GraphT.Controllers.UpdateTask;
using GraphT.Controllers.FindTaskUpstreamsById;
using GraphT.Controllers.GetTaskEnumsItems;

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
		services.AddScoped<IFindTaskDownstreamsByIdController, FindTaskDownstreamsById.Controller>();
		services.AddScoped<IFindTaskLifeAreasByIdController, FindTaskLifeAreasById.Controller>();
		services.AddScoped<IFindInProgressTasksController, FindInProgressTasks.Controller>();
		services.AddScoped<IFindFinishedTasksController, FindFinishedTasks.Controller>();
		services.AddScoped<IDeleteTaskController, DeleteTask.Controller>();
		services.AddScoped<IGetTaskEnumsItemsController, GetTaskEnumsItems.Controller>();
		services.AddScoped<IFindTasksWithoutUpstreamsController, FindTasksWithoutUpstreams.Controller>();

		return services;
	}
}
