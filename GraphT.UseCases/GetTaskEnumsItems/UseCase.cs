using System.Numerics;

using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.GetTaskEnumsItems;

public interface IInputPort : IPort { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}
	
	public async ValueTask Handle()
	{
		int item;
		string label;
		List<EnumItemAndLabel> complexityItems = new();
		Array complexityEnumItems = Enum.GetValues(typeof(Complexity));
		
		foreach (Complexity complexity in complexityEnumItems)
		{
			item = (int)complexity;
			label = complexity.GetLabel();
			EnumItemAndLabel enumItemAndLabel = new(item, label);
			complexityItems.Add(enumItemAndLabel);
		}

		List<EnumItemAndLabel> priorityItems = new();
		Array priorityEnumItems = Enum.GetValues(typeof(Priority));
		
		foreach (Priority priority in priorityEnumItems)
		{
			item = (int)priority;
			label = priority.GetLabel();
			EnumItemAndLabel enumItemAndLabel = new(item, label);
			priorityItems.Add(enumItemAndLabel);
		}
		
		List<EnumItemAndLabel> relevanceItems = new();
		Array relevanceEnumItems = Enum.GetValues(typeof(Relevance));
		
		foreach (Relevance relevance in relevanceEnumItems)
		{
			item = (int)relevance;
			label = relevance.GetLabel();
			EnumItemAndLabel enumItemAndLabel = new(item, label);
			relevanceItems.Add(enumItemAndLabel);
		}
		
		List<EnumItemAndLabel> statusItems = new();
		Array statusEnumItems = Enum.GetValues(typeof(Status));
		
		foreach (Status status in statusEnumItems)
		{
			item = (int)status;
			label = status.GetLabel();
			EnumItemAndLabel enumItemAndLabel = new(item, label);
			statusItems.Add(enumItemAndLabel);
		}

		List<List<EnumItemAndLabel>> items = new();
		items.Add(complexityItems);
		items.Add(priorityItems);
		items.Add(relevanceItems);
		items.Add(statusItems);
		
		await _outputPort.Handle(new OutputDto(){ Items = items });
	}
}

public class OutputDto
{
	public List<List<EnumItemAndLabel>> Items { get; set; }
}

