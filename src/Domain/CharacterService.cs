using Domain.Repositories;

namespace Domain;

public class CharacterService(ICharacterRepository repository) : ICharacterService
{
    public async Task<CharacterId> Create(CreateCharacterCommand character)
    {
        var newCharacterId = new CharacterId(Guid.NewGuid());
        var newCharacter = new Character(newCharacterId, character.Name);

        return await repository.Add(newCharacter);
    }

    public async Task<Character?> Get(CharacterId characterId)
    {
        return await repository.Get(characterId);
    }

    public async Task<Character?> Update(UpdateCharacterCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Character name cannot be empty", nameof(command.Name));
        
        var updatedCharacter = new Character(command.Id, command.Name);
        var wasUpdated = await repository.Update(updatedCharacter);
        
        return wasUpdated ? updatedCharacter : null;
    }

    public async Task<bool> Delete(CharacterId characterId)
    {
        return await repository.Delete(characterId);
    }
}