using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Application;

public class ListCharacters(ICharacterService characterService) : IListCharacters
{
    public async Task<IReadOnlyCollection<CharacterDto>> Execute()
    {
        var characters = await characterService.List();
        return characters
            .Select(CharacterDto.FromCharacter)
            .OrderBy(character => character.Name, StringComparer.Ordinal)
            .ToList();
    }
}
