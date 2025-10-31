using Domain;

namespace Application;

public interface IDeleteCharacter
{
    Task<bool> Execute(CharacterId characterId);
}
