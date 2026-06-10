# Selection

## Selection Modes

| Mode | Behavior |
|---|---|
| `'single'` | Click to select one row; click again to deselect |
| `'multiple'` | Checkbox column; Shift/Ctrl+click for range/additive |
| `'none'` | No selection (default) |

## Enabling Selection (all frameworks)

### jQuery

```js
$('#data-grid').dxDataGrid({
    selection: {
        mode: 'multiple',
        showCheckBoxesMode: 'always'  // 'onClick' | 'onLongTap' | 'always' | 'none'
    }
});
```

### Angular

```html
<dx-data-grid>
    <dxo-data-grid-selection mode="multiple" showCheckBoxesMode="always"></dxo-data-grid-selection>
</dx-data-grid>
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid>
    <DxSelection mode="multiple" show-check-boxes-mode="always" />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxSelection } from 'devextreme-vue/data-grid';
```

### React (TSX)

```tsx
import { DataGrid, Selection } from 'devextreme-react/data-grid';

<DataGrid>
    <Selection mode="multiple" showCheckBoxesMode="always" />
</DataGrid>
```

---

## Reading Selected Row Data

### jQuery — via `onSelectionChanged`

```js
$('#data-grid').dxDataGrid({
    selection: { mode: 'multiple' },
    onSelectionChanged(e) {
        const selectedRows = e.selectedRowsData;
        console.log('Selected:', selectedRows);
    }
});
```

### Angular

```html
<dx-data-grid (onSelectionChanged)="onSelectionChanged($event)">
    <dxo-data-grid-selection mode="multiple"></dxo-data-grid-selection>
</dx-data-grid>
```

```ts
import { DxDataGridTypes } from 'devextreme-angular/ui/data-grid';

onSelectionChanged(e: DxDataGridTypes.SelectionChangedEvent) {
    const selected: Employee[] = e.selectedRowsData as Employee[];
    console.log('Selected:', selected);
}
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid @selection-changed="onSelectionChanged">
    <DxSelection mode="multiple" />
</DxDataGrid>
```

```ts
import { DxDataGridTypes } from 'devextreme-vue/data-grid';

function onSelectionChanged(e: DxDataGridTypes.SelectionChangedEvent) {
    console.log('Selected:', e.selectedRowsData);
}
```

### React (TSX)

```tsx
import { DataGrid, Selection, type DataGridTypes } from 'devextreme-react/data-grid';

function onSelectionChanged(e: DataGridTypes.SelectionChangedEvent) {
    console.log(e.selectedRowsData);
}

<DataGrid onSelectionChanged={onSelectionChanged}>
    <Selection mode="multiple" />
</DataGrid>
```

---

## Pre-selecting Rows

Set `selectedRowKeys` to programmatically pre-select rows (must match `keyExpr` values).

```js
// jQuery
$('#data-grid').dxDataGrid({ selectedRowKeys: [1, 3, 5] });
```

```html
<!-- Angular -->
<dx-data-grid [selectedRowKeys]="[1, 3, 5]"></dx-data-grid>
```

```vue
<!-- Vue -->
<DxDataGrid :selected-row-keys="[1, 3, 5]" />
```

```tsx
// React
<DataGrid defaultSelectedRowKeys={[1, 3, 5]} />
```

---

## Getting Selected Rows Programmatically

```js
// jQuery — using instance methods
const grid = $('#data-grid').dxDataGrid('instance');
const keys = grid.getSelectedRowKeys();
const rowsData = grid.getSelectedRowsData();
```

```ts
// Angular — via ViewChild
@ViewChild(DxDataGridComponent) gridRef!: DxDataGridComponent;

getSelected() {
    return this.gridRef.instance.getSelectedRowsData();
}
```

```ts
// Vue — via ref
const gridRef = ref<InstanceType<typeof DxDataGrid>>();
const selected = gridRef.value?.instance.getSelectedRowsData();
```

```ts
// React — via onInitialized
import dxDataGrid from 'devextreme/ui/data_grid';
const gridInstance = useRef<dxDataGrid | null>(null);
const selected = gridInstance.current?.getSelectedRowsData();
```

---

## `onSelectionChanged` Event Object

| Property | Description |
|---|---|
| `e.selectedRowKeys` | Array of keys for all currently selected rows |
| `e.selectedRowsData` | Array of data objects for all currently selected rows |
| `e.currentSelectedRowKeys` | Keys added in this selection change |
| `e.currentDeselectedRowKeys` | Keys removed in this selection change |
| `e.component` | The DataGrid instance |
