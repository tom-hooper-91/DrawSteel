using Domain;

namespace Application;

public record CreateCharacterRequest(string Name, CharacterClass characterClass);