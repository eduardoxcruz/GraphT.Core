using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Presenters;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTPresenters(this IServiceCollection services)
	{
		services.AddScoped<UseCases.GetFinishedTasks.IOutputPort, GetFinishedTasks.Presenter>();
		services.AddScoped<UseCases.FindUnfinishedTasksByName.IOutputPort, FindUnfinishedTasksByName.Presenter>();
		services.AddScoped<UseCases.UpdateTask.IOutputPort, UpdateTask.Presenter>();
		services.AddScoped<UseCases.AddNewTask.IOutputPort, AddNewTask.Presenter>();
		return services;
	}
}
