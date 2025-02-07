namespace GraphT.UseCases;

public class EnumItemAndLabel
{
	public int EnumItem { get; set; }
	public string EnumLabel { get; set; }

	public EnumItemAndLabel(int enumItem, string enumLabel)
	{
		EnumItem = enumItem;
		EnumLabel = enumLabel;
	}
}
