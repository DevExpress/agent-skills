# Remote Data

## Overview

By default, Scheduler loads all data upfront and filters client-side. For large datasets, enable server-side date-range filtering:

1. Create a `CustomStore` whose `load` function reads `loadOptions.startDate` and `loadOptions.endDate`.
2. Set `remoteFiltering: true` on the Scheduler.

The Scheduler passes the currently visible date range to `load` whenever the view or date changes — only the relevant appointments are fetched.

---

## CustomStore with Date-Range Filtering

```ts
// TypeScript — works with any framework
import CustomStore from 'devextreme/data/custom_store';
import type { LoadOptions } from 'devextreme/data';

interface Appointment {
    id: number;
    text: string;
    startDate: string;
    endDate: string;
}

const appointmentStore = new CustomStore<Appointment>({
    key: 'id',
    async load(loadOptions: LoadOptions & { startDate?: Date; endDate?: Date }) {
        const params = new URLSearchParams();
        if (loadOptions.startDate) {
            params.set('startDate', loadOptions.startDate.toISOString());
        }
        if (loadOptions.endDate) {
            params.set('endDate', loadOptions.endDate.toISOString());
        }

        const response = await fetch(`/api/appointments?${params}`);
        if (!response.ok) throw new Error('Failed to load appointments');
        const data: Appointment[] = await response.json();
        return { data };
    },
    async insert(values: Appointment) {
        const response = await fetch('/api/appointments', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(values)
        });
        if (!response.ok) throw new Error('Insert failed');
        return response.json();
    },
    async update(key: number, values: Partial<Appointment>) {
        const response = await fetch(`/api/appointments/${key}`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(values)
        });
        if (!response.ok) throw new Error('Update failed');
    },
    async remove(key: number) {
        const response = await fetch(`/api/appointments/${key}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Delete failed');
    }
});
```

---

## Wiring to the Scheduler

### jQuery

```js
$('#scheduler').dxScheduler({
    dataSource: appointmentStore,
    remoteFiltering: true,
    currentDate: new Date(),
    currentView: 'week'
});
```

### Angular

```html
<dx-scheduler
    [dataSource]="appointmentStore"
    [remoteFiltering]="true"
    [currentDate]="currentDate"
    currentView="week">
</dx-scheduler>
```

```ts
import { Component } from '@angular/core';
import { DxSchedulerComponent } from 'devextreme-angular/ui/scheduler';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxSchedulerComponent]
})
export class AppComponent {
    currentDate = new Date();
    appointmentStore = appointmentStore; // from above
}
```

### Vue

```vue
<DxScheduler
    :data-source="appointmentStore"
    :remote-filtering="true"
    :current-date="currentDate"
    current-view="week"
/>
```

### React

```tsx
import { Scheduler } from 'devextreme-react/scheduler';

function App() {
    return (
        <Scheduler
            dataSource={appointmentStore}
            remoteFiltering={true}
            currentDate={new Date()}
            defaultCurrentView="week"
            height={600}
        />
    );
}
```

---

## Expected Server Response Shape

The `load` function must return an object with a `data` field (array of appointments). `totalCount` is not needed for Scheduler:

```json
{
    "data": [
        {
            "id": 1,
            "text": "Team Meeting",
            "startDate": "2026-05-05T09:00:00.000Z",
            "endDate": "2026-05-05T10:00:00.000Z"
        }
    ]
}
```

> You can also return the array directly (without wrapping in `{ data }`), but the object form is preferred for consistency with other DevExtreme components.

---

## Example: ASP.NET Core Endpoint

```csharp
// AppointmentsController.cs
[HttpGet]
public IActionResult GetAppointments([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
{
    var appointments = _db.Appointments
        .Where(a => a.StartDate < endDate && a.EndDate > startDate)
        .Select(a => new {
            a.Id,
            a.Text,
            StartDate = a.StartDate.ToString("o"), // ISO 8601
            EndDate   = a.EndDate.ToString("o")
        })
        .ToList();

    return Ok(new { data = appointments });
}
```

---

## OData Store

For OData services, use `ODataStore`. Scheduler date-range filtering via `remoteFiltering: true` works with OData as well:

```ts
import ODataStore from 'devextreme/data/odata/store';

const oDataStore = new ODataStore({
    url: 'https://my-service.com/odata/Appointments',
    key: 'Id',
    keyType: 'Int32'
});
```

```js
$('#scheduler').dxScheduler({
    dataSource: oDataStore,
    remoteFiltering: true,
    textExpr: 'Subject',
    startDateExpr: 'StartDate',
    endDateExpr: 'EndDate'
});
```

---

## Without Remote Filtering (Client-Side Only)

If your dataset is small enough to load entirely upfront, omit `remoteFiltering`. The Scheduler loads all data once and filters locally:

```js
$('#scheduler').dxScheduler({
    dataSource: 'https://my-api.com/appointments', // URL — loads all records
    // remoteFiltering: false (default)
});
```

---

## When `remoteFiltering` Changes

`startDate` and `endDate` are only injected into `loadOptions` when `remoteFiltering: true`. If you forget to set this flag, `loadOptions.startDate` and `loadOptions.endDate` will be `undefined`.

Check:

```ts
async load(loadOptions) {
    console.log(loadOptions.startDate, loadOptions.endDate);
    // undefined unless remoteFiltering: true is set on the Scheduler
}
```
