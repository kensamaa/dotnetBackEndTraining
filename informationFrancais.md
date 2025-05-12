# Guide de Préparation aux Entretiens ASP.NET Core

## Table des Matières

- [Injection de Dépendances et Durées de Vie des Services](#injection-de-dépendances-et-durées-de-vie-des-services)
- [Concepts Clés de C# et .NET](#concepts-clés-de-c-et-net)
- [Entity Framework Core](#entity-framework-core)
- [Architecture ASP.NET Core](#architecture-aspnet-core)
- [Migrations de Base de Données](#migrations-de-base-de-données)
- [Questions Fréquentes en Entretien](#questions-fréquentes-en-entretien)

## Injection de Dépendances et Durées de Vie des Services

ASP.NET Core propose trois durées de vie pour les services:

- **Singleton**: Une seule instance est créée pour toute la durée de vie de l'application

  - Utilisation: Services sans état, configuration, journalisation, caches mémoire
  - Exemple: `services.AddSingleton<IMonService, MonService>();`

- **Scoped** (Délimité): Une instance est créée par requête HTTP (ou par étendue)

  - Utilisation: DbContext, services dépendant du DbContext, données spécifiques à la requête
  - Exemple: `services.AddScoped<IServiceEtudiant, ServiceEtudiant>();`

- **Transient** (Transitoire): Une nouvelle instance est créée chaque fois que le service est injecté
  - Utilisation: Services légers sans état
  - Exemple: `services.AddTransient<IValidateur, Validateur>();`

### Considérations Importantes

- **IDisposable et using**: Garantit que les ressources non gérées (comme DbContext) sont correctement libérées

  - Exemple: `using var contexte = new ApplicationDbContext();`

- **Conseils pour le Choix de Durée de Vie**:
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
  - Configuré dans `Program.cs` à l'aide de `app.UseMiddleware<T>()` ou de méthodes d'extension comme `app.UseAuthentication()`

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

- **IEnumerable vs IQueryable**:

  - **IEnumerable**: Collection en mémoire, filtres appliqués après récupération des données
  - **IQueryable**: Traduit en requêtes SQL, filtres appliqués au niveau de la base de données
  - Bonne pratique: Renvoyer `IQueryable` depuis les dépôts, `IEnumerable` depuis les services

- **AsNoTracking**: Désactive le suivi des modifications pour les requêtes en lecture seule

  - Exemple: `_contexte.Etudiants.AsNoTracking().ToListAsync();`
  - Améliore les performances lorsque vous n'avez pas besoin de mettre à jour les entités

- **LINQ & EF Core**:

  - Écrire des requêtes contre les collections `DbSet<T>`
  - Exemple: `_contexte.Etudiants.Where(e => e.Age > 18).OrderBy(e => e.Nom).ToListAsync()`
  - Utilisez `Include()` pour le chargement hâtif des entités liées

- **Propriétés de Navigation**:
  - Marquez comme **virtual** pour activer le chargement différé (nécessite Microsoft.EntityFrameworkCore.Proxies)
  - Exemple: `public virtual Departement Departement { get; set; }`

## Architecture ASP.NET Core

- **Modèle MVC**:

  - **Modèle**: Données et logique métier
  - **Vue**: Représentation de l'interface utilisateur (Razor Pages, Blazor ou SPA)
  - **Contrôleur**: Gère les requêtes HTTP, orchestre les modèles et les vues

- **Contrôleurs API**:

  - Utilisez l'attribut `[ApiController]` et héritez de `ControllerBase`
  - Exemple: `[Route("api/[controller]")] public class EtudiantsController : ControllerBase {}`

- **Injection de Dépendances**:

  - Enregistrez les services dans Program.cs
  - Injectez via le constructeur ou l'attribut `[FromServices]`
  - Exemple: `public EtudiantsController(IServiceEtudiant serviceEtudiant) { _serviceEtudiant = serviceEtudiant; }`

- **Modèle d'Options**:
  - Accès typé à la configuration
  - Exemple: `services.Configure<ParametresSmtp>(Configuration.GetSection("ParametresSmtp"));`

## Migrations de Base de Données

### Ajouter une Migration

```bash
cd Api
dotnet ef migrations add CreationInitiale --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj --output-dir Data/Migrations
```

### Appliquer les Migrations

```bash
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
```

### Autres Commandes Utiles

- **Générer un script SQL**:

  ```bash
  dotnet ef migrations script --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
  ```

- **Supprimer la dernière migration**:
  ```bash
  dotnet ef migrations remove --project ../Infrastructure/Infrastructure.csproj --startup-project Api.csproj
  ```

## Questions Fréquentes en Entretien

1. **Expliquez la différence entre les services Scoped et Transient.**

   - Scoped: Une instance par requête, partagée au sein de cette requête
   - Transient: Nouvelle instance à chaque fois, même au sein de la même requête

2. **Qu'est-ce qu'un middleware dans ASP.NET Core?**

   - Composants logiciels qui forment un pipeline pour traiter les requêtes et réponses
   - Chaque middleware peut: effectuer des actions avant/après le composant suivant, court-circuiter le pipeline ou passer le contrôle au composant suivant

3. **Comment Entity Framework Core suit-il les modifications?**

   - DbContext maintient un ChangeTracker qui surveille les états des entités
   - Les états incluent: Added (Ajouté), Modified (Modifié), Deleted (Supprimé), Unchanged (Inchangé), Detached (Détaché)
   - SaveChanges() transforme les changements suivis en commandes SQL

4. **Expliquez les modèles Repository et Unit of Work.**

   - Repository: Abstraction de la logique d'accès aux données
   - Unit of Work: Suit les changements à travers plusieurs dépôts et les valide comme une seule transaction
   - DbContext implémente déjà le modèle Unit of Work

5. **Qu'est-ce que l'approche Clean Architecture?**
   - Couches: Domaine, Application, Infrastructure, Présentation
   - Les dépendances vont vers l'intérieur, le domaine est indépendant des frameworks
   - Utilisez des interfaces aux frontières entre les couches
