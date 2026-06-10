# Appointments

## Appointment Types

| Type | Description | Key Fields |
|---|---|---|
| One-time | Single occurrence with explicit start/end | `startDate`, `endDate` |
| All-day | Spans the full day (no time) | `allDay: true` (or mapped via `allDayExpr`) |
| Recurring | Repeats per iCalendar RRULE | `recurrenceRule`, optionally `recurrenceException` |

---

## One-Time Appointments

```ts
const appointments = [
    {
        text: 'Team Meeting',
        startDate: new Date('2026-05-05T09:00:00.000Z'),
        endDate:   new Date('2026-05-05T10:00:00.000Z')
    },
    {
        text: 'Lunch',
        startDate: new Date('2026-05-05T12:00:00.000Z'),
        endDate:   new Date('2026-05-05T13:00:00.000Z')
    }
];
```

---

## All-Day Appointments

Set `allDay: true` (or the mapped field) to span the full all-day row:

```ts
{
    text: 'Company Holiday',
    startDate: new Date('2026-05-25T00:00:00.000Z'),
    endDate:   new Date('2026-05-25T00:00:00.000Z'),
    allDay: true
}
```

Control the all-day panel visibility:

```js
$('#scheduler').dxScheduler({ showAllDayPanel: true });
```

---

## Recurring Appointments

Set `recurrenceRule` to an [iCalendar RFC 2445](https://tools.ietf.org/html/rfc2445#section-4.3.10) RRULE string.

### Common RRULE Examples

| Pattern | RRULE |
|---|---|
| Daily | `FREQ=DAILY` |
| Every weekday | `FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR` |
| Weekly on Tuesday and Friday, 10 times | `FREQ=WEEKLY;BYDAY=TU,FR;COUNT=10` |
| Monthly on the 15th | `FREQ=MONTHLY;BYMONTHDAY=15` |
| Yearly | `FREQ=YEARLY` |
| Until a date | `FREQ=DAILY;UNTIL=20261231T000000Z` |
| Every 2 weeks | `FREQ=WEEKLY;INTERVAL=2` |

```ts
{
    text: 'Weekly Standup',
    startDate: new Date('2026-05-05T09:00:00.000Z'),
    endDate:   new Date('2026-05-05T09:30:00.000Z'),
    recurrenceRule: 'FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR'
}
```

### Excluding Occurrences

`recurrenceException` holds a comma-separated list of ISO 8601 UTC datetimes for excluded occurrences:

```ts
{
    text: 'Daily Standup',
    startDate: new Date('2026-05-04T09:00:00.000Z'),
    endDate:   new Date('2026-05-04T09:30:00.000Z'),
    recurrenceRule: 'FREQ=DAILY',
    recurrenceException: '20260507T090000Z,20260508T090000Z' // skip Wed and Thu
}
```

### Editing Recurrence Behavior

Control how the Scheduler handles edits to recurring appointments:

```js
$('#scheduler').dxScheduler({
    recurrenceEditMode: 'dialog'    // 'dialog' | 'occurrence' | 'series'
});
```

| Value | Behavior |
|---|---|
| `'dialog'` | Asks user: edit this occurrence or all? (default) |
| `'occurrence'` | Always edits only this occurrence |
| `'series'` | Always edits the whole series |

---

## Appointment Data Fields (Full Reference)

| Field | Type | Description |
|---|---|---|
| `text` | string | Title (overridden by `textExpr`) |
| `startDate` | Date / string | Start time (ISO 8601 string recommended) |
| `endDate` | Date / string | End time |
| `allDay` | boolean | All-day flag |
| `recurrenceRule` | string | iCalendar RRULE |
| `recurrenceException` | string | Excluded occurrence datetimes (comma-separated ISO UTC) |
| `description` | string | Optional description shown in the edit form |
| `startDateTimeZone` | string | IANA timezone for start date |
| `endDateTimeZone` | string | IANA timezone for end date |
| `disabled` | boolean | Renders appointment as non-editable |
| `hidden` | boolean | Hides appointment from all views |

---

## Getting Occurrences Programmatically

Use `getOccurrences(startDate, endDate, appointments)` to expand recurring appointments into a flat list of occurrences within a date range. Useful for overlap detection or data export.

```ts
// TypeScript — overlap detection before adding
import Scheduler from 'devextreme/ui/scheduler';

function hasOverlap(instance: Scheduler, candidate: { startDate: Date; endDate: Date }): boolean {
    const dayStart = new Date(candidate.startDate);
    dayStart.setHours(0, 0, 0, 0);
    const dayEnd = new Date(dayStart);
    dayEnd.setDate(dayEnd.getDate() + 1);

    const all = instance.getDataSource().items();
    const occurrences = instance.getOccurrences(dayStart, dayEnd, all);
    return occurrences.some(
        (item) => item.startDate < candidate.endDate && candidate.startDate < item.endDate
    );
}
```

```ts
// Export occurrences in a date window
const start = new Date(2026, 4, 1);
const end   = new Date(2026, 4, 31);
const all   = schedulerInstance.getDataSource().items();
const rows  = schedulerInstance.getOccurrences(start, end, all).map((o) => ({
    subject: o.appointmentData.text,
    start: o.startDate.toISOString(),
    end:   o.endDate.toISOString()
}));
```

---

## Customizing Appointment Appearance

Use `appointmentTemplate` for full visual control.

### jQuery

```js
$('#scheduler').dxScheduler({
    appointmentTemplate(data, index, element) {
        element.append(
            $('<div>').addClass('custom-appt').text(data.appointmentData.text)
        );
    }
});
```

### Angular

```html
<dx-scheduler [appointmentTemplate]="'myTemplate'">
    <ng-template #myTemplate let-data>
        <div class="custom-appt">{{ data.appointmentData.text }}</div>
    </ng-template>
</dx-scheduler>
```

### Vue

```vue
<DxScheduler>
    <template #appointment="{ data }">
        <div class="custom-appt">{{ data.appointmentData.text }}</div>
    </template>
</DxScheduler>
```

### React

```tsx
import { Scheduler } from 'devextreme-react/scheduler';

function AppointmentTemplate({ data }: { data: { appointmentData: { text: string } } }) {
    return <div className="custom-appt">{data.appointmentData.text}</div>;
}

<Scheduler appointmentRender={AppointmentTemplate} />
```

> The template receives `{ appointmentData, targetedAppointmentData }`. For recurring appointments, `targetedAppointmentData` contains the specific occurrence dates while `appointmentData` contains the series definition.
