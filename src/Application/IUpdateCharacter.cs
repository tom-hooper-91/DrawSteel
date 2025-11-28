using Domain;

namespace Application;

public interface IUpdateCharacter
{
    Task<CharacterDto?> Execute(UpdateCharacterInput request);
}
