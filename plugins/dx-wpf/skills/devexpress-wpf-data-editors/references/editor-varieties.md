# Editor Varieties — BaseEdit Descendants and Operation Modes

Every editor that inherits from **`DevExpress.Xpf.Editors.BaseEdit`** shares the same anatomy: an `EditValue` (the bound value), optional `Mask` settings, optional `StyleSettings` for switching operation modes, and an `*EditSettings` counterpart for use as an in-place editor inside `GridControl`, `PropertyGrid`, `Bar` items, etc.

> **`StyleSettings` is the operation-mode switch** — it changes the editor's behavior and appearance (e.g., turn `ComboBoxEdit` into a checked combo or a token combo). It is **not** the WPF `Style` property.

## When to Use This Reference

Use this when you need to:

- Pick the right editor for a given data type
- Switch a `ComboBoxEdit`, `ListBoxEdit`, or `LookUpEdit` between default / checked / radio / token / search modes
- Switch `DateEdit` between calendar / picker modes
- Switch `TrackBarEdit` / `ProgressBarEdit` / `SparklineEdit` between visualization styles
- Find the `*EditSettings` class for embedding an editor inside `GridControl`

## The `BaseEdit` Contract

Every editor below inherits from `BaseEdit` and supports:

- **`EditValue`** — the bindable value (two-way by default). Type-aware: `decimal` for `SpinEdit`, `DateTime` for `DateEdit`, `string` for `TextEdit`, etc.
- **`StyleSettings`** — assign an operation-mode object (e.g., `CheckedComboBoxStyleSettings`)
- **`IsReadOnly`**, **`AllowNullInput`**, **`InvalidValueBehavior`** — value handling
- **`Validate` event**, **`HasValidationError`** — validation
- **`*EditSettings` class** — in-place form for use inside grid columns and property grids

## Editor Inventory by Category

### Text Editors

| Class | Purpose | In-Place Class |
|---|---|---|
| `TextEdit` | Single-line text with optional masking | `TextEditSettings` |
| `ButtonEdit` | Text editor with a customizable button strip — base for many editors below | `ButtonEditSettings` |
| `MemoEdit` | Multi-line text | `MemoEditSettings` |
| `PasswordBoxEdit` | Password input (masked characters) | `PasswordBoxEditSettings` |
| `HyperlinkEdit` | Hyperlink-style display + edit | `HyperlinkEditSettings` |
| `BrowsePathEdit` | Text editor with file/folder browse button | `BrowsePathEditSettings` |
| `AutoSuggestEdit` | Text editor with as-you-type suggestions | `AutoSuggestEditSettings` |

### Numeric Editors

| Class | Purpose | In-Place Class |
|---|---|---|
| `SpinEdit` | Numeric input with up/down spinner buttons | `SpinEditSettings` |
| `PopupCalcEdit` | Numeric input with a popup calculator | `CalcEditSettings` |

### Date and Time Editors

| Class | Purpose | In-Place Class |
|---|---|---|
| `DateEdit` | Date picker with dropdown calendar | `DateEditSettings` |

(Note: `DateNavigator`, `DateRangeControl`, `TimePicker` exist but **don't inherit from `BaseEdit`** — see [simple-controls.md](simple-controls.md).)

### Combobox / Listbox / Lookup

| Class | Purpose | In-Place Class |
|---|---|---|
| `ComboBoxEdit` | Dropdown selector (single, checked, radio, token, checked-token) | `ComboBoxEditSettings` |
| `ListBoxEdit` | Visible list selector | `ListBoxEditSettings` |
| `FontEdit` | Font-family picker | `FontEditSettings` |
| `LookUpEdit` | Grid-backed lookup (single, multi-select, search, token, search-token) | `LookUpEditSettings` |

> `LookUpEdit` lives in `DevExpress.Xpf.Grid.LookUp` — use `xmlns:dxg=".../winfx/2008/xaml/grid"`, not `dxe:`.

### Input Toggles

| Class | Purpose | In-Place Class |
|---|---|---|
| `CheckEdit` | Checkbox (binary or three-state) | `CheckEditSettings` |
| `ToggleSwitchEdit` | iOS-style toggle | `ToggleSwitchEditSettings` |

### Image and Color

| Class | Purpose | In-Place Class |
|---|---|---|
| `ImageEdit` | Embedded image editor | `ImageEditSettings` |
| `PopupImageEdit` | Popup variant of `ImageEdit` | `PopupImageEditSettings` |
| `ColorEdit` | Color picker (inline palette) | `ColorEditSettings` |
| `PopupColorEdit` | Color picker (popup) | `PopupColorEditSettings` |
| `BrushEdit` / `PopupBrushEdit` | Brush picker (in `DevExpress.Xpf.PropertyGrid`) | `BrushEditSettings` / `PopupBrushEditSettings` |

### Visualization

| Class | Purpose | In-Place Class |
|---|---|---|
| `ProgressBarEdit` | Progress bar (native / themed) | `ProgressBarEditSettings` |
| `TrackBarEdit` | Slider (native / themed / range / zoom) | `TrackBarEditSettings` |
| `SparklineEdit` | Inline sparkline (line / area / bar / win-loss) | `SparklineEditSettings` |
| `RatingEdit` | Star-rating control | `RatingEditSettings` |
| `BarCodeEdit` | Barcode renderer (EAN-13, QR, etc.) | `BarCodeEditSettings` |

## Operation Modes via `StyleSettings`

`StyleSettings` is the central mechanism for switching an editor's mode without changing its class. You assign a specific settings object; the editor reconfigures itself accordingly.

### ComboBoxEdit Modes

| Mode | StyleSettings class |
|---|---|
| Default | `ComboBoxStyleSettings` |
| Checked | `CheckedComboBoxStyleSettings` |
| Radio | `RadioComboBoxStyleSettings` |
| Token | `TokenComboBoxStyleSettings` |
| Checked Token | `CheckedTokenComboBoxStyleSettings` |

```xaml
<dxe:ComboBoxEdit ItemsSource="{Binding Cities}" SeparatorString="; ">
    <dxe:ComboBoxEdit.StyleSettings>
        <dxe:CheckedComboBoxStyleSettings/>
    </dxe:ComboBoxEdit.StyleSettings>
</dxe:ComboBoxEdit>
```

### ListBoxEdit Modes

| Mode | StyleSettings class |
|---|---|
| Default | `ListBoxEditStyleSettings` |
| Checked | `CheckedListBoxEditStyleSettings` |
| Radio | `RadioListBoxEditStyleSettings` |

### LookUpEdit Modes

`LookUpEdit` is the most-configurable editor — five distinct modes, all driven by `StyleSettings`:

| Mode | StyleSettings class | Single / Multi | Search | Tokens |
|---|---|---|---|---|
| Default | `LookUpEditStyleSettings` | Single | No | No |
| SearchLookUp | `SearchLookUpEditStyleSettings` | Single | Yes | No |
| MultiSelect | `MultiSelectLookUpEditStyleSettings` | Multi | No | No |
| Token | `TokenLookUpEditStyleSettings` | Multi | No | Yes |
| SearchToken | `SearchTokenLookUpEditStyleSettings` | Multi | Yes | Yes |

```xaml
<dxg:LookUpEdit ItemsSource="{Binding Customers}"
                DisplayMember="Name"
                SeparatorString="; ">
    <dxg:LookUpEdit.StyleSettings>
        <dxg:SearchTokenLookUpEditStyleSettings AllowGrouping="False"/>
    </dxg:LookUpEdit.StyleSettings>
</dxg:LookUpEdit>
```

Common style-settings properties:

- `AllowColumnFiltering` — filter the embedded grid
- `AllowGrouping` — group rows in the embedded grid
- `AllowSorting` — sort rows
- (and lookup-specific options on each subclass)

### DateEdit Modes

`DateEdit` can show different dropdown contents based on `StyleSettings`:

- `DateEditCalendarStyleSettings` (default) — single-month calendar
- `DateEditNavigatorStyleSettings` — `DateNavigator`-based calendar
- `DateEditNavigatorWithTimePickerStyleSettings` — calendar plus a time picker
- `DateEditPickerStyleSettings` — independent date columns (day/month/year)
- `DateEditTimePickerStyleSettings` — time-only picker

(All derive from `DateEditStyleSettingsBase`.) See `articles/.../editor-operation-modes/dateedit.md` (https://docs.devexpress.com/content/WPF/116793?md=true) for the full set.

### TrackBarEdit Modes

| Mode | StyleSettings class | Use |
|---|---|---|
| Native | `TrackBarStyleSettings` | Classic single-thumb slider |
| Range | `TrackBarRangeStyleSettings` | Two-thumb range slider |
| Zoom | `TrackBarZoomStyleSettings` | Zoom-style slider with `+` and `−` buttons |
| Zoom range | `TrackBarZoomRangeStyleSettings` | Two-thumb zoom-style slider |

### ProgressBarEdit Modes

| Mode | StyleSettings class | Use |
|---|---|---|
| Native | `ProgressBarStyleSettings` (default) | Standard progress bar |
| Marquee | `ProgressBarMarqueeStyleSettings` | Indeterminate animation |

### SparklineEdit Modes

| Visualization | StyleSettings class |
|---|---|
| Line | `LineSparklineStyleSettings` |
| Area | `AreaSparklineStyleSettings` |
| Bar | `BarSparklineStyleSettings` |
| Win-Loss | `WinLossSparklineStyleSettings` |

## In-Place Editors Inside GridControl / PropertyGrid

For every BaseEdit-derived control, there's an `*EditSettings` class. Use the settings class — **not** the control class — inside grid columns and property grid items:

```xaml
<dxg:GridColumn FieldName="UnitPrice">
    <dxg:GridColumn.EditSettings>
        <dxe:SpinEditSettings MinValue="0" MaxValue="10000"
                              Mask="c2" MaskType="Numeric"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

Inside the settings class you also set `StyleSettings` to switch the in-place editor's operation mode:

```xaml
<dxg:GridColumn FieldName="Categories">
    <dxg:GridColumn.EditSettings>
        <dxe:ComboBoxEditSettings ItemsSource="{Binding Source={x:Static local:Lists.Categories}}">
            <dxe:ComboBoxEditSettings.StyleSettings>
                <dxe:TokenComboBoxStyleSettings/>
            </dxe:ComboBoxEditSettings.StyleSettings>
        </dxe:ComboBoxEditSettings>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

## Choosing the Right Editor

| Data type / scenario | Editor |
|---|---|
| Free-form short text | `TextEdit` |
| Multi-line text / notes | `MemoEdit` |
| Password / secret | `PasswordBoxEdit` |
| URL display + edit | `HyperlinkEdit` |
| File or folder path | `BrowsePathEdit` |
| Type-ahead suggestions | `AutoSuggestEdit` |
| Integer / decimal | `SpinEdit` |
| Calculator-driven number | `PopupCalcEdit` |
| Date | `DateEdit` |
| Single choice from short list | `ComboBoxEdit` |
| Multi choice (chips) | `ComboBoxEdit` + `TokenComboBoxStyleSettings`, or `LookUpEdit` + `TokenLookUpEditStyleSettings` |
| Single choice with rich grid + search | `LookUpEdit` + `SearchLookUpEditStyleSettings` |
| Always-visible list | `ListBoxEdit` |
| Font picker | `FontEdit` |
| Yes/no | `CheckEdit` or `ToggleSwitchEdit` |
| Color | `ColorEdit` (inline) or `PopupColorEdit` (compact) |
| Brush | `BrushEdit` / `PopupBrushEdit` (PropertyGrid only) |
| Image | `ImageEdit` or `PopupImageEdit` |
| Progress | `ProgressBarEdit` |
| Slider | `TrackBarEdit` |
| Range slider | `TrackBarEdit` + `TrackBarRangeStyleSettings` |
| 1–5 star rating | `RatingEdit` |
| Inline sparkline chart | `SparklineEdit` |
| Barcode display | `BarCodeEdit` |

## Common Issues

- **`StyleSettings` ignored** — assigned the wrong settings class for the editor type, e.g., `CheckedComboBoxStyleSettings` on a `ListBoxEdit`. Each editor accepts only matching settings classes.
- **`LookUpEdit` not found in `dxe:`** — it's in `dxg:` (`DevExpress.Xpf.Grid.LookUp`). Add the grid namespace.
- **Editor settings ignored inside `GridColumn.EditSettings`** — used the control class (`<dxe:SpinEdit/>`) instead of the settings class (`<dxe:SpinEditSettings/>`).
- **Two-way binding shows wrong type** — bound `Text` instead of `EditValue`. `Text` is always a string; `EditValue` is the typed value.

## Source Material

- `articles/controls-and-libraries/data-editors/included-components.md` (https://docs.devexpress.com/content/WPF/6933?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes.md` (https://docs.devexpress.com/content/WPF/116526?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/comboboxedit.md` (https://docs.devexpress.com/content/WPF/116528?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/lookupedit.md` (https://docs.devexpress.com/content/WPF/116556?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/listboxedit.md` (https://docs.devexpress.com/content/WPF/116529?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/dateedit.md` (https://docs.devexpress.com/content/WPF/116793?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/trackbaredit.md` (https://docs.devexpress.com/content/WPF/116531?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/progressbaredit.md` (https://docs.devexpress.com/content/WPF/116530?md=true)
- `articles/controls-and-libraries/data-editors/common-features/editor-operation-modes/sparklineedit.md` (https://docs.devexpress.com/content/WPF/116527?md=true)
