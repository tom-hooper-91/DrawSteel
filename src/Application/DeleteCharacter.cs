using Domain;

namespace Application;

public class DeleteCharacter(ICharacterService characterService) : IDeleteCharacter
{
    public async Task<bool> Execute(CharacterId characterId)
    {
        return await characterService.Delete(characterId);
    }
}
