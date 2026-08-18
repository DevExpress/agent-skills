# Bars and their Layout

This reference covers the classic Bars stack in WinForms — `BarManager` + `Bar` + `BarDockControl` + role assignments (`MainMenu`, `StatusBar`) — and how to position bars on the four sides of the form, float them, or place them as standalone toolbars in arbitrary container controls.

> **Naming clarification**: there are no separate `MainMenuControl` / `StatusBarControl` / `ToolBarControl` classes in **DevExpress WinForms**. That naming belongs to the WPF Bars stack. In WinForms, **all** of these roles are played by the same `Bar` class — what differentiates them is how `BarManager.MainMenu` / `BarManager.StatusBar` are assigned and how `Bar.DockStyle` is configured.

For the items that live inside bars, see [items-and-settings.md](items-and-settings.md). For the ribbon alternative, see [ribbon-structure.md](ribbon-structure.md).

## When to Use This Reference

- Building a Visual-Studio-style UI with a main menu, multiple toolbars, and a status bar.
- Picking between Bars and Ribbon.
- Docking bars to specific sides + rows of the form.
- Making a toolbar float (no docking).
- Placing a toolbar inside a `UserControl` or a side panel via `StandaloneBarDockControl`.

## Bars vs Ribbon — When to Pick Each

| Use Bars when | Use Ribbon when |
|---|---|
| You want a Visual-Studio-style UI (menus + toolbars + status bar). | You want an Office-style UI with tabs and large icons. |
| Vertical density matters (many commands in a compact space). | Discoverability matters (icons + captions + grouping). |
| Users expect runtime drag-to-reorder of toolbars. | You want contextual tabs (Picture Tools, Table Tools, …). |
| Simple host form is enough. | You want QAT integrated into the title bar (requires `RibbonForm`). |

> Don't mix `BarManager` and `RibbonControl` in the same form — they conflict.

## `BarManager` — The Top-Level Component

A `BarManager` is a non-visual component that owns all bars, items, and the four dock containers. Add it to a form (or set `Form = this` in code).

```csharp
var bars = new BarManager { Form = this };
```

Four `BarDockControl` instances are auto-created and docked to all four sides of the form. They are invisible until a bar is docked to them — their size is 0 until then.

## `Bar` — One Toolbar / Menu / Status Strip

Every visible strip is a `Bar`. Bars share one class — their role is determined by how they are configured.

```csharp
var menuBar = new Bar(bars, "Main Menu") {
    DockStyle = BarDockStyle.Top,
    DockRow   = 0,
    OptionsBar = {
        MultiLine                  = false,
        DrawDragBorder             = false,        // hide the drag handle
        UseWholeRow                = true,
        DisableCustomization       = false,
        DisableClose               = true,         // user cannot remove this bar
        RotateWhenVertical         = true
    }
};
```

| Property | Purpose |
|---|---|
| `BarName` | Display name (visible in the customization form). |
| `DockStyle` | `Top` / `Bottom` / `Left` / `Right` / `Standalone` / `Float` / `None`. |
| `DockRow` | Order within one side (multiple bars stack by row index). |
| `DockCol` | Horizontal position within a row, when multiple bars share a row. |
| `OptionsBar` | Sub-options: customization, drag handle, rotation, whole-row, MDI children list. |
| `LinksPersistInfo` | The bar's `BarItemLink` collection settings used by layout serialization. |
| `Visible` | Hide/show the bar. |

## Role Assignments

Two `BarManager` properties promote a `Bar` to a specific role:

```csharp
bars.MainMenu  = menuBar;     // this Bar plays the main-menu role; auto-merges in MDI; spans the full row.
bars.StatusBar = statusBar;   // this Bar is the status bar; auto-docks to the bottom.
```

| Role | Property | Effect |
|---|---|---|
| Main menu | `BarManager.MainMenu` | Stretches across its row. Auto-merges with child MDI forms. Cannot be closed by users. |
| Status bar | `BarManager.StatusBar` | Convention: dock to `Bottom`. Auto-rotation off. |
| Regular toolbar | none — any other `Bar` | Free to dock/float, draggable by users (unless disabled). |

You can have zero or more regular toolbars in addition to the main menu and status bar.

## Dock Containers — `BarDockControl`

The four auto-created `BarDockControl`s (one per side) host bars docked to their side. Each `BarDockControl`:

- Is invisible (`Size = 0`) until at least one bar is docked to it.
- Auto-sizes to its content.
- Rotates vertically (Left / Right) docked bars by 90 ° clockwise unless `OptionsBar.RotateWhenVertical = false`.
- Holds bars in `DockRow` order from outside toward inside.

```text
┌────────────────────────────────────────────────┐
│  BarDockControl (Top)                          │
│  ┌─ Bar "Main Menu"  (DockRow=0) ──────────┐   │
│  ├─ Bar "Toolbar"    (DockRow=1) ──────────┤   │
│  └─ Bar "Format"     (DockRow=2) ──────────┘   │
│                                                │
│  Form client area                              │
│                                                │
│  ┌─ Bar "Status"  ─────────────────────────┐   │
│  BarDockControl (Bottom)                       │
└────────────────────────────────────────────────┘
```

### `DockRow` and `DockCol`

- `DockRow` orders bars from edge inward. Lower `DockRow` is closer to the form edge.
- `DockCol` orders multiple bars sharing the same `DockRow`. Set both to put two short toolbars side-by-side.

```csharp
toolbarFile.DockStyle = BarDockStyle.Top; toolbarFile.DockRow = 1; toolbarFile.DockCol = 0;
toolbarEdit.DockStyle = BarDockStyle.Top; toolbarEdit.DockRow = 1; toolbarEdit.DockCol = 1;
```

## Floating Bars

A `Bar` with `DockStyle = None` lives in a floating panel. The panel shows the `BarName` and a close button. Users can drag it around and re-dock to any `BarDockControl`.

```csharp
var palette = new Bar(bars, "Color Palette") {
    DockStyle = BarDockStyle.None,
    FloatLocation = new Point(800, 200),
    FloatSize     = new Size(180, 240)
};
```

## Standalone Toolbars

Sometimes you want a toolbar that does *not* live in any of the four form-side dock containers — e.g., inside a `UserControl`, a `XtraTabPage`, or a side panel.

Add a `StandaloneBarDockControl` to the target container and set the bar's `DockStyle = Standalone`:

```csharp
var standalone = new StandaloneBarDockControl { Dock = DockStyle.Top };
panel.Controls.Add(standalone);
standalone.Manager = bars;

var localToolbar = new Bar(bars, "Local") { DockStyle = BarDockStyle.Standalone };
localToolbar.StandaloneBarDockControl = standalone;   // host this bar in the standalone dock control
```

`StandaloneBarDockControl` is what makes "context toolbars" practical — a toolbar that belongs to one panel, not the whole form.

## Per-Item Alignment Inside a Bar

Each link on a bar can be aligned to the left or right edge:

```csharp
var link = bar.AddItem(viewItem);
link.UserAlignment = BarItemLinkAlignment.Right;
```

For status bars, this places info text (e.g., zoom indicator) at the right side.

## Customization Form

Set `bars.AllowCustomization = true` to let users open the customization form (Ctrl + click a toolbar, or `bars.Customize()` in code). Users can:

- Add/remove items from each bar (drag from the form to the bar).
- Re-order bars across rows.
- Rename bars and change their `PaintStyle`.
- Reset to default.

Disable per-bar customization with `OptionsBar.DisableCustomization = true`.

## Saving and Restoring Layout

```csharp
bars.SaveLayoutToXml("bars-layout.xml");
bars.RestoreLayoutFromXml("bars-layout.xml");
// Also JSON/Stream/Registry overloads.
```

For the serializer to work reliably, set `BarItem.Id` (via `manager.GetNewItemId()`) on every item created in code.

## Recently Used Items

Mark a link as recently used with `BarItemLink.MostRecentlyUsed = true` — useful in popup menus that hide rarely used commands behind an "expand" chevron.

## Recommended Bar Layout Patterns

### Pattern 1 — Main menu + one toolbar + status bar

```csharp
var menu   = new Bar(bars, "Main Menu") { DockStyle = BarDockStyle.Top, DockRow = 0 };
var tools  = new Bar(bars, "Standard")  { DockStyle = BarDockStyle.Top, DockRow = 1 };
var status = new Bar(bars, "Status")    { DockStyle = BarDockStyle.Bottom };

bars.MainMenu  = menu;
bars.StatusBar = status;
```

### Pattern 2 — Two side-by-side toolbars in one row

```csharp
var file = new Bar(bars, "File") { DockStyle = BarDockStyle.Top, DockRow = 1, DockCol = 0 };
var edit = new Bar(bars, "Edit") { DockStyle = BarDockStyle.Top, DockRow = 1, DockCol = 1 };
```

### Pattern 3 — Vertical toolbox on the left

```csharp
var tools = new Bar(bars, "Tools") {
    DockStyle = BarDockStyle.Left,
    OptionsBar = { RotateWhenVertical = false }   // keep horizontal item orientation
};
```

### Pattern 4 — Floating palette at app start

```csharp
var palette = new Bar(bars, "Palette") {
    DockStyle    = BarDockStyle.None,
    FloatLocation = new Point(600, 300),
    FloatSize     = new Size(220, 320)
};
```

### Pattern 5 — Status bar with right-aligned editor

```csharp
var zoomItem = new BarEditItem(bars.Manager, new RepositoryItemTrackBar()) {
    EditWidth = 120, EditValue = 100
};
var link = statusBar.AddItem(zoomItem);
link.UserAlignment = BarItemLinkAlignment.Right;
```

## Common Issues

- **Bar appears in the wrong row** — check both `DockStyle` and `DockRow`. Rows are unique per side.
- **`MainMenu` is just a toolbar look-alike** — assign `bars.MainMenu = mainBar` for it to span the row and auto-merge in MDI.
- **Status bar items missing borders** — `OptionsBar.DrawDragBorder = false` is enabled by default for the status bar role. That is intentional.
- **Floating panel ignores `FloatLocation`** — set it before docking changes; the location is captured when the bar transitions to `None`.
- **Standalone bar empty** — the bar's `StandaloneBarDockControl` is not assigned. Set `bar.StandaloneBarDockControl = theDockControl` (and `theDockControl.Manager = bars`).
- **Vertical bar text upside down** — `OptionsBar.RotateWhenVertical = true` is the default. Set it to `false` to keep items horizontal in a Left/Right docked bar.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/index.md` — Bars overview (`xref:WindowsForms.5361`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/tutorials/add-and-remove-toolbars.md` (`xref:WindowsForms.116781`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/tutorials/add-bar-items-to-toolbars.md` (`xref:WindowsForms.116782`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/toolbar-customization.md` (`xref:WindowsForms.116784`).
- `api/DevExpress.XtraBars.BarManager.yml`.
- `api/DevExpress.XtraBars.Bar.yml`.
- `api/DevExpress.XtraBars.BarDockControl.yml`.
- `api/DevExpress.XtraBars.StandaloneBarDockControl.yml`.
- `api/DevExpress.XtraBars.Bar.DockStyle.yml`.
