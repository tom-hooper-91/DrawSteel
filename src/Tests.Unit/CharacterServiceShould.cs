using Domain;
using Domain.Repositories;
using FakeItEasy;

namespace Tests;

[TestFixture]
public class CharacterServiceShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Add_a_Character_to_the_Repository_and_return_CharacterId(string name)
    {
        var character = new Character(name);
        var repository = A.Fake<ICharacterRepository>();
        var service = new CharacterService(repository);

        var characterId = await service.Create(new CreateCharacterCommand(name));

        A.CallTo(() => repository.Add(A<Character>.That.Matches(c => c.Name.Equals(character.Name))))
            .MustHaveHappenedOnceExactly();
        Assert.That(characterId, Is.TypeOf<CharacterId>());
    }
}