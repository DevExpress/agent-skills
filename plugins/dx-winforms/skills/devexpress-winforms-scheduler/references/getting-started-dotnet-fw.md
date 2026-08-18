# Getting Started with the DevExpress WinForms Scheduler (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, every other reference in this skill applies identically to both target frameworks.

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- DevExpress WinForms subscription via the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress packages from nuget.org
- A valid DevExpress license

## Install the NuGet Package

```powershell
# SDK-style project
dotnet add package DevExpress.Win.Scheduler

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.Scheduler
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the package is also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

## Setup (.NET Framework)

The control, its `SchedulerDataStorage`, the field mappings, and an optional `DateNavigator` are declared exactly as on .NET 8+ — see [getting-started.md](getting-started.md).

> **Typed DataSets** are available in .NET Framework projects (not in .NET SDK projects) for bound mode.

## Required Assemblies (Manual Reference)

Prefer the NuGet package (it pulls all dependencies). If you reference assemblies directly, add (replace `26.1` with your version):

- `DevExpress.XtraScheduler.v26.1.dll` (the control)
- `DevExpress.XtraScheduler.v26.1.Core.dll` (scheduler engine)
- `DevExpress.XtraEditors.v26.1.dll`, `DevExpress.Utils.v26.1.dll`, `DevExpress.Data.v26.1.dll` (core dependencies)
- `DevExpress.Printing.v26.1.Core.dll` (only for print / export)

## Minimal Unbound Example

```csharp
using DevExpress.XtraScheduler;
using DevExpress.XtraEditors;
using System;

public partial class MainForm : XtraForm {
    SchedulerControl schedulerControl1;
    SchedulerDataStorage storage;
    public MainForm() {
        InitializeComponent();
        storage = new SchedulerDataStorage();
        schedulerControl1.DataStorage = storage;
        // Unbound mode: no field mappings needed — add appointments in code.
        schedulerControl1.Start = DateTime.Today;
    }
}
```

> Field mappings apply only to **bound** mode. In unbound mode (above), appointments are created in code with `storage.CreateAppointment(...)` — no mappings required.

See [getting-started.md](getting-started.md) for the full setup and [data-binding.md](data-binding.md) for bound mode with field mappings.
