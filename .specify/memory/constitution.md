<!--
  SYNC IMPACT REPORT
  ==================
  Version Change: 1.0.0 → 1.0.1
  Date: 2025-10-31
  
  Modified Principles:
  - IV. Testing Standards: Clarified test naming conventions and structure requirements
  - VI. Infrastructure as Code: Removed immutable infrastructure requirement
  
  Added Sections: None
  
  Removed Sections: None
  
  Clarifications:
  - Test class naming: <ClassUnderTest>Should (e.g., CharacterServiceShould)
  - Test method naming: Capital_snake_case describing outcome, not conditions
  - Removed Arrange-Act-Assert comment requirement (pattern should be evident from code)
  - API technology corrected: ASP.NET Core Web API (not Azure Functions)
  - Removed infrastructure immutability requirement from IaC principle
  
  Template Alignment Status:
  ✅ plan-template.md - No changes required (general constitution gates still valid)
  ✅ spec-template.md - No changes required
  ✅ tasks-template.md - Test naming conventions now clarified
  
  Follow-up TODOs: None
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
- Comments explain WHY, not WHAT (code explains what)

**Rationale**: SOLID principles ensure maintainable, testable, and extensible code that can evolve without breaking existing functionality.

### II. Extreme Programming (XP) Practices

Development MUST follow XP core practices:

- **Test-Driven Development (TDD)**: Tests written first, code written to pass tests (see Principle IV)
- **Continuous Integration**: Frequent integration with automated builds and tests
- **Refactoring**: Continuous code improvement without changing behavior
- **Pair Programming**: Encouraged for complex features and knowledge sharing
- **Simple Design**: Simplest solution that works, avoiding over-engineering (YAGNI)
- **Collective Code Ownership**: Any team member can improve any part of the codebase
- **Sustainable Pace**: Maintain quality and avoid burnout

**Rationale**: XP practices reduce risk, improve quality, and enable rapid response to changing requirements while maintaining team productivity.

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

**Version**: 1.0.1 | **Ratified**: 2025-10-31 | **Last Amended**: 2025-10-31
