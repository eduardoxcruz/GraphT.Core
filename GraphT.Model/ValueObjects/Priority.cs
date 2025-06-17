using GraphT.Model.ValueObjects.EnumLabel;

namespace GraphT.Model.ValueObjects;

public enum Priority
{
	[EnumLabel("\ud83d\udecc Distraction")]
	Distraction = 0,
	[EnumLabel("\ud83e\uddcd\u200d\u2642\ufe0f Consider")]
	Consider = 1,
	[EnumLabel("\ud83c\udfc3\u200d\u2642\ufe0f Urgent")]
	Urgent = 2,
	[EnumLabel("\ud83d\udeb4\u200d\u2642\ufe0f Critical")]
	Critical = 3
}
