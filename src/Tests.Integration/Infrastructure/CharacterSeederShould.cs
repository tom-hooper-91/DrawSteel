using Infrastructure.Data;
using Microsoft.Azure.Cosmos;

namespace Tests.Integration.Infrastructure;

[TestFixture]
public class CharacterSeederShould
{
    [Test]
    public async Task Create_Characters_container()
    {
        var seeder = new CharacterSeeder(Fixture.Client);
        
        await seeder.SeedDatabase();
        var database = Fixture.Client.GetDatabase("drawsteel");
        
        Assert.That(database.GetContainer("characters"), Is.Not.Null);
    }
}