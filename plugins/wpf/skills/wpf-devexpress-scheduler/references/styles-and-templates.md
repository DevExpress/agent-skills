# Appearance Customization — Styles, Templates, and Brushes

The Scheduler exposes three layers of appearance control:

1. **Built-in appointment presenters** + a content template (`AppointmentContentTemplate`) for layout
2. **`AppointmentStyle`** per view — set the appointment control's instance properties via standard WPF styles
3. **Brushes** — `BrushSet` for a named palette, `BrushProvider` for theme-aware customization of resource cells, headers, navigation buttons

Plus a global toggle between the modern **Outlook2019** style and the legacy **Classic** style.

## When to Use This Reference

Use this when you need to:

- Customize how appointment text/icons are laid out (`AppointmentContentTemplate`)
- Use the built-in presenters (`AppointmentIntervalSubjectPresenter`, `AppointmentLocationPresenter`, `AppointmentDescriptionPresenter`, `AppointmentImagesPanel`)
- Set per-view styles for appointments (`AppointmentStyle`)
- Define a color palette (`BrushSet`) and reference it by name
- Override theme brushes for resource headers, navigation buttons, time-slot cells (`BrushProvider`)
- Switch between the modern (v19.1+) and Classic appearance

## Three Customization Layers

| Layer | Use for | Granularity |
|---|---|---|
| **Content template** (`AppointmentContentTemplate`) | Rearrange what's shown inside an appointment box (subject, location, description, icons) | Per view |
| **Style** (`AppointmentStyle`) | Toggle visibility of presenters, set font, padding, background | Per view |
| **Brush** (`BrushSet`, `BrushProvider`, item-level `Brush`) | Colors for resources, labels, statuses, time regions, cells, headers | Per item or per theme |

Pick the lowest layer that gets the job done. For "show description below subject", use a content template. For "make the time slot lighter", use a brush.

## Layer 1: Appointment Controls and Presenters

The Scheduler renders appointments through one of several appointment controls (depending on view type and whether the appointment is all-day):

- **`AppointmentControl`** — the abstract base
- Vertical appointment control (used in Day / Week / Work Week views for non-all-day)
- Horizontal appointment control (used in all-day banner, Month view, Timeline view)

Inside the control sit several **content presenters** — small UI components that render specific data:

| Presenter | What it shows |
|---|---|
| `AppointmentIntervalSubjectPresenter` | Start/end time + subject |
| `AppointmentLocationPresenter` | Location |
| `AppointmentDescriptionPresenter` | Description |
| `AppointmentImagesPanel` | Recurrence / exception / reminder icons |

These presenters live in the `DevExpress.Xpf.Scheduling.Visual` namespace (`dxschv:` prefix):

```xml
xmlns:dxschv="http://schemas.devexpress.com/winfx/2008/xaml/scheduling/visual"
```

## Layer 1 in Action: Override `AppointmentContentTemplate`

The Scheduler's built-in template wraps the presenters in an `AppointmentContentPanel`. Override the template to rearrange the layout:

### Vertical Appointments (Day / Week)

```xaml
<dxsch:DayView.AppointmentContentTemplate>
    <DataTemplate>
        <dxschv:AppointmentContentPanel>
            <dxschv:AppointmentContentPanel.IntervalSubject>
                <dxschv:AppointmentIntervalSubjectPresenter
                    WordWrap="True" FontWeight="Bold"/>
            </dxschv:AppointmentContentPanel.IntervalSubject>
            <dxschv:AppointmentContentPanel.Location>
                <dxschv:AppointmentLocationPresenter
                    WordWrap="True" FontStyle="Italic"/>
            </dxschv:AppointmentContentPanel.Location>
            <dxschv:AppointmentContentPanel.Description>
                <dxschv:AppointmentDescriptionPresenter
                    Margin="0,1,0,0" WordWrap="True"/>
            </dxschv:AppointmentContentPanel.Description>
            <dxschv:AppointmentContentPanel.Images>
                <dxschv:AppointmentImagesPanel/>
            </dxschv:AppointmentContentPanel.Images>
        </dxschv:AppointmentContentPanel>
    </DataTemplate>
</dxsch:DayView.AppointmentContentTemplate>
```

### Horizontal Appointments (Month / Timeline / All-Day)

| Template property | Where it applies |
|---|---|
| `DayViewBase.AppointmentContentTemplate` | Vertical appointments in Day / Work Week / Week views |
| `DayViewBase.AllDayAppointmentContentTemplate` | All-day banner in Day-family views |
| `MonthView.AppointmentContentTemplate` | Month view |
| `TimelineView.AppointmentContentTemplate` | Timeline view |

> Note: the Modern Timeline view (v20.2+) uses a different layout than the Legacy timeline. Templates designed for one aren't portable to the other.

## Layer 2: `AppointmentStyle`

Each view exposes an `AppointmentStyle` property — a standard WPF `Style` that targets the appointment control class. Use setters to toggle presenter visibility, fonts, padding, background:

```xaml
<dxsch:DayView>
    <dxsch:DayView.AppointmentStyle>
        <Style TargetType="dxschv:AppointmentControl">
            <Setter Property="ShowDescription" Value="True"/>
            <Setter Property="ShowLocation" Value="True"/>
            <Setter Property="ShowStatus" Value="True"/>
            <Setter Property="FontSize" Value="12"/>
        </Style>
    </dxsch:DayView.AppointmentStyle>
</dxsch:DayView>
```

| `AppointmentControlBase` property | Effect |
|---|---|
| `ShowInterval` | Show start/end time |
| `ShowStatus` | Show the status bar |
| `ShowLocation` | Show the location text |
| `ShowDescription` | Show the description |
| `ShowRecurrenceImage` / `ShowReminderImage` | Show icons |

Use `AppointmentStyle` for **toggling visibility** without writing a full template. Use a template when you need to change layout.

### Per-View AppointmentStyle Properties

| View | Property |
|---|---|
| Day / Work Week / Week | `DayViewBase.AppointmentStyle` |
| Month | `MonthView.AppointmentStyle` |
| Timeline | `TimelineView.AppointmentStyle` |

## Layer 3: Brushes

The Scheduler colors many elements: resource columns, label-coded appointments, status bars, time regions, headers, navigation buttons, time slots. There are three brush layers:

| Brush layer | Use for |
|---|---|
| **Item-level `Brush`** (`ResourceItem.Brush`, `AppointmentLabelItem.Brush`, etc.) | A specific resource / label / status's color |
| **`BrushSet`** (palette) | Named brushes shared by multiple items; reference via `BrushName` |
| **`BrushProvider`** | Theme-aware brushes for cells, headers, nav buttons — affects the whole Scheduler chrome |

### How Appointment Colors Work

In modern themes:

- An appointment's color = its **label color** if non-transparent
- Else, the color = its **resource color** (when `GroupType = Resource`)
- Else, the theme's default appointment brush

In classic themes, the same rules apply but the brushes are also used for time slots.

### Set Brushes on Items Directly

```xaml
<dxsch:SchedulerControl.ResourceItems>
    <dxsch:ResourceItem Id="1" Caption="Dr. Smith" Brush="LightBlue"/>
    <dxsch:ResourceItem Id="2" Caption="Dr. Jones" Brush="LightCoral"/>
</dxsch:SchedulerControl.ResourceItems>
```

In bound mode, set `ResourceMappings.Brush` to a property name on the resource class that holds the brush.

### `BrushSet` — Named Palette

Define a palette once, reference brushes by name across multiple items:

```xaml
<dxsch:SchedulerControl.BrushSet>
    <dxsch:BrushSet>
        <dxsch:BrushInfo Name="Red" Brush="Red"/>
        <dxsch:BrushInfo Name="Blue" Brush="Blue"/>
    </dxsch:BrushSet>
</dxsch:SchedulerControl.BrushSet>

<dxsch:ResourceItem Id="1" Caption="Dr. Smith" BrushName="Red"/>
<dxsch:ResourceItem Id="2" Caption="Dr. Jones" BrushName="Blue"/>
```

`BrushName` properties exist on `ResourceItem`, `AppointmentLabelItem`, `AppointmentStatusItem`, `TimeRegionItem`. **`Brush` (direct) has higher priority than `BrushName`**.

### Default Brush Names — `DefaultBrushNames`

The static `DefaultBrushNames` class exposes theme-aware names — `Resource1`, `Resource2`, `LabelImportant`, etc. Reference them to get colors that adapt to the active theme:

```xaml
<dxsch:AppointmentLabelItem Id="0"
                            Caption="Important"
                            BrushName="{x:Static dxsch:DefaultBrushNames.LabelImportant}"/>
```

Other available constants include `Resource1`–`Resource16`, `LabelBusiness`, `LabelPersonal`, `LabelTravelRequired`, `LabelMustAttend`, `LabelNeedsPreparation`, `LabelAnniversary`, `LabelBirthday`, `LabelPhoneCall`, and `LabelNone`.

### Override Default Brushes Globally

Provide entries in your `BrushSet` keyed by `DefaultBrushNames.*` to replace default colors in all themes:

```xaml
<dxsch:SchedulerControl.BrushSet>
    <dxsch:BrushSet>
        <dxsch:BrushInfo Name="{x:Static dxsch:DefaultBrushNames.Resource1}" Brush="LightBlue"/>
        <dxsch:BrushInfo Name="{x:Static dxsch:DefaultBrushNames.Resource2}" Brush="Bisque"/>
        <dxsch:BrushInfo Name="{x:Static dxsch:DefaultBrushNames.Resource3}">
            <SolidColorBrush Color="#E9AFA3"/>
        </dxsch:BrushInfo>
    </dxsch:BrushSet>
</dxsch:SchedulerControl.BrushSet>
```

### Per-Theme Customization

Use a `SchedulerThemeKey` resource key to scope the brush set to one specific theme:

```xaml
xmlns:dxscht="http://schemas.devexpress.com/winfx/2008/xaml/scheduling/themekeys"

<UserControl.Resources>
    <dxsch:BrushSet x:Key="{dxscht:SchedulerThemeKey
                            ResourceKey=BrushSet,
                            ThemeName=Office2019Colorful}">
        <dxsch:BrushInfo Name="{x:Static dxsch:DefaultBrushNames.Resource1}" Brush="LightBlue"/>
    </dxsch:BrushSet>
</UserControl.Resources>
```

## `BrushProvider` — Cells, Headers, Navigation Buttons

`BrushProvider` is the lower-level system that colors the **Scheduler chrome** — resource header backgrounds, time-slot cells, navigation arrows. Each theme ships a default `BrushProvider`; you customize via `SchedulerControl.BrushProvider`.

### Key Areas Covered

| Area | Properties (prefix when `GroupType=None` / `Resource`) |
|---|---|
| Headers | `DefaultHeaderBackground` / `ResourceHeaderBackground`, plus `*Foreground` and `*BorderBrush` |
| "Today" header | `DefaultHeaderTodayBackground` / `ResourceHeaderTodayBackground` |
| Navigation buttons | `DefaultNavigationButtonNormalBackground`, `*Hover`, `*Pressed`, `*Disabled` |
| Time slot cells | `DefaultLightCellBackground` / `DefaultDarkCellBackground` (alternating) |
| Cell borders | `DefaultLightCellLightBorderBrush`, `DefaultDarkCellDarkBorderBrush`, etc. |
| Default label color | `DefaultLabelBrush` |

In `GroupType=Resource` / `Date` mode, the properties with `Resource*` prefix accept **brush transformations** instead of direct brushes — `BrightnessBrushTransform`, `MultiplyBrushTransform`, `OverrideBrushTransform`. This lets the resource header brighten / darken based on the resource's own color rather than picking arbitrary brushes.

### Override a Default Brush in All Themes

```xaml
<dxsch:SchedulerControl.BrushProvider>
    <dxsch:BrushProvider DefaultLabelBrush="LightBlue"/>
</dxsch:SchedulerControl.BrushProvider>
```

### Theme-Specific BrushProvider

```xaml
<UserControl.Resources>
    <dxsch:BrushProvider
        x:Key="{dxscht:SchedulerThemeKey ResourceKey=BrushProvider, ThemeName=Office2019Colorful}"
        BasedOn="{DynamicResource {dxscht:SchedulerThemeKey ResourceKey=BrushProvider, ThemeName=Office2019Colorful}}"
        DefaultHeaderTodayBackground="Red"
        ResourceHeaderTodayBackground="{dxsch:OverrideBrushTransform OverrideBrush=Green}"
        ResourceHeaderBackground="{dxsch:BrightnessBrushTransform Brightness=-0.5}"/>
</UserControl.Resources>
```

`BasedOn` inherits from the theme's existing provider, so you only override what you change.

### Conditional Overrides

`BrushProviderOverride` applies only when a condition matches — for example, only in Timeline view:

```xaml
<dxsch:SchedulerControl.BrushProvider>
    <dxsch:BrushProvider>
        <dxsch:BrushProviderOverride
            ConditionViewType="TimelineView"
            ResourceHeaderBackground="{dxsch:BrightnessBrushTransform Brightness=-0.5}"/>
    </dxsch:BrushProvider>
</dxsch:SchedulerControl.BrushProvider>
```

### Brush Transformations

| Transform | Effect |
|---|---|
| `BrightnessBrushTransform` | Adjust brightness by `Brightness` (-1 to +1) |
| `MultiplyBrushTransform` | Multiply by a color (typical for derived shades) |
| `OverrideBrushTransform` | Replace entirely with a fixed brush |
| `IdentityBrushTransform` | No-op (useful as a `BasedOn` placeholder) |

## Modern (Outlook2019) vs Classic UI Style

Starting with v19.1, the Scheduler uses **Outlook2019** style — resource colors and derived colors paint elements (this is the recommended default).

In v18.2 and earlier, the **Classic** style uses fixed theme color schemes.

Switch globally:

```csharp
DevExpress.Xpf.Core.CompatibilitySettings.SchedulerAppearanceStyle =
    DevExpress.Xpf.Core.SchedulerAppearanceStyle.Classic;
```

> Classic mode is a compatibility setting for migrating legacy apps. New apps should leave it on the default Outlook2019.

The Classic style uses its own resource keys for palettes:

| Element | Classic resource key |
|---|---|
| Resources palette | `SchedulerThemeKeys.ResourceColors_Classic` |
| Labels palette | `SchedulerThemeKeys.LabelColors_Classic` |

## Application Theme and Lightweight Themes

The Scheduler honors the application-wide DevExpress theme. Set it once at startup with `ApplicationThemeHelper.ApplicationThemeName`:

```csharp
DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName =
    DevExpress.Xpf.Core.Theme.Win11LightName;
```

For data-dense schedulers (many appointments, large timelines) consider **lightweight themes** — they visually replicate the regular themes but offer faster startup and lower memory by building a smaller visual tree. Enable them in the `App` constructor:

```csharp
using DevExpress.Xpf.Core;

public partial class App : Application {
    static App() {
        CompatibilitySettings.UseLightweightThemes = true;
        ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
    }
}
```

Requires the `DevExpress.Wpf.ThemesLW` NuGet package (or the `DevExpress.Xpf.ThemesLW.v<XX.X>` assembly). The `LightweightTheme` class lists the available themes (`Win11Light`/`Win11Dark`, `Win10Light`/`Win10Dark`, `Office2019Colorful`, the VS2019 family, etc.). Caveats: lightweight themes apply to the entire application only (not per control), are not shown at design time, and have no touch versions.

## Common Patterns

### Bold Subject + Italic Location

```xaml
<dxsch:DayView.AppointmentContentTemplate>
    <DataTemplate>
        <dxschv:AppointmentContentPanel>
            <dxschv:AppointmentContentPanel.IntervalSubject>
                <dxschv:AppointmentIntervalSubjectPresenter FontWeight="Bold"/>
            </dxschv:AppointmentContentPanel.IntervalSubject>
            <dxschv:AppointmentContentPanel.Location>
                <dxschv:AppointmentLocationPresenter FontStyle="Italic"/>
            </dxschv:AppointmentContentPanel.Location>
        </dxschv:AppointmentContentPanel>
    </DataTemplate>
</dxsch:DayView.AppointmentContentTemplate>
```

### Resource Headers with Darker Today Color

```xaml
<dxsch:SchedulerControl.BrushProvider>
    <dxsch:BrushProvider
        ResourceHeaderTodayBackground="{dxsch:OverrideBrushTransform OverrideBrush=#FF2D5F8B}"/>
</dxsch:SchedulerControl.BrushProvider>
```

### Color-Coded Labels Replacing Defaults

```xaml
<dxsch:SchedulerControl>
    <dxsch:SchedulerControl.LabelItems>
        <dxsch:AppointmentLabelItem Id="1" Caption="Surgery"   Brush="LightCoral"/>
        <dxsch:AppointmentLabelItem Id="2" Caption="Checkup"   Brush="LightGreen"/>
        <dxsch:AppointmentLabelItem Id="3" Caption="Follow-up" Brush="LightSkyBlue"/>
    </dxsch:SchedulerControl.LabelItems>
</dxsch:SchedulerControl>
```

Appointments with `LabelId = 1` show with a coral background.

### Hide the Description in All Views via Style

```xaml
<Window.Resources>
    <Style x:Key="hideDesc" TargetType="dxschv:AppointmentControl">
        <Setter Property="ShowDescription" Value="False"/>
    </Style>
</Window.Resources>

<dxsch:SchedulerControl>
    <dxsch:DayView AppointmentStyle="{StaticResource hideDesc}"/>
    <dxsch:WeekView AppointmentStyle="{StaticResource hideDesc}"/>
</dxsch:SchedulerControl>
```

## Common Issues

- **Custom template doesn't apply** — assigned to the wrong view property. Each view has its own `AppointmentContentTemplate`. The Modern Timeline view uses a separate template surface than the Legacy timeline.
- **Style setters don't change presenters** — wrong property names, or the `Style` `TargetType` isn't the appointment control. The `Show*` members are regular instance dependency properties on `AppointmentControlBase`; in a `Style` targeting `dxschv:AppointmentControl`, set them with bare names (`<Setter Property="ShowDescription" .../>`), not the type-qualified attached-property form.
- **Brush has no effect** — both `Brush` and `BrushName` are set; `Brush` wins. Either clear `Brush` or remove the `BrushName` setting.
- **Theme override isn't applied** — the `SchedulerThemeKey` `ThemeName` doesn't match the active theme. Verify the spelling and case (`Office2019Colorful`, `DXStyle`).
- **Resource colors don't show** — `GroupType="None"` (no per-resource lanes). Set `GroupType="Resource"` to get resource-tinted lanes.
- **Legacy customization broke after upgrade** — Modern Timeline view (v20.2+) doesn't support Legacy templates. Set `TimelineView.ViewMode="Legacy"` to roll back, or migrate.
- **Appointments are all gray** — none of the labels has a non-transparent brush, and `GroupType="None"`. Either assign brushes to labels, or group by resource and color the resources.

## Source Material

- `articles/controls-and-libraries/scheduler/styles-and-templates.md` (https://docs.devexpress.com/content/WPF/119866?md=true)
- `articles/controls-and-libraries/scheduler/styles-and-templates/visual-appointment.md` (https://docs.devexpress.com/content/WPF/119867?md=true)
- `articles/controls-and-libraries/scheduler/styles-and-templates/scheduler-elements-that-support-templates.md` (https://docs.devexpress.com/content/WPF/400598?md=true)
- `articles/controls-and-libraries/scheduler/appearance-customization.md` (https://docs.devexpress.com/content/WPF/400994?md=true)
