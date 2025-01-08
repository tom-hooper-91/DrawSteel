using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory characterFactory) : ICreateCharacter
{
    public void Execute(CreateCharacterCommand command)
    {
        characterFactory.Create(command);
    }
}