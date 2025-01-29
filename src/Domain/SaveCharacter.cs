using Domain.Repositories;

namespace Domain;

public class SaveCharacter(ICharacterRepository characterRepository) : ISaveCharacter
{
    public CharacterId Save(Character character)
    {
        return characterRepository.Add(character);
    }
}