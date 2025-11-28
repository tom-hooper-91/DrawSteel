<!--
  SYNC IMPACT REPORT
  ==================
  Version Change: 1.0.2 → 2.0.0
  Date: 2025-11-28
  
  Modified Principles:
  - II. Extreme Programming (XP) Practices → II. Simplicity & Ruthless Deletion
  
  Added Sections:
  - Simplicity Enforcement Checklist within Principle II
  
  Removed Sections:
  - XP practice requirements (TDD, CI, Pair Programming, Simple Design, Collective Ownership, Sustainable Pace)
  
  Templates Requiring Updates:
  - ✅ .specify/templates/plan-template.md (Constitution Check now enforces simplicity gate)
  - ✅ .specify/templates/tasks-template.md (Task guidance emphasizes deletion-first mindset)
  - ⚠ .specify/templates/commands/ (directory absent; confirm whether command guidance must be restored)
  
  Follow-up TODOs:
  - TODO(CONFIRM_COMMAND_TEMPLATES): Determine if `.specify/templates/commands/` should exist or if this requirement can be dropped from future amendments.
-->

# Draw Steel Constitution

## Core Principles

### I. Clean Code & SOLID Principles

All code MUST adhere to clean code practices and SOLID principles:

- **Single Responsibility Principle**: Each class/module has one reason to change
- **Open/Closed Principle**: Open for extension, closed for modification
- **Liskov Substitution Principle**: Derived types must be substitutable for base types
- **Interface Segregation Principle**: Clients should not depend on interfaces they don't use
- **Dependency Inversion Principle**: Depend on abstractions, not concretions
- Code MUST be self-documenting with clear naming and intent
- Functions/methods MUST be small, focused, and do one thing well
- **Comments explain WHY, not WHAT** (code explains what)

**Rationale**: SOLID principles ensure maintainable, testable, and extensible code that can evolve without breaking existing functionality.

#### Comment Anti-Patterns (FORBIDDEN)

The following comment patterns are **explicit violations** of this principle:

❌ **Task Tracking Comments**

```csharp
// T022: Implement Update method
// TODO: T015 - Add validation
public async Task<Character?> Update(CharacterId id, string name)
```

**Why forbidden**: Task tracking belongs in git commit messages, not source code. Use: `git commit -m "T022: Implement CharacterService.Update method"`

❌ **Obvious Operation Descriptions**

```csharp
// Validate GUID format
if (!Guid.TryParse(id, out _))
    return new BadRequestObjectResult("Invalid ID format");

// Null means not found
if (character is null)
    return new NotFoundResult();

// Return the character
return character;
```

**Why forbidden**: These comments describe WHAT the code obviously does. Clear code structure and naming make these comments redundant noise.

❌ **Test Structure Comments**

```csharp
[Test]
public async Task Return_character_when_found()
{
    // Arrange
    var characterId = new CharacterId(Guid.NewGuid());
    
    // Act
    var result = await _service.Get(characterId);
    
    // Assert
    Assert.That(result, Is.Not.Null);
}
```

**Why forbidden**: The Arrange-Act-Assert pattern should be self-evident from code organization (blank lines between sections, clear variable names). These comments are visual clutter.

❌ **Redundant Implementation Comments**

```csharp
// Create a new character instance
var character = new Character(characterId, name);

// Save to repository
await _repository.Save(character);

// Return the saved character
return character;
```

**Why forbidden**: The code already says exactly what it does. These comments add zero value.

#### Acceptable Comment Examples (WHY, Not WHAT)

✅ **Non-Obvious Technical Decisions**

```csharp
// MongoDB ReplaceOneAsync returns MatchedCount=1 even when document content is identical
// We rely on this to distinguish "found" vs "not found" rather than "changed" vs "unchanged"
return result.MatchedCount > 0;
```

✅ **Business Rule Rationale**

```csharp
// Idempotent delete: Always return 200 even if already deleted to avoid client confusion
// about whether the resource state is correct (it is - the resource doesn't exist)
return new OkResult();
```

✅ **API Design Decisions**

```csharp
// ArgumentException from domain layer maps to 400 Bad Request per REST conventions
// Business rule violations are client errors, not server errors
catch (ArgumentException ex)
{
    return new BadRequestObjectResult(ex.Message);
}
```

✅ **Performance or Security Considerations**

```csharp
// Eagerly load relationships to avoid N+1 query problem with lazy loading
var characters = await _context.Characters
    .Include(c => c.Abilities)
    .Include(c => c.Equipment)
    .ToListAsync();
```

**Summary**: If removing the comment would make a reviewer ask "Why did they do it this way?", it's a good comment. If removing the comment changes nothing because the code already explains itself, delete the comment.

### II. Simplicity & Ruthless Deletion

Always favor the smallest viable solution and delete code at every opportunity:

- **Start by removing**: Before writing new code, delete or consolidate dead, duplicate, or low-value code paths related to the change.
- **Bias toward primitives**: Prefer straightforward data structures and synchronous flows until a measurable requirement demands more complexity.
- **No speculative abstractions**: Interfaces, patterns, feature toggles, and extension points require at least two proven consumers or documented evidence that duplication costs more than the added indirection.
- **Every addition needs a deletion**: Pull requests MUST call out what was removed or simplified; if nothing can be removed, reviewers demand justification explaining why.
- **Dependency diet**: New packages or services are prohibited when an existing library/standard API satisfies the requirement. Removing obsolete dependencies is mandatory work, not backlog.
- **Sunset plans**: Temporary code (flags, migrations, compatibility layers) MUST include removal criteria, owner, and target date in the plan/tasks.

#### Simplicity Enforcement Checklist

- Can the requirement be met by configuration, documentation, or process instead of code? If yes, code changes are blocked.
- Does the change reduce total abstractions, files, or branches? If not, rewrite until it does or document an explicit exception.
- Is there a net negative diff in lines of code across the affected layer? If not, provide concrete reasoning for the additional surface area.
- Were unused endpoints, DTOs, or repositories evaluated for deletion as part of this change? Evidence of the review MUST appear in the plan/tasks.

**Rationale**: Smaller systems cost less to comprehend, review, and secure. Ruthless deletion keeps the Interaction-Driven Design architecture lean so that new contributors can ship features quickly without navigating abandoned scaffolding.

### III. Interaction-Driven Design (IDD) Architecture

Architecture MUST follow Interaction-Driven Design with clear layer separation:

- **Domain Layer**: Core business logic, entities, and domain services - technology-agnostic
- **Application Layer**: Use cases and orchestration - coordinates domain operations
- **Presentation Layer**: API or Web layer - selected per feature based on requirements
  - **API Layer**: RESTful endpoints for programmatic access
  - **Web Layer**: Blazor components for user interface
- **Infrastructure Layer**: External concerns (database, file system, external APIs)
- **Communication Rule**: Both API and Web layers MUST communicate with Domain through Application layer only
- **Dependency Rule**: Dependencies point inward - outer layers depend on inner layers, never reverse

**Rationale**: IDD enables independent testing of business logic, flexible presentation layer selection per feature, and clear boundaries that prevent tight coupling.

### IV. Testing Standards (NON-NEGOTIABLE)

Testing is mandatory and MUST follow these standards:

- **Testing Framework**: NUnit for all tests
- **Mocking Framework**: FakeItEasy for test doubles and mocks
- **Integration Testing**: Testcontainers for containerized dependencies (databases, message queues)
- **Test Pyramid Adherence**:
  - **Unit Tests**: Majority of tests - fast, isolated, test single units
  - **Integration Tests**: Moderate quantity - test component interactions and contracts
  - **E2E Tests**: Minimal - only for critical user pathways
- **Test Naming Conventions**:
  - **Test Class Names**: `<ClassUnderTest>Should` (e.g., `CharacterServiceShould`, `CreateCharacterShould`)
  - **Test Method Names**: Capital_snake_case describing the desired outcome, NOT the conditions
    - Example: `Return_character_id_when_valid`, NOT `WhenCreatingCharacter_WithValidData_ShouldReturnCharacterId`
    - Start with capital letter, use underscores to separate words
    - Focus on WHAT the result is, conditions are clear from test setup
  - **Test Structure**: Arrange-Act-Assert pattern MUST be evident from code structure
    - Do NOT add comments like `// Arrange`, `// Act`, `// Assert`
    - Code organization makes the pattern self-evident
  - No unclear magic values - use descriptive constants or variables
- **Test-First**: Tests written BEFORE implementation, must fail initially
- **Coverage**: Unit tests for all business logic, integration tests for cross-boundary operations

**Rationale**: Comprehensive testing prevents regressions, enables refactoring confidence, and serves as living documentation. Consistent naming conventions make tests immediately understandable. The test pyramid ensures fast feedback loops while maintaining sufficient integration coverage.

### V. Frontend Standards

Web layer MUST follow these standards:

- **Architecture**: Simple, responsive design focused on usability
- **User Experience**: Intuitive interfaces requiring minimal learning curve
- **Responsive Design**: Mobile-first approach, works across all device sizes
- **Minimal Dependencies**: Prefer built-in framework capabilities over external libraries
- **Accessibility**: WCAG 2.1 AA compliance where applicable
- **Performance**: Optimize for fast load times and smooth interactions

**Rationale**: Simple UX reduces user friction and support burden. Minimal dependencies reduce maintenance overhead and security surface area. Responsive design ensures broad accessibility.

### VI. Infrastructure as Code (NON-NEGOTIABLE)

All infrastructure MUST be defined as code:

- **Provider**: Azure cloud platform
- **IaC Tool**: Terraform exclusively
- **Provider Priority**: Use `azurerm` provider wherever possible; only use `azapi` provider when `azurerm` lacks required functionality
- **Version Control**: All infrastructure code in repository, versioned alongside application code
- **State Management**: Terraform state managed securely with appropriate backend configuration
- **Documentation**: Infrastructure changes documented with clear reasoning

**Rationale**: IaC ensures reproducible environments, reduces configuration drift, enables disaster recovery, and provides audit trail for infrastructure changes.

### VII. CI/CD & Deployment

Continuous Integration and Deployment MUST follow these standards:

- **CI/CD Platform**: GitHub Actions exclusively
- **Pipeline Requirements**:
  - Automated test execution on all pull requests
  - Build and containerization for deployable artifacts
  - Automated deployment to appropriate environments
- **Deployment Strategy**: Triggered on push to `main` branch
- **Quality Gates**: Tests must pass before merge and deployment
- **Container Registry**: DockerHub for container image storage
- **Environment Progression**: Changes flow through test → production pipeline

**Rationale**: Automated CI/CD reduces manual errors, ensures consistent deployments, maintains code quality through automated gates, and enables rapid delivery of features.

## Technology Stack

### Core Technologies

- **Language**: C# / .NET
- **Application Architecture**: Interaction-Driven Design (IDD)
  - **API**: ASP.NET Core Web API deployed to Azure Container Apps
  - **Application**: Use case orchestration layer
  - **Domain**: Core business logic and entities
  - **Infrastructure**: MongoDB for persistence, Azure-managed services
  - **Web**: Blazor for web UI components
- **Database**: MongoDB with persistent volume
- **Container Orchestration**: Docker Compose (local), Azure Container Apps (production)
- **Build Automation**: psake (PowerShell-based build tool)

### Testing Stack

- **Unit Testing**: NUnit
- **Mocking**: FakeItEasy
- **Integration Testing**: Testcontainers
- **Test Organization**: Tests.Unit and Tests.Integration projects

### Infrastructure & DevOps

- **Cloud Provider**: Microsoft Azure
- **IaC**: Terraform (azurerm provider preferred, azapi as fallback)
- **CI/CD**: GitHub Actions
- **Container Registry**: DockerHub
- **Deployment Target**: Azure Container Apps

### Dependency Philosophy

- **Minimal Dependencies**: Evaluate every dependency for necessity
- **Standard Libraries First**: Prefer framework/standard library solutions
- **Well-Maintained Only**: Dependencies must be actively maintained with good community support
- **Security**: Regular dependency updates and vulnerability scanning

## Development Workflow

### Feature Development Process

1. **Specification**: Feature defined in `.specify/specs/[###-feature-name]/spec.md`
2. **Planning**: Implementation plan created in `plan.md` with constitution compliance check
3. **Task Breakdown**: Detailed tasks in `tasks.md` organized by user story priority
4. **Test-First Development**:
   - Write failing tests (unit, integration as appropriate)
   - Implement minimum code to pass tests
   - Refactor while keeping tests green
5. **Code Review**: Pull request reviewed for constitution compliance and code quality
6. **Integration**: Merge to main triggers CI/CD pipeline
7. **Deployment**: Automated deployment to Azure after successful build and tests

### Code Review Requirements

All pull requests MUST:

- Pass all automated tests (unit and integration)
- Maintain or improve test coverage
- Follow clean code and SOLID principles
- Include self-documenting tests for new functionality
- Comply with architecture guidelines (IDD layer separation)
- Update relevant documentation
- Have descriptive commit messages

### Quality Gates

Before merging, code MUST:

- Pass all unit tests
- Pass all integration tests
- Have no critical security vulnerabilities
- Follow established coding standards (enforced by linters where applicable)
- Be reviewed and approved by at least one team member
- Have IaC changes reviewed for compliance with Terraform standards

### Branching Strategy

- **Main Branch**: Production-ready code, protected, requires PR approval
- **Feature Branches**: Named `[###-feature-name]` for traceability to specification
- **Branch Lifecycle**: Create → Develop → PR → Review → Merge → Delete

## Governance

This constitution supersedes all other development practices and guidelines. All development decisions MUST align with these principles.

### Amendment Process

1. **Proposal**: Amendment proposed with clear rationale and impact analysis
2. **Review**: Team reviews compatibility with existing codebase and principles
3. **Migration Plan**: If approved, create plan for bringing existing code into compliance
4. **Documentation**: Update constitution with new version number (semantic versioning)
5. **Propagation**: Update all dependent templates and documentation

### Versioning Policy

Constitution follows semantic versioning:

- **MAJOR**: Backward-incompatible changes (principle removal or redefinition)
- **MINOR**: New principles added or material expansions
- **PATCH**: Clarifications, wording improvements, non-semantic refinements

### Compliance Verification

- All PRs must verify constitution compliance during code review
- Plan templates include explicit Constitution Check gate
- Complexity or deviations must be documented and justified in plan
- Regular retrospectives assess adherence and identify improvement areas

### Living Document

This constitution evolves with the project. As we learn and grow, principles may be refined, but the core commitment to quality, testability, and maintainability remains constant.

**Version**: 2.0.0 | **Ratified**: 2025-10-31 | **Last Amended**: 2025-11-28
