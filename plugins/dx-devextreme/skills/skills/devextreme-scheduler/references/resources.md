# Resources

## Overview

Resources let you categorize appointments by entities such as rooms, employees, or equipment. Each resource type is defined in the `resources[]` array. Appointments carry the resource identifier as a field value, and the Scheduler colors or groups appointments accordingly.

---

## Defining Resources

### Resource Type Shape

```ts
interface ResourceConfig {
    fieldExpr: string;          // Field in appointment data that holds the resource ID
    dataSource: any[] | Store;  // Resource instances
    label?: string;             // Label shown in the edit form
    allowMultiple?: boolean;    // Allow multiple resources per appointment (default: false)
    useColorAsDefault?: boolean;// Color appointments by this resource (default: false for first, true for first only)
    valueExpr?: string;         // Field in resource items that is the ID (default: 'id')
    displayExpr?: string;       // Field in resource items that is the label (default: 'text')
    colorExpr?: string;         // Field in resource items that holds the color (default: 'color')
}
```

### Resource Instance Shape

```ts
const rooms = [
    { id: 1, text: 'Room 101', color: '#4a90d9' },
    { id: 2, text: 'Room 102', color: '#5cb85c' },
    { id: 3, text: 'Conference Hall', color: '#d9534f' }
];
```

---

## Basic Resource Setup (All Frameworks)

### jQuery

```js
const appointments = [
    { text: 'Team Meeting', startDate: new Date('2026-05-05T09:00:00Z'), endDate: new Date('2026-05-05T10:00:00Z'), roomId: 1 },
    { text: 'Workshop',     startDate: new Date('2026-05-05T11:00:00Z'), endDate: new Date('2026-05-05T12:00:00Z'), roomId: 2 }
];

const rooms = [
    { id: 1, text: 'Room 101', color: '#4a90d9' },
    { id: 2, text: 'Room 102', color: '#5cb85c' }
];

$('#scheduler').dxScheduler({
    dataSource: appointments,
    resources: [{ fieldExpr: 'roomId', dataSource: rooms, label: 'Room' }]
});
```

### Angular

```html
<dx-scheduler [dataSource]="appointments">
    <dxi-scheduler-resource
        fieldExpr="roomId"
        [dataSource]="rooms"
        label="Room">
    </dxi-scheduler-resource>
</dx-scheduler>
```

```ts
rooms = [
    { id: 1, text: 'Room 101', color: '#4a90d9' },
    { id: 2, text: 'Room 102', color: '#5cb85c' }
];
```

### Vue

```vue
<DxScheduler :data-source="appointments">
    <DxResource
        field-expr="roomId"
        :data-source="rooms"
        label="Room"
    />
</DxScheduler>
```

```ts
import { DxScheduler, DxResource } from 'devextreme-vue/scheduler';
```

### React

```tsx
import { Scheduler, Resource } from 'devextreme-react/scheduler';

<Scheduler dataSource={appointments}>
    <Resource fieldExpr="roomId" dataSource={rooms} label="Room" />
</Scheduler>
```

---

## Multiple Resources Per Appointment

Set `allowMultiple: true` and store an array of IDs in the appointment field:

```ts
// Appointment data
{ text: 'Joint Presentation', startDate: ..., endDate: ..., roomId: [1, 2] }
```

```js
resources: [{ fieldExpr: 'roomId', dataSource: rooms, allowMultiple: true }]
```

---

## Multiple Resource Types

Define multiple entries in `resources[]`. The appointment must carry all relevant fields:

```ts
// Appointment with both a room and a teacher
{ text: 'Math Class', startDate: ..., endDate: ..., roomId: 1, teacherId: 2 }
```

```js
resources: [
    { fieldExpr: 'roomId',    dataSource: rooms,    label: 'Room' },
    { fieldExpr: 'teacherId', dataSource: teachers, label: 'Teacher' }
]
```

---

## Grouping Appointments by Resources

Set the `groups[]` array to resource `fieldExpr` names. The Scheduler renders a column (or row in Timeline) for each resource instance.

```js
// jQuery — group by room, then by teacher
$('#scheduler').dxScheduler({
    groups: ['roomId', 'teacherId'],
    resources: [
        { fieldExpr: 'roomId',    dataSource: rooms },
        { fieldExpr: 'teacherId', dataSource: teachers }
    ]
});
```

```html
<!-- Angular -->
<dx-scheduler [groups]="['roomId', 'teacherId']">
    <dxi-scheduler-resource fieldExpr="roomId"    [dataSource]="rooms"></dxi-scheduler-resource>
    <dxi-scheduler-resource fieldExpr="teacherId" [dataSource]="teachers"></dxi-scheduler-resource>
</dx-scheduler>
```

```vue
<!-- Vue -->
<DxScheduler :groups="['roomId', 'teacherId']">
    <DxResource field-expr="roomId"    :data-source="rooms" />
    <DxResource field-expr="teacherId" :data-source="teachers" />
</DxScheduler>
```

```tsx
// React
<Scheduler groups={['roomId', 'teacherId']}>
    <Resource fieldExpr="roomId"    dataSource={rooms} />
    <Resource fieldExpr="teacherId" dataSource={teachers} />
</Scheduler>
```

> **Agenda view does not support grouping.** Grouping works with all other view types.

### `groupByDate`

By default, Timeline views group resource headers before dates (resource outer, date inner). Set `groupByDate: true` on a view to swap this — date outer, resource inner:

```html
<!-- Angular -->
<dxi-scheduler-view type="timelineWeek" [groupByDate]="true"></dxi-scheduler-view>
```

---

## Customizing Resource Headers

Use `resourceCellTemplate` to customize the header cell for each resource group:

### jQuery

```js
$('#scheduler').dxScheduler({
    resourceCellTemplate(data, index, element) {
        element.append($('<div>').addClass('resource-header').text(data.text));
    }
});
```

### Angular

```html
<dx-scheduler [resourceCellTemplate]="'myResourceCell'">
    <ng-template #myResourceCell let-data>
        <div class="resource-header">{{ data.text }}</div>
    </ng-template>
</dx-scheduler>
```

### Vue

```vue
<DxScheduler>
    <template #resource-cell="{ data }">
        <div class="resource-header">{{ data.text }}</div>
    </template>
</DxScheduler>
```

### React

```tsx
function ResourceCell({ data }: { data: { text: string } }) {
    return <div className="resource-header">{data.text}</div>;
}
<Scheduler resourceCellRender={ResourceCell} />
```
