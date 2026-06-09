# Module Setup — Code Snippets

## Add Reports V2 in Startup

```csharp
// Program.cs / Startup.cs
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl.EF;

builder.Modules
    .AddReports(options => {
        // EF Core storage type
        options.ReportDataType = typeof(ReportDataV2);

        // Required for ShowInReport in Detail Views
        options.EnableInplaceReports = true;

        options.Events.OnBeforeShowPreview = context => {
            // context.ServiceProvider gives access to scoped services
        };
    });
```

For XPO projects, use the XPO storage type:

```csharp
using DevExpress.Persistent.BaseImpl;

builder.Modules
    .AddReports(options => {
        options.ReportDataType = typeof(ReportDataV2);
        options.EnableInplaceReports = true;
    });
```

## Platform Modules

- `ReportsModuleV2` — core module (required)
- `ReportsBlazorModuleV2` — Blazor UI integration
- `ReportsWindowsFormsModuleV2` — WinForms UI integration

Always include the base module and the platform module matching your UI.

## Register Report Storage in DbContext (EF Core)

```csharp
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext {
    public DbSet<ReportDataV2> ReportData { get; set; }
    // ... other entities
}
```

After adding `DbSet<ReportDataV2>`, run EF Core migration (or XAF schema update / EnsureCompatibility) so the reports table is created.

NuGet packages:
- Blazor: `DevExpress.ExpressApp.ReportsV2.Blazor`
- WinForms: `DevExpress.ExpressApp.ReportsV2.Win`
