# Buttons in Editors

This reference covers the `EditorButton` infrastructure shared across every `ButtonEdit` descendant — `ButtonEdit`, `ComboBoxEdit`, `LookUpEdit`, `SpinEdit`, `DateEdit`, `TimeEdit`, `CalcEdit`, `FontEdit`, `MRUEdit`, `BreadCrumbEdit`, `MemoExEdit`, `HyperLinkEdit`, `ColorEdit`, `ColorPickEdit`, `BlobBaseEdit`, `ImageEdit`, `PopupBaseEdit`, `PopupContainerEdit`, `PopupGalleryEdit`, `LookUpEditBase`, `GridLookUpEdit`, `SearchLookUpEdit`, `TreeListLookUpEdit`, `BrowsePathEdit`. Any editor in that chain exposes a `Properties.Buttons` collection of `EditorButton` objects with shared API for kinds (predefined glyphs), images, captions, alignment, shortcuts, click handling, and the special `TextEditStyle` modes.

## When to Use This Reference

- Adding or removing buttons in a `ButtonEdit`-family editor.
- Picking a predefined `ButtonPredefines.Kind` for a button (Ellipsis, Search, Plus, Minus, Clear, Delete, OK, Close, Up, Down, Left, Right, Combo, DropDown, …).
- Replacing the default icon with a custom SVG / raster image.
- Showing a caption next to the image.
- Aligning a button to the left edge.
- Handling clicks and identifying which button was clicked.
- Hiding the text box so the control is button-only.
- Defining keyboard shortcuts for individual buttons.

## Concepts

- **`EditorButton`** (`DevExpress.XtraEditors.Controls`) — one button. Has `Kind`, `Caption`, `Width`, `Visible`, `Enabled`, `IsLeft`, `IsDefaultButton`, `Shortcut`, `ToolTip`, `Tag`, `ImageOptions`.
- **`EditorButtonCollection`** — the `Properties.Buttons` collection.
- **`ButtonPredefines`** — enum of predefined glyph kinds (`Glyph`, `Ellipsis`, `OK`, `Close`, `Delete`, `Clear`, `Plus`, `Minus`, `Up`, `Down`, `Left`, `Right`, `Search`, `Combo`, `DropDown`, `Redo`, `Undo`, `SpinUp`, `SpinDown`, `SpinLeft`, `SpinRight`, `Separator`, …).
- **`EditorButtonImageOptions`** — `SvgImage`, `Image`, `SvgImageSize`, `Location`, `ImageToTextAlignment`, `ImageToTextIndent`.
- **`RepositoryItemButtonEdit.ButtonClick`** / **`.ButtonPressed`** — click events.
- **`RepositoryItemButtonEdit.TextEditStyle`** — `Standard`, `HideTextEditor`, `DisableTextEditor`.

## The Default Button

A newly created `ButtonEdit` has **one** button — `Kind = Ellipsis`. Lookup-family editors have a default **DropDown** button instead. To customize, modify `Buttons[0]` or clear the collection.

```csharp
// Inspect the default
EditorButton b = buttonEdit1.Properties.Buttons[0];
// b.Kind == ButtonPredefines.Ellipsis
```

## Add a Button

```csharp
using DevExpress.XtraEditors.Controls;

// Append a predefined Clear button
buttonEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Clear));

// Replace the default with two custom-kind buttons
buttonEdit1.Properties.Buttons.Clear();
buttonEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Search)   { Tag = "find"  });
buttonEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Ellipsis) { Tag = "more"  });
```

When adding multiple buttons in code, wrap the changes in `BeginInit()` / `EndInit()` for performance:

```csharp
buttonEdit1.Properties.BeginInit();
buttonEdit1.Properties.Buttons.Clear();
buttonEdit1.Properties.Buttons.AddRange(new[] {
    new EditorButton(ButtonPredefines.Plus),
    new EditorButton(ButtonPredefines.Minus)
});
buttonEdit1.Properties.EndInit();
```

## Button Click

There is no `Click` event on `EditorButton` itself — all buttons share the editor's `ButtonClick` event. Identify the button via `e.Button.Kind`, `e.Button.Tag`, or the collection index.

```csharp
buttonEdit1.ButtonClick += (sender, e) => {
    var edit = (ButtonEdit)sender;
    switch (e.Button.Kind) {
        case ButtonPredefines.Ellipsis: OpenDialog(edit); break;
        case ButtonPredefines.Clear:    edit.Clear();      break;
        case ButtonPredefines.Search:   RunSearch(edit);   break;
    }
    // or by Tag
    if ((string)e.Button.Tag == "find") RunSearch(edit);
};
```

For grid-embedded editors, the equivalent is `Properties.ButtonClick`:

```csharp
var ri = new RepositoryItemButtonEdit();
ri.Buttons[0].Kind = ButtonPredefines.OK;
ri.ButtonClick += (s, e) => { /* … */ };
gridControl.RepositoryItems.Add(ri);
gridView.Columns["Action"].ColumnEdit = ri;
```

The `ButtonPressed` event fires on `MouseDown` (before commit) and is rarely needed unless implementing repeat-press behavior.

## Predefined Kinds

`ButtonPredefines` enum (subset — see the API reference for the full list):

| Kind | Glyph |
|---|---|
| `Glyph` | (custom image via `ImageOptions`) |
| `Ellipsis` | `…` |
| `OK` | check |
| `Close` | × |
| `Delete` | trash / × |
| `Clear` | broom / × |
| `Plus` | `+` |
| `Minus` | `−` |
| `Up` / `Down` | arrows |
| `Left` / `Right` | arrows |
| `Search` | magnifier |
| `Combo` / `DropDown` | combo arrow |
| `SpinUp` / `SpinDown` | spin arrows (vertical) |
| `SpinLeft` / `SpinRight` | spin arrows (horizontal) |
| `Redo` / `Undo` | redo / undo |
| `Separator` | vertical separator |

When `Kind` is one of `Glyph`, `Ellipsis`, `Delete`, `OK`, `Plus`, `Minus`, `Redo`, `Undo`, `DropDown` the button can display **both** an image *and* a caption (set `ImageOptions.Location` to anything except `MiddleCenter`).

## Custom Image

```csharp
var send = new EditorButton(ButtonPredefines.Glyph);
send.ImageOptions.SvgImage     = SvgImage.FromFile("send.svg");
send.ImageOptions.SvgImageSize = new Size(16, 16);
send.ImageOptions.Location     = ImageLocation.MiddleCenter;
buttonEdit1.Properties.Buttons.Add(send);

// Raster image alternative
send.ImageOptions.Image = Properties.Resources.Send;
```

For SVG, prefer a shared `SvgImageCollection` to keep memory usage low:

```csharp
send.ImageOptions.SvgImage     = svgImageCollection1["send"];
send.ImageOptions.SvgImageSize = new Size(16, 16);
```

## Caption + Image

```csharp
var add = buttonEdit1.Properties.Buttons[0];
add.Kind                     = ButtonPredefines.Glyph;
add.Caption                  = "Add Employee";
add.ImageOptions.SvgImage    = svgImageCollection1["plus"];
add.ImageOptions.Location    = ImageLocation.MiddleLeft;
add.Width                    = 140;
```

A captioned button is wider than a glyph-only button — set `Width` explicitly or the editor stretches the text box.

## Alignment (Left / Right)

By default, buttons sit on the **right** edge of the text box. To pin a button to the **left**:

```csharp
var leftBtn = new EditorButton(ButtonPredefines.Search) { IsLeft = true };
buttonEdit1.Properties.Buttons.Add(leftBtn);
```

Multiple left buttons stack from the left edge in collection order; right buttons stack from the right edge in collection order.

## Keyboard Shortcut

Each button can have a `Shortcut`:

```csharp
buttonEdit1.Properties.Buttons[0].Shortcut = new KeyShortcut(Keys.F4);
```

Pressing the shortcut fires `ButtonClick` with that button as `e.Button`.

## Default Button

```csharp
buttonEdit1.Properties.Buttons[0].IsDefaultButton = true;
```

The default button responds to `Enter` when the editor has focus (without moving focus elsewhere). Useful for a single primary action like *Apply* / *Submit*.

## Visibility and Enabled State

```csharp
buttonEdit1.Properties.Buttons[1].Visible = false;
buttonEdit1.Properties.Buttons[0].Enabled = !string.IsNullOrEmpty((string)buttonEdit1.EditValue);
```

Tip — handle `EditValueChanged` to toggle a button based on the current value (e.g., disable Clear when empty).

## ToolTip

```csharp
buttonEdit1.Properties.Buttons[0].ToolTip = "Browse for a file";
buttonEdit1.Properties.Buttons[0].SuperTip = new DevExpress.Utils.SuperToolTip();
```

The editor uses the host form's tooltip controller; no separate `ToolTip` component needed.

## `TextEditStyle` — Hide or Disable the Text Box

`RepositoryItemButtonEdit.TextEditStyle` controls the text-box portion:

| Value | Behavior |
|---|---|
| `Standard` *(default)* | Text box is editable. |
| `HideTextEditor` | Text box hidden — control becomes button-only. |
| `DisableTextEditor` | Text box visible but read-only (no text edit / selection). |

```csharp
// Button-only editor
buttonEdit1.Properties.TextEditStyle = TextEditStyles.HideTextEditor;
buttonEdit1.Properties.Buttons[0].Caption       = "Pick…";
buttonEdit1.Properties.Buttons[0].Kind          = ButtonPredefines.Glyph;
buttonEdit1.Properties.Buttons[0].ImageOptions.Location = ImageLocation.MiddleLeft;
buttonEdit1.Properties.Buttons[0].Width         = 120;
```

Use `HideTextEditor` for "command-button-with-popup" UX (e.g., the gear/settings dropdown). Use `DisableTextEditor` to force users to pick via buttons/dropdown only.

## Button in Specific Editors

### `ComboBoxEdit` / `LookUpEdit` / `FontEdit` / `MRUEdit`

Default button is `DropDown`. Replace via the same `Properties.Buttons` collection. To add a Clear button next to the dropdown:

```csharp
lookUpEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
lookUpEdit1.ButtonClick += (s, e) => {
    if (e.Button.Kind == ButtonPredefines.Clear) ((LookUpEdit)s).EditValue = null;
};
```

### `SpinEdit`

Default buttons are `SpinUp` + `SpinDown` (vertical, right-aligned). Switch to horizontal:

```csharp
spinEdit1.Properties.SpinStyle = DevExpress.XtraEditors.Controls.SpinStyles.Horizontal;
```

Add an extra Clear button:

```csharp
spinEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Clear));
```

### `DateEdit` / `TimeEdit` / `CalcEdit` / `ColorPickEdit` / `PopupGalleryEdit` / `PopupContainerEdit`

Default button is the popup-trigger (`DropDown` or specific glyph). Add side buttons to inject custom actions:

```csharp
dateEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph) {
    Caption = "Today",
    ImageOptions = { SvgImage = svgImageCollection1["today"] }
});
dateEdit1.ButtonClick += (s, e) => {
    if (e.Button.Caption == "Today") dateEdit1.EditValue = DateTime.Today;
};
```

### `BrowsePathEdit`

`BrowsePathEdit` is a `ButtonEdit` descendant pre-wired with a folder/file picker; the default button opens an `XtraFolderBrowserDialog` (or file dialog depending on `Properties.SelectionMode`). Replace the default:

```csharp
browsePathEdit1.Properties.Buttons.Clear();
browsePathEdit1.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph) {
    Caption = "Browse…",
    ImageOptions = { SvgImage = svgImageCollection1["folder"] }
});
```

## Tooltips and Animation

`EditorButton` itself does not have a `Click` event but participates in:

- `ButtonClick` — single click.
- `ButtonPressed` — `MouseDown`.
- `Properties.LeftButtonClick` / `RightButtonClick` *(internal)* — rarely used.

## Show / Hide Buttons on Cell Hover (Grid embedding)

A common pattern: show editor buttons only when the cell is hot-tracked. See the `winforms-grid-show-editor-buttons-on-cell-hover` DevExpress example — handle `GridView.CustomDrawCell` and `CustomDrawEmptyForeground` to compose buttons over cells.

## Custom Button Position via `Width`

`EditorButton.Width = 0` makes the button measure its caption + image automatically. Set an explicit width to align multiple buttons or reserve space:

```csharp
buttonEdit1.Properties.Buttons[0].Width = 24;     // glyph only
buttonEdit1.Properties.Buttons[1].Width = 80;     // captioned
```

## Two Custom Buttons Per Side — Worked Example

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

var pathEdit = new ButtonEdit { Width = 360 };
pathEdit.Properties.NullValuePrompt = "Document path";
pathEdit.Properties.Buttons.Clear();
pathEdit.Properties.Buttons.AddRange(new[] {
    new EditorButton(ButtonPredefines.Search)   { IsLeft = true,  Tag = "preview" },
    new EditorButton(ButtonPredefines.Ellipsis) { Tag = "browse",  ToolTip = "Choose a file…" },
    new EditorButton(ButtonPredefines.Clear)    { Tag = "clear",   ToolTip = "Remove path"   }
});
pathEdit.ButtonClick += (s, e) => {
    var edit = (ButtonEdit)s;
    switch ((string)e.Button.Tag) {
        case "preview": PreviewDocument(edit.EditValue as string); break;
        case "browse" :
            using (var dlg = new XtraOpenFileDialog())
                if (dlg.ShowDialog() == DialogResult.OK) edit.EditValue = dlg.FileName;
            break;
        case "clear"  : edit.Clear(); break;
    }
};
pathEdit.EditValueChanged += (_, _) =>
    pathEdit.Properties.Buttons[2].Enabled = !string.IsNullOrEmpty(pathEdit.Text);
```

## Common Issues

- **Custom SVG icon does not appear**: `Kind` is not `Glyph`. Set it to `Glyph` explicitly — only then does `ImageOptions` win over the predefined glyph.
- **Caption invisible when image is centered**: `ImageOptions.Location = MiddleCenter` hides the caption. Use `MiddleLeft`/`MiddleRight`/`Top`/`Bottom`.
- **`ButtonClick` not raised for shortcut**: the editor must have focus for `Shortcut` to dispatch; set a form-wide shortcut via `BarManager` or `RibbonControl` if needed globally.
- **`Tag` is `null` after deserialization**: `EditorButton.Tag` is *not* serialized by `XtraSerializer` by default. Rebuild the button collection in `Form_Load` or rely on `Kind`/`Caption` to identify buttons.
- **Repeated clicks on hold**: editor buttons do not auto-repeat. Use `ButtonPressed` with a timer if you need repeat.
- **Cannot detect which button was pressed in a grid cell**: cast `e.Button` and compare `Kind` / `Tag` — `Properties.Buttons` indexes are still valid inside the grid's `ButtonClick`.

## Source Material

- `articles/controls-and-libraries/editors-and-simple-controls/examples/how-to-create-buttonedit-control-in-code.md` (`xref:WindowsForms.9460`).
- `articles/controls-and-libraries/editors-and-simple-controls/examples/how-to-respond-to-clicking-buttonedits-embedded-buttons.md` (`xref:WindowsForms.9461`).
- `api/DevExpress.XtraEditors.ButtonEdit.yml`.
- `api/DevExpress.XtraEditors.Controls.EditorButton.yml`.
- `api/DevExpress.XtraEditors.Controls.EditorButton.Kind.yml`.
- `api/DevExpress.XtraEditors.Controls.EditorButton.ImageOptions.yml`.
- `api/DevExpress.XtraEditors.Controls.EditorButton.Caption.yml`.
- `api/DevExpress.XtraEditors.Controls.ButtonPredefines.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit.Buttons.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit.ButtonClick.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit.TextEditStyle.yml`.
