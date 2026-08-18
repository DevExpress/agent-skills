# Columns

This reference covers everything about Grid and TreeList columns — definition in `InitializeComponent()` and in code, auto-generation, unbound columns, bands in `BandedGridView` and `AdvBandedGridView`, runtime customization on events, and the `FieldName` vs `Name` vs `ColumnEdit` triangle.

> **WinForms note**: there is no XAML. Columns are declared in `Form.Designer.cs` (`InitializeComponent`) or created directly in code. When the developer asks for "XAML columns", interpret it as columns declared in the designer file or in code.

## When to Use This Reference

- Declaring a grid's columns in `MainForm.Designer.cs`.
- Generating columns at runtime from the data source.
- Adding unbound (calculated or externally supplied) columns.
- Configuring multi-row column headers (bands) in `BandedGridView` / `AdvBandedGridView`.
- Differentiating `FieldName`, `Name`, `Caption`, `ColumnEditName`, `ColumnEdit`.
- Customizing individual columns at runtime via events.

## Column Classes

| View | Column class | Notes |
|---|---|---|
| `GridView` | `GridColumn` | Single-row column header. |
| `BandedGridView` / `AdvBandedGridView` | `BandedGridColumn` + `GridBand` | Columns belong to bands; bands can nest. |
| `CardView` | `GridColumn` | One row per data-source field; "columns" are fields inside a card. |
| `LayoutView` | `LayoutViewColumn` + `LayoutViewField` | Field has independent positioning inside the template card. |
| `TileView` | `TileViewColumn` | Manages content templates in tiles. |
| `WinExplorerView` | `WinExplorerViewColumn` | Maps fields to roles (small text, large text, image, etc.). |
| `TreeList` | `TreeListColumn` | Same shape as `GridColumn`. |

The cornerstone APIs (`Columns`, `OptionsColumn`, `ColumnEdit`, `FieldName`, `Caption`, `Visible`, `DisplayFormat`, `Width`) live on the shared base classes and behave the same across views.

## Automatic Column Generation

When `AutoPopulateColumns` is `true` (default), the view creates a column for each public field of the bound data source the first time the source is assigned and the column collection is empty.

```csharp
gridView1.OptionsBehavior.AutoPopulateColumns = true;   // default
gridControl1.DataSource = orders;                       // columns auto-created
```

To prevent generation of a specific property, decorate it with `[Display(AutoGenerateField = false)]`:

```csharp
public class Customer {
    public int    Id { get; set; }
    public string Name { get; set; } = "";
    [Display(AutoGenerateField = false)]
    public string Notes { get; set; } = "";   // no column generated
}
```

You can also use `[Display(Name = "...")]` for caption, `[DisplayFormat]` for format, and `[Browsable(false)]` to hide the column from the customization form.

### Regenerate after a data-source change

`AutoPopulateColumns` only auto-creates columns when the column collection is empty. To rebuild columns when swapping data sources, call `PopulateColumns()`:

```csharp
gridControl1.DataSourceChanged += (s, e) => {
    if (gridControl1.MainView is ColumnView v) v.PopulateColumns();
};
```

`PopulateColumns` has overloads for `DataTable`, `IList`, `DataColumnInfo[]`, and code-first types (with `PopulateColumnsParameters`).

## Define Columns in Code

### Add bound columns by field name

```csharp
gridView1.OptionsBehavior.AutoPopulateColumns = false;
gridView1.Columns.AddVisible("OrderDate", "Order Date");
gridView1.Columns.AddVisible("Customer");
var totalCol = gridView1.Columns.AddVisible("Total");
totalCol.DisplayFormat.FormatType   = FormatType.Numeric;
totalCol.DisplayFormat.FormatString = "c2";
```

`AddVisible` returns a `GridColumn` already visible; `AddField(name)` adds an invisible column and returns it.

### Add a fully constructed column

```csharp
var col = new GridColumn {
    Name       = "colTotal",
    FieldName  = "Total",
    Caption    = "Total",
    Visible    = true,
    VisibleIndex = 2
};
gridView1.Columns.Add(col);
```

### Access columns

```csharp
GridColumn col1 = gridView1.Columns["colTotal"];               // by Name (component name)
GridColumn col2 = gridView1.Columns.ColumnByFieldName("Total");// by data field
```

The `Columns[]` indexer is keyed by `Name` (component name) — not `FieldName`. Use `ColumnByFieldName` to look up by field. Cache typed references whenever possible.

## `FieldName` vs `Name` vs `Caption`

| Member | Purpose |
|---|---|
| `GridColumn.Name` | Component/identity name; default is the variable name used in the designer file (e.g., `colOrderDate`). Use it in `Columns[name]`. |
| `GridColumn.FieldName` | Data-source field this column is bound to. Unbound columns set this to a unique value not present in the data source. |
| `GridColumn.Caption` | Text shown in the column header. Defaults to a humanized `FieldName`. |
| `GridColumn.ColumnEditName` | The name of the `RepositoryItem` (from the grid's repository) used as in-place editor. Synchronized with `ColumnEdit`. |
| `GridColumn.ColumnEdit` | The actual `RepositoryItem` instance — see [cell-display-and-editing.md](cell-display-and-editing.md). |

> Binding two columns to the same `FieldName` is **not** supported and produces side effects (filter, sort, group all break). Make every bound column's `FieldName` unique.

## Unbound Columns

Unbound columns display custom values calculated on the fly or supplied via an event. They participate in sort, filter, group, summary, and export like bound columns.

### Setup Checklist

1. Add a column to the view.
2. Set `FieldName` to a unique value that does **not** appear in the data source.
3. Set `UnboundDataType` to the column data type (`int`, `decimal`, `DateTime`, `string`, etc.).
4. Populate the column via `UnboundExpression` (expression-based) or `CustomUnboundColumnData` (event-based).

### Expression-based (`UnboundExpression`)

```csharp
var totalCol = gridView1.Columns.AddVisible("Total");
totalCol.UnboundDataType = typeof(decimal);
totalCol.UnboundExpression = "[Quantity] * [UnitPrice] * (1 - [Discount])";
totalCol.OptionsColumn.AllowEdit = false;
```

The expression language is the DevExpress criteria/expression syntax — see the *Criteria Language Syntax* topic.

### Event-based (`CustomUnboundColumnData`)

```csharp
var noteCol = gridView1.Columns.AddVisible("CustomNote");
noteCol.UnboundDataType = typeof(string);

var unboundStore = new Dictionary<int, string>();
gridView1.CustomUnboundColumnData += (s, e) => {
    if (e.Column != noteCol) return;
    if (e.IsGetData) {
        e.Value = unboundStore.TryGetValue(e.ListSourceRowIndex, out var v) ? v : null;
    }
    if (e.IsSetData) {
        unboundStore[e.ListSourceRowIndex] = e.Value as string;
    }
};
```

Use `ListSourceRowIndex` (data-source index) — not the visible `RowHandle` — to address the underlying row.

### Important

The grid requires a data source. For 100 % calculated grids, use `UnboundSource` to set `RowCount` (see [data-binding.md](data-binding.md)).

## Layout

### Column order

- `GridColumn.VisibleIndex` — position among visible columns. `-1` hides the column without removing it.
- `GridColumn.Visible` — show/hide. Hidden columns still serialize and participate in sort/group/filter when set in code.
- `gridView1.OptionsView.ColumnAutoWidth` — controls horizontal sizing. `true` stretches columns to fill width; `false` keeps explicit `Width` values and shows a horizontal scrollbar when needed.

### Width

- `GridColumn.Width` — explicit width in pixels.
- `GridColumn.MinWidth` / `MaxWidth` — limits.
- `gridView1.BestFitColumns()` — auto-size to content.
- `gridView1.BestFitColumns(true)` — include hidden rows when sampling.
- `GridColumn.BestFit()` — single column.

### Fixed columns

Pin columns to the left/right edge:

```csharp
gridView1.Columns["OrderID"].Fixed = FixedStyle.Left;
gridView1.Columns["Total"].Fixed   = FixedStyle.Right;
```

## Bands (`BandedGridView` / `AdvBandedGridView`)

`BandedGridView` displays columns under one row of bands. `AdvBandedGridView` supports multi-row bands plus arbitrary column stacking within a band.

```csharp
var view = new BandedGridView(gridControl1);
gridControl1.MainView = view;

var bandPersonal = new GridBand { Caption = "Personal" };
var bandContact  = new GridBand { Caption = "Contact" };
view.Bands.AddRange(new[] { bandPersonal, bandContact });

var colFirst = new BandedGridColumn { FieldName = "FirstName", Visible = true };
var colLast  = new BandedGridColumn { FieldName = "LastName",  Visible = true };
var colEmail = new BandedGridColumn { FieldName = "Email",     Visible = true };

view.Columns.AddRange(new[] { colFirst, colLast, colEmail });
colFirst.OwnerBand = bandPersonal;
colLast.OwnerBand  = bandPersonal;
colEmail.OwnerBand = bandContact;

view.OptionsView.ShowColumnHeaders = false;  // band caption replaces column header
view.OptionsBand.ShowCaption       = true;
```

In `AdvBandedGridView`, additionally use `BandedGridColumn.RowIndex` and `BandedGridColumn.ColIndex` to position the column within a band, and `RowCount` on the band to set the number of rows.

## Runtime Customization on Events

Customize per-cell appearance, per-cell editors, or per-row column visibility via events on the view:

- `CustomColumnDisplayText` — change displayed text (see [cell-display-and-editing.md](cell-display-and-editing.md)).
- `CustomRowCellEdit` / `CustomRowCellEditForEditing` — swap the editor per cell.
- `CustomDrawCell` / `RowCellStyle` — change appearance per cell.
- `ShowingEditor` — cancel editor activation for specific cells.

## Lookup Columns

To display a different field than the bound value (typical foreign-key scenario), use a `RepositoryItemLookUpEdit` or `RepositoryItemGridLookUpEdit` as the `ColumnEdit`:

```csharp
var lookup = new RepositoryItemLookUpEdit {
    DataSource   = categories,
    ValueMember  = "CategoryId",
    DisplayMember = "CategoryName",
    NullText     = "(none)"
};
gridControl1.RepositoryItems.Add(lookup);
gridView1.Columns["CategoryId"].ColumnEdit = lookup;

// Sort by display text (not the integer key) in ServerMode too:
gridView1.Columns["CategoryId"].FieldNameSortGroup = "CategoryName";
```

## TreeList Columns

`TreeList` columns work like `GridView` columns. Differences:

- The class is `TreeListColumn` and lives in `DevExpress.XtraTreeList.Columns`.
- `TreeList.PopulateColumns()` does **not** auto-create columns for the Key/Parent/Image fields — set `TreeListOptionsBehavior.PopulateServiceColumns = true` to include them.
- `TreeList.OptionsBehavior.AutoPopulateColumns` mirrors the grid setting.
- Use `TreeList.Columns.AddVisible("FieldName", "Caption")` in code.

## Save and Restore Columns

Column visibility/order/width/filter/group/sort/summary are part of the view's layout. Persist them via `SaveLayoutToXml` / `RestoreLayoutFromXml` — see [saving-and-restoring-layout.md](saving-and-restoring-layout.md).

## Common Issues

- **Column appears but is empty**: `FieldName` does not match a data-source property (case-sensitive). Verify spelling.
- **`PopulateColumns` clears my custom columns**: by design — `PopulateColumns` rebuilds the entire collection. Either set `AutoPopulateColumns = false` and manage columns manually, or recreate custom columns after `PopulateColumns`.
- **Unbound column not exported**: ensure `UnboundDataType` is set; without it the grid treats the column as `object` and may skip it.
- **Lookup column sorts by raw key**: set `column.FieldNameSortGroup` to the display field.
- **`Display(AutoGenerateField = false)` ignored in EF**: works only when `AutoPopulateColumns` is `true` and columns are generated. Manual `PopulateColumns()` overloads do not respect data-annotations.

## Source Material

- `articles/controls-and-libraries/data-grid/views/grid-view/columns.md` — Grid columns (`xref:WindowsForms.3483`).
- `articles/controls-and-libraries/data-grid/unbound-columns.md` (`xref:WindowsForms.1477`).
- `articles/controls-and-libraries/data-grid/getting-started/walkthroughs/data-binding-and-working-with-columns/tutorial-create-and-manage-columns-at-design-time.md` (`xref:WindowsForms.114681`).
- `articles/controls-and-libraries/data-grid/getting-started/walkthroughs/data-binding-and-working-with-columns/tutorial-working-with-columns-in-code.md` (`xref:WindowsForms.114708`).
- `articles/controls-and-libraries/tree-list/feature-center/data-presentation/columns.md` (`xref:WindowsForms.326`).
- `api/DevExpress.XtraGrid.Columns.GridColumn.yml`.
- `api/DevExpress.XtraGrid.Views.Base.ColumnView.PopulateColumns.yml`.
- `api/DevExpress.XtraTreeList.TreeList.PopulateColumns.yml`.
