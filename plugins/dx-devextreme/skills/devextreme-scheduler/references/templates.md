# Scheduler Templates

## When to Use This Reference

Read this file before generating appointment, tooltip, date-cell, time-cell, data-cell, or resource-cell templates. Scheduler templates do not share one universal context object, and some are unavailable in particular views.

## Choose the Correct Template

| Template | Purpose | Important context |
|---|---|---|
| `appointmentTemplate` | Appointment displayed in the timetable | `appointmentData` and `targetedAppointmentData` |
| `appointmentTooltipTemplate` | Appointment tooltip content | Tooltip-specific appointment context |
| `dropDownAppointmentTemplate` | Appointment shown in a collector/drop-down | Appointment-specific context |
| `dataCellTemplate` | Main timetable cells | `startDate`, `endDate`, `allDay`, `groups`, `groupIndex`, `text` |
| `dateCellTemplate` | Date-scale cells | `date`, grouping fields, and documented text where applicable |
| `timeCellTemplate` | Time-scale cells | `date`, `text`, and grouping fields |
| `resourceCellTemplate` | Resource headers | Resource item fields plus grouping context |

Do not use an appointment field such as `text` as a free variable when the contract places it under `appointmentData`.

## View Limitations

- `dataCellTemplate` does not apply to Agenda view.
- `dateCellTemplate` is ignored by views without a date scale.
- `timeCellTemplate` is ignored by views without a time scale.
- A view-level template can override the Scheduler-level template for that view.

Verify the selected view's current API before combining templates with custom views.

## Appointment Template Across Frameworks

### jQuery

```js
$('#scheduler').dxScheduler({
    dataSource: appointments,
    appointmentTemplate(data) {
        return $('<div>').addClass('appointment').text(data.appointmentData.text);
    }
});
```

### Angular

```html
<dx-scheduler [dataSource]="appointments" appointmentTemplate="appointment">
    <div *dxTemplate="let info of 'appointment'" class="appointment">
        {{ info.appointmentData.text }}
    </div>
</dx-scheduler>
```

### Vue

```vue
<DxScheduler :data-source="appointments" appointment-template="appointment">
    <template #appointment="{ data: info }">
        <div class="appointment">{{ info.appointmentData.text }}</div>
    </template>
</DxScheduler>
```

### React

```tsx
function renderAppointment(info: SchedulerTypes.AppointmentTemplateData) {
    return <div className="appointment">{info.appointmentData.text}</div>;
}

<Scheduler dataSource={appointments} appointmentRender={renderAppointment} />
```

### ASP.NET Core Controls

```cshtml
@(Html.DevExtreme().Scheduler()
    .ID("scheduler")
    .DataSource(Model.Appointments)
    .AppointmentTemplate(new TemplateName("appointment"))
)

@using (Html.DevExtreme().NamedTemplate("appointment")) {
    <div class="appointment"><%- appointmentData.text %></div>
}
```

Scheduler's built-in appointment fields use names such as `text`, `startDate`, and `endDate`. Wrapper-provided context names such as `appointmentData` retain their documented casing; custom fields follow the serializer/model configuration.

## Cell Context Example

For a data cell, use the cell contract directly:

```vue
<DxScheduler :data-source="appointments" data-cell-template="dataCell">
    <template #dataCell="{ data: cell }">
        <span :class="{ 'all-day-cell': cell.allDay }">{{ cell.text }}</span>
    </template>
</DxScheduler>
```

Do not use `appointmentData` in a data-cell template; it renders a time slot, not an appointment.

## Syntax Boundaries

| Framework | Template binding | Context access |
|---|---|---|
| jQuery | Callback property | Callback argument |
| Angular | Named `*dxTemplate` | The `let` variable |
| Vue | Named slot | Slot `data` |
| React | `...Render` or `...Component` | Function/component argument |
| ASP.NET Core | Razor block or `TemplateName` | ERB-style `<%- ... %>` expressions |

Never copy expressions between frameworks. In particular, `${...}` is not ASP.NET Core client-template syntax, and appointment object properties are not automatically free variables.

## Official References

- [Scheduler configuration and cell template contracts](https://js.devexpress.com/Vue/Documentation/ApiReference/UI_Components/dxScheduler/Configuration/)
- [Scheduler appointment template data](https://js.devexpress.com/React/Documentation/ApiReference/UI_Components/dxScheduler/Types/AppointmentTemplateData/)
- [ASP.NET Core template concepts](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
