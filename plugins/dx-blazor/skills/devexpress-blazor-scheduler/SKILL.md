---
name: devexpress-blazor-scheduler
description: Build and configure the DevExpress Blazor Scheduler (DxScheduler) — an appointment calendar component for Blazor Server, WebAssembly, and Hybrid apps. Use when displaying appointments in Day, Week, Work Week, Month, or Timeline views; binding calendar data with DxSchedulerDataStorage and AppointmentMappings; implementing appointment creation, editing, and deletion; configuring recurring appointments with DxSchedulerRecurrenceInfo; managing resources; setting appointment labels and statuses. Also use when someone mentions DxScheduler, DevExpress Scheduler, Blazor calendar, appointment calendar, DxSchedulerDataStorage, DxSchedulerAppointmentMappings, DxSchedulerWeekView, DxSchedulerDayView, or recurring appointments in Blazor.
compatibility: Requires .NET 8, 9, or 10. NuGet package DevExpress.Blazor from the DevExpress feed (https://nuget.devexpress.com/free/api). A valid DevExpress license is required. Requires interactive render mode (InteractiveServer, InteractiveWebAssembly, or InteractiveAuto).
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: 8493730c9e9a47a009fc307a37c307e157663819
---

# DevExpress Blazor Scheduler

`DxScheduler` is a calendar/appointment component for Blazor. It visualizes time-based data as appointments across Day, Week, Work Week, Month, and Timeline views. Users can create, edit, drag, resize, and delete appointments through built-in UI.

## When to Use This Skill

- Display appointments or events in calendar views
- Bind to a collection of appointment data objects via `DxSchedulerDataStorage`
- Support recurring appointments (daily, weekly, monthly, yearly)
- Manage multiple resources (rooms, employees, etc.)
- Create, edit, or delete appointments through the built-in edit forms
- Show work-time-only views with `ShowWorkTimeOnly`
- Navigate between dates with the scheduler toolbar

## Prerequisites & Installation

### NuGet Package

| Package | Purpose |
|---|---|
| `DevExpress.Blazor` | Scheduler + all standard Blazor UI components |

```bash
# If a local NuGet feed with DevExpress 25.2+ or 26.1+ packages is configured (check: dotnet nuget list source):
dotnet add package DevExpress.Blazor --source <local-feed-name>

# No local feed — add the online DevExpress feed, then install:
dotnet nuget add source https://nuget.devexpress.com/free/api -n DevExpress
dotnet add package DevExpress.Blazor
```

### Setup

1. Register in `Program.cs`:
   ```csharp
   builder.Services.AddDevExpressBlazor();
   ```
2. Apply a theme and add client scripts in `App.razor` inside `<head>`:
   ```razor
   @using DevExpress.Blazor
   @DxResourceManager.RegisterTheme(Themes.Fluent)
   @DxResourceManager.RegisterScripts()
   ```
3. Add namespace to `_Imports.razor`:
   ```razor
   @using DevExpress.Blazor
   ```

## Before You Start — Ask the Developer

1. **Render mode**: Are you using `InteractiveServer`, `InteractiveWebAssembly`, or `InteractiveAuto`?
2. **Data model**: What is your appointment data class? What are the field names for start, end, subject, type, recurrence, etc.?
3. **Initial view**: Which view should be shown by default? (Day, Week, Work Week, Month, Timeline)
4. **Start date**: What date should the scheduler open on?
5. **Features**: Do you need recurring appointments? Resources? Custom labels/statuses?

## Component Overview

`DxScheduler` provides:

- **Data Storage** (`DxSchedulerDataStorage`): Connects the component to appointment data via `AppointmentsSource` and `AppointmentMappings`
- **Appointment Mappings** (`DxSchedulerAppointmentMappings`): Maps your data model fields to scheduler concepts (Id, Start, End, Subject, Type, RecurrenceInfo, etc.)
- **Views**: `DxSchedulerDayView`, `DxSchedulerWeekView`, `DxSchedulerWorkWeekView`, `DxSchedulerMonthView`, `DxSchedulerTimelineView`
- **Recurrence** (`DxSchedulerRecurrenceInfo`): Built-in recurrence rule support (Daily, Weekly, Monthly, Yearly)
- **Resources**: Bind appointment resources via `DxSchedulerResourceMappings` and `DxSchedulerDataStorage`
- **Labels & Statuses**: Configurable labels and status indicators for appointments
- **Appointment Templates** (`HorizontalAppointmentTemplate`, `VerticalAppointmentTemplate` on each view; `AppointmentTooltipTemplate` on `DxScheduler`): Customize how appointments and hover tooltips render
- **Drag & Resize** (`AllowDragAppointment`, `AllowDragAppointmentBetweenResources`, `AllowResizeAppointment`): Users can drag appointments to reschedule them, drag between resource groups, or resize by dragging appointment borders; all three are enabled by default; set to `false` to restrict a specific operation; `AppointmentStartDragging`, `AppointmentStartResizing`, and `AppointmentDraggingBetweenResources` events allow customizing or canceling these operations
- **Toolbar** (`ToolbarItems`): Add, remove, or replace built-in toolbar items (`DxSchedulerDateNavigatorToolbarItem`, `DxSchedulerTodayToolbarItem`, etc.) and inject custom `DxToolbarItem` commands

### Core Entry Point (Razor)

```razor
@rendermode InteractiveServer

<DxScheduler StartDate="@DateTime.Today"
             DataStorage="@DataStorage">
    <DxSchedulerWeekView ShowWorkTimeOnly="true" />
</DxScheduler>

@code {
    DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage {
        AppointmentsSource = new List<AppointmentData>(),
        AppointmentMappings = new DxSchedulerAppointmentMappings {
            // Required mappings:
            Id = nameof(AppointmentData.Id),
            Type = nameof(AppointmentData.AppointmentType),
            Start = nameof(AppointmentData.StartDate),
            End = nameof(AppointmentData.EndDate),
            Subject = nameof(AppointmentData.Caption),
            // Optional mappings (add as needed):
            // AllDay = nameof(AppointmentData.AllDay),
            // Description = nameof(AppointmentData.Description),
            // Location = nameof(AppointmentData.Location),
            // LabelId = nameof(AppointmentData.Label),
            // StatusId = nameof(AppointmentData.Status),
            // RecurrenceInfo = nameof(AppointmentData.Recurrence)
        }
    };
}
```

## Documentation & Navigation Guide

### Getting Started
📄 [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up the scheduler from scratch
- Configure `DxSchedulerDataStorage` and `AppointmentMappings`
- Display your first calendar view

### Data Binding
📄 [references/data-binding.md](references/data-binding.md)

When you need to:
- Connect your appointment data model to the scheduler
- Load appointments from a service or database
- Understand all `AppointmentMappings` properties

### Appointments
📄 [references/appointments.md](references/appointments.md)

When you need to:
- Handle appointment creation, editing, and deletion events
- Configure recurring appointments
- Customize appointment display with labels and statuses
- Use the compact form settings

### Views
📄 [references/views.md](references/views.md)

When you need to:
- Choose between Day, Week, Work Week, Month, and Timeline views
- Configure `ShowWorkTimeOnly`
- Set visible time range and cell duration

### Examples
💻 [examples/quickstart.razor](examples/quickstart.razor) — Multi-view scheduler with appointment CRUD, labels, and statuses  
💻 [examples/resources.razor](examples/resources.razor) — Multi-resource timeline view with `ResourcesSource`, `ResourceMappings`, and `GroupType.Resource`

## Quick Start Example

```razor
@page "/scheduler-demo"
@rendermode InteractiveServer

<DxScheduler StartDate="@DateTime.Today"
             DataStorage="@DataStorage">
    <DxSchedulerDayView />
    <DxSchedulerWeekView ShowWorkTimeOnly="true" />
    <DxSchedulerWorkWeekView />
    <DxSchedulerMonthView />
</DxScheduler>

@code {
    DxSchedulerDataStorage DataStorage;

    protected override void OnInitialized() {
        var appointments = new List<AppointmentData> {
            new AppointmentData {
                Id = 1, AppointmentType = 0,
                StartDate = DateTime.Today.AddHours(9),
                EndDate = DateTime.Today.AddHours(10),
                Caption = "Team Meeting",
                Description = "Weekly sync",
                Label = 2, Status = 0
            },
            new AppointmentData {
                Id = 2, AppointmentType = 0,
                StartDate = DateTime.Today.AddDays(1).AddHours(14),
                EndDate = DateTime.Today.AddDays(1).AddHours(15),
                Caption = "Code Review",
                Label = 3, Status = 0
            },
        };

        DataStorage = new DxSchedulerDataStorage {
            AppointmentsSource = appointments,
            AppointmentMappings = new DxSchedulerAppointmentMappings {
                Id = nameof(AppointmentData.Id),
                Type = nameof(AppointmentData.AppointmentType),
                Start = nameof(AppointmentData.StartDate),
                End = nameof(AppointmentData.EndDate),
                Subject = nameof(AppointmentData.Caption),
                Description = nameof(AppointmentData.Description),
                AllDay = nameof(AppointmentData.AllDay),
                Location = nameof(AppointmentData.Location),
                LabelId = nameof(AppointmentData.Label),
                StatusId = nameof(AppointmentData.Status),
                RecurrenceInfo = nameof(AppointmentData.Recurrence)
            }
        };
    }

    class AppointmentData {
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

## Key Properties

### DxScheduler

| Property | Type | Description |
|---|---|---|
| `StartDate` | `DateTime` | Initial date displayed |
| `DataStorage` | `DxSchedulerDataStorage` | Data source and mapping configuration |
| `ActiveViewType` | `SchedulerViewType` | Currently active view |
| `GroupType` | `SchedulerGroupType` | Group appointments by date or resource |

### DxSchedulerDataStorage

| Property | Type | Description |
|---|---|---|
| `AppointmentsSource` | `IList` | Collection of appointment data objects |
| `AppointmentMappings` | `DxSchedulerAppointmentMappings` | Field mapping configuration |
| `ResourcesSource` | `IList` | Collection of resource objects |
| `ResourceMappings` | `DxSchedulerResourceMappings` | Resource field mappings |

### DxSchedulerAppointmentMappings

| Property | Maps To |
|---|---|
| `Id` | Appointment unique identifier |
| `Type` | Appointment type (0=regular, 1=pattern, 3=all-day pattern) |
| `Start` | Start date/time |
| `End` | End date/time |
| `Subject` | Appointment title |
| `AllDay` | All-day flag (bool) |
| `Description` | Appointment notes |
| `Location` | Location string |
| `LabelId` | Label ID (int?) |
| `StatusId` | Status ID (int) |
| `RecurrenceInfo` | Recurrence XML string |

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| No appointments shown | `AppointmentMappings` fields not matching model | Use `nameof()` for all mapping properties |
| Appointments show wrong time | Time zone mismatch | Ensure `StartDate`/`EndDate` are stored in local or UTC consistently |
| Scheduler doesn't respond to clicks | Static render mode | Add `@rendermode InteractiveServer` to the page |
| Recurring appointments broken | `RecurrenceInfo` mapping missing | Set `RecurrenceInfo = nameof(AppointmentData.Recurrence)` in mappings |
| `MissingMethodException: Constructor on type 'Int32Converter' not found` (WASM) | Blazor linker removes required types during WebAssembly compilation | Set `<BlazorWebAssemblyEnableLinking>false</BlazorWebAssemblyEnableLinking>` in the project file, or add a `LinkerConfig.xml` to preserve required types |
| `"Unhandled exception on the current circuit"` with no detail | `CircuitOptions.DetailedErrors` not set | Add `builder.Services.Configure<CircuitOptions>(o => o.DetailedErrors = true);` in `Program.cs` (development only) |
| `"Component parameter 'ValueChanged' is used two or more times"` compile error | `@bind-Value` and `ValueChanged` used together | Use `@bind-Value="@val"` for two-way binding, or `Value="@val" ValueChanged="@handler"` — never both simultaneously |
| `dx-blazor.js` not found (404) behind a reverse proxy | Reverse proxy strips the app base path | Add `app.UsePathBase("/subpath")` before `app.MapBlazorHub()`, or set `<base href="/subpath/" />` in `App.razor` |
| Static assets return 404 (`dx-blazor.css`, `dx-blazor.js`) | `UseStaticWebAssets()` not called | Add `app.UseStaticWebAssets();` in `Program.cs` before `app.UseStaticFiles()` |
| `"Could not find 'X' in 'window.DxBlazor'"` JavaScript error | Stale browser-cached JS from an older DevExpress version | Hard-refresh the browser (Ctrl+Shift+R), clear site data, or verify all DevExpress NuGet packages are the same version |
| `"Cannot pass the parameter 'X' to component 'Y' with rendermode"` | Non-serializable parameter passed across a render mode boundary | Move the component to a child `.razor` file with its own `@rendermode` directive; pass only serializable parameters |

## Constraints & Rules

0. **Never invent API**: If a property, method, event, or feature is not documented in this skill or its references, do **not** assume it exists. When asked about an unfamiliar API, first try to verify it using the DevExpress documentation MCP (`devexpress_docs_search`) or the local `apidoc/` folder. Only after checking: if confirmed, use the API; if not found, explicitly state that it does not appear to be part of the `DxScheduler` API. Do not warn that a feature "may have been introduced in a recent version" as a way to justify inventing it.
1. **Render mode**: `DxScheduler` requires interactive render mode for appointment creation, editing, and navigation.
2. **AppointmentMappings**: All mapping properties must exactly match your data model field names. Use `nameof()` to avoid spelling errors.
3. **AppointmentType**: Required for recurring appointments. Regular non-recurring = `0`.
4. **NuGet packages**: Use `DevExpress.Blazor`. Match versions across all DevExpress packages.
5. **Build verification**: Run `dotnet build` after changes.

## Using DevExpress Documentation MCP

If `devexpress_docs_search` and `devexpress_docs_get_content` are available:

1. `devexpress_docs_search(technology="Blazor", query="Scheduler recurrence appointments data binding")`
2. `devexpress_docs_get_content(url="https://docs.devexpress.com/Blazor/...")`

Use MCP for: resource grouping configuration, custom appointment tooltips, drag-and-drop event handling, compact form settings, and time zone support.
