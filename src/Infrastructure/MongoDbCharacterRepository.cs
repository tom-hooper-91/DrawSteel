﻿using Domain;
using Domain.Repositories;
using MongoDB.Driver;

namespace Infrastructure;

public class MongoDbCharacterRepository(IMongoCollection<Character> characters) : ICharacterRepository
{
    public async Task<CharacterId> Add(Character character)
    {
        await characters.InsertOneAsync(character);
        return character.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        return await characters.Find(character => character.Id == id).SingleOrDefaultAsync();
    }
}