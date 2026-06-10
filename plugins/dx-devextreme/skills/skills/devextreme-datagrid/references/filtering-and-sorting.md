# Filtering and Sorting

## Filter UI Elements

| Element | Option | What it shows |
|---|---|---|
| Filter row | `filterRow.visible` | Per-column input row below the header |
| Search panel | `searchPanel.visible` | Global text search input in toolbar |
| Header filter | `headerFilter.visible` | Dropdown filter icon in each column header |
| Filter panel | `filterPanel.visible` | Visual filter builder bar below the grid |

These can be combined freely. For most use cases, `filterRow` + `searchPanel` is sufficient.

---

## Enabling Filter UI (all frameworks)

### jQuery

```js
$('#data-grid').dxDataGrid({
    filterRow:   { visible: true },
    searchPanel: { visible: true, width: 240, placeholder: 'Search...' },
    headerFilter: { visible: true },
    filterPanel: { visible: true }
});
```

### Angular

```html
<dx-data-grid>
    <dxo-data-grid-filter-row    [visible]="true"></dxo-data-grid-filter-row>
    <dxo-data-grid-search-panel  [visible]="true" [width]="240" placeholder="Search..."></dxo-data-grid-search-panel>
    <dxo-data-grid-header-filter [visible]="true"></dxo-data-grid-header-filter>
    <dxo-data-grid-filter-panel  [visible]="true"></dxo-data-grid-filter-panel>
</dx-data-grid>
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid>
    <DxFilterRow    :visible="true" />
    <DxSearchPanel  :visible="true" :width="240" placeholder="Search..." />
    <DxHeaderFilter :visible="true" />
    <DxFilterPanel  :visible="true" />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxFilterRow, DxSearchPanel, DxHeaderFilter, DxFilterPanel } from 'devextreme-vue/data-grid';
```

### React (TSX)

```tsx
import { DataGrid, FilterRow, SearchPanel, HeaderFilter, FilterPanel } from 'devextreme-react/data-grid';

<DataGrid>
    <FilterRow    visible={true} />
    <SearchPanel  visible={true} width={240} placeholder="Search..." />
    <HeaderFilter visible={true} />
    <FilterPanel  visible={true} />
</DataGrid>
```

---

## Applying a Programmatic Filter

Use `dataSource.filter()` or the component's `filter()` method to apply a filter from code.

```js
// jQuery — filter on a column value
const grid = $('#data-grid').dxDataGrid('instance');
grid.filter(['Country', '=', 'USA']);

// Clear the filter
grid.clearFilter();
```

For a DataSource-level filter that survives reloads:

```ts
dataSource.filter(['IsActive', '=', true]);
dataSource.load();
```

---

## Per-Column Filter Configuration

Disable or customize filtering on individual columns:

```js
// jQuery — disable filtering for a specific column
{ dataField: 'InternalNotes', allowFiltering: false }

// Custom header filter values
{
    dataField: 'Status',
    headerFilter: {
        dataSource: ['Active', 'Inactive', 'Pending']
    }
}
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="InternalNotes" [allowFiltering]="false"></dxi-data-grid-column>
```

---

## Sorting

Sorting is enabled by default for all columns. Users click column headers to sort. Multi-column sort is available via Ctrl+click.

### Default sort order

```js
// jQuery — pre-sort by HireDate descending
{ dataField: 'HireDate', sortOrder: 'desc', sortIndex: 0 }
```

```html
<!-- Angular -->
<dxi-data-grid-column dataField="HireDate" sortOrder="desc" [sortIndex]="0"></dxi-data-grid-column>
```

```vue
<!-- Vue -->
<DxColumn data-field="HireDate" sort-order="desc" :sort-index="0" />
```

```tsx
// React
<Column dataField="HireDate" sortOrder="desc" sortIndex={0} />
```

### Disable sorting globally

```js
// jQuery
$('#data-grid').dxDataGrid({ sorting: { mode: 'none' } });
```

### Disable sorting per column

```js
{ dataField: 'Photo', allowSorting: false }
```

---

## Remote Filtering and Sorting

When using a `CustomStore` with server-side data, set `remoteOperations: true` so the grid passes filter/sort state to `LoadOptions` instead of processing locally.

```js
// jQuery
$('#data-grid').dxDataGrid({ remoteOperations: true });
```

For fine-grained control:

```js
remoteOperations: {
    filtering: true,
    sorting: true,
    paging: true,
    grouping: false,    // group client-side
    summary: false      // summarize client-side
}
```
