## life cycle

Singleton: one instance for app lifetime

Scoped: one instance per HTTP request

Transient: new instance each injection

### infos

Transient: new StudentService each injection. Good when lightweight.

IDisposable & using: ensures unmanaged resources (like DbContext) are disposed.

virtual vs sealed: make EF navigation properties virtual to enable lazy loading. sealed prevents inheritance.

Async/Parallel: use Task and await for non-blocking I/O. Use PLINQ or Parallel.ForEach for CPU-bound tasks.

IEnumerable vs IQueryable: IQueryable translates to SQL, defer execution. IEnumerable works in-memory.

AsNoTracking: use for read-only queries to improve performance.

LINQ & EF: write queries against DbSet<T>, filter, project, and materialize asynchronously.

## add migration

Add a new migration
cd Api
dotnet ef migrations add InitialCreate --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj --output-dir Data/Migrations

### 2. Apply the migration to create/update the database

dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
