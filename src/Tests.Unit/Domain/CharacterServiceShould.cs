using Application;
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

        var characterId = await _service.Create(new CreateCharacterCommand(name, CharacterClass.Tactician));

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

    [TestCase("Frodo", "Frodo Baggins")]
    [TestCase("Sam", "Samwise Gamgee")]
    public async Task Update_character_name_successfully(string originalName, string newName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, newName);
        var expectedCharacter = new Character(characterId, newName);
        A.CallTo(() => _repository.Update(A<Character>.That.Matches(c => c.Id == characterId && c.Name == newName)))
            .Returns(true);

        var result = await _service.Update(command);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(newName));
            Assert.That(result.Id, Is.EqualTo(characterId));
        });
    }

    [Test]
    public async Task Return_null_when_updating_nonexistent_character()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, "New Name");
        A.CallTo(() => _repository.Update(A<Character>._)).Returns(false);

        var result = await _service.Update(command);

        Assert.That(result, Is.Null);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void Throw_exception_when_updating_with_empty_name(string? emptyName)
    {
        var characterId = new CharacterId(Guid.NewGuid());
        var command = new UpdateCharacterCommand(characterId, emptyName!);

        Assert.ThrowsAsync<ArgumentException>(async () => await _service.Update(command));
    }

    [Test]
    public async Task Delete_character_successfully()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _repository.Delete(characterId)).Returns(true);

        var result = await _service.Delete(characterId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Return_false_when_deleting_nonexistent_character()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _repository.Delete(characterId)).Returns(false);

        var result = await _service.Delete(characterId);

        Assert.That(result, Is.False);
    }
}