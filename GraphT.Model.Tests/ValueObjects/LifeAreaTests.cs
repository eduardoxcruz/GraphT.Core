using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class LifeAreaTests
{
	[Fact]
	public void LifeArea_ShouldHave_Name_WhenCreated()
	{
		string expected = "Name";

		ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new LifeArea());
		
		Assert.Equal("Can not create Life Area without name. Use constructor with param instead. (Parameter 'Name')", ex.Message);
		Assert.Contains(expected,typeof(LifeArea).GetProperties().Select(p => p.Name));  
		Assert.True(typeof(LifeArea).GetProperty(expected).PropertyType == typeof(string));
	}

	[Fact]
	public void LifeArea_ShouldBeEquatable_ViaName()
	{
		Assert.Equal(new LifeArea("Name"), new LifeArea("Name"));
		Assert.NotEqual(new LifeArea("Name"), new LifeArea("Name2"));
	}
}
