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

    public Task<Character> Get(CharacterId characterId)
    {
        throw new NotImplementedException();
    }
}