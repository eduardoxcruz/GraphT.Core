using GraphT.Model.ValueObjects.EnumLabel;

namespace GraphT.Model.ValueObjects;

public enum Complexity
{
	[EnumLabel("\ud83e\udd37 Indefinite")]
	Indefinite = 0,
	[EnumLabel("\ud83e\udd71 Low")]
	Low = 1,
	[EnumLabel("\ud83d\ude31 High")]
	High = 2
}
