# Authentication Providers, Security Tiers & Audit Trail — Code Snippets

## Adding OAuth2 (Blazor)

```csharp
// Startup.cs
builder.Security
    .UseIntegratedMode(options => { /* ... */ })
    .AddPasswordAuthentication()
    .AddWindowsAuthentication(options => {
        options.CreateUserAutomatically((objectSpace, user) => {
            var defaultRole = objectSpace.FirstOrDefault<PermissionPolicyRole>(
                r => r.Name == "Default");
            ((ApplicationUser)user).Roles.Add(defaultRole);
        });
    });

// Add external providers
services.AddAuthentication()
    .AddMicrosoftAccount(options => {
        options.ClientId = Configuration["OAuth:Microsoft:ClientId"];
        options.ClientSecret = Configuration["OAuth:Microsoft:ClientSecret"];
        options.CallbackPath = "/signin-microsoft";
    })
    .AddGoogle(options => {
        options.ClientId = Configuration["OAuth:Google:ClientId"];
        options.ClientSecret = Configuration["OAuth:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
        options.Scope.Add("email");
        options.Scope.Add("profile");
    });
```

## Security Tiers

| Tier | Description | When to Use |
|------|-------------|-------------|
| **Integrated** | Security runs in the same process as the app and is enforced at the data access layer | Default for trusted deployments and server-side Blazor |
| **Middle Tier** | Security runs on a separate server; client communicates via service API and does not access DB directly | Distributed clients and sensitive data scenarios |
| **UI Level** | Client-side UI visibility only | UX convenience only; not a security boundary |

```csharp
// Integrated (default):
builder.Security.UseIntegratedMode(options => { /* ... */ });

// Middle Tier (EF Core):
builder.Security.UseMiddleTierMode(options => {
    options.BaseAddress = new Uri("https://localhost:5001/");
});
```

Use Middle Tier when the client executable is distributed and direct database access must be blocked.

UI Level configuration can hide actions or fields but does not protect data from direct database/API access.

## Audit Trail (EF Core and XPO)

```csharp
// EF Core
builder.Modules.AddAuditTrailEFCore();

// XPO
builder.Modules.AddAuditTrailXpo();

// Optional custom audit item type
builder.Modules.AddAuditTrailEFCore(options => {
    options.AuditDataItemPersistentType = typeof(AuditDataItemPersistent);
});
```

### Tracked Changes

| Change Type | Description |
|-------------|-------------|
| `ObjectCreated` | Object created |
| `InitialValueAssigned` | Initial value set before first save |
| `ObjectChanged` | Property value changed |
| `ObjectDeleted` | Object deleted |
| `AddedToCollection` | Object added to collection |
| `RemovedFromCollection` | Object removed from collection |

Audit data is stored in the application database and can include authentication-related events depending on configuration.
