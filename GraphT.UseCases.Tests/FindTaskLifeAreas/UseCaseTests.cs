using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
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
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		LifeArea lifeArea1 = new("Life Area 1");
		LifeArea lifeArea2 = new("Life Area 2");
		testTask.AddLifeAreas([lifeArea1, lifeArea2]);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeLifeAreasSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<TaskIncludeLifeAreasSpecification>());
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
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeLifeAreasSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<TaskIncludeLifeAreasSpecification>());
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
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeLifeAreasSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 2, 5));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Is<TaskIncludeLifeAreasSpecification>(spec =>
			spec.PageNumber == pagingParams.PageNumber &&
			spec.PageSize == pagingParams.PageSize
		));
	}
}
