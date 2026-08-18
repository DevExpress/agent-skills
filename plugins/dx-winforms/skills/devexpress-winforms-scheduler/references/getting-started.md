# Getting Started with SchedulerControl

> **.NET Framework?** For .NET Framework 4.6.2+ project setup, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).


The `SchedulerControl` is a calendar/appointment management control from the `DevExpress.XtraScheduler` namespace. It renders appointments in multiple views (Day, Work Week, Month, Timeline, Agenda, Gantt) and uses a companion `SchedulerDataStorage` component to store appointments and resources.

## When to Use This Reference

- Adding `SchedulerControl` to a project for the first time
- Understanding the NuGet package and assembly requirements
- Connecting the storage component and setting the initial view
- Pairing with `DateNavigator` for calendar-based navigation
- Adding a Ribbon UI with scheduler commands

## NuGet Package

```
DevExpress.Win.Scheduler
```

This ships `DevExpress.XtraScheduler.v26.1.dll` (the main control) and associated assemblies.

> **Install the package:**
> ```powershell
> dotnet add package DevExpress.Win.Scheduler
> ```

## Required Namespace Imports

```csharp
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;  // AppointmentViewInfo, custom draw
using DevExpress.XtraEditors;            // XtraForm
```

## Host Form

Host `SchedulerControl` on `XtraForm` (or `RibbonForm` when using a Ribbon) for correct skin integration.

## Authoring the `.Designer.cs` File

Write the form the way the WinForms designer serializes it: declare the `SchedulerControl` and `SchedulerDataStorage` as fields and create/configure them — including `DataStorage` wiring, the active view, and (in bound mode) the field mappings — inside `InitializeComponent()` in `MainForm.Designer.cs`. Keep only data loading (assigning `DataSource`, creating appointments) and event handlers in `MainForm.cs`. This is what keeps the form openable in the Visual Studio WinForms designer — if you instead `new` the control/storage and set mappings in the form constructor body, the designer file stays empty and the form can no longer be edited visually.

**Rules of thumb — what goes where:**

| `MainForm.Designer.cs` (`InitializeComponent`) | `MainForm.cs` |
| --- | --- |
| `SchedulerControl`, `SchedulerDataStorage`, `DateNavigator` as **fields**; their construction and property setup | Assigning `Appointments.DataSource` / `Resources.DataSource` (bound mode) |
| `DataStorage` wiring, `ActiveViewType`, view options, `Appointments.Mappings.*` | Creating ad-hoc appointments (`CreateAppointment` + `Add`) |
| `BeginInit`/`EndInit`, `SuspendLayout`/`ResumeLayout`, `Controls.Add` | Event handlers (`ReminderAlert`, `AppointmentViewInfoCustomizing`, …), setting `Start` |

**`MainForm.Designer.cs`** — control, storage, and bound-mode mappings, as the designer would emit it:

```csharp
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.XtraScheduler.SchedulerControl schedulerControl1;
    private DevExpress.XtraScheduler.SchedulerDataStorage schedulerDataStorage1;

    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.schedulerControl1 = new DevExpress.XtraScheduler.SchedulerControl();
        this.schedulerDataStorage1 = new DevExpress.XtraScheduler.SchedulerDataStorage();
        ((System.ComponentModel.ISupportInitialize)(this.schedulerDataStorage1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.schedulerControl1)).BeginInit();
        this.SuspendLayout();
        //
        // schedulerDataStorage1 — bound-mode field mappings live here
        //
        this.schedulerDataStorage1.Appointments.Mappings.Start = "StartTime";
        this.schedulerDataStorage1.Appointments.Mappings.End = "EndTime";
        this.schedulerDataStorage1.Appointments.Mappings.Subject = "Subject";
        //
        // schedulerControl1
        //
        this.schedulerControl1.DataStorage = this.schedulerDataStorage1;
        this.schedulerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.schedulerControl1.Name = "schedulerControl1";
        this.schedulerControl1.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.WorkWeek;
        // View-specific options (e.g. DayView.TimeScale, and keeping TimeRulers) also belong here, not in the constructor.
        //
        // MainForm
        //
        this.Controls.Add(this.schedulerControl1);
        this.Name = "MainForm";
        this.Text = "Schedule";
        ((System.ComponentModel.ISupportInitialize)(this.schedulerDataStorage1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.schedulerControl1)).EndInit();
        this.ResumeLayout(false);
    }
}
```

**`MainForm.cs`** — only data and behavior:

```csharp
using System;
using DevExpress.XtraEditors;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                          // builds scheduler + storage + mappings
        // Bound mode: assign the whole collection to the storage (not item-by-item).
        schedulerDataStorage1.Appointments.DataSource = Meeting.All;
        schedulerControl1.Start = DateTime.Today;
    }
}
```

## Minimal Setup in Code

This condensed form (control created in the constructor) is fine for a throwaway prototype or a control hosted in code you never open in the designer. For a form you want to keep editing visually, use the designer-file split shown above instead.

```csharp
using DevExpress.XtraScheduler;
using DevExpress.XtraEditors;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var storage = new SchedulerDataStorage();
        var scheduler = new SchedulerControl {
            Dock = DockStyle.Fill,
            DataStorage = storage,
            ActiveViewType = SchedulerViewType.WorkWeek,
            Start = DateTime.Today
        };
        Controls.Add(scheduler);

        // Add appointments in unbound mode
        var apt = storage.CreateAppointment(AppointmentType.Normal,
            DateTime.Today.AddHours(9),
            DateTime.Today.AddHours(10),
            "Team Meeting");
        apt.Location = "Room 101";
        storage.Appointments.Add(apt);
    }
}
```

## Pairing with DateNavigator

`DateNavigator` is a calendar control that automatically synchronizes selection with the `SchedulerControl`. Place both in a `SplitContainerControl` to allow runtime resizing.

Declare the `DateNavigator` as a field in `MainForm.Designer.cs` and create/dock it in `InitializeComponent()` — the only line you typically set in code is the link to the scheduler:

```csharp
// dateNavigator1 and schedulerControl1 are declared in MainForm.Designer.cs
dateNavigator1.SchedulerControl = schedulerControl1;
```

Selecting a date in `DateNavigator` scrolls the scheduler. Selecting a range automatically picks the best-fitting view.

## RibbonForm Integration

For a Ribbon-based application, derive the form from `RibbonForm` and declare the Ribbon in `MainForm.Designer.cs`. A scheduler Ribbon typically includes:
- **Navigate** tab — Back/Forward, date picker, view switcher
- **View** tab — view type buttons, time scale, grouping options
- **File** tab — print and export

## Setting the Initial Date

```csharp
schedulerControl1.Start = new DateTime(2026, 5, 1);
```

The scheduler focuses the current day by default. Set `Start` when displaying historical or future data.

## Source Material

- `articles/controls-and-libraries/scheduler/getting-started.md` (`xref:2949`)
- `articles/controls-and-libraries/scheduler.md` (`xref:1971`)
- `DevExpress.XtraScheduler.SchedulerControl` (`xref:DevExpress.XtraScheduler.SchedulerControl`)
- `DevExpress.XtraScheduler.SchedulerDataStorage` (`xref:DevExpress.XtraScheduler.SchedulerDataStorage`)
- `DevExpress.XtraScheduler.DateNavigator` (`xref:DevExpress.XtraScheduler.DateNavigator`)
