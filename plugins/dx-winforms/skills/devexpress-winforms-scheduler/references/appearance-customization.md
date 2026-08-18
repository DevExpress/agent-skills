# Appearance Customization

This reference covers how to control the visual appearance of the WinForms Scheduler: lightweight event-based styling, full owner-draw custom painting, customizing appointment display text, and drawing individual UI elements (time cells, resource headers, navigation buttons).

## When to Use This Reference

- Changing appointment background / border colors conditionally without full custom painting
- Owner-drawing appointments (e.g., progress bars, icons, custom shapes)
- Customizing the text shown on appointment cells
- Custom-drawing time cells or resource header rows
- Applying different background colors to off-hours or holidays

## Namespace Requirement

Many appearance objects live in the drawing namespace. Add this `using` at the top of any file that works with `AppointmentViewInfo`:

```csharp
using DevExpress.XtraScheduler.Drawing;
```

---

## AppointmentViewInfoCustomizing Event (Lightweight Styling)

This is the **preferred, lightweight approach** for conditional appointment styling. No custom drawing required — modify appearance properties on the `AppointmentViewInfo` object and the control handles rendering.

```csharp
schedulerControl1.AppointmentViewInfoCustomizing +=
    OnAppointmentViewInfoCustomizing;

private void OnAppointmentViewInfoCustomizing(object sender,
    AppointmentViewInfoCustomizingEventArgs e)
{
    Appointment apt = e.ViewInfo.Appointment;
    AppointmentViewInfo vi = e.ViewInfo;

    if (apt.Subject.Contains("URGENT"))
    {
        vi.Appearance.BackColor    = Color.OrangeRed;
        vi.Appearance.BackColor2   = Color.Tomato;
        vi.Appearance.BorderColor  = Color.DarkRed;
        vi.Appearance.ForeColor    = Color.White;
        vi.Appearance.FontStyleDelta = FontStyle.Bold;
    }
}
```

### `AppointmentViewInfo` Key Members

| Member | Description |
|---|---|
| `Appointment` | The underlying `Appointment` object |
| `Appearance` | `AppointmentAppearance` — `BackColor`, `BackColor2`, `ForeColor`, `BorderColor`, `FontStyleDelta` |
| `Image` | Optional icon displayed in the appointment cell |
| `Description` | Text shown in the description line |
| `StatusColor` | Overrides the status side-strip color |
| `Bounds` | Rectangle occupied by the appointment in the view |

---

## CustomDrawAppointment Event (Full Owner-Draw)

Use when you need complete control over how an appointment is rendered — e.g., drawing a progress bar, sparkline, or custom shape.

```csharp
schedulerControl1.CustomDrawAppointment +=
    OnCustomDrawAppointment;

private void OnCustomDrawAppointment(object sender,
    CustomDrawObjectEventArgs e)
{
    AppointmentViewInfo vi = (AppointmentViewInfo)e.ObjectInfo;
    Appointment apt        = vi.Appointment;

    // Draw the default appointment first
    e.DrawDefault();

    // Overlay a progress bar if the appointment has percent-complete data
    if (apt.CustomFields["PercentComplete"] is int pct && pct > 0)
    {
        Rectangle bounds = e.Bounds;
        bounds.Height = 4;
        bounds.Y      = e.Bounds.Bottom - 6;

        // Background strip
        e.Cache.FillRectangle(Color.LightGray, bounds);

        // Progress portion
        bounds.Width = (int)(bounds.Width * pct / 100.0);
        e.Cache.FillRectangle(Color.MediumSeaGreen, bounds);
    }

    // Set e.Handled = true to suppress default rendering entirely
    // (here we called DrawDefault first so we do NOT set Handled = true)
}
```

### Event Args Key Members

| Member | Description |
|---|---|
| `e.ObjectInfo` | Cast to `AppointmentViewInfo` |
| `e.Bounds` | Drawing rectangle |
| `e.Cache` | `GraphicsCache` — access to `Graphics`, helper fill/draw methods |
| `e.Handled` | Set `true` to suppress the default render entirely |
| `e.DrawDefault()` | Renders the standard appointment then lets you overlay |

> **Important**: Either call `e.DrawDefault()` to draw the default content and overlay on top, **or** set `e.Handled = true` and draw everything yourself. Do not do both.

---

## CustomDrawAppointmentBackground Event

Fires before the appointment is painted. Use to draw a custom background while still letting the foreground (text, icons) render normally.

```csharp
schedulerControl1.CustomDrawAppointmentBackground +=
    (s, e) =>
    {
        AppointmentViewInfo vi = (AppointmentViewInfo)e.ObjectInfo;

        if (vi.Appointment.LabelKey?.ToString() == "holiday")
        {
            using var brush = new LinearGradientBrush(
                e.Bounds, Color.SkyBlue, Color.AliceBlue,
                LinearGradientMode.Vertical);
            e.Cache.Graphics.FillRectangle(brush, e.Bounds);
            e.Handled = true;  // skip default background; foreground still draws
        }
    };
```

---

## InitAppointmentDisplayText Event

Customizes the text (subject and/or description) displayed on the appointment cell — without any custom drawing.

```csharp
schedulerControl1.InitAppointmentDisplayText +=
    (s, e) =>
    {
        Appointment apt = e.Appointment;

        // Prepend a tag to the subject
        if (apt.CustomFields["priority"] is string priority)
            e.Text = $"[{priority}] {e.Text}";

        // Optionally change description text
        e.Description = $"{apt.Duration.TotalMinutes:0} min — {apt.Location}";
    };
```

---

## CustomDrawTimeCell Event

Fires for each time cell in Day/WorkWeek/FullWeek views. Use to highlight holidays, off-hours, or special dates.

```csharp
schedulerControl1.CustomDrawTimeCell +=
    (s, e) =>
    {
        // Cell characteristics (interval, resource, selection) come from e.ObjectInfo —
        // cast it to SelectableIntervalViewInfo; they are NOT direct members of e.
        var info = e.ObjectInfo as DevExpress.XtraScheduler.Drawing.SelectableIntervalViewInfo;
        // Highlight lunch hour
        if (info != null && info.Interval.Start.Hour == 12)
        {
            e.Cache.FillRectangle(
                Color.FromArgb(30, Color.Green), e.Bounds);
            e.DrawDefault();
            e.Handled = true;
        }
    };
```

| `e.` Member | Description |
|---|---|
| `ObjectInfo` | Cast to `SelectableIntervalViewInfo` for the cell's `Interval` (`TimeInterval`), `Resource`, and selection state |
| `Bounds` | Cell rectangle |
| `Cache` | `GraphicsCache` for drawing |
| `Handled` | Set `true` to suppress the default cell render |
| `DrawDefault()` | Render the default cell content first |

---

## CustomDrawResourceHeader Event

Fires for each resource header (column/row header in grouped views). Use to show an avatar, a custom badge, or a rich header layout.

```csharp
schedulerControl1.CustomDrawResourceHeader +=
    (s, e) =>
    {
        // Draw default content
        e.DrawDefault();

        // Overlay an availability dot
        Resource res = e.Resource;
        Color dot = IsAvailable(res) ? Color.LimeGreen : Color.Tomato;
        int r = 6;
        Rectangle dotRect = new Rectangle(
            e.Bounds.Right - r - 4, e.Bounds.Top + 4, r, r);
        e.Cache.FillEllipse(dot, dotRect);
    };
```

---

## CustomDrawNavigationButton Event

Fires for the forward/back navigation arrows in Day/WorkWeek views. Rarely needed; use for icon replacement or branding.

```csharp
schedulerControl1.CustomDrawNavigationButton +=
    (s, e) =>
    {
        // Replace arrow with a custom image
        e.Cache.DrawImage(myArrowImage, e.Bounds);
        e.Handled = true;
    };
```

---

## Event Selection Guide

| Goal | Event |
|---|---|
| Change appointment color/font conditionally | `AppointmentViewInfoCustomizing` |
| Full custom painting of an appointment | `CustomDrawAppointment` |
| Custom background, normal foreground | `CustomDrawAppointmentBackground` |
| Modify displayed subject / description text | `InitAppointmentDisplayText` |
| Highlight time cells (holidays, lunch, off-hours) | `CustomDrawTimeCell` |
| Custom resource column/row headers | `CustomDrawResourceHeader` |
| Replace navigation arrows | `CustomDrawNavigationButton` |

---

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| `AppointmentViewInfo` cast throws `InvalidCastException` | `e.ObjectInfo` is a different type (e.g., `TimelineAppointmentViewInfo`) | Cast to the base `AppointmentViewInfo` — both types derive from it |
| Custom background appears behind the status strip | Status strip is drawn after `CustomDrawAppointmentBackground` | Use `CustomDrawAppointment` with `e.DrawDefault()` + overlay instead |
| `InitAppointmentDisplayText` not firing | Appointment text area has zero height (too small) | Increase cell height or time scale |
| Appointment text (foreground) disappears | `e.Handled = true` set in `CustomDrawAppointment` without drawing the text yourself — `Handled` there suppresses *all* default drawing, including text | Either draw the text in the handler (or call `e.DrawDefault()`), or move the customization to `CustomDrawAppointmentBackground`, where `Handled` suppresses only the default background and the foreground still renders |

---

## Source Material

- `articles/controls-and-libraries/scheduler/appearance/appointment-viewinfo-customizing.md` (`xref:1753`)
- `articles/controls-and-libraries/scheduler/appearance/custom-draw-appointment.md` (`xref:1754`)
- `articles/controls-and-libraries/scheduler/appearance/init-appointment-display-text.md` (`xref:1781`)
- `DevExpress.XtraScheduler.SchedulerControl.AppointmentViewInfoCustomizing` (`xref:DevExpress.XtraScheduler.SchedulerControl.AppointmentViewInfoCustomizing`)
- `DevExpress.XtraScheduler.SchedulerControl.CustomDrawAppointment` (`xref:DevExpress.XtraScheduler.SchedulerControl.CustomDrawAppointment`)
- `DevExpress.XtraScheduler.Drawing.AppointmentViewInfo` (`xref:DevExpress.XtraScheduler.Drawing.AppointmentViewInfo`)
