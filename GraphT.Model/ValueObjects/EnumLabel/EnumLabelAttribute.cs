namespace GraphT.Model.ValueObjects.EnumLabel;

[AttributeUsage(AttributeTargets.Field)]
public class EnumLabelAttribute : Attribute
{
	public string Label { get; }

	public EnumLabelAttribute(string label)
	{
		Label = label;
	}
}
