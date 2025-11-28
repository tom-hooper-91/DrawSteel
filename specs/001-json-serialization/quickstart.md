# Quickstart: Consistent Character JSON Contract

**Feature**: Consistent Character JSON Contract  
**Date**: November 28, 2025  
**Audience**: Developers implementing serialization fixes and contract-focused tests

## Overview

Bring every Character API payload (requests & responses) to a single-level `id` property while expanding HTTP-level integration tests that verify the JSON contract. Work is confined to the API, Application, Domain, Infrastructure, and Tests projects—no Web UI changes.

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop (for Mongo/Testcontainers)
- Existing DrawSteel solution cloned and able to run API/tests
- Familiarity with Interaction-Driven Design layering and Minimal API patterns

## Implementation Flow

1. **Align DTOs** (`src/API/Requests` & mapping helpers).
   - Introduce/update `CharacterResponse`, `CharacterListItem`, and `CharacterRequest` types exposing `id` as `string`.
   - Map between Domain `CharacterId` and DTOs via explicit helper to avoid nested serialization.
2. **Configure Serialization** (`src/API/Program.cs`).
   - Ensure System.Text.Json options keep camelCase and ignore unknown fields.
   - Add model binding/validation that rejects payloads containing `id.value` or malformed `id`.
3. **Update Endpoints** (`src/API/Characters.cs`).
   - Use new DTOs for both input and output.
   - Validate route `id` vs body `id` on updates; return ProblemDetails with `errors.id` on mismatch.
4. **Propagate Changes Downstack**.
   - Application use cases now accept DTOs with flattened ids and convert to `CharacterId` internally.
   - Infrastructure repositories continue to store GUIDs; verify mappings don't reintroduce nested values.
5. **Strengthen Integration Tests** (`Tests.Integration/Acceptance`).
   - Add `CharacterSerializationTests` covering create/get/list/update/delete + validation failures.
   - Snapshot expected JSON (either via inline strings or `Verify`-style helpers) to detect drift.
6. **Document the Contract**.
   - Update `API.http` samples, README snippets, and `contracts/*.openapi.yaml` to reflect new payloads.

## Commands

```powershell
# Run unit tests (guards DTO mapping + validators)
dotnet test src/Tests.Unit/Tests.Unit.csproj

# Run integration tests with Testcontainers
dotnet test src/Tests.Integration/Tests.Integration.csproj --filter Category=Serialization

# Launch API locally for manual verification
cd src/API
dotnet run
```

## Testing Expectations

- **Unit Level**: Verify mapper outputs plain `id` string; ensure validators reject nested `id.value`.
- **Integration Level**: For each endpoint, assert the full JSON body (status + payload) exactly matches fixtures. Include negative cases (bad GUID, legacy schema) returning problem details.
- **Regression Guard**: Add tests that deserialize stored Mongo documents to prove repository mapping unaffected.

## Rollout Tips

- Deploy behind existing version without version bump since schema change is backward-compatible for compliant consumers.
- Communicate schema update to known consumers with new sample payloads.
- When legitimate schema changes occur later, update both fixtures and OpenAPI definition in the same PR to keep consumers aligned.
