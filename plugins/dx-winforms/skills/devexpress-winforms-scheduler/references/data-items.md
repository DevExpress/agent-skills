# Data Items: Appointments, Resources, Labels, and Statuses

This reference covers the data model of the WinForms Scheduler: what an `Appointment` is, its type hierarchy, its properties, recurrence, reminders, and the supporting objects — `Resource`, `AppointmentLabel`, and `AppointmentStatus`.

## When to Use This Reference

- Understanding appointment types (`Normal`, `Pattern`, `Occurrence`, `ChangedOccurrence`)
- Configuring recurrence patterns in code
- Working with reminders
- Using labels and statuses to categorize appointments
- Accessing and modifying appointments at runtime (selection, enumeration, CRUD)

## Appointment Types

| `AppointmentType` | Description |
|---|---|
| `Normal` | A single, non-recurring appointment. Most common. |
| `Pattern` | The master record for a recurring series. Its `RecurrenceInfo` defines the pattern. Not displayed directly — generates `Occurrence` instances. |
| `Occurrence` | One instance of a recurring series. Generated automatically from `Pattern`. |
| `ChangedOccurrence` | An `Occurrence` that has been individually edited (different time, subject, etc.) — conceptually an *exception*. |
| `DeletedOccurrence` | An `Occurrence` deleted from a series — retained as a placeholder so the series skips that date. |

## Appointment Properties

| Property | Type | Description |
|---|---|---|
| `Subject` | `string` | Title displayed in the appointment cell |
| `Description` | `string` | Longer text shown in detail views |
| `Location` | `string` | Location text |
| `Start` | `DateTime` | Start date/time |
| `End` | `DateTime` | End date/time |
| `Duration` | `TimeSpan` | Read/write alias for `End - Start` |
| `AllDay` | `bool` | When `true`, the appointment spans the full day header |
| `LabelKey` | `object` | Key identifying the appointment's label (background color group) |
| `StatusKey` | `object` | Key identifying the appointment's status (side strip style) |
| `ResourceId` | `object` | Single resource identifier |
| `ResourceIds` | `AppointmentResourceIdCollection` | Multiple resource identifiers when `ResourceSharing = true` |
| `Type` | `AppointmentType` | See type table above |
| `RecurrencePattern` | `Appointment` | The `Pattern` appointment this occurrence belongs to (null for `Normal`) |
| `RecurrenceIndex` | `int` | Zero-based occurrence index within the series |
| `HasReminder` | `bool` | Whether the appointment has at least one reminder |
| `Reminder` | `Reminder` | The first (or only) reminder |
| `Reminders` | `ReminderCollection` | All reminders |
| `CustomFields` | indexer | Arbitrary custom data by field key |

## Creating Appointments in Code

```csharp
// Simple appointment
Appointment apt = storage.CreateAppointment(
    AppointmentType.Normal,
    DateTime.Today.AddHours(14),
    DateTime.Today.AddHours(15),
    "Client Call");
apt.Location    = "Zoom";
apt.Description = "Quarterly review with ACME Corp.";
apt.LabelKey    = "business";   // key of a defined label
apt.StatusKey   = "busy";       // key of a defined status
storage.Appointments.Add(apt);
```

## Recurrence

Recurring appointments are created as `Pattern` type. The `RecurrenceInfo` object encodes the pattern.

### RecurrenceInfo Properties

| Property | Description |
|---|---|
| `Type` | `RecurrenceType` — `Daily`, `Weekly`, `Monthly`, `Yearly`, `Hourly`, `Minutely` |
| `Start` | Pattern start (must equal `apt.Start`) |
| `Periodicity` | Interval between occurrences (e.g., every 2 weeks) |
| `WeekDays` | `WeekDays` flags — `Monday`, `Tuesday`, … `WeekendDays`, `WorkDays` |
| `DayNumber` | Day-of-month for monthly/yearly patterns |
| `WeekOfMonth` | `First`/`Second`/`Third`/`Fourth`/`Last` for monthly patterns |
| `Month` | Month number for yearly patterns |
| `Range` | `RecurrenceRange.NoEndDate`, `OccurrenceCount`, `EndByDate` |
| `OccurrenceCount` | Used when `Range = OccurrenceCount` |
| `End` | End date when `Range = EndByDate` |

### Daily — every weekday, no end

```csharp
Appointment apt = storage.CreateAppointment(AppointmentType.Pattern,
    DateTime.Today.AddHours(9), DateTime.Today.AddHours(9).AddMinutes(30), "Standup");
apt.RecurrenceInfo.Type     = RecurrenceType.Daily;
apt.RecurrenceInfo.Start    = apt.Start;
apt.RecurrenceInfo.WeekDays = WeekDays.WorkDays;
apt.RecurrenceInfo.Range    = RecurrenceRange.NoEndDate;
storage.Appointments.Add(apt);
```

### Weekly — every Monday and Wednesday, 15 occurrences

```csharp
apt.RecurrenceInfo.Type             = RecurrenceType.Weekly;
apt.RecurrenceInfo.Start            = apt.Start;
apt.RecurrenceInfo.Periodicity      = 1;
apt.RecurrenceInfo.WeekDays         = WeekDays.Monday | WeekDays.Wednesday;
apt.RecurrenceInfo.Range            = RecurrenceRange.OccurrenceCount;
apt.RecurrenceInfo.OccurrenceCount  = 15;
```

### Monthly — first Monday of each month

```csharp
apt.RecurrenceInfo.Type         = RecurrenceType.Monthly;
apt.RecurrenceInfo.Start        = apt.Start;
apt.RecurrenceInfo.WeekDays     = WeekDays.Monday;
apt.RecurrenceInfo.WeekOfMonth  = WeekOfMonth.First;
apt.RecurrenceInfo.Range        = RecurrenceRange.NoEndDate;
```

### Editing a Single Occurrence (Exception)

End-users can edit one occurrence via the UI (Edit → "This appointment only"). In code, get the occurrence and modify it — the scheduler automatically converts it to a `ChangedOccurrence` (conceptually, an *exception*):

```csharp
// Get occurrences visible in the current view
AppointmentBaseCollection visible = schedulerControl1.ActiveView
    .GetAppointments();
Appointment occ = visible.OfType<Appointment>()
    .FirstOrDefault(a => a.Type == AppointmentType.Occurrence
                      && a.Subject == "Standup");
if (occ != null) {
    occ.BeginUpdate();
    occ.Subject = "Standup — cancelled";
    occ.EndUpdate();
}
```

## Reminders

```csharp
Appointment apt = storage.CreateAppointment(AppointmentType.Normal,
    DateTime.Today.AddHours(10), DateTime.Today.AddHours(11), "Meeting");
Reminder reminder = apt.CreateNewReminder();
reminder.TimeBeforeStart = TimeSpan.FromMinutes(15);
apt.Reminders.Add(reminder);
storage.Appointments.Add(apt);
```

`SchedulerDataStorage.ReminderAlert` fires when a reminder is due. Reach the appointment through `e.AlertNotifications[i].ActualAppointment` — there is no direct `e.Appointment`:

```csharp
storage.ReminderAlert += (s, e) => {
    // AlertNotifications is a non-generic collection — index it (foreach var infers object).
    for (int i = 0; i < e.AlertNotifications.Count; i++) {
        Appointment apt = e.AlertNotifications[i].ActualAppointment;
        Console.WriteLine($"Reminder due: {apt.Subject} at {apt.Start}");
    }
};
```

`SchedulerControl.RemindersFormShowing` lets you replace the built-in alert dialog.

## Labels

Labels color the appointment background. The scheduler ships with default labels (None, Important, Business, Personal, etc.); you can add custom ones.

```csharp
// Define a custom label
var lbl = storage.Appointments.Labels.CreateNewLabel("vi", "Very Important");
lbl.SetColor(Color.IndianRed);
storage.Appointments.Labels.Add(lbl);

// Assign to appointment
apt.LabelKey = "vi";
```

## Statuses

Statuses control the colored side strip on the left edge of an appointment cell.

```csharp
// Define a custom status
var status = storage.Appointments.Statuses.CreateNewStatus("vb", "Very Busy");
status.SetBrush(new HatchBrush(HatchStyle.ForwardDiagonal,
    Color.IndianRed, Color.White));
storage.Appointments.Statuses.Add(status);

// Assign to appointment
apt.StatusKey = "vb";
```

## Accessing Appointments at Runtime

```csharp
// All appointments in storage
AppointmentBaseCollection all = schedulerDataStorage1.Appointments.Items;

// Appointments visible in the current view
AppointmentBaseCollection visible = schedulerControl1.ActiveView.GetAppointments();

// Selected appointments
AppointmentBaseCollection selected = schedulerControl1.SelectedAppointments;

// Appointments in a time range
var range = new TimeInterval(DateTime.Today, DateTime.Today.AddDays(7));
AppointmentBaseCollection week = schedulerDataStorage1.GetAppointments(range);
```

## Resources at Runtime

```csharp
// All resources
ResourceBaseCollection resources = schedulerDataStorage1.Resources.Items;

// Resource by ID (the Resources indexer is positional, so use GetResourceById for lookups by ID)
Resource res = schedulerDataStorage1.Resources.GetResourceById(resourceId);
string caption = res.Caption;
```

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| Recurring appointment shows only one occurrence | `Type` mapping missing — all records treated as `Normal` | Add `Mappings.Type` pointing to an `int` field storing `AppointmentType` |
| Exception changes are lost on reload | `Id` and `RecurrenceInfo` mappings missing | Both are required for exception persistence |
| Pattern generates unexpected extra occurrences | `RecurrenceInfo.Start` not set to `apt.Start` | Always set `RecurrenceInfo.Start = apt.Start` before adding the pattern |
| Reminder fires for every occurrence | `ReminderInfo` mapping missing — reminder is stored on the occurrence, not the pattern | Add `Mappings.ReminderInfo` to a `string` column |

## Source Material

- `articles/controls-and-libraries/scheduler/appointments.md` (`xref:1753`)
- `articles/controls-and-libraries/scheduler/appointments/appointment-labels-and-statuses.md` (`xref:1754`)
- `articles/controls-and-libraries/scheduler/appointments/reminders-for-appointments.md` (`xref:1778`)
- `DevExpress.XtraScheduler.Appointment` (`xref:CoreLibraries/DevExpress.XtraScheduler.Appointment`)
- `DevExpress.XtraScheduler.RecurrenceInfo` (`xref:CoreLibraries/DevExpress.XtraScheduler.RecurrenceInfo`)
