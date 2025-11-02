using Microsoft.Extensions.DependencyInjection;

using SeedWork;

namespace GraphT.UseCases;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
	{
		services.AddScoped<IFullPort<AddNewTask.InputDto, AddNewTask.OutputDto>, AddNewTask.UseCase>();
		services.AddScoped<IPortWithInput<RemoveTask.InputDto>, RemoveTask.UseCase>();
		services.AddScoped<IFullPort<FindTaskById.InputDto, FindTaskById.OutputDto>, FindTaskById.UseCase>();
		
		return services;
	}
}
