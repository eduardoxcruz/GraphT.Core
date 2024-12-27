using GraphT.Controllers.GetFinishedOrUnfinishedTasks;
using GraphT.Controllers.UpdateTask;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Controllers;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTControllers(this IServiceCollection services)
	{
		services.AddScoped<IGetFinishedTasksController, GetFinishedTasksController>();
		services.AddScoped<IGetUnfinishedTasksController, GetUnfinishedTasksController>();
		services.AddScoped<IUpdateTaskController, UpdateTaskController>();
		
		return services;
	}
}
