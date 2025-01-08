using Domain;

namespace Application;

public interface ICreateCharacter
{
    void Execute(CreateCharacterCommand command);
}