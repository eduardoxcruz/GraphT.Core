using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class RelevanceTests
{
	[Fact]
	public void Relevance_ShouldHave_IndexForEveryOption()
	{
		Assert.Equal(0, Relevance.Superfluous.Index);
		Assert.Equal(1, Relevance.Entertaining.Index);
		Assert.Equal(2, Relevance.Necessary.Index);
		Assert.Equal(3, Relevance.Purposeful.Index);
	}
	
	[Fact]
	public void Relevance_ShouldHave_NameForEveryOption()
	{
		Assert.Equal("Superfluous", Relevance.Superfluous.Name);
		Assert.Equal("Entertaining", Relevance.Entertaining.Name);
		Assert.Equal("Necessary", Relevance.Necessary.Name);
		Assert.Equal("Purposeful", Relevance.Purposeful.Name);
	}
	
	[Fact]
	public void Relevance_ShouldHave_EmojiForEveryOption()
	{
		Assert.Equal("\ud83d\ude12", Relevance.Superfluous.Emoji);
		Assert.Equal("\ud83e\udd24", Relevance.Entertaining.Emoji);
		Assert.Equal("\ud83e\uddd0", Relevance.Necessary.Emoji);
		Assert.Equal("\ud83d\ude0e", Relevance.Purposeful.Emoji);
	}
	
	[Fact]
	public void Relevance_ShouldReflectIsFunAndIsProductive()
	{
		Assert.Equal(new Relevance(false, false), Relevance.Superfluous);
		Assert.Equal(new Relevance(true, false), Relevance.Entertaining);
		Assert.Equal(new Relevance(false, true), Relevance.Necessary);
		Assert.Equal(new Relevance(true, true), Relevance.Purposeful);
	}
}
