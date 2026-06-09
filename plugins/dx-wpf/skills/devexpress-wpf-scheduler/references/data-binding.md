# Data Binding — DevExpress WPF Scheduler

The Scheduler can run **bound** or **unbound**. **Bound mode** is the simpler, typical choice: set `*Source` properties for each item type and define **mappings** that connect your data class's properties to the Scheduler's internal model — the Scheduler then loads, edits, and saves items for you. **Unbound mode** gives you full control and full responsibility: you own the item collections, the change events, and persistence. Mappings are case-sensitive and required — every bound binding needs a mapping to work.

## When to Use This Reference

Use this when you need to:

- Bind appointments, resources, labels, statuses, time regions
- Define `AppointmentMappings`, `ResourceMappings`, etc.
- Add custom (project-specific) fields via `CustomFieldMapping`
- Convert between your storage format and the Scheduler model (`Mapping.Converter`)
- Implement load-on-demand for huge data sets
- Decide between unbound and bound mode

## Unbound Mode

If you don't bind a data source, the Scheduler maintains items internally. Access them through these collections:

| Item type | Collection |
|---|---|
| Appointment | `SchedulerControl.AppointmentItems` |
| Resource | `SchedulerControl.ResourceItems` |
| Label | `SchedulerControl.LabelItems` |
| Status | `SchedulerControl.StatusItems` |
| Time Region | `SchedulerControl.TimeRegionItems` |

```csharp
scheduler.AppointmentItems.Add(new AppointmentItem {
    Start = DateTime.Today.AddHours(9),
    End = DateTime.Today.AddHours(10),
    Subject = "Meeting",
});
```

**Limitation**: nothing persists when the app closes. Suitable for prototypes / demos / read-only displays, or when you want full manual control over how data is loaded and saved (e.g., custom serialization, non-standard storage).

## Bound Mode

Bind external data through `DataSource` (a property of `SchedulerControl`). It holds all source-collection bindings and mappings.

```xaml
<dxsch:SchedulerControl>
    <dxsch:SchedulerControl.DataSource>
        <dxsch:DataSource
            AppointmentsSource="{Binding Appointments}"
            ResourcesSource="{Binding Resources}"
            AppointmentLabelsSource="{Binding Labels}"
            AppointmentStatusesSource="{Binding Statuses}"
            TimeRegionsSource="{Binding TimeRegions}">
            <!-- mappings here -->
        </dxsch:DataSource>
    </dxsch:SchedulerControl.DataSource>
</dxsch:SchedulerControl>
```

| Property | What it binds |
|---|---|
| `DataSource.AppointmentsSource` | Appointment collection |
| `DataSource.ResourcesSource` | Resource collection (doctors, rooms, equipment, ...) |
| `DataSource.AppointmentLabelsSource` | Custom labels (categories) |
| `DataSource.AppointmentStatusesSource` | Custom statuses (Free / Busy / Tentative / ...) |
| `DataSource.TimeRegionsSource` | Time-region collection (blocked time, holidays, ...) |

### Mixed Mode

You can bind some sources and use `*Items` for others. Common pattern: bind `AppointmentsSource` and `ResourcesSource` to a database, but define `LabelItems` and `StatusItems` inline in XAML.

## Mappings — Connecting Data Fields to Scheduler Model

The Scheduler has a fixed internal model — `AppointmentItem.Start`, `.End`, `.Subject`, etc. Your data classes have arbitrary property names. **Mappings translate between the two.** Without mappings, the Scheduler doesn't know which property holds the start time.

Mappings are required for each binding to work. Define them inside `DataSource`:

```xaml
<dxsch:DataSource.AppointmentMappings>
    <dxsch:AppointmentMappings
        Start="StartTime"
        End="EndTime"
        Subject="Title"/>
</dxsch:DataSource.AppointmentMappings>
```

Property values are **data-class property names** (case-sensitive), not bindings.

## Appointment Mappings

| Mapping | Mandatory | What it maps |
|---|---|---|
| `Start` | **Required** | Appointment start (DateTime) |
| `End` | **Required** | Appointment end (DateTime) |
| `Type` | Required for recurring | `AppointmentType` enum value (Normal / Pattern / Occurrence / DeletedOccurrence / ChangedOccurrence) |
| `RecurrenceInfo` | Required for recurring | Serialized recurrence pattern (XML string) |
| `Subject` | Optional | Caption text |
| `Description` | Optional | Long-form notes |
| `Location` | Optional | Location text |
| `AllDay` | Optional | Boolean — render as all-day banner |
| `Id` | Optional | Unique identifier |
| `ResourceId` | Optional | Single resource ID, OR a collection of IDs / XML for shared-resource mode |
| `LabelId` | Optional | Foreign key into the labels collection |
| `StatusId` | Optional | Foreign key into the statuses collection |
| `Reminder` | Optional | Serialized reminder info |
| `TimeZoneId` | Optional | TZ identifier (`Pacific Standard Time`, etc.) |
| `QueryStart` / `QueryEnd` | Optional | For load-on-demand — narrows server-side queries |

## Resource Mappings

| Mapping | What it maps |
|---|---|
| `Id` | Resource identifier |
| `Caption` | Visible name in the resource header |
| `Brush` | Color (any `Brush`) |
| `BrushName` | Color by name in the `BrushSet` palette |
| `BrushSavingType` | How the brush is persisted (`Brush` / `BrushName` / `BrushAndBrushName`) |
| `Visible` | Toggle resource visibility |

## Appointment Label Mappings

Labels are categories (Important, Personal, Travel, ...) — color-coded.

| Mapping | What it maps |
|---|---|
| `Id` | Label identifier |
| `Caption` | Display name |
| `Brush` / `BrushName` / `BrushSavingType` | Color |

## Appointment Status Mappings

Statuses indicate availability (Free, Busy, Tentative, Out of Office, ...).

| Mapping | What it maps |
|---|---|
| `Id` | Status identifier |
| `Caption` | Display name |
| `Brush` / `BrushName` / `BrushSavingType` | Color |

## Time Region Mappings

Time regions block off time (lunch breaks, holidays, ...).

| Mapping | What it maps |
|---|---|
| `Type` | Region type |
| `Brush` / `BrushName` / `BrushSavingType` | Color |

## Custom Field Mappings — Project-Specific Fields

When your data class has fields the Scheduler doesn't recognize (insurance number, patient priority, etc.), add `CustomFieldMapping`:

```xaml
<dxsch:DataSource.AppointmentMappings>
    <dxsch:AppointmentMappings Start="StartTime" End="EndTime" Subject="PatientName">
        <dxsch:CustomFieldMapping Name="InsuranceNumber" Mapping="InsuranceNumber"/>
        <dxsch:CustomFieldMapping Name="FirstVisit" Mapping="FirstVisit"/>
    </dxsch:AppointmentMappings>
</dxsch:DataSource.AppointmentMappings>
```

- `Name` — key used to retrieve the value later
- `Mapping` — name of the property on the data class

Read custom fields in code:

```csharp
var item = scheduler.AppointmentItems[0];
string insurance = (string)item.CustomFields["InsuranceNumber"];
```

Custom fields are available on all `SourceObjectContainer` descendants (`AppointmentItem`, `ResourceItem`, `AppointmentLabelItem`, `AppointmentStatusItem`).

## Mapping Converters — Custom Storage Formats

When your storage format doesn't match the Scheduler's expected type, use `Mapping.Converter`:

```xaml
<Window.Resources>
    <local:RecurrenceJsonConverter x:Key="recJsonConverter"/>
</Window.Resources>

<dxsch:DataSource.AppointmentMappings>
    <dxsch:AppointmentMappings ...>
        <dxsch:AppointmentMappings.RecurrenceInfo>
            <dxsch:Mapping Name="RecurrencePatternJson" Converter="{StaticResource recJsonConverter}"/>
        </dxsch:AppointmentMappings.RecurrenceInfo>
    </dxsch:AppointmentMappings>
</dxsch:DataSource.AppointmentMappings>
```

Common use case: the data class stores `RecurrenceInfo` as JSON, but the Scheduler expects the XML format. The converter handles the back-and-forth.

For the `RecurrenceInfo` field specifically, you can also use the built-in `AppointmentResourceIdCollectionXmlPersistenceHelper` and similar helpers for serialization.

## Shared Resources — One Appointment, Many Resources

By default, `ResourceId` maps to a single value. For shared appointments (a meeting with two doctors), enable resource sharing:

```xaml
<dxsch:DataSource ResourceSharing="True" ...>
```

The `ResourceId` field then maps to a collection of IDs (or an XML string of IDs). See `AppointmentResourceIdCollectionXmlPersistenceHelper`.

## React to Appointment Changes

The Scheduler raises lifecycle events as appointments are added, removed, edited, or restored. Each action has a cancelable `*ing` pre-event and an `*ed` post-event.

**Persisting changes** — the post-events (`AppointmentAdded`, `AppointmentEdited`, `AppointmentRemoved`, `AppointmentRestored`) all share `AppointmentCRUDEventArgs`, which exposes the work lists `AddToSource`, `UpdateInSource`, and `DeleteFromSource`. Each entry's `SourceObject` is your data-class instance. Wire one handler to all of them:

```csharp
void ProcessChanges(object sender, AppointmentCRUDEventArgs e) {
    foreach (var appt in e.AddToSource.Select(x => (MyAppointment)x.SourceObject))
        _store.Add(appt);
    foreach (var appt in e.UpdateInSource.Select(x => (MyAppointment)x.SourceObject))
        _store.Update(appt);
    foreach (var appt in e.DeleteFromSource.Select(x => (MyAppointment)x.SourceObject))
        _store.Delete(appt);
    _store.Save();
}

scheduler.AppointmentAdded   += ProcessChanges;
scheduler.AppointmentEdited  += ProcessChanges;
scheduler.AppointmentRemoved += ProcessChanges;
```

**Validating / vetoing** — use the cancelable pre-events when you need to approve an operation before it completes. `AppointmentAdding` exposes `e.Appointments` (and `e.CanceledAppointments`), `AppointmentEditing` exposes `e.EditAppointments`, and `AppointmentRemoving` exposes the single `e.Appointment`; set `e.Cancel = true` to block the action.

## CreateSourceObject — Customize New-Appointment Creation

When the user creates a new appointment, the Scheduler needs to instantiate your data class. By default it uses `Activator.CreateInstance` (requires a parameterless constructor). Override:

```csharp
var schedulerDataSource = (DataSource)schedulerControl.DataSource;
schedulerDataSource.CreateSourceObject = type => {
    if (type == typeof(MedicalAppointment))
        return new MedicalAppointment { Notes = "Created by user" };
    return Activator.CreateInstance(type);
};
```

## Load on Demand

For very large data sets, load only the visible time range:

1. Use `QueryStart` / `QueryEnd` mappings to tell the Scheduler which appointments are eligible (server-side filter).
2. Set `DataSource.FetchMode` and handle `DataSource.FetchAppointments` to load the next chunk when the visible interval changes.

See `articles/controls-and-libraries/scheduler/data-binding/load-data-on-demand.md` for the full pattern.

## Common Issues

- **Required mappings missing** — `Start` and `End` must always be mapped. Recurring appointments additionally need `Type` and `RecurrenceInfo`.
- **Mapping names don't match data class properties** — names are **case-sensitive** and must match the data class's public property names exactly.
- **Recurrence doesn't expand** — `RecurrenceInfo` is stored in a non-XML format. Either store XML, or supply a `Mapping.Converter` that converts your format to XML.
- **Changes don't persist** — unbound mode (no `DataSource` bound), or no save logic in the `AppointmentAdded` / `AppointmentEdited` / `AppointmentRemoved` handlers. Wire up persistence explicitly.
- **`BindingList<T>` collection doesn't update the Scheduler** — not supported in some contexts. Prefer `ObservableCollection<T>`.
- **Custom field shows `null`** — `CustomFieldMapping.Mapping` typo, or the property isn't `public`. Verify case and accessibility.
- **Shared-resource appointment shows only one resource** — `DataSource.ResourceSharing="True"` isn't set, or `ResourceId` is mapped to a scalar instead of a collection / XML string.

## Source Material

- `articles/controls-and-libraries/scheduler/data-binding.md` (https://docs.devexpress.com/content/WPF/119226?md=true)
- `articles/controls-and-libraries/scheduler/data-binding/mappings.md` (https://docs.devexpress.com/content/WPF/119493?md=true)
- `articles/controls-and-libraries/scheduler/data-binding/custom-fields.md` (https://docs.devexpress.com/content/WPF/119962?md=true)
- `articles/controls-and-libraries/scheduler/data-binding/mapping-converters.md` (https://docs.devexpress.com/content/WPF/119833?md=true)
- `articles/controls-and-libraries/scheduler/data-binding/load-data-on-demand.md` (https://docs.devexpress.com/content/WPF/402187?md=true)
