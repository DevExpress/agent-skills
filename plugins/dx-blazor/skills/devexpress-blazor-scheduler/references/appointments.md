# Appointments — Blazor Scheduler

When you need to: handle appointment creation, editing, and deletion events; configure recurring appointments; customize labels and statuses.

## Appointment CRUD Events

| Event | Fires When |
|---|---|
| `AppointmentInserting` | User saves a new appointment |
| `AppointmentUpdating` | User saves changes to an existing appointment |
| `AppointmentRemoving` | User deletes an appointment |

```razor
<DxScheduler StartDate="@DateTime.Today"
             DataStorage="@DataStorage"
             AppointmentInserting="OnInsert"
             AppointmentUpdating="OnUpdate"
             AppointmentRemoving="OnRemove">
    <DxSchedulerWeekView />
</DxScheduler>

@code {
    List<AppointmentData> Appointments = new();
    DxSchedulerDataStorage DataStorage;

    protected override void OnInitialized() {
        DataStorage = new DxSchedulerDataStorage {
            AppointmentsSource = Appointments,
            AppointmentMappings = new DxSchedulerAppointmentMappings {
                Id = nameof(AppointmentData.Id),
                Type = nameof(AppointmentData.AppointmentType),
                Start = nameof(AppointmentData.StartDate),
                End = nameof(AppointmentData.EndDate),
                Subject = nameof(AppointmentData.Caption),
                AllDay = nameof(AppointmentData.AllDay),
                Description = nameof(AppointmentData.Description),
                Location = nameof(AppointmentData.Location),
                LabelId = nameof(AppointmentData.Label),
                StatusId = nameof(AppointmentData.Status),
                RecurrenceInfo = nameof(AppointmentData.Recurrence)
            }
        };
    }

    async Task OnInsert(SchedulerAppointmentOperationEventArgs e) {
        var appt = (AppointmentData)e.Appointment.SourceObject;
        appt.Id = Appointments.Count > 0 ? Appointments.Max(a => a.Id) + 1 : 1;
        Appointments.Add(appt);
    }

    async Task OnUpdate(SchedulerAppointmentOperationEventArgs e) {
        // The scheduler updates the source object in-place.
        // Persist changes here if needed (e.g., call db.SaveChangesAsync()).
    }

    async Task OnRemove(SchedulerAppointmentOperationEventArgs e) {
        Appointments.Remove((AppointmentData)e.Appointment.SourceObject);
    }
}
```

## SchedulerAppointmentOperationEventArgs Members

| Member | Type | Description |
|---|---|---|
| `Appointment` | `DxSchedulerAppointmentItem` | The appointment being operated on |
| `Appointment.SourceObject` | `object` | The underlying data object — cast to your type |
| `Cancel` | `bool` | Set to `true` to prevent the operation |

## Recurring Appointments

The Scheduler supports recurrence rules stored as XML in your data model's `Recurrence` field. When the user creates a recurring appointment through the built-in edit form, the recurrence XML is written automatically to `RecurrenceInfo` mapping field.

### Appointment Type Values

| `AppointmentType` value | Meaning |
|---|---|
| `0` | Regular (non-recurring) appointment |
| `1` | Recurring appointment pattern |
| `2` | Occurrence of a recurring pattern |
| `3` | Exception to a recurring pattern |

> When creating appointments programmatically, set `AppointmentType = 0` for regular appointments. The scheduler manages types 1–3 automatically when users define recurrence via the UI.

## Labels

Add named labels with colors to appointments:

```csharp
DataStorage = new DxSchedulerDataStorage {
    // ...
    AppointmentLabels = {
        new DxSchedulerAppointmentLabelItem { Id = 1, Caption = "Important",  Color = System.Drawing.Color.Red },
        new DxSchedulerAppointmentLabelItem { Id = 2, Caption = "Meeting",    Color = System.Drawing.Color.Blue },
        new DxSchedulerAppointmentLabelItem { Id = 3, Caption = "Personal",   Color = System.Drawing.Color.Green },
    }
};
```

Set the label on an appointment: set the `Label` field (mapped via `LabelId`) to the corresponding label `Id`.

## Statuses

Add status indicators (free/busy/tentative):

```csharp
DataStorage = new DxSchedulerDataStorage {
    // ...
    AppointmentStatuses = {
        new DxSchedulerAppointmentStatusItem { Id = 0, Caption = "Free",      Color = System.Drawing.Color.LightGray },
        new DxSchedulerAppointmentStatusItem { Id = 1, Caption = "Busy",      Color = System.Drawing.Color.DodgerBlue },
        new DxSchedulerAppointmentStatusItem { Id = 2, Caption = "Tentative", Color = System.Drawing.Color.Gold },
    }
};
```

## Compact Form Settings

Control the inline popup form behavior:

```razor
<DxScheduler DataStorage="@DataStorage">
    <DxSchedulerWeekView />
    <DxSchedulerCompactFormSettings ShowDescription="true"
                                    ShowLocation="true"
                                    ShowStatus="true"
                                    ShowLabel="true" />
</DxScheduler>
```
