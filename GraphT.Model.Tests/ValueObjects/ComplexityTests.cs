using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class ComplexityTests
{
	[Fact]
	public void Complexity_ShouldHave_IndefiniteAsDefaultOption()
	{
		// Arrange
		Complexity sut = new();

		Assert.Equal(0, sut.Index);
		Assert.Equal("Indefinite", sut.Name);
		Assert.Equal("\ud83e\udd37", sut.Emoji);
	}
	
	[Fact]
	public void Complexity_ShouldHave_LowAsSelectableOption()
	{
		Assert.Equal(1, Complexity.Low.Index);
		Assert.Equal("Low", Complexity.Low.Name);
		Assert.Equal("\ud83e\udd71", Complexity.Low.Emoji);
	}
	
	[Fact]
	public void Complexity_ShouldHave_HighAsSelectableOption()
	{
		Assert.Equal(2, Complexity.High.Index);
		Assert.Equal("High", Complexity.High.Name);
		Assert.Equal("\ud83d\ude31", Complexity.High.Emoji);
	}

	[Fact]
	public void Complexity_ShouldHave_IndexForEveryOption()
	{
		Assert.Equal(0, Complexity.Indefinite.Index);
		Assert.Equal(1, Complexity.Low.Index);
		Assert.Equal(2, Complexity.High.Index);
	}

	[Fact]
	public void Complexity_ShouldHave_NameForEveryOption()
	{
		Assert.Equal("Indefinite", Complexity.Indefinite.Name);
		Assert.Equal("Low", Complexity.Low.Name);
		Assert.Equal("High", Complexity.High.Name);
	}
	
	[Fact]
	public void Complexity_ShouldHave_EmojiForEveryOption()
	{
		Assert.Equal("\ud83e\udd37", Complexity.Indefinite.Emoji);
		Assert.Equal("\ud83e\udd71", Complexity.Low.Emoji);
		Assert.Equal("\ud83d\ude31", Complexity.High.Emoji);
	}
}
