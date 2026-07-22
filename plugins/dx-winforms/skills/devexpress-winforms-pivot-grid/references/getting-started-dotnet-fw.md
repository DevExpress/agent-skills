# Getting Started with the DevExpress WinForms PivotGrid (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, every other reference in this skill applies identically to both target frameworks.

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- DevExpress WinForms subscription via the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (designer-first). Run the installer, then create the project from the **DevExpress Template Gallery**, or drag `PivotGridControl` from the toolbox onto an `XtraForm`. References are added automatically.
2. **NuGet package** (recommended for source control / CI):

   ```powershell
   Install-Package DevExpress.Win.PivotGrid
   ```

## Designer Workflow (.NET Framework)

1. Drop a `PivotGridControl` on an `XtraForm`; set `Dock = Fill`.
2. Set `DataSource` via the smart tag. The **Data Source Configuration Wizard / typed DataSets** are available in .NET Framework projects (not in .NET SDK projects) — convenient for database binding.
3. Open **Run Designer** → assign fields to the Filter / Row / Column / Data areas and set summary types and formats.

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
