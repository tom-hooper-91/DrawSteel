using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory characterFactory, ISaveCharacter saveCharacter) : ICreateCharacter
{
    public void Execute(CreateCharacterCommand command)
    {
        var character = characterFactory.Create(command);
        saveCharacter.Save(character);
    }
}