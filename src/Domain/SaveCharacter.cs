using Domain.Repositories;

namespace Domain;

public class SaveCharacter(ICharacterRepository characterRepository) : ISaveCharacter
{
    public void Save(Character character)
    {
        characterRepository.Add(character);
    }
}