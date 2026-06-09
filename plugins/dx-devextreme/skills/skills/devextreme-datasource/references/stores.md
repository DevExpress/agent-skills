# ArrayStore, LocalStore, and ODataStore

## ArrayStore

Use `ArrayStore` when your data lives in a JavaScript array in memory. It supports full CRUD, key lookup, and change tracking.

### Basic usage

```ts
import ArrayStore from 'devextreme/data/array_store';

const store = new ArrayStore({
    key: 'id',
    data: [
        { id: 1, name: 'Alice', department: 'IT' },
        { id: 2, name: 'Bob',   department: 'HR' }
    ]
});
```

### Wrap in DataSource for paging/sorting

```ts
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';

const dataSource = new DataSource({
    store: new ArrayStore({
        key: 'id',
        data: employees
    }),
    sort: 'name',
    pageSize: 20
});
```

### Shorthand — store config object

```ts
const dataSource = new DataSource({
    store: {
        type: 'array',
        key: 'id',
        data: employees
    },
    sort: 'name'
});
```

### Push changes (reactive updates)

`ArrayStore.push()` updates the in-memory array and propagates changes to bound components when `reshapeOnPush: true` is set on the `DataSource`.

```ts
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';

const store = new ArrayStore({ key: 'id', data: employees });
const dataSource = new DataSource({ store, reshapeOnPush: true });

// Later — push an update without reloading
store.push([
    { type: 'update', key: 1, data: { name: 'Alice Smith' } },
    { type: 'insert', data: { id: 3, name: 'Carol' } },
    { type: 'remove', key: 2 }
]);
```

Push change types: `'insert'`, `'update'`, `'remove'`.

---

## LocalStore

`LocalStore` persists data in the browser's `localStorage` under a configurable `name` key. The API is identical to `ArrayStore`.

```ts
import LocalStore from 'devextreme/data/local_store';

const store = new LocalStore({
    key: 'id',
    name: 'myAppEmployees',  // localStorage key
    data: employees           // initial seed data (only used if the key does not exist yet)
});
```

> **Caution**: `localStorage` is synchronous and has a ~5 MB limit. Use `LocalStore` only for small, non-sensitive datasets.

---

## ODataStore

Use `ODataStore` to connect directly to an OData v2, v3, or v4 endpoint.

### Minimal setup

```ts
import ODataStore from 'devextreme/data/odata/store';

const store = new ODataStore({
    url: 'https://services.odata.org/V4/Northwind/Northwind.svc/Products',
    key: 'ProductID',
    version: 4
});
```

### With a DataSource for paging/filtering

```ts
import DataSource from 'devextreme/data/data_source';
import ODataStore from 'devextreme/data/odata/store';

const dataSource = new DataSource({
    store: new ODataStore({
        url: 'https://services.odata.org/V4/Northwind/Northwind.svc/Products',
        key: 'ProductID',
        version: 4
    }),
    select: ['ProductID', 'ProductName', 'UnitPrice'],
    filter: ['Discontinued', '=', false]
});
```

### Key ODataStore options

| Option | Type | Description |
|---|---|---|
| `url` | `String` | OData entity collection URL |
| `key` | `String \| Array` | Primary key field(s) |
| `version` | `Number` | OData version: `2`, `3`, or `4` (default `2`) |
| `withCredentials` | `Boolean` | Sends cookies with requests |
| `beforeSend` | `function(request)` | Modify request before it is sent (add auth headers, etc.) |
| `processDatesAsUtc` | `Boolean` | (**v26.1+**, replaces `deserializeDates`) Store time-zone information from OData date fields so that timezone-aware values are handled correctly both client-side and in server requests |
| `deserializeDates` | `Boolean` | **Deprecated in v26.1** — replaced by `processDatesAsUtc`. Auto-convert OData date strings to `Date` objects (default `true`) |
| `errorHandler` | `function(error)` | Handle load/CRUD errors |
| `onLoaded` | `function(result)` | Fires after data loads |

### Adding authentication headers

```ts
const store = new ODataStore({
    url: 'https://api.example.com/odata/Products',
    key: 'ID',
    version: 4,
    beforeSend(request) {
        request.headers['Authorization'] = `Bearer ${getToken()}`;
    }
});
```

### Shorthand — store config object in DataSource

```ts
const dataSource = new DataSource({
    store: {
        type: 'odata',
        url: 'https://services.odata.org/V4/Northwind/Northwind.svc/Products',
        key: 'ProductID',
        version: 4
    }
});
```
