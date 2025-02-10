using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Presenters;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTPresenters(this IServiceCollection services)
	{
		services.AddScoped<UseCases.UpdateTask.IOutputPort, UpdateTask.Presenter>();
		services.AddScoped<UseCases.AddNewTask.IOutputPort, AddNewTask.Presenter>();
		services.AddScoped<UseCases.FindTaskById.IOutputPort, FindTaskById.Presenter>();
		services.AddScoped<UseCases.FindTaskUpstreamsById.IOutputPort, FindTaskUpstreamsById.Presenter>();
		services.AddScoped<UseCases.FindReadyToStartTasks.IOutputPort, FindReadyToStartTasks.Presenter>();
		services.AddScoped<UseCases.FindTaskDownstreamsById.IOutputPort, FindTaskDownstreamsById.Presenter>();
		services.AddScoped<UseCases.FindTaskLifeAreasById.IOutputPort, FindTaskLifeAreasById.Presenter>();
		services.AddScoped<UseCases.FindInProgressTasks.IOutputPort, FindInProgressTasks.Presenter>();
		services.AddScoped<UseCases.FindFinishedTasks.IOutputPort, FindFinishedTasks.Presenter>();
		services.AddScoped<UseCases.DeleteTask.IOutputPort, DeleteTask.Presenter>();
		services.AddScoped<UseCases.GetTaskEnumsItems.IOutputPort, GetTaskEnumsItems.Presenter>();

		return services;
	}
}
