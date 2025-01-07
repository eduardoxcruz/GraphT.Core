using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.FindUnfinishedTasksByName;
using GraphT.Controllers.GetFinishedTasks;
using GraphT.Controllers.UpdateTask;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Controllers;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTControllers(this IServiceCollection services)
	{
		services.AddScoped<IGetFinishedTasksController, GetFinishedTasks.Controller>();
		services.AddScoped<IFindUnfinishedTasksByNameController, FindUnfinishedTasksByName.Controller>();
		services.AddScoped<IUpdateTaskController, UpdateTask.Controller>();
		services.AddScoped<IAddNewTaskController, AddNewTask.Controller>();
		
		return services;
	}
}
