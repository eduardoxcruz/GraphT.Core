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
		var items = new List<List<EnumItemAndLabel>>
		{
			ConvertEnumToItems<Complexity>(),
			ConvertEnumToItems<Priority>(),
			ConvertEnumToItems<Relevance>(),
			ConvertEnumToItems<Status>()
		};

		await _outputPort.Handle(new OutputDto { Items = items });
	}

	private static List<EnumItemAndLabel> ConvertEnumToItems<TEnum>() 
		where TEnum : struct, Enum
	{
		return Enum.GetValues<TEnum>()
			.Select(e => new EnumItemAndLabel(Convert.ToInt32(e), e.GetLabel()))
			.ToList();
	}
}

public class OutputDto
{
	public List<List<EnumItemAndLabel>> Items { get; set; }
}

