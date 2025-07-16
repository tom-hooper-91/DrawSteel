using Domain;
using Domain.Repositories;
using FakeItEasy;

namespace Tests.Domain;

[TestFixture]
public class CharacterServiceShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Add_a_Character_to_the_Repository_and_return_CharacterId(string name)
    {
        var expectedId = new CharacterId(Guid.NewGuid());
        var character = new Character(expectedId, name);
        var repository = A.Fake<ICharacterRepository>();
        var service = new CharacterService(repository);
        A.CallTo(() => repository.Add(A<Character>.That.Matches(c => c.Name.Equals(character.Name))))
            .Returns(Task.FromResult(expectedId));

        var characterId = await service.Create(new CreateCharacterCommand(name));

        Assert.That(characterId, Is.EqualTo(expectedId));
    }
}