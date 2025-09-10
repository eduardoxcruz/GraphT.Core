using GraphT.Model.ValueObjects;
using GraphT.Model.ValueObjects.EnumLabel;

using SeedWork;

namespace GraphT.UseCases.GetTaskEnumsItems;

public interface IInputPort : IPort { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;

	public UseCase(IOutputPort outputPort)
	{
		_outputPort = outputPort;
	}
	
	public async ValueTask Handle()
	{
		Dictionary<string, List<EnumItemAndLabel>> items = new()
		{
            { "complexities", ConvertEnumToItems<Complexity>() },
            { "priorities", ConvertEnumToItems<Priority>() },
            { "relevances", ConvertEnumToItems<OldRelevance>() },
            { "statuses", ConvertEnumToItems<Status>() }
        };

		await _outputPort.Handle(new OutputDto(items));
	}

	private static List<EnumItemAndLabel> ConvertEnumToItems<TEnum>() 
		where TEnum : struct, Enum
	{
		return Enum.GetValues<TEnum>()
			.Select(e => new EnumItemAndLabel(Convert.ToUInt16(e), e.GetLabel()))
			.ToList();
	}
}

public record struct OutputDto(Dictionary<string, List<EnumItemAndLabel>> Items) {}

