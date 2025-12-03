namespace Domain;

public record CreateGardenerCommand(string Name) : CreateCharacterCommand(Name, CharacterClass.Gardener)
{
}