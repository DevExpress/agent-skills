# DataSource Configuration Options

`DataSource` sits between a component and a `Store`. It applies paging, sorting, filtering, grouping, and transformation before the data reaches the UI.

## When to Use DataSource Directly

- You need to control `pageSize` or turn paging off (`paginate: false`)
- You need a default sort or filter applied before the component renders
- You need to transform data (`map`, `postProcess`)
- You need reactive push updates (`reshapeOnPush`)
- You want to share one data pipeline across multiple components

For simple cases, passing an array or a Store directly to the component is sufficient — DevExtreme wraps it automatically.

---

## Full Constructor Pattern

```ts
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';

const dataSource = new DataSource({
    store: new ArrayStore({ key: 'id', data: employees }),
    sort: [{ selector: 'lastName', desc: false }],
    filter: ['department', '=', 'IT'],
    pageSize: 25,
    paginate: true,
    requireTotalCount: true,
    reshapeOnPush: true
});
```

---

## Paging

```ts
// Enable paging with a custom page size
const ds = new DataSource({ store, paginate: true, pageSize: 50 });

// Disable paging (load all data)
const ds = new DataSource({ store, paginate: false });
```

Components like DataGrid manage pagination UI themselves. For List or SelectBox with server-side paging, set `paginate: true` and ensure the store's `load` function handles `skip`/`take` from `LoadOptions`.

---

## Sorting

Pass a field name string or an array of sort descriptors.

```ts
// Single field, ascending
const ds = new DataSource({ store, sort: 'lastName' });

// Multiple fields
const ds = new DataSource({
    store,
    sort: [
        { selector: 'department', desc: false },
        { selector: 'lastName',   desc: false }
    ]
});
```

---

## Filtering

Filter expressions use the `[field, operator, value]` array format.

```ts
// Simple filter
const ds = new DataSource({ store, filter: ['active', '=', true] });

// Compound filter (AND)
const ds = new DataSource({
    store,
    filter: [
        ['department', '=', 'IT'],
        'and',
        ['active', '=', true]
    ]
});

// Update filter at runtime and reload
dataSource.filter(['department', '=', 'HR']);
dataSource.load();
```

**Common operators**: `'='`, `'<>'`, `'<'`, `'<='`, `'>'`, `'>='`, `'contains'`, `'notcontains'`, `'startswith'`, `'endswith'`.

---

## Search (simple text search)

```ts
const ds = new DataSource({
    store,
    searchExpr: ['firstName', 'lastName'],
    searchValue: 'ali',
    searchOperation: 'contains' // default
});

// Update at runtime
ds.searchValue('bob');
ds.load();
```

---

## Grouping

```ts
const ds = new DataSource({
    store,
    group: 'department'
});

// Multiple levels
const ds = new DataSource({
    store,
    group: ['department', 'role']
});
```

---

## Field Projection (`select`)

Load only the fields you need — reduces payload size with server-side stores.

```ts
const ds = new DataSource({
    store,
    select: ['id', 'firstName', 'lastName', 'email']
});
```

---

## Data Transformation

### `map` — transform each item individually

```ts
const ds = new DataSource({
    store,
    map(item) {
        return {
            ...item,
            fullName: `${item.firstName} ${item.lastName}`
        };
    }
});
```

### `postProcess` — transform the entire loaded dataset

```ts
const ds = new DataSource({
    store,
    postProcess(data: unknown[]) {
        return data.filter(item => (item as any).active);
    }
});
```

---

## Reactive Push Updates

Use `reshapeOnPush: true` together with `ArrayStore.push()` to update the UI without a full reload.

```ts
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';

const store = new ArrayStore({ key: 'id', data: [] });
const dataSource = new DataSource({ store, reshapeOnPush: true });

// Add a new row — the bound component updates immediately
store.push([{ type: 'insert', data: { id: 99, name: 'New Item' } }]);

// Update a row
store.push([{ type: 'update', key: 1, data: { name: 'Updated Name' } }]);

// Remove a row
store.push([{ type: 'remove', key: 2 }]);
```

---

## Runtime DataSource Control

```ts
// Reload data (re-runs load(), resets to page 1)
dataSource.reload();

// Navigate pages manually
dataSource.pageIndex(2);
dataSource.load();

// Get current items (after last load)
const items = dataSource.items();

// Get total count (requires requireTotalCount: true)
const total = dataSource.totalCount();

// Check if data is currently loading
const loading = dataSource.isLoading();
```

---

## Events

```ts
const ds = new DataSource({
    store,
    onChanged() {
        // Fires after data is loaded or changed
    },
    onLoadingChanged(isLoading: boolean) {
        // Fires when loading state toggles
    },
    onLoadError(e: { message: string }) {
        console.error('Load error:', e.message);
    }
});
```
