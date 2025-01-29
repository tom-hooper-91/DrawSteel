﻿using Domain;
using Domain.Repositories;
using Infrastructure;

namespace Tests.Unit.Infrastructure;

[TestFixture]
public class InMemoryCharacterRepositoryShould
{
    [Test]
    public void Get_a_character_from_query()
    {
        var character = new Character("Frodo");
        var repository = new InMemoryCharacterRepository();
        var characterId = character.Id;
        
        repository.Add(character);
        
        Assert.That(repository.Get(characterId), Is.EqualTo(character));
    }
}