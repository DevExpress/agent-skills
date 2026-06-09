# Validation — DevExpress WPF TreeList

`TreeListControl` supports the same validation strategies as `GridControl` — **GridControl-level** (event / MVVM command on user edits) and **Data-level** (interfaces or DataAnnotations). The naming differs at the row scope: TreeList uses **`ValidateNodeCommand`** instead of `ValidateRowCommand`.

**For the full validation reference (decision matrix, `ValidationErrorInfo`, severity types, `IDXDataErrorInfo`/`IDataErrorInfo`/`INotifyDataErrorInfo`, DataAnnotations, error window customization, `ValidationService`, `CellTemplate` caveat) see [the data-grid skill's `references/validation.md`](../../devexpress-wpf-data-grid/references/validation.md).** This reference covers only TreeList-specific differences.

## When to Use This Reference

Use this when you need to:

- Wire MVVM validation in a TreeList (use `ValidateNodeCommand`, not `ValidateRowCommand`)
- Validate cell values in a TreeList (`ValidateCellCommand` works identically)
- Customize the invalid-node error window
- Apply interface-based or attribute-based validation to TreeList data

## TreeList-Specific Members

| GridControl member | TreeListControl equivalent |
|---|---|
| `GridViewBase.ValidateRow` event | `TreeListView.ValidateNode` event |
| `GridViewBase.ValidateRowCommand` | **`TreeListView.ValidateNodeCommand`** |
| `GridViewBase.ValidateCell` event | `TreeListView.ValidateCell` event |
| `GridViewBase.ValidateCellCommand` | `TreeListView.ValidateCellCommand` |
| `GridViewBase.InvalidRowExceptionCommand` | `TreeListView.InvalidNodeExceptionCommand` |

Everything else (`ValidationErrorInfo`, severity types, attribute-based, interface-based, `ValidationService`, `HasValidationError`/`HasErrors`) is identical.

## Wire Node Validation (MVVM)

```xaml
<dxg:TreeListView ValidateNodeCommand="{Binding ValidateNodeCommand}"
                  ValidateCellCommand="{Binding ValidateCellCommand}"
                  InvalidNodeExceptionCommand="{Binding InvalidNodeCommand}"/>
```

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;

public class MainViewModel : ViewModelBase {
    [Command]
    public void ValidateNode(NodeValidationArgs args) {
        var task = (Task)args.Item;
        if (task.StartDate > task.EndDate)
            args.Result = new ValidationErrorInfo(
                "Start date must be before end date.",
                ValidationErrorType.Critical);
    }

    [Command]
    public void ValidateCell(NodeCellValidationArgs args) {
        if (args.FieldName == nameof(Task.Progress)) {
            var progress = Convert.ToInt32(args.NewValue);
            if (progress < 0 || progress > 100)
                args.Result = "Progress must be 0–100.";
        }
    }

    [Command]
    public void InvalidNode(InvalidNodeExceptionArgs args) {
        args.ExceptionMode = ExceptionMode.NoAction;   // Suppress the modal error window
    }
}
```

> Note: the args types are TreeList-specific — **`NodeValidationArgs`** for `ValidateNodeCommand`, **`NodeCellValidationArgs`** for `ValidateCellCommand`, and **`InvalidNodeExceptionArgs`** for `InvalidNodeExceptionCommand` (all in `DevExpress.Mvvm.Xpf`). These are siblings of the `GridControl` `Row*` arg classes (common base `ValidationArgs` / `InvalidItemExceptionArgs`), so you must use the `Node*` names here — `RowValidationArgs` will not bind to a TreeList node command. The shared members (`Item`, `Result`) come from the common base; `Item` gives you the bound data object.

If you need the processed node and its ancestors, use the `NodeValidationArgs.Path` property (it starts with the processed node and includes all parent nodes):

```csharp
[Command]
public void ValidateNode(NodeValidationArgs args) {
    var node = args.Path.First();            // the processed node's data item
    var parents = args.Path.Skip(1);         // ancestor data items
    // ... validation that depends on ancestors
}
```

## When Validation Runs in a Tree

- **`ValidateCell`** — when a cell editor closes (user navigates to another cell in the same node).
- **`ValidateNode`** — when the user moves focus to another **node**, or `CommitEditing` is called.
- **Move to a parent/child node** counts as a row move (= node validation runs).
- **Expand/collapse a node** does NOT trigger validation — only focus changes do.

## Validation Error Display in Tree Context

- **Cell errors**: error icon inside the cell, tooltip on hover (same as Grid).
- **Node errors**: error icon at the node's left side, before the expand button. Tooltip on hover.
- The error icon respects the node's indent level — it appears at the level of the node, not at column 0.

## Indeterminate Validation State for Nodes With Children

When a parent node is collapsed and a hidden child has a validation error, the parent's appearance depends on:

```xaml
<dxg:TreeListView ShowChildErrorIndication="True"/>
```

> Verify exact property name (`ShowChildErrorIndication` vs `IndicateChildErrors` vs similar) against the apidoc — paraphrased here from common DevExpress conventions:
> `devexpress_docs_search(technology="WPF TreeList", query="show child error indication validation")`

Without this property: collapsed parent shows no error even though its child fails validation.

## Interface-Based Validation in a Tree

`IDXDataErrorInfo`, `IDataErrorInfo`, `INotifyDataErrorInfo` all work identically to `GridControl`. The grid evaluates per-record errors regardless of tree level. See [data-grid validation.md § Interface-Based Validation](../../devexpress-wpf-data-grid/references/validation.md).

For tree-specific scenarios where a parent's validity depends on its children's state, implement the parent's `GetError` to walk the children:

```csharp
public class Task : IDXDataErrorInfo {
    public List<Task> SubTasks { get; set; } = new();

    public void GetError(ErrorInfo info) {
        if (SubTasks.Any(s => !s.IsCompleted) && IsCompleted) {
            info.ErrorText = "Cannot complete a task while subtasks are incomplete.";
            info.ErrorType = ErrorType.Critical;
        }
    }

    public void GetPropertyError(string propertyName, ErrorInfo info) {
        // Standard per-property validation
    }
}
```

For TreeList in **hierarchical binding mode** (`ChildNodesPath`), this works naturally — the parent object owns its children. In **self-referential mode** (`KeyFieldName`/`ParentFieldName`), iterating children requires looking up by parent key, which the data class doesn't know about by itself. Move child-aware validation to the ViewModel command:

```csharp
[Command]
public void ValidateNode(NodeValidationArgs args) {
    var task = (Task)args.Item;
    var children = AllTasks.Where(t => t.ParentID == task.ID).ToList();
    if (children.Any(c => !c.IsCompleted) && task.IsCompleted) {
        args.Result = "Subtasks must be complete first.";
    }
}
```

## DataAnnotations in a Tree

DataAnnotations attributes work identically:

```csharp
public class Task {
    [Required(ErrorMessage = "Task name required.")]
    public string Name { get; set; } = "";

    [Range(0, 100)]
    public int Progress { get; set; }
}
```

See [data-grid validation.md § Attribute-Based Validation](../../devexpress-wpf-data-grid/references/validation.md).

**Same limitation**: DataAnnotations don't work on `ColumnBase.Binding`-bound columns. Use `FieldName`.

## Validation Inside Edit Forms (Tree)

Edit Forms work on `TreeListView` (`EditFormShowMode`, `ShowEditFormOnDoubleClick`, etc.). Validation pipeline is identical to GridControl's Edit Form validation — DataAnnotations + interface errors show on form editors; `ValidateNodeCommand` fires on form save.

See [data-grid cell-display-and-editing.md § Edit Form](../../devexpress-wpf-data-grid/references/cell-display-and-editing.md).

## Common Issues

- **`ValidateRowCommand` doesn't fire on TreeList** — wrong command name. Use **`ValidateNodeCommand`**.
- **Parent node shows no error when collapsed child fails validation** — set `ShowChildErrorIndication="True"` (verify exact name).
- **Validation runs on every cell edit, even when other cells are still being edited** — by design: `ValidateCell` fires on each cell commit. Use `ValidateNode` if you want validation only on row commit.
- **`args.Item` is null in `ValidateNode`** — only happens during data initialization. Guard with `if (args.Item == null) return;`.
- **Custom CellTemplate breaks validation** — same as Grid: needs `PART_Editor` name. See data-grid's validation.md.

## Apply to GridControl

For all shared concerns (`ValidationErrorInfo`, severity types, interface-based, attribute-based, `ValidationService`, decision matrix), see [the data-grid skill's `references/validation.md`](../../devexpress-wpf-data-grid/references/validation.md).

This tree-list reference covers only the renamed commands (`ValidateNodeCommand`, `InvalidNodeExceptionCommand`) and tree-specific patterns (parent-child validation, child error indication).

## Source Material

- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/cell-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/row-validation.md` (contains `ValidateNodeCommand` mapping)
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/interface-based-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/attribute-based-validation.md`
- The data-grid skill's `references/validation.md` (full shared reference)
