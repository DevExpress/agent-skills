# Editing

## The `editing` Object

All editing operations are enabled by default. Disable individual operations as needed:

```js
// jQuery
$('#scheduler').dxScheduler({
    editing: {
        allowAdding:       true,
        allowUpdating:     true,
        allowDeleting:     true,
        allowDragging:     true,   // drag to reschedule
        allowResizing:     true,   // resize to change duration
        allowTimeZoneEditing: false // show timezone fields in edit form
    }
});
```

```html
<!-- Angular -->
<dx-scheduler>
    <dxo-scheduler-editing
        [allowAdding]="true"
        [allowUpdating]="true"
        [allowDeleting]="true"
        [allowDragging]="false"
        [allowResizing]="true">
    </dxo-scheduler-editing>
</dx-scheduler>
```

```vue
<!-- Vue -->
<DxScheduler>
    <DxEditing
        :allow-adding="true"
        :allow-updating="true"
        :allow-deleting="true"
        :allow-dragging="false"
        :allow-resizing="true"
    />
</DxScheduler>
```

```tsx
// React
import { Scheduler, Editing } from 'devextreme-react/scheduler';

<Scheduler>
    <Editing
        allowAdding={true}
        allowUpdating={true}
        allowDeleting={true}
        allowDragging={false}
        allowResizing={true}
    />
</Scheduler>
```

### `editing` Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `allowAdding` | boolean | true | Allow creating appointments |
| `allowUpdating` | boolean | true | Allow editing appointments |
| `allowDeleting` | boolean | true | Allow deleting appointments |
| `allowDragging` | boolean | true | Allow drag-and-drop rescheduling |
| `allowResizing` | boolean | true | Allow resize to change duration |
| `allowTimeZoneEditing` | boolean | false | Show time zone fields in the edit form |

---

## CRUD Events

Use the `...ing` events to validate or cancel, and the `...ed` events to sync with a backend.

```ts
// TypeScript — cancellable validation on add
import type { AppointmentAddingEvent } from 'devextreme/ui/scheduler';

function onAppointmentAdding(e: AppointmentAddingEvent): void {
    const { startDate, endDate } = e.appointmentData;
    if ((endDate as Date).getTime() - (startDate as Date).getTime() < 30 * 60 * 1000) {
        e.cancel = true; // block appointments shorter than 30 minutes
    }
}
```

```ts
// Save to backend after add
import type { AppointmentAddedEvent } from 'devextreme/ui/scheduler';

async function onAppointmentAdded(e: AppointmentAddedEvent): Promise<void> {
    await fetch('/api/appointments', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(e.appointmentData)
    });
}
```

### Angular — All CRUD Events

```html
<dx-scheduler
    (onAppointmentAdding)="onAdding($event)"
    (onAppointmentAdded)="onAdded($event)"
    (onAppointmentUpdating)="onUpdating($event)"
    (onAppointmentUpdated)="onUpdated($event)"
    (onAppointmentDeleting)="onDeleting($event)"
    (onAppointmentDeleted)="onDeleted($event)">
</dx-scheduler>
```

```ts
import type { DxSchedulerTypes } from 'devextreme-angular/ui/scheduler';

onAdding(e: DxSchedulerTypes.AppointmentAddingEvent): void {
    // e.cancel = true to block
}
onUpdating(e: DxSchedulerTypes.AppointmentUpdatingEvent): void {
    // e.newData contains the proposed changes
}
```

### Vue — CRUD Events

```vue
<DxScheduler
    @appointment-adding="onAdding"
    @appointment-added="onAdded"
    @appointment-updating="onUpdating"
    @appointment-updated="onUpdated"
    @appointment-deleting="onDeleting"
    @appointment-deleted="onDeleted"
/>
```

### React — CRUD Events

```tsx
<Scheduler
    onAppointmentAdding={onAdding}
    onAppointmentAdded={onAdded}
    onAppointmentUpdating={onUpdating}
    onAppointmentUpdated={onUpdated}
    onAppointmentDeleting={onDeleting}
    onAppointmentDeleted={onDeleted}
/>
```

---

## Customizing the Edit Form (`onAppointmentFormOpening`)

Use this event to add, remove, or modify fields in the popup edit form. The form is a DevExtreme Form component — use the same `items[]` API.

```ts
// Add a custom "Priority" SelectBox field — all frameworks
function onAppointmentFormOpening(e: any): void {
    const form = e.form;
    const items = form.option('items') as any[];

    items.push({
        dataField: 'priority',
        label: { text: 'Priority' },
        editorType: 'dxSelectBox',
        editorOptions: {
            items: ['Low', 'Medium', 'High'],
            value: 'Medium'
        }
    });

    form.option('items', items);
}
```

```html
<!-- Angular -->
<dx-scheduler (onAppointmentFormOpening)="onFormOpening($event)"></dx-scheduler>
```

```vue
<!-- Vue -->
<DxScheduler @appointment-form-opening="onFormOpening" />
```

```tsx
// React
<Scheduler onAppointmentFormOpening={onFormOpening} />
```

### Make a Field Read-Only for Existing Appointments

```ts
function onAppointmentFormOpening(e: any): void {
    if (!e.appointmentData?.text) return; // new appointment — skip
    const form = e.form;
    const items: any[] = form.option('items');

    const textItem = items.find((i) => i.dataField === 'text');
    if (textItem) {
        textItem.editorOptions = { ...textItem.editorOptions, readOnly: true };
        form.option('items', items);
    }
}
```

---

## Disable Editing Per Appointment

Set `disabled: true` on individual data objects to make them non-editable:

```ts
{
    text: 'Public Holiday',
    startDate: new Date('2026-12-25T00:00:00.000Z'),
    endDate:   new Date('2026-12-25T00:00:00.000Z'),
    allDay: true,
    disabled: true  // user cannot drag, resize, or edit this appointment
}
```
