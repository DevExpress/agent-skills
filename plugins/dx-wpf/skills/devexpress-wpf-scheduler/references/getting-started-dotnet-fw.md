# Getting Started — WPF Scheduler (.NET Framework 4.x)

## When to Use This Reference

Use when targeting .NET Framework 4.6.2 or later with the DevExpress WPF `SchedulerControl`. For .NET 8+ (SDK-style projects), use [getting-started.md](getting-started.md) instead — the Scheduler API is identical; only the project plumbing differs.

## NuGet Setup

Install the DevExpress packages through the Package Manager Console:

```powershell
Install-Package DevExpress.Wpf.Scheduling
Install-Package DevExpress.Mvvm           # optional — ViewModelSource / DelegateCommand helpers
Install-Package DevExpress.Wpf.Ribbon     # optional — only if you add a scheduler Ribbon
```

`DevExpress.Wpf.Scheduling` transitively brings `DevExpress.Wpf.Core` (themes, MVVM helpers). All DevExpress packages in a project must share the same version.

## Assembly References (without NuGet)

When referencing the offline assemblies (Unified Component Installer) directly, the minimum set for `SchedulerControl` is:

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.Scheduling.v<XX.X>.dll`
- `DevExpress.XtraScheduler.v<XX.X>.Core.dll`
- `DevExpress.XtraScheduler.v<XX.X>.Core.Desktop.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`

Replace `<XX.X>` with your installed version (for example `v26.1`).

## Minimal Bound Scheduler

### XAML

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxsch="http://schemas.devexpress.com/winfx/2008/xaml/scheduling"
        Title="Calendar" Height="600" Width="900">
    <dxsch:SchedulerControl x:Name="scheduler" ActiveViewIndex="0">
        <dxsch:SchedulerControl.DataSource>
            <dxsch:DataSource AppointmentsSource="{Binding Appointments}">
                <dxsch:DataSource.AppointmentMappings>
                    <dxsch:AppointmentMappings Start="StartTime"
                                               End="EndTime"
                                               Subject="Title"/>
                </dxsch:DataSource.AppointmentMappings>
            </dxsch:DataSource>
        </dxsch:SchedulerControl.DataSource>
    </dxsch:SchedulerControl>
</Window>
```

### Code-Behind

```csharp
using System.Collections.ObjectModel;
using System.Windows;

namespace MyApp {
    public partial class MainWindow : Window {
        public ObservableCollection<MyAppointment> Appointments { get; } =
            new ObservableCollection<MyAppointment>();

        public MainWindow() {
            InitializeComponent();
            Appointments.Add(new MyAppointment {
                Title = "Kickoff",
                StartTime = System.DateTime.Today.AddHours(9),
                EndTime = System.DateTime.Today.AddHours(10),
            });
            DataContext = this;
        }
    }

    public class MyAppointment {
        public string Title { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
    }
}
```

`Start` and `End` are the only required mappings for non-recurring appointments. See [data-binding.md](data-binding.md) for the full mapping reference and recurrence (`Type` + `RecurrenceInfo`).

## Differences from .NET 8+

| Aspect | .NET Framework 4.x | .NET 8+ |
|---|---|---|
| Project file | Standard (non-SDK) `.csproj` | SDK-style `.csproj` |
| `UseWPF` | Not used (WPF is implicit) | Required in `.csproj` |
| `app.config` binding redirects | May be required | Not applicable |
| `Application` ambiguity | Not an issue | Qualify `System.Windows.Application` (implicit usings clash with WinForms) |
| Scheduler / mapping API | Same | Same |

## app.config Binding Redirects

If you hit assembly-version conflicts, add redirects for the DevExpress assemblies:

```xml
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <dependentAssembly>
      <assemblyIdentity name="DevExpress.Xpf.Scheduling.v26.1" publicKeyToken="b88d1754d700e49a" />
      <bindingRedirect oldVersion="0.0.0.0-65535.65535.65535.65535" newVersion="26.1.X.0" />
    </dependentAssembly>
  </assemblyBinding>
</runtime>
```

## What to Learn Next

- [Data Binding](data-binding.md) — `DataSource`, mappings, custom fields, mapping converters, change events
- [Data Items](data-items.md) — appointments, resources, labels, statuses, time regions, reminders, recurrence
- [Views](views.md) — Day / Work Week / Week / Month / Timeline / Agenda / List
- [Styles and Templates](styles-and-templates.md) — `AppointmentContentTemplate`, brushes, theme customization
