# Building Layouts

This reference covers the rules and patterns for constructing layouts in each DevExpress WinForms layout control: `LayoutControl`, `DataLayoutControl`, `DockManager`, `StackPanel`, and `TablePanel`.

---

## LayoutControl

### Hierarchical Structure

Every `LayoutControl` has a single **root group** (`LayoutControl.Root`), which is a `LayoutControlGroup`. All layout elements are nested inside it. The supported element types are:

| Element | Class | Description |
|---|---|---|
| Layout Item | `LayoutControlItem` | Wraps one external control, optionally with a label |
| Layout Group | `LayoutControlGroup` | Container for items, nested groups, and tabbed groups; can have a caption and border |
| Tabbed Group | `TabbedControlGroup` | Presents nested `LayoutControlGroup` objects as tabs |
| Empty Space Item | `EmptySpaceItem` | Invisible filler that occupies free space |
| Splitter Item | `SplitterItem` | Drag to resize adjacent items |
| Label Item | `SimpleLabelItem` | Static text within the layout |
| Separator Item | `SimpleSeparator` | Horizontal or vertical divider line |

### Golden Rules

1. **Every external control must live inside a `LayoutControlItem`**. You cannot place a `TextEdit` directly on the `LayoutControl.Root` group — the `LayoutControl` wraps it automatically when you drag it in, or you create the wrapper explicitly with `AddItem(caption, control)`.
2. **Do not set `Dock`, `Anchor`, `Location`, or `Size` on controls hosted in a `LayoutControl`**. Those properties are managed by the layout engine. Use `LayoutControlItem.MinSize` / `MaxSize` for constraints.
3. **Use `BeginUpdate()` / `EndUpdate()`** when adding multiple items in code to suppress intermediate repaints.
4. **Each layout item must have a unique `Name`** — the serializer uses `Name` to match items when restoring a saved layout.
5. **`EmptySpaceItem` fills remaining space**. Add one when you want an item to stay at a fixed size rather than stretching.

### Adding Items in Code

```csharp
layoutControl1.BeginUpdate();
try {
    // Add a control with a label
    var firstNameEdit = new TextEdit { Name = "edFirstName" };
    LayoutControlItem item1 = layoutControl1.AddItem("First Name", firstNameEdit);
    item1.Name = "lciFirstName";

    // Add a control without a label (there is no AddItem(Control) overload; pass a
    // caption and hide it). Real overloads are AddItem(string, Control) / AddItem(BaseLayoutItem).
    var memoEdit = new MemoEdit { Name = "edNotes" };
    LayoutControlItem item2 = layoutControl1.AddItem("Notes", memoEdit);
    item2.TextVisible = false;
    item2.Name = "lciNotes";
}
finally {
    layoutControl1.EndUpdate();
}
```

### Groups

```csharp
layoutControl1.BeginUpdate();
try {
    // Add a named group inside the root
    LayoutControlGroup group = layoutControl1.Root.AddGroup();
    group.Text = "Personal Info";
    group.Name = "lcgPersonal";

    var nameEdit  = new TextEdit { Name = "edName" };
    var emailEdit = new TextEdit { Name = "edEmail" };

    group.AddItem("Name",  nameEdit).Name  = "lciName";
    group.AddItem("Email", emailEdit).Name = "lciEmail";
}
finally {
    layoutControl1.EndUpdate();
}
```

### Tabbed Groups

```csharp
layoutControl1.BeginUpdate();
try {
    TabbedControlGroup tabGroup = layoutControl1.Root.AddTabbedGroup();
    tabGroup.Name = "tcgMain";

    // First tab
    LayoutControlGroup tab1 = tabGroup.AddTabPage() as LayoutControlGroup;
    tab1.Text = "General";
    tab1.Name = "lcgGeneral";
    tab1.AddItem("Name", new TextEdit { Name = "edName" }).Name = "lciName";

    // Second tab
    LayoutControlGroup tab2 = tabGroup.AddTabPage() as LayoutControlGroup;
    tab2.Text = "Details";
    tab2.Name = "lcgDetails";
    tab2.AddItem("Notes", new MemoEdit { Name = "edNotes" }).Name = "lciNotes";

    tabGroup.SelectedTabPage = tab1;
}
finally {
    layoutControl1.EndUpdate();
}
```

### Flow Layout Mode

Enable Flow Layout on any group. Items wrap to the next row when the group width is exceeded. The `LayoutMode` enum lives in `DevExpress.XtraLayout.Utils` — add `using DevExpress.XtraLayout.Utils;`:

```csharp
using DevExpress.XtraLayout.Utils;   // LayoutMode

LayoutControlGroup group = layoutControl1.Root.AddGroup();
group.Text = "Tags";
group.Name = "lcgTags";
group.LayoutMode = LayoutMode.Flow;  // enable Flow Layout

foreach (var tag in tags) {
    var btn = new SimpleButton { Text = tag, AutoSizeMode = AutoSizeMode.GrowAndShrink };
    var item = group.AddItem(tag, btn);   // AddItem(string, Control); caption hidden below
    item.TextVisible = false;
    item.SizeConstraintsType = SizeConstraintsType.Default;
}
```

### Table Layout Mode

Enable Table Layout on a group, then assign row/column indexes per item:

```csharp
using DevExpress.XtraLayout.Utils;   // LayoutMode

LayoutControlGroup group = layoutControl1.Root.AddGroup();
group.LayoutMode = LayoutMode.Table;
group.OptionsTableLayoutGroup.ColumnCount = 2;
group.OptionsTableLayoutGroup.RowCount    = 3;

var item1 = group.AddItem("First Name", new TextEdit { Name = "edFirst" });
item1.Name = "lciFirst";
item1.OptionsTableLayoutItem.ColumnIndex = 0;
item1.OptionsTableLayoutItem.RowIndex    = 0;

var item2 = group.AddItem("Last Name", new TextEdit { Name = "edLast" });
item2.Name = "lciLast";
item2.OptionsTableLayoutItem.ColumnIndex = 1;
item2.OptionsTableLayoutItem.RowIndex    = 0;

// Span across both columns
var item3 = group.AddItem("Notes", new MemoEdit { Name = "edNotes" });
item3.Name = "lciNotes";
item3.OptionsTableLayoutItem.ColumnIndex = 0;
item3.OptionsTableLayoutItem.RowIndex    = 1;
item3.OptionsTableLayoutItem.ColumnSpan  = 2;
```

### Size Constraints

```csharp
// Fixed size — never grows or shrinks
item.SizeConstraintsType = SizeConstraintsType.Custom;
item.MaxSize = new Size(200, 24);
item.MinSize = new Size(200, 24);

// Minimum only — can grow but not shrink below 120px wide
item.SizeConstraintsType = SizeConstraintsType.Custom;
item.MinSize = new Size(120, 0);  // 0 = no constraint on that axis

// Free — remove all constraints
item.SizeConstraintsType = SizeConstraintsType.Default;
```

### Hiding and Showing Items

```csharp
// Hide — item becomes invisible but space is preserved
layoutControlItem1.Visibility = LayoutVisibility.Never;

// Show
layoutControlItem1.Visibility = LayoutVisibility.Always;

// Show only in customization mode (visible in the Customization form, hidden at runtime)
layoutControlItem1.Visibility = LayoutVisibility.OnlyInCustomization;

// Show only at runtime (visible in the running app, hidden in the Customization form)
layoutControlItem1.Visibility = LayoutVisibility.OnlyInRuntime;
```

### Customizing Labels

```csharp
item.Text     = "Phone Number";      // label text
item.TextLocation = Locations.Left;  // Left (default), Top, Right, Bottom
item.TextAlignMode = TextAlignModeItem.AutoSize;  // or CustomSize
item.TextSize  = new Size(120, 0);   // fixed label width
item.TextVisible = false;            // hide label entirely
```

---

## DataLayoutControl

`DataLayoutControl` uses the same building rules as `LayoutControl` for manual editing, plus these specific rules:

1. **Set `DataSource` before calling `RetrieveFields()`** — the control cannot generate editors without a data source.
2. **Use `[Display]` attribute** on business object properties to control captions and grouping:

   ```csharp
   [Display(Name = "First Name", GroupName = "Personal Info", Order = 1)]
   public string FirstName { get; set; }
   ```

3. **Nested objects become groups** when the property is a complex type. Use `[Display(GroupName = "...")]` on nested object properties.
4. **Change editor type** by replacing the item's `Control`:

   ```csharp
   ((LayoutControlItem)dataLayoutControl1.GetItemByFieldName("BirthDate"))
       .Control = new DateEdit { Name = "edBirthDate" };
   ```

5. **Everything else** (groups, tabs, visibility, sizes) follows the same rules as `LayoutControl`.

---

## DockManager

### Panel Creation Rules

1. **One `DockManager` per form** — placing multiple `DockManager` instances on the same form causes conflicts.
2. **`DockManager.Form` must be set** before calling `AddPanel` — assign it explicitly (`dockManager1.Form = this;`) wherever you create the manager.
3. **Use `DockPanel.Controls.Add(control)`** to place a control inside a panel. The hosted control should have `Dock = DockStyle.Fill`.

### Docking and Grouping

```csharp
// Dock to form edges
DockPanel left   = dm.AddPanel(DockingStyle.Left);
DockPanel bottom = dm.AddPanel(DockingStyle.Bottom);
DockPanel right  = dm.AddPanel(DockingStyle.Right);

// Dock a panel as a sibling (right of left panel, inside left panel's container)
DockPanel left2 = dm.AddPanel(DockingStyle.Float);
left2.DockTo(left, DockingStyle.Right, 0);  // splits left panel vertically

// Create a tab group (two panels as tabs)
DockPanel propPanel   = dm.AddPanel(DockingStyle.Right);
DockPanel searchPanel = dm.AddPanel(DockingStyle.Float);
searchPanel.DockAsTab(propPanel);           // search becomes a second tab inside propPanel
```

### Visibility and Auto-Hide

```csharp
// Hide a panel (it remains in HiddenPanels, not destroyed)
panel.Visibility = DockVisibility.Hidden;

// Show again
panel.Visibility = DockVisibility.Visible;

// Auto-hide programmatically
panel.DockTo(panel.Dock);   // re-dock in same position
// Users toggle auto-hide by clicking the pin button at runtime
```

### Placing Content in a Panel

```csharp
DockPanel panel = dm.AddPanel(DockingStyle.Left);
panel.Text  = "Solution Explorer";
panel.Width = 250;

// The panel's ControlContainer is the actual content area
var tree = new TreeView { Dock = DockStyle.Fill };
panel.ControlContainer.Controls.Add(tree);
// OR (shorthand — DockPanel.Controls redirects to ControlContainer)
panel.Controls.Add(tree);
```

### Important: RibbonForm Performance

When using `DockManager` on a `RibbonForm`, call `ForceInitialize()` in `Form_Load` to avoid slow startup:

```csharp
private void MainForm_Load(object sender, EventArgs e) {
    dockManager1.ForceInitialize();
}
```

---

## StackPanel

```
StackPanel
  └── Child controls arranged in a single direction
```

### Rules

- `LayoutDirection = StackPanelLayoutDirection.TopDown` (vertical) or `LeftToRight` (horizontal, default); `RightToLeft` and `BottomUp` are also available.
- Child controls do **not** need `Dock` or `Anchor` — `StackPanel` sizes them automatically.
- Use `Padding` on the `StackPanel` and `Margin` on children for spacing.
- No label support — it is a pure container.
- `StackPanel` and `StackPanelLayoutDirection` live in `DevExpress.Utils.Layout` — add `using DevExpress.Utils.Layout;`.

```csharp
using DevExpress.Utils.Layout;   // StackPanel, StackPanelLayoutDirection

var stack = new StackPanel {
    Dock            = DockStyle.Top,
    LayoutDirection = StackPanelLayoutDirection.LeftToRight,
    Padding         = new Padding(4)
};
stack.Controls.Add(new SimpleButton { Text = "Save",   Width = 80 });
stack.Controls.Add(new SimpleButton { Text = "Cancel", Width = 80 });
Controls.Add(stack);
```

---

## TablePanel

### Row and Column Sizing

| `TablePanelEntityStyle` | Meaning |
|---|---|
| `Absolute` | Fixed pixel size |
| `AutoSize` | Shrinks/grows to fit the largest child in that row/column |
| `Relative` | Proportional share of remaining space (like CSS `fr`) |
| `Separator` | A thin fixed-size divider row/column |

`TablePanel`, `TablePanelRow`, `TablePanelColumn`, and `TablePanelEntityStyle` live in `DevExpress.Utils.Layout` — add `using DevExpress.Utils.Layout;`.

```csharp
using DevExpress.Utils.Layout;   // TablePanel, TablePanelRow, TablePanelColumn, TablePanelEntityStyle

var table = new TablePanel { Dock = DockStyle.Fill };

// Two rows: header (auto) + content (relative, takes all remaining)
table.Rows.Add(new TablePanelRow(TablePanelEntityStyle.AutoSize, 1));
table.Rows.Add(new TablePanelRow(TablePanelEntityStyle.Relative, 100));

// Two columns: fixed label (120px) + flexible editor (takes rest)
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Absolute, 120));
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Relative, 100));

// Add controls to the panel, then assign each to a cell by row and column index
var label = new LabelControl { Text = "Name:" };
table.Controls.Add(label);
table.SetCell(label, 0, 0);   // SetCell(control, row, column)

var edit = new TextEdit();
table.Controls.Add(edit);
table.SetCell(edit, 0, 1);

Controls.Add(table);
```

### Column/Row Span

```csharp
table.SetCell(control, 1, 0);       // place at row 1, column 0
table.SetColumnSpan(control, 2);    // span two columns
table.SetRowSpan(control, 1);       // span one row (optional; 1 is the default)
```

---

## Common Issues

| Symptom | Cause | Fix |
|---|---|---|
| Controls overlap in `LayoutControl` | Setting `Dock` or `Anchor` on hosted controls | Remove `Dock` / `Anchor`; use `LayoutControlItem.MinSize`/`MaxSize` instead |
| Layout breaks after restore | Item `Name` properties changed since layout was saved | Ensure every item has a stable, unique `Name` |
| `DockManager` panels disappear on restart | Layout not saved | Save layout in `FormClosing`, restore in `FormLoad` |
| `DockPanel` has no visible content area | Control added to `DockPanel` directly instead of `ControlContainer` | Add to `panel.ControlContainer.Controls` or `panel.Controls` |
| Flow layout group not wrapping | Group not in Flow mode | Set `group.LayoutMode = LayoutMode.Flow` (`DefaultLayoutType` only sets the default item orientation and does not enable Flow mode) |
| Table layout items overlap | `RowIndex`/`ColumnIndex` duplicated | Each item needs a unique row+column combination |

## Source Material

- Customizing a Layout In Code: `https://docs.devexpress.com/content/WindowsForms/2327?md=true`
- Layout Hierarchical Structure: `https://docs.devexpress.com/content/WindowsForms/2210?md=true`
- Flow Layout: `https://docs.devexpress.com/content/WindowsForms/17548?md=true`
- Table Layout: `https://docs.devexpress.com/content/WindowsForms/114044?md=true`
- DockPanel: `https://docs.devexpress.com/content/WindowsForms/1244?md=true`
- Creating and Destroying Dock Panels: `https://docs.devexpress.com/content/WindowsForms/1263?md=true`
- Docking Panels Programmatically: `https://docs.devexpress.com/content/WindowsForms/1265?md=true`
