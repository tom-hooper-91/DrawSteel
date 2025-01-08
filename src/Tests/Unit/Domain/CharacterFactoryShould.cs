using Domain;

namespace Tests.Unit.Domain;

[TestFixture]
public class CharacterFactoryShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Return_a_character(string name)
    {
        var expectedCharacter = new Character(name);
        var factory = new CharacterFactory();
        var command = new CreateCharacterCommand(name);
        
        Assert.That(factory.Create(command), Is.EqualTo(expectedCharacter));
    }
}