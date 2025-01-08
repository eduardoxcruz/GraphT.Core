using GraphT.Presenters.GetFinishedTasks;
using GraphT.UseCases.GetFinishedTasks;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Presenters;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTPresenters(this IServiceCollection services)
	{
		services.AddScoped<IOutputPort, Presenter>();
		services.AddScoped<UseCases.UpdateTask.IOutputPort, UpdateTask.Presenter>();
		services.AddScoped<UseCases.AddNewTask.IOutputPort, AddNewTask.Presenter>();
		return services;
	}
}
