# BikoPrime Backend - Architecture Guidelines

## Project Overview

BikoPrime is a backend API built with **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS (Command Query Responsibility Segregation)**, and **Test-Driven Development (TDD)**.

---

## Architecture Layers

The project is organized into **5 independent layers** with **one-way dependencies**:

```
Domain (no dependencies)
   ↓
Application (depends on Domain)
   ↓
Infrastructure + Persistence (depend on Application + Domain)
   ↓
API (depends on all - orchestrator only)
```

### 1. **BikoPrime.Domain**

**Responsibility**: Business entities, domain exceptions, and value objects.

**What belongs here**:
- ✅ Entities (User.cs, RefreshToken.cs)
- ✅ Domain Exceptions (DomainException.cs)
- ✅ Value Objects (LocationDto as immutable value)
- ✅ Domain Interfaces (IUserRepository contract - defined, not implemented)

**What DOES NOT belong here**:
- ❌ No external dependencies (Entity Framework, HTTP, Database)
- ❌ No service implementations
- ❌ No DTOs for API responses
- ❌ No configuration or middleware

**Example**:
```csharp
namespace BikoPrime.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    // ... business properties only
}
```

---

### 2. **BikoPrime.Application**

**Responsibility**: Use case orchestration, Commands/Queries, DTOs, and business rules validation.

**What belongs here**:
- ✅ DTOs (UserDto, RegisterRequestDto, AuthResponseDto)
- ✅ Commands (RegisterCommand, LoginCommand, etc.)
- ✅ Queries (RefreshTokenQuery)
- ✅ Command/Query Handlers (RegisterCommandHandler, etc.)
- ✅ Validators (RegisterCommandValidator using FluentValidation)
- ✅ Service Interfaces (ITokenService, IGoogleTokenValidator - contracts only)
- ✅ Repository Interfaces (IUserRepository, IRefreshTokenRepository - contracts only)
- ✅ ServiceCollection Extension: AddApplication()

**What DOES NOT belong here**:
- ❌ Database context or Entity Framework
- ❌ HTTP/Web-specific concerns
- ❌ Service implementations (those go in Infrastructure)
- ❌ Repository implementations (those go in Persistence)
- ❌ Middleware or Controllers

**Example - Command Handler**:
```csharp
namespace BikoPrime.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Orchestrate the use case
        // Validate, create user, generate tokens
        // Return DTO (never return Entity)
    }
}
```

**Example - DTOs (not Entities)**:
```csharp
namespace BikoPrime.Application.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // ... DTO properties for API response
}
```

---

### 3. **BikoPrime.Infrastructure**

**Responsibility**: External services, JWT generation, third-party integrations.

**What belongs here**:
- ✅ TokenService (JWT generation and validation)
- ✅ GoogleTokenValidator (validates Google OAuth tokens)
- ✅ Any external API clients
- ✅ ServiceCollection Extension: AddInfrastructure(IConfiguration)
- ✅ ServiceCollection Extension: AddJwtAuthentication(IConfiguration)

**What DOES NOT belong here**:
- ❌ Database context or repositories
- ❌ Entity Framework anything
- ❌ Middleware or Controllers
- ❌ Application business logic

**Example**:
```csharp
namespace BikoPrime.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] ?? "default";
        var googleClientId = configuration["Google:ClientId"] ?? "";

        services.AddScoped<ITokenService>(provider => new TokenService(jwtSecret));
        services.AddScoped<IGoogleTokenValidator>(provider => new GoogleTokenValidator(googleClientId));

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] ?? "default";

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = "BikoPrime",
                    ValidateAudience = true,
                    ValidAudience = "BikoPrimeClients",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
```

---

### 4. **BikoPrime.Persistence**

**Responsibility**: Database context, repository implementations, Entity Framework configuration, Identity setup.

**What belongs here**:
- ✅ BikoPrimeDbContext (Entity Framework DbContext)
- ✅ Repository Implementations (UserRepository, RefreshTokenRepository)
- ✅ Identity Configuration (AddIdentity<User, IdentityRole>)
- ✅ ServiceCollection Extension: AddPersistence(IConfiguration)
- ✅ EF Core migrations and configurations

**What DOES NOT belong here**:
- ❌ Business logic
- ❌ External services (JWT, Google OAuth)
- ❌ Middleware or Controllers
- ❌ API-specific concerns

**IMPORTANT**: The AddPersistence method must handle BOTH DbContext AND Identity registration:

```csharp
namespace BikoPrime.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=BikoPrimeDb;Trusted_Connection=true;";

        // DbContext
        services.AddDbContext<BikoPrimeDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Identity Configuration (database-aware, belongs in Persistence)
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<BikoPrimeDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
```

**Example - Repository Implementation**:
```csharp
namespace BikoPrime.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BikoPrimeDbContext _context;

    public UserRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
```

---

### 5. **BikoPrime.API**

**Responsibility**: HTTP entry point, controllers, middleware, orchestration of all layers.

**What belongs here**:
- ✅ Controllers (AuthController with endpoints)
- ✅ Middleware (ErrorHandlingMiddleware)
- ✅ Models for responses (ErrorResponse)
- ✅ Program.cs (orchestration only - no business logic)
- ✅ appsettings.json

**What DOES NOT belong here**:
- ❌ Database context or Entity Framework imports
- ❌ Service implementations
- ❌ Business logic or validations
- ❌ Repository implementations
- ❌ Identity configuration (that's Persistence's job)

**CRITICAL RULE**: API MUST NOT import `BikoPrimeDbContext` or any Entity Framework types.

**Example - Program.cs**:
```csharp
// ✅ CORRECT - API only calls extension methods
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();  // Application layer
builder.Services.AddPersistence(builder.Configuration);  // Persistence layer (handles DbContext + Identity)
builder.Services.AddInfrastructure(builder.Configuration);  // Infrastructure layer
builder.Services.AddJwtAuthentication(builder.Configuration);  // JWT configuration

// ❌ WRONG - Never do this in API:
// builder.Services.AddDbContext<BikoPrimeDbContext>(...);  // NO! This belongs in Persistence
// builder.Services.AddIdentity<User, IdentityRole>(...);  // NO! This belongs in Persistence
```

**Example - Controller**:
```csharp
namespace BikoPrime.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand { /* map from request */ };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), result);
    }
}
```

---

## Dependency Injection Pattern

Each layer registers itself via a ServiceCollection extension method:

### Order of Registration in Program.cs

```csharp
// Order matters! Each layer is independent
builder.Services.AddApplication();                          // Lowest: no dependencies on other custom layers
builder.Services.AddPersistence(builder.Configuration);     // Middle: depends on Application
builder.Services.AddInfrastructure(builder.Configuration);  // Middle: depends on Application
builder.Services.AddJwtAuthentication(builder.Configuration); // Middle: depends on Infrastructure

// No AddIdentityConfiguration() - it's part of AddPersistence()
```

### Extension Method Pattern

**Each extension reads its own configuration**:

```csharp
// ✅ CORRECT - Persistence reads connection string
public static IServiceCollection AddPersistence(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    // ...
}

// ✅ CORRECT - Infrastructure reads JWT secret
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    var jwtSecret = configuration["Jwt:Secret"];
    // ...
}

// ❌ WRONG - API reading configuration and passing it
var jwtSecret = builder.Configuration["Jwt:Secret"];
builder.Services.AddInfrastructure(jwtSecret, googleClientId);  // Bad!
```

---

## CQRS Pattern Implementation

### Commands (Write Operations)

Each command has:
1. **Command class** - Input data
2. **Validator** - FluentValidation rules
3. **Handler** - Business logic execution

```csharp
// Command
public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Name { get; set; }
    public string Email { get; set; }
    // ...
}

// Validator
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(6);
    }
}

// Handler
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Orchestrate use case
    }
}
```

### Queries (Read Operations)

Similar structure to commands:

```csharp
public class RefreshTokenQuery : IRequest<RefreshTokenResponseDto>
{
    public string RefreshToken { get; set; }
}

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, RefreshTokenResponseDto>
{
    public async Task<RefreshTokenResponseDto> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        // Read and return data
    }
}
```

---

## File Organization

```
BikoPrime/
├── src/
│   ├── BikoPrime.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   └── RefreshToken.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── BikoPrime.Application/
│   │   ├── DTOs/Auth/
│   │   │   ├── LocationDto.cs
│   │   │   ├── UserDto.cs
│   │   │   ├── RegisterRequestDto.cs
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── GoogleAuthRequestDto.cs
│   │   │   ├── AuthResponseDto.cs
│   │   │   ├── RefreshTokenRequestDto.cs
│   │   │   └── RefreshTokenResponseDto.cs
│   │   ├── Interfaces/
│   │   │   ├── ITokenService.cs
│   │   │   ├── IGoogleTokenValidator.cs
│   │   │   ├── IUserRepository.cs
│   │   │   └── IRefreshTokenRepository.cs
│   │   ├── Features/Auth/
│   │   │   ├── Register/
│   │   │   │   ├── RegisterCommand.cs
│   │   │   │   ├── RegisterCommandValidator.cs
│   │   │   │   └── RegisterCommandHandler.cs
│   │   │   ├── Login/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   ├── LoginCommandValidator.cs
│   │   │   │   └── LoginCommandHandler.cs
│   │   │   ├── GoogleAuth/
│   │   │   │   ├── GoogleAuthCommand.cs
│   │   │   │   ├── GoogleAuthCommandValidator.cs
│   │   │   │   └── GoogleAuthCommandHandler.cs
│   │   │   ├── Logout/
│   │   │   │   ├── LogoutCommand.cs
│   │   │   │   └── LogoutCommandHandler.cs
│   │   │   └── RefreshToken/
│   │   │       ├── RefreshTokenQuery.cs
│   │   │       ├── RefreshTokenQueryValidator.cs
│   │   │       └── RefreshTokenQueryHandler.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── BikoPrime.Infrastructure/
│   │   ├── Services/
│   │   │   ├── TokenService.cs
│   │   │   └── GoogleTokenValidator.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── BikoPrime.Persistence/
│   │   ├── Data/
│   │   │   └── BikoPrimeDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── UserRepository.cs
│   │   │   └── RefreshTokenRepository.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   │
│   └── BikoPrime.API/
│       ├── Controllers/Auth/
│       │   └── AuthController.cs
│       ├── Middlewares/
│       │   └── ErrorHandlingMiddleware.cs
│       ├── Models/
│       │   └── ErrorResponse.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
│
└── tests/
    └── BikoPrime.Tests/
        ├── Features/Auth/
        │   ├── Register/
        │   │   ├── RegisterCommandHandlerTests.cs
        │   │   └── RegisterCommandValidatorTests.cs
        │   └── Login/
        │       ├── LoginCommandHandlerTests.cs
        │       └── LoginCommandValidatorTests.cs
        └── Infrastructure/Services/
            └── TokenServiceTests.cs
```

---

## Key Rules

### ✅ DO

1. **Use interfaces for dependencies** - Services depend on abstractions, not implementations
2. **Each class in its own file** - No multiple classes per file
3. **DTO for API responses** - Never return Entities from controllers
4. **Validate in Application layer** - Use FluentValidation in validators
5. **Handle exceptions at middleware** - ErrorHandlingMiddleware catches all exceptions
6. **Register configuration in each layer** - Each extension reads its own settings
7. **Use MediatR for commands/queries** - Keeps separation of concerns
8. **Test handlers, not endpoints** - Unit tests on business logic

### ❌ DON'T

1. **❌ Never import DbContext in API** - API should not know about persistence
2. **❌ Never put business logic in Controller** - Use handlers
3. **❌ Never return Entities from API** - Use DTOs
4. **❌ Never configure DbContext in API** - That's Persistence's job
5. **❌ Never configure Identity in API** - That's Persistence's job
6. **❌ Never mix responsibilities** - Keep layers independent
7. **❌ Never create circular dependencies** - Follow the dependency graph
8. **❌ Never use static methods for DI** - Use extension methods on IServiceCollection

---

## Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                     BikoPrime.API                               │
│  (Controllers, Middleware, Program.cs - ORCHESTRATOR ONLY)     │
└──────────┬──────────────┬──────────────┬──────────────┬─────────┘
           │              │              │              │
           ▼              ▼              ▼              ▼
    ┌─────────────┐ ┌──────────────┐ ┌────────────────┐
    │Application  │ │Infrastructure│ │  Persistence   │
    │ (Commands,  │ │ (JWT, Google)│ │(DbContext,     │
    │ Queries)    │ │              │ │ Repos, Identity)
    └──────┬──────┘ └──────┬───────┘ └────────┬───────┘
           │              │                   │
           └──────────────┼───────────────────┘
                          │
                          ▼
                    ┌──────────────┐
                    │Domain        │
                    │(Entities,    │
                    │ Exceptions)  │
                    └──────────────┘
```

---

## Testing Strategy

- **Unit Tests**: Test handlers, validators, services
- **Using XUnit + Moq**: Mock dependencies
- **20+ test cases**: Register, Login, Token validation
- **Never test endpoints**: Test business logic only

---

## Important Notes

1. **Configuration Management**: Each layer reads its own configuration via IConfiguration
2. **No Circular Dependencies**: Always respect the dependency direction
3. **Clean Separation**: If a class imports something from a higher layer, architecture is broken
4. **Interface-Based Design**: All dependencies use interfaces (abstractions)
5. **Single Responsibility**: Each class has one reason to change

---

## Common Violations & How to Fix Them

| Violation | ❌ Wrong | ✅ Right |
|-----------|---------|---------|
| API imports DbContext | `using BikoPrime.Persistence.Data;` | Don't import persistence in API |
| API configures DbContext | `services.AddDbContext<>()` in API | Call `AddPersistence()` from API |
| API configures Identity | `services.AddIdentity<>()` in API | Handle in `AddPersistence()` |
| Business logic in Controller | Logic in `Register()` method | Create RegisterCommandHandler |
| Returning Entity from API | `return user;` (Entity) | Return `userDto` (DTO) |
| Multiple classes per file | 2+ classes in 1 file | 1 class per file |
| Static DI registration | `ServiceProvider.Add()` | `services.AddX()` extension |
| Skipping validation layer | Direct handler logic | Create validator class |
| API reading config | `var secret = config["Jwt:Secret"]` | Pass `IConfiguration` to extension |

---

## Summary

**BikoPrime follows strict Clean Architecture + DDD principles**:
- **Domain**: Pure business models, zero external dependencies
- **Application**: Use cases (Commands/Queries), DTOs, validators
- **Infrastructure**: External services (JWT, Google OAuth)
- **Persistence**: Database access (DbContext, Repositories, Identity)
- **API**: HTTP entry point, controllers, middleware

**Each layer is independent and testable. Never violate the dependency graph.**
