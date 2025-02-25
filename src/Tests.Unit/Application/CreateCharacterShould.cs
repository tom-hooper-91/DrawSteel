using Application;
using Domain;
using FakeItEasy;

namespace Tests.Application;

[TestFixture]
public class CreateCharacterShould
{
    private CreateCharacter _createCharacter = null!;
    private ICharacterFactory _characterFactory;
    private ISaveCharacter _saveCharacter;

    [SetUp]
    public void Setup()
    {
        _characterFactory = A.Fake<ICharacterFactory>();
        _saveCharacter = A.Fake<ISaveCharacter>();
        _createCharacter = new CreateCharacter(_characterFactory, _saveCharacter);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_CharacterFactory(string name)
    {
        var command = new CreateCharacterCommand(name);

        _createCharacter.Execute(command);

        A.CallTo(() => _characterFactory.Create(command)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Call_SaveCharacter()
    {
        var command = new CreateCharacterCommand("Frodo");
        var frodo = new Character(command.Name);
        A.CallTo(() => _characterFactory.Create(command)).Returns(frodo);

        _createCharacter.Execute(command);

        A.CallTo(() => _saveCharacter.Save(frodo)).MustHaveHappenedOnceExactly();
    }
}