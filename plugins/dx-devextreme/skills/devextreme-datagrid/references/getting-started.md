# Getting Started with DataGrid

## Minimal Setup (all frameworks)

Set `height` on the container when using virtual or infinite scrolling — without it the grid renders all rows at once and virtualization has no effect. For standard paging, height is optional.

### jQuery

```html
<!-- index.html -->
<div id="data-grid"></div>
```

```js
// index.js
$(function() {
    $('#data-grid').dxDataGrid({
        dataSource: employees,
        keyExpr: 'EmployeeID',
        showBorders: true
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-data-grid
    id="data-grid"
    [dataSource]="employees"
    keyExpr="EmployeeID"
    [showBorders]="true">
</dx-data-grid>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';

interface Employee {
    EmployeeID: number;
    FullName: string;
    Position: string;
}

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [DxDataGridComponent],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class AppComponent {
    employees: Employee[] = [
        { EmployeeID: 1, FullName: 'Nancy Davolio', Position: 'Sales Representative' },
        { EmployeeID: 2, FullName: 'Andrew Fuller',  Position: 'VP Sales' }
    ];
}
```


### Vue (Composition API + TypeScript)

```vue
<!-- App.vue -->
<template>
    <DxDataGrid
        id="data-grid"
        :data-source="employees"
        key-expr="EmployeeID"
        :show-borders="true"
    />
</template>

<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { DxDataGrid } from 'devextreme-vue/data-grid';

interface Employee { EmployeeID: number; FullName: string; Position: string; }

const employees: Employee[] = [
    { EmployeeID: 1, FullName: 'Nancy Davolio', Position: 'Sales Representative' },
    { EmployeeID: 2, FullName: 'Andrew Fuller',  Position: 'VP Sales' }
];
</script>
```

### React (TSX + hooks)

```tsx
// App.tsx
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { DataGrid } from 'devextreme-react/data-grid';

interface Employee { EmployeeID: number; FullName: string; Position: string; }

const employees: Employee[] = [
    { EmployeeID: 1, FullName: 'Nancy Davolio', Position: 'Sales Representative' },
    { EmployeeID: 2, FullName: 'Andrew Fuller',  Position: 'VP Sales' }
];

function App() {
    return (
        <DataGrid
            id="data-grid"
            dataSource={employees}
            keyExpr="EmployeeID"
            showBorders={true}
        />
    );
}

export default App;
```

### ASP.NET Core Controls

```cshtml
@(Html.DevExtreme().DataGrid()
    .ID("data-grid")
    .DataSource(employees, "EmployeeID")
    .ShowBorders(true)
)
```

> **Don't pair `.DataSource(array)` with `.KeyExpr(...)`.** Passing a plain array/`IEnumerable` to `.DataSource()` implicitly wraps it in an `ArrayStore`, and `ArrayStore`'s key is set on the store itself, not via the grid's `keyExpr` — a separate, later-applied option. Use the `.DataSource(data, "KeyField")` overload instead; it sets the `ArrayStore` key correctly. Mixing the two produces warning `W1011` ("the keyExpr option is not applied when dataSource is not an array") and editing/selection silently misbehave.

```cshtml
@{
    var employees = new[] {
        new { EmployeeID = 1, FullName = "Nancy Davolio", Position = "Sales Representative" },
        new { EmployeeID = 2, FullName = "Andrew Fuller", Position = "VP Sales" }
    };
}
```

---

## Binding to a Remote Data Source

> **See also**: `devextreme-datasource` skill — for `CustomStore`, `ODataStore`, and `DataSource` configuration.

```ts
// Any framework — create a DataSource with a CustomStore
import DataSource from 'devextreme/data/data_source';
import CustomStore from 'devextreme/data/custom_store';
import type { LoadOptions } from 'devextreme/data';

const dataSource = new DataSource({
    store: new CustomStore({
        key: 'ID',
        load(loadOptions: LoadOptions) {
            const params = new URLSearchParams();
            (['skip', 'take', 'sort', 'filter'] as (keyof LoadOptions)[]).forEach(k => {
                const v = loadOptions[k];
                if (v !== undefined && v !== null && v !== '') params.set(k, JSON.stringify(v));
            });
            return fetch(`/api/employees?${params}`).then(r => r.json()); // { data, totalCount }
        },
        byKey: (key: number) => fetch(`/api/employees/${key}`).then(r => r.json())
    })
});
```

Pass this `dataSource` to the grid and set `remoteOperations: true` so the grid delegates sort/filter/paging to the server:

```js
// jQuery
$('#data-grid').dxDataGrid({ dataSource, keyExpr: 'ID', remoteOperations: true });
```

```html
<!-- Angular -->
<dx-data-grid [dataSource]="dataSource" keyExpr="ID" [remoteOperations]="true"></dx-data-grid>
```

```vue
<!-- Vue -->
<DxDataGrid :data-source="dataSource" key-expr="ID" :remote-operations="true" />
```

```tsx
// React
<DataGrid dataSource={dataSource} keyExpr="ID" remoteOperations={true} />
```

```cshtml
<!-- ASP.NET Core Controls -->
@(Html.DevExtreme().DataGrid()
    .ID("data-grid")
    .DataSource(dataSource)
    .KeyExpr("ID")
    .RemoteOperations(true)
)
```

---

## Key Events

| Event | When to use |
|---|---|
| `onRowClick(e)` | React to a row being clicked; `e.data` has row data |
| `onCellPrepared(e)` | Style individual cells based on data value |
| `onContentReady(e)` | Grid finished rendering (initial load and refreshes) |
| `onInitialized(e)` | Capture grid instance: `e.component` |

```ts
// onCellPrepared — highlight negative values red
onCellPrepared(e) {
    if (e.rowType === 'data' && e.column.dataField === 'balance' && e.value < 0) {
        e.cellElement.style.color = 'red';
    }
}
```
