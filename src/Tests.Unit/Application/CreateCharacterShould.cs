using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

public class CreateCharacterShould
{
    [TestCaseSource(nameof(Sources))]
    public async Task Call_CharacterService_Create_with_the_correct_CreateCharacterCommand_and_return_CharacterID(string name, CharacterClass characterClass, CreateCharacterCommand expectedCommand)
    {
        var request = new CreateCharacterRequest(name, characterClass);
        var command = new CreateWarriorCommand(name);
        var service = A.Fake<ICharacterService>();
        var createCharacter = new CreateCharacter(service);
        var expectedId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => service.Create(expectedCommand)).Returns(expectedId);

        var characterId = await createCharacter.Execute(request);

        A.CallTo(() => service.Create(command)).MustHaveHappenedOnceExactly();
        Assert.That(characterId, Is.EqualTo(expectedId));
    }

    public static object[] Sources =
    [
        new object[] { "Frodo", CharacterClass.Warrior, A<CreateWarriorCommand>._ }, // solve this record shenanigans
        new object[] { "Sam", CharacterClass.Gardener, A<CreateGardenerCommand>._ }
    ];
}