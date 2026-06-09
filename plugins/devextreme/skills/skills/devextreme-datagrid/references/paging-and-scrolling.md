# Paging and Scrolling

## Pager (Standard Paging)

Standard paging shows a fixed number of rows per page with navigation controls.

### jQuery

```js
$('#data-grid').dxDataGrid({
    paging: {
        enabled: true,
        pageSize: 20,
        pageIndex: 0      // zero-based; initial page
    },
    pager: {
        visible: true,
        showPageSizeSelector: true,
        allowedPageSizes: [10, 20, 50],
        showInfo: true,              // "Page 1 of 5 (100 items)"
        showNavigationButtons: true
    }
});
```

### Angular

```html
<dx-data-grid>
    <dxo-data-grid-paging [enabled]="true" [pageSize]="20"></dxo-data-grid-paging>
    <dxo-data-grid-pager
        [visible]="true"
        [showPageSizeSelector]="true"
        [allowedPageSizes]="[10, 20, 50]"
        [showInfo]="true">
    </dxo-data-grid-pager>
</dx-data-grid>
```

### Vue (Composition API + TypeScript)

```vue
<DxDataGrid>
    <DxPaging :enabled="true" :page-size="20" />
    <DxPager
        :visible="true"
        :show-page-size-selector="true"
        :allowed-page-sizes="[10, 20, 50]"
        :show-info="true"
    />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxPaging, DxPager } from 'devextreme-vue/data-grid';
```

### React (TSX)

```tsx
import { DataGrid, Paging, Pager } from 'devextreme-react/data-grid';

<DataGrid>
    <Paging enabled={true} pageSize={20} />
    <Pager visible={true} showPageSizeSelector={true} allowedPageSizes={[10, 20, 50]} showInfo={true} />
</DataGrid>
```

---

## Virtual and Infinite Scrolling

Use scrolling modes for large datasets where paging UI is undesirable. **Always set a fixed `height` on the grid** — without it, virtual scrolling renders all rows and loses its performance benefit.

| Mode | Behavior |
|---|---|
| `'standard'` | Default paging with pager bar |
| `'virtual'` | Rows outside the viewport are recycled; renders a fixed window |
| `'infinite'` | Appends rows as the user scrolls down; no upward scroll |

### jQuery

```js
$('#data-grid').dxDataGrid({
    height: 600,
    scrolling: { mode: 'virtual' },
    paging: { pageSize: 50 }       // controls the load batch size for virtual scrolling
});
```

### Angular

```html
<dx-data-grid [height]="600">
    <dxo-data-grid-scrolling mode="virtual"></dxo-data-grid-scrolling>
    <dxo-data-grid-paging [pageSize]="50"></dxo-data-grid-paging>
</dx-data-grid>
```

### Vue

```vue
<DxDataGrid :height="600">
    <DxScrolling mode="virtual" />
    <DxPaging :page-size="50" />
</DxDataGrid>
```

```ts
import { DxDataGrid, DxScrolling, DxPaging } from 'devextreme-vue/data-grid';
```

### React

```tsx
import { DataGrid, Scrolling, Paging } from 'devextreme-react/data-grid';

<DataGrid height={600}>
    <Scrolling mode="virtual" />
    <Paging pageSize={50} />
</DataGrid>
```

---

## Remote Operations with Paging

When using a `CustomStore` for server-side data, set `remoteOperations: true` so the grid passes `skip` and `take` to `LoadOptions`. The server response must include `totalCount` for paging to work.

> **See also**: `devextreme-datasource` skill — `CustomStore` server-side load pattern.

```js
// jQuery
$('#data-grid').dxDataGrid({
    dataSource: myCustomStore,
    remoteOperations: true,
    paging: { pageSize: 20 }
});
```

Expected server response shape:

```json
{ "data": [...], "totalCount": 250 }
```

---

## Disabling Paging (Load All Data)

```js
// jQuery
$('#data-grid').dxDataGrid({
    paging: { enabled: false }
});
```

> Avoid this for large datasets. Use virtual scrolling instead.
