using Application;
using Domain;
using System.Linq;

namespace API.Contracts;

public static class CharacterContractMapper
{
    public static CharacterResponse ToResponse(CharacterDto dto) => new(dto.Id, dto.Name);

    public static CharacterResponse ToResponse(Character character) => new(character.Id.Value.ToString(), character.Name);

    public static CharacterListItem ToListItem(CharacterDto dto) => new(dto.Id, dto.Name);

    public static IEnumerable<CharacterListItem> ToListItems(IEnumerable<CharacterDto> dtos) => dtos.Select(ToListItem);
}
