# Save and Restore Layout — DevExpress WPF Data Grid

The Data Grid (and every DevExpress WPF control) can serialize its **layout** — column visibility / position / width, sort, filter, group, summaries, expand state, fixed columns, bands — to XML or a `Stream`. Restoring brings the grid back to that state. The serialization engine is `DevExpress.Xpf.Core.Serialization.DXSerializer`. Customize via `StoreLayoutMode`, `AllowProperty` event, and a few options.

## When to Use This Reference

Use this when you need to:

- Persist a user's grid customizations between sessions (column widths, sort, filter, group, layout)
- Save / load layout to a file (`SaveLayoutToXml` / `RestoreLayoutFromXml`)
- Save / load to / from a stream (database column, in-memory blob, network)
- Limit what gets saved (`StoreLayoutMode`) or block specific properties (`AllowProperty` event)
- Handle column changes between save and restore (`AddNewColumns`, `RemoveOldColumns`)
- Preserve focus / selection / check / group state when `ItemsSource` changes (`RestoreStateOnSourceChange`)
- Identify columns uniquely when they're generated from a ViewModel
- Save layouts of nested grids / master-detail

## What Gets Saved

| Saved by default (StoreLayoutMode = UI) | Marked with `GridUIProperty` attribute |
|---|---|
| Column visibility, position, width | ✅ |
| Sort order, sort index | ✅ |
| Group settings, group expand state | ✅ |
| Total / group summary settings | ✅ |
| Filter criteria | ✅ |
| Fixed-column / band state | ✅ |
| Auto Filter Row state | ✅ |
| Conditional formatting rules | ✅ |
| Bands hierarchy | ✅ |

| NOT saved (any mode) |
|---|
| `Style` properties (templates can't be serialized) |
| `Template` properties (same reason) |
| `CellTemplate`, `RowStyle`, `GroupRowStyle`, `CardStyle` — all template-based properties |
| Custom-object filter criteria (only primitive-type filters survive) |
| Enum-typed filter values (limited) |

Source: `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md`.

## Basic Save / Restore

### To / From XML File

```csharp
// Save
gridControl.SaveLayoutToXml(@"C:\layouts\my-grid.xml");

// Restore
gridControl.RestoreLayoutFromXml(@"C:\layouts\my-grid.xml");
```

### To / From Stream

```csharp
using var stream = new MemoryStream();

// Save
gridControl.SaveLayoutToStream(stream);
stream.Position = 0;
byte[] bytes = stream.ToArray();   // For DB / network / settings store

// Restore (later)
using var loadStream = new MemoryStream(bytes);
gridControl.RestoreLayoutFromStream(loadStream);
```

Both pairs round-trip through `DXSerializer`. The XML form is human-readable (useful for debugging); the stream form is more compact.

Source: `articles/common-concepts/save-and-restore-layouts.md` and `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md`.

## Unique Identification — `x:Name` and `UseFieldNameForSerialization`

The serializer matches elements between the saved layout and the current grid by **name**. Without unique names, restoration may fail or silently misapply settings to the wrong column.

### Set `x:Name` on Columns and Bands

```xaml
<dxg:GridControl x:Name="gridControl">
    <dxg:GridControl.Bands>
        <dxg:GridControlBand Name="productBand" Header="Product"/>
        <dxg:GridControlBand Name="pricingBand" Header="Pricing"/>
    </dxg:GridControl.Bands>
    <dxg:GridControl.Columns>
        <dxg:GridColumn x:Name="orderIdColumn"   FieldName="OrderId"/>
        <dxg:GridColumn x:Name="orderDateColumn" FieldName="OrderDate"/>
    </dxg:GridControl.Columns>
</dxg:GridControl>
```

### `UseFieldNameForSerialization` (Default `True`)

Instead of requiring explicit `x:Name` on every column, set this to `true` (default) and the serializer uses `FieldName` as the unique identifier:

```xaml
<dxg:GridControl UseFieldNameForSerialization="True">
    <dxg:GridColumn FieldName="OrderId"/>   <!-- No x:Name needed -->
</dxg:GridControl>
```

Implicit when columns are auto-generated from data annotations. Set to `false` only if you have multiple columns with the same `FieldName` (rare).

### ViewModel-Generated Elements — `XamlHelper.Name`

When columns or bands are generated from a `ColumnsSource` / `BandsSource` collection, you can't use `Name="..."` directly because the elements don't have a direct XAML name. Instead, use the `dx:XamlHelper.Name` attached property to bind the name to a ViewModel property:

```xaml
<DataTemplate x:Key="BandTemplate">
    <dxg:GridControlBand
        dx:XamlHelper.Name="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).UniqueName,
                                     RelativeSource={RelativeSource Self}}"/>
</DataTemplate>
```

`UniqueName` is a string property on each ViewModel item that uniquely identifies the band / column. The full namespace prefixes:

```xml
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
xmlns:dxci="http://schemas.devexpress.com/winfx/2008/xaml/core/internal"
```

Source: `articles/common-concepts/save-and-restore-layouts.md` and `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md`.

## `StoreLayoutMode` — Control What's Saved

`DXSerializer.StoreLayoutMode` (attached property) controls the breadth of serialization:

| Value | What's saved |
|---|---|
| `None` | Nothing |
| `All` | All settings — visual + data-aware + behavior + customization |
| `UI` (**default**) | Only properties marked `[GridUIProperty]` — visibility, position, width, sort, group, summary, filter, etc. |

```xaml
<dxg:GridControl dxs:DXSerializer.StoreLayoutMode="All">
    <!-- ... -->
</dxg:GridControl>
```

```xml
xmlns:dxs="http://schemas.devexpress.com/winfx/2008/xaml/core/serialization"
```

> Verify the exact XAML namespace for `DXSerializer` (and whether it's the `core/serialization` or `core` URI) via DxDocs MCP. The pattern is consistent in modern versions.

Most apps use the default `UI` — the user-facing state. Use `All` only when restoring a complete grid configuration (including code-set properties that aren't user-modifiable).

## `AllowProperty` Event — Per-Property Control

For fine-grained control over which properties serialize, handle `DXSerializer.AllowProperty`:

```csharp
using DevExpress.Xpf.Core.Serialization;

public MainWindow() {
    InitializeComponent();

    // Wire on a specific column
    grid.Columns[nameof(Customer.ID)].AddHandler(
        DXSerializer.AllowPropertyEvent,
        new AllowPropertyEventHandler(OnAllowProperty));
}

void OnAllowProperty(object sender, AllowPropertyEventArgs e) {
    if (e.DependencyProperty == GridColumn.WidthProperty)
        e.Allow = false;   // Don't save / restore the width of this column
}
```

`AllowPropertyEventArgs`:
- `DependencyProperty` — which property is being serialized
- `Allow` — set `false` to skip

This is a routed event — you can attach it on `GridControl`, the View, individual columns, or bands.

Common use cases:
- Block `Width` so columns retain their default size
- Block `Visible` so columns can't be hidden by the user
- Block specific column's `FilterCriteria` for sensitive data

## Handle Column Diffs — `AddNewColumns` / `RemoveOldColumns`

When loading a layout, the grid's current columns may differ from those in the saved layout:

| Property | Default | Effect |
|---|---|---|
| `DataControlSerializationOptions.AddNewColumns` | `false` | If `true`, columns that exist in the loaded layout but not in the current grid are **added** to the grid. |
| `DataControlSerializationOptions.RemoveOldColumns` | `true` | If `true`, columns that exist in the current grid but not in the loaded layout are **removed**. Set to `false` to keep them. |

```xaml
<dxg:GridControl>
    <dxg:GridControl.SerializationOptions>
        <dxg:DataControlSerializationOptions AddNewColumns="False" RemoveOldColumns="False"/>
    </dxg:GridControl.SerializationOptions>
</dxg:GridControl>
```

> Verify exact property path (`GridControl.SerializationOptions` vs an attached property) via DxDocs MCP. The class name `DataControlSerializationOptions` is confirmed in the article.

Common pattern for app upgrades:
- `AddNewColumns="True" RemoveOldColumns="False"` — preserve user customizations across schema additions / removals.

Source: `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md` § "Layout Loading Specifics".

## Restore State on Source Change — `RestoreStateOnSourceChange`

A different scenario: not save/load to disk, but **preserve focus / selection / check / group state when `ItemsSource` is reassigned** (e.g., after re-fetching from a database).

```xaml
<dxg:GridControl ItemsSource="{Binding Items}"
                 RestoreStateOnSourceChange="True"
                 RestoreStateKeyFieldName="ID">
    <dxg:GridControl.View>
        <dxg:TableView/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

- **`RestoreStateOnSourceChange`** (`true`) — after `ItemsSource` is reassigned, restore the focused row, selected rows, checked rows, group expansion to records matching the previous state.
- **`RestoreStateKeyFieldName`** — the field used to match records across source changes (typically the primary key).

What's preserved:
- Focused row (`CurrentItem` / `FocusedRowHandle`)
- Selected rows (`SelectedItems`)
- Check state of rows (TreeList)
- Group expand state

What's NOT preserved here (use `SaveLayoutToXml` for these):
- Column widths
- Sort / filter / group settings
- Other layout

> `RestoreStateOnSourceChange` is for live data refreshes. `SaveLayoutToXml` / `RestoreLayoutFromXml` is for persistence between app sessions. Use both for the full picture.

Source: `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md` § "Restore State On Source Change".

## DXSerializer Lifecycle Events

For richer customization (e.g., transform XML between save and load, log serialization, encrypt sensitive parts), DXSerializer raises events. Common ones (verify exact names via apidoc):

| Event | When |
|---|---|
| `DXSerializer.SerializingProperty` | Before each property is written |
| `DXSerializer.DeserializingProperty` | Before each property is read |
| `DXSerializer.AllowProperty` | Decide whether to serialize a property at all (covered above) |
| `DXSerializer.SerializingItems` / `DeserializingItems` | For collections like `Columns`, `Bands` |

```csharp
DXSerializer.AddSerializingPropertyHandler(grid, OnSerializingProperty);
```

See `articles/common-concepts/save-and-restore-layouts.md` § "Advanced Scenarios" and `articles/common-concepts/dxserializer-events-advanced-scenarios.md` (if present in your version).

## MVVM Pattern — Store Layout in a ViewModel

```csharp
public class MainViewModel : ViewModelBase {
    public byte[] SavedLayout {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    public ICommand SaveLayoutCommand => new DelegateCommand<GridControl>(grid => {
        using var stream = new MemoryStream();
        grid.SaveLayoutToStream(stream);
        SavedLayout = stream.ToArray();
        // Persist `SavedLayout` to settings / DB / etc.
    });

    public ICommand RestoreLayoutCommand => new DelegateCommand<GridControl>(grid => {
        if (SavedLayout == null) return;
        using var stream = new MemoryStream(SavedLayout);
        grid.RestoreLayoutFromStream(stream);
    });
}
```

```xaml
<Button Content="Save Layout"
        Command="{Binding SaveLayoutCommand}"
        CommandParameter="{Binding ElementName=grid}"/>
<Button Content="Restore Layout"
        Command="{Binding RestoreLayoutCommand}"
        CommandParameter="{Binding ElementName=grid}"
        IsEnabled="{Binding SavedLayout, Converter={StaticResource NullToFalseConverter}}"/>
```

`MainViewModel` exposes the byte[] for persistence. The View handles the actual grid reference via `CommandParameter`.

For a pure MVVM solution (no `GridControl` in ViewModel), use `EventToCommand` + a behavior — or just accept that layout serialization is inherently bound to the View, and pass `GridControl` via a parameter.

## Save / Restore Layout in Master-Detail

Master and detail grids serialize their layouts **independently**. Master `SaveLayoutToXml` does NOT include detail-grid layouts.

To save detail layouts, iterate over expanded master rows and call `SaveLayoutTo*` on each detail control:

```csharp
foreach (var rowHandle in grid.GetVisibleRows()) {
    var detail = grid.GetVisibleDetail(rowHandle) as GridControl;
    detail?.SaveLayoutToXml($"detail-{rowHandle}.xml");
}
```

> `GetVisibleRows` / `GetVisibleDetail` exact method names — verify against `apidoc/DevExpress.Xpf.Grid/GridControl/`.

In practice, master-detail apps usually save only the master layout — detail layouts are reset on each master row expand.

## What Doesn't Serialize

Per the source article, the following are NOT included in any save:

- **`*Style` properties** — `RowStyle`, `GroupRowStyle`, `CellStyle`, `CardStyle`, etc. Templates can't be serialized.
- **`*Template` properties** — `CellTemplate`, `CellDisplayTemplate`, `CellEditTemplate`, `RowDetailsTemplate`, `BandHeaderTemplate`, etc.
- **Custom-object filter criteria** — if the filter contains custom or enumeration objects, it won't serialize.
- **Dynamic data binding** — `ColumnBase.Binding` expressions don't survive a roundtrip (only `FieldName` is referenced).
- **Event handlers / commands** — code-behind connections aren't part of layout.

Design implication: design your styles / templates as XAML resources that load with the application, not as runtime-set state. Then layout serialization handles the user-customizable state cleanly.

## Apply to TreeListControl

Same APIs:

| GridControl | TreeListControl |
|---|---|
| `DataControlBase.SaveLayoutToXml` / `Stream` | same (inherited from `DataControlBase`) |
| `DataControlBase.RestoreLayoutFromXml` / `Stream` | same |
| `DataControlBase.UseFieldNameForSerialization` | same |
| `DataControlBase.RestoreStateOnSourceChange` | same — preserves expand state + checked nodes for TreeList |
| `DataControlBase.RestoreStateKeyFieldName` | same |
| `DXSerializer.StoreLayoutMode` | same |
| `DXSerializer.AllowProperty` event | same |

Additional for TreeList: serialization includes node expand state (`TreeListNode.IsExpanded`) and check state when `RestoreStateOnSourceChange="True"`.

## Apply to PivotGridControl

PivotGrid has its own dedicated layout serialization (different from `DXSerializer`):

```csharp
pivotGrid.SaveLayoutToXml("pivot-layout.xml");
pivotGrid.RestoreLayoutFromXml("pivot-layout.xml");
pivotGrid.SaveLayoutToStream(stream);
pivotGrid.RestoreLayoutFromStream(stream);
```

The Pivot Grid layout includes: field positions (Row / Column / Data / Filter areas), group intervals, sort order, filter values, column widths.

See `articles/controls-and-libraries/pivot-grid/layout/save-and-restore-layout.md`.

## Common Issues

- **Layout restored but columns are wrong / shuffled** — columns lack unique identification. Set `x:Name` on each, or ensure `UseFieldNameForSerialization="True"` and each column has a unique `FieldName`.
- **ViewModel-generated columns lose layout** — bind `dx:XamlHelper.Name` to a unique-name property on each ViewModel item.
- **`AddNewColumns="False"` doesn't add new columns** — that's by design. Set to `true` to add columns from saved layouts that don't exist in the current grid.
- **Custom filter doesn't restore** — filter contains custom-object operands. Use primitive types (string, int, DateTime) in filter values, or rebuild the filter from saved state manually.
- **`StoreLayoutMode="UI"` skips custom property** — your property isn't marked with `[GridUIProperty]`. Use `"All"` or handle `AllowProperty` to opt in.
- **Restore takes ages on large grids** — column count too high. Consider serializing only what changed via custom `AllowProperty` event filtering.
- **Master-detail: detail layouts lost** — detail grids serialize independently. Save each detail's layout separately if needed.

## Source Material

- `articles/common-concepts/save-and-restore-layouts.md`
- `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md`
- `articles/controls-and-libraries/pivot-grid/layout/save-and-restore-layout.md` (Pivot Grid–specific)
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-save-and-restore-layout-to-from-xml.md`
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-save-and-restore-layout-to-from-a-stream.md`
- `apidoc/DevExpress.Xpf.Core.Serialization/` (apidoc namespace for `DXSerializer`)
