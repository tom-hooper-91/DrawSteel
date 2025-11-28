using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class GetCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Return_character_dto_with_flat_id(string name)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var service = A.Fake<ICharacterService>();
        var getCharacter = new GetCharacter(service);
        var expectedCharacter = new Character(characterId, name);
        A.CallTo(() => service.Get(characterId)).Returns(expectedCharacter);

        var dto = await getCharacter.Execute(characterId);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto!.Id, Is.EqualTo(characterId.Value.ToString()));
            Assert.That(dto.Name, Is.EqualTo(name));
        });
    }
}