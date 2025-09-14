using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class StatusTests
{
	[Fact]
	public void Status_ShouldHave_CreatedAsDefaultOption()
	{
		Assert.Equal(Status.Created, new Status());
		Assert.NotNull(typeof(Status).GetField("Created"));
	}

	[Fact]
	public void Status_ShouldHave_BacklogAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("Backlog"));
	}

	[Fact]
	public void Status_ShouldHave_ReadyToStartAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("ReadyToStart"));
	}

	[Fact]
	public void Status_ShouldHave_CurrentlyDoingAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("CurrentlyDoing"));
	}

	[Fact]
	public void Status_ShouldHave_PausedAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("Paused"));
	}

	[Fact]
	public void Status_ShouldHave_DroppedAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("Dropped"));
	}

	[Fact]
	public void Status_ShouldHave_CompletedAsSelectableOption()
	{
		Assert.NotNull(typeof(Status).GetField("Completed"));
	}

	[Fact]
	public void Status_ShouldHave_IndexForEveryOption()
	{
		Assert.Equal(0, Status.Created.Index);
		Assert.Equal(1, Status.Backlog.Index);
		Assert.Equal(2, Status.ReadyToStart.Index);
		Assert.Equal(3, Status.CurrentlyDoing.Index);
		Assert.Equal(4, Status.Paused.Index);
		Assert.Equal(5, Status.Dropped.Index);
		Assert.Equal(6, Status.Completed.Index);
	}
	
	[Fact]
	public void Status_ShouldHave_NameForEveryOption()
	{
		Assert.Equal("Created", Status.Created.Name);
		Assert.Equal("Backlog", Status.Backlog.Name);
		Assert.Equal("Ready To Start", Status.ReadyToStart.Name);
		Assert.Equal("Currently Doing", Status.CurrentlyDoing.Name);
		Assert.Equal("Paused", Status.Paused.Name);
		Assert.Equal("Dropped", Status.Dropped.Name);
		Assert.Equal("Completed", Status.Completed.Name);
	}
	[Fact]
	public void Status_ShouldHave_EmojiForEveryOption()
	{
		Assert.Equal("\u270c", Status.Created.Emoji);
		Assert.Equal("\ud83d\udcdd", Status.Backlog.Emoji);
		Assert.Equal("\ud83d\udc4c", Status.ReadyToStart.Emoji);
		Assert.Equal("\u25b6", Status.CurrentlyDoing.Emoji);
		Assert.Equal("\u23f8", Status.Paused.Emoji);
		Assert.Equal("\ud83d\uddd1", Status.Dropped.Emoji);
		Assert.Equal("\u2705", Status.Completed.Emoji);
	}
}
