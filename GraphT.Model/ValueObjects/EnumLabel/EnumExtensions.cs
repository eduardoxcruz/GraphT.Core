using System.Reflection;

namespace GraphT.Model.ValueObjects.EnumLabel;

public static class EnumExtensions
{
	public static string GetLabel(this Enum value)
	{
		FieldInfo? field = value.GetType().GetField(value.ToString());
		EnumLabelAttribute? attribute = field?.GetCustomAttribute<EnumLabelAttribute>();
        
		return attribute?.Label ?? value.ToString();
	}
}
