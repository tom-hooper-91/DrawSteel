using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class GetCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Call_CharacterService_Get_and_return_a_Character(string name)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var service = A.Fake<ICharacterService>();
        var getCharacter = new GetCharacter(service);
        var expectedCharacter = new Character(characterId, name);
        A.CallTo(() => service.Get(characterId)).Returns(expectedCharacter);

        var character = await getCharacter.Execute(characterId);

        Assert.That(character, Is.EqualTo(expectedCharacter));
    }
}