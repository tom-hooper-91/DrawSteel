using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Tests.Integration.Acceptance.Fixtures;

public static class ContractFixtureStore
{
    private const string FeatureDirectory = "specs/001-json-serialization/contracts/fixtures";
    private static readonly Lazy<string> FixturesRoot = new(ResolveFixturesRoot);

    public static JsonDocument LoadJson(string fixtureName, IDictionary<string, string>? tokens = null)
    {
        var path = Path.Combine(FixturesRoot.Value, EnsureJsonExtension(fixtureName));
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Fixture '{fixtureName}' not found.", path);
        }

        var payload = File.ReadAllText(path, Encoding.UTF8);
        if (tokens is not null)
        {
            foreach (var token in tokens)
            {
                payload = payload.Replace($"{{{{{token.Key}}}}}", token.Value, StringComparison.Ordinal);
            }
        }

        return JsonDocument.Parse(payload);
    }

    private static string EnsureJsonExtension(string fixtureName)
    {
        return fixtureName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? fixtureName
            : fixtureName + ".json";
    }

    private static string ResolveFixturesRoot()
    {
        var repoRoot = ResolveRepositoryRoot();
        return Path.GetFullPath(Path.Combine(repoRoot, FeatureDirectory.Replace('/', Path.DirectorySeparatorChar)));
    }

    private static string ResolveRepositoryRoot()
    {
        var current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            if (Directory.Exists(Path.Combine(current, ".git")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new InvalidOperationException("Unable to locate the repository root for fixture resolution.");
    }
}