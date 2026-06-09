# Views — Day / Work Week / Week / Month / Timeline / Agenda / List

The Scheduler ships with **seven view types**, all enabled out of the box. Each view shows the same underlying data with a different layout — a Day view shows hourly slots for one date; a Month view shows a calendar grid; a Timeline view shows horizontal bars for resource scheduling; an Agenda is a list. Switch among them with `ActiveViewIndex`, configure each with its own properties, or pre-declare a custom set in XAML.

## When to Use This Reference

Use this when you need to:

- Pick the right view for the data (one day vs whole month vs resource grid)
- Configure view-specific properties (visible time, day count, work-time-only)
- Group by resource or date (`GroupType`)
- Pre-declare which views are available
- Switch views at runtime
- Use multiple views of the same type with different settings

## View Picker

| Need | Pick |
|---|---|
| One day at a time with hourly slots | **Day View** |
| Working days of a week | **Work Week View** |
| All seven days of a week | **Week View** |
| Calendar grid spanning a month or more | **Month View** |
| Horizontal bars per resource over a time scale (project scheduling, room booking) | **Timeline View** |
| Chronological list of appointments grouped by day | **Agenda View** |
| Sortable / filterable grid of appointments | **List View** |

## View Defaults

By default, **all seven views are enabled** with default settings. Set `ActiveViewIndex` to switch among them:

```xaml
<dxsch:SchedulerControl ActiveViewIndex="0"/>
```

`ActiveViewIndex` is 0-based; the implicit default order is Day (0), Work Week (1), Week (2), Month (3), Timeline (4), Agenda (5), List (6).

### Custom View Set

Declaring **any** view in XAML overrides the default set — only the declared views become available. Use this to restrict the user to specific views or to customize view configuration.

```xaml
<dxsch:SchedulerControl ActiveViewIndex="0">
    <dxsch:DayView DayCount="2"/>
    <dxsch:WeekView/>
    <dxsch:MonthView/>
</dxsch:SchedulerControl>
```

This setup gives the user three views only (Day-with-2-days, Week, Month) and starts on the Day view.

### Multiple Views of the Same Type

Define the same view type multiple times with different settings — e.g., a 2-day view and a 5-day view both based on `DayView`:

```xaml
<dxsch:DayView DayCount="2" x:Name="twoDayView"/>
<dxsch:DayView DayCount="5" x:Name="fiveDayView"/>
```

Switch via `ActiveViewIndex`.

## View Classes and Inheritance

| Class | Base | Notes |
|---|---|---|
| `DayView` | `DayViewBase` → `ViewBase` | Single or multi-day vertical layout |
| `WorkWeekView` | `DayViewBase` → `ViewBase` | Only work-days of a week |
| `WeekView` | `DayViewBase` → `ViewBase` | All seven days |
| `MonthView` | `ViewBase` | Calendar grid |
| `TimelineView` | `ViewBase` | Horizontal bars |
| `AgendaView` | `ViewBase` | Vertical chronological list |
| `ListView` | `ViewBase` | Grid with sort/filter |

Views derived from `DayViewBase` share many properties (`ShowAllDayArea`, `VisibleTime`, `ShowWorkTimeOnly`, `AppointmentMinHeight`, etc.).

## Day View

Vertical time slots for one or several consecutive days.

```xaml
<dxsch:DayView DayCount="3"
               VisibleTime="08:00:00 - 18:00:00"
               ShowWorkTimeOnly="False"
               ShowAllDayArea="True"
               SnapToCellsMode="Always"/>
```

| Property | Use |
|---|---|
| `DayCount` | How many consecutive days to show (1–14) |
| `VisibleTime` | Time range visible at any moment (the user can still scroll) |
| `ShowWorkTimeOnly` | Hide hours outside `SchedulerControl.WorkTime` |
| `ShowAllDayArea` | Show the all-day appointments banner at the top |
| `SnapToCellsMode` | Snap appointment edges to time cells (`Always`, `Never`, etc.) |
| `AppointmentMinHeight` | Minimum visual height in pixels |
| `TimeIndicatorVisibility` | Show the red "now" line |

## Work Week View

Same anatomy as Day view, but only renders days in `SchedulerControl.WorkDays`.

```xaml
<dxsch:WorkWeekView ShowWorkTimeOnly="True"/>
```

Useful for business apps where weekends shouldn't take screen space.

## Week View

All seven days of a week in a single horizontal strip.

```xaml
<dxsch:WeekView/>
```

`SchedulerControl.FirstDayOfWeek` controls whether the week starts Monday, Sunday, etc.

## Month View

Calendar grid showing entire months.

```xaml
<dxsch:MonthView ViewMode="Standard"/>
```

| Property | Use |
|---|---|
| `ViewMode` | `MonthViewMode.Standard` (default — fixed grid that always fits the requested months) or `MonthViewMode.UnlimitedScrolling` (continuous vertical scroll across months) |
| `WeekCount` | How many weeks to display |
| `MonthCount` | How many consecutive months to show |
| `ShowWorkDaysOnly` | Hide non-working days (inverse of "show weekends") |
| `DisplayUnit` | Granularity of the calendar grid (`MonthViewDisplayUnit.Month` / `Week` / `Day`) |
| `HighlightEvenMonths` | Visually alternate background between adjacent months |
| `StretchAppointments` | Stretch appointments to fill the cell height |

The Month view inherits directly from `ViewBase` (not `DayViewBase`), so day-view properties don't apply.

## Timeline View

Horizontal bars along a time scale — the best view for resource scheduling, project Gantt-like layouts, room bookings.

```xaml
<dxsch:TimelineView>
    <dxsch:TimelineView.TimeScales>
        <dxsch:TimeScale ScaleUnit="Hour" UnitCount="1"/>
        <dxsch:TimeScale ScaleUnit="Day" UnitCount="1"/>
    </dxsch:TimelineView.TimeScales>
</dxsch:TimelineView>
```

| Property | Use |
|---|---|
| `TimeScales` | Collection of `TimeScale` objects defining the zoom granularity |
| `ViewMode` | `TimelineViewMode.Standard` (modern layout, v20.2+) or `TimelineViewMode.Legacy` (older layout); the property is nullable (`TimelineViewMode?`) — leave unset to pick the default automatically |
| `AppointmentContentTemplate` | Custom template for appointment bars |

### Time Scales

Each `TimeScale` defines a single division unit. Multiple scales let users zoom: when zoomed out, longer scales (Day, Week, Month) are active; zoomed in, shorter scales (Hour, Minute) take over.

```xaml
<dxsch:TimelineView.TimeScales>
    <dxsch:TimeScale ScaleUnit="Minute" UnitCount="15"/>
    <dxsch:TimeScale ScaleUnit="Hour" UnitCount="1"/>
    <dxsch:TimeScale ScaleUnit="Day" UnitCount="1"/>
    <dxsch:TimeScale ScaleUnit="Week" UnitCount="1"/>
</dxsch:TimelineView.TimeScales>
```

The scale with the shortest visible duration of a single division (`ScaleUnit × UnitCount`) is the one displayed at the current zoom level.

### Legacy vs Modern Timeline

v20.2 introduced a redesigned Timeline view with:

- Adaptive `TimeScale`
- Per-pixel scrolling
- Expand/collapse for resources with overlapping appointments
- Adaptive resource height

**Custom styles and templates from the Legacy view aren't compatible with the Modern view.** If you have an existing Legacy customization, set `TimelineView.ViewMode = Legacy` to keep working until you migrate.

## Agenda View

A list-style chronological view, like Outlook's Agenda or a phone's calendar app.

```xaml
<dxsch:AgendaView DayCount="14"
                  ShowAppointmentDescription="True"
                  ShowAppointmentLocation="True"/>
```

| Property | Use |
|---|---|
| `DayCount` | How many days to render in the list |
| `Days` | Explicit dates to include (alternative to `DayCount`) |
| `ShowAppointmentDescription` | Include the description text |
| `ShowAppointmentLocation` | Include the location |
| `ShowAppointmentInterval` | Show start/end times |
| `ShowAppointmentResource` | Show the resource caption |
| `ShowAppointmentStatus` / `ShowAppointmentLabel` | Show colored markers |
| `ShowAppointmentRecurrenceImage` / `ShowAppointmentReminderImage` / `ShowAppointmentArrowImages` | Toggle icons |

## List View

A `GridControl`-style view of appointments — sortable, filterable, groupable by appointment fields.

```xaml
<dxsch:ListView ShowChangedOccurrences="True"
                ShowDeletedOccurrences="False"/>
```

| Property | Use |
|---|---|
| `ShowChangedOccurrences` | Include exceptions from recurring patterns |
| `ShowDeletedOccurrences` | Include explicitly-deleted occurrences |

Use this when users want spreadsheet-style analysis of appointments. Note: a `DateNavigator` paired with the Scheduler does NOT filter the List view's data — use the List view's built-in filter UI instead.

## Grouping — Resource vs Date

`SchedulerControl.GroupType` toggles how lanes are organized:

| `SchedulerGroupType` | Effect |
|---|---|
| `None` | Single timeline; resources don't influence layout |
| `Resource` | One lane per resource (vertical in Day/Week, horizontal in Timeline) |
| `Date` | One lane per date with resources stacked inside each lane |

```xaml
<dxsch:SchedulerControl GroupType="Resource">
```

Group by resource for personnel scheduling (each doctor's day stack). Group by date for room/equipment overview (each room's bookings stacked).

## Switch Views at Runtime

```csharp
scheduler.ActiveViewIndex = 2;   // Show the Week view
```

Or via the auto-generated Ribbon's view-toggle buttons (Quick Actions → Create Ribbon).

## Programmatic Navigation

`SchedulerControl` doesn't expose `GoTo*` shortcuts. Step navigation lives on `ViewBase` (every view inherits `NavigateBackward` / `NavigateForward` / `ZoomIn` / `ZoomOut`); jumping to a specific date uses the built-in dialog or the day-based views' `Days` collection.

```csharp
// Step backward / forward by one view interval (works on all views).
scheduler.ActiveView.NavigateForward();
scheduler.ActiveView.NavigateBackward();

// Open the built-in "Go To Date" dialog (a target date is required).
scheduler.ShowGotoDateWindow(DateTime.Today);

// Jump a day-based view to a specific date by setting Days.
if (scheduler.ActiveView is DayView dayView)
    dayView.Days = new[] { new DateTime(2026, 6, 15) };
```

A paired `DateNavigator` (from `DevExpress.Xpf.Editors`) is the typical UI for date jumping — bind its `SelectedDates` to a day-based view's `Days`.

## Common Patterns

### Office-Style Set

```xaml
<dxsch:SchedulerControl ActiveViewIndex="1" GroupType="None" FirstDayOfWeek="Monday">
    <dxsch:DayView/>
    <dxsch:WorkWeekView/>
    <dxsch:WeekView/>
    <dxsch:MonthView/>
</dxsch:SchedulerControl>
```

### Resource-Centric (Doctor's Office)

```xaml
<dxsch:SchedulerControl ActiveViewIndex="0" GroupType="Resource" FirstDayOfWeek="Monday">
    <dxsch:DayView DayCount="1"/>
    <dxsch:WorkWeekView/>
</dxsch:SchedulerControl>
```

One day per doctor in Day view; work-week overview when planning.

### Project Timeline

```xaml
<dxsch:SchedulerControl ActiveViewIndex="0" GroupType="Resource">
    <dxsch:TimelineView>
        <dxsch:TimelineView.TimeScales>
            <dxsch:TimeScale ScaleUnit="Day" UnitCount="1"/>
            <dxsch:TimeScale ScaleUnit="Week" UnitCount="1"/>
            <dxsch:TimeScale ScaleUnit="Month" UnitCount="1"/>
        </dxsch:TimelineView.TimeScales>
    </dxsch:TimelineView>
</dxsch:SchedulerControl>
```

Resource lanes with day/week/month zoom — perfect for project planning.

### Read-Only Agenda

```xaml
<dxsch:SchedulerControl ActiveViewIndex="0">
    <dxsch:AgendaView DayCount="30" ShowAppointmentDescription="True"/>
</dxsch:SchedulerControl>
```

Single Agenda view — looks like a phone calendar. Disable editing via `SchedulerControl.AllowAppointmentEdit="False"` etc. for a true read-only display.

## Common Issues

- **All seven views always show when I just want Day + Week** — explicitly declare the views you want in XAML. Even adding just `<dxsch:DayView/>` overrides the default "all enabled" behavior.
- **`ActiveViewIndex` shows the wrong view** — index doesn't match the XAML declaration order. Indexes are 0-based and follow the XAML order in `SchedulerControl.Views`.
- **TimelineView custom template doesn't render** — built against the Legacy view. Set `TimelineView.ViewMode="Legacy"` to roll back, or migrate the template to the Modern view's elements.
- **Group lanes don't appear** — `GroupType` is `None`. Set `Resource` or `Date`.
- **DateNavigator doesn't filter ListView** — by design; ListView ignores DateNavigator. Use the ListView's filter row.
- **Same view shown twice in user-visible navigation** — multiple view declarations of the same type. That's intentional if you want two configurations; if not, remove duplicates.

## Source Material

- `articles/controls-and-libraries/scheduler/views.md` (https://docs.devexpress.com/content/WPF/119203?md=true)
- `articles/controls-and-libraries/scheduler/views/day-view.md` (https://docs.devexpress.com/content/WPF/119204?md=true)
- `articles/controls-and-libraries/scheduler/views/work-week-view.md` (https://docs.devexpress.com/content/WPF/119205?md=true)
- `articles/controls-and-libraries/scheduler/views/week-view.md` (https://docs.devexpress.com/content/WPF/119206?md=true)
- `articles/controls-and-libraries/scheduler/views/month-view.md` (https://docs.devexpress.com/content/WPF/119207?md=true)
- `articles/controls-and-libraries/scheduler/views/timeline-view.md` (https://docs.devexpress.com/content/WPF/119586?md=true)
- `articles/controls-and-libraries/scheduler/views/agenda-view.md` (https://docs.devexpress.com/content/WPF/400420?md=true)
- `articles/controls-and-libraries/scheduler/views/list-view.md` (https://docs.devexpress.com/content/WPF/400421?md=true)
- `articles/controls-and-libraries/scheduler/grouping.md`
