using Domain.Repositories;

namespace Domain;

public class SaveCharacter(ICharacterRepository repository) : ISaveCharacter
{
    public async Task<CharacterId> This(Character character)
    {
        return await repository.Add(character);
    }
}