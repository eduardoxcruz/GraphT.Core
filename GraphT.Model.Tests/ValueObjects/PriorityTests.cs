using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class PriorityTests
{
	[Fact]
	public void Priority_ShouldHave_DistractionAsDefaultOption()
	{
		Priority sut = new();
		
		Assert.Equal(Priority.Distraction, sut);
	}

	[Fact]
	public void Priority_ShouldHave_ConsiderAsSelectableOption()
	{
		Assert.Equal(1, Priority.Consider.Index);
		Assert.Equal("Consider", Priority.Consider.Name);
		Assert.Equal("\ud83e\uddcd\u200d\u2642\ufe0f", Priority.Consider.Emoji);
	}
	
	[Fact]
	public void Priority_ShouldHave_IndexForEveryOption()
	{
		Assert.Equal(0, Priority.Distraction.Index);
		Assert.Equal(1, Priority.Consider.Index);
		Assert.Equal(2, Priority.Urgent.Index);
		Assert.Equal(3, Priority.Critical.Index);
	}

	[Fact]
	public void Priority_ShouldHave_UrgentAsSelectableOption()
	{
		Assert.Equal(2, Priority.Urgent.Index);
		Assert.Equal("Urgent", Priority.Urgent.Name);
		Assert.Equal("\ud83c\udfc3\u200d\u2642\ufe0f", Priority.Urgent.Emoji);
	}

	[Fact]
	public void Priority_ShouldHave_CriticalAsSelectableOption()
	{
		Assert.Equal(3, Priority.Critical.Index);
		Assert.Equal("Critical", Priority.Critical.Name);
		Assert.Equal("\ud83d\udeb4\u200d\u2642\ufe0f", Priority.Critical.Emoji);
	}

	[Fact]
	public void Priority_ShouldHave_NameForEveryOption()
	{
		Assert.Equal("Distraction", Priority.Distraction.Name);
		Assert.Equal("Consider", Priority.Consider.Name);
		Assert.Equal("Urgent", Priority.Urgent.Name);
		Assert.Equal("Critical", Priority.Critical.Name);
	}
	
	[Fact]
	public void Priority_ShouldHave_EmojiForEveryOption()
	{
		Assert.Equal("\ud83d\udecc", Priority.Distraction.Emoji);
		Assert.Equal("\ud83e\uddcd\u200d\u2642\ufe0f", Priority.Consider.Emoji);
		Assert.Equal("\ud83c\udfc3\u200d\u2642\ufe0f", Priority.Urgent.Emoji);
		Assert.Equal("\ud83d\udeb4\u200d\u2642\ufe0f", Priority.Critical.Emoji);
	}
}
