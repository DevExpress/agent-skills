# Data Binding

`SchedulerControl` does not hold data directly — all appointments and resources live in the companion `SchedulerDataStorage` component. The storage can operate in two modes: **bound** (data comes from an external source) and **unbound** (data is added in code).

## When to Use This Reference

- Binding appointments or resources to a `DataTable`, `BindingList<T>`, EF DbSet, or XPO collection
- Configuring field-to-property mappings (`Mappings` API)
- Adding custom fields not built into the scheduler model
- Adding appointments and resources in code (unbound mode)
- Refreshing data when the underlying source changes

## Storage Architecture

`SchedulerControl` stores its data in a `SchedulerDataStorage` linked via `SchedulerControl.DataStorage`. Create the storage and assign it explicitly.

```
SchedulerControl.DataStorage
├── Appointments      (AppointmentDataStorage)
│   ├── DataSource / DataMember
│   ├── Mappings      (AppointmentMappingInfo)
│   ├── CustomFieldMappings
│   ├── Labels        (AppointmentLabelCollection)
│   └── Statuses      (AppointmentStatusCollection)
└── Resources         (ResourceDataStorage)
    ├── DataSource / DataMember
    └── Mappings      (ResourceMappingInfo)
```

> **Legacy note:** `SchedulerStorage` and `AppointmentStorage` are legacy types, replaced since v18.1 by `SchedulerDataStorage` and `AppointmentDataStorage`. Always use the current types.

## Bound Mode — Appointments

> **Assign the whole collection to `DataSource` — do not add items one by one.** When your appointments (or resources) come from an existing collection or property, set `Appointments.DataSource = theCollection` and configure mappings. Do **not** loop over the data calling `CreateAppointment` + `Appointments.Add(...)` — that is unbound-mode code and bypasses binding, so edits won't round-trip to your source and the storage holds a disconnected copy. Per-item `CreateAppointment`/`Add` is only for unbound mode (no data source) or ad-hoc additions.

Assign any `IList`, `IBindingList`, `ITypedList`, `DataTable`, or EF/LINQ/XPO collection to `Appointments.DataSource`, then map data source fields to scheduler properties.

```csharp
// Example: bind to a List<MyEvent>
schedulerDataStorage1.Appointments.DataSource = myEventList;

// Required mappings — Start and End are the only truly required ones
schedulerDataStorage1.Appointments.Mappings.Start = "StartDate";
schedulerDataStorage1.Appointments.Mappings.End   = "EndDate";

// Optional — but strongly recommended
schedulerDataStorage1.Appointments.Mappings.Id          = "Id";
schedulerDataStorage1.Appointments.Mappings.Subject     = "Title";
schedulerDataStorage1.Appointments.Mappings.Description = "Notes";
schedulerDataStorage1.Appointments.Mappings.Location    = "Place";
schedulerDataStorage1.Appointments.Mappings.AllDay      = "IsAllDay";
schedulerDataStorage1.Appointments.Mappings.Status      = "StatusId";
schedulerDataStorage1.Appointments.Mappings.Label       = "LabelId";
schedulerDataStorage1.Appointments.Mappings.ResourceId  = "ResourceId";
schedulerDataStorage1.Appointments.Mappings.RecurrenceInfo = "RecurrenceXml";
schedulerDataStorage1.Appointments.Mappings.Type        = "AppointmentType";
schedulerDataStorage1.Appointments.Mappings.ReminderInfo = "ReminderXml";
```

### Required vs Optional Mappings

| Mapping | Required | Notes |
|---|---|---|
| `Start` | Yes | `DateTime` field |
| `End` | Yes | `DateTime` field; mutually exclusive with `Duration` |
| `Id` | Recommended | Needed for recurring appointments and save-back |
| `Type` | Required for recurrence | `int` — `AppointmentType` value |
| `RecurrenceInfo` | Required for recurrence | `string` — XML serialized by the scheduler |
| `Subject` | Optional | `string` |
| `Description` | Optional | `string` |
| `Location` | Optional | `string` |
| `AllDay` | Optional | `bool` |
| `Label` | Optional | `int` — index into the labels collection |
| `Status` | Optional | `int` — index into the statuses collection |
| `ResourceId` | Optional | Scalar or XML-list depending on `ResourceSharing` |
| `ReminderInfo` | Optional | `string` — XML serialized by the scheduler |

### DataSet / DataMember

When the source is a `DataSet` with multiple tables, specify `DataMember`:

```csharp
schedulerDataStorage1.Appointments.DataSource = myDataSet;
schedulerDataStorage1.Appointments.DataMember = "Appointments";

schedulerDataStorage1.Resources.DataSource = myDataSet;
schedulerDataStorage1.Resources.DataMember = "Resources";
```

## Bound Mode — Resources

```csharp
schedulerDataStorage1.Resources.DataSource = resourceList;
schedulerDataStorage1.Resources.Mappings.Id      = "ResourceId";
schedulerDataStorage1.Resources.Mappings.Caption = "Name";
schedulerDataStorage1.Resources.Mappings.Color   = "ColorArgb";   // int
schedulerDataStorage1.Resources.Mappings.Image   = "Photo";       // byte[]
```

After binding resources, restore the `ResourceId` mapping on appointments so the scheduler can associate appointments with their owners.

## Custom Fields

Map data source fields the scheduler does not know about natively:

```csharp
schedulerDataStorage1.Appointments.CustomFieldMappings.Add(
    new AppointmentCustomFieldMapping("Priority", "PriorityField"));

// Access at runtime
string priority = apt.CustomFields["Priority"]?.ToString();
```

## Unbound Mode (Code)

Whether or not a data source is bound, appointments can always be created in code:

```csharp
// Normal (single, non-recurring) appointment
Appointment apt = schedulerDataStorage1.CreateAppointment(
    AppointmentType.Normal,
    DateTime.Today.AddHours(9),
    DateTime.Today.AddHours(10),
    "Daily Standup");
apt.Location    = "Room 101";
apt.Description = "Daily team sync";
schedulerDataStorage1.Appointments.Add(apt);

// All-day appointment
Appointment allDay = schedulerDataStorage1.CreateAppointment(AppointmentType.Normal);
allDay.AllDay    = true;
allDay.Start     = DateTime.Today;
allDay.End       = DateTime.Today.AddDays(1);
allDay.Subject   = "Company Holiday";
schedulerDataStorage1.Appointments.Add(allDay);

// Recurring appointment (pattern)
Appointment pattern = schedulerDataStorage1.CreateAppointment(
    AppointmentType.Pattern,
    DateTime.Today.AddHours(10),
    DateTime.Today.AddHours(11),
    "Weekly Review");
pattern.RecurrenceInfo.Type         = RecurrenceType.Weekly;
pattern.RecurrenceInfo.Start        = pattern.Start;
pattern.RecurrenceInfo.WeekDays     = WeekDays.Friday;
pattern.RecurrenceInfo.Range        = RecurrenceRange.NoEndDate;
schedulerDataStorage1.Appointments.Add(pattern);
```

## Resources in Code

```csharp
Resource res = schedulerDataStorage1.CreateResource(1);
res.Caption = "Alice";
res.SetColor(Color.LightBlue);   // SetColor extension (DevExpress.XtraScheduler); Resource.Color is obsolete, there is no BackColor
schedulerDataStorage1.Resources.Add(res);
```

## Refreshing Data

The scheduler does not auto-detect external database changes. Call `RefreshData()` explicitly:

```csharp
// Manually triggered
schedulerControl1.RefreshData();

// Via Timer (periodic polling)
private void timer1_Tick(object sender, EventArgs e) {
    if (schedulerControl1.Services.SchedulerState.IsDataRefreshAllowed)
        schedulerControl1.RefreshData();
}
```

`RefreshData` fires the `FetchAppointments` event, which you can handle to load only the visible time range for large datasets.

## Grouping by Resource

```csharp
schedulerControl1.GroupType = SchedulerGroupType.Resource;
// Per-view: how many resources to show simultaneously
schedulerControl1.DayView.ResourcesPerPage = 3;
```

## Mapping Converters

When a source field type does not match the target property type, implement `ISchedulerMappingConverter`:

```csharp
class ColorConverter : ISchedulerMappingConverter {
    public object Convert(object obj, Type targetType, object parameter)
        => obj is string s ? Color.FromName(s) : Color.White;

    public object ConvertBack(object obj, Type targetType, object parameter)
        => obj is Color c ? c.Name : "White";
}

schedulerDataStorage1.Resources.Mappings.ColorConverter = new ColorConverter();
schedulerDataStorage1.Resources.ColorSaving = ColorSavingType.Color;
schedulerDataStorage1.Resources.Mappings.Color = "CategoryName";
schedulerDataStorage1.Resources.Mappings.ColorConversionBehavior =
    MappingConversionBehavior.InPlaceOfMapping;
```

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| No appointments displayed | `ResourceId` mapping points to a field that has no matching resource | Clear the `ResourceId` mapping if resources are not yet configured |
| Recurring appointments missing | `Type` and `RecurrenceInfo` mappings absent | Add both mappings; the `RecurrenceInfo` field stores XML generated by the scheduler |
| Changes not saved back to DB | Storage events not handled | Subscribe to `AppointmentsInserted`, `AppointmentsChanged`, `AppointmentsDeleted` and write back |
| All appointments appear on wrong date | Time zone mismatch | Add `TimeZoneId` mapping; ensure the `Start`/`End` fields are UTC-based |

## Source Material

- `articles/controls-and-libraries/scheduler/data-binding.md` (`xref:8386`)
- `articles/controls-and-libraries/scheduler/data-binding/mappings.md` (`xref:15468`)
- `articles/controls-and-libraries/scheduler/data-binding/data-sources.md` (`xref:3289`)
- `DevExpress.XtraScheduler.AppointmentDataStorage` (`xref:DevExpress.XtraScheduler.AppointmentDataStorage`)
- `DevExpress.XtraScheduler.ResourceDataStorage` (`xref:DevExpress.XtraScheduler.ResourceDataStorage`)
