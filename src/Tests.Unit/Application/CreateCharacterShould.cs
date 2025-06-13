using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class CreateCharacterShould
{
    private CreateCharacter _createCharacter = null!;
    private ICharacterFactory _factory;
    private ISaveCharacter _save;

    [SetUp]
    public void Setup()
    {
        _factory = A.Fake<ICharacterFactory>();
        _save = A.Fake<ISaveCharacter>();
        _createCharacter = new CreateCharacter(_factory, _save);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_CharacterFactory(string name)
    {
        var newCharacter = new CreateCharacterCommand(name);

        _ = _createCharacter.Execute(newCharacter);

        A.CallTo(() => _factory.Create(newCharacter)).MustHaveHappenedOnceExactly();
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_SaveCharacter(string name)
    {
        var newCharacter = new CreateCharacterCommand(name);
        var character = new Character(newCharacter.Name);
        A.CallTo(() => _factory.Create(newCharacter)).Returns(character);

        _ = _createCharacter.Execute(newCharacter);

        A.CallTo(() => _save.This(character)).MustHaveHappenedOnceExactly();
    }
}