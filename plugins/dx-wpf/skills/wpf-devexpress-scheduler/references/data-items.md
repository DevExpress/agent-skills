# Data Items — Appointments, Resources, Labels, Statuses, Time Regions, Reminders

The Scheduler has six built-in item types. Each represents a real-world concept (a calendar event, a person, a category, a status, a blocked-time window, a notification). Most apps care primarily about **appointments** and **resources**; **labels** and **statuses** are color-coded metadata; **time regions** mark off-limits time; **reminders** are notifications. Recurrence ties appointments to repeating patterns.

## When to Use This Reference

Use this when you need to:

- Understand what each item type represents
- Configure recurring appointments
- Define labels (categories) and statuses (availability)
- Block off time with time regions
- Set reminders
- Pick the right item type for new data

## Item Type Overview

| Item type | What it represents | Class | Imperative collection |
|---|---|---|---|
| Appointment | A scheduled activity (meeting, visit, deadline) | `AppointmentItem` | `SchedulerControl.AppointmentItems` |
| Resource | A person, room, equipment, or anything an appointment is "for" | `ResourceItem` | `SchedulerControl.ResourceItems` |
| Label | Category / tag with a color | `AppointmentLabelItem` | `SchedulerControl.LabelItems` |
| Status | Availability indicator with a color | `AppointmentStatusItem` | `SchedulerControl.StatusItems` |
| Time Region | A time range with special semantics (holiday, maintenance window) | `TimeRegionItem` | `SchedulerControl.TimeRegionItems` |
| Reminder | A notification attached to an appointment | `ReminderItem` | (managed per-appointment via `AppointmentItem.Reminders`) |

## Appointments

The core item type. An appointment has a **subject**, a **time range**, optional **location** / **description**, and many optional metadata fields (label, status, recurrence, reminder).

### `AppointmentItem` Key Properties

| Property | Purpose |
|---|---|
| `Start` | DateTime — start of the activity |
| `End` | DateTime — end of the activity |
| `AllDay` | If true, renders as a banner spanning the whole day |
| `Subject` | Short caption |
| `Location` | Where the activity happens |
| `Description` | Long-form notes |
| `LabelId` | Foreign key to a label (category) |
| `StatusId` | Foreign key to a status (availability) |
| `ResourceId` | Foreign key to a resource (single or collection for shared appointments) |
| `Type` | `AppointmentType.Normal` (default), `Pattern`, `Occurrence`, `DeletedOccurrence`, `ChangedOccurrence` |
| `RecurrenceInfo` | Serialized recurrence pattern (XML) |
| `IsException` | True if this is a `ChangedOccurrence` |
| `TimeZoneId` | Time zone for `Start` / `End` |

### Create an Appointment in Code

```csharp
scheduler.AppointmentItems.Add(new AppointmentItem {
    Start = DateTime.Today.AddHours(14),
    End   = DateTime.Today.AddHours(15),
    Subject = "Team Standup",
    Location = "Conference Room A",
});
```

### Appointment Types

`AppointmentType` enum distinguishes regular vs recurring activities:

| Type | Meaning |
|---|---|
| `Normal` | Single non-recurring event |
| `Pattern` | The "master" of a recurring series — defines the pattern via `RecurrenceInfo` |
| `Occurrence` | A specific occurrence in a recurring series — runtime-generated, not stored |
| `DeletedOccurrence` | An occurrence the user explicitly removed |
| `ChangedOccurrence` | An occurrence the user moved or modified — overrides the pattern's default |

When binding, store `Type` and `RecurrenceInfo` per appointment. Pattern + DeletedOccurrence + ChangedOccurrence rows live in the data source; the Scheduler generates plain Occurrence rows in memory.

### Recurrence

Recurring patterns are defined by **`RecurrenceInfo`** — a serialized XML string. To create one in code, use the `RecurrenceInfo` class and serialize:

```csharp
var info = new DevExpress.XtraScheduler.RecurrenceInfo {
    Type = DevExpress.XtraScheduler.RecurrenceType.Weekly,
    WeekDays = DevExpress.XtraScheduler.WeekDays.Monday |
               DevExpress.XtraScheduler.WeekDays.Wednesday,
    OccurrenceCount = 20,
};

var pattern = new AppointmentItem {
    Start = DateTime.Today.AddHours(9),
    End   = DateTime.Today.AddHours(10),
    Subject = "Recurring Standup",
    Type = DevExpress.XtraScheduler.AppointmentType.Pattern,
    RecurrenceInfo = info.ToXml(),
};
scheduler.AppointmentItems.Add(pattern);
```

Most apps don't construct `RecurrenceInfo` manually — they let the Scheduler's built-in recurrence dialog do it. The XML format is opaque; treat it as a blob.

## Resources

A resource is anyone or anything an appointment is "for" — a doctor, a meeting room, a piece of equipment. Resources let the Scheduler **group** appointments visually (one column per resource).

### `ResourceItem` Key Properties

| Property | Purpose |
|---|---|
| `Id` | Unique identifier — referenced by `AppointmentItem.ResourceId` |
| `Caption` | Visible name in the resource header |
| `Brush` | Color used to paint this resource's column / appointments |
| `BrushName` | Color by name (from `SchedulerControl.BrushSet`) |
| `Visible` | Hide a resource from the view |

### Grouping by Resource

Set `SchedulerControl.GroupType="Resource"` to render one lane per resource. Each appointment renders in its resource's lane.

```xaml
<dxsch:SchedulerControl GroupType="Resource">
    <dxsch:SchedulerControl.ResourceItems>
        <dxsch:ResourceItem Id="1" Caption="Dr. Smith"/>
        <dxsch:ResourceItem Id="2" Caption="Dr. Jones"/>
    </dxsch:SchedulerControl.ResourceItems>
</dxsch:SchedulerControl>
```

### Shared Resources

An appointment can belong to multiple resources (e.g., a meeting with both doctors). Enable on `DataSource`:

```xaml
<dxsch:DataSource ResourceSharing="True" ...>
```

`AppointmentItem.ResourceId` then stores a collection of IDs (or an XML serialization). See [data-binding.md](data-binding.md) for the persistence helper.

## Labels — Color-Coded Categories

Labels are user-defined categories like Personal, Important, Travel, Vacation. Each has a color; appointments take their color from the assigned label (when grouping by None, in modern themes).

### `AppointmentLabelItem` Key Properties

| Property | Purpose |
|---|---|
| `Id` | Identifier |
| `Caption` | Display name (shown in right-click context menu) |
| `Brush` | Color |
| `BrushName` | Color by palette name |

### Default Labels

The Scheduler ships with a default set of labels (Important, Business, Personal, Vacation, ...). Replace by populating `LabelItems`:

```xaml
<dxsch:SchedulerControl>
    <dxsch:SchedulerControl.LabelItems>
        <dxsch:AppointmentLabelItem Id="1" Caption="Patient Visit" Brush="LightSkyBlue"/>
        <dxsch:AppointmentLabelItem Id="2" Caption="Surgery" Brush="LightCoral"/>
        <dxsch:AppointmentLabelItem Id="3" Caption="Follow-up" Brush="LightGreen"/>
    </dxsch:SchedulerControl.LabelItems>
</dxsch:SchedulerControl>
```

Each appointment's `LabelId` points to one of these IDs.

## Statuses — Availability Indicators

Statuses indicate whether the time slot is Free, Busy, Tentative, Out of Office. The status's color is shown as a vertical bar inside the appointment.

### `AppointmentStatusItem` Key Properties

Same structure as labels: `Id`, `Caption`, `Brush` / `BrushName`.

### Default Statuses

The Scheduler ships with the typical Outlook set (Free, Tentative, Busy, Out of Office, Working Elsewhere). Override by populating `StatusItems`.

## Time Regions

A time region blocks off specific times — lunch breaks, holidays, scheduled maintenance, non-working hours. Appointments can still be created in these regions, but the visual cue tells users it's special time.

### `TimeRegionItem` Key Properties

| Property | Purpose |
|---|---|
| `Start` / `End` | Time range |
| `Type` | Blocking style |
| `Brush` / `BrushName` | Color |
| `RecurrenceInfo` | Optional — make the time region recur (e.g., daily lunch break) |

### Static Time Region

```xaml
<dxsch:SchedulerControl.TimeRegionItems>
    <dxsch:TimeRegionItem Start="2026-12-25" End="2026-12-26" Caption="Christmas"/>
</dxsch:SchedulerControl.TimeRegionItems>
```

### Recurring Time Region (Lunch Break Every Day)

```csharp
var lunch = new TimeRegionItem {
    Start = DateTime.Today.AddHours(12),
    End   = DateTime.Today.AddHours(13),
    Caption = "Lunch",
    Type = TimeRegionType.None,
};

var recurrence = new DevExpress.XtraScheduler.RecurrenceInfo {
    Type = DevExpress.XtraScheduler.RecurrenceType.Daily,
};
lunch.RecurrenceInfo = recurrence.ToXml();

scheduler.TimeRegionItems.Add(lunch);
```

## Reminders

Reminders fire notifications before an appointment starts. Stored per-appointment in `AppointmentItem.Reminders` (a collection — an appointment can have multiple reminders).

### Add a Reminder in Code

```csharp
var appointment = new AppointmentItem { /*...*/ };
appointment.Reminders.Add(new ReminderItem {
    TimeBeforeStart = TimeSpan.FromMinutes(15),
});
```

### Bind Reminders

In bound mode, map `AppointmentMappings.Reminder` to a string field that stores serialized reminder data. The Scheduler handles serialization to XML.

### Show Reminder Notifications

The Scheduler shows the reminder UI automatically when the reminder fires. Users can dismiss or snooze. To customize the reminders window, handle `SchedulerControl.RemindersWindowShowing`; to react when a single reminder's alert time elapses, handle `ReminderItem.AlertTimeExpired`:

```csharp
scheduler.RemindersWindowShowing += (s, e) => {
    // e.TriggeredReminders contains the reminders being shown
};
```

## Putting It All Together — Bound Mode

```xaml
<dxsch:SchedulerControl GroupType="Resource">
    <dxsch:SchedulerControl.DataSource>
        <dxsch:DataSource
            AppointmentsSource="{Binding Appointments}"
            ResourcesSource="{Binding Doctors}"
            AppointmentLabelsSource="{Binding Categories}"
            AppointmentStatusesSource="{Binding Statuses}"
            TimeRegionsSource="{Binding TimeOffs}">
            <dxsch:DataSource.AppointmentMappings>
                <dxsch:AppointmentMappings
                    Start="StartTime" End="EndTime"
                    Subject="PatientName" Description="Notes"
                    Location="Location" AllDay="AllDay" Id="Id"
                    Type="Type"
                    ResourceId="DoctorId" LabelId="CategoryId"
                    StatusId="StatusId"
                    RecurrenceInfo="RecurrenceInfo"
                    Reminder="ReminderInfo"/>
            </dxsch:DataSource.AppointmentMappings>
            <dxsch:DataSource.ResourceMappings>
                <dxsch:ResourceMappings Id="Id" Caption="Name" Brush="Color"/>
            </dxsch:DataSource.ResourceMappings>
            <dxsch:DataSource.AppointmentLabelMappings>
                <dxsch:AppointmentLabelMappings Id="Id" Caption="Name" Brush="Color"/>
            </dxsch:DataSource.AppointmentLabelMappings>
            <dxsch:DataSource.AppointmentStatusMappings>
                <dxsch:AppointmentStatusMappings Id="Id" Caption="Name" Brush="Color"/>
            </dxsch:DataSource.AppointmentStatusMappings>
        </dxsch:DataSource>
    </dxsch:SchedulerControl.DataSource>
</dxsch:SchedulerControl>
```

All five item types bound. Each has its own mappings; mappings name the data-class properties.

## Common Issues

- **Appointment doesn't render** — `Start` / `End` weren't mapped or have invalid values. Check the data; both are required.
- **Recurring appointment shows only one instance** — `Type` and `RecurrenceInfo` mappings missing, or `RecurrenceInfo` has invalid XML. Use a `Mapping.Converter` if your storage format isn't XML.
- **Appointment has no color** — `LabelId` is null AND the resource has no `Brush`. Color falls through: label color > resource color > theme default.
- **`ResourceId` foreign key doesn't link** — `ResourceMappings.Id` not set on the data class side, or the IDs don't actually match. Verify the types (int vs Guid).
- **Time regions don't block appointment creation** — by design: time regions are visual cues, not constraints. Use `SchedulerControl.AllowAppointmentCreate` or handle the `AppointmentAdding` event to enforce blocks.
- **Custom default labels don't replace built-ins** — populating `LabelItems` overrides the defaults; verify the IDs you reference from appointments match the IDs in your custom labels.
- **Reminders never fire** — verify `SchedulerControl.AllowReminders` (defaults to true) and that the appointment's start time is in the future when the reminder is added.

## Source Material

- `articles/controls-and-libraries/scheduler/appointments.md` (https://docs.devexpress.com/content/WPF/119211?md=true)
- `articles/controls-and-libraries/scheduler/appointments/appointment.md` (https://docs.devexpress.com/content/WPF/119212?md=true)
- `articles/controls-and-libraries/scheduler/appointments/recurrence.md` (https://docs.devexpress.com/content/WPF/119213?md=true)
- `articles/controls-and-libraries/scheduler/resources.md` (https://docs.devexpress.com/content/WPF/119219?md=true)
- `articles/controls-and-libraries/scheduler/appointments/labels.md` (https://docs.devexpress.com/content/WPF/119214?md=true)
- `articles/controls-and-libraries/scheduler/appointments/statuses.md` (https://docs.devexpress.com/content/WPF/119215?md=true)
- `articles/controls-and-libraries/scheduler/time-regions.md` (https://docs.devexpress.com/content/WPF/401378?md=true)
- `articles/controls-and-libraries/scheduler/reminders.md` (https://docs.devexpress.com/content/WPF/119237?md=true)
