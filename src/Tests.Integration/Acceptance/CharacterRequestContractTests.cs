using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain;
using Tests.Integration.Acceptance.Assertions;

namespace Tests.Integration.Acceptance;

[TestFixture]
[Category("Serialization")]
public class CharacterRequestContractTests
{
    [SetUp]
    public async Task Setup()
    {
        await CharacterApiFixture.ClearCharactersAsync();
    }

    [Test]
    public async Task Post_accepts_flattened_payloads()
    {
        var payload = JsonContent.Create(new { name = "Faramir" });

        var response = await CharacterApiFixture.HttpClient.PostAsync("/api/characters", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var json = await ReadJsonAsync(response);
        json.RootElement.ShouldContainScalarId();
        Assert.That(json.RootElement.GetProperty("name").GetString(), Is.EqualTo("Faramir"));
    }

    [Test]
    public async Task Put_accepts_matching_identifier()
    {
        var characterId = await SeedCharacterAsync("Gimli");
        var payload = JsonContent.Create(new { id = characterId.ToString(), name = "Gimli son of Gl\u00f3in" });

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{characterId}", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var json = await ReadJsonAsync(response);
        json.RootElement.ShouldContainScalarId();
        Assert.That(json.RootElement.GetProperty("id").GetString(), Is.EqualTo(characterId.ToString()));
    }

    [Test]
    public async Task Put_rejects_nested_identifier_payloads()
    {
        var characterId = await SeedCharacterAsync("Legolas");
        var legacyPayload = new
        {
            id = new { value = characterId.ToString() },
            name = "Prince of the Woodland Realm"
        };
        var content = new StringContent(JsonSerializer.Serialize(legacyPayload), Encoding.UTF8, "application/json");

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{characterId}", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var problem = await ReadJsonAsync(response);
        Assert.That(problem.RootElement.TryGetProperty("errors", out var errors), Is.True,
            $"Response: {problem.RootElement.GetRawText()}");
        Assert.That(errors.TryGetProperty("id", out var idErrors), Is.True,
            $"Response: {problem.RootElement.GetRawText()}");
        Assert.That(idErrors[0].GetString(), Does.Contain("Legacy"));
    }

    [Test]
    public async Task Put_rejects_mismatched_identifier_payloads()
    {
        var characterId = await SeedCharacterAsync("Boromir");
        var payload = JsonContent.Create(new { id = Guid.NewGuid().ToString(), name = "Captain of the White Tower" });

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{characterId}", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var problem = await ReadJsonAsync(response);
        Assert.That(problem.RootElement.TryGetProperty("errors", out var errors), Is.True,
            $"Response: {problem.RootElement.GetRawText()}");
        Assert.That(errors.TryGetProperty("id", out var idErrors), Is.True,
            $"Response: {problem.RootElement.GetRawText()}");
        Assert.That(idErrors[0].GetString(), Does.Contain("match the route"));
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }

    private static async Task<Guid> SeedCharacterAsync(string name)
    {
        var characterId = Guid.NewGuid();
        await CharacterApiFixture.Characters.InsertOneAsync(new Character(new CharacterId(characterId), name));
        return characterId;
    }
}
