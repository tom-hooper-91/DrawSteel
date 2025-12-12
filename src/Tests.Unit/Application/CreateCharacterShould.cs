using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

public class CreateCharacterShould
{
    [TestCaseSource(nameof(Sources))]
    public async Task Call_CharacterService_Create_with_the_correct_CreateCharacterCommand_and_return_CharacterID(string name, CharacterClass characterClass, Func<string, CreateCharacterCommand> commandFactory)
    {
        var request = new CreateCharacterRequest(name, characterClass);
        var command = commandFactory(name);
        var service = A.Fake<ICharacterService>();
        var createCharacter = new CreateCharacter(service);
        var expectedId = new CharacterId(Guid.NewGuid());
        
        A.CallTo(() => service.Create(command)).Returns(expectedId);

        var characterId = await createCharacter.Execute(request);

        Assert.That(characterId, Is.EqualTo(expectedId));
    }

    [Test]
    public void Throw_ArgumentException_when_unsupported_character_class_provided()
    {
        var request = new CreateCharacterRequest("Sauron", (CharacterClass)100);
        var service = A.Fake<ICharacterService>();
        var createCharacter = new CreateCharacter(service);
        
        Assert.ThrowsAsync<ArgumentException>(() => createCharacter.Execute(request));
    }

    private static IEnumerable<TestCaseData> Sources()
    {
        yield return new TestCaseData(
            "Frodo", 
            CharacterClass.Warrior, 
            (Func<string, CreateCharacterCommand>)(name => new CreateWarriorCommand(name)));
        yield return new TestCaseData(
            "Sam", 
            CharacterClass.Gardener, 
            (Func<string, CreateCharacterCommand>)(name => new CreateGardenerCommand(name)));
    }
}