# Save and Restore Layout — DevExpress WPF TreeList

`TreeListControl` uses the same layout-serialization API as `GridControl`: `SaveLayoutToXml/Stream` and `RestoreLayoutFromXml/Stream` (inherited from `DataControlBase`). The serialized state additionally captures TreeList-specific data — **node expand state** and **node check state** — alongside the standard column / sort / filter / group state.

**For the full reference (`DXSerializer`, `StoreLayoutMode`, `AllowProperty` event, `AddNewColumns` / `RemoveOldColumns`, `XamlHelper.Name`, MVVM persistence patterns, what doesn't serialize) see [the data-grid skill's `references/save-restore-layout.md`](../../devexpress-wpf-data-grid/references/save-restore-layout.md).** This reference covers TreeList-specific concerns.

## When to Use This Reference

Use this when you need to:

- Persist TreeList layout (columns + sort/filter + **node expand state**) between sessions
- Preserve focus / selection / check / **expand state** on `ItemsSource` reassignment
- Understand what TreeList-specific state survives the save/restore round-trip

## Basic Save / Restore

Identical API:

```csharp
// Save
treeListControl.SaveLayoutToXml(@"C:\layouts\tree.xml");
treeListControl.SaveLayoutToStream(stream);

// Restore
treeListControl.RestoreLayoutFromXml(@"C:\layouts\tree.xml");
treeListControl.RestoreLayoutFromStream(stream);
```

## What's Additionally Saved in TreeList

Beyond the standard column / sort / filter / group / summary state, TreeList layout includes:

- **Node expand state** — which nodes were expanded vs collapsed
- **Node check state** — `IsChecked` for each node (when `ShowCheckboxes="True"`)
- **TreeListView-specific properties** — `AutoExpandAllNodes`, `KeyFieldName`, `ParentFieldName`, etc. (per `StoreLayoutMode`)

> The bound data must be reassigned BEFORE restoring the layout — `RestoreLayoutFromXml` matches nodes by key. If the data isn't loaded yet, the expand / check state won't apply to any node.

Typical sequence:

```csharp
// 1. Load data
viewModel.LoadEmployees();
// (TreeListControl is now populated)

// 2. Restore layout
treeListControl.RestoreLayoutFromXml("tree.xml");
// (expand state, sort, filter etc. applied)
```

## Preserve State on `ItemsSource` Reassign — `RestoreStateOnSourceChange`

When the bound collection is re-fetched (e.g., after a refresh), preserve the user's interaction state:

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}"
                     RestoreStateOnSourceChange="True"
                     RestoreStateKeyFieldName="ID">
    <dxg:TreeListControl.View>
        <dxg:TreeListView KeyFieldName="ID" ParentFieldName="ParentID"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

For TreeList, `RestoreStateOnSourceChange="True"` preserves:

| State | Preserved? |
|---|---|
| Focused node (matched by `RestoreStateKeyFieldName`) | ✅ |
| Selected nodes | ✅ |
| Checked nodes | ✅ |
| **Expand state per node** | ✅ |
| Group state | ✅ |

`RestoreStateKeyFieldName` must match the `KeyFieldName` used by `TreeListView` for self-referential binding — typically the primary key.

For hierarchical mode (`ChildNodesPath`), `RestoreStateKeyFieldName` matches by a chosen unique field on the data class.

> This is different from `SaveLayoutTo*` — that's for persistence to disk. `RestoreStateOnSourceChange` is for live data refreshes within a session.

Source: `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md` § "Restore State On Source Change".

## ExpandStateBinding / ExpandStateFieldName — Alternative Persistence

Instead of layout serialization, you can bind each node's expand state to a data property:

```xaml
<dxg:TreeListView ExpandStateFieldName="IsExpanded"/>
```

Now `Employee.IsExpanded` (a `bool` data property) IS the expand state. Persist your data → persist the expansion.

```csharp
public class Employee {
    public int ID { get; set; }
    public int? ParentID { get; set; }
    public string Name { get; set; } = "";
    public bool IsExpanded { get; set; }   // Bound to node expand state
}
```

See [nodes.md § Bind Expand State](nodes.md) for the full pattern including `ExpandStateBinding` (precedes the field-name property).

**Trade-off**:
- `SaveLayoutToXml` — captures everything (columns + expand + check + sort + filter) in one blob, persisted separately from data
- `ExpandStateFieldName` — only expand state, persisted with data (simpler for some scenarios)

Use both together if you need fine-grained control.

## Column Identification

Same rules as Grid (see data-grid reference):

```xaml
<dxg:TreeListColumn x:Name="nameColumn" FieldName="Name"/>
<dxg:TreeListColumn x:Name="deptColumn" FieldName="Department"/>
```

OR rely on `UseFieldNameForSerialization="True"` (default).

For ViewModel-generated columns / bands, use `dx:XamlHelper.Name` — see [data-grid save-restore-layout.md § ViewModel-Generated Elements](../../devexpress-wpf-data-grid/references/save-restore-layout.md).

## MVVM Persistence

Same pattern as Grid (byte[] in ViewModel, persisted to settings / DB):

```csharp
public class MainViewModel : ViewModelBase {
    public byte[] SavedLayout {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    public ICommand SaveLayoutCommand => new DelegateCommand<TreeListControl>(tree => {
        using var stream = new MemoryStream();
        tree.SaveLayoutToStream(stream);
        SavedLayout = stream.ToArray();
    });

    public ICommand RestoreLayoutCommand => new DelegateCommand<TreeListControl>(tree => {
        if (SavedLayout == null) return;
        using var stream = new MemoryStream(SavedLayout);
        tree.RestoreLayoutFromStream(stream);
    });
}
```

```xaml
<Button Content="Save Layout"
        Command="{Binding SaveLayoutCommand}"
        CommandParameter="{Binding ElementName=treeList}"/>
<Button Content="Restore Layout"
        Command="{Binding RestoreLayoutCommand}"
        CommandParameter="{Binding ElementName=treeList}"/>
```

## Tree-Specific Caveats

### Restore Must Happen After Data Load

Unlike Grid (which can restore column metadata independently of data), TreeList expand state can only be applied to existing nodes. Sequence matters:

1. Set `ItemsSource` → grid builds the tree
2. Call `RestoreLayoutFromXml` → expand / check / sort / filter applied

Calling `RestoreLayoutFromXml` before `ItemsSource` is set means the tree-specific state has no nodes to apply to (only the column/view state survives).

### Self-Referential vs Hierarchical Mode

- **Self-referential** (`KeyFieldName`/`ParentFieldName`): nodes are matched by `KeyFieldName` value. If the data is replaced with records that have different keys, expansion / check state is lost (no match).
- **Hierarchical** (`ChildNodesPath`): nodes are matched by `RestoreStateKeyFieldName` (which must be a property on the data class, e.g., a primary key).

### Unbound Mode

In unbound mode (XAML / code `TreeListNode` tree), layout serialization captures the tree structure too — but if you recreate the `Nodes` tree from code after restore, the saved state may not match. Use `ExpandStateBinding` on each node instead.

## What Doesn't Serialize (Reminder)

Same as Grid:
- `*Style` properties (`RowStyle`, `GroupRowStyle`, `CellStyle`)
- `*Template` properties (`CellTemplate`, `CellDisplayTemplate`, `BandHeaderTemplate`, etc.)
- Custom-object filter operands
- `Binding`-based column references
- Event handlers / commands
- Per-node `Image` properties (Image source itself)

## Common Issues

- **Expand state not restored** — `ItemsSource` not yet loaded when `RestoreLayoutFromXml` is called. Restore AFTER data load.
- **Check state not restored** — `RestoreStateKeyFieldName` doesn't match a unique field, OR the records' keys changed between save and restore.
- **Layout restored but tree is empty** — `ItemsSource` is null. Bind data first.
- **Wrong column data shown after restore** — columns lack unique identification. Set `UseFieldNameForSerialization="True"` (default) or `x:Name` per column.
- **Unbound nodes lose state** — XAML-defined `TreeListNode` trees are recreated fresh each load. Use `ExpandStateBinding` per node to persist expand state via data instead.

## Apply to GridControl

For all shared serialization concepts (`DXSerializer`, `StoreLayoutMode`, `AllowProperty`, `AddNewColumns`/`RemoveOldColumns`, MVVM patterns, lifecycle events):

See [the data-grid skill's `references/save-restore-layout.md`](../../devexpress-wpf-data-grid/references/save-restore-layout.md).

This tree-list reference covers only the TreeList-specific state (expand / check) and ordering requirements (load before restore).

## Source Material

- `articles/common-concepts/save-and-restore-layouts.md`
- `articles/controls-and-libraries/data-grid/miscellaneous/save-and-restore-layout.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/expand-and-collapse-nodes.md` § "Expand State"
- The data-grid skill's `references/save-restore-layout.md` (shared infrastructure)
