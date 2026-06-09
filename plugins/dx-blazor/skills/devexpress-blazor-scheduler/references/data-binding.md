# Data Binding — Blazor Scheduler

When you need to: connect your appointment data model to the scheduler; load appointments from a service or database; understand all AppointmentMappings properties.

## AppointmentMappings Reference

`DxSchedulerAppointmentMappings` maps your data class field names to scheduler semantics. Set each property to the corresponding field name in your data model (use `nameof()` for type safety).

| Mapping Property | Expected Data Type | Description |
|---|---|---|
| `Id` | any | Unique identifier for the appointment |
| `Type` | `int` | Appointment type: `0`=regular, `1`=recurring pattern, `2`=occurrence, `3`=exception |
| `Start` | `DateTime` | Appointment start |
| `End` | `DateTime` | Appointment end |
| `Subject` | `string` | Appointment title shown in the UI |
| `AllDay` | `bool` | Whether this is an all-day appointment |
| `Description` | `string` | Notes or details |
| `Location` | `string` | Location text |
| `LabelId` | `int?` | Reference to a label item ID |
| `StatusId` | `int` | Reference to a status item ID |
| `RecurrenceInfo` | `string` | Serialized recurrence rule (XML) |
| `ResourceId` | `int?` | Reference to a resource item ID |

## Minimal Appointment Data Model

```csharp
public class AppointmentData {
    public int Id { get; set; }
    public int AppointmentType { get; set; }    // 0 = regular
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Caption { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public bool AllDay { get; set; }
    public int Label { get; set; }              // optional — label color
    public int Status { get; set; }             // optional — busy/free
    public string Recurrence { get; set; }      // optional — recurrence rule XML
}
```

## Full DataStorage Configuration

```razor
@code {
    DxSchedulerDataStorage DataStorage;

    protected override void OnInitialized() {
        DataStorage = new DxSchedulerDataStorage {
            AppointmentsSource = AppointmentService.GetAll(),
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
}
```

## Loading from a Service (async)

```razor
@inject IAppointmentService AppointmentService

<DxScheduler StartDate="@DateTime.Today" DataStorage="@DataStorage">
    <DxSchedulerWeekView />
</DxScheduler>

@code {
    DxSchedulerDataStorage DataStorage;

    protected override async Task OnInitializedAsync() {
        var appointments = await AppointmentService.GetAppointmentsAsync();
        DataStorage = new DxSchedulerDataStorage {
            AppointmentsSource = appointments,
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
}
```

## Resources

To show appointments per resource (e.g., per employee or room):

```csharp
DataStorage = new DxSchedulerDataStorage {
    AppointmentsSource = appointments,
    AppointmentMappings = new DxSchedulerAppointmentMappings {
        // ... standard mappings ...
        ResourceId = nameof(AppointmentData.ResourceId)
    },
    ResourcesSource = new List<ResourceData> {
        new ResourceData { Id = 1, Name = "Alice" },
        new ResourceData { Id = 2, Name = "Bob" }
    },
    ResourceMappings = new DxSchedulerResourceMappings {
        Id = nameof(ResourceData.Id),
        Caption = nameof(ResourceData.Name)
    }
};
```
