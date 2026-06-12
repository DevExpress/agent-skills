# Appointments — Blazor Scheduler

Use this reference for appointment CRUD, recurring appointments, labels, and statuses.

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
| `3` | Changed occurrence |
| `4` | Deleted occurrence |

When creating recurring appointments programmatically, set `AppointmentType = 1` and populate the field mapped to `RecurrenceInfo`.

```csharp
var start = DateTime.Today.AddDays(1).AddHours(14);
var end = DateTime.Today.AddDays(1).AddHours(15);

var recurringAppointment = new AppointmentData {
    Id = 3,
    AppointmentType = 1,
    StartDate = start,
    EndDate = end,
    Caption = "Planning Session",
    Description = "Sprint planning",
    Label = 5,
    Status = 1,
    Recurrence = string.Format(
        System.Globalization.CultureInfo.InvariantCulture,
        "<RecurrenceInfo Start=\"{0}\" End=\"{1}\" WeekDays=\"{2}\" Frequency=\"1\" Range=\"0\" Type=\"1\" Id=\"{3}\" />",
        start,
        end,
        1 << (int)start.DayOfWeek,
        Guid.NewGuid())
};
```

This recurrence rule creates a weekly series on the same day of the week as `start`. Use `Range="0"` for a series with no end date.

Use the following recurrence rule shapes for other common patterns:

| Pattern | Rule Body Example |
|---|---|
| Daily | `Type="0" Frequency="3"` |
| Weekly | `Type="1" Frequency="2" WeekDays="16"` |
| Monthly by day number | `Type="2" Frequency="1" WeekOfMonth="0" DayNumber="30"` |
| Monthly by weekday | `Type="2" Frequency="1" WeekOfMonth="1" WeekDays="2"` |
| Yearly by date | `Type="3" Frequency="1" Month="3" WeekOfMonth="0" DayNumber="5"` |
| Yearly by weekday | `Type="3" Frequency="1" Month="3" WeekOfMonth="1" WeekDays="2"` |

Use the following range shapes to define when a series ends:

| Range | Effect |
|---|---|
| `Range="0"` | No end date |
| `Range="1" OccurrenceCount="N"` | End after `N` occurrences |
| `Range="2" End="..."` | End on a specific date |

Changed and deleted occurrences use an exception rule that contains `Id` and `Index` only:

```csharp
"<RecurrenceInfo Id=\"6de79b21-6b16-4dea-9736-c500058ec858\" Index=\"25\"/>"
```

Use the following value mappings when a recurrence prompt requires exact weekday or week position
control:

| `WeekDays` value | Meaning |
|---|---|
| `1` | Sunday |
| `2` | Monday |
| `4` | Tuesday |
| `8` | Wednesday |
| `16` | Thursday |
| `32` | Friday |
| `64` | Saturday |
| `62` | WorkDays |
| `65` | WeekendDays |
| `127` | EveryDay |

| `WeekOfMonth` value | Meaning |
|---|---|
| `0` | None |
| `1` | First |
| `2` | Second |
| `3` | Third |
| `4` | Fourth |
| `5` | Last |

For monthly and yearly rules, `WeekOfMonth="0"` means the rule uses `DayNumber` instead of
`WeekDays`. `DayNumber` accepts values from `1` through `31`. For yearly rules, `Month` accepts
values from `1` through `12`.

If a project uses recurrence objects instead of direct XML strings, use `FromXml(string)` to load
an XML rule and `ToXml()` to serialize it back to the mapped `Recurrence` field.

If recurring appointments use time zones, map the source field with `DxSchedulerAppointmentMappings.TimeZoneId`.

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
