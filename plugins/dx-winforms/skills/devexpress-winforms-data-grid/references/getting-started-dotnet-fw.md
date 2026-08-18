# Getting Started with the DevExpress WinForms Data Grid (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, every other reference in this skill applies identically to both target frameworks.

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- DevExpress WinForms subscription via the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress packages from nuget.org
- A valid DevExpress license

## Install the NuGet Packages

```powershell
# SDK-style project
dotnet add package DevExpress.Win.Grid
dotnet add package DevExpress.Win.TreeList   # only if you use the TreeList

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.Grid
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the packages are also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

## Setup (.NET Framework)

The form is authored exactly as on .NET 8+ — see [Minimal Example](#minimal-example) below and [getting-started.md](getting-started.md).

> **Typed DataSets** are available in .NET Framework projects — and *not* in .NET SDK projects. If you need a typed dataset for a .NET project, generate it in a .NET Framework project and add the files to the .NET project (see the Constraints in SKILL.md).

## Required Assemblies (Manual Reference)

Prefer the NuGet packages (they pull all dependencies). If you reference assemblies directly, add (replace `26.1` with your version):

- `DevExpress.XtraGrid.v26.1.dll` (the control and all views)
- `DevExpress.XtraTreeList.v26.1.dll` (only if you use the TreeList)
- `DevExpress.XtraEditors.v26.1.dll`, `DevExpress.Utils.v26.1.dll`, `DevExpress.Data.v26.1.dll` (core dependencies)
- `DevExpress.Printing.v26.1.Core.dll` (only for print / export)

## Minimal Example

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();
        var grid = new GridControl { Dock = DockStyle.Fill };
        var view = new GridView(grid);
        grid.MainView = view;
        grid.ViewCollection.Add(view);
        Controls.Add(grid);
        grid.DataSource = LoadOrders();           // any IList / BindingList / DataTable
        view.OptionsView.ShowAutoFilterRow = true;
    }
}
```

See [getting-started.md](getting-started.md) for the full walkthrough and [data-binding.md](data-binding.md) for ServerMode/InstantFeedback and other binding modes.
