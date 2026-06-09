# Grouping and Summaries

## Grouping

### Enable the group panel (drag-to-group UI)

```js
// jQuery
$('#data-grid').dxDataGrid({
    groupPanel: { visible: true }
});
```

```html
<!-- Angular -->
<dx-data-grid><dxo-data-grid-group-panel [visible]="true"></dxo-data-grid-group-panel></dx-data-grid>
```

```vue
<!-- Vue -->
<DxDataGrid><DxGroupPanel :visible="true" /></DxDataGrid>
```

```tsx
// React
import { DataGrid, GroupPanel } from 'devextreme-react/data-grid';
<DataGrid><GroupPanel visible={true} /></DataGrid>
```

### Pre-group in code

Set `groupIndex` on a column to pre-group data. Multiple columns can be grouped; `groupIndex: 0` is the outermost group.

```js
// jQuery
columns: [
    { dataField: 'Country',     groupIndex: 0 },
    { dataField: 'Department',  groupIndex: 1 },
    { dataField: 'FullName' }
]
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="Country"    [groupIndex]="0"></dxi-data-grid-column>
<dxi-data-grid-column dataField="Department" [groupIndex]="1"></dxi-data-grid-column>
```

```vue
<!-- Vue -->
<DxColumn data-field="Country"    :group-index="0" />
<DxColumn data-field="Department" :group-index="1" />
```

```tsx
// React
<Column dataField="Country"    groupIndex={0} />
<Column dataField="Department" groupIndex={1} />
```

### Auto-expand groups

```js
// jQuery — collapse all groups by default
$('#data-grid').dxDataGrid({
    grouping: { autoExpandAll: false }
});
```

```html
<!-- Angular -->
<dxo-data-grid-grouping [autoExpandAll]="false"></dxo-data-grid-grouping>
```

---

## Summaries

Summaries aggregate data and display results in a **total row** (bottom of the grid) or **group rows**.

### Predefined aggregate functions

| `summaryType` | Description |
|---|---|
| `'sum'` | Sum of all values |
| `'avg'` | Average |
| `'min'` | Minimum value |
| `'max'` | Maximum value |
| `'count'` | Row count |

### Total summaries

Displayed in a fixed row at the bottom of the grid.

```js
// jQuery
$('#data-grid').dxDataGrid({
    summary: {
        totalItems: [
            { column: 'Salary', summaryType: 'sum',   displayFormat: 'Total: {0}', valueFormat: { type: 'currency', precision: 0 } },
            { column: 'FullName', summaryType: 'count', displayFormat: '{0} employees' }
        ]
    }
});
```

```html
<!-- Angular -->
<dxo-data-grid-summary>
    <dxi-data-grid-total-item column="Salary"   summaryType="sum"   displayFormat="Total: {0}" [valueFormat]="currencyFormat"></dxi-data-grid-total-item>
    <dxi-data-grid-total-item column="FullName" summaryType="count" displayFormat="{0} employees"></dxi-data-grid-total-item>
</dxo-data-grid-summary>
```

```vue
<!-- Vue -->
<template>
    <DxDataGrid>
        <DxSummary>
            <DxTotalItem column="Salary"   summary-type="sum"   display-format="Total: {0}" :value-format="{ type: 'currency', precision: 0 }" />
            <DxTotalItem column="FullName" summary-type="count" display-format="{0} employees" />
        </DxSummary>
    </DxDataGrid>
</template>

<script setup lang="ts">
import { DxDataGrid, DxSummary, DxTotalItem } from 'devextreme-vue/data-grid';
</script>
```

```tsx
// React
import { DataGrid, Summary, TotalItem } from 'devextreme-react/data-grid';

const salaryFormat = { type: 'currency', precision: 0 };

<DataGrid>
    <Summary>
        <TotalItem column="Salary"   summaryType="sum"   displayFormat="Total: {0}" valueFormat={salaryFormat} />
        <TotalItem column="FullName" summaryType="count" displayFormat="{0} employees" />
    </Summary>
</DataGrid>
```

### Group summaries

Displayed inside each group row.

```js
// jQuery
summary: {
    groupItems: [
        { summaryType: 'count', displayFormat: '{0} records' },
        { column: 'Salary', summaryType: 'avg', displayFormat: 'Avg salary: {0}', valueFormat: { type: 'currency', precision: 0 }, alignByColumn: true }
    ]
}
```

```html
<!-- Angular -->
<dxo-data-grid-summary>
    <dxi-data-grid-group-item summaryType="count"  displayFormat="{0} records"></dxi-data-grid-group-item>
    <dxi-data-grid-group-item column="Salary" summaryType="avg" displayFormat="Avg: {0}" [alignByColumn]="true"></dxi-data-grid-group-item>
</dxo-data-grid-summary>
```

```vue
<!-- Vue -->
<DxSummary>
    <DxGroupItem summary-type="count" display-format="{0} records" />
    <DxGroupItem column="Salary" summary-type="avg" display-format="Avg: {0}" :align-by-column="true" />
</DxSummary>
```

```tsx
// React
<Summary>
    <GroupItem summaryType="count" displayFormat="{0} records" />
    <GroupItem column="Salary" summaryType="avg" displayFormat="Avg: {0}" alignByColumn={true} />
</Summary>
```
