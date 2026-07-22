# Getting Started — DevExpress WPF Scheduler

This guide walks through setting up `SchedulerControl` in a .NET 8+ WPF project. The end result: a calendar that shows medical appointments grouped by doctor, bound to a ViewModel.

## System Requirements

- .NET 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install the NuGet Package

```bash
dotnet add package DevExpress.Wpf.Scheduling
```

This brings `DevExpress.Wpf.Core` (MVVM helpers) and the chart core libraries transitively. Optional add-ons:

```bash
dotnet add package DevExpress.Wpf.Ribbon        # if you want the auto-generated Ribbon UI
dotnet add package DevExpress.Wpf.Printing      # if you need print preview / export
```

All DevExpress packages in a project must share the same version.

## Step 2: Configure the Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

## Step 3: Build the Data Model

Define the appointment and resource classes that your scheduler will display:

```csharp
public class MedicalAppointment {
    public int Id { get; set; }
    public bool AllDay { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string PatientName { get; set; } = "";
    public string Notes { get; set; } = "";
    public int StatusId { get; set; }
    public int CategoryId { get; set; }   // Label
    public int Type { get; set; }         // AppointmentType: Normal, Pattern, Occurrence, etc.
    public string Location { get; set; } = "";
    public string RecurrenceInfo { get; set; } = "";
    public string ReminderInfo { get; set; } = "";
    public int? DoctorId { get; set; }    // Resource ID
}

public class Doctor {
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
```

The Scheduler doesn't dictate the class names — it maps your fields to its model through `AppointmentMappings` (see [data-binding.md](data-binding.md)).

## Step 4: Build the ViewModel

```csharp
using System.Collections.ObjectModel;
using DevExpress.Mvvm.DataAnnotations;

namespace MyApp.ViewModels;

[POCOViewModel]
public class MainViewModel {
    public ObservableCollection<MedicalAppointment> Appointments { get; }
    public ObservableCollection<Doctor> Doctors { get; }

    public MainViewModel() {
        Doctors = new ObservableCollection<Doctor> {
            new() { Id = 1, Name = "Dr. Smith" },
            new() { Id = 2, Name = "Dr. Jones" },
        };
        Appointments = new ObservableCollection<MedicalAppointment> {
            new() {
                Id = 1, StartTime = DateTime.Today.AddHours(9), EndTime = DateTime.Today.AddHours(10),
                PatientName = "Alice Brown", DoctorId = 1, Location = "Room 101"
            },
            new() {
                Id = 2, StartTime = DateTime.Today.AddHours(11), EndTime = DateTime.Today.AddHours(12),
                PatientName = "Bob White", DoctorId = 2, Location = "Room 202"
            },
        };
    }
}
```

`[POCOViewModel]` is the DevExpress MVVM attribute — it lets `dxmvvm:ViewModelSource` create a wired-up instance.

## Step 5: Place the SchedulerControl

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxsch="http://schemas.devexpress.com/winfx/2008/xaml/scheduling"
        xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="Calendar" Height="600" Width="900"
        DataContext="{dxmvvm:ViewModelSource vm:MainViewModel}">
    <dxsch:SchedulerControl GroupType="Resource" FirstDayOfWeek="Monday">
        <dxsch:SchedulerControl.DataSource>
            <dxsch:DataSource ResourcesSource="{Binding Doctors}"
                              AppointmentsSource="{Binding Appointments}">
                <dxsch:DataSource.ResourceMappings>
                    <dxsch:ResourceMappings Id="Id" Caption="Name"/>
                </dxsch:DataSource.ResourceMappings>
                <dxsch:DataSource.AppointmentMappings>
                    <dxsch:AppointmentMappings
                        Type="Type"
                        Start="StartTime"
                        End="EndTime"
                        Subject="PatientName"
                        Location="Location"
                        Description="Notes"
                        AllDay="AllDay"
                        Id="Id"
                        ResourceId="DoctorId"
                        StatusId="StatusId"
                        LabelId="CategoryId"
                        RecurrenceInfo="RecurrenceInfo"
                        Reminder="ReminderInfo"/>
                </dxsch:DataSource.AppointmentMappings>
            </dxsch:DataSource>
        </dxsch:SchedulerControl.DataSource>
    </dxsch:SchedulerControl>
</Window>
```

| Property | Purpose |
|---|---|
| `GroupType="Resource"` | Display one column per doctor |
| `FirstDayOfWeek="Monday"` | Affects Week / Work Week views |
| `ResourcesSource` | The doctors collection |
| `AppointmentsSource` | The appointments collection |
| `ResourceMappings.Id` / `.Caption` | Map `Doctor.Id` and `Doctor.Name` to the resource model |
| `AppointmentMappings.Start` / `.End` | Required — map start/end times |
| `AppointmentMappings.Subject` | The appointment caption |
| `AppointmentMappings.ResourceId` | Link an appointment to a doctor |
| `AppointmentMappings.Type` / `.RecurrenceInfo` | Required for recurring appointments |

## Step 6: Build and Run

```bash
dotnet build
dotnet run
```

You'll see a scheduler grouped by doctor, with the two pre-populated appointments visible. Double-click an appointment to edit; right-click for label/status changes; select a time cell and type to create a new appointment.

## Step 7 (Optional): Add a Ribbon

In Visual Studio Designer, select the `SchedulerControl` and use its **Quick Actions** menu → **Create Ribbon**. This adds a `RibbonControl` with built-in scheduler commands (navigate, change view, switch grouping, today, etc.).

Requires the `DevExpress.Wpf.Ribbon` package. The host window must be a `ThemedWindow` with `WindowKind="Ribbon"` for full integration — see the `devexpress-wpf-ribbon-and-bars` skill.

## Unbound Mode (No Data Binding)

For a quick prototype, drop the `DataSource` block entirely:

```xaml
<dxsch:SchedulerControl/>
```

Users can still create / edit / delete appointments — they're held in `SchedulerControl.AppointmentItems` in memory. Lost on app close.

To pre-populate from code:

```csharp
scheduler.AppointmentItems.Add(new AppointmentItem {
    Start = DateTime.Today.AddHours(9),
    End = DateTime.Today.AddHours(10),
    Subject = "Meeting",
});
```

Same for `ResourceItems`, `LabelItems`, `StatusItems`, `TimeRegionItems`.

## .NET Framework Variant

```csharp
using System.Windows;

namespace MyApp;

public partial class App : Application {
}
```

Required assemblies (when not using NuGet):

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.Scheduling.v<XX.X>.dll`
- `DevExpress.XtraScheduler.v<XX.X>.Core.dll`
- `DevExpress.XtraScheduler.v<XX.X>.Core.Desktop.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`

## What to Learn Next

- [Data Binding](data-binding.md) — full reference for `DataSource`, mappings, custom fields, mapping converters
- [Data Items](data-items.md) — appointments, resources, labels, statuses, time regions, reminders, recurrence
- [Views](views.md) — Day / Work Week / Week / Month / Timeline / Agenda / List
- [Styles and Templates](styles-and-templates.md) — `AppointmentContentTemplate`, brushes, theme customization

## Source Material

- `articles/controls-and-libraries/scheduler.md`
- `articles/controls-and-libraries/scheduler/getting-started/create-a-simple-scheduling-application.md` (https://docs.devexpress.com/content/WPF/119796?md=true)
