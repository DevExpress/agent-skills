# Getting Started

> **.NET Framework?** For .NET Framework 4.6.2+ project setup, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).


This reference covers the prerequisites, NuGet packages, and minimal code needed to put a DevExpress `GridControl` or `TreeList` on a form, bind it to data, and apply a skin. Read this once before any of the other references — every other topic assumes the setup described here is already in place.

## When to Use This Reference

- A new project needs DevExpress WinForms Grid or TreeList for the first time.
- You are choosing between umbrella `DevExpress.Win` vs. atomic per-control packages.
- The grid renders but appears unskinned or behaves like a blank `Form`.
- You need a minimal end-to-end sample to start customizing from.

## NuGet Packages

| Package | Contents |
|---|---|
| `DevExpress.Win.Grid` | `GridControl`, `GridView`, `BandedGridView`, `AdvBandedGridView`, `CardView`, `LayoutView`, `TileView`, `WinExplorerView`. |
| `DevExpress.Win.TreeList` | `TreeList`, `TreeListColumn`, `TreeListNode`. |
| `DevExpress.Win.Navigation` | Shared navigation primitives; transitive dependency of both above. |
| `DevExpress.Win.Printing` | Print preview and export-to-pdf/xls/xlsx/csv/html/rtf. Add when you call `ExportTo*` or `ShowPrintPreview`. |
| `DevExpress.Data` | Core data engine (transitive). Adds `EntityServerModeSource`, `LinqServerModeSource`, `VirtualServerModeSource`, `CriteriaOperator`. |
| `DevExpress.Win` *(umbrella)* | Adds most WinForms UI controls including Grid, TreeList, Editors, Bars, etc. Heavier; faster to start with, slower designer load. |
| `DevExpress.Win.Design` *(umbrella + extra design-time)* | `DevExpress.Win` plus additional design-time components. |

Atomic packages give 15–20% faster designer load over the umbrella package. Install them with `dotnet add package` (they are on nuget.org and in the local DevExpress Unified Component Installer feed). Keep all DevExpress NuGet versions aligned across the solution.

### Project file example

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Grid" Version="25.2.3" />
  <PackageReference Include="DevExpress.Win.TreeList" Version="25.2.3" />
  <PackageReference Include="DevExpress.Win.Printing" Version="25.2.3" />
</ItemGroup>
```

## Common Imports

```csharp
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
```

## Host Form

The grid renders on any `Form`, but DevExpress skin integration looks best on `XtraForm` (or `RibbonForm` when the form has a ribbon).

## Minimal Setup in Code

### GridControl + GridView

```csharp
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var grid = new GridControl { Dock = DockStyle.Fill };
        var view = new GridView(grid);
        grid.ViewCollection.Add(view);
        grid.MainView = view;
        Controls.Add(grid);

        grid.DataSource = LoadOrders();
        // Columns are auto-generated because
        // view.OptionsBehavior.AutoPopulateColumns is true by default.

        view.OptionsView.ShowGroupPanel = true;
        view.OptionsView.ShowAutoFilterRow = true;
        view.BestFitColumns();
    }

    static List<Order> LoadOrders() => new() {
        new Order { Id = 1, Customer = "Acme", Total = 199.95m, OrderDate = DateTime.Today },
        new Order { Id = 2, Customer = "Globex", Total = 49.00m, OrderDate = DateTime.Today.AddDays(-1) }
    };
}

public class Order {
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
}
```

### TreeList from a flat self-referenced list

```csharp
treeList1.KeyFieldName    = "Id";
treeList1.ParentFieldName = "ParentId";
treeList1.RootValue       = 0;
treeList1.DataSource      = LoadDepartments();
treeList1.ExpandAll();

static List<Dept> LoadDepartments() => new() {
    new Dept { Id = 1, ParentId = 0, Name = "Headquarters" },
    new Dept { Id = 2, ParentId = 1, Name = "Sales" },
    new Dept { Id = 3, ParentId = 1, Name = "Marketing" },
    new Dept { Id = 4, ParentId = 3, Name = "Digital" }
};
```

## ForceInitialize

The grid is lazy: views and columns are not fully created until the grid is shown. In `Form_Load` (or anywhere before the grid is painted), call `gridControl1.ForceInitialize()` if you need to access `view.Columns` or `view.MainView` immediately:

```csharp
private void MainForm_Load(object sender, EventArgs e) {
    gridControl1.ForceInitialize();
    gridControl1.MainView.RestoreLayoutFromXml("grid-layout.xml");
}
```

## Verify the Setup

Add a status-bar item that displays the focused row to confirm wiring works:

```csharp
gridView1.FocusedRowChanged += (s, e) => {
    var row = gridView1.GetFocusedRow() as Order;
    statusLabel.Text = row is null ? "No row" : $"Focused: {row.Customer}";
};
```

## Common Issues

- **Designer cannot resolve `GridControl`**: NuGet not installed or restored. Run `dotnet restore` and reload the designer.
- **Skin not applied**: `SetSkinStyle` called after the form is created. Move the call to the top of `Main`.
- **`view.Columns` empty in `Form_Load`**: call `gridControl1.ForceInitialize()` first.
- **`DevExpress.XtraGrid` namespace missing in a .NET project**: confirm `<TargetFramework>` is `net*-windows` (or .NET Framework) — DevExpress WinForms requires Windows target.
- **Designer is slow with a large grid**: switch from `DevExpress.Win` to atomic `DevExpress.Win.Grid` + `DevExpress.Win.TreeList`.

## Source Material

- `articles/controls-and-libraries/data-grid/index.md` — Data Grid overview (`xref:WindowsForms.3455`).
- `articles/controls-and-libraries/data-grid/getting-started-with-data-grid-and-views.md` — Getting started (`xref:WindowsForms.113894`).
- `articles/general-information/create-a-devexpress-powered-net-app-for-winforms-with-design-time-support.md` — DevExpress NuGet packages (`xref:WindowsForms.405090`).
- `articles/controls-and-libraries/tree-list/feature-center/data-binding/bound-mode.md` — TreeList bound mode (`xref:WindowsForms.116708`).
- `api/DevExpress.XtraGrid.GridControl.yml` — `GridControl` API.
- `api/DevExpress.XtraTreeList.TreeList.yml` — `TreeList` API.
