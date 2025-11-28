using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class UpdateCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Delegate_to_character_service(string name)
    {
        var routeId = Guid.NewGuid();
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        var expectedCharacter = new Character(new CharacterId(routeId), name);
        A.CallTo(() => service.Update(A<UpdateCharacterCommand>._)).Returns(expectedCharacter);

        var request = new UpdateCharacterInput(routeId, routeId.ToString(), name);
        var result = await updateCharacter.Execute(request);

        A.CallTo(() => service.Update(A<UpdateCharacterCommand>.That.Matches(command =>
                command.Id.Value == routeId &&
                command.Name == name)))
            .MustHaveHappenedOnceExactly();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(routeId.ToString()));
            Assert.That(result.Name, Is.EqualTo(name));
        });
    }

    [Test]
    public async Task Return_null_when_service_returns_null()
    {
        var routeId = Guid.NewGuid();
        var request = new UpdateCharacterInput(routeId, routeId.ToString(), "New Name");
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        A.CallTo(() => service.Update(A<UpdateCharacterCommand>._)).Returns(Task.FromResult<Character?>(null));

        var result = await updateCharacter.Execute(request);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Throw_when_payload_identifier_is_missing()
    {
        var routeId = Guid.NewGuid();
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        var request = new UpdateCharacterInput(routeId, null, "Aragorn");

        Assert.That(
            () => updateCharacter.Execute(request),
            Throws.ArgumentException.With.Message.Contain("payload id"));
    }

    [Test]
    public void Throw_when_route_and_payload_identifiers_differ()
    {
        var service = A.Fake<ICharacterService>();
        var updateCharacter = new UpdateCharacter(service);
        var routeId = Guid.NewGuid();
        var mismatchedId = Guid.NewGuid().ToString();
        var request = new UpdateCharacterInput(routeId, mismatchedId, "Boromir");

        Assert.That(
            () => updateCharacter.Execute(request),
            Throws.ArgumentException.With.Message.Contain("match the route"));
    }
}
