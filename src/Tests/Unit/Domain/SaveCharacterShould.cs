using Domain;
using Domain.Repositories;
using FakeItEasy;

namespace Tests.Unit.Domain;

[TestFixture]
public class SaveCharacterShould
{
    [Test]
    public void Call_Character_Repository()
    {
        var characterRepository = A.Fake<ICharacterRepository>();
        var saveCharacter = new SaveCharacter(characterRepository);
        var frodo = new Character("Frodo");
        
        saveCharacter.Save(frodo);

        A.CallTo(() => characterRepository.Add(frodo)).MustHaveHappenedOnceExactly();
    }
}