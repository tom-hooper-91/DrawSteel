using Domain;

namespace Application;

public class GetCharacter : IGetCharacter
{
    public Task<Character> Execute(CharacterId characterId)
    {
        throw new NotImplementedException();
    }
}