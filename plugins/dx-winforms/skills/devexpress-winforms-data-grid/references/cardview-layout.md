# Card View — Card Layout and Settings

This reference covers the two card-shaped views inside `GridControl`:

- **`CardView`** — classic Outlook-style cards arranged down then across, with caption-and-fields-stacked layout.
- **`LayoutView`** — more flexible cards based on a `TemplateCard` (a layout declared in the designer file), supporting `Carousel` and `MultiColumn` modes.

Pick `CardView` for the simple "field caption + value" stack. Pick `LayoutView` when you need an arbitrary layout (image, multi-line text, grouped sections, embedded controls).

Most cross-view features (data binding, columns, filtering, sorting, conditional formatting, validation, export, selection, layout persistence) are documented in the matching topic and apply identically to `CardView` / `LayoutView`.

## When to Use This Reference

- Switching a `GridControl` from `GridView` to `CardView` or `LayoutView`.
- Sizing cards (`CardWidth`, `MaximumCardRows`, `MaximumCardColumns`, `CardMinSize`).
- Customizing card captions, borders, expand buttons.
- Choosing between `CardView` and `LayoutView`.

## `CardView` Setup

```csharp
var grid = new GridControl { Dock = DockStyle.Fill };
var card = new CardView(grid);
grid.ViewCollection.Add(card);
grid.MainView = card;
grid.DataSource = LoadEmployees();
```

The view auto-creates fields from data-source columns. Each `GridColumn` becomes a stacked row inside every card: caption on the left, value on the right.

### Caption

```csharp
card.OptionsView.ShowCardCaption  = true;
card.CardCaptionFormat            = "{0} {LastName}";   // {0} = ordinal, {Name} = field
card.OptionsView.ShowQuickCustomizeButton = false;
```

Supply icons in the caption via the `CustomCardCaptionImage` event.

### Field captions and values

```csharp
card.OptionsView.ShowFieldCaptions = true;
card.Appearance.FieldCaption.Font  = new Font("Segoe UI", 8, FontStyle.Bold);
card.Appearance.FieldValue.ForeColor = Color.DimGray;
```

To hide a field per card (conditional), assign a read-only repository item via `CustomRowCellEdit` and pre-empt drawing in `CustomDrawCardField`.

### Layout — rows and columns

```csharp
card.OptionsBehavior.AutoHorzWidth   = true;     // stretch cards horizontally to fill the view
card.CardWidth                       = 240;       // default 200
card.MaximumCardColumns              = 4;
card.MaximumCardRows                 = 2;         // 0 means unlimited (the typical case)
card.OptionsView.ShowEmptyFields     = false;     // hide rows whose value is null/empty
```

- **`AutoHorzWidth = true`** + `MaximumCardColumns` produces a flexible flow where cards expand horizontally to fill the available view width, limited by the maximum column count.
- **`AutoHorzWidth = false`** keeps cards at `CardWidth` and shows a horizontal scrollbar when needed.

### Expand / collapse buttons

```csharp
card.OptionsView.ShowCardExpandButton = true;
card.OptionsBehavior.AllowExpandCollapse = true;
card.ExpandAll();
card.CollapseAll();
card.SetCardCollapsed(rowHandle, true);
```

### Card separators and scrolling

```csharp
card.OptionsView.ShowLines           = true;
card.VertScrollVisibility            = ScrollVisibility.Auto;
```

`CardView` shows up/down scroll arrows inside a card when content overflows.

## `LayoutView` Setup

```csharp
var lv = new LayoutView(grid);
grid.ViewCollection.Add(lv);
grid.MainView = lv;
grid.DataSource = LoadEmployees();

lv.OptionsView.ViewMode = LayoutViewMode.MultiColumn;     // SingleRecord / MultiColumn / Carousel
lv.OptionsView.ShowCardCaption = true;
lv.CardMinSize = new Size(220, 180);
```

### Customize the template card

Open the Designer → right-click the view → **Edit Template Card…** Drag fields, group panes, image controls, embedded editors into a layout that matches your design. Every card is rendered with this layout filled in from the row's data.

```csharp
lv.TemplateCard = layoutViewTemplateCard1;     // assigned by the designer
```

### Field visibility, location, alignment

`LayoutView`'s `LayoutViewField`s mirror the layout `LayoutControl` API — `TextVisible`, `TextLocation`, `TextAlignMode`, `SizeConstraintsType` per field.

```csharp
lv.CardFields["Name"].TextLocation = LayoutGroupItemTextLocation.Top;
lv.CardFields["Notes"].TextVisible = false;
```

### Modes

| Mode | UX |
|---|---|
| `SingleRecord` | One card per row of the view. |
| `MultiColumn` | Cards arranged down then across, like `CardView`. |
| `Carousel` | Carousel-style 3D rotation; user navigates one card at a time. |

```csharp
lv.OptionsView.ViewMode = LayoutViewMode.Carousel;
lv.OptionsCarouselMode.PageItemCount = 5;
```

### Card minimum size

```csharp
lv.CardMinSize = new Size(220, 180);
```

Cards grow as needed to fit their content, but never below `CardMinSize`.

## Choose `CardView` vs `LayoutView`

| Need | Pick |
|---|---|
| Simple "field: value" stack | `CardView` |
| Arbitrary card layout, image + multi-section | `LayoutView` |
| Carousel UX | `LayoutView` |
| Resizable cards by user | `CardView` (`AllowSize` on splitters) |
| Tightly controlled cell rendering | `LayoutView` |
| Marquee multi-select | `LayoutView` (CardView supports Ctrl/Shift click; LayoutView adds marquee) |

## Caption + Field Captions Across Both Views

| Concept | `CardView` | `LayoutView` |
|---|---|---|
| Show card caption | `CardOptionsView.ShowCardCaption` | `LayoutViewOptionsView.ShowCardCaption` |
| Caption format string | `CardView.CardCaptionFormat` | `LayoutView.CardCaptionFormat` |
| Caption icon event | `CardView.CustomCardCaptionImage` | `LayoutView.CustomCardCaptionImage` |
| Show expand button | `CardOptionsView.ShowCardExpandButton` | `LayoutViewOptionsView.ShowCardExpandButton` |
| Show field captions | `CardOptionsView.ShowFieldCaptions` | `LayoutViewOptionsView.ShowFieldCaptions` (via field's `TextVisible`) |
| Auto width | `CardOptionsBehavior.AutoHorzWidth` | n/a (use `ViewMode = MultiColumn`) |

## Selection in Cards

Same multi-select API as `GridView` (see [focus-and-selection.md](focus-and-selection.md)):

```csharp
cardView.OptionsSelection.MultiSelect = true;
foreach (int handle in cardView.GetSelectedRows()) { /* ... */ }
```

`LayoutView` additionally supports marquee (drag-rectangle) selection out of the box.

## Common Issues

- **Cards all share the same height**: `CardView` sizes cards to fit the tallest content per row. To allow per-card heights, switch to `LayoutView` with `CardMinSize`.
- **Card width not respected**: `AutoHorzWidth = true` overrides `CardWidth`. Set it to `false` or use `MaximumCardColumns` to constrain.
- **Caption shows "Record 1"**: default `CardCaptionFormat = "Record N {0}"`. Override with a custom format.
- **`LayoutView` template-card changes lost**: changes to the template at runtime must happen through `lv.TemplateCard` rather than via individual field edits.

## Source Material

- `articles/controls-and-libraries/data-grid/views/card-and-layout-views.md` (`xref:WindowsForms.114638`).
- `articles/controls-and-libraries/data-grid/views/card-and-layout-views/layout-view.md` (`xref:WindowsForms.5787`).
- `articles/controls-and-libraries/data-grid/visual-elements/card-view-elements/card.md` (`xref:WindowsForms.523`).
- `articles/controls-and-libraries/data-grid/visual-elements/card-view-elements/card-caption.md` (`xref:WindowsForms.524`).
- `articles/controls-and-libraries/data-grid/visual-elements/layout-view-elements/card.md` (`xref:WindowsForms.3441`).
- `articles/controls-and-libraries/data-grid/visual-elements/layout-view-elements/card-caption.md` (`xref:WindowsForms.3442`).
- `api/DevExpress.XtraGrid.Views.Card.CardView.yml`.
- `api/DevExpress.XtraGrid.Views.Layout.LayoutView.yml`.
- `api/DevExpress.XtraGrid.Views.Card.CardView.CardWidth.yml`.
