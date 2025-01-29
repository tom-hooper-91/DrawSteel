using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory characterFactory, ISaveCharacter saveCharacter) : ICreateCharacter
{
    public CharacterId Execute(CreateCharacterCommand command)
    {
        var character = characterFactory.Create(command);
        return saveCharacter.Save(character);
    }
}