# Business Model — Registration, Seeding & Migrations

Code snippets for registering entity types, supplying initial data, and configuring EF Core migrations.

---

## Supplying Initial Data (Updater)

```csharp
// File: MySolution.Module/DatabaseUpdate/Updater.cs
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion)
        : base(objectSpace, currentDBVersion) { }

    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        if (ObjectSpace.GetObjectsQuery<Department>(true).FirstOrDefault(d => d.Name == "HQ") == null) {
            var dept = ObjectSpace.CreateObject<Department>();
            dept.Name = "HQ";
        }
        ObjectSpace.CommitChanges();
    }
}
```

## Registering External Types (XPO)

When your XPO business classes live in a separate library (not in the module project), register them explicitly:

```csharp
using DevExpress.ExpressApp;

public sealed class MySolutionModule : ModuleBase {
    public MySolutionModule() {
        AdditionalExportedTypes.AddRange(
            new Type[] { typeof(MyLibrary.Customer), typeof(MyLibrary.Order) });
    }
}
```

## Exporting Types from a Reusable Module

When building a reusable module, override `GetDeclaredTypes()` to export your entity types to any application that references the module:

```csharp
using DevExpress.ExpressApp;

public sealed class MyReusableModule : ModuleBase {
    protected override IEnumerable<Type> GetDeclaredTypes() {
        return new[] {
            typeof(Invoice),
            typeof(InvoiceLine)
        };
    }
}
```

> `GetDeclaredTypes()` is the preferred approach for reusable modules. `AdditionalExportedTypes` (populated in the constructor) is an alternative for ad-hoc registration.

## EF Core Migrations Setup

```csharp
// 1. Disable automatic schema update in ApplicationBuilder:
builder.ObjectSpaceProviders
    .AddEFCore(options => options.SchemaUpdateOptions.DisableUpdateSchema = true)
    .WithDbContext<MySolutionDbContext>((app, options) => { /* ... */ });

// 2. Add migration via Package Manager Console:
// add-migration MigrationName -StartupProject "MySolution.Module" -Project "MySolution.Module"
// update-database -StartupProject "MySolution.Module" -Project "MySolution.Module"
```

## EF Core DbContext Registration

Every EF Core entity needs a `DbSet<T>` property in the application's `DbContext`:

```csharp
public class MySolutionDbContext : DbContext {
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Note> Notes { get; set; }
}
```

Ensure `options.UseChangeTrackingProxies()` is configured in your `DbContext` setup for XAF proxy-based change tracking.

## CheckCompatibilityType and EF Core Migrations

`XafApplication.CheckCompatibilityType` (namespace `DevExpress.ExpressApp`) controls how XAF validates the database schema on startup:

| Value | Behavior |
|-------|----------|
| `CheckCompatibilityType.DatabaseSchema` | Compares the database schema against the business model and can auto-update tables/columns. **Not suitable for production with EF Core migrations** — it bypasses migration history. |
| `CheckCompatibilityType.ModuleInfo` | Compares module assembly versions stored in the `ModuleInfo` table against the running application. Triggers `DatabaseVersionMismatch` when versions differ. |

When using EF Core migrations, disable XAF's automatic schema update and rely on migrations exclusively:

```csharp
builder.ObjectSpaceProviders
    .AddEFCore(options => options.SchemaUpdateOptions.DisableUpdateSchema = true)
    .WithDbContext<MySolutionDbContext>((app, options) => { /* ... */ });
```

Set `CheckCompatibilityType` to `ModuleInfo` so XAF detects version mismatches without attempting to modify the schema directly — all schema changes flow through `dotnet ef` migrations instead.

## Reverse-Engineering an Existing Database (Database First)

Use `dotnet ef dbcontext scaffold` (or `Scaffold-DbContext` in Package Manager Console) to generate EF Core entity classes from existing tables:

```shell
dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir MyModels \
  --project MySolution.Module \
  --context MyScaffoldedDbContext \
  --tables Customer,Order \
  --data-annotations \
  --no-onconfiguring
```

### Required XAF Adaptations

The scaffolded classes are plain EF Core entities. Adapt them for XAF:

1. **Make all properties `virtual`** — required for `UseChangeTrackingProxies()` change tracking.
2. **Replace `List<T>` with `ObservableCollection<T>`** in collection navigation properties so XAF UI updates work correctly.
3. **Inherit from `BaseObject`** (or implement `IXafEntityObject` + `IObjectSpaceLink` on the class) so XAF lifecycle hooks (`OnCreated`, `OnLoaded`, `OnSaving`) are available and the entity gets a Guid key. If you keep the original key, annotate it with `[Key]` instead.
4. **Move `DbSet<T>` declarations** into your application's main `DbContext` (the one registered with XAF), or replace the scaffolded `DbContext` entirely.
5. **Add `[DefaultClassOptions]`** to entities that should appear in navigation.
6. **Update the namespace** to match your module's `BusinessObjects` namespace.
7. **Configure the `DbContext`** — add `HasChangeTrackingStrategy(ChangingAndChangedNotificationsWithOriginalValues)` and `UsePropertyAccessMode(PreferFieldDuringConstruction)` in `OnModelCreating`.

```csharp
using System.Collections.ObjectModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

[DefaultClassOptions]
public class Customer : BaseObject {
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual IList<Order> Orders { get; set; } = new ObservableCollection<Order>();
}
```
