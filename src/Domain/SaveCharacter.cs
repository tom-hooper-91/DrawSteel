using Domain.Repositories;

namespace Domain;

public class SaveCharacter(ICharacterRepository characterRepository) : ISaveCharacter
{
    public async Task<CharacterId> Save(Character character)
    {
        return await characterRepository.Add(character);
    }
}