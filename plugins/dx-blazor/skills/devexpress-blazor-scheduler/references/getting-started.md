# Getting Started — Blazor Scheduler

When you need to: install the package; configure data storage; display your first calendar view.

## Prerequisites

- .NET 8, 9, or 10
- `DevExpress.Blazor` NuGet package installed from the DevExpress feed
- A valid DevExpress license
- Interactive render mode (`InteractiveServer`, `InteractiveWebAssembly`, or `InteractiveAuto`)

> **Note**: DevExpress Blazor components require .NET 8 or later. .NET Framework is not supported — Blazor itself is a .NET Core/.NET 5+ technology.

## Step 1 — Install NuGet Package

```bash
dotnet add package DevExpress.Blazor --source https://nuget.devexpress.com/free/api
```

## Step 2 — Register Services

In `Program.cs`:

```csharp
builder.Services.AddDevExpressBlazor();
```

## Step 3 — Apply Theme and Scripts

In `App.razor`, inside `<head>`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

## Step 4 — Add Namespace

In `_Imports.razor`:

```razor
@using DevExpress.Blazor
```

## Step 5 — Create a Minimal Scheduler

```razor
@page "/calendar"
@rendermode InteractiveServer

<DxScheduler StartDate="@DateTime.Today"
             DataStorage="@DataStorage">
    <DxSchedulerWeekView ShowWorkTimeOnly="true" />
</DxScheduler>

@code {
    DxSchedulerDataStorage DataStorage;

    protected override void OnInitialized() {
        DataStorage = new DxSchedulerDataStorage {
            AppointmentsSource = new List<Appointment>(),
            AppointmentMappings = new DxSchedulerAppointmentMappings {
                Id = nameof(Appointment.Id),
                Type = nameof(Appointment.AppointmentType),
                Start = nameof(Appointment.StartDate),
                End = nameof(Appointment.EndDate),
                Subject = nameof(Appointment.Caption),
                AllDay = nameof(Appointment.AllDay),
                Description = nameof(Appointment.Description),
                Location = nameof(Appointment.Location),
                LabelId = nameof(Appointment.Label),
                StatusId = nameof(Appointment.Status),
                RecurrenceInfo = nameof(Appointment.Recurrence)
            }
        };
    }

    class Appointment {
        public int Id { get; set; }
        public int AppointmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int Label { get; set; }
        public int Status { get; set; }
        public string Recurrence { get; set; }
    }
}
```

## Interactive Render Mode

The Scheduler requires interactive render mode for:
- Appointment creation, editing, and deletion
- Drag-and-drop and resize
- Date navigation
- View switching

```razor
@rendermode InteractiveServer
```

## Data Storage Architecture

```
DxScheduler
  └── DataStorage (DxSchedulerDataStorage)
        ├── AppointmentsSource  ← your data collection
        └── AppointmentMappings ← maps your field names to scheduler concepts
              ├── Id
              ├── Type
              ├── Start / End
              ├── Subject
              ├── AllDay
              ├── Description / Location
              ├── LabelId / StatusId
              └── RecurrenceInfo
```

The Scheduler does **not** use direct property binding like `DxGrid`. All field access goes through the string-based mapping in `DxSchedulerAppointmentMappings`.
