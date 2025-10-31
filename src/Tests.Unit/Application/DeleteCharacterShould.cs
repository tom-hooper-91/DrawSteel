using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class DeleteCharacterShould
{
    // T034: Delegate to character service
    [Test]
    public async Task Delegate_to_character_service()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var service = A.Fake<ICharacterService>();
        var deleteCharacter = new DeleteCharacter(service);
        A.CallTo(() => service.Delete(A<CharacterId>._)).Returns(true);

        var result = await deleteCharacter.Execute(characterId);

        A.CallTo(() => service.Delete(characterId)).MustHaveHappenedOnceExactly();
        Assert.That(result, Is.True);
    }

    // T035: Return false when service returns false
    [Test]
    public async Task Return_false_when_service_returns_false()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var service = A.Fake<ICharacterService>();
        var deleteCharacter = new DeleteCharacter(service);
        A.CallTo(() => service.Delete(A<CharacterId>._)).Returns(false);

        var result = await deleteCharacter.Execute(characterId);

        Assert.That(result, Is.False);
    }
}
