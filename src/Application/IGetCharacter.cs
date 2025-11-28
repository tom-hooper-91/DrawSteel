using Domain;

namespace Application;

public interface IGetCharacter
{
    Task<CharacterDto?> Execute(CharacterId characterId);
}