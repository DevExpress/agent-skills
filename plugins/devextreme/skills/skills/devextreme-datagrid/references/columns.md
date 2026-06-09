# Columns Configuration

## Auto-Generated vs Explicit Columns

If you omit `columns`, the DataGrid auto-generates one column per field in the first data record. To control order, appearance, and behavior, always declare `columns[]` explicitly.

```js
// jQuery — string shorthand (just dataField, no extra config)
$('#data-grid').dxDataGrid({
    dataSource: employees,
    keyExpr: 'EmployeeID',
    columns: ['FullName', 'Position', 'HireDate']
});
```

---

## Column Properties

| Property | Type | Description |
|---|---|---|
| `dataField` | `String` | Field name in the data object |
| `caption` | `String` | Column header text (auto-generated from `dataField` if omitted) |
| `dataType` | `String` | `'string'`, `'number'`, `'date'`, `'datetime'`, `'boolean'`, `'object'` |
| `format` | `String \| Object` | Display format (e.g., `'currency'`, `'percent'`, `{ type: 'fixedPoint', precision: 2 }`) |
| `width` | `Number \| String` | Column width in px or `'%'` |
| `minWidth` | `Number` | Minimum column width in px |
| `visible` | `Boolean` | Hides column from view (still loaded) |
| `allowSorting` | `Boolean` | Enable/disable sorting for this column (default `true`) |
| `allowFiltering` | `Boolean` | Enable/disable filtering for this column |
| `allowGrouping` | `Boolean` | Enable/disable grouping for this column |
| `allowEditing` | `Boolean` | Enable/disable editing for this column |
| `fixed` | `Boolean` | Pins the column (requires `fixedPosition`) |
| `fixedPosition` | `String` | `'left'` or `'right'` |
| `groupIndex` | `Number` | Pre-groups data by this column; lower index = outer group |
| `sortIndex` | `Number` | Multi-column sort order |
| `sortOrder` | `String` | `'asc'` or `'desc'` |
| `alignment` | `String` | `'left'`, `'center'`, `'right'` |
| `cssClass` | `String` | CSS class applied to all cells in this column |
| `encodeHtml` | `Boolean` | HTML-encodes cell values (default `true` — do not disable unless trusted) |
| `lookup` | `Object` | Renders a SelectBox editor and displays `displayExpr` text |
| `cellTemplate` | `template` | Custom cell render function |
| `headerCellTemplate` | `template` | Custom header render function |
| `editCellTemplate` | `template` | Custom editor render function in edit mode |
| `calculateCellValue` | `function(row)` | Compute a derived cell value |
| `calculateDisplayValue` | `String \| function` | Compute display text without affecting sorting |

---

## Basic Column Configuration (all frameworks)

### jQuery

```js
$('#data-grid').dxDataGrid({
    columns: [
        { dataField: 'FullName',  caption: 'Name',       width: 200 },
        { dataField: 'Position',  caption: 'Job Title' },
        { dataField: 'HireDate',  caption: 'Hired',       dataType: 'date',   format: 'shortDate' },
        { dataField: 'Salary',    caption: 'Salary',      dataType: 'number', format: { type: 'currency', precision: 0 } },
        { dataField: 'IsActive',  caption: 'Active',      dataType: 'boolean' }
    ]
});
```

### Angular

```html
<dx-data-grid [dataSource]="employees" keyExpr="EmployeeID">
    <dxi-data-grid-column dataField="FullName"  caption="Name"      [width]="200"></dxi-data-grid-column>
    <dxi-data-grid-column dataField="Position"  caption="Job Title"></dxi-data-grid-column>
    <dxi-data-grid-column dataField="HireDate"  caption="Hired"     dataType="date"    format="shortDate"></dxi-data-grid-column>
    <dxi-data-grid-column dataField="Salary"    caption="Salary"    dataType="number"  [format]="salaryFormat"></dxi-data-grid-column>
    <dxi-data-grid-column dataField="IsActive"  caption="Active"    dataType="boolean"></dxi-data-grid-column>
</dx-data-grid>
```

```ts
salaryFormat = { type: 'currency', precision: 0 };
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid :data-source="employees" key-expr="EmployeeID">
    <DxColumn data-field="FullName"  caption="Name"      :width="200" />
    <DxColumn data-field="Position"  caption="Job Title" />
    <DxColumn data-field="HireDate"  caption="Hired"     data-type="date"    format="shortDate" />
    <DxColumn data-field="Salary"    caption="Salary"    data-type="number"  :format="{ type: 'currency', precision: 0 }" />
    <DxColumn data-field="IsActive"  caption="Active"    data-type="boolean" />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxColumn } from 'devextreme-vue/data-grid';
```

### React (TSX)

```tsx
import { DataGrid, Column } from 'devextreme-react/data-grid';

const salaryFormat = { type: 'currency', precision: 0 };

<DataGrid dataSource={employees} keyExpr="EmployeeID">
    <Column dataField="FullName"  caption="Name"      width={200} />
    <Column dataField="Position"  caption="Job Title" />
    <Column dataField="HireDate"  caption="Hired"     dataType="date"    format="shortDate" />
    <Column dataField="Salary"    caption="Salary"    dataType="number"  format={salaryFormat} />
    <Column dataField="IsActive"  caption="Active"    dataType="boolean" />
</DataGrid>
```

---

## Lookup Column (SelectBox-like display)

A `lookup` column stores a key value but displays a human-readable label. Use it for FK columns (e.g., `DepartmentID` → department name).

> **See also**: `devextreme-selectbox` skill — lookup uses similar `valueExpr`/`displayExpr` semantics.

```js
// jQuery
{
    dataField: 'DepartmentID',
    caption: 'Department',
    lookup: {
        dataSource: departments,   // [{ ID: 1, Name: 'IT' }, ...]
        valueExpr: 'ID',
        displayExpr: 'Name'
    }
}
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="DepartmentID" caption="Department">
    <dxo-data-grid-column-lookup [dataSource]="departments" valueExpr="ID" displayExpr="Name"></dxo-data-grid-column-lookup>
</dxi-data-grid-column>
```

```vue
<!-- Vue -->
<DxColumn data-field="DepartmentID" caption="Department">
    <DxLookup :data-source="departments" value-expr="ID" display-expr="Name" />
</DxColumn>
```

```tsx
// React
import { Column, Lookup } from 'devextreme-react/data-grid';
<Column dataField="DepartmentID" caption="Department">
    <Lookup dataSource={departments} valueExpr="ID" displayExpr="Name" />
</Column>
```

---

## Fixed (Frozen) Columns

```js
// jQuery
{ dataField: 'FullName', fixed: true, fixedPosition: 'left' }
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="FullName" [fixed]="true" fixedPosition="left"></dxi-data-grid-column>
```

---

## Cell Template (Custom Rendering)

> **See also**: `devextreme-button` skill — for rendering action buttons inside cells.

```js
// jQuery
{
    dataField: 'Status',
    cellTemplate(container, options) {
        const badge = $('<span>').addClass(`badge badge-${options.value.toLowerCase()}`).text(options.value);
        container.append(badge);
    }
}
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="Status" cellTemplate="statusTemplate"></dxi-data-grid-column>
<div *dxTemplate="let cell of 'statusTemplate'">
    <span [class]="'badge badge-' + cell.value.toLowerCase()">{{ cell.value }}</span>
</div>
```

```vue
<!-- Vue -->
<DxColumn data-field="Status" cell-template="statusTemplate" />
<template #statusTemplate="{ data }">
    <span :class="'badge badge-' + data.value.toLowerCase()">{{ data.value }}</span>
</template>
```

```tsx
// React
function StatusCell({ data }: { data: { value: string } }) {
    return <span className={`badge badge-${data.value.toLowerCase()}`}>{data.value}</span>;
}
const renderStatus = (data: { value: string }) => <StatusCell data={data} />;
<Column dataField="Status" cellRender={renderStatus} />
```

---

## Calculated Columns

```js
// jQuery — computed full name, not stored in data
{
    caption: 'Full Name',
    calculateCellValue(row) {
        return `${row.FirstName} ${row.LastName}`;
    },
    allowSorting: false
}
```
