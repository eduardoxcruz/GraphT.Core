using GraphT.Model.ValueObjects;
using GraphT.Model.ValueObjects.EnumLabel;
using GraphT.UseCases.GetTaskEnumsItems;

using NSubstitute;

namespace GraphT.UseCases.Tests.GetTaskEnumsItems;

public class UseCaseTests
{
	[Fact]
    public async Task Handle_ReturnsEnumListsWithLabels()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        UseCase useCase = new(outputPort);

        // Act
        await useCase.Handle();

        // Assert
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto =>
			dto.Items.All(keyPairValue => keyPairValue.Value.Count != 0) &&
            ValidateEnumContents(dto)
        ));
    }

    private static bool ValidateEnumContents(OutputDto dto)
    {
        // Get enum lists
        List<EnumItemAndLabel> complexities = dto.Items["complexities"];
        List<EnumItemAndLabel> priorities = dto.Items["priorities"];
        List<EnumItemAndLabel> relevances = dto.Items["relevances"];
        List<EnumItemAndLabel> statuses = dto.Items["statuses"];

        // Verify that each list has the correct number of elements
        if (complexities.Count != Enum.GetValues<OldComplexity>().Length ||
            priorities.Count != Enum.GetValues<OldPriority>().Length ||
            relevances.Count != Enum.GetValues<OldRelevance>().Length ||
            statuses.Count != Enum.GetValues<Status>().Length)
            return false;

        // Verify that IDs and labels are valid for each enum
        return ValidateEnumItems<OldComplexity>(complexities) &&
               ValidateEnumItems<OldPriority>(priorities) &&
               ValidateEnumItems<OldRelevance>(relevances) &&
               ValidateEnumItems<Status>(statuses);
    }

    private static bool ValidateEnumItems<TEnum>(List<EnumItemAndLabel> items) where TEnum : struct, Enum
    {
        TEnum[] enumValues = Enum.GetValues<TEnum>();

        return (from enumValue in enumValues let id = Convert.ToUInt32(enumValue) select enumValue.GetLabel())
	        .All(expectedLabel => items.Any(item => item.Label == expectedLabel));
    }
}
