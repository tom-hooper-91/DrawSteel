namespace Domain;

public record CreateWarriorCommand(string Name)
    : CreateCharacterCommand(Name, CharacterClass.Warrior);