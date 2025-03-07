using Domain;

namespace Application;

public interface ICreateCharacter
{
    Task<CharacterId> Execute(CreateCharacterCommand command);
}