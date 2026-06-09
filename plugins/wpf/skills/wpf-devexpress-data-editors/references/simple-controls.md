# Simple Controls — Non-BaseEdit Controls

A handful of controls ship in the Data Editors library but **do not inherit from `BaseEdit`** — they have no `EditValue`, no mask support, no `StyleSettings` operation modes. They're either pure UI controls (buttons) or special-purpose widgets (range sliders, calendars, calculators, flyouts).

> Why this matters: data-binding patterns differ. These controls bind through their native properties (`Content`, `Items`, `SelectedDate`, `Value`, etc.) — never through `EditValue`. Masks and the `Validate` event from `BaseEdit` don't apply.

## When to Use This Reference

Use this when you need to:

- Add a themed button (`SimpleButton`), dropdown button, or split button
- Show a standalone calendar (`DateNavigator`) or date-range picker (`DateRangeControl`)
- Show a standalone time picker (`TimePicker`)
- Show a calculator UI (`Calculator`) — standalone, not as an editor
- Use `FlyoutControl` for popup overlays / inline dialogs
- Add a `RangeControl` (range slider with a visualization track)

## Inventory

### Buttons (in `DevExpress.Xpf.Core`)

| Class | Purpose | XAML prefix |
|---|---|---|
| `SimpleButton` | Themed button replacement for `System.Windows.Controls.Button` | `dx:` |
| `DropDownButton` | Button that opens a popup on click | `dx:` |
| `SplitButton` | Button with separate Main and Dropdown halves | `dx:` |

### Standalone Date/Time Pickers (in `DevExpress.Xpf.Editors`)

| Class | Purpose | XAML prefix |
|---|---|---|
| `DateNavigator` | Multi-month calendar grid for picking dates | `dxe:` |
| `DateRangeControl` | Two calendars for picking a date range | `dxe:` |
| `TimePicker` | Hours / minutes / seconds picker | `dxe:` |

### Special-Purpose

| Class | Purpose | XAML prefix |
|---|---|---|
| `Calculator` | Standalone calculator UI | `dxe:` |
| `FlyoutControl` | Popup overlay (notification / inline dialog) | `dxe:` |
| `RangeControl` | Interactive range slider with a visualization area | `dxe:` |

> **Don't confuse standalone with editor**: `DateNavigator` (standalone, no `EditValue`) vs `DateEdit` (editor, has `EditValue` and a dropdown that hosts a `DateNavigator`). Use `DateEdit` when you need a bindable date input; use `DateNavigator` when you need a calendar surface on the form itself.
>
> Same for `Calculator` (standalone surface) vs `PopupCalcEdit` (editor with a calculator dropdown).

## When to Use Which

### Buttons: `SimpleButton` vs `DropDownButton` vs `SplitButton`

```xaml
<!-- Simple action -->
<dx:SimpleButton Content="Save" Command="{Binding SaveCommand}"/>

<!-- Button with menu (always opens menu on click) -->
<dx:DropDownButton Content="Export">
    <dx:DropDownButton.PopupContent>
        <StackPanel>
            <dx:SimpleButton Content="To PDF" Command="{Binding ExportPdfCommand}"/>
            <dx:SimpleButton Content="To XLSX" Command="{Binding ExportXlsxCommand}"/>
        </StackPanel>
    </dx:DropDownButton.PopupContent>
</dx:DropDownButton>

<!-- Default action + dropdown arrow for alternatives -->
<dx:SplitButton Content="Save" Command="{Binding SaveCommand}">
    <dx:SplitButton.PopupContent>
        <StackPanel>
            <dx:SimpleButton Content="Save As..." Command="{Binding SaveAsCommand}"/>
            <dx:SimpleButton Content="Save All" Command="{Binding SaveAllCommand}"/>
        </StackPanel>
    </dx:SplitButton.PopupContent>
</dx:SplitButton>
```

| Pick | When |
|---|---|
| `SimpleButton` | One action. Most common. |
| `DropDownButton` | Click always opens a menu of related actions; no default. |
| `SplitButton` | One primary action + secondary actions in the dropdown. |

### Date/Time Pickers: Standalone vs Editor

| Use Case | Standalone (no `EditValue`) | Editor (bindable) |
|---|---|---|
| Pick a single date | `DateNavigator` | `DateEdit` |
| Pick a date range visually | `DateRangeControl` | `DateEdit` (twice) or `RangeControl` |
| Pick a time | `TimePicker` | `DateEdit` (with a time mask) |

Use **standalone** when the calendar/picker is the main UI of the screen (a scheduling view, a date-range filter panel). Use **editor** when it's one input among many on a form.

### `RangeControl` — Range + Visualization

`RangeControl` is more than a slider: it can render an inline chart or sparkline behind the range thumbs, letting users pan and zoom a data visualization. Common uses: time-series range selector, histogram range, custom-rendered range over any data.

```xaml
<!-- The chart clients live in DevExpress.Xpf.Charts.RangeControlClient (charts assembly),
     NOT the editors namespace. Add:
     xmlns:dxrcc="http://schemas.devexpress.com/winfx/2008/xaml/charts/rangecontrolclient" -->
<dxe:RangeControl RangeStart="{Binding RangeStart, Mode=TwoWay}"
                  RangeEnd="{Binding RangeEnd, Mode=TwoWay}">
    <dxe:RangeControl.Client>
        <dxrcc:DateTimeChartRangeControlClient ItemsSource="{Binding Visits}"
                                               ArgumentDataMember="Day"
                                               ValueDataMember="Count">
            <dxrcc:DateTimeChartRangeControlClient.View>
                <dxrcc:RangeControlClientLineView/>
            </dxrcc:DateTimeChartRangeControlClient.View>
        </dxrcc:DateTimeChartRangeControlClient>
    </dxe:RangeControl.Client>
</dxe:RangeControl>
```

Concrete clients: `NumericChartRangeControlClient`, `DateTimeChartRangeControlClient`, `TimeSpanChartRangeControlClient`. View types: `RangeControlClientLineView`, `RangeControlClientAreaView`, `RangeControlClientBarView`.

For a plain numeric range slider with no visualization, prefer **`TrackBarEdit` + `TrackBarRangeStyleSettings`** — it inherits from `BaseEdit`, so `EditValue` is two-way bindable and validation works.

### `FlyoutControl` — Popup Overlay

For toast-style notifications, inline confirmations, or dropdown panels that should attach to a host element:

```xaml
<dxe:FlyoutControl x:Name="confirmFlyout"
                   PlacementTarget="{Binding ElementName=deleteButton}"
                   StaysOpen="False"
                   Closed="OnFlyoutClosed">
    <StackPanel Margin="12">
        <TextBlock Text="Delete this record?"/>
        <StackPanel Orientation="Horizontal" Margin="0,8,0,0">
            <dx:SimpleButton Content="Yes" Command="{Binding ConfirmDeleteCommand}"/>
            <dx:SimpleButton Content="No" Click="OnFlyoutNo_Click" Margin="8,0,0,0"/>
        </StackPanel>
    </StackPanel>
</dxe:FlyoutControl>
```

```csharp
confirmFlyout.IsOpen = true;
```

`FlyoutControl` derives from `ContentControl` (via `FlyoutBase`): put your UI inside it as `Content` and toggle `IsOpen`. There is no built-in `Caption` / `ButtonType` — build the buttons yourself. Position it with `PlacementTarget` (and `FlyoutSettings.Placement`).

Use when you need a lightweight popup tied to a UI element. For full modal dialogs, prefer `DXMessageBox` or `DXDialog` (in `DevExpress.Xpf.Core`).

### `Calculator` — Standalone Calculator

A bound surface; the result is exposed via `Calculator.Value`. Use it inside layout panels when you want a permanent calculator UI on screen. Use **`PopupCalcEdit`** instead when you want a text field that opens a calculator on demand.

## Why These Aren't BaseEdit

The shared `BaseEdit` API assumes a single "edited value." These controls don't fit that model:

- **Buttons** invoke commands; they don't edit a value
- **`RangeControl`** has *two* edited values (`RangeStart` and `RangeEnd`) plus a visualization client
- **`DateNavigator`** is a stateful calendar surface, not a single picked value
- **`FlyoutControl`** is a popup container, not a value editor
- **`Calculator`** exposes `Value` but is designed for embedding inside other controls (notably `PopupCalcEdit`'s dropdown)

Because they aren't `BaseEdit`, they:

- **Cannot** be used as in-place editors inside `GridControl` columns (no `*EditSettings` class)
- **Don't** support masks
- **Don't** raise the `Validate` event — use WPF's `Validation.ErrorTemplate` or behaviors instead
- **Don't** bind through `EditValue` — use their native value properties (`Content`, `Value`, `SelectedDates`, etc.)

## Common Issues

- **Trying to use `EditValue` on a `SimpleButton`** — it doesn't exist. Use `Content` for the label and `Command` for the action.
- **`DateNavigator` won't bind two-way to a `DateTime` property** — `DateNavigator` doesn't have a single `EditValue`; its model is multi-selection. Use `SelectedDate` / `SelectedDates`, or switch to `DateEdit` if you need a single bindable value.
- **`RangeControl` doesn't validate input** — `BaseEdit.Validate` isn't available. Validate the range in your ViewModel setter or via WPF `Validation.ErrorTemplate` on the bound property.
- **`Calculator` placed in a grid column does nothing** — `Calculator` isn't a `BaseEdit`, so it has no `CalculatorSettings` for in-place use. Use `PopupCalcEdit` + `CalcEditSettings` instead.

## Source Material

- `articles/controls-and-libraries/data-editors.md` § Simple Controls (https://docs.devexpress.com/content/WPF/6190?md=true#simple-controls)
- `articles/controls-and-libraries/data-editors/included-components.md` § Buttons (https://docs.devexpress.com/content/WPF/6933?md=true)
- `articles/controls-and-libraries/data-editors/range-control.md`
