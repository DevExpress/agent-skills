# Getting Started

## Minimal Setup

Scheduler requires a container with an explicit height.

### jQuery

```html
<!-- index.html -->
<div id="scheduler"></div>
```

```css
/* index.css */
#scheduler { height: 600px; }
```

```js
// index.js
$(function () {
    $('#scheduler').dxScheduler({
        dataSource: appointments,
        currentDate: new Date(2026, 4, 5),
        currentView: 'week',
        startDayHour: 8,
        endDayHour: 20
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-scheduler
    id="scheduler"
    [dataSource]="appointments"
    [currentDate]="currentDate"
    currentView="week"
    [startDayHour]="8"
    [endDayHour]="20">
</dx-scheduler>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxSchedulerComponent } from 'devextreme-angular/ui/scheduler';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrl: './app.css',
    standalone: true,
    imports: [DxSchedulerComponent]
})
export class AppComponent {
    currentDate = new Date(2026, 4, 5);
    appointments = [ /* your data */ ];
}
```

```css
/* app.css */
#scheduler { height: 600px; }
```

### Vue

```vue
<template>
    <DxScheduler
        id="scheduler"
        :data-source="appointments"
        :current-date="currentDate"
        current-view="week"
        :start-day-hour="8"
        :end-day-hour="20"
    />
</template>

<script setup lang="ts">
import { DxScheduler } from 'devextreme-vue/scheduler';

const currentDate = new Date(2026, 4, 5);
const appointments = [ /* your data */ ];
</script>

<style>
#scheduler { height: 600px; }
</style>
```

### React

```tsx
import { Scheduler } from 'devextreme-react/scheduler';

const appointments = [ /* your data */ ];
const currentDate = new Date(2026, 4, 5);

function App() {
    return (
        <Scheduler
            id="scheduler"
            dataSource={appointments}
            currentDate={currentDate}
            defaultCurrentView="week"
            startDayHour={8}
            endDayHour={20}
            height={600}
        />
    );
}
```

---

## Binding to Data

### Appointment Data Shape (Default Field Names)

If your data objects use these exact field names, no mapping is needed:

```ts
interface Appointment {
    text: string;           // Title displayed on the appointment
    startDate: Date | string; // ISO 8601 string recommended for server data
    endDate: Date | string;
    allDay?: boolean;
    recurrenceRule?: string; // iCalendar RRULE
    recurrenceException?: string;
    description?: string;
    disabled?: boolean;     // Prevent editing this appointment
    hidden?: boolean;       // Hide from view
}
```

### Custom Field Names (`...Expr` Mapping)

If your API uses different field names, map them with `...Expr` options:

```js
// Your data uses: { title, startAt, endAt, isAllDay, rrule }
$('#scheduler').dxScheduler({
    dataSource: appointments,
    textExpr: 'title',
    startDateExpr: 'startAt',
    endDateExpr: 'endAt',
    allDayExpr: 'isAllDay',
    recurrenceRuleExpr: 'rrule'
});
```

```html
<!-- Angular -->
<dx-scheduler
    [dataSource]="appointments"
    textExpr="title"
    startDateExpr="startAt"
    endDateExpr="endAt"
    allDayExpr="isAllDay"
    recurrenceRuleExpr="rrule">
</dx-scheduler>
```

```vue
<!-- Vue -->
<DxScheduler
    :data-source="appointments"
    text-expr="title"
    start-date-expr="startAt"
    end-date-expr="endAt"
    all-day-expr="isAllDay"
    recurrence-rule-expr="rrule"
/>
```

```tsx
// React
<Scheduler
    dataSource={appointments}
    textExpr="title"
    startDateExpr="startAt"
    endDateExpr="endAt"
    allDayExpr="isAllDay"
    recurrenceRuleExpr="rrule"
/>
```

---

## Setting the Current Date

`currentDate` controls which date is centered in the view. Combine with `onCurrentDateChange` (React/Angular/Vue controlled) or let the Scheduler manage state internally with `defaultCurrentDate`.

```tsx
// React — controlled
const [currentDate, setCurrentDate] = useState(new Date(2026, 4, 5));
<Scheduler
    currentDate={currentDate}
    onCurrentDateChange={setCurrentDate}
/>
```

```html
<!-- Angular — two-way binding -->
<dx-scheduler
    [(currentDate)]="currentDate">
</dx-scheduler>
```

```vue
<!-- Vue — v-model equivalent -->
<DxScheduler
    v-model:current-date="currentDate"
/>
```

---

## Time Zone Configuration

```js
// Display all appointments in New York time regardless of browser locale
$('#scheduler').dxScheduler({
    timeZone: 'America/New_York'
});
```

```tsx
// React
<Scheduler timeZone="America/New_York" />
```

For per-appointment time zones, set `startDateTimeZone` / `endDateTimeZone` on individual data objects (or map via `startDateTimeZoneExpr` / `endDateTimeZoneExpr`).

> Use IANA timezone identifiers (e.g., `'Europe/London'`, `'Asia/Tokyo'`). See the [IANA tz database](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones).
>
> Always store date/time values as ISO 8601 strings when data is shared with a server — `new Date()` creates objects in the client's local timezone and will cause off-by-one-hour bugs near DST transitions.
