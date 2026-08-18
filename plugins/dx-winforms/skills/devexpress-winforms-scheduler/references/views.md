# Views

`SchedulerControl` displays appointments through interchangeable views. Switch views via `SchedulerControl.ActiveViewType`. Each view type has a dedicated options object accessible via `SchedulerControl.Views.<ViewName>` (e.g., `schedulerControl1.DayView`).

## When to Use This Reference

- Choosing which view to show by default and which to expose to end users
- Configuring view-specific options (time scale, work hours, week compression)
- Switching views programmatically
- Using `Timeline` or `Gantt` views for project-style scheduling
- Configuring how many resources appear side-by-side per view

## View Types Overview

| `SchedulerViewType` | Class | Best For |
|---|---|---|
| `Day` | `DayView` | Daily agenda, precise time-slot scheduling, up to ~7 days |
| `WorkWeek` | `WorkWeekView` | Current work week (Mon–Fri), most common default for business apps |
| `FullWeek` | `FullWeekView` | Full 7-day week including weekends |
| `Month` | `MonthView` | Monthly calendar overview; shows many appointments compactly |
| `Year` | `YearView` | Annual overview; all 12 months at once |
| `Timeline` | `TimelineView` | Horizontal timeline; best for resource scheduling and overlapping tasks |
| `Agenda` | `AgendaView` | Chronological list of upcoming appointments; Outlook-style |
| `Gantt` | `GanttView` | Task/project scheduling with dependencies and percent-complete |
| `Week` *(outdated)* | `WeekView` | Legacy 7-day week; not shown in Ribbon UI; kept for compatibility |

## Switching Views

```csharp
// Programmatically
schedulerControl1.ActiveViewType = SchedulerViewType.Month;

// Respond to view change
schedulerControl1.ActiveViewChanged += (s, e) => {
    Console.WriteLine($"Switched to {schedulerControl1.ActiveViewType}");
};
```

## Day View

Displays appointments on a vertical time axis. Supports multi-day selection via `DateNavigator`.

```csharp
DayView dayView = schedulerControl1.DayView;

dayView.TimeScale           = TimeSpan.FromMinutes(30);  // cell height interval
dayView.ShowWorkTimeOnly    = true;                      // hide non-working hours
dayView.WorkTime            = new WorkTimeInterval(
    TimeSpan.FromHours(8), TimeSpan.FromHours(18));      // 8:00–18:00

dayView.DayCount            = 3;     // show 3 days side by side
dayView.ResourcesPerPage    = 2;     // show 2 resources per day column
```

### Time Ruler

The time ruler is the vertical hour gauge down the left side of the Day and Work Week views. **Keep at least one time ruler visible** — without it users cannot tell which time slot a cell represents. The Day/Work Week views ship with a default ruler (`TimeRulers[0]`), so do **not** clear `TimeRulers`; only add to or style the collection.

```csharp
// Show current time marker
dayView.TimeRulers[0].ShowCurrentTime = true;

// Two rulers with different time formats (e.g., local + UTC)
var utcRuler = new TimeRuler { TimeZoneId = "UTC" };
dayView.TimeRulers.Add(utcRuler);
```

## Work Week View

Identical to Day View but always shows Mon–Fri of the current week.

```csharp
WorkWeekView wwView = schedulerControl1.WorkWeekView;
wwView.TimeScale        = TimeSpan.FromMinutes(15);
wwView.ShowWorkTimeOnly = false;
```

## Full Week View

Like Work Week, but includes Saturday and Sunday.

```csharp
FullWeekView fwView = schedulerControl1.FullWeekView;
fwView.TimeScale = TimeSpan.FromHours(1);
```

## Month View

Shows a month-grid calendar. Long appointments span across day cells.

```csharp
MonthView monthView = schedulerControl1.MonthView;

monthView.CompressWeekend  = true;   // show Sat+Sun in a single narrow column
monthView.ShowWeekend       = true;   // hide Sat+Sun entirely when false
monthView.WeekCount         = 6;      // number of week rows visible
```

> Automatic cell height (`CellsAutoHeightOptions`) is a Timeline/Gantt View feature, not a Month View option. See [Timeline View](#timeline-view).

## Year View

Displays all 12 months. Best paired with `DateNavigator` for selection.

```csharp
YearView yearView = schedulerControl1.YearView;
yearView.MonthCount = 12;   // number of months displayed (default 12)
```

## Timeline View

Appointments appear as horizontal bars. Resources appear in rows. Best for resource-centric scheduling and capacity planning.

```csharp
TimelineView tlView = schedulerControl1.TimelineView;
tlView.ResourcesPerPage = 5;

// Configure time scales (the stacked header bands of the Timeline view).
// Note: time scales are NOT the time ruler. Scales are the horizontal header
// bands of the Timeline view; the time ruler is the vertical hour gauge on the
// left of the Day/Work Week views (see "Time Ruler" above, dayView.TimeRulers).
tlView.Scales.Clear();
tlView.Scales.Add(new TimeScaleDay());   // top scale band: days
tlView.Scales.Add(new TimeScaleHour());  // bottom scale band: hours

// Set the visible range: pass a TimeIntervalCollection to SetVisibleIntervals.
// (TimelineView has no IntervalCount property; the visible range is defined by
//  the time intervals you supply, relative to the base scale.)
var intervals = new TimeIntervalCollection();
intervals.Add(new TimeInterval(schedulerControl1.Start, TimeSpan.FromDays(14)));  // 14 days
tlView.SetVisibleIntervals(intervals);
```

### Automatic Cell Height

When grouping by resource, the Timeline (and Gantt) View can size resource rows automatically to fit their appointments. Configure this via `CellsAutoHeightOptions`; `AutoHeightMode` accepts `SchedulerCellAutoHeightMode` values (`None`, `Limited`, `Full`):

```csharp
schedulerControl1.GroupType = SchedulerGroupType.Resource;
tlView.CellsAutoHeightOptions.AutoHeightMode = SchedulerCellAutoHeightMode.Limited;
```

### Time Scale Classes

| Class | Granularity |
|---|---|
| `TimeScale15Minutes` | 15 minutes (finest built-in scale) |
| `TimeScaleHour` | Hours |
| `TimeScaleWorkHour` | Working hours only |
| `TimeScaleDay` | Days |
| `TimeScaleWorkDay` | Working days only |
| `TimeScaleWeek` | Weeks |
| `TimeScaleMonth` | Months |
| `TimeScaleQuarter` | Quarters |
| `TimeScaleYear` | Years |

## Agenda View

Lists upcoming appointments chronologically by day. Useful for a read-focused "what's next" display.

```csharp
AgendaView agendaView = schedulerControl1.AgendaView;
agendaView.DayCount = 30;  // number of days whose appointments are listed
```

## Gantt View

Shows appointments as tasks with optional dependency arrows and a percent-complete fill. Use `AppointmentDependency` objects to define predecessor/successor relationships.

```csharp
schedulerControl1.ActiveViewType = SchedulerViewType.Gantt;

// Add a dependency — created by appointment ID on the AppointmentDependencies collection
object idA = taskA.Id, idB = taskB.Id;
schedulerDataStorage1.AppointmentDependencies.CreateAppointmentDependency(
    idA, idB, AppointmentDependencyType.FinishToStart);
```

> For complex project management needs, prefer the stand-alone `GanttControl` (`DevExpress.Win.Gantt`).

## Hiding Views from End Users

Disable individual views by setting `Enabled = false`:

```csharp
schedulerControl1.Views.DayView.Enabled = false;
schedulerControl1.Views.GanttView.Enabled = false;
```

## Grouping by Date or Resource

Both grouping modes work across view types — the visual layout of the groups
depends on the active view, not on the `GroupType`:

```csharp
// Group by resource. How resources are laid out depends on the view:
// Day/Work Week views render each resource as a vertical column, while
// Timeline/Gantt views render each resource as a horizontal band (row).
schedulerControl1.GroupType = SchedulerGroupType.Resource;

// Group by date — supported by multiple views (not only Timeline); each
// date forms its own group in the layout used by the active view.
schedulerControl1.GroupType = SchedulerGroupType.Date;

// No grouping
schedulerControl1.GroupType = SchedulerGroupType.None;
```

## View-Level Selected Interval

```csharp
TimeInterval sel = schedulerControl1.ActiveView.SelectedInterval;
Resource selRes  = schedulerControl1.ActiveView.SelectedResource;
```

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| Appointments invisible in Day/WorkWeek view | `ShowWorkTimeOnly = true` and appointments are outside work hours | Set `ShowWorkTimeOnly = false` or extend `WorkTime` |
| Timeline shows too many or too few rows | `ResourcesPerPage` not set | Adjust `TimelineView.ResourcesPerPage` |
| Timeline resource rows too small for their appointments | Auto cell height disabled (grouped by resource) | Set `TimelineView.CellsAutoHeightOptions.AutoHeightMode = SchedulerCellAutoHeightMode.Limited` (or `Full`) |
| View switch removes the selected date | `Start` property not updated | Sync `schedulerControl1.Start` after changing `ActiveViewType` |

## Source Material

- `articles/controls-and-libraries/scheduler/views.md` (`xref:1757`)
- `articles/controls-and-libraries/scheduler/views/day-view.md` (`xref:1758`)
- `articles/controls-and-libraries/scheduler/views/work-week-view.md` (`xref:1772`)
- `articles/controls-and-libraries/scheduler/views/month-view.md` (`xref:1760`)
- `articles/controls-and-libraries/scheduler/views/timeline-view.md` (`xref:3109`)
- `articles/controls-and-libraries/scheduler/views/agenda-view.md` (`xref:115961`)
- `articles/controls-and-libraries/scheduler/views/gantt-view.md` (`xref:10698`)
- `articles/controls-and-libraries/scheduler/views/year-view.md` (`xref:401984`)
- `DevExpress.XtraScheduler.SchedulerControl.ActiveViewType` (`xref:DevExpress.XtraScheduler.SchedulerControl.ActiveViewType`)
