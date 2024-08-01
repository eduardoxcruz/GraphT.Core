using GraphT.Presenters.GetFinishedTasks;
using GraphT.UseCases.GetFinishedTasks;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Presenters;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTPresenters(this IServiceCollection services)
	{
		services.AddScoped<IOutputPort, Presenter>();
		services.AddScoped<UseCases.GetUnfinishedTasks.IOutputPort, GetUnfinishedTasks.Presenter>();
		return services;
	}
}
