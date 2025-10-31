using Domain;

namespace Application;

public interface IUpdateCharacter
{
    Task<Character?> Execute(UpdateCharacterCommand command);
}
