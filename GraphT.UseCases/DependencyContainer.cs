using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
	{
		services.AddScoped<AddNewTask.IInputPort, AddNewTask.UseCase>();
		services.AddScoped<UpdateTask.IInputPort, UpdateTask.UseCase>();
		services.AddScoped<FindTaskUpstreamsById.IInputPort, FindTaskUpstreamsById.UseCase>();
		services.AddScoped<FindTaskById.IInputPort, FindTaskById.UseCase>();

		return services;
	}
}
