# ASP.NET Core Interview Preparation Guide

## Table of Contents

- [Dependency Injection & Service Lifetimes](#dependency-injection--service-lifetimes)
- [Key C# & .NET Concepts](#key-c--net-concepts)
- [Entity Framework Core](#entity-framework-core)
- [ASP.NET Core Architecture](#aspnet-core-architecture)
- [Database Migrations](#database-migrations)
- [Common Interview Questions](#common-interview-questions)

## Dependency Injection & Service Lifetimes

ASP.NET Core has three service lifetimes:

- **Singleton**: One instance is created for the entire application lifetime

  - Use for: Stateless services, configuration, logging, memory caches
  - Example: `services.AddSingleton<IMyService, MyService>();`

- **Scoped**: One instance is created per HTTP request (or scope)

  - Use for: DbContext, services that depend on DbContext, request-specific data
  - Example: `services.AddScoped<IStudentService, StudentService>();`

- **Transient**: New instance is created each time the service is injected
  - Use for: Lightweight, stateless services
  - Example: `services.AddTransient<IValidator, Validator>();`

### Important Considerations

- **IDisposable & using**: Ensures unmanaged resources (like DbContext) are properly disposed when no longer needed

  - Example: `using var context = new ApplicationDbContext();`

- **Lifetime Selection Tips**:
  - Use Singleton for stateless, thread-safe services
  - Use Scoped for database contexts and request-specific services
  - Use Transient for lightweight services with no shared state

## Key C# & .NET Concepts

### Object-Oriented Programming

- **Virtual**: Keyword that allows a method or property to be overridden in derived classes

  - Example: `public virtual ICollection<Student> Students { get; set; }`
  - Used in EF Core for navigation properties to enable lazy loading

- **Sealed**: Prevents inheritance of a class or prevents overriding specific methods

  - Example: `public sealed class FinalClass {}`
  - Use when you want to prevent further specialization of a class

- **Record**: Immutable reference type with value-based equality semantics (C# 9+)

  - Example: `public record Person(string FirstName, string LastName);`
  - Great for DTOs, domain events, and value objects

- **Middleware**: Software components that handle HTTP requests and responses

  - Example: Authentication, CORS, exception handling
  - Configured in `Program.cs` using `app.UseMiddleware<T>()` or extension methods like `app.UseAuthentication()`

- **Interface**: Contract that defines a set of methods and properties without implementation
  - Example: `public interface IRepository<T> { Task<T> GetByIdAsync(int id); }`
  - Used for dependency injection and loose coupling

### Asynchronous Programming

- **Async/Await**: Pattern for non-blocking I/O operations

  - Example: `public async Task<Student> GetStudentAsync(int id) { return await _context.Students.FindAsync(id); }`
  - Use for database operations, HTTP requests, file I/O

- **Task**: Represents an asynchronous operation

  - Example: `Task<List<Student>>` represents a future result of type `List<Student>`

- **Parallel Processing**: Used for CPU-bound tasks
  - Example: `Parallel.ForEach(items, item => ProcessItem(item));`
  - Use PLINQ with `AsParallel()` for parallel LINQ queries

## Entity Framework Core

- **IEnumerable vs IQueryable**:

  - **IEnumerable**: In-memory collection, filters applied after data is retrieved
  - **IQueryable**: Translates to SQL queries, filters applied at database level
  - Best practice: Return `IQueryable` from repositories, `IEnumerable` from services

- **AsNoTracking**: Disables change tracking for read-only queries

  - Example: `_context.Students.AsNoTracking().ToListAsync();`
  - Improves performance when you don't need to update entities

- **LINQ & EF Core**:

  - Write queries against `DbSet<T>` collections
  - Example: `_context.Students.Where(s => s.Age > 18).OrderBy(s => s.LastName).ToListAsync()`
  - Use `Include()` for eager loading related entities

- **Navigation Properties**:
  - Mark as **virtual** to enable lazy loading (requires Microsoft.EntityFrameworkCore.Proxies)
  - Example: `public virtual Department Department { get; set; }`

## ASP.NET Core Architecture

- **MVC Pattern**:

  - **Model**: Data and business logic
  - **View**: UI representation (Razor Pages, Blazor, or SPAs)
  - **Controller**: Handles HTTP requests, orchestrates models and views

- **API Controllers**:

  - Use `[ApiController]` attribute and inherit from `ControllerBase`
  - Example: `[Route("api/[controller]")] public class StudentsController : ControllerBase {}`

- **Dependency Injection**:

  - Register services in Program.cs
  - Inject via constructor or `[FromServices]` attribute
  - Example: `public StudentsController(IStudentService studentService) { _studentService = studentService; }`

- **Options Pattern**:
  - Type-safe access to configuration
  - Example: `services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));`

## Database Migrations

### Adding a Migration

```bash
cd Api
dotnet ef migrations add InitialCreate --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj --output-dir Data/Migrations
```

### Applying Migrations

```bash
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
```

### Other Useful Commands

- **Generate SQL script**:

  ```bash
  dotnet ef migrations script --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
  ```

- **Remove last migration**:
  ```bash
  dotnet ef migrations remove --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
  ```

## Common Interview Questions

1. **Explain the difference between Scoped and Transient services.**

   - Scoped: One instance per request, shared within that request
   - Transient: New instance every time, even within the same request

2. **What is middleware in ASP.NET Core?**

   - Software components that form a pipeline to handle requests and responses
   - Each middleware can: perform actions before/after the next component, short-circuit the pipeline, or pass control to the next component

3. **How does Entity Framework Core track changes?**

   - DbContext maintains a ChangeTracker that monitors entity states
   - States include: Added, Modified, Deleted, Unchanged, Detached
   - SaveChanges() transforms tracked changes into SQL commands

4. **Explain Repository and Unit of Work patterns.**

   - Repository: Abstraction over data access logic
   - Unit of Work: Tracks changes across multiple repositories and commits them as a single transaction
   - DbContext already implements Unit of Work pattern

5. **What is the Clean Architecture approach?**
   - Layers: Domain, Application, Infrastructure, Presentation
   - Dependencies flow inward, domain is independent of frameworks
   - Use interfaces at boundaries between layers
