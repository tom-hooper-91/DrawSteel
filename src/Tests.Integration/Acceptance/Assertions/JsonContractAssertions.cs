using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;

namespace Tests.Integration.Acceptance.Assertions;

public static class JsonContractAssertions
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static void ShouldMatchContract<TExpected>(this JsonElement actual, TExpected expected)
    {
        using var expectedJson = JsonDocument.Parse(JsonSerializer.Serialize(expected, SerializerOptions));
        Assert.That(actual.ToString(), Is.EqualTo(expectedJson.RootElement.ToString()), "JSON payload deviates from the documented contract.");
    }

    public static void ShouldMatchFixture(this JsonElement actual, JsonElement expected, string? because = null)
    {
        var actualPayload = JsonSerializer.Serialize(actual, SerializerOptions);
        var expectedPayload = JsonSerializer.Serialize(expected, SerializerOptions);
        Assert.That(actualPayload, Is.EqualTo(expectedPayload), because ?? "JSON payload deviates from the approved fixture.");
    }

    public static void ShouldContainScalarId(this JsonElement payload)
    {
        Assert.That(payload.TryGetProperty("id", out var idProperty), Is.True, "Serialized payload must include an 'id' property.");
        Assert.That(idProperty.ValueKind, Is.EqualTo(JsonValueKind.String), "'id' must be a string.");
        Assert.DoesNotThrow(() => Guid.Parse(idProperty.GetString() ?? string.Empty), "'id' must contain a valid GUID.");
    }
}
