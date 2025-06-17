using Domain.Repositories;

namespace Domain;

public class CharacterService(ICharacterRepository repository) : ICharacterService
{
    public Task<CharacterId> Create(CreateCharacterCommand character)
    {
        var newCharacter = new Character(character.Name);
        repository.Add(newCharacter);
        
        return Task.FromResult(newCharacter.Id);
    }
}