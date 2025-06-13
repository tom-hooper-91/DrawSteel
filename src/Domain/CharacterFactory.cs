namespace Domain;

public class CharacterFactory : ICharacterFactory
{
    public Character Create(CreateCharacterCommand newCharacter)
    {
        return new Character(newCharacter.Name);
    }
}