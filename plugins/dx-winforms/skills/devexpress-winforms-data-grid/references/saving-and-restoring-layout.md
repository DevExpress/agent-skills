# Saving and Restoring Layout

This reference covers how to persist the visual state of a `GridControl`/`TreeList` (columns, bands, widths, visibility, sort, group, filter, summaries, format rules, appearance) to XML, JSON, a `Stream`, or the system registry — and how to selectively control which settings persist via `OptionsLayoutBase` descendants.

## When to Use This Reference

- Saving each user's column arrangement between sessions.
- Bundling grid layout into a `PersistenceBehavior` for the whole form.
- Re-restoring layout but excluding data bindings or appearance.
- Performing post-restore fix-ups (`LayoutUpgrade`).
- Deciding between `SaveLayoutToXml` vs JSON vs the registry.

## API Surface

`BaseView` (the base of all grid views) and `TreeList` expose the same set of methods:

```csharp
// Save
view.SaveLayoutToXml(path);
view.SaveLayoutToXml(path, options);
view.SaveLayoutToJson(stream);
view.SaveLayoutToStream(stream);
view.SaveLayoutToRegistry("HKEY_CURRENT_USER\\Software\\Acme\\MainForm");

// Restore
view.RestoreLayoutFromXml(path);
view.RestoreLayoutFromXml(path, options);
view.RestoreLayoutFromJson(stream);
view.RestoreLayoutFromStream(stream);
view.RestoreLayoutFromRegistry("HKEY_CURRENT_USER\\Software\\Acme\\MainForm");
```

`TreeList`, `VGridControl`, `PropertyGridControl`, `PivotGridControl`, `TileControl`, and `SchedulerControl` all expose the same methods on the control itself (no inner "view").

## Default vs. Explicit Options

The methods come in two overloads — with and without an `OptionsLayoutBase` parameter:

- **Without options**: persist a *default subset* — visibility, position, width of columns/bands/fields; sort & group state; current and MRU filters; summaries; `OptionsView` settings.
- **With options**: persist exactly what the options object enables. Appearance, behavior, data bindings, customization, and selection are **omitted by default** — pass options to include them.

```csharp
var opts = new DevExpress.Utils.OptionsLayoutGrid {
    StoreAppearance = true,
    StoreVisualOptions = true,
    StoreAllOptions = false,
    StoreDataSettings = true,
    Columns = { StoreLayout = true, StoreAppearance = true }
};
gridView1.SaveLayoutToXml(@"%APPDATA%\Acme\layout.xml", opts);
gridView1.RestoreLayoutFromXml(@"%APPDATA%\Acme\layout.xml", opts);
```

`OptionsLayoutBase.FullLayout` is a shortcut for "store everything":

```csharp
gridView1.SaveLayoutToXml(path, DevExpress.Utils.OptionsLayoutBase.FullLayout);
```

| Options class | Use for |
|---|---|
| `OptionsLayoutGrid` | Grid views (`GridView`, `BandedGridView`, `AdvBandedGridView`, `CardView`, `LayoutView`, `TileView`, `WinExplorerView`). |
| `OptionsLayoutTreeList` | `TreeList`. |
| `PivotGridOptionsLayout` | `PivotGridControl`. |
| `OptionsLayoutBase` | Base — `FullLayout` static property. |

Each options class exposes nested `OptionsLayoutBase` instances for sub-elements (e.g., `OptionsLayoutGrid.Columns.StoreAllOptions`).

## Default-Persisted vs Optional Settings

| Setting | Persisted by default? |
|---|---|
| Column visibility, order, width | yes |
| Sort / group / summary state | yes |
| Active filter + MRU filters | yes |
| `OptionsView` settings (e.g., `ShowGroupPanel`, `ShowAutoFilterRow`) | yes |
| `OptionsBehavior`, `OptionsCustomization`, `OptionsSelection` | no — set `StoreAllOptions = true` |
| Appearance (colors, fonts) | no — set `StoreAppearance = true` |
| Data bindings (`DataSource`, `DataMember`) | no — set `StoreDataSettings = true` if needed |
| Format rules (conditional formatting) | yes (with rule reference); appearance per rule needs `StoreAppearance` |

> **Set `Name` on every column/band**: the layout is keyed by `Name` (component name). Default names like `gridColumn1` are fragile — assign meaningful unique names to avoid round-trip mismatches.

## Post-Restore Fix-ups (`LayoutUpgrade`)

`LayoutUpgrade` fires after restore so you can patch differences between saved and current schema versions:

```csharp
gridView1.LayoutUpgrade += (s, e) => {
    if (e.PreviousVersion != null && e.PreviousVersion.CompareTo(new Version("2.0")) < 0) {
        // Rename a column that existed in old layouts
        var oldCol = gridView1.Columns["OldName"];
        if (oldCol != null) oldCol.Name = "NewName";
    }
};
```

Set `gridView1.LayoutVersion = "2.0"` to write a version string into the saved layout.

## Fine-grained Hooks

For per-property control over serialization, handle the corresponding events on the control:

- `BaseView.PropertySerializing` / `PropertyDeserializing` — skip or rewrite specific properties.
- `VGridControlBase.PropertySerializing` / `PropertyDeserializing` — same for VGrid.

```csharp
gridView1.PropertySerializing += (s, e) => {
    if (e.SerializingProperty.Name == "FilterString") e.Handled = true;  // never persist filters
};
```

## Persistence Behavior / Workspace Manager

When the form hosts many DevExpress controls, the form-level **Persistence Behavior** or **Workspace Manager** components serialize every supported control with one call:

```csharp
behaviorManager1.Attach<PersistenceBehavior>(this);
// On exit:
behaviorManager1.GetBehavior<PersistenceBehavior>(this).SaveLayout(@"%APPDATA%\Acme\form.xml");
```

The form layout includes `GridControl`, `TreeList`, `LayoutControl`, dock layouts, etc. The grid's `OptionsLayout` (default subset) is what gets included unless you customize it.

## Where to Save

| Destination | Pros | Cons |
|---|---|---|
| **XML file in `%APPDATA%`** | Human-readable, easy to diff, easy to ship. | File path management, permissions on locked-down machines. |
| **JSON file** | Easy to merge into modern app settings stores. | Slightly newer; double-check round-trip with custom properties. |
| **`Stream`** | Embed inside a database column, isolated storage, custom encryption. | Plumbing. |
| **Registry** | Per-user without file management. | Discouraged for new apps; per-user paths only — `HKCU\Software\...`. |

## TreeList Specifics

`TreeList` uses `OptionsLayoutTreeList` and stores node expansion state if `StoreAppearance = true` plus `Nodes.StoreLayout = true` are enabled. Otherwise nodes restore collapsed. To restore expansion explicitly:

```csharp
// Capture the *expanded* nodes (not treeList1.Selection, which is the selected
// nodes). TreeList exposes no expanded-nodes collection, so walk the tree.
var expandedKeys = new HashSet<object>();
void CollectExpanded(TreeListNodes nodes) {
    foreach (TreeListNode n in nodes) {
        if (n.Expanded) expandedKeys.Add(n.GetValue(treeList1.KeyFieldName));
        CollectExpanded(n.Nodes);
    }
}
CollectExpanded(treeList1.Nodes);

treeList1.RestoreLayoutFromXml(path);

foreach (var key in expandedKeys) {
    var n = treeList1.FindNodeByKeyID(key);
    if (n != null) n.Expanded = true;
}
```

## Common Issues

- **Layout file restores columns but not appearance**: default overload omits appearance. Use `new OptionsLayoutGrid { StoreAppearance = true }`.
- **`FileNotFoundException` on restore**: `RestoreLayoutFromXml` throws when the file does not exist. Wrap in `if (File.Exists(...))`.
- **Restored columns are duplicated**: column names collided. Set `Name` to unique values; do not rely on `gridColumn1`, `gridColumn2`.
- **Layout silently mismatches after a rename**: handle `LayoutUpgrade` and translate old names to new.
- **`ForceInitialize` needed before restore**: call `gridControl1.ForceInitialize()` before restoring inside `Form_Load`; otherwise the view has no columns yet.

## Source Material

- `articles/common-features/saving-and-restoring-layouts-to-a-file-stream-and-system-registry.md` (`xref:WindowsForms.2404`).
- `articles/common-features/save-and-restore-layouts/layout-options-xtragrid-xtrapivotgrid.md` (`xref:WindowsForms.1875`).
- `articles/controls-and-libraries/data-grid/save-and-restore-layout.md` (`xref:WindowsForms.772`).
- `articles/controls-and-libraries/data-grid/save-and-restore-layout/saving-the-layout-at-design-time.md` (`xref:WindowsForms.2379`).
- `articles/common-features/save-and-restore-layouts/upgrading-layout.md` (`xref:WindowsForms.2383`).
- `articles/common-features/behaviors/persistence-behavior.md` (`xref:WindowsForms.117283`).
- `api/DevExpress.XtraGrid.Views.Base.BaseView.SaveLayoutToXml.yml`.
- `api/DevExpress.XtraGrid.Views.Base.BaseView.RestoreLayoutFromXml.yml`.
- `api/DevExpress.XtraGrid.Views.Base.BaseView.LayoutUpgrade.yml`.
- `api/DevExpress.Utils.OptionsLayoutGrid.yml`.
- `api/DevExpress.Utils.OptionsLayoutBase.yml`.
- `api/DevExpress.XtraTreeList.OptionsLayoutTreeList.yml`.
