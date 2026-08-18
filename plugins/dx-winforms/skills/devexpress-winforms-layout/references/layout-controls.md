# Layout Control Variants

DevExpress WinForms ships several layout controls that serve different purposes. This reference explains what each control does and, critically, **when to choose which one**.

## Quick Selection Table

| Control | Class | Best For |
|---|---|---|
| `LayoutControl` | `DevExpress.XtraLayout.LayoutControl` | General-purpose form layout — wraps any controls, auto-aligns labels, supports runtime customization |
| `DataLayoutControl` | `DevExpress.XtraDataLayout.DataLayoutControl` | Data-entry forms — auto-generates editors from a bound data source (DataTable, business object) |
| `DockManager` | `DevExpress.XtraBars.Docking.DockManager` | Tool-window / IDE-style UI — panels that users can drag, float, pin, and auto-hide |
| `StackPanel` | `DevExpress.Utils.Layout.StackPanel` | Simple directional flow of controls — lightweight replacement for `FlowLayoutPanel` |
| `TablePanel` | `DevExpress.Utils.Layout.TablePanel` | Grid-like rows/columns — lightweight replacement for `TableLayoutPanel` |

> **Note on naming**: There is no separate `FlowLayoutControl` or `TableLayoutControl` class in DevExpress WinForms. *Flow Layout* and *Table Layout* are layout **modes** (`LayoutMode`) that can be enabled on any `LayoutControlGroup` inside a `LayoutControl`.

---

## LayoutControl

**Class**: `DevExpress.XtraLayout.LayoutControl`  
**NuGet**: `DevExpress.Win.Navigation`  
**Namespace**: `DevExpress.XtraLayout`

### What It Does

`LayoutControl` replaces the old manual pixel-based form layout. Instead of setting `Left`/`Top`/`Width`/`Height` on each control, you drop controls into the `LayoutControl` and it manages:

- Proportional resizing when the form is resized
- Automatic alignment of label edges across all items
- Size constraints (minimum / maximum per item)
- Runtime customization — end-users can rearrange, hide, and resize items without code changes
- Groups with captions and tabbed groups without needing extra `Panel` / `TabControl` widgets

### Layout Modes

| Mode | Description |
|---|---|
| **Free (default)** | Items can be freely resized and rearranged. Proportional resize on form resize. |
| **Flow** | Items are arranged left-to-right in rows, wrapping at the group edge. Enable per-group via `group.LayoutMode = LayoutMode.Flow`. |
| **Table** | Items occupy cells of a grid defined by rows and columns. Enable via `group.LayoutMode = LayoutMode.Table`, then set `item.OptionsTableLayoutItem.RowIndex / ColumnIndex / RowSpan / ColumnSpan`. |

### When to Choose

- You are building a **data-entry, settings, or properties form** with labeled editors.
- You want consistent label alignment without manual positioning.
- You want to support **end-user runtime customization** (hide fields, rearrange editors).
- You want **tabbed groups** without adding a separate `TabControl`.

### When NOT to Choose

- The form content is driven by a data source and you want editors auto-generated → use `DataLayoutControl`.
- You need panels that the user can float and pin like VS tool windows → use `DockManager`.
- You only need a simple vertical/horizontal stack of controls → use `StackPanel`.

---

## DataLayoutControl

**Class**: `DevExpress.XtraDataLayout.DataLayoutControl`  
**NuGet**: `DevExpress.Win.Navigation`  
**Namespace**: `DevExpress.XtraDataLayout`

### What It Does

`DataLayoutControl` extends `LayoutControl` with **automatic layout generation from a data source**. When bound to a `DataTable`, `BindingSource`, or a business object (`INotifyPropertyChanged`), it can:

- Auto-generate editors for each bound field (TextEdit for strings, CheckEdit for bools, DateEdit for dates, etc.)
- Create `LayoutControlItem` wrappers with label captions from property/column names or `[DisplayName]` attributes
- Use `[DataAnnotations]` attributes (`[Required]`, `[StringLength]`, `[Display]`) to drive validation and captions
- Nest sub-objects as collapsible `LayoutControlGroup` sections (`[Display(GroupName = "Address")]`)

### Workflow

1. Set `DataSource` (+ `DataMember` for `DataSet`)
2. Call `RetrieveFields()` to generate editors and `LayoutControlItem` wrappers
3. Customize the generated layout as with a plain `LayoutControl`

### When to Choose

- You have a **business object or DataTable** and want to scaffold an edit form automatically.
- You want `[DataAnnotations]` attributes to drive the UI (captions, groups, required markers).
- You want to minimize boilerplate for CRUD forms.

### When NOT to Choose

- The form structure is custom (not driven by a data source) → use plain `LayoutControl`.
- You need dockable panels or a launcher UI.

---

## DockManager

**Class**: `DevExpress.XtraBars.Docking.DockManager`  
**NuGet**: `DevExpress.Win.Navigation`  
**Namespace**: `DevExpress.XtraBars.Docking`

### What It Does

`DockManager` is a **non-visual component** that manages `DockPanel` objects. Each `DockPanel` is a resizable, movable window that the user can:

- Dock to any edge of the form (Left, Right, Top, Bottom, Fill)
- Float as a free-standing window above the form
- Auto-hide (pin/unpin) — collapses to a tab strip when not focused
- Group with other panels as tabs (drag one panel onto another)

### Key Capabilities

| Feature | How |
|---|---|
| Create a panel | `dm.AddPanel(DockingStyle.Left)` |
| Dock to another panel | `panel.DockTo(otherPanel, DockingStyle.Right, 0)` |
| Create a tab group | `panel.DockAsTab(otherPanel)` |
| Auto-hide a panel | `panel.Dock = DockingStyle.Left; panel.AutoHideContainers.Add(...)` or call `panel.DockTo(DockingStyle.Top)` then click pin button at runtime |
| Prevent floating | `panel.Options.AllowFloating = false` |
| Prevent closing | `panel.Options.ShowCloseButton = false` |
| Close / restore | `panel.Visibility = DockVisibility.Hidden` / `DockVisibility.Visible` |

### When to Choose

- You are building a **Visual Studio / IDE-inspired UI** with resizable, dockable tool windows.
- The form has distinct regions (file tree, properties panel, output window, editor area) that the user may want to rearrange.
- You need auto-hide panels (slide-out sidebars).

### When NOT to Choose

- You need a fixed, non-dockable form layout → use `LayoutControl`.

### Integration with DocumentManager

`DockManager` pairs naturally with `DocumentManager` (`DevExpress.XtraBars.Docking2010.DocumentManager`). `DocumentManager` manages the central content area (tabbed documents, MDI emulation), while `DockManager` manages the tool windows around it. Together they recreate the full Visual Studio shell UI pattern.

---

## StackPanel

**Class**: `DevExpress.Utils.Layout.StackPanel`  
**NuGet**: `DevExpress.Utils` (assembly `DevExpress.Utils.dll`; also pulled in transitively by `DevExpress.Win.Navigation`)  
**Namespace**: `DevExpress.Utils.Layout`

### What It Does

`StackPanel` arranges child controls in a single direction (top-to-bottom or left-to-right), similar to WPF's `StackPanel`. It is a **lightweight drop-in replacement** for `System.Windows.Forms.FlowLayoutPanel` that respects DevExpress skins.

```csharp
var stack = new StackPanel {
    Dock            = DockStyle.Fill,
    LayoutDirection = StackPanelLayoutDirection.TopDown  // or LeftToRight (default), RightToLeft, BottomUp
};
```

### When to Choose

- You need a **simple vertical or horizontal sequence** of controls (buttons, editors) with auto-sizing.
- You want a skin-aware `FlowLayoutPanel` replacement.
- No label alignment or grouping features needed.

---

## TablePanel

**Class**: `DevExpress.Utils.Layout.TablePanel`  
**NuGet**: `DevExpress.Utils` (assembly `DevExpress.Utils.dll`; also pulled in transitively by `DevExpress.Win.Navigation`)  
**Namespace**: `DevExpress.Utils.Layout`

### What It Does

`TablePanel` arranges child controls in a grid of rows and columns, similar to WPF's `Grid` or `System.Windows.Forms.TableLayoutPanel`. Each row and column can have an `Absolute`, `AutoSize`, or `Relative` (percentage) size.

```csharp
var table = new TablePanel { Dock = DockStyle.Fill };
table.Rows.Add(new TablePanelRow(TablePanelEntityStyle.AutoSize, 1));
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Relative, 70));
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Relative, 30));
```

### When to Choose

- You need a **grid-based layout** without label alignment features.
- A skin-aware `TableLayoutPanel` replacement is required.
- You want row/column span support without the complexity of `LayoutControl`'s Table mode.

---

## Decision Tree

```
Do users need to drag/float/pin panels like VS tool windows?
  YES → DockManager
  NO  → continue

Is the layout driven by a data source (auto-generate editors from a table/class)?
  YES → DataLayoutControl
  NO  → continue

Do you need labels, groups, tabbed groups, runtime customization?
  YES → LayoutControl
  NO  → continue

Simple directional flow only?
  YES → StackPanel
  Need rows AND columns? → TablePanel
```

## Source Material

- Layout and Data Layout Controls overview: `https://docs.devexpress.com/content/WindowsForms/3406?md=true`
- Docking Library: `https://docs.devexpress.com/content/WindowsForms/5368?md=true`
- Form Layout Managers and Containers: `https://docs.devexpress.com/content/WindowsForms/114577?md=true`
