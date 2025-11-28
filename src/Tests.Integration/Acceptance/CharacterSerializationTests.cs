using System.Net.Http.Json;
using System.Text.Json;
using Domain;
using Tests.Integration.Acceptance.Assertions;

namespace Tests.Integration.Acceptance;

[TestFixture]
[Category("Serialization")]
public class CharacterSerializationTests
{
    [SetUp]
    public async Task Setup()
    {
        await CharacterApiFixture.ClearCharactersAsync();
    }

    [Test]
    public async Task Create_character_returns_flat_id()
    {
        var response = await CharacterApiFixture.HttpClient.PostAsJsonAsync("/api/characters", new { name = "Gandalf" });

        Assert.That(response.IsSuccessStatusCode, Is.True);
        using var payload = await ReadPayloadAsync(response);
        payload.RootElement.ShouldContainScalarId();
        Assert.That(payload.RootElement.GetProperty("name").GetString(), Is.EqualTo("Gandalf"));
    }

    [Test]
    public async Task Get_character_responds_with_flat_id()
    {
        var characterId = new CharacterId(Guid.NewGuid());
        await CharacterApiFixture.Characters.InsertOneAsync(new Character(characterId, "Frodo"));

        var response = await CharacterApiFixture.HttpClient.GetAsync($"/api/characters/{characterId.Value}");

        Assert.That(response.IsSuccessStatusCode, Is.True);
        using var payload = await ReadPayloadAsync(response);
        payload.RootElement.ShouldContainScalarId();
        Assert.That(payload.RootElement.GetProperty("name").GetString(), Is.EqualTo("Frodo"));
    }

    [Test]
    public async Task List_characters_responds_with_flat_ids()
    {
        var frodo = new Character(new CharacterId(Guid.NewGuid()), "Frodo");
        var sam = new Character(new CharacterId(Guid.NewGuid()), "Sam");
        await CharacterApiFixture.Characters.InsertManyAsync(new[] { frodo, sam });

        var response = await CharacterApiFixture.HttpClient.GetAsync("/api/characters");

        Assert.That(response.IsSuccessStatusCode, Is.True);
        using var payload = await ReadPayloadAsync(response);
        Assert.That(payload.RootElement.TryGetProperty("items", out var items), "List response should contain an items array.");
        Assert.That(items.GetArrayLength(), Is.EqualTo(2));
        foreach (var item in items.EnumerateArray())
        {
            item.ShouldContainScalarId();
        }
    }

    private static async Task<JsonDocument> ReadPayloadAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}
