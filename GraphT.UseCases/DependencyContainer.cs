using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
	{
		services.AddScoped<FindUnfinishedTasksByName.IInputPort, FindUnfinishedTasksByName.UseCase>();
		services.AddScoped<FindUnfinishedTasksByName.IInputPort, FindUnfinishedTasksByName.UseCase>();
		services.AddScoped<UpdateTask.IInputPort, UpdateTask.UseCase>();

		return services;
	}
}
