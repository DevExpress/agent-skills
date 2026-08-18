# Getting Started — WinForms PivotGridControl (DevExpress v26.1)

> **.NET Framework?** For .NET Framework 4.6.2+ project setup, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).


## NuGet Package

```
DevExpress.Win.PivotGrid
```

Install the package:

```powershell
dotnet add package DevExpress.Win.PivotGrid
```

**Assembly**: `DevExpress.XtraPivotGrid.v26.1.dll`  
**Primary namespace**: `DevExpress.XtraPivotGrid`

---

## Required `using` Statements

```csharp
using DevExpress.XtraPivotGrid; // PivotGridControl, PivotGridField, DataSourceColumnBinding
// For Excel data source (optional):
using DevExpress.DataAccess.Excel;
// For appearance helpers:
using DevExpress.Utils;
```

---

## Add PivotGridControl to a Form

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

public partial class Form1 : XtraForm
{
    PivotGridControl pivotGridControl1;

    public Form1()
    {
        InitializeComponent();
        pivotGridControl1 = new PivotGridControl { Dock = DockStyle.Fill };
        Controls.Add(pivotGridControl1);
    }
}
```

---

## Minimal Binding (IList / DataTable)

```csharp
// 1. Assign a data source
pivotGridControl1.DataSource = myBindingSource; // BindingSource, DataTable, List<T>, etc.

// 2. Enable Optimized engine (recommended)
pivotGridControl1.OptionsData.DataProcessingEngine =
    PivotDataProcessingEngine.Optimized;

// 3. Add fields via the shortcut helper
pivotGridControl1.Fields.AddDataSourceColumn("Country",   PivotArea.FilterArea);
pivotGridControl1.Fields.AddDataSourceColumn("Category",  PivotArea.RowArea);
pivotGridControl1.Fields.AddDataSourceColumn("OrderDate", PivotArea.ColumnArea);
pivotGridControl1.Fields.AddDataSourceColumn("Sales",     PivotArea.DataArea);
```

`AddDataSourceColumn(fieldName, area)` creates the field, sets its `DataBinding` to a
`DataSourceColumnBinding`, and adds it to the `Fields` collection in one call.

---

## Auto-Populate Fields with RetrieveFields

```csharp
// Assign the data source first, then call:
pivotGridControl1.RetrieveFields(PivotArea.FilterArea, false);
// All data-source columns are added to the Filter area with no specific ordering.
// The second parameter (true) would also show them immediately.
```

---

## Batch Update / Async

Always wrap multiple field/configuration changes in `BeginUpdate`/`EndUpdate`:

```csharp
pivotGridControl1.BeginUpdate();
try
{
    // configure fields, areas, captions …
}
finally
{
    pivotGridControl1.EndUpdate();
    // or: await pivotGridControl1.EndUpdateAsync();
}
```

### Async Mode

```csharp
pivotGridControl1.OptionsBehavior.UseAsyncMode = true;
// All data operations run on a background thread; UI stays responsive.
// Use EndUpdateAsync() / CollapseAllRowsAsync() / ExpandAllColumnsAsync() etc.
```

---

## Minimal Complete Example

```csharp
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

public partial class MainForm : XtraForm
{
    public MainForm()
    {
        InitializeComponent();

        var data = new List<SaleRecord>
        {
            new SaleRecord { Country = "USA",    Category = "Bikes",    Year = 2023, Sales = 12000 },
            new SaleRecord { Country = "Germany", Category = "Clothes", Year = 2023, Sales = 8500  },
            new SaleRecord { Country = "USA",    Category = "Bikes",    Year = 2024, Sales = 14000 },
        };

        pivotGridControl1.BeginUpdate();
        pivotGridControl1.OptionsData.DataProcessingEngine =
            PivotDataProcessingEngine.Optimized;
        pivotGridControl1.DataSource = data;
        pivotGridControl1.Fields.AddDataSourceColumn("Country",  PivotArea.FilterArea);
        pivotGridControl1.Fields.AddDataSourceColumn("Category", PivotArea.RowArea);
        pivotGridControl1.Fields.AddDataSourceColumn("Year",     PivotArea.ColumnArea);
        pivotGridControl1.Fields.AddDataSourceColumn("Sales",    PivotArea.DataArea);
        pivotGridControl1.EndUpdate();
    }
}

public class SaleRecord
{
    public string Country  { get; set; }
    public string Category { get; set; }
    public int    Year     { get; set; }
    public double Sales    { get; set; }
}
```

---

## Data Processing Engines

| Engine | `PivotDataProcessingEngine` value | Notes |
|---|---|---|
| Optimized | `Optimized` | Recommended. Supports window calculations and calculated bindings. |
| Legacy Optimized | `LegacyOptimized` | Fallback when OLAP or certain legacy features are needed. |

Set via:
```csharp
pivotGridControl1.OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized;
```

---

## Key API Reference

| Member | Description |
|---|---|
| `PivotGridControl.DataSource` | Assigns the data source |
| `PivotGridControl.DataMember` | Selects a table/member from the source |
| `PivotGridControl.Fields` | `PivotGridFieldCollection` — all fields |
| `Fields.AddDataSourceColumn(name, area)` | Shorthand field creation |
| `PivotGridControl.RetrieveFields(area, visible)` | Auto-create fields from data source columns |
| `PivotGridControl.BeginUpdate()` / `EndUpdate()` | Batch layout update |
| `PivotGridControl.EndUpdateAsync()` | Async batch update completion |
| `OptionsData.DataProcessingEngine` | Choose calculation engine |
| `OptionsBehavior.UseAsyncMode` | Enable background data processing |

---

## Source

- Overview: https://docs.devexpress.com/content/WindowsForms/3409?md=true
- PivotGridControl class: https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraPivotGrid.PivotGridControl?md=true
- Getting started walkthroughs: https://docs.devexpress.com/content/WindowsForms/404088?md=true
