# Application Startup Optimization — Code Snippets

## Enable Model Cache

Caches Application Model content after first startup:

```csharp
// In your WinForms Program.cs or Module constructor
winApplication.EnableModelCache = true;

// Optional: parallel loading and skip empty nodes
ModelCacheManager.UseMultithreadedLoading = true;
ModelCacheManager.SkipEmptyNodes = true;
```

## Override Type Collection Methods

Avoid reflection-based type scanning by returning known types directly:

```csharp
public sealed class MyModule : ModuleBase {
    protected override IEnumerable<Type> GetDeclaredExportedTypes() {
        return new[] {
            typeof(Customer),
            typeof(Order),
            typeof(Product)
        };
    }

    protected override IEnumerable<Type> GetDeclaredControllerTypes() {
        return new[] {
            typeof(MyCustomController),
            typeof(OrderProcessingController)
        };
    }

    protected override IEnumerable<Type> GetRegularTypes() {
        return new[] {
            typeof(IModelMyOptions)
        };
    }
}
```

## Disable Action Attribute Scanning

If you do not use `[Action]`-attributed methods on business objects, set this **before** `Application.Setup()` is called (e.g., in `Main()` or `Program.cs`):

```csharp
// Must be set before Application.Setup() — setting it later has no effect
ObjectMethodActionsViewController.Enabled = false;
```
