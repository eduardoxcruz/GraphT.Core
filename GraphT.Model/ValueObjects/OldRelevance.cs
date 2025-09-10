using GraphT.Model.ValueObjects.EnumLabel;

namespace GraphT.Model.ValueObjects;

public enum OldRelevance { 
	[EnumLabel("\ud83d\ude12 Superfluous")]
	Superfluous = 0,
	[EnumLabel("\ud83e\udd24 Entertaining")]
	Entertaining = 1,
	[EnumLabel("\ud83e\uddd0 Necessary")]
	Necessary = 2,
	[EnumLabel("\ud83d\ude0e Purposeful")]
	Purposeful = 3 
}
