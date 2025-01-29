using Domain;

namespace Application;

public interface ICreateCharacter
{
    CharacterId Execute(CreateCharacterCommand command);
}