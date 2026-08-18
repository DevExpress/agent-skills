# Non-BaseEdit Simple Controls

This reference covers DevExpress controls in `DevExpress.XtraEditors` that do **not** inherit from `BaseEdit` and therefore do **not** expose an `EditValue` property. They are used for display, actions, navigation, container layout, and filter UI — not for editing a typed value.

For editors that do edit values, see [editor-variants.md](editor-variants.md).

## When to Use This Reference

- Adding a label, button, list box, or navigator to a form.
- Picking between `SimpleButton` / `DropDownButton` / `CheckButton`.
- Choosing `ListBoxControl` vs `CheckedListBoxControl` vs `ImageListBoxControl`.
- Needing a search-as-you-type box bound to another control (`SearchControl`).
- Showing a loading overlay (`ProgressPanel`).
- Hosting record-navigation buttons (`DataNavigator`, `ControlNavigator`).
- Showing a built-in filter editor (`FilterControl`).
- Replacing OS file dialogs with skinned ones (`XtraOpenFileDialog`, `XtraSaveFileDialog`, `XtraFolderBrowserDialog`).

## Buttons

| Control | Behavior | When to use |
|---|---|---|
| **`SimpleButton`** | Standard push button. Text + image, skin-aware. `Click` event. | Submit, OK/Cancel, Save, navigation triggers. |
| **`DropDownButton`** | Button with an arrow that shows an attached `PopupMenu` or `PopupControlContainer`. Can prevent focus on click. | Split-button UX, menu launchers. |
| **`CheckButton`** | Two-state button (elevated / depressed); can join a radio group via `GroupIndex`. | Toggle filters, mutually exclusive view modes. |

```csharp
using DevExpress.XtraEditors;

var btn = new SimpleButton {
    Text   = "Save",
    Image  = saveSvgImage,                            // or ImageOptions.SvgImage
    Width  = 100,
    Height = 28
};
btn.Click += (_, _) => SaveCurrent();

var split = new DropDownButton { Text = "Export", DropDownControl = exportPopup };
```

Properties shared across all three: `ImageOptions` (SVG + raster + alignment), `AllowFocus`, `Appearance`, `AutoWidthInLayoutControl`, `PaintStyle` (Default, Light, Strip).

## Labels

| Control | Notes |
|---|---|
| **`LabelControl`** | Plain skinned label. Supports multi-line, images, **HTML tags** in `Text` (when `AllowHtmlString = true`). Set `AutoSizeMode` for auto-fit. |
| **`HyperlinkLabelControl`** | Label whose text (or a span of it) is a clickable hyperlink. HTML supported. `HyperlinkClick` event. |

```csharp
var lbl = new LabelControl {
    Text = "<b>Warning</b>: <color=red>Disk full</color>",
    AllowHtmlString = true,
    AutoSizeMode    = LabelAutoSizeMode.Vertical
};

var link = new HyperlinkLabelControl {
    Text = "Read <href=https://docs>the docs</href> for details.",
    AllowHtmlString = true
};
link.HyperlinkClick += (s, e) => {
    // e.Link is the (possibly user-controlled) href — validate it before launching.
    // Allowlist http/https so a crafted link can't open arbitrary URI schemes or local files.
    if (Uri.TryCreate(e.Link, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
        Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
    }
};
```

## List Boxes

| Control | UX | Notes |
|---|---|---|
| **`ListBoxControl`** | Skinned list. Data-bindable (`DataSource` + `DisplayMember` / `ValueMember`). Supports HTML in items, item images, hot-tracking, multi-select. | General-purpose selection list. |
| **`ImageListBoxControl`** | Same as `ListBoxControl` plus per-item image (from `ImageList` or `SvgImageCollection`). | When list items must show icons. |
| **`CheckedListBoxControl`** | List where each item has a checkbox. Items can be `Checked`, `Unchecked`, or `Indeterminate` (grayed). Data-bindable. | Multi-select list with persistent checkmarks (e.g., role assignment). |

These are **not** `BaseEdit` descendants — there is no single `EditValue`. To read selections, use `SelectedItem`, `SelectedValue`, `SelectedItems`, or `CheckedItems`/`CheckedIndices`.

```csharp
listBoxControl1.DataSource    = customers;
listBoxControl1.DisplayMember = nameof(Customer.Name);
listBoxControl1.ValueMember   = nameof(Customer.Id);
listBoxControl1.SelectionMode = SelectionMode.MultiExtended;

checkedListBoxControl1.Items.AddRange(new[] {
    new CheckedListBoxItem("Read", true),
    new CheckedListBoxItem("Write", false),
    new CheckedListBoxItem("Delete", false)
});
```

## Search and Filter

| Control | Purpose |
|---|---|
| **`SearchControl`** | Stand-alone search box that filters another control (Data Grid, TreeList, ListBox, etc.) via the `Client` property. Same Find-Panel syntax users see in grids. |
| **`FilterControl`** | Visual + text filter editor. Pairs with a `SourceControl` (typically a Grid Control) to build complex criteria. |

```csharp
searchControl1.Client = gridView1;     // grid filters as the user types
filterControl1.SourceControl = gridControl1;
filterControl1.FilterCriteria = CriteriaOperator.Parse("[Status] = 'Open'");
```

## Navigation

| Control | UX | Notes |
|---|---|---|
| **`DataNavigator`** | First/Prev/Next/Last/Append/Remove buttons bound to a `DataSource`. | Old-school data-source navigator (BindingNavigator equivalent). |
| **`ControlNavigator`** | Same buttons but the data source is inferred from a `NavigatableControl` (a `GridView`, `LayoutView`, etc.). | Grid navigation. |
| **`NavigatorBase`** | Abstract base. | — |

```csharp
controlNavigator1.NavigatableControl = gridControl1;
controlNavigator1.Buttons.Append.Visible = false;     // hide buttons selectively
```

## Loading Overlays

| Control | UX |
|---|---|
| **`ProgressPanel`** | Animated marquee panel with caption ("Loading…", "Saving…"). Add it to a form/panel, set `Visible = true` during long operations. |
| **`WaitForm`** / **`SplashScreen`** *(separate skill)* | Modal overlays. |

## Range Control

| Control | UX |
|---|---|
| **`RangeControl`** | Generic range selection — pair with `ChartRangeControlClient`, `GridRangeControlClient`, or custom client. |
| **`RangeTrackBarControl`** *(BaseEdit)* | Simpler two-thumb numeric range. |

`RangeControl` is **not** a `BaseEdit` — it is a complex composite control with a `Client` data view. Use it for time-range pickers tied to charts.

## Calendar

| Control | Notes |
|---|---|
| **`CalendarControl`** (in `DevExpress.XtraEditors.Controls`) | Standalone month/week/day calendar grid; same engine as the `DateEdit` dropdown. Useful when the calendar is the primary UI rather than a dropdown. |

## Image Slider

| Control | UX |
|---|---|
| **`ImageSlider`** | Carousel of images with prev/next buttons. Bind an `ImageCollection` and let users browse. Useful in product/avatar lists in the Data Grid. |

## File / Folder Dialogs

`DevExpress.Win.Dialogs` ships skinned replacements for the OS file dialogs:

| Class | OS equivalent |
|---|---|
| **`XtraOpenFileDialog`** | `OpenFileDialog`. |
| **`XtraSaveFileDialog`** | `SaveFileDialog`. |
| **`XtraFolderBrowserDialog`** | `FolderBrowserDialog`. |

```csharp
using var dlg = new XtraOpenFileDialog {
    Filter = "JSON|*.json|All|*.*",
    Title  = "Open document"
};
if (dlg.ShowDialog() == DialogResult.OK) Open(dlg.FileName);
```

These dialogs are **components**, not controls — create them in code and call `ShowDialog()`.

There is also `BrowsePathEdit` — a `ButtonEdit` descendant (which **is** a `BaseEdit`) that combines a text box with a built-in browse button. Use it when the path should be both visible and editable; use the dialogs above when you only need to ask once.

## Utility Components

| Component | Purpose |
|---|---|
| **`DefaultLookAndFeel`** | Sets a skin per-form via a drop-on component. |
| **`SvgImageCollection`** | Shared collection of SVG icons used by editor buttons, list items, etc. |
| **`ImageCollection`** | Shared raster image collection. |
| **`PersistenceBehavior`** *(via `BehaviorManager`)* | Saves/restores form-wide layout including editors. |
| **`DXMouseEventHandler`** / **`DXMouseEventArgs.GetMouseArgs`** | Mark mouse events as handled to short-circuit default behavior. |

## Why It Matters — `EditValue` vs No `EditValue`

A `BaseEdit` exposes a single bindable `EditValue` (`object`) that round-trips through `INotifyPropertyChanged` / `IBindingList`. Non-`BaseEdit` controls do not — instead:

- Labels expose `Text` (one-way display only).
- Buttons expose `Click` (action, no value).
- List boxes expose `SelectedItem` / `SelectedValue` / `CheckedItems` (collection or current selection).
- Navigators expose buttons that mutate a bound source through `BindingManagerBase`.
- Filter / search controls expose `FilterCriteria` / search text.
- Progress panels expose `Description` + a `Show()` / `Hide()` flow.

Knowing this distinction prevents the common bug of trying `simpleButton.DataBindings.Add("EditValue", source, "X")` — there is no such property; bind `Enabled` or `Text` instead.

## Common Issues

- **`LabelControl.Text` ignores HTML tags**: set `AllowHtmlString = true` (default depends on version).
- **`CheckedListBoxControl.CheckedItems` empty after restore**: the control populates items synchronously but checks asynchronously — re-apply checks in the layout-restore handler.
- **`SearchControl.Client` not filtering**: the target control's "find panel" must be supported. Grids / TreeList work out of the box; for arbitrary controls subscribe to `SearchControl.QueryIsSearchColumn`.
- **`SimpleButton.DialogResult` not closing the form**: set `DialogResult = DialogResult.OK` and the form's `AcceptButton`/`CancelButton`.
- **`DropDownButton` does not show the menu**: set `DropDownControl` to a `PopupMenu` or `PopupControlContainer` — not a `ContextMenuStrip`.

## Source Material

- `articles/controls-and-libraries/editors-and-simple-controls/included-controls-and-components.md` — Included controls (`xref:WindowsForms.401381`).
- `articles/controls-and-libraries/editors-and-simple-controls/controls-as-on-toolbox.md` — Toolbox map (`xref:WindowsForms.115864`).
- `api/DevExpress.XtraEditors.SimpleButton.yml`.
- `api/DevExpress.XtraEditors.LabelControl.yml`.
- `api/DevExpress.XtraEditors.ListBoxControl.yml`.
- `api/DevExpress.XtraEditors.CheckedListBoxControl.yml`.
- `api/DevExpress.XtraEditors.SearchControl.yml`.
- `api/DevExpress.XtraEditors.FilterControl.yml`.
- `api/DevExpress.XtraEditors.ProgressPanel.yml`.
- `api/DevExpress.XtraEditors.DataNavigator.yml`.
- `api/DevExpress.XtraEditors.ControlNavigator.yml`.
- `api/DevExpress.XtraEditors.XtraOpenFileDialog.yml`.
