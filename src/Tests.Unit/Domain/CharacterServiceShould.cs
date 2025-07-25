using Domain;
using Domain.Repositories;
using FakeItEasy;

namespace Tests.Domain;

[TestFixture]
public class CharacterServiceShould
{
    private CharacterService _service;
    private ICharacterRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = A.Fake<ICharacterRepository>();
        _service = new CharacterService(_repository);
    }

    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Add_a_Character_to_the_Repository_and_return_CharacterId(string name)
    {
        var expectedId = new CharacterId(Guid.NewGuid());
        var character = new Character(expectedId, name);
        A.CallTo(() => _repository.Add(A<Character>.That.Matches(c => c.Name.Equals(character.Name))))
            .Returns(Task.FromResult(expectedId));

        var characterId = await _service.Create(new CreateCharacterCommand(name));

        Assert.That(characterId, Is.EqualTo(expectedId));
    }

    
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public async Task Get_a_Character_from_the_Repository_and_return_it(string name)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var expectedCharacter = new Character(characterId, name);
        A.CallTo(() => _repository.Get(characterId)).Returns(expectedCharacter);

        var character = await _service.Get(characterId);
        
        Assert.That(character, Is.EqualTo(expectedCharacter));
    }
}