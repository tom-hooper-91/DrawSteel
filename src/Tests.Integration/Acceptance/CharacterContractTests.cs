using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Domain;
using Tests.Integration.Acceptance.Assertions;
using Tests.Integration.Acceptance.Fixtures;

namespace Tests.Integration.Acceptance;

[TestFixture]
[Category("Serialization")]
public class CharacterContractTests
{
    private static readonly Guid FrodoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SamId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid AragornId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid DeleteId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    [SetUp]
    public async Task Setup()
    {
        await CharacterApiFixture.ClearCharactersAsync();
    }

    [Test]
    public async Task Post_response_matches_contract_fixture()
    {
        var response = await CharacterApiFixture.HttpClient.PostAsJsonAsync("/api/characters", new { name = "Beregond" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var actual = await ReadJsonAsync(response);
        var characterId = actual.RootElement.GetProperty("id").GetString()!;

        using var expected = ContractFixtureStore.LoadJson(
            "create-character.success",
            new Dictionary<string, string> { ["characterId"] = characterId });

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Create character contract drifted.");
    }

    [Test]
    public async Task Get_response_matches_contract_fixture()
    {
        await SeedCharacterAsync(FrodoId, "Frodo");

        var response = await CharacterApiFixture.HttpClient.GetAsync($"/api/characters/{FrodoId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("get-character.success");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Get character contract drifted.");
    }

    [Test]
    public async Task List_response_matches_contract_fixture()
    {
        await SeedCharacterAsync(FrodoId, "Frodo");
        await SeedCharacterAsync(SamId, "Sam");

        var response = await CharacterApiFixture.HttpClient.GetAsync("/api/characters");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("list-characters.success");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "List characters contract drifted.");
    }

    [Test]
    public async Task Put_response_matches_contract_fixture()
    {
        await SeedCharacterAsync(AragornId, "Aragorn");
        var payload = JsonContent.Create(new { id = AragornId.ToString(), name = "Aragorn Elessar" });

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{AragornId}", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("update-character.success");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Update character contract drifted.");
    }

    [Test]
    public async Task Delete_response_matches_contract_fixture()
    {
        await SeedCharacterAsync(DeleteId, "Boromir");

        var response = await CharacterApiFixture.HttpClient.DeleteAsync($"/api/characters/{DeleteId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("delete-character.success");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Delete character contract drifted.");
    }

    [Test]
    public async Task Put_rejects_legacy_identifier_payload_per_contract()
    {
        await SeedCharacterAsync(Guid.NewGuid(), "Legolas");
        var legacyPayload = new
        {
            id = new { value = AragornId.ToString() },
            name = "Prince of the Woodland Realm"
        };
        var content = JsonContent.Create(legacyPayload);

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{AragornId}", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("update-character.error.legacy-id");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Legacy identifier validation drifted.");
    }

    [Test]
    public async Task Put_rejects_mismatched_identifier_payload_per_contract()
    {
        await SeedCharacterAsync(AragornId, "Aragorn");
        var payload = JsonContent.Create(new { id = Guid.NewGuid().ToString(), name = "Aragorn" });

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{AragornId}", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("update-character.error.mismatched-id");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Identifier mismatch contract drifted.");
    }

    [Test]
    public async Task Put_rejects_invalid_identifier_payload_per_contract()
    {
        await SeedCharacterAsync(AragornId, "Aragorn");
        var payload = JsonContent.Create(new { id = "not-a-guid", name = "Aragorn" });

        var response = await CharacterApiFixture.HttpClient.PutAsync($"/api/characters/{AragornId}", payload);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var actual = await ReadJsonAsync(response);
        using var expected = ContractFixtureStore.LoadJson("update-character.error.invalid-id");

        actual.RootElement.ShouldMatchFixture(expected.RootElement, "Invalid identifier contract drifted.");
    }

    private static async Task SeedCharacterAsync(Guid characterId, string name)
    {
        await CharacterApiFixture.Characters.InsertOneAsync(new Character(new CharacterId(characterId), name));
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}
