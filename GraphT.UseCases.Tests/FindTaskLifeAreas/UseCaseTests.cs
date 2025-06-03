using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.FindTaskLifeAreasById;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskLifeAreas;

public class UseCaseTests
{
	[Fact]
	public async Task Handle_WhenTaskHasLifeAreas_ReturnsPagedListOfLifeAreas()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ILifeAreasRepository lifeAreasRepository = Substitute.For<ILifeAreasRepository>();

		Guid taskId = Guid.NewGuid();
		LifeArea lifeArea1 = new("Life Area 1");
		LifeArea lifeArea2 = new("Life Area 2");
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		// Configure repository mock behavior
		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		lifeAreasRepository.FindTaskLifeAreasById(taskId)
			.Returns(new PagedList<LifeArea>(
				[lifeArea1, lifeArea2], 
				2, 
				1, 
				10));

		UseCase useCase = new(outputPort, todoTaskRepository, lifeAreasRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await lifeAreasRepository.Received(1).FindTaskLifeAreasById(taskId);
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.LifeAreas.Count == 2 &&
			dto.LifeAreas.TotalCount == 2 &&
			dto.LifeAreas.CurrentPage == 1 &&
			dto.LifeAreas.PageSize == 10 &&
			dto.LifeAreas.Any(lifeArea => lifeArea.Id.Equals(lifeArea1.Id)) &&
			dto.LifeAreas.Any(lifeArea => lifeArea.Id.Equals(lifeArea2.Id))
		));
	}

	[Fact]
	public async Task Handle_WhenTaskHasNoLifeAreas_ReturnsEmptyPagedList()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ILifeAreasRepository lifeAreasRepository = Substitute.For<ILifeAreasRepository>();

		Guid taskId = Guid.NewGuid();
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		// Configure repository mock behavior
		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		lifeAreasRepository.FindTaskLifeAreasById(taskId)
			.Returns(new PagedList<LifeArea>(
				[], 
				0, 
				1, 
				10));

		UseCase useCase = new(outputPort, todoTaskRepository, lifeAreasRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await lifeAreasRepository.Received(1).FindTaskLifeAreasById(taskId);
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.LifeAreas.Count == 0 &&
			dto.LifeAreas.TotalCount == 0
		));
	}

	[Fact]
	public async Task Handle_ValidatesPagingParameters()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ILifeAreasRepository lifeAreasRepository = Substitute.For<ILifeAreasRepository>();

		Guid taskId = Guid.NewGuid();
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		// Configure repository mock behavior
		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		lifeAreasRepository.FindTaskLifeAreasById(taskId)
			.Returns(new PagedList<LifeArea>(
				[], 
				0, 
				2, 
				5));

		UseCase useCase = new(outputPort, todoTaskRepository, lifeAreasRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await lifeAreasRepository.Received(1).FindTaskLifeAreasById(taskId);
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.LifeAreas.PageSize == pagingParams.PageSize &&
			dto.LifeAreas.CurrentPage == pagingParams.PageNumber
		));
	}
}
