# Feature Specification: Consistent Character JSON Contract

**Feature Branch**: `001-json-serialization`  
**Created**: November 28, 2025  
**Status**: Draft  
**Input**: User description: "I want to fix the inconsisten json serialisation of the API, particularly how the characterId is serialised as 'id.value =' instead of 'id ='. I would also like more robust integration testing, it would be ideal if they gave me more confidence so I was sure of the json the end user passes in and get's out"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consistent Identifier Output (Priority: P1)

Players, automation suites, and downstream services that consume the Character API need every endpoint to return the same flattened identifier field (`id`) so payloads are predictable and easy to parse.

**Why this priority**: Without a consistent identifier, clients must write brittle parsing logic and risk missing updates. This is the core defect the user reported, so correcting it unlocks immediate value.

**Independent Test**: Can be fully tested by calling any read/update/create endpoint and validating the raw HTTP JSON body includes `"id": "<guid>"` instead of nested `id.value`.

**Acceptance Scenarios**:

1. **Given** a character exists, **When** a client retrieves it via the API, **Then** the JSON response exposes `id` as a single string property and no nested `value` keys appear.
2. **Given** a client lists multiple characters, **When** the API responds, **Then** each element in the collection uses the same `id` field name and casing.
3. **Given** system responses are logged or forwarded to another service, **When** that service inspects the payload schema, **Then** it matches previously documented contract samples using `id`.

---

### User Story 2 - Predictable Request Payloads (Priority: P2)

API clients creating or updating characters need to send payloads that mirror the documented response schema so they can reuse DTOs and testing fixtures.

**Why this priority**: Aligning input and output schemas reduces client-side translation bugs and simplifies automated contract testing, but it depends on having a stable response contract first.

**Independent Test**: Can be fully tested by posting or putting JSON that uses the flattened `id` field and confirming the API accepts it or ignores it (when server-generated) without requiring nested objects.

**Acceptance Scenarios**:

1. **Given** a client sends a request body containing `id` as a top-level property, **When** the API processes the request, **Then** the payload is validated successfully (or gracefully rejected with a descriptive error if `id` is not expected for that operation).
2. **Given** a client accidentally sends legacy `id.value` payloads, **When** the request is validated, **Then** the API responds with a clear validation message indicating the supported schema.

---

### User Story 3 - Contract-Focused Integration Tests (Priority: P3)

Developers maintaining the API need automated integration tests that hit the HTTP layer, assert concrete JSON payloads, and fail loudly when the contract drifts.

**Why this priority**: Once the schema is corrected, keeping it stable is the next most important investment. Contract tests guard against regressions without relying on manual verification.

**Independent Test**: Can be fully tested by running the integration suite and observing it fail when the JSON contract changes, independent of other features.

**Acceptance Scenarios**:

1. **Given** the integration suite runs in CI, **When** a developer alters serialization, **Then** a contract test fails with output showing the mismatch between expected and actual JSON.
2. **Given** a developer inspects test fixtures, **When** they review sample request/response pairs, **Then** the fixtures match the documented schema and clarify which fields are required or optional.

### Edge Cases

- How are responses handled when a character lacks a persisted identifier (e.g., creation failure before ID assignment)?
- What response does the API return if a payload includes both `id` and `id.value` fields?
- How does the system communicate validation errors when the `id` value is not a valid GUID?
- What happens if downstream consumers still expect the legacy nested structure—do they receive a deprecation notice or rely on versioned endpoints?
- How does the integration suite ensure snapshots remain up to date when legitimate schema changes are introduced?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: All character-related API responses MUST represent the identifier as a single `id` property containing the canonical string value (no nested `.value`).
- **FR-002**: System MUST remove the legacy `id.value` or similar nested serialization from every response, including aggregated lists and error payloads that echo submitted data.
- **FR-003**: Request validation MUST treat `id` as the authoritative field name whenever a client supplies or references an identifier.
- **FR-004**: System MUST reject payloads that mix legacy and new identifier shapes, returning actionable error messages that reference the supported schema.
- **FR-005**: System MUST document invalid identifier formats (e.g., not a GUID) via consistent validation errors surfaced to clients.
- **FR-006**: Integration tests MUST exercise create, retrieve, update, delete, and list flows using real HTTP calls and assert the exact JSON sent and received.
- **FR-007**: Integration tests MUST fail when serialized JSON deviates from approved fixtures or schemas, preventing unnoticed contract drift.
- **FR-008**: Integration tests MUST confirm that error responses for malformed JSON clearly describe the offending fields and do not expose implementation details.
- **FR-009**: Test data management MUST allow fixtures to be updated intentionally (e.g., via approved helpers) so legitimate schema changes can be reflected in a single place.
- **FR-010**: System MUST provide human-readable documentation or samples (e.g., in API.http or README) that mirror the tested schema for both requests and responses.

### Key Entities *(include if feature involves data)*

- **Character Representation**: The JSON payload shared between the API and clients, comprising `id`, `name`, and additional descriptive fields. Must now present `id` as a single scalar value across every endpoint.
- **Contract Test Fixture**: Canonical request/response examples consumed by integration tests to assert schema fidelity. Includes happy-path and validation-error payloads for each operation.
- **Identifier Validation Rule**: Business rule describing acceptable identifier formats and placement within payloads. Shared between runtime validation and integration assertions.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of API responses that include characters present a single-level `id` field, confirmed by automated contract tests.
- **SC-002**: Integration suite covers at least the five primary character flows (create, retrieve single, list, update, delete) and executes in under 3 minutes in CI, ensuring timely feedback.
- **SC-003**: Validation errors for malformed identifier payloads are returned within 1 second and include actionable guidance referencing `id` rather than internal field names.
- **SC-004**: Contract-focused integration tests detect any intentional JSON schema change before release (zero production regressions attributable to serialization mismatch in a release cycle).

## Assumptions

- Clients are willing to adopt the flattened `id` field without requiring version negotiation; no parallel legacy contract is maintained.
- Character identifiers remain GUID strings generated server-side, and clients may include them only when updating existing resources.
- All affected endpoints belong to the Character API; other bounded contexts are out of scope for this change.
- Integration tests can spin up required infrastructure (database, API host) similarly to existing suites without new tooling.
- Documentation samples (e.g., API.http) are treated as part of the contract and must stay synchronized with the integration fixtures.
