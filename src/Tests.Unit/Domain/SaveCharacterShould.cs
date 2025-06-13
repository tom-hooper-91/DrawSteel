using Domain;
using Domain.Repositories;
using FakeItEasy;

namespace Tests.Domain;

[TestFixture]
public class SaveCharacterShould
{
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_Character_Repository(string name)
    {
        var repository = A.Fake<ICharacterRepository>();
        var save = new SaveCharacter(repository);
        var character = new Character(name);
        
        _ = save.This(character);

        A.CallTo(() => repository.Add(character)).MustHaveHappenedOnceExactly();
    }
}