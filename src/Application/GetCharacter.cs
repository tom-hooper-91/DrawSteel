using Domain;

namespace Application;

public class GetCharacter(ICharacterService service) : IGetCharacter
{
    public async Task<CharacterDto?> Execute(CharacterId characterId)
    {
        var character = await service.Get(characterId);
        return character is null ? null : CharacterDto.FromCharacter(character);
    }
}