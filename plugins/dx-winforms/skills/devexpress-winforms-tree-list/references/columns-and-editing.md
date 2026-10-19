# Columns and Editing

Define `TreeListColumn` columns, assign in-place editors, and control editing behavior.

## When to Use This Reference

- Populating / defining columns
- Assigning in-place editors per column or per cell
- Using the Edit Form
- Controlling read-only / editable behavior

## Where to Define Columns and Editors

Prefer the **designer file**. For a designer-backed form, declare `TreeListColumn`s and `RepositoryItem*` editors as fields and configure them in `InitializeComponent()` (`MainForm.Designer.cs`) so they appear in the Tree List Designer and survive round-tripping — this is the recommended layout:

```csharp
// MainForm.Designer.cs — inside InitializeComponent()
this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
    this.colName, this.colSales});
this.colName.Caption = "Name";  this.colName.FieldName = "Name";  this.colName.VisibleIndex = 0;
this.colSales.Caption = "Sales"; this.colSales.FieldName = "Sales"; this.colSales.VisibleIndex = 1;
this.colSales.ColumnEdit = this.repoSales;        // in-place editor (a RepositoryItem field)
this.treeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
    this.repoSales});
```

See [getting-started.md](getting-started.md#authoring-the-designercs-file) for the full designer-file form. The runtime API below is the right choice for columns/editors that are **created dynamically** (count or type unknown until run time) or for code-only controls — not as a substitute for the designer file on a designer-backed form.

## Columns

In bound mode, columns auto-generate from the data source. Control this via `OptionsBehavior`:

```csharp
treeList.OptionsBehavior.AutoPopulateColumns = true;   // default
// If disabled, create columns manually:
treeList.PopulateColumns();
```

Access and configure generated columns:

```csharp
using DevExpress.XtraTreeList.Columns;

TreeListColumn colRegion = treeList.Columns["Region"];
colRegion.Caption     = "Region";
colRegion.VisibleIndex = 0;
colRegion.Visible     = true;
colRegion.OptionsColumn.ReadOnly = true;

// Hide the key/parent service columns
treeList.Columns[treeList.KeyFieldName].Visible    = false;
treeList.Columns[treeList.ParentFieldName].Visible = false;
```

Add a new column (bound or unbound) with `Columns.AddField`:

```csharp
TreeListColumn col = treeList.Columns.AddField("Total");
col.Caption = "Total";
col.VisibleIndex = treeList.Columns.Count;
```

HTML in captions:

```csharp
treeList.OptionsView.AllowHtmlDrawHeaders = true;
col.Caption = "<i>Previous <b>March</b> Sales</i>";
```

Auto-size columns to content after the control is shown:

```csharp
this.BeginInvoke(new MethodInvoker(() => treeList.BestFitColumns()));
treeList.OptionsView.AutoWidth = false;   // don't stretch columns to control width
```

## In-Place Editors

In-place editors are `RepositoryItem*` objects (from `DevExpress.XtraEditors.Repository`) added to `TreeList.RepositoryItems` and assigned to a column via `ColumnEdit`.

```csharp
using DevExpress.XtraEditors.Repository;

var spin = new RepositoryItemSpinEdit();
spin.DisplayFormat.FormatType   = DevExpress.Utils.FormatType.Numeric;
spin.DisplayFormat.FormatString = "c0";
treeList.RepositoryItems.Add(spin);

treeList.Columns["MarchSales"].ColumnEdit = spin;
treeList.Columns["SeptemberSales"].ColumnEdit = spin;
```

Common repository items: `RepositoryItemTextEdit`, `RepositoryItemSpinEdit`, `RepositoryItemComboBox`, `RepositoryItemDateEdit`, `RepositoryItemCheckEdit`, `RepositoryItemLookUpEdit`, `RepositoryItemImageComboBox`.

### Per-Cell Editors

To assign different editors to cells **within the same column**, handle `CustomNodeCellEdit` (return the repository item for the given node/column):

```csharp
treeList.CustomNodeCellEdit += (s, e) => {
    if (e.Column.FieldName == "Value" && e.Node.GetValue("Kind")?.ToString() == "Date")
        e.RepositoryItem = dateEditRepositoryItem;
};
```

## Editing Behavior

```csharp
treeList.OptionsBehavior.Editable = true;          // master switch for editing
treeList.Columns["Region"].OptionsColumn.ReadOnly  = true;   // per-column read-only
treeList.Columns["Region"].OptionsColumn.AllowEdit = false;  // disallow editing
```

- `ShowingEditor` (event) — cancelable; veto opening the editor for specific nodes/columns (`e.Cancel = true`).
- `CellValueChanged` / `CellValueChanging` — react to edits.

## Edit Form

Instead of in-place editing, the TreeList can present an Edit Form (a popup/inline form with all editable fields). Configure it via `OptionsEditForm` and per-column `OptionsColumn.OptionsEditForm`. For the full Edit Form layout/customization API, search the docs:

```
devexpress_docs_search(technologies=["WindowsForms"], question="TreeList Edit Form customize layout OptionsEditForm")
```

## Source Material

- [TreeListColumn](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.Columns.TreeListColumn?md=true) — column reference
