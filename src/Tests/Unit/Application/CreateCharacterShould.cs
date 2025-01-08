using Application;
using Domain;
using FakeItEasy;

namespace Tests.Unit.Application;

[TestFixture]
public class CreateCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_CharacterFactory(string name)
    {
        var characterFactory = A.Fake<ICharacterFactory>();
        var createCharacter = new CreateCharacter(characterFactory);
        var command = new CreateCharacterCommand(name);
        
        createCharacter.Execute(command);
        
        A.CallTo(() => characterFactory.Create(command)).MustHaveHappenedOnceExactly();
    }
}