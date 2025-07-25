using Domain;

namespace Application;

public interface IGetCharacter
{
    Task<Character> Execute(CharacterId characterId);
}