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

    // T022: Implement Update method
    public async Task<Character?> Update(UpdateCharacterCommand command)
    {
        // Validate name is not empty
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Character name cannot be empty", nameof(command.Name));
        
        // Create updated character instance (immutable)
        var updatedCharacter = new Character(command.Id, command.Name);
        
        // Attempt update via repository
        var wasUpdated = await repository.Update(updatedCharacter);
        
        // Return character if updated, null if not found
        return wasUpdated ? updatedCharacter : null;
    }

    public async Task<bool> Delete(CharacterId characterId)
    {
        return await repository.Delete(characterId);
    }
}