using Domain;

namespace Application;

public record CharacterDto(string Id, string Name)
{
    public static CharacterDto FromCharacter(Character character) => new(character.Id.Value.ToString(), character.Name);
}
