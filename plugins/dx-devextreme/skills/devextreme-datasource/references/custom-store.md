# CustomStore

Use `CustomStore` when your data comes from any backend not covered by the built-in stores (REST API, GraphQL, etc.).

## Choosing a Processing Mode

| Mode | When to use | `loadMode` value |
|---|---|---|
| **Client-side** | You can load all data at once; DevExtreme handles paging/sorting/filtering locally | `'raw'` |
| **Server-side** | Large datasets; the server handles paging, sorting, and filtering | `'processed'` (default) |

> DataGrid, TreeList, PivotGrid, and Scheduler default to server-side mode — omit `loadMode` for these.
> List, SelectBox, TagBox, DropDownBox, and other editor components default to `'processed'` but should use `'raw'` when loading all data from a simple endpoint.

---

## Client-Side Mode (`loadMode: 'raw'`)

Load all data in one call; DevExtreme handles the rest.

### TypeScript (framework-agnostic)

```ts
import CustomStore from 'devextreme/data/custom_store';

const store = new CustomStore({
    key: 'ID',
    loadMode: 'raw',
    async load() {
        const response = await fetch('https://api.example.com/items');
        if (!response.ok) throw 'Data loading error';
        return response.json();
    }
});
```

### Angular (with HttpClient)

```ts
// app.ts
import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { CustomStore } from 'devextreme-angular/common/data';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxDataGridComponent]
})
export class AppComponent {
    customStore: CustomStore;

    constructor(private http: HttpClient) {
        this.customStore = new CustomStore({
            key: 'ID',
            loadMode: 'raw',
            load: () =>
                lastValueFrom(this.http.get<unknown[]>('https://api.example.com/items'))
                    .catch(() => { throw 'Data loading error'; })
        });
    }
}
```

---

## Server-Side Mode (default)

The component passes a `LoadOptions` object to `load()` describing the current paging, sorting, filtering, and grouping state. The server must apply these and return the data (plus metadata).

### Expected server response shape

```json
{
    "data": [...],
    "totalCount": 100,
    "summary": [...],
    "groupCount": 5
}
```

`totalCount` is required when `requireTotalCount` is `true` (DataGrid enables this by default for paging).

### TypeScript (framework-agnostic)

```ts
import CustomStore from 'devextreme/data/custom_store';
import DataSource from 'devextreme/data/data_source';
import type { LoadOptions } from 'devextreme/data';

function isNotEmpty(value: unknown): boolean {
    return value !== undefined && value !== null && value !== '';
}

const store = new CustomStore({
    key: 'ID',
    async load(loadOptions: LoadOptions) {
        const params = new URLSearchParams();

        (
            [
                'filter', 'group', 'groupSummary', 'parentIds',
                'requireGroupCount', 'requireTotalCount',
                'searchExpr', 'searchOperation', 'searchValue',
                'select', 'sort', 'skip', 'take',
                'totalSummary', 'userData'
            ] as (keyof LoadOptions)[]
        ).forEach((key) => {
            const value = loadOptions[key];
            if (isNotEmpty(value)) {
                params.set(key, JSON.stringify(value));
            }
        });

        const response = await fetch(`https://api.example.com/items?${params}`);
        if (!response.ok) throw 'Data loading error';
        return response.json(); // { data, totalCount }
    },
    // Required for SelectBox, Lookup, DropDownBox with server-side stores
    async byKey(key: number) {
        const response = await fetch(`https://api.example.com/items/${key}`);
        if (!response.ok) throw 'Item loading error';
        return response.json();
    }
});

const dataSource = new DataSource({ store });
```

### Angular (with HttpClient, server-side)

```ts
import { Component } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { CustomStore, DataSource } from 'devextreme-angular/common/data';
import type { LoadOptions } from 'devextreme-angular/common/data';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxDataGridComponent]
})
export class AppComponent {
    dataSource: DataSource;

    constructor(private http: HttpClient) {
        const isNotEmpty = (v: unknown) => v !== undefined && v !== null && v !== '';

        this.dataSource = new DataSource({
            store: new CustomStore({
                key: 'ID',
                load: (loadOptions: LoadOptions) => {
                    let params = new HttpParams();
                    (
                        ['filter','group','groupSummary','requireGroupCount',
                         'requireTotalCount','select','sort','skip','take',
                         'totalSummary','userData'] as (keyof LoadOptions)[]
                    ).forEach((key) => {
                        const value = loadOptions[key];
                        if (isNotEmpty(value)) {
                            params = params.set(key, JSON.stringify(value));
                        }
                    });
                    return lastValueFrom(
                        this.http.get<{ data: unknown[]; totalCount: number }>(
                            'https://api.example.com/items', { params }
                        )
                    ).catch(() => { throw 'Data loading error'; });
                },
                byKey: (key: number) =>
                    lastValueFrom(this.http.get(`https://api.example.com/items/${key}`))
                        .catch(() => { throw 'Item loading error'; })
            })
        });
    }
}
```

---

## Implementing CRUD Operations

Add `insert`, `update`, and `remove` to enable editing (e.g., in DataGrid with `editing.mode`).

```ts
import CustomStore from 'devextreme/data/custom_store';

const BASE = 'https://api.example.com/items';

const store = new CustomStore({
    key: 'ID',
    load(loadOptions) {
        // ... server-side or raw load
    },
    async insert(values) {
        const response = await fetch(BASE, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(values)
        });
        if (!response.ok) throw 'Insertion failed';
        return response.json(); // return the new record (with server-assigned key)
    },
    async update(key, values) {
        const response = await fetch(`${BASE}/${encodeURIComponent(key)}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(values)
        });
        if (!response.ok) throw 'Update failed';
    },
    async remove(key) {
        const response = await fetch(`${BASE}/${encodeURIComponent(key)}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw 'Deletion failed';
    }
});
```

---

## `byKey` — When It Is Required

`byKey` must be implemented in server-side stores when the component needs to display the label of a preselected value:

- **SelectBox**, **Lookup** — display text for an existing `value`
- **AutoComplete**, **DropDownBox** — resolve selected keys
- **DataGrid** with `lookup` columns — display text for key-based columns

```ts
async byKey(key: number) {
    const r = await fetch(`https://api.example.com/items/${key}`);
    if (!r.ok) throw 'Item not found';
    return r.json();
}
```

---

## Error Handling

Throw a string or `Error` inside any store function. DevExtreme catches it and fires `onLoadError` / displays it in the component.

```ts
load(loadOptions) {
    return fetch(url)
        .then(r => {
            if (!r.ok) throw `Server error: ${r.status}`;
            return r.json();
        });
}
```

To handle errors globally on a `DataSource`:

```ts
const dataSource = new DataSource({
    store,
    onLoadError(e) {
        console.error('Load failed:', e.message);
    }
});
```
