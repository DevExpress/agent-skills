# Editor Variants (BaseEdit Descendants)

This reference is a decision guide for every editor that inherits from `BaseEdit` — i.e., every control with an `EditValue` property and a matching `RepositoryItem*`. The classes group naturally by branch in the hierarchy: `TextEdit` and its many descendants (text, numeric, date/time, dropdown, lookup, calculator, color picker, popup gallery, breadcrumb, memo, hyperlink), `BaseCheckEdit` (check + toggle), and the leaf `BaseEdit` controls (picture, progress, radio group, rating, track bar, token edit, sparkline, image edit).

For non-`BaseEdit` controls (labels, buttons, list boxes, navigators), see [non-baseedit-controls.md](non-baseedit-controls.md).

## When to Use This Reference

- Picking the right editor among `TextEdit` vs `ButtonEdit` vs `ComboBoxEdit` vs `LookUpEdit` vs `GridLookUpEdit` vs `SearchLookUpEdit`.
- Choosing between `DateEdit` / `TimeEdit` / `DateTimeOffsetEdit` / `TimeSpanEdit`.
- Deciding `CheckEdit` vs `ToggleSwitch` vs `RadioGroup`.
- Numeric input: `SpinEdit` vs `CalcEdit` vs plain `TextEdit` + numeric mask.
- Image rendering: `PictureEdit` vs `ImageEdit` vs `ImageComboBoxEdit`.
- Progress: `ProgressBarControl` vs `MarqueeProgressBarControl`.
- Sliders: `TrackBarControl` vs `RangeTrackBarControl` vs `ZoomTrackBarControl` vs `RatingControl`.

## Text Editors

| Editor | Repository item | Notes |
|---|---|---|
| **`TextEdit`** | `RepositoryItemTextEdit` | Single-line text box. All other text editors derive from it. Supports masks, password char, watermark (`NullValuePrompt`). |
| **`MemoEdit`** | `RepositoryItemMemoEdit` | Multi-line text. **No mask support.** |
| **`MemoExEdit`** | `RepositoryItemMemoExEdit` | Single-line text box + drop-down panel that contains a `MemoEdit` for multi-line content. **No mask support.** |
| `RepositoryItemRichTextEdit` *(repository-only)* | — | RTF cell editor for grid cells. No standalone `RichTextEdit` editor class — host a `RichEditControl` instead. |

```csharp
var memo = new MemoEdit { Width = 300, Height = 120 };
memo.Properties.MaxLength = 1000;
memo.Properties.NullValuePrompt = "Notes…";
```

## Numeric Editors

| Editor | Repository item | When to use |
|---|---|---|
| **`SpinEdit`** | `RepositoryItemSpinEdit` | Numeric value with up/down buttons; integer or decimal. Honors `MinValue`/`MaxValue`/`Increment`/`IsFloatValue`. |
| **`CalcEdit`** | `RepositoryItemCalcEdit` | `SpinEdit` look with a drop-down calculator. Useful when the value is the result of arithmetic. |
| `TextEdit` + Numeric mask | `RepositoryItemTextEdit` | Simpler numeric input without buttons; use when you want pure typing with format validation. |

```csharp
spinEdit1.Properties.MinValue     = 0;
spinEdit1.Properties.MaxValue     = 100;
spinEdit1.Properties.Increment    = 1;
spinEdit1.Properties.IsFloatValue = false;       // integer mode
```

## Date / Time Editors

| Editor | Stores | Notes |
|---|---|---|
| **`DateEdit`** | `DateTime` | Drop-down calendar (Vista, Touch, Classic, Office, … styles). |
| **`TimeEdit`** | `DateTime` (time portion) | Spin buttons for hours/minutes/seconds. |
| **`DateTimeOffsetEdit`** | `DateTimeOffset` | Same UI as `DateEdit` plus timezone offset. |
| **`TimeSpanEdit`** | `TimeSpan` | For duration values; supports `[d.]HH:mm:ss[.fff]` patterns. |

```csharp
// Touch style: DateEdit.Properties.CalendarView (DevExpress.XtraEditors.Repository.CalendarView) — NOT
// VistaCalendarViewStyle, whose values are the zoom levels (MonthView/YearView/…).
dateEdit1.Properties.CalendarView            = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
dateEdit1.Properties.AllowNullInput          = DefaultBoolean.True;
dateEdit1.Properties.NullDate                = DateTime.MinValue;
dateEdit1.Properties.DisplayFormat.FormatType   = FormatType.DateTime;
dateEdit1.Properties.DisplayFormat.FormatString = "d";
```

## Boolean Editors

| Editor | UX | Use when |
|---|---|---|
| **`CheckEdit`** | Classic checkbox; supports `Checked` / `Unchecked` / `Indeterminate` (`AllowGrayed = true`). Multiple `CheckEdit`s in a `RadioGroupIndex` form a radio group. | Form fields, grid cells, mutually exclusive boolean choices. |
| **`ToggleSwitch`** | iOS-style switch with `OnText` / `OffText` captions. | When the boolean is a setting that flips state visibly (notifications on/off). |
| **`RadioGroup`** | Multiple radio buttons in one container. Items have value + caption. | Pick one from 2–6 mutually exclusive named options. |

```csharp
checkEdit1.Properties.Caption    = "Send email";
checkEdit1.Properties.AllowGrayed = false;
checkEdit1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;

toggleSwitch1.Properties.OnText  = "On";
toggleSwitch1.Properties.OffText = "Off";

radioGroup1.Properties.Items.Add(new RadioGroupItem(0, "Low"));
radioGroup1.Properties.Items.Add(new RadioGroupItem(1, "Medium"));
radioGroup1.Properties.Items.Add(new RadioGroupItem(2, "High"));
```

## Dropdown Editors (Single-Choice)

`PopupBaseEdit` and `PopupBaseAutoSearchEdit` are the abstract parents. Concrete classes:

| Editor | Drop-down content | Use when |
|---|---|---|
| **`ComboBoxEdit`** | Flat list of `string` items. | Hardcoded list of strings; values are display-only. |
| **`ImageComboBoxEdit`** | Items with image + display text + underlying value. | Same as combo but with icons; underlying value can be any type. |
| **`FontEdit`** | Installed fonts. | Font picker. |
| **`MRUEdit`** | Most-recently-used strings the user has entered. | Filter input, search box where users add to the list. |
| **`PopupGalleryEdit`** | Tile gallery with icons + captions. | Pick-from-many UX with rich visuals. |
| **`PopupContainerEdit`** | Custom panel (separate `PopupContainerControl`). | When the drop-down needs arbitrary UI you author. |
| **`PopupColorEdit`** *(legacy)* / **`ColorPickEdit`** | Color palette with Web / System / Recent / Custom tabs. | Color picker. |
| **`ColorEdit`** | Same as `ColorPickEdit` but the value is exposed as `Color`. | Simple color picker. |

```csharp
comboBoxEdit1.Properties.Items.AddRange(new[] { "Small", "Medium", "Large" });
comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;   // pick-only

var img = new ImageComboBoxEdit();
img.Properties.Items.AddRange(new[] {
    new ImageComboBoxItem("Low",    LowPriority,    imageIndex: 0),
    new ImageComboBoxItem("High",   HighPriority,   imageIndex: 1)
});
img.Properties.SmallImages = svgImageCollection1;
```

## Multi-Choice Editors

| Editor | UX | Use when |
|---|---|---|
| **`CheckedComboBoxEdit`** | Combo with checkboxes in the dropdown. | Multi-select from a known list; emits comma-separated string by default. |
| **`TokenEdit`** | Text box that turns substrings into chips/tokens. | Tagging, multi-email entry, taggable filter. |

```csharp
checkedComboBoxEdit1.Properties.Items.AddRange(roles);
checkedComboBoxEdit1.Properties.SeparatorChar = ',';
```

## Lookup Editors

Lookups are data-bound dropdowns that show a data source in the panel and write the key field to `EditValue`.

| Editor | Drop-down | Use when |
|---|---|---|
| **`LookUpEdit`** | Simple list (1 or more columns). | Small lookups (< ~10 k rows); no filtering on type. |
| **`GridLookUpEdit`** | Embedded `GridView` (fully featured grid). | Larger lookups, when users need sorting/filtering inside the dropdown. |
| **`SearchLookUpEdit`** | `GridView` + always-on Find Panel above it. | Best UX for medium/large lookups — users type to filter. |
| **`TreeListLookUpEdit`** | Embedded `TreeList`. | Hierarchical lookup (categories, org tree). |

```csharp
lookUp.Properties.DataSource    = customers;
lookUp.Properties.ValueMember   = nameof(Customer.Id);
lookUp.Properties.DisplayMember = nameof(Customer.Name);
lookUp.Properties.NullText      = "(none)";

// Multi-column lookup
lookUp.Properties.Columns.AddRange(new[] {
    new LookUpColumnInfo(nameof(Customer.Code), 80, "Code"),
    new LookUpColumnInfo(nameof(Customer.Name), 200, "Name")
});
```

> Lookups do **not** support masks.

## Image Editors

| Editor | Stores | Notes |
|---|---|---|
| **`PictureEdit`** | `Image` / `byte[]` | Single image with zoom/pan, image-source button. |
| **`ImageEdit`** | `byte[]` | Image plus a drop-down panel that lets users edit (crop, replace, drag-drop). |
| **`ImageSlider`** (non-`BaseEdit`) | Image collection | Carousel of images with prev/next nav. |
| **`ImageComboBoxEdit`** | Item value | Combo with icons; value is item-defined. |
| `BlobBaseEdit` | `byte[]` | Abstract — use `ImageEdit` for images. |

## Progress & Activity

| Editor | UX |
|---|---|
| **`ProgressBarControl`** | Classic 0–100 bar; supports text overlay, `PercentView`, gradient styles. |
| **`MarqueeProgressBarControl`** | Indeterminate scrolling block (operation running, no known total). |

## Sliders

| Editor | UX | Stores |
|---|---|---|
| **`TrackBarControl`** | Single slider with ticks. | `int` |
| **`RangeTrackBarControl`** | Two thumbs picking a range. | A range structure |
| **`ZoomTrackBarControl`** | Slider with – / + buttons; zoom-style UI. | `int` |
| **`RatingControl`** | N identical icons (stars by default) for rating. | `int` (or `decimal` in half-step modes) |

```csharp
ratingControl1.Properties.ItemCount = 5;
ratingControl1.Properties.FillPrecision = RatingItemFillPrecision.Half;
ratingControl1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
```

## Hyperlink

| Editor | UX |
|---|---|
| **`HyperLinkEdit`** | Displays a clickable link with `EditValue` as URL or display text. |

```csharp
hyperLinkEdit1.Text = "https://docs.devexpress.com";
hyperLinkEdit1.OpenLink += (s, e) => { /* override navigation */ };
```

## Breadcrumb

| Editor | UX |
|---|---|
| **`BreadCrumbEdit`** | Explorer-style hierarchical path navigator with provider-supplied node tree. |

Set `Properties.RootNode` and handle `Properties.QueryChildNodes` (or assign a `BreadCrumbProvider`).

## Token Editor

`TokenEdit` is a text editor that transforms validated substrings into tokens. Two modes:

- **Free** — users type any text; on validation success the text becomes a token.
- **Predefined** — users pick tokens from a fixed list.

```csharp
tokenEdit1.Properties.EditMode = TokenEditMode.TokenList;
tokenEdit1.Properties.Tokens.AddRange(new[] {
    new TokenEditToken("Urgent", 1),
    new TokenEditToken("Review", 2)
});
```

## Sparkline

| Editor | Stores |
|---|---|
| **`SparklineEdit`** | Small inline chart bound to a numeric collection (`EditValue` is the collection). |

## Generic Popup

| Editor | UX |
|---|---|
| **`PopupContainerEdit`** | Text box + drop-down panel that hosts **any** WinForms controls (a `PopupContainerControl` you author). The base case when no built-in dropdown fits. |

## Editor → Repository-Item Map

Every editor above has the same-named repository item with `RepositoryItem` prefix:

| Editor | Repository item |
|---|---|
| `TextEdit` | `RepositoryItemTextEdit` |
| `MemoEdit` | `RepositoryItemMemoEdit` |
| `MemoExEdit` | `RepositoryItemMemoExEdit` |
| `ButtonEdit` | `RepositoryItemButtonEdit` |
| `SpinEdit` | `RepositoryItemSpinEdit` |
| `CalcEdit` | `RepositoryItemCalcEdit` |
| `DateEdit` | `RepositoryItemDateEdit` |
| `TimeEdit` | `RepositoryItemTimeEdit` |
| `TimeSpanEdit` | `RepositoryItemTimeSpanEdit` |
| `DateTimeOffsetEdit` | `RepositoryItemDateTimeOffsetEdit` |
| `CheckEdit` | `RepositoryItemCheckEdit` |
| `ToggleSwitch` | `RepositoryItemToggleSwitch` |
| `RadioGroup` | `RepositoryItemRadioGroup` |
| `ComboBoxEdit` | `RepositoryItemComboBox` |
| `ImageComboBoxEdit` | `RepositoryItemImageComboBox` |
| `CheckedComboBoxEdit` | `RepositoryItemCheckedComboBoxEdit` |
| `FontEdit` | `RepositoryItemFontEdit` |
| `MRUEdit` | `RepositoryItemMRUEdit` |
| `LookUpEdit` | `RepositoryItemLookUpEdit` |
| `GridLookUpEdit` | `RepositoryItemGridLookUpEdit` |
| `SearchLookUpEdit` | `RepositoryItemSearchLookUpEdit` |
| `TreeListLookUpEdit` | `RepositoryItemTreeListLookUpEdit` |
| `ColorEdit` | `RepositoryItemColorEdit` |
| `ColorPickEdit` | `RepositoryItemColorPickEdit` |
| `PopupGalleryEdit` | `RepositoryItemPopupGalleryEdit` |
| `PopupContainerEdit` | `RepositoryItemPopupContainerEdit` |
| `BreadCrumbEdit` | `RepositoryItemBreadCrumbEdit` |
| `HyperLinkEdit` | `RepositoryItemHyperLinkEdit` |
| `PictureEdit` | `RepositoryItemPictureEdit` |
| `ImageEdit` | `RepositoryItemImageEdit` |
| `ProgressBarControl` | `RepositoryItemProgressBar` |
| `MarqueeProgressBarControl` | `RepositoryItemMarqueeProgressBar` |
| `TrackBarControl` | `RepositoryItemTrackBar` |
| `RangeTrackBarControl` | `RepositoryItemRangeTrackBar` |
| `ZoomTrackBarControl` | `RepositoryItemZoomTrackBar` |
| `RatingControl` | `RepositoryItemRatingControl` |
| `TokenEdit` | `RepositoryItemTokenEdit` |
| `SparklineEdit` | `RepositoryItemSparklineEdit` |

To embed any editor in a Data Grid column or Ribbon `BarEditItem`, create the matching repository item, add it to `gridControl.RepositoryItems` (or `barManager.RepositoryItems`), and assign it.

## Quick Decision Table

| Need | Pick |
|---|---|
| Single-line text | `TextEdit` |
| Multi-line text | `MemoEdit` (inline) or `MemoExEdit` (single line + popup) |
| Integer / decimal | `SpinEdit` (or `CalcEdit` with calculator) |
| Date | `DateEdit` |
| Time | `TimeEdit` |
| Duration | `TimeSpanEdit` |
| Boolean | `CheckEdit` (form), `ToggleSwitch` (setting) |
| One of 2–6 options | `RadioGroup` |
| One of many (small list) | `ComboBoxEdit` |
| One of many (huge list) | `SearchLookUpEdit` |
| Hierarchical pick | `TreeListLookUpEdit` |
| Multiple from list | `CheckedComboBoxEdit` |
| Tags / chips | `TokenEdit` |
| Color | `ColorPickEdit` or `ColorEdit` |
| File / folder path | `ButtonEdit` + custom dialog, or `BrowsePathEdit` (from `DevExpress.Win.Dialogs`) |
| Image | `PictureEdit` (display + simple), `ImageEdit` (editable popup) |
| Progress | `ProgressBarControl` (known) / `MarqueeProgressBarControl` (unknown) |
| Slider | `TrackBarControl`, `RangeTrackBarControl`, `ZoomTrackBarControl`, `RatingControl` |
| URL | `HyperLinkEdit` |
| Path / navigation | `BreadCrumbEdit` |
| Sparkline | `SparklineEdit` |
| Custom popup | `PopupContainerEdit` |

## Common Issues

- **`LookUpEdit` shows raw key instead of display text**: `DisplayMember`/`ValueMember` reversed, or `DataSource` not assigned before `EditValue` is set.
- **`ComboBoxEdit.EditValue` is `null` when an item is selected**: by default `ComboBoxEdit` stores the selected string; switch to `ImageComboBoxEdit` if values must differ from captions.
- **`DateEdit.EditValue == DateTime.MinValue` not treated as "empty"**: set `Properties.AllowNullInput = DefaultBoolean.True` and `Properties.NullDate = DateTime.MinValue`.
- **`RatingControl.EditValue` is `0` instead of half value**: set `Properties.FillPrecision = RatingItemFillPrecision.Half;` (`RatingItemFillPrecision` is in `DevExpress.XtraEditors`).

## Source Material

- `articles/controls-and-libraries/editors-and-simple-controls/included-controls-and-components.md` (`xref:WindowsForms.401381`).
- `articles/controls-and-libraries/editors-and-simple-controls/lookup-editors.md` (`xref:WindowsForms.116008`).
- `api/DevExpress.XtraEditors.TextEdit.yml`.
- `api/DevExpress.XtraEditors.ButtonEdit.yml`.
- `api/DevExpress.XtraEditors.PopupBaseEdit.yml`.
- `api/DevExpress.XtraEditors.LookUpEdit.yml`.
- `api/DevExpress.XtraEditors.BaseSpinEdit.yml`.
- `api/DevExpress.XtraEditors.DateEdit.yml`.
- `api/DevExpress.XtraEditors.CheckEdit.yml`.
- `api/DevExpress.XtraEditors.ToggleSwitch.yml`.
- `api/DevExpress.XtraEditors.RadioGroup.yml`.
- `api/DevExpress.XtraEditors.TokenEdit.yml`.
