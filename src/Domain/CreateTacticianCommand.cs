namespace Domain;

public record CreateTacticianCommand(string Name)
    : CreateCharacterCommand(Name, CharacterClass.Tactician);