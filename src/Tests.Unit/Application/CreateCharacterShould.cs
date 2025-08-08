using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class CreateCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Call_CharacterService_Create_and_return_CharacterID(string name)
    {
        var character = new CreateCharacterCommand(name);
        var service = A.Fake<ICharacterService>();
        var createCharacter = new CreateCharacter(service);
        var expectedId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => service.Create(A<CreateCharacterCommand>._)).Returns(expectedId);

        var characterId = await createCharacter.Execute(character);

        A.CallTo(() => service.Create(character)).MustHaveHappenedOnceExactly();
        Assert.That(characterId, Is.EqualTo(expectedId));
    }
}