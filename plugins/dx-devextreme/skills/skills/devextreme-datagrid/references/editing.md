# Editing

## Editing Modes

| Mode | Description |
|---|---|
| `'row'` | Edit button per row; one row editable at a time |
| `'cell'` | Click any cell to edit it inline; auto-saves on focus leave |
| `'batch'` | Edit multiple cells; changes accumulate until Save is clicked |
| `'popup'` | Opens a popup dialog with a Form for the row |
| `'form'` | Expands the row into an inline Form |

## Enabling Editing

### jQuery

```js
$('#data-grid').dxDataGrid({
    editing: {
        mode: 'row',          // 'row' | 'cell' | 'batch' | 'popup' | 'form'
        allowAdding: true,
        allowUpdating: true,
        allowDeleting: true
    }
});
```

### Angular

```html
<dx-data-grid [dataSource]="employees" keyExpr="EmployeeID">
    <dxo-data-grid-editing
        mode="row"
        [allowAdding]="true"
        [allowUpdating]="true"
        [allowDeleting]="true">
    </dxo-data-grid-editing>
</dx-data-grid>
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid :data-source="employees" key-expr="EmployeeID">
    <DxEditing
        mode="row"
        :allow-adding="true"
        :allow-updating="true"
        :allow-deleting="true"
    />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxEditing } from 'devextreme-vue/data-grid';
```

### React (TSX)

```tsx
import { DataGrid, Editing } from 'devextreme-react/data-grid';

<DataGrid dataSource={employees} keyExpr="EmployeeID">
    <Editing mode="row" allowAdding={true} allowUpdating={true} allowDeleting={true} />
</DataGrid>
```

---

## Column-Level Validation Rules

Specify `validationRules` on individual columns. Rules fire when the user saves a row.

> **See also**: `devextreme-form` skill — `editing.mode: 'popup'` and `editing.mode: 'form'` render validation inside a Form.

```js
// jQuery
columns: [
    {
        dataField: 'FullName',
        validationRules: [{ type: 'required' }]
    },
    {
        dataField: 'Email',
        validationRules: [{ type: 'required' }, { type: 'email' }]
    },
    {
        dataField: 'Salary',
        validationRules: [{ type: 'range', min: 0 }]
    }
]
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="FullName">
    <dxi-data-grid-validation-rule type="required"></dxi-data-grid-validation-rule>
</dxi-data-grid-column>
<dxi-data-grid-column dataField="Email">
    <dxi-data-grid-validation-rule type="required"></dxi-data-grid-validation-rule>
    <dxi-data-grid-validation-rule type="email"></dxi-data-grid-validation-rule>
</dxi-data-grid-column>
```

```vue
<!-- Vue -->
<DxColumn data-field="FullName">
    <DxRequiredRule />
</DxColumn>
<DxColumn data-field="Email">
    <DxRequiredRule />
    <DxEmailRule />
</DxColumn>
```

```tsx
// React
import { Column, RequiredRule, EmailRule, RangeRule } from 'devextreme-react/data-grid';

<Column dataField="FullName"><RequiredRule /></Column>
<Column dataField="Email"><RequiredRule /><EmailRule /></Column>
<Column dataField="Salary"><RangeRule min={0} /></Column>
```

---

## Popup and Form Editing

For `mode: 'popup'` and `mode: 'form'`, configure the embedded form via `editing.form` and the popup via `editing.popup`.

> **See also**: `devextreme-form` skill — `editing.form` accepts the same options as `dxForm`.

```js
// jQuery — popup mode with custom form layout
$('#data-grid').dxDataGrid({
    editing: {
        mode: 'popup',
        allowAdding: true,
        allowUpdating: true,
        popup: {
            title: 'Employee Details',
            showTitle: true,
            width: 700,
            height: 450
        },
        form: {
            colCount: 2,
            items: [
                { dataField: 'FullName',  isRequired: true },
                { dataField: 'Position' },
                { dataField: 'HireDate',  dataType: 'date' },
                { dataField: 'Email',     validationRules: [{ type: 'email' }] }
            ]
        }
    }
});
```

```html
<!-- Angular -->
<dx-data-grid>
    <dxo-data-grid-editing mode="popup" [allowAdding]="true" [allowUpdating]="true">
        <dxo-data-grid-popup title="Employee Details" [showTitle]="true" [width]="700" [height]="450"></dxo-data-grid-popup>
        <dxo-data-grid-form [colCount]="2">
            <dxi-data-grid-simple-item dataField="FullName" [isRequired]="true"></dxi-data-grid-simple-item>
            <dxi-data-grid-simple-item dataField="Position"></dxi-data-grid-simple-item>
        </dxo-data-grid-form>
    </dxo-data-grid-editing>
</dx-data-grid>
```

---

## Controlling Edit Access Per Row

Use `allowUpdating` and `allowDeleting` as functions to conditionally allow editing based on row data.

```js
// jQuery
editing: {
    mode: 'row',
    allowUpdating(e) { return e.row.data.status !== 'locked'; },
    allowDeleting(e) { return e.row.data.status !== 'locked'; }
}
```

```html
<!-- Angular -->
<dxo-data-grid-editing
    mode="row"
    [allowUpdating]="canEdit"
    [allowDeleting]="canEdit">
</dxo-data-grid-editing>
```

```ts
// Angular component
canEdit = (e: { row: { data: Employee } }) => e.row.data.status !== 'locked';
```

---

## Handling Save/Cancel Events

```js
// jQuery — react to a batch save or row insert
$('#data-grid').dxDataGrid({
    onSaving(e) {
        // e.changes — array of pending changes
        // e.cancel = true — abort the save
    },
    onRowInserted(e) {
        console.log('Inserted:', e.data);
    },
    onRowUpdated(e) {
        console.log('Updated key:', e.key, 'New values:', e.data);
    },
    onRowRemoved(e) {
        console.log('Removed key:', e.key);
    }
});
```

For server-connected grids, implement `insert`, `update`, `remove` on the `CustomStore` instead of handling these events.
