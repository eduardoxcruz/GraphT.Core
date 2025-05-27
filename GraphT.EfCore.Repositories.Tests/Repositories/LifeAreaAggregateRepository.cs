namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class LifeAreaAggregateRepository : IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public LifeAreaAggregateRepository(TestDatabaseFixture fixture) => Fixture = fixture;
}
