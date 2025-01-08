namespace Domain;

public interface ICharacterFactory
{
    Character Create(CreateCharacterCommand command);
}