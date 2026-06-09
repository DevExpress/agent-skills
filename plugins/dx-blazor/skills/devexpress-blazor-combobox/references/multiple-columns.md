# ComboBox — Multiple Columns

**When to use this reference**: Load this reference when the drop-down list needs to display data across multiple columns.

---

## Overview

`DxComboBox` can show data in a multi-column table inside the drop-down window. This helps users identify items when multiple fields are needed (e.g., ID + First Name + Last Name).

### Steps

1. Add `<Columns>…</Columns>` inside `<DxComboBox>`.
2. Populate with `DxListEditorColumn` objects — one per column.
3. Set `FieldName` on each column to the data source property to display.
4. Use `EditFormat` on the `DxComboBox` to control how the selected value is shown in the edit box (uses `{0}`, `{1}`, … positional format based on column order).

---

## Example

```razor
@using StaffData

<DxComboBox Data="@Staff.DataSource"
            @bind-Value="@SelectedPerson"
            EditFormat="{1} {2}">
    <Columns>
        <DxListEditorColumn FieldName="Id" Width="50px" />
        <DxListEditorColumn FieldName="FirstName" Caption="Name" />
        <DxListEditorColumn FieldName="LastName" Caption="Surname" />
    </Columns>
</DxComboBox>

@code {
    Person SelectedPerson { get; set; } = Staff.DataSource[0];
}
```

`EditFormat="{1} {2}"` shows column index 1 (FirstName) and column index 2 (LastName) separated by a space in the edit box when an item is selected.

---

## `DxListEditorColumn` Properties

| Property | Type | Description |
|---|---|---|
| `FieldName` | `string` | Data source field for this column (required) |
| `Caption` | `string` | Column header label |
| `Width` | `string` | CSS width value, e.g. `"50px"`, `"20%"` |
| `SearchEnabled` | `bool` | Whether this column participates in search (default `true`) |
| `CellDisplayTemplate` | `RenderFragment<ListBoxColumnCellDisplayTemplateContext<TData>>` | Custom cell rendering for this column |

---

## Cell Display Templates

Use `CellDisplayTemplate` on a column to customise individual cell rendering. Use `ColumnCellDisplayTemplate` on the `DxComboBox` itself as a fallback for all columns. Column-specific templates take precedence.

```razor
<DxComboBox Data="@Subscriptions.Plans"
            @bind-Value="@SelectedPlan"
            EditFormat="{0}">
    <Columns>
        <DxListEditorColumn FieldName="Name" Caption="Plan">
            <CellDisplayTemplate>
                <b>@context.Value Subscription</b>
            </CellDisplayTemplate>
        </DxListEditorColumn>
        <DxListEditorColumn FieldName="PriceMonth" Caption="Monthly" />
        <DxListEditorColumn FieldName="PriceYear" Caption="Annual" />
    </Columns>
    <ColumnCellDisplayTemplate>
        <div style="text-align:right">@($"{context.Value:C}")</div>
    </ColumnCellDisplayTemplate>
</DxComboBox>

@code {
    SubscriptionPlan SelectedPlan { get; set; }
}
```

In the example above:
- The `Name` column uses its own `CellDisplayTemplate` (bold text).
- The `PriceMonth` and `PriceYear` columns fall back to `ColumnCellDisplayTemplate` (right-aligned currency format).

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Edit box shows wrong text after selection | Adjust `EditFormat` — indices correspond to the column order (0-based) |
| Columns not appearing | Ensure `<Columns>` tag is a direct child of `<DxComboBox>` |
| Search includes unwanted columns | Set `SearchEnabled="false"` on columns that should not participate in filtering |
