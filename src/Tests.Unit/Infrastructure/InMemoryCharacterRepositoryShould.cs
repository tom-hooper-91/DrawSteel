using Domain;
using Infrastructure;

namespace Tests.Infrastructure;

[TestFixture]
public class InMemoryCharacterRepositoryShould
{
    [Test]
    public async Task Get_a_character_from_query()
    {
        var character = new Character("Frodo");
        var repository = new InMemoryCharacterRepository();
        var characterId = character.Id;
        
        await repository.Add(character);
        
        Assert.That(await repository.Get(characterId), Is.EqualTo(character));
    }
}