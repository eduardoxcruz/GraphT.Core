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

		return services;
	}
}
