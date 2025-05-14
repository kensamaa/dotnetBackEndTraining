# Guide de Préparation aux Entretiens ASP.NET Core

## Table des Matières
- [Injection de Dépendances et Durées de Vie des Services](#injection-de-dépendances-et-durées-de-vie-des-services)
- [Concepts Clés de C# et .NET](#concepts-clés-de-c-et-net)
- [Entity Framework Core](#entity-framework-core)
- [Stratégies de Chargement des Données](#stratégies-de-chargement-des-données)
- [Le Mot-clé "using" en C#](#le-mot-clé-using-en-c)
- [Architecture ASP.NET Core](#architecture-aspnet-core)
- [Migrations de Base de Données](#migrations-de-base-de-données)
- [Patrons de Conception Courants](#patrons-de-conception-courants)
- [Performance et Optimisation](#performance-et-optimisation)
- [Sécurité en ASP.NET Core](#sécurité-en-aspnet-core)
- [Questions Fréquentes en Entretien](#questions-fréquentes-en-entretien)

## Injection de Dépendances et Durées de Vie des Services
ASP.NET Core propose trois durées de vie pour les services:

**Singleton**: Une seule instance est créée pour toute la durée de vie de l'application
- Utilisation: Services sans état, configuration, journalisation, caches mémoire
- Exemple: `services.AddSingleton<IMonService, MonService>();`

**Scoped (Délimité)**: Une instance est créée par requête HTTP (ou par étendue)
- Utilisation: DbContext, services dépendant du DbContext, données spécifiques à la requête
- Exemple: `services.AddScoped<IServiceEtudiant, ServiceEtudiant>();`

**Transient (Transitoire)**: Une nouvelle instance est créée chaque fois que le service est injecté
- Utilisation: Services légers sans état
- Exemple: `services.AddTransient<IValidateur, Validateur>();`

### Considérations Importantes
- **Piège courant**: Injection de services Scoped dans des Singletons (peut causer des fuites de mémoire)
- **IDisposable et using**: Garantit que les ressources non gérées (comme DbContext) sont correctement libérées
  - Exemple: `using var contexte = new ApplicationDbContext();`

### Conseils pour le Choix de Durée de Vie:
- Utilisez Singleton pour les services sans état et thread-safe
- Utilisez Scoped pour les contextes de base de données et services spécifiques aux requêtes
- Utilisez Transient pour les services légers sans état partagé

## Concepts Clés de C# et .NET

### Programmation Orientée Objet
- **Virtual**: Mot-clé qui permet à une méthode ou propriété d'être redéfinie dans les classes dérivées
  - Exemple: `public virtual ICollection<Etudiant> Etudiants { get; set; }`
  - Utilisé dans EF Core pour les propriétés de navigation pour activer le chargement différé

- **Sealed**: Empêche l'héritage d'une classe ou la redéfinition de méthodes spécifiques
  - Exemple: `public sealed class ClasseFinale {}`
  - Utilisez lorsque vous voulez empêcher la spécialisation d'une classe

- **Record**: Type de référence immuable avec sémantique d'égalité basée sur les valeurs (C# 9+)
  - Exemple: `public record Personne(string Prenom, string Nom);`
  - Idéal pour les DTO, événements de domaine et objets de valeur

- **Middleware**: Composants logiciels qui traitent les requêtes et réponses HTTP
  - Exemple: Authentification, CORS, gestion des exceptions
  - Configuré dans Program.cs à l'aide de `app.UseMiddleware<T>()` ou de méthodes d'extension comme `app.UseAuthentication()`

- **Interface**: Contrat qui définit un ensemble de méthodes et propriétés sans implémentation
  - Exemple: `public interface IDepot<T> { Task<T> ObtenirParIdAsync(int id); }`
  - Utilisé pour l'injection de dépendances et le couplage faible

### Programmation Asynchrone
- **Async/Await**: Modèle pour les opérations d'E/S non bloquantes
  - Exemple: `public async Task<Etudiant> ObtenirEtudiantAsync(int id) { return await _contexte.Etudiants.FindAsync(id); }`
  - Utilisez pour les opérations de base de données, requêtes HTTP, E/S de fichiers

- **Task**: Représente une opération asynchrone
  - Exemple: `Task<List<Etudiant>>` représente un résultat futur de type `List<Etudiant>`

- **Traitement Parallèle**: Utilisé pour les tâches liées au CPU
  - Exemple: `Parallel.ForEach(elements, element => TraiterElement(element));`
  - Utilisez PLINQ avec `AsParallel()` pour les requêtes LINQ parallèles

## Entity Framework Core

### IEnumerable vs IQueryable:
- **IEnumerable**: Collection en mémoire, filtres appliqués après récupération des données
- **IQueryable**: Traduit en requêtes SQL, filtres appliqués au niveau de la base de données
- Bonne pratique: Renvoyer IQueryable depuis les dépôts, IEnumerable depuis les services

### AsNoTracking: 
Désactive le suivi des modifications pour les requêtes en lecture seule
- Exemple: `_contexte.Etudiants.AsNoTracking().ToListAsync();`
- Améliore les performances lorsque vous n'avez pas besoin de mettre à jour les entités

### LINQ & EF Core:
- Écrire des requêtes contre les collections `DbSet<T>`
- Exemple: `_contexte.Etudiants.Where(e => e.Age > 18).OrderBy(e => e.Nom).ToListAsync()`
- Utilisez `Include()` pour le chargement hâtif des entités liées

### Propriétés de Navigation:
- Marquez comme `virtual` pour activer le chargement différé (nécessite Microsoft.EntityFrameworkCore.Proxies)
- Exemple: `public virtual Departement Departement { get; set; }`

## Stratégies de Chargement des Données

### Eager Loading (Chargement Hâtif):
- Charge les entités associées en même temps que l'entité principale dans une seule requête
- Utilise la méthode `Include()` et `ThenInclude()`
- Exemple: 
```csharp
_contexte.Etudiants
    .Include(e => e.Cours)
    .ThenInclude(c => c.Professeur)
    .ToListAsync();
```
- **Avantages**: Réduit le nombre de requêtes SQL, tout est chargé en une fois
- **Inconvénients**: Peut charger des données non nécessaires, utilisation excessive de mémoire

### Lazy Loading (Chargement Différé):
- Charge les entités associées à la demande, uniquement lorsqu'elles sont accédées
- Nécessite:
  1. Propriétés de navigation marquées comme `virtual`
  2. Package Microsoft.EntityFrameworkCore.Proxies
  3. Configuration: `optionsBuilder.UseLazyLoadingProxies()`
- Exemple:
```csharp
// La propriété Cours sera chargée uniquement lorsqu'elle est accédée
var etudiant = await _contexte.Etudiants.FindAsync(id);
foreach(var cours in etudiant.Cours) { // Déclenche une requête SQL ici
    Console.WriteLine(cours.Nom);
}
```
- **Avantages**: Charge uniquement ce qui est nécessaire, simplifie le code
- **Inconvénients**: Peut causer le problème N+1 (de nombreuses requêtes SQL individuelles)

### Explicit Loading (Chargement Explicite):
- Charge manuellement les entités associées après avoir chargé l'entité principale
- Exemple:
```csharp
var etudiant = await _contexte.Etudiants.FindAsync(id);
await _contexte.Entry(etudiant).Collection(e => e.Cours).LoadAsync();
await _contexte.Entry(etudiant).Reference(e => e.Adresse).LoadAsync();
```
- **Avantages**: Contrôle précis sur ce qui est chargé et quand
- **Inconvénients**: Code plus verbeux

## Le Mot-clé "using" en C#

### Déclaration using:
- Assure la libération automatique des ressources implémentant `IDisposable`
- Appelle la méthode `Dispose()` même en cas d'exception

### Syntaxes:
1. **Traditionnelle**:
```csharp
using (var connection = new SqlConnection(connectionString))
{
    // Utiliser connection...
} // Dispose() appelé automatiquement ici
```

2. **Simplifiée** (C# 8+):
```csharp
using var connection = new SqlConnection(connectionString);
// Utiliser connection...
// Dispose() appelé à la fin du bloc englobant
```

### Directive using:
- Importe un namespace pour simplifier les références aux types
- Exemple: `using System.Linq;` permet d'utiliser directement LINQ sans qualificateur

### using static:
- Importe les membres statiques d'une classe
- Exemple: `using static System.Math;` permet d'utiliser directement `Sqrt()` au lieu de `Math.Sqrt()`

### Global using (C# 10+):
- Importe des namespaces pour l'ensemble du projet
- Exemple: `global using System.Collections.Generic;`

## Architecture ASP.NET Core

### Modèle MVC:
- **Modèle**: Données et logique métier
- **Vue**: Représentation de l'interface utilisateur (Razor Pages, Blazor ou SPA)
- **Contrôleur**: Gère les requêtes HTTP, orchestre les modèles et les vues

### Contrôleurs API:
- Utilisez l'attribut `[ApiController]` et héritez de `ControllerBase`
- Exemple: `[Route("api/[controller]")] public class EtudiantsController : ControllerBase {}`

### Injection de Dépendances:

#### 1. Enregistrement des Services:
- **Dans Program.cs** (ASP.NET Core 6+):
```csharp
// Configuration des services
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IMonService, MonService>();
builder.Services.AddTransient<IValidateur, Validateur>();
builder.Services.AddSingleton<IConfiguration>(Configuration);
```

- **Dans Startup.cs** (ASP.NET Core 2-5):
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IMonService, MonService>();
    // Autres enregistrements...
}
```

#### 2. Types d'Injection de Dépendances:

##### A. Injection par Constructeur (la plus courante):
```csharp
public class EtudiantsController : ControllerBase
{
    private readonly IServiceEtudiant _serviceEtudiant;
    private readonly ILogger<EtudiantsController> _logger;

    // Les dépendances sont injectées lors de la création de l'instance
    public EtudiantsController(IServiceEtudiant serviceEtudiant, ILogger<EtudiantsController> logger)
    {
        _serviceEtudiant = serviceEtudiant;
        _logger = logger;
    }
}
```

##### B. Injection par Attribut `[FromServices]` (dans les méthodes d'action):
```csharp
public class EtudiantsController : ControllerBase
{
    // Aucune dépendance injectée par constructeur

    [HttpGet]
    public IActionResult Get([FromServices] IServiceEtudiant serviceEtudiant)
    {
        // Utilisation directe du service dans l'action
        var etudiants = serviceEtudiant.ObtenirTous();
        return Ok(etudiants);
    }
}
```

##### C. Service Locator Pattern (à utiliser avec précaution):
```csharp
public class MonService
{
    private readonly IServiceProvider _serviceProvider;

    public MonService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void MaMethode()
    {
        // Résolution de service à la demande
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Utilisation de dbContext...
    }
}
```

##### D. Injection de Vue dans Razor (pour les applications MVC):
```html
@inject IServiceEtudiant ServiceEtudiant
@inject IOptions<MesOptions> Options

<h2>Liste des étudiants</h2>
<ul>
    @foreach(var etudiant in ServiceEtudiant.ObtenirTous())
    {
        <li>@etudiant.NomComplet</li>
    }
</ul>

<p>Configuration: @Options.Value.Parametre</p>
```

##### E. Middleware Factory (pour les middlewares personnalisés):
```csharp
public class MonMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMonService _monService;

    public MonMiddleware(RequestDelegate next, IMonService monService)
    {
        _next = next;
        _monService = monService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Utilisation de _monService
        await _next(context);
    }
}

// Enregistrement
app.UseMiddleware<MonMiddleware>();
```

##### F. Injection dans HttpClientFactory (pour les clients HTTP):
```csharp
// Enregistrement
services.AddHttpClient<MonApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.exemple.com");
});

// Client avec injection
public class MonApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ApiOptions> _options;

    public MonApiClient(HttpClient httpClient, IOptions<ApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }
}
```

### Modèle d'Options:
- Accès typé à la configuration
- Exemple: `services.Configure<ParametresSmtp>(Configuration.GetSection("ParametresSmtp"));`

## Migrations de Base de Données

### Ajouter une Migration
```
cd Api
dotnet ef migrations add CreationInitiale --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj --output-dir Data/Migrations
```

### Appliquer les Migrations
```
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
```

### Autres Commandes Utiles

#### Générer un script SQL:
```
dotnet ef migrations script --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
```

#### Supprimer la dernière migration:
```
dotnet ef migrations remove --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
```

## Patrons de Conception Courants

### Repository Pattern:
- Abstrait la logique d'accès aux données
- Exemple:
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### CQRS (Command Query Responsibility Segregation):
- Sépare les opérations de lecture (Queries) des opérations d'écriture (Commands)
- Exemple:
```csharp
// Command
public class CreateEtudiantCommand : IRequest<int>
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
}

// Query
public class GetEtudiantByIdQuery : IRequest<EtudiantDto>
{
    public int Id { get; set; }
}
```

### Mediator:
- Réduit les dépendances directes entre les composants
- Souvent implémenté avec MediatR
- Exemple:
```csharp
// Handler
public class CreateEtudiantCommandHandler : IRequestHandler<CreateEtudiantCommand, int>
{
    private readonly IApplicationDbContext _context;
    
    public CreateEtudiantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> Handle(CreateEtudiantCommand request, CancellationToken cancellationToken)
    {
        var etudiant = new Etudiant { Nom = request.Nom, Prenom = request.Prenom };
        _context.Etudiants.Add(etudiant);
        await _context.SaveChangesAsync(cancellationToken);
        return etudiant.Id;
    }
}
```

## Performance et Optimisation

### Mise en Cache:
- **In-Memory Cache**: `services.AddMemoryCache();`
- **Distributed Cache**: Redis ou SQL Server
- Exemple:
```csharp
if (!_cache.TryGetValue(cacheKey, out List<Etudiant> etudiants))
{
    etudiants = await _context.Etudiants.ToListAsync();
    _cache.Set(cacheKey, etudiants, TimeSpan.FromMinutes(10));
}
```

### Compression des Réponses:
- Réduire la taille des réponses HTTP
- Configuration:
```csharp
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

### Optimisation d'EF Core:
- Projections: Utilisez `Select()` pour limiter les colonnes retournées
```csharp
var noms = await _context.Etudiants
    .Select(e => new { e.Nom, e.Prenom })
    .ToListAsync();
```
- Requêtes compilées: Réutilisez les requêtes fréquemment exécutées
```csharp
private static readonly Func<ApplicationDbContext, int, Task<Etudiant>> GetEtudiantById =
    EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
        context.Etudiants.FirstOrDefault(e => e.Id == id));
```

## Sécurité en ASP.NET Core

### Authentification et Autorisation:
- **JWT**: JSON Web Tokens pour l'authentification API
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Configuration...
    });
```

- **Policy-based Authorization**:
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
```

### Protection CSRF:
- Anti-forgery tokens pour protéger contre les attaques CSRF
```csharp
<form asp-action="Create">
    @Html.AntiForgeryToken()
    <!-- Form fields -->
</form>
```

### Sécurisation des Données:
- **Data Protection API**: Pour chiffrer les données sensibles
```csharp
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys\"))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));
```

### Questions Fréquentes en Entretien

#### Comment fonctionne l'injection de dépendances dans ASP.NET Core?
- ASP.NET Core utilise un conteneur d'injection de dépendances intégré (IServiceProvider)
- Les services sont enregistrés dans Program.cs/Startup.cs avec une durée de vie spécifique
- Le framework résout automatiquement les dépendances des constructeurs
- Vous pouvez injecter des services par constructeur, attribut [FromServices], @inject dans Razor, etc.

#### Expliquez la différence entre les services Scoped et Transient.
- **Scoped**: Une instance par requête, partagée au sein de cette requête
- **Transient**: Nouvelle instance à chaque fois, même au sein de la même requête

#### Qu'est-ce qu'un middleware dans ASP.NET Core?
- Composants logiciels qui forment un pipeline pour traiter les requêtes et réponses
- Chaque middleware peut: effectuer des actions avant/après le composant suivant, court-circuiter le pipeline ou passer le contrôle au composant suivant

#### Comment Entity Framework Core suit-il les modifications?
- DbContext maintient un ChangeTracker qui surveille les états des entités
- Les états incluent: Added (Ajouté), Modified (Modifié), Deleted (Supprimé), Unchanged (Inchangé), Detached (Détaché)
- SaveChanges() transforme les changements suivis en commandes SQL

#### Expliquez les modèles Repository et Unit of Work.
- **Repository**: Abstraction de la logique d'accès aux données
- **Unit of Work**: Suit les changements à travers plusieurs dépôts et les valide comme une seule transaction
- DbContext implémente déjà le modèle Unit of Work

#### Qu'est-ce que l'approche Clean Architecture?
- **Couches**: Domaine, Application, Infrastructure, Présentation
- Les dépendances vont vers l'intérieur, le domaine est indépendant des frameworks
- Utilisez des interfaces aux frontières entre les couches

#### Quels sont les avantages et inconvénients du Lazy Loading?
- **Avantages**: Simplifie le code, charge les données uniquement lorsque nécessaire
- **Inconvénients**: Peut causer le problème N+1 (multiples requêtes SQL), difficile à suivre/déboguer

#### Comment optimiser les performances des requêtes EF Core?
- Utilisez AsNoTracking() pour les requêtes en lecture seule
- Projetez les propriétés nécessaires avec Select()
- Utilisez Include() pour charger les données associées en une seule requête
- Employez des requêtes compilées pour les requêtes fréquentes

#### Qu'est-ce que le pattern CQRS et quand l'utiliser?
- Sépare les opérations de lecture et d'écriture
- **Avantages**: Scalabilité différente pour les lectures/écritures, modèles optimisés par cas d'utilisation
- **Quand l'utiliser**: Applications complexes avec des charges de lecture/écriture asymétriques

#### Comment gérer les exceptions dans ASP.NET Core?
- Middleware de gestion d'exceptions globale
- Filtres d'exception au niveau contrôleur ou action
- Try/catch pour la gestion personnalisée dans les méthodes spécifiques

#### Expliqez le principe de l'inversion de contrôle (IoC).
- Les modules de haut niveau ne dépendent pas des modules de bas niveau, mais d'abstractions
- Facilite les tests unitaires et la maintenance
- L'injection de dépendances est l'une des implémentations de l'IoC

#### Quelle est la différence entre AddTransient, AddScoped et AddSingleton?
- **AddTransient**: Nouvelle instance à chaque injection, idéal pour les services légers sans état
- **AddScoped**: Une instance par requête HTTP, idéal pour les services liés à la requête comme DbContext
- **AddSingleton**: Une seule instance pour toute l'application, idéal pour les services partagés sans état