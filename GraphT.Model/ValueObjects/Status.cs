using GraphT.Model.ValueObjects.EnumLabel;

namespace GraphT.Model.ValueObjects;

public enum Status
{
	[EnumLabel("\u270c Created")]
	Created = 0,
	[EnumLabel("\ud83d\udcdd Backlog")]
	Backlog = 1,
	[EnumLabel("\ud83d\udc4c Ready To Start")]
	Ready = 2,
	[EnumLabel("\u25b6 Currently Doing")]
	Doing = 3,
	[EnumLabel("\u23f8 Paused")]
	Paused = 4,
	[EnumLabel("\ud83d\uddd1 Dropped")]
	Dropped = 5,
	[EnumLabel("\u2705 Completed")]
	Completed = 6
}
