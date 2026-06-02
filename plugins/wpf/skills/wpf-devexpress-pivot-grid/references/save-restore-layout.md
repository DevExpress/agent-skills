# Save and Restore Layout — DevExpress WPF Pivot Grid

The Pivot Grid persists its **complete user-visible state** — field visibility, area, position, sorting, filtering, grouping intervals, summaries, format conditions, and column sizes — through `SaveLayoutTo*` / `RestoreLayoutFrom*` methods on `PivotGridControl`.

> Pivot Grid uses each `PivotGridField.Name` to match fields in the saved layout to fields in the current control. **A field without a `Name` cannot be reliably restored** — always set `Name` on every field that participates in layout persistence.

## When to Use This Reference

Use this when you need to:

- Persist the pivot's layout between sessions
- Save the **collapsed/expanded** state of pivot field headers (separate from layout!)
- Restore a layout asynchronously without blocking the UI
- Reconcile a saved layout with a control whose field set has changed
- Cancel or upgrade a layout during load

## API Overview

### Save / Restore Layout (Field Configuration)

```csharp
// XML
pivotGridControl1.SaveLayoutToXml("pivot-layout.xml");
pivotGridControl1.RestoreLayoutFromXml("pivot-layout.xml");

// Stream
using var ms = new MemoryStream();
pivotGridControl1.SaveLayoutToStream(ms);
ms.Position = 0;
pivotGridControl1.RestoreLayoutFromStream(ms);

// Async restore (keeps UI responsive on big layouts)
await pivotGridControl1.RestoreLayoutFromStreamAsync(stream);
```

### Save / Restore Collapsed State (Separate)

Collapsed/expanded state of headers is **NOT** included in the regular layout. Persist it with the parallel `CollapsedState` API:

```csharp
pivotGridControl1.SaveCollapsedStateToFile("collapsed.xml");
pivotGridControl1.SaveCollapsedStateToStream(stream);
pivotGridControl1.RestoreCollapsedStateFromFile("collapsed.xml");
pivotGridControl1.RestoreCollapsedStateFromStream(stream);
await pivotGridControl1.RestoreCollapsedStateFromStreamAsync(stream);
```

Without these calls, restored fields are always **fully expanded**, even if they were collapsed when saved.

### What's Saved by Default (`StoreLayoutMode.AllOptions`)

`PivotSerializationOptions.StoreLayoutMode` controls the breadth of saved state. The default `AllOptions` includes:

- Field **visibility**, **position** (area + areaIndex), **size**
- Data settings: **grouping**, **sorting**, **filtering**, **summaries**
- **Appearance** settings and **format conditions** (conditional formatting rules)
- Field **data format** for exporting

Narrower modes (e.g., visibility-only, sort-and-filter-only) are also available — see `xref:DevExpress.Xpf.PivotGrid.StoreLayoutMode`.

## Reconcile Layout Differences (`AddNewFields` / `RemoveOldFields`)

When the field set in the saved layout differs from the field set on the control, two flags on `PivotSerializationOptions` decide what happens. Match is by `Name`.

| `AddNewFields` | `RemoveOldFields` | Behavior for fields **only in saved layout** | Behavior for fields **only on control** |
|---|---|---|---|
| `true` | `true` | **Removed** from saved layout | **Kept** on control |
| `false` | `false` | **Restored** to control | **Removed** from control |
| `true` | `false` | **Restored** to control | **Kept** on control (union of both sets) |
| `false` | `true` | **Removed** from saved layout | **Removed** from control |

Use **`true` / `false`** when you want the current code to be authoritative about which fields exist but want any field-level customization in the saved layout to apply when names match.

For field groups: use `PivotSerializationOptions.AddNewGroups`.

## Layout Versioning

When the saved layout's version differs from the current control's `DXSerializer.LayoutVersion`, `PivotGridControl.LayoutUpgrade` fires. The event args (`PivotLayoutUpgradeEventArgs`) expose:

- `e.PreviousVersion` (`string`, read-only) — textual representation of the previous layout version.

Use the handler to log the upgrade or migrate stored state in code. `PivotLayoutUpgradeEventArgs` does **not** expose an `Allow` flag — to suppress applying a saved layout entirely, cancel it from `BeforeLoadLayout` (whose args, `PivotLayoutAllowEventArgs`, do have `Allow`).

```csharp
pivotGridControl1.LayoutUpgrade += (s, e) => {
    // e.PreviousVersion is a string like "1.0", "2.1", etc.
    if (e.PreviousVersion == "1.0") {
        // Migrate values from v1.0 layout here.
    }
};
```

## Cancel Layout Load

`PivotGridControl.BeforeLoadLayout` fires before any layout is applied. Set `e.Allow = false` to cancel:

```csharp
pivotGridControl1.BeforeLoadLayout += (s, e) => {
    if (!_userPreferences.AutoRestoreLayout) e.Allow = false;
};
```

## Typical Persistence Pattern

```csharp
// On window close
private void Window_Closing(object sender, CancelEventArgs e) {
    using var layoutStream = File.Create(LayoutFile);
    pivotGridControl1.SaveLayoutToStream(layoutStream);

    using var collapsedStream = File.Create(CollapsedFile);
    pivotGridControl1.SaveCollapsedStateToStream(collapsedStream);
}

// On window load — after DataSource is set and fields are added
private async void Window_Loaded(object sender, RoutedEventArgs e) {
    pivotGridControl1.DataSource = _viewModel.Sales;

    AddField("Country", FieldArea.RowArea, "Country", name: "fieldCountry");
    AddField("Year", FieldArea.ColumnArea, "OrderDate", name: "fieldYear");
    AddField("Sales", FieldArea.DataArea, "Amount", name: "fieldSales");

    if (File.Exists(LayoutFile)) {
        using var s = File.OpenRead(LayoutFile);
        await pivotGridControl1.RestoreLayoutFromStreamAsync(s);
    }
    if (File.Exists(CollapsedFile)) {
        using var s = File.OpenRead(CollapsedFile);
        await pivotGridControl1.RestoreCollapsedStateFromStreamAsync(s);
    }
}
```

> Restore order matters: `DataSource` first, then **define the fields you expect** (or rely on `AddNewFields`), then restore.

## MVVM Persistence

Persist as `byte[]` in the ViewModel; bind save/restore to commands:

```csharp
public class MainViewModel : ViewModelBase {
    public byte[] SavedLayout {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    public ICommand SaveLayoutCommand => new DelegateCommand<PivotGridControl>(pivot => {
        using var ms = new MemoryStream();
        pivot.SaveLayoutToStream(ms);
        SavedLayout = ms.ToArray();
    });

    public ICommand RestoreLayoutCommand => new DelegateCommand<PivotGridControl>(async pivot => {
        if (SavedLayout == null) return;
        using var ms = new MemoryStream(SavedLayout);
        await pivot.RestoreLayoutFromStreamAsync(ms);
    });
}
```

```xaml
<Button Content="Save Layout"
        Command="{Binding SaveLayoutCommand}"
        CommandParameter="{Binding ElementName=pivot}"/>
```

## Common Issues

- **Restored layout shows wrong fields, or fields in wrong areas** — fields lack `Name` values. Set `Name` on every field that's involved in persistence.
- **Restored layout has all groups expanded** — collapsed state isn't part of the regular layout. Also call `SaveCollapsedState*` / `RestoreCollapsedState*`.
- **Restore wipes fields you added in code** — `AddNewFields` is `false`. Set it to `true` to keep code-defined fields that aren't in the saved layout.
- **Restore fails after upgrade** — saved layout is from an older version. Handle `LayoutUpgrade` and either migrate or reject the old layout.

## Source Material

- `articles/controls-and-libraries/pivot-grid/layout/save-and-restore-layout.md` (`xref:8023`)
- `articles/common-concepts/save-and-restore-layouts.md` (`xref:7391`)
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-save-and-restore-layout-to-from-xml.md`
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-save-and-restore-layout-to-from-a-stream.md`
