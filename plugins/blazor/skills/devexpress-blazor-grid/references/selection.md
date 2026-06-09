# Selection — Blazor Grid

When you need to: enable single or multiple row selection; get selected items; add checkboxes; select rows programmatically.

## Selection Modes

| Mode | Description |
|---|---|
| `GridSelectionMode.Single` | Only one row can be selected at a time |
| `GridSelectionMode.Multiple` | Multiple rows can be selected (default) |

## Single Row Selection

```razor
<DxGrid Data="@Items"
        KeyFieldName="Id"
        SelectionMode="GridSelectionMode.Single"
        @bind-SelectedDataItem="@SelectedItem">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
</DxGrid>

@if (SelectedItem is Product p) {
    <p>Selected: @p.Name</p>
}

@code {
    object SelectedItem { get; set; }
    List<Product> Items { get; set; } = new();
}
```

## Multiple Row Selection with Checkboxes

```razor
<DxGrid Data="@Items"
        KeyFieldName="Id"
        SelectionMode="GridSelectionMode.Multiple"
        @bind-SelectedDataItems="@SelectedItems">
    <Columns>
        <DxGridSelectionColumn Width="50px" AllowSelectAll="true" />
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Price" />
    </Columns>
</DxGrid>

<p>Selected count: @SelectedItems.Count</p>

@code {
    IReadOnlyList<object> SelectedItems { get; set; } = new List<object>();
    List<Product> Items { get; set; } = new();
}
```

## Allow Row Click to Select

```razor
<DxGrid AllowSelectRowByClick="true"
        SelectionMode="GridSelectionMode.Multiple"
        @bind-SelectedDataItems="@SelectedItems"
        ...>
```

Keyboard shortcuts when `AllowSelectRowByClick` is true:
- `Click` — select row, clear others
- `Shift+Click` — extend selection range
- `Ctrl+Click` — toggle row in/out of selection

## Programmatic Selection

| Method | Description |
|---|---|
| `SelectRow(visibleIndex)` | Select a row by visible index |
| `DeselectRow(visibleIndex)` | Deselect a row by visible index |
| `SelectDataItem(dataItem)` | Select by data item reference |
| `DeselectDataItem(dataItem)` | Deselect by data item reference |
| `SelectAllAsync()` | Select all rows (async) |
| `DeselectAllAsync()` | Deselect all rows (async) |

```csharp
// Select all rows programmatically
await Grid.SelectAllAsync();

// Select a specific item
var item = Items.First(p => p.Id == 5);
Grid.SelectDataItem(item);
```

## SelectedDataItemsChanged Event

React when selection changes:

```razor
<DxGrid SelectedDataItemsChanged="OnSelectionChanged" ...>

@code {
    async Task OnSelectionChanged(IReadOnlyList<object> items) {
        // Do something with newly selected items
    }
}
```

## KeyFieldName Requirement

Always specify `KeyFieldName` (or `KeyFieldNames` for composite keys) when using selection. Without it, the Grid uses reference equality which may produce inconsistent results when data is reloaded:

```razor
<DxGrid KeyFieldName="ProductId" ...>

<!-- Composite key: -->
<DxGrid KeyFieldNames="@(new[] { "OrderId", "ProductId" })" ...>
```
