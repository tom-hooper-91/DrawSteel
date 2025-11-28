using System.Collections.Generic;

namespace Domain;

public interface ICharacterService
{
    Task<CharacterId> Create(CreateCharacterCommand character);
    Task<Character?> Get(CharacterId characterId);
    Task<Character?> Update(UpdateCharacterCommand command);
    Task<bool> Delete(CharacterId characterId);
    Task<IReadOnlyCollection<Character>> List();
}