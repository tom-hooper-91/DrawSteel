using Domain;

namespace Application;

public class GetCharacter(ICharacterService service) : IGetCharacter
{
    public async Task<Character> Execute(CharacterId characterId)
    {
        return await service.Get(characterId);
    }
}