using GraphT.Controllers.AddNewTask;
using GraphT.Controllers.FindTaskById;
using GraphT.Controllers.UpdateTask;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Controllers;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTControllers(this IServiceCollection services)
	{
		services.AddScoped<IUpdateTaskController, UpdateTask.Controller>();
		services.AddScoped<IAddNewTaskController, AddNewTask.Controller>();
		services.AddScoped<IFindTaskByIdController, FindTaskById.Controller>();
		
		return services;
	}
}
