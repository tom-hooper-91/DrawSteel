﻿using System.Text;
using System.Text.Json;
using API;
using Application;
using Domain;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Tests.Integration.Acceptance;

[TestFixture]
public class CreateCharacterFeature
{
    [Test]
    public async Task Return_a_success()
    {
        var httpContext = new DefaultHttpContext();
        var stringData = Encoding.Default.GetBytes("{\n\"name\":\"Frodo\"\n}");
        httpContext.Request.ContentLength = stringData.Length;
        httpContext.Request.ContentType = "application/json";
        httpContext.Request.Body = new MemoryStream(stringData);
        
        var characterFactory = new CharacterFactory();
        var seeder = new CharacterSeeder(Fixture.Client);
        await seeder.SeedDatabase();
        var container = Fixture.Client.GetDatabase("drawsteel").GetContainer("characters");
        var characterRepository = new CosmosDbCharacterRepository(container);
        // var characterRepository = new InMemoryCharacterRepository();
        var createCharacterAction = new CreateCharacter(characterFactory, new SaveCharacter(characterRepository));
        var characters = new Characters(createCharacterAction);

        var actionResult = await characters.Create(httpContext.Request) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(actionResult!.Value!.ToString()!);
        var frodo = await characterRepository.Get(characterId!);
        
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
            Assert.That(frodo.Name, Is.EqualTo("Frodo"));
        });
    }
}