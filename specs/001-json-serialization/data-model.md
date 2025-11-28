# Data Model: Consistent Character JSON Contract

**Feature**: Consistent Character JSON Contract  
**Date**: November 28, 2025  
**Purpose**: Describe how Character identifiers and related payloads are represented across layers, plus the validation and state expectations driving the serialization fix.

## Domain Entities (unchanged)

### Character

- **Type**: Domain entity (`record Character(CharacterId Id, string Name)`)
- **Responsibility**: Encapsulates game character identity and display name.
- **Validation Rules**:
  - `Id` must be a valid `CharacterId` (GUID value object).
  - `Name` cannot be null/empty/whitespace; enforced inside domain services.
- **State**: Immutable; updates create new instances.

### CharacterId

- **Type**: Value object (`record CharacterId(Guid Value)`)
- **Responsibility**: Strong typing over GUID identifiers, prevents primitive obsession.
- **Validation Rules**:
  - `Value` must be a `Guid`. Construction fails for invalid input.
- **State**: Immutable.

## Presentation DTOs (updated)

### CharacterResponse

- **Scope**: API layer output for single-character endpoints (GET, POST, PUT).
- **Fields**:
  - `string id` — flattened identifier exposed to clients. Mirrors `Character.Id.Value`.
  - `string name` — display name.
- **Validation / Formatting**:
  - `id` always serialized as lowercase GUID string; never nested under `.value`.
  - Field casing is camelCase to match existing API conventions.

### CharacterListItem

- **Scope**: API layer output for collection endpoints (GET /characters).
- **Fields**: identical to `CharacterResponse`; reuses same mapping helper to guarantee parity.
- **Notes**: Created to ensure list responses cannot diverge from single-resource responses.

### CharacterRequest

- **Scope**: API layer input for POST/PUT.
- **Fields**:
  - `string? id` — optional; ignored on create but required on update.
  - `string name` — required payload property.
- **Validation Rules**:
  - `id` (when supplied) must parse as GUID; API returns 400 with problem details otherwise.
  - `name` must be 1-100 chars (leverages existing domain validation plus API-level guard).

### ErrorResponse (Problem Details alias)

- **Scope**: API validation failures.
- **Fields**:
  - `string type`, `string title`, `int status`, `string detail`, `Dictionary<string,string[]> errors`.
- **Rules**:
  - `errors["id"]` contains human-friendly explanation when identifier payload is invalid or uses legacy `id.value` structure.

## Mapping & Serialization Rules

1. API layer converts Domain `Character` → `CharacterResponse` via dedicated mapper or extension method to ensure the GUID is stringified once.
2. Incoming HTTP payloads bind to `CharacterRequest`; update flows confirm payload `id` matches route `id`.
3. Minimal API endpoints configure `JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase` (already default) and explicitly ignore legacy nested `value` tokens.
4. Integration tests snapshot serialized JSON to lock schema (see `Tests.Integration/Acceptance/CharacterSerializationTests`).

## Validation Allocation

| Layer | Responsibility |
|-------|----------------|
| API | Request shape enforcement (`id` scalar, GUID parsing, forbidden nested `value`). |
| Application | Ensures DTOs populate Domain commands consistently. |
| Domain | Business rules (non-empty name). |
| Infrastructure | Persists GUID using Mongo's `_id` and existing mappings; no schema change required. |

## State Transitions (serialization-focused)

1. **Create**: Client sends `{ "name": "..." }`; server assigns `Guid`, persists, returns `{ "id": "...", "name": "..." }`.
2. **Update**: Client sends `{ "id": "...", "name": "..." }` (no nested `value`); server validates IDs, persists, returns same shape.
3. **List**: Server enumerates stored characters, projects each to `{ "id": "...", "name": "..." }`.
4. **Error**: Any malformed identifier (missing, not GUID, nested) results in `application/problem+json` with `errors.id` entry describing fix.

## Relationships

```text
Http JSON ⇆ CharacterRequest/Response ⇆ Application Commands ⇆ Domain Character ⇆ Mongo Repository
```

- Presentation DTOs isolate serialization policy so Domain + Infrastructure remain unchanged.
- Integration tests exercise the full chain to guarantee mappings stay synchronized.
