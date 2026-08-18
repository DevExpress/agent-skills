# Getting Started with the DevExpress WinForms PivotGrid (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, every other reference in this skill applies identically to both target frameworks.

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- DevExpress WinForms subscription via the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress packages from nuget.org
- A valid DevExpress license

## Install the NuGet Package

```powershell
# SDK-style project
dotnet add package DevExpress.Win.PivotGrid

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.PivotGrid
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the package is also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

## Setup (.NET Framework)

The control is created, bound, and configured exactly as on .NET 8+ — see [getting-started.md](getting-started.md).

> **Typed DataSets** are available in .NET Framework projects (not in .NET SDK projects) — convenient for database binding.

## Required Assemblies (Manual Reference)

Prefer the NuGet package (it pulls all dependencies). If you reference assemblies directly, add (replace `26.1` with your version):

- `DevExpress.XtraPivotGrid.v26.1.dll` (the control)
- `DevExpress.XtraEditors.v26.1.dll`, `DevExpress.Utils.v26.1.dll`, `DevExpress.Data.v26.1.dll` (core dependencies)
- `DevExpress.PivotGrid.v26.1.Core.dll` (pivot engine)
- `DevExpress.Printing.v26.1.Core.dll` (only for print / export)

## Minimal Example

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

public partial class Form1 : XtraForm {
    PivotGridControl pivotGridControl1;
    public Form1() {
        InitializeComponent();
        pivotGridControl1.BeginUpdate();
        pivotGridControl1.DataSource = GetSalesData();
        pivotGridControl1.Fields.AddDataSourceColumn("Category", PivotArea.RowArea);
        pivotGridControl1.Fields.AddDataSourceColumn("Year",     PivotArea.ColumnArea);
        pivotGridControl1.Fields.AddDataSourceColumn("Sales",    PivotArea.DataArea);
        pivotGridControl1.EndUpdate();
    }
}
```

See [getting-started.md](getting-started.md) for the full setup and [data-binding.md](data-binding.md) for binding modes (DataSourceColumnBinding, ExpressionDataBinding, OLAP).
