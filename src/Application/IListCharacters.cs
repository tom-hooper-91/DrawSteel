using System.Collections.Generic;

namespace Application;

public interface IListCharacters
{
    Task<IReadOnlyCollection<CharacterDto>> Execute();
}
