using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.GetFinishedTasks;
using GraphT.Controllers.UpdateTask;

using Microsoft.Extensions.DependencyInjection;

using Controller = GraphT.Controllers.GetFinishedTasks.Controller;

namespace GraphT.Controllers;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTControllers(this IServiceCollection services)
	{
		services.AddScoped<IGetFinishedTasksController, Controller>();
		services.AddScoped<IUpdateTaskController, UpdateTask.Controller>();
		services.AddScoped<IAddNewTaskController, AddNewTask.Controller>();
		
		return services;
	}
}
