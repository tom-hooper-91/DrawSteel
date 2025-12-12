namespace Domain;

public record CreateFuryCommand(string Name) : CreateCharacterCommand(Name, CharacterClass.Fury)
{
}