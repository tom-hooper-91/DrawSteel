using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class UpdateCharacterShould
{
    // T014: Delegate to character service
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Delegate_to_character_service(string name)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, name);
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        var expectedCharacter = new Character(characterId, name);
        A.CallTo(() => service.Update(A<UpdateCharacterCommand>._)).Returns(expectedCharacter);

        var result = await updateCharacter.Execute(command);

        A.CallTo(() => service.Update(command)).MustHaveHappenedOnceExactly();
        Assert.That(result, Is.EqualTo(expectedCharacter));
    }

    // T015: Return null when service returns null
    [Test]
    public async Task Return_null_when_service_returns_null()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, "New Name");
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        A.CallTo(() => service.Update(A<UpdateCharacterCommand>._)).Returns(Task.FromResult<Character>(null!));

        var result = await updateCharacter.Execute(command);

        Assert.That(result, Is.Null);
    }
}
