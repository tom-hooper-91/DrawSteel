# Research: Consistent Character JSON Contract

## JSON Serialization Strategy

- **Decision**: Use System.Text.Json configuration plus API-level DTO shaping so every HTTP response/request exposes `id` as a single string property, with custom converters only if DTO mapping cannot absorb value objects.
- **Rationale**: Minimal APIs already rely on System.Text.Json; shaping DTOs keeps the Domain model (value objects) intact while giving presentation-friendly payloads. Only the API project needs awareness of the flattening rule, simplifying maintenance and avoiding converter side effects across unrelated types.
- **Alternatives considered**:
  - Custom `JsonConverter` on `CharacterId` to project `.Value` automatically. Rejected to avoid leaking presentation-specific formatting into Domain serialization or other contexts (e.g., persistence snapshots).
  - Switching to Newtonsoft.Json for flexible converters. Rejected because System.Text.Json meets requirements and swapping serializers would add unnecessary dependencies.

## Mongo Identifier Mapping

- **Decision**: Continue persisting the identifier through the `CharacterId` value object but ensure MongoDB `BsonClassMap` (or repository mapping logic) reads/writes the GUID value so repository DTOs align with the flattened API contract.
- **Rationale**: The repository already handles value objects; keeping the mapping localized avoids schema churn inside Mongo collections. Only the serialization boundary changes, preventing costly migrations.
- **Alternatives considered**:
  - Refactor persistence to store nested `id.value` documents for symmetry with Domain. Rejected because it perpetuates the bug and complicates queries.
  - Introduce a separate persistence model with primitive Ids. Deferred due to extra mapping maintenance cost for little gain right now.

## Contract-Focused Integration Tests

- **Decision**: Expand `Tests.Integration/Acceptance` suite with Testcontainers-backed API tests that spin up the full HTTP stack, issue real JSON payloads via `HttpClient`, and snapshot both request and response bodies for each Character operation.
- **Rationale**: Contract regressions originate at the HTTP boundary, so end-to-end API tests (still within CI-friendly scope) provide the strongest signal. Using Testcontainers keeps the environment representative while remaining deterministic.
- **Alternatives considered**:
  - Rely solely on unit tests of DTOs/mappers. Rejected because they cannot guarantee the final HTTP output after middleware/filters.
  - Introduce an external contract testing tool (e.g., Pact). Deferred until multiple consumers require consumer-driven contracts.

## Validation & Error Messaging

- **Decision**: Standardize validation responses so malformed identifiers trigger 400 responses with structured problem details referencing the `id` field, reusing ASP.NET Core's `ProblemDetails` factory.
- **Rationale**: Aligns with REST norms and keeps guidance user-facing. Consistent messaging also makes integration tests deterministic.
- **Alternatives considered**:
  - Custom ad-hoc error payloads per endpoint. Rejected because that increases drift risk and duplicates logic.
  - Silent coercion of bad IDs to empty GUIDs. Rejected for security/clarity reasons.
