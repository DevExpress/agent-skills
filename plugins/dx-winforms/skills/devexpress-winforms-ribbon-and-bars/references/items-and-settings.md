# Items and Settings

This reference covers the items / links architecture used by both Ribbon and Bars, the catalog of `BarItem` types, the core item and link properties (`Caption`, `ImageOptions`, `RibbonStyle`, `PaintStyle`, `ButtonStyle`, `GroupIndex`, `Hint`/`SuperTip`, `ItemShortcut`, `SearchTags`), per-link overrides (`UserCaption`, `UserGlyph`, `UserRibbonStyle`, `BeginGroup`, `UserAlignment`), and data binding via the cross-platform `DevExpress.Mvvm` package (the WinForms-applicable subset of MVVM).

## When to Use This Reference

- Picking the right `BarItem*` type for a command.
- Understanding why `BarItem` is non-visual and `BarItemLink` is the visible thing.
- Configuring caption + image + style of an item.
- Creating one item, displaying it as multiple links with per-link overrides.
- Wiring a `BarItem` to a view-model property + `ICommand` in a WinForms project.

## Items vs Links

A `BarItem` is a non-visual component holding all settings: `Caption`, `ImageOptions`, `Hint`, `ItemClick`, `Enabled`, `Visibility`, `ItemShortcut`, etc. Putting an item on a `Bar` / `RibbonPageGroup` / `Toolbar` / `RibbonStatusBar` creates a **`BarItemLink`** — the actual on-screen control.

You can create multiple links to the same item. Clicking any of them fires the same `ItemClick`. Each link can override the visual style independently (`UserPaintStyle`, `UserRibbonStyle`, `UserCaption`, `UserGlyph`, `UserAlignment`, `BeginGroup`).

```text
                 ┌──────────────────────┐
   item ────────►│ BarButtonItem        │  ← settings live here, ItemClick fires here
                 │   Caption  "Save"    │
                 │   Glyph    save.svg  │
                 │   Shortcut Ctrl+S    │
                 └─────────┬────────────┘
                           │
        ┌──────────────────┼───────────────────┐
        ▼                  ▼                   ▼
   linkInGroup        linkInQAT          linkInStatusBar   ← live, on-screen
   (large icon)       (small icon)       (icon only)
```

Use this to put *Save* in a ribbon page group, in the QAT, in a context menu, and on a status bar — all four buttons share one event handler.

## Item Catalog

| Item | Link | Purpose | Notes |
|---|---|---|---|
| **`BarButtonItem`** | `BarButtonItemLink` | Push, check, dropdown, or check-dropdown button. | `ButtonStyle = Default/Check/DropDown/CheckDropDown`. For dropdowns set `DropDownControl` to a `PopupMenu` or `PopupControlContainer`. Use `GroupIndex` to make multiple checks behave as a radio group. |
| **`BarLargeButtonItem`** | `BarLargeButtonItemLink` | Toolbar-only large button. | In the ribbon, prefer `BarButtonItem` with `RibbonStyle = Large`. |
| **`BarCheckItem`** | `BarCheckItemLink` | Toggle button with `Checked` / `Unchecked` state. | Handle `CheckedChanged`. Combine via `GroupIndex` for radio behavior. |
| **`BarToggleSwitchItem`** | `BarToggleSwitchItemLink` | iOS-style switch. | `CheckedChanged` fires on toggle. |
| **`BarSubItem`** | `BarSubItemLink` | Sub-menu host. | Add children via `AddItem` / `AddItems`. Used for the *File / Edit / View* menus. |
| **`BarEditItem`** | `BarEditItemLink` | Wraps a DevExpress editor (`SpinEdit`, `LookUpEdit`, `ComboBoxEdit`, …) as a bar item. | Set `Edit` to a `RepositoryItem*` and use `EditValue` / `EditWidth`. |
| **`BarStaticItem`** | `BarStaticItemLink` | Non-clickable label / image (status text). | Set `Alignment`. |
| **`BarHeaderItem`** | `BarHeaderItemLink` | Bold header inside a popup menu / Application Menu. | Used as section caption. |
| **Split button** (a `BarButtonItem`) | `BarButtonItemLink` | Button with a side dropdown arrow. | Not a separate type: set `ButtonStyle = BarButtonStyle.DropDown` and a `DropDownControl` on a `BarButtonItem`, or call `BarItems.CreateSplitButton(caption, popupMenu)` (returns a `BarButtonItem`). |
| **`BarMdiChildrenListItem`** | `BarMdiChildrenListItemLink` | Auto-populated list of MDI child windows. | Drop in *Window* menu. |
| **`RibbonGalleryBarItem`** | — | Gallery (tiles) inside a ribbon group. | Configure via `Gallery.ItemImageLayout` / `Gallery.Groups`. |
| **`SkinDropDownButtonItem`** | — | Skin picker. | Drop into a ribbon to let users pick a skin at runtime. |
| **`SkinPaletteDropDownButtonItem`** | — | Skin-palette picker (color variant of the current skin). | Pairs well with `SkinDropDownButtonItem`. |

For galleries, see [ribbon-structure.md](ribbon-structure.md). For the editor wrapper, see [BarEditItem (Embedded Editor)](#bareditem-embedded-editor) below.

## Adding Items in Code

```csharp
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

// Ribbon: ribbon.Items is the manager's item collection
var newCmd = ribbon.Items.CreateButton("New");        // BarButtonItem
newCmd.Id = ribbon.Manager.GetNewItemId();
newCmd.RibbonStyle = RibbonItemStyles.Large;

// Or construct explicitly
var open = new BarButtonItem(ribbon.Manager, "Open");
open.Id = ribbon.Manager.GetNewItemId();

// Bars: bars.Items is the manager's item collection
var save = new BarButtonItem(bars, "Save");
```

`BarItems` exposes these factory helpers: `CreateButton`, `CreateCheckItem`, `CreateMenu`, `CreatePopupMenu`, and `CreateSplitButton`. There are no `CreateSubItem` / `CreateEditItem` / `CreateStaticItem` helpers — construct those types directly:

```csharp
var fileMenu = new BarSubItem(ribbon.Manager, "File");        // sub-menu host
var zoom     = new BarEditItem(ribbon.Manager, spinRepo);     // embedded editor
var ready    = new BarStaticItem { Caption = "Ready" };       // static label
ribbon.Items.Add(fileMenu);
ribbon.Items.Add(zoom);
ribbon.Items.Add(ready);
```

`Id` must be set for every item created in code if you intend to call `SaveLayoutToXml` / `RestoreLayoutFromXml`. Use `manager.GetNewItemId()` to obtain a unique id.

## Adding Links

```csharp
// Ribbon group
ribbonPageGroup1.ItemLinks.Add(newCmd);

// Bar (toolbar / main menu / status bar)
bar1.ItemLinks.Add(newCmd);
bar1.AddItem(newCmd);          // equivalent

// Quick Access Toolbar
ribbon.Toolbar.ItemLinks.Add(newCmd);

// Page header items (next to tab captions)
ribbon.PageHeaderItemLinks.Add(newCmd);

// Form title bar
ribbon.CaptionBarItemLinks.Add(newCmd);

// Status bar
ribbonStatusBar1.ItemLinks.Add(newCmd);
```

`ItemLinks.Add` accepts either a `BarItem` (creates a default link) or a fully constructed `BarItemLink`.

## Core Item Properties

| Property | Purpose |
|---|---|
| `Caption` | Text shown in the link. Affects every link unless overridden via `UserCaption`. |
| `ImageOptions.SvgImage` | SVG glyph from a file or `SvgImageCollection`. Preferred. |
| `ImageOptions.Image` | Raster glyph. Legacy fallback. |
| `ImageOptions.ImageUri.Uri` | DevExpress image library URI — e.g., `"Save;Size16x16"`. |
| `ImageOptions.SmallImages` / `LargeImages` | Backing `ImageCollection`s when using index-based references. |
| `RibbonStyle` (`BarItem.RibbonStyle`) | Bitmask: `Default` / `Large` / `SmallWithText` / `SmallWithoutText` / `All`. Determines the allowed sizes inside a ribbon group; the group decides which one to use based on available width. |
| `PaintStyle` | Inside a `Bar` toolbar — `Default` / `Standard` (text+image) / `CaptionGlyph` / `CaptionInMenu`. Bars and the ribbon paint slightly differently. |
| `ButtonStyle` (`BarButtonItem` only) | `Default` push / `Check` toggle / `DropDown` / `CheckDropDown`. |
| `Down` (`BarBaseButtonItem`) | Pressed state for `Check`/`CheckDropDown` buttons. |
| `GroupIndex` (`BarBaseButtonItem`) | Combines multiple check buttons into a radio group. |
| `DropDownControl` (`BarButtonItem`) | A `PopupMenu` or `PopupControlContainer` for the dropdown (used with `ButtonStyle = DropDown`/`CheckDropDown` to build a split button). |
| `ItemShortcut` | Keyboard shortcut (`BarShortcut(Keys.Control \| Keys.S)`). Form-scoped. |
| `ShortcutKeyDisplayString` | Custom string shown next to the shortcut (e.g., `"Ctrl+S"`). |
| `Hint` | Simple tooltip text. |
| `SuperTip` | Rich tooltip with title + body + image. |
| `SearchTags` | Comma-separated keywords for ribbon Search Menu. |
| `Tag` | Free-form `object` you can attach. |
| `Id` | Unique numeric id used by layout serialization. |
| `Enabled` / `Visibility` (Visibility flags) | Per-item state. |
| `Category` | Optional grouping for the customization form. |

## Setting an Image

```csharp
using DevExpress.Utils.Svg;

// 1) From an SvgImageCollection by key
itemNew.ImageOptions.SvgImage = svgImageCollection1["new"];

// 2) From a file path
itemOpen.ImageOptions.SvgImage = SvgImage.FromFile("icons/open.svg");

// 3) From a stream
itemSave.ImageOptions.SvgImage = SvgImage.FromStream(stream);

// 4) From the DevExpress Image Gallery URI
itemPrint.ImageOptions.ImageUri.Uri = "Print;Size32x32";

// 5) Raster image (legacy)
itemExit.ImageOptions.Image = Properties.Resources.Exit;
```

For ribbon items, also set `RibbonStyle = RibbonItemStyles.Large` if you want the group to display the large variant.

> **Most reliable approach: assign `ImageOptions.SvgImage` from an `SvgImageCollection` (options 1–3).** Create the collection once, add your SVGs, set `ribbon.Images = svgImageCollection1`, then assign by key. This avoids guesswork.
>
> `ImageOptions.ImageUri.Uri` is the DevExpress image-gallery string and its format is easy to get wrong — it is **`"<ImageName>;Size<W>x<H>"`** for a built-in raster image (e.g., `"Print;Size32x32"`, `"Save;Size16x16"`), **not** a file path or an arbitrary `image://…` URL. If you are unsure of an exact gallery name/size, prefer an `SvgImage` from a collection or `SvgImage.FromFile(...)` instead of guessing the URI string.

## RibbonStyle vs PaintStyle

- **`BarItem.RibbonStyle`** controls how the item *can* be displayed inside a `RibbonPageGroup`. The group picks `Large`/`SmallWithText`/`SmallWithoutText` depending on available width and the bitmask. Default is `All`.
- **`BarItemLink.UserPaintStyle`** overrides how a single link paints in a classic `Bar` (`Default`, `Standard`, `CaptionGlyph`, `CaptionInMenu`). `BarItemLink.PaintStyle` itself is read-only (it reflects the item), so set `UserPaintStyle` on the link. Has no effect in ribbon groups.

```csharp
itemNew.RibbonStyle = RibbonItemStyles.Large | RibbonItemStyles.SmallWithText;

var linkInToolbar = bar1.AddItem(itemNew);
linkInToolbar.UserPaintStyle = BarItemPaintStyle.CaptionGlyph;
```

## Per-Link Overrides

A single item can appear as multiple links with independent visuals:

```csharp
var linkInGroup = groupFile.ItemLinks.Add(itemNew);    // large icon + caption (default)

var linkInQAT = ribbon.Toolbar.ItemLinks.Add(itemNew);
linkInQAT.UserRibbonStyle = RibbonItemStyles.SmallWithoutText;     // icon only

var linkInStatus = ribbonStatusBar1.ItemLinks.Add(itemNew);
linkInStatus.UserCaption = "Quick Save";
linkInStatus.UserAlignment = BarItemLinkAlignment.Right;
linkInStatus.BeginGroup = true;   // separator before this link
```

Link-only properties:

| Property | Purpose |
|---|---|
| `UserCaption` | Override the item's `Caption` for this link only. |
| `UserGlyph` | Override the item's icon for this link only. |
| `UserRibbonStyle` | Override `RibbonStyle` for this link only. |
| `UserAlignment` | Align link to `Left`/`Right` of its container. |
| `BeginGroup` | Show a separator before the link. |
| `MostRecentlyUsed` | Mark as recently used (for popup menus that hide unused items). |
| `Visible` | Per-link visibility (independent of item-level state). A link's `Enabled` is read-only — set `Enabled` on the item. |
| `UserPaintStyle` | Override the item's `PaintStyle` for this link only (`PaintStyle` on the link is read-only). |

## Wiring Clicks

```csharp
itemNew.ItemClick += (s, e) => {
    BarItem item = e.Item;          // the bar item
    BarItemLink link = e.Link;      // which link was clicked
    // …
};
```

A bar's manager raises `BarManager.ItemClick` for every click on every item — useful for cross-cutting concerns (logging, analytics). For the ribbon, `RibbonControl.ItemClick` is the equivalent. Per-item handlers are preferred.

Check items use `CheckedChanged`:

```csharp
viewBoldItem.CheckedChanged += (s, e) =>
    selection.Bold = viewBoldItem.Checked;
```

## BarEditItem (Embedded Editor)

```csharp
var spinRepo = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit {
    MinValue = 0, MaxValue = 100, Increment = 5
};
ribbon.RepositoryItems.Add(spinRepo);

var zoomItem = new BarEditItem(ribbon.Manager, spinRepo) {
    Caption  = "Zoom",
    EditWidth = 80,
    EditValue = 100
};
zoomItem.EditValueChanged += (s, e) => SetZoom((int)zoomItem.EditValue);

ribbonStatusBar1.ItemLinks.Add(zoomItem);
```

`BarEditItem` supports any `RepositoryItem*` — text, combo, lookup, date, calc, spin, color, etc. See the editors skill for the full catalog.

## Bind to a View-Model (MVVM-Style in WinForms)

WinForms does not have a native MVVM framework on par with WPF's. The cross-platform `DevExpress.Mvvm` package brings `BindableBase`, `DelegateCommand`, and `Messenger` and works inside WinForms.

### View-model

```csharp
using DevExpress.Mvvm;

public class ShellViewModel : BindableBase {
    public string DocumentTitle {
        get => GetProperty(() => DocumentTitle);
        set => SetProperty(() => DocumentTitle, value);
    }

    public bool IsDirty {
        get => GetProperty(() => IsDirty);
        set => SetProperty(() => IsDirty, value, () => SaveCommand.RaiseCanExecuteChanged());
    }

    public DelegateCommand SaveCommand { get; }

    public ShellViewModel() {
        SaveCommand = new DelegateCommand(Save, () => IsDirty);
    }

    void Save() { /* … */ }
}
```

### Bind item properties

```csharp
var vm = new ShellViewModel();
var bs = new BindingSource { DataSource = vm };

// Caption, Enabled, Visibility — direct property binding
saveCmd.DataBindings.Add(nameof(BarItem.Enabled), bs, nameof(vm.IsDirty));
ribbon.ApplicationDocumentCaption = vm.DocumentTitle;
vm.PropertyChanged += (_, e) => {
    if (e.PropertyName == nameof(vm.DocumentTitle))
        ribbon.ApplicationDocumentCaption = vm.DocumentTitle;
};

// Click → command
saveCmd.ItemClick += (_, _) => vm.SaveCommand.Execute(null);
```

### Helper: bind an item to an `ICommand`

```csharp
public static class BarItemBindings {
    public static void BindCommand(this BarBaseButtonItem item, System.Windows.Input.ICommand command) {
        item.ItemClick += (_, _) => { if (command.CanExecute(null)) command.Execute(null); };
        command.CanExecuteChanged += (_, _) => item.Enabled = command.CanExecute(null);
        item.Enabled = command.CanExecute(null);
    }
}

// usage
saveCmd.BindCommand(vm.SaveCommand);
```

For `BarCheckItem`s, two-way binding to a `bool` property:

```csharp
boldItem.DataBindings.Add(nameof(BarCheckItem.Checked), bs, nameof(vm.IsBold), true, DataSourceUpdateMode.OnPropertyChanged);
```

For `BarEditItem` editors:

```csharp
zoomItem.DataBindings.Add(nameof(BarEditItem.EditValue), bs, nameof(vm.Zoom), true, DataSourceUpdateMode.OnPropertyChanged);
```

> Do not use WPF's `DevExpress.Mvvm.UI` `Interaction.Behaviors` or `RibbonCommand` attached properties — those are WPF-only. The WinForms wiring above is the practical equivalent.

## Common Issues

- **Item created in designer but no visible link**: forgot to add the item to a `Bar.ItemLinks` / `RibbonPageGroup.ItemLinks`. Items are invisible until linked.
- **`Id = 0` for many items after `SaveLayoutToXml`**: missing `manager.GetNewItemId()` calls. Set unique ids in code.
- **Same item shown twice after restoring layout**: layout file references items by name + id; if `Id` is missing/duplicated, the deserializer may add duplicates.
- **`ItemShortcut` does nothing**: the bar manager's host form is not set. Set `barManager.Form = this` (or `ribbon.Manager.Form = this`, but the ribbon does this automatically).
- **`BarEditItem` does not raise `EditValueChanged`**: bind the underlying repository item's `EditValue` event (`ribbon.Manager.EditValueChanged`) or use the item's own event.
- **Caption of a single link should differ from the item**: set `link.UserCaption`. Setting `item.Caption` affects every link with no `UserCaption`.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/common-features/the-list-of-bar-items-and-links.md` (`xref:WindowsForms.2511`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/common-features/what-are-bar-items-and-bar-item-links.md` (`xref:WindowsForms.2514`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/tutorials/bar-item-links.md` (`xref:WindowsForms.1087`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/common-features/bar-item-display-options.md` (`xref:WindowsForms.2517`).
- `api/DevExpress.XtraBars.BarItem.yml`.
- `api/DevExpress.XtraBars.BarButtonItem.yml`.
- `api/DevExpress.XtraBars.BarItemLink.yml`.
- `api/DevExpress.XtraBars.BarItem.RibbonStyle.yml`.
- `api/DevExpress.XtraBars.BarEditItem.yml`.
