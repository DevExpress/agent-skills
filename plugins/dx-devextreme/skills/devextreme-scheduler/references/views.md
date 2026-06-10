# Views

## View Types

| Type string | Description |
|---|---|
| `'day'` | Single day, hourly time slots |
| `'week'` | 7-day week, hourly time slots |
| `'workWeek'` | Monday–Friday, hourly time slots |
| `'month'` | Full calendar month grid |
| `'timelineDay'` | Horizontal timeline for a single day |
| `'timelineWeek'` | Horizontal timeline for a week |
| `'timelineWorkWeek'` | Horizontal timeline for Mon–Fri |
| `'timelineMonth'` | Horizontal timeline for a month |
| `'agenda'` | List of upcoming appointments |

---

## Configuring Views

The `views` array accepts either strings (default config) or objects (custom config). Mix both:

```js
// jQuery
$('#scheduler').dxScheduler({
    views: [
        'month',              // default config
        {
            type: 'week',
            startDayHour: 8,
            endDayHour: 20,
            cellDuration: 60  // 1-hour slots
        },
        {
            type: 'day',
            startDayHour: 8,
            endDayHour: 20,
            maxAppointmentsPerCell: 3
        }
    ],
    currentView: 'week'
});
```

### Angular

```html
<dx-scheduler currentView="week">
    <dxi-scheduler-view type="month"></dxi-scheduler-view>
    <dxi-scheduler-view
        type="week"
        [startDayHour]="8"
        [endDayHour]="20"
        [cellDuration]="60">
    </dxi-scheduler-view>
    <dxi-scheduler-view
        type="day"
        [startDayHour]="8"
        [endDayHour]="20"
        [maxAppointmentsPerCell]="3">
    </dxi-scheduler-view>
</dx-scheduler>
```

### Vue

```vue
<DxScheduler current-view="week">
    <DxView type="month" />
    <DxView
        type="week"
        :start-day-hour="8"
        :end-day-hour="20"
        :cell-duration="60"
    />
    <DxView
        type="day"
        :start-day-hour="8"
        :end-day-hour="20"
        :max-appointments-per-cell="3"
    />
</DxScheduler>
```

```ts
// Vue — import
import { DxScheduler, DxView } from 'devextreme-vue/scheduler';
```

### React

```tsx
import { Scheduler, View } from 'devextreme-react/scheduler';

<Scheduler defaultCurrentView="week">
    <View type="month" />
    <View type="week" startDayHour={8} endDayHour={20} cellDuration={60} />
    <View type="day"  startDayHour={8} endDayHour={20} maxAppointmentsPerCell={3} />
</Scheduler>
```

---

## Per-View Configuration Options

| Option | Type | Description |
|---|---|---|
| `type` | string | View type (required when using object form) |
| `startDayHour` | number | First visible hour for this view |
| `endDayHour` | number | Last visible hour for this view |
| `cellDuration` | number | Time cell size in minutes |
| `maxAppointmentsPerCell` | number / `'auto'` / `'unlimited'` | Max visible appointments per cell |
| `intervalCount` | number | Number of days/weeks/months shown (e.g., 2-week view) |
| `groupByDate` | boolean | Group resource columns by date (Timeline views) |
| `agendaDuration` | number | Days shown in Agenda view (default: 7) |
| `dateCellTemplate` | template | Customize the date header cells |
| `timeCellTemplate` | template | Customize the time panel cells |
| `resourceCellTemplate` | template | Customize resource header cells |
| `appointmentTemplate` | template | Customize appointments in this view |
| `name` | string | Identifier for this view (used in `currentView`) |

---

## Multi-Day / Multi-Week Views

Use `intervalCount` to span multiple time periods:

```js
// 2-week view
{ type: 'week', intervalCount: 2 }

// 3-day view
{ type: 'day', intervalCount: 3 }
```

---

## Timeline Views

Timeline views display appointments horizontally. Useful for resource scheduling:

```html
<!-- Angular — timeline week with resource grouping -->
<dx-scheduler
    [groups]="['roomId']"
    currentView="timelineWeek">
    <dxi-scheduler-view
        type="timelineWeek"
        [startDayHour]="8"
        [endDayHour]="20">
    </dxi-scheduler-view>
    <dxi-scheduler-resource
        fieldExpr="roomId"
        [dataSource]="rooms">
    </dxi-scheduler-resource>
</dx-scheduler>
```

```tsx
// React
import { Scheduler, View, Resource } from 'devextreme-react/scheduler';

<Scheduler groups={['roomId']} defaultCurrentView="timelineWeek">
    <View type="timelineWeek" startDayHour={8} endDayHour={20} />
    <Resource fieldExpr="roomId" dataSource={rooms} />
</Scheduler>
```

---

## Agenda View

The Agenda view lists upcoming appointments as a scrollable list. Configure the number of days shown:

```js
// jQuery — show 14 days in Agenda
$('#scheduler').dxScheduler({
    views: [{ type: 'agenda', agendaDuration: 14 }],
    currentView: 'agenda'
});
```

```html
<!-- Angular -->
<dx-scheduler currentView="agenda">
    <dxi-scheduler-view type="agenda" [agendaDuration]="14"></dxi-scheduler-view>
</dx-scheduler>
```

> Agenda view does not support resource grouping (`groups`).

---

## Changing Views Programmatically

```ts
// jQuery — get instance and switch view
const scheduler = $('#scheduler').dxScheduler('instance');
scheduler.option('currentView', 'month');
scheduler.option('currentDate', new Date(2026, 5, 1));
```

```ts
// Angular — ViewChild
import { ViewChild } from '@angular/core';
import { DxSchedulerComponent } from 'devextreme-angular';

@ViewChild(DxSchedulerComponent) schedulerRef!: DxSchedulerComponent;

switchView(): void {
    this.schedulerRef.instance.option('currentView', 'month');
}
```

```ts
// Vue — template ref
const schedulerRef = ref();
schedulerRef.value.instance.option('currentView', 'month');
```

```tsx
// React — onInitialized
import Scheduler from 'devextreme/ui/scheduler';
import { useRef, useCallback } from 'react';

function App() {
    const schedulerInstance = useRef<Scheduler | null>(null);
    const handleInitialized = useCallback((e) => { schedulerInstance.current = e.component; }, []);

    function switchToMonth() {
        schedulerInstance.current?.option('currentView', 'month');
    }

    return <Scheduler onInitialized={handleInitialized} />;
}
```
