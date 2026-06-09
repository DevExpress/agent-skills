# Views — Blazor Scheduler

When you need to: choose a view type; configure work hours; control cell duration; navigate between dates.

## Available Views

Nest view components inside `DxScheduler`. Multiple views can be declared — the scheduler toolbar lets users switch between them:

```razor
<DxScheduler StartDate="@DateTime.Today" DataStorage="@DataStorage">
    <DxSchedulerDayView />
    <DxSchedulerWeekView ShowWorkTimeOnly="true" />
    <DxSchedulerWorkWeekView />
    <DxSchedulerMonthView />
    <DxSchedulerTimelineView />
</DxScheduler>
```

## View Types

| Component | SchedulerViewType | Description |
|---|---|---|
| `DxSchedulerDayView` | `Day` | Single-day column with hourly rows |
| `DxSchedulerWeekView` | `Week` | 7-day week grid |
| `DxSchedulerWorkWeekView` | `WorkWeek` | Monday–Friday, business week |
| `DxSchedulerMonthView` | `Month` | Full month calendar |
| `DxSchedulerTimelineView` | `Timeline` | Horizontal time axis for resources |

## DxSchedulerDayView Properties

| Property | Type | Description |
|---|---|---|
| `ShowWorkTimeOnly` | `bool` | Show only work hours (hides off-hours) |
| `WorkTime` | `DxSchedulerTimeSpanRange` | Define work hours start/end |
| `CellDuration` | `TimeSpan` | Time interval per row (default: 30 min) |
| `DayCount` | `int` | Number of days visible at once |
| `VisibleTime` | `DxSchedulerTimeSpanRange` | Visible time range regardless of work time |

## DxSchedulerWeekView Properties

| Property | Type | Description |
|---|---|---|
| `ShowWorkTimeOnly` | `bool` | Show only work hours |
| `WorkTime` | `DxSchedulerTimeSpanRange` | Work hours range |
| `CellDuration` | `TimeSpan` | Time interval per row |

## DxSchedulerMonthView Properties

| Property | Type | Description |
|---|---|---|
| `AppointmentDisplayMode` | `SchedulerMonthViewAppointmentDisplayMode` | How appointments are displayed (inline or compact) |

## DxSchedulerTimelineView Properties

| Property | Type | Description |
|---|---|---|
| `CellDuration` | `TimeSpan` | Width of each timeline cell |
| `DayCount` | `int` | Number of days in the timeline |

## Examples

### Day View — Custom Work Hours

```razor
<DxSchedulerDayView ShowWorkTimeOnly="true"
                    WorkTime="@(new DxSchedulerTimeSpanRange(
                        TimeSpan.FromHours(8),
                        TimeSpan.FromHours(18)))"
                    CellDuration="@TimeSpan.FromMinutes(15)" />
```

### Active View Control

Set the initial view type:

```razor
<DxScheduler StartDate="@DateTime.Today"
             DataStorage="@DataStorage"
             ActiveViewType="SchedulerViewType.Month">
    <DxSchedulerWeekView />
    <DxSchedulerMonthView />
</DxScheduler>
```

### Single View (no toolbar switcher)

If only one view is declared, the view-switching toolbar disappears automatically:

```razor
<DxScheduler DataStorage="@DataStorage" StartDate="@DateTime.Today">
    <DxSchedulerMonthView />
</DxScheduler>
```

## Date Navigation

The Scheduler includes a built-in navigation toolbar with Previous/Today/Next buttons when running in an interactive render mode. Use `StartDate` to set the initial date:

```razor
<DxScheduler StartDate="@new DateTime(2024, 6, 1)"
             DataStorage="@DataStorage">
    <DxSchedulerWeekView />
</DxScheduler>
```

For two-way binding on the current date:

```razor
<DxScheduler @bind-StartDate="CurrentDate" DataStorage="@DataStorage">
    <DxSchedulerWeekView />
</DxScheduler>

@code {
    DateTime CurrentDate { get; set; } = DateTime.Today;
}
```
