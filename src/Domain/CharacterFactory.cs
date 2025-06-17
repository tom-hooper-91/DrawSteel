namespace Domain;

public class CharacterFactory : ICharacterFactory
{
    public Character Create(CreateCharacterCommand command)
    {
        return new Character(command.Name);
    }
}