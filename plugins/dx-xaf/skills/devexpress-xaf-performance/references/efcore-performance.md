# EF Core Performance — Code Snippets

## Eager Loading with PreFetchReferenceProperties

Eliminates N+1 queries for reference properties (not collections):

```csharp
// Startup.cs
builder.ObjectSpaceProviders
    .AddSecuredEFCore(options => options.PreFetchReferenceProperties())
    .WithDbContext<MyDbContext>((serviceProvider, options) => {
        options.UseConnectionString(connectionString);
    });
```

## Query Splitting Behavior

For objects with many collection properties, split queries reduce data transfer:

```csharp
// Startup.cs
builder.ObjectSpaceProviders
    .AddSecuredEFCore(options => options.PreFetchReferenceProperties())
    .WithDbContext<MyDbContext>((serviceProvider, options) => {
        options.UseConnectionString(connectionString);
        options.UseSqlServer(o =>
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    });
```

**When to use split queries**: Many users, many permissions, many collection properties.
**When to use single queries**: Few users/permissions, fast local network.

## Change Tracking Proxies

XAF uses `ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues` with change-tracking proxies. All persistent properties **must** be `virtual`:

```csharp
public class Department : BaseObject {
    public virtual string Name { get; set; } // Must be virtual
    public virtual IList<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
}
```

## EF Core SQL Logging

Enable in `appsettings.json` to see generated queries:

```json
{
  "Logging": {
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

Alternatively, configure logging directly in the `DbContext`:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    base.OnConfiguring(optionsBuilder);
    optionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging(); // Includes parameter values in log output
}
```

> **Warning:** `EnableSensitiveDataLogging()` exposes query parameter values (including user data) in log output. Disable it before deploying to production.
