# Columns & Templates — Blazor Grid

When you need to: configure columns; add command or selection columns; customize cell display or edit templates; create unbound columns; use band (header) columns.

## Column Types

| Column Type | Class | Purpose |
|---|---|---|
| Data column | `DxGridDataColumn` | Displays and edits data source fields |
| Command column | `DxGridCommandColumn` | Shows New / Edit / Delete buttons |
| Selection column | `DxGridSelectionColumn` | Adds checkboxes for row selection |
| Band column | `DxGridBandColumn` | Groups columns under a shared header |

## DxGridDataColumn Key Properties

| Property | Type | Description |
|---|---|---|
| `FieldName` | `string` | Data source field name (required, case-sensitive) |
| `Caption` | `string` | Column header text |
| `Width` | `string` | Column width (`"150px"`, `"20%"`) |
| `MinWidth` | `int` | Minimum column width in pixels |
| `DisplayFormat` | `string` | Format string for display (`"d"`, `"C2"`, `"N0"`) |
| `Visible` | `bool` | Show or hide the column |
| `FixedPosition` | `GridColumnFixedPosition` | Freeze column to Left or Right |
| `AllowSort` | `bool` | Allow user sorting |
| `AllowGroup` | `bool` | Allow grouping by this column |
| `AllowResize` | `bool` | Allow column resize |
| `SortOrder` | `GridColumnSortOrder` | Ascending / Descending |
| `SortIndex` | `int` | Position in multi-column sort (0-based) |
| `GroupIndex` | `int` | Position in group-by order |
| `GroupInterval` | `GridColumnGroupInterval` | Group by date/numeric interval |
| `UnboundExpression` | `string` | Expression for calculated values |
| `UnboundType` | `GridUnboundColumnType` | Data type for unbound column |

## DxGridCommandColumn Key Properties

| Property | Type | Description |
|---|---|---|
| `Width` | `string` | Column width |
| `NewButtonVisible` | `bool` | Show the New row button |
| `EditButtonVisible` | `bool` | Show the Edit button |
| `DeleteButtonVisible` | `bool` | Show the Delete button |
| `SaveButtonVisible` | `bool` | Show Save button (edit mode) |
| `CancelButtonVisible` | `bool` | Show Cancel button (edit mode) |

## Templates

### CellDisplayTemplate — Custom Cell Rendering

```razor
<DxGridDataColumn FieldName="Status">
    <CellDisplayTemplate>
        @{
            var status = (string)context.Value;
            <span class="badge @(status == "Active" ? "bg-success" : "bg-secondary")">
                @status
            </span>
        }
    </CellDisplayTemplate>
</DxGridDataColumn>
```

`context` is `GridDataColumnCellDisplayTemplateContext`:
- `context.Value` — raw cell value
- `context.DisplayText` — formatted text
- `context.DataItem` — full data item
- `context.Grid` — the grid instance

### CellEditTemplate — Custom Editor in Edit Mode

```razor
<DxGridDataColumn FieldName="Priority">
    <CellEditTemplate>
        <DxComboBox Data="@Priorities"
                    Value="(int)context.EditModel.Priority"
                    ValueChanged="(int v) => context.EditModel.Priority = v" />
    </CellEditTemplate>
</DxGridDataColumn>
```

### HeaderCaptionTemplate — Custom Column Header

```razor
<DxGridDataColumn FieldName="Revenue">
    <HeaderCaptionTemplate>
        <strong>Revenue ($)</strong>
    </HeaderCaptionTemplate>
</DxGridDataColumn>
```

### EditFormTemplate — Custom Edit Form

When `EditMode` is `EditForm` or `PopupEditForm`, use `EditFormTemplate` to design the form:

```razor
<DxGrid Data="@Employees" EditMode="GridEditMode.EditForm" ...>
    <Columns>...</Columns>
    <EditFormTemplate Context="editFormContext">
        <DxFormLayout>
            <DxFormLayoutItem Caption="First Name:">
                @editFormContext.GetEditor("FirstName")
            </DxFormLayoutItem>
            <DxFormLayoutItem Caption="Last Name:">
                @editFormContext.GetEditor("LastName")
            </DxFormLayoutItem>
        </DxFormLayout>
    </EditFormTemplate>
</DxGrid>
```

> Use `Context="editFormContext"` to avoid naming conflicts with parent `context` parameters.

### ToolbarTemplate — Grid Toolbar

```razor
<DxGrid Data="@Items">
    <ToolbarTemplate>
        <DxToolbar>
            <DxToolbarItem Text="Export" Click="ExportData" />
        </DxToolbar>
    </ToolbarTemplate>
    <Columns>...</Columns>
</DxGrid>
```

## Unbound Columns

Unbound columns calculate their values using an expression or via the `UnboundColumnData` event:

```razor
<DxGridDataColumn FieldName="FullName"
                  UnboundType="GridUnboundColumnType.String"
                  UnboundExpression="[FirstName] + ' ' + [LastName]" />
```

Or handle `UnboundColumnData`:

```razor
<DxGrid UnboundColumnData="OnUnboundColumnData" ...>
    <Columns>
        <DxGridDataColumn FieldName="FullName" UnboundType="GridUnboundColumnType.String" />
    </Columns>
</DxGrid>

@code {
    void OnUnboundColumnData(GridUnboundColumnDataEventArgs e) {
        if (e.FieldName == "FullName") {
            var item = (Employee)e.DataItem;
            e.Value = $"{item.FirstName} {item.LastName}";
        }
    }
}
```

## Band (Header) Columns

Group related data columns under a shared header:

```razor
<DxGrid Data="@Items">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridBandColumn Caption="Address">
            <Columns>
                <DxGridDataColumn FieldName="City" />
                <DxGridDataColumn FieldName="Country" />
            </Columns>
        </DxGridBandColumn>
    </Columns>
</DxGrid>
```
