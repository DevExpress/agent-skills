# Building Layouts — Rules per Control

Once you've picked one of the six controls (see [control-varieties.md](control-varieties.md)), this page covers **how to actually build the layout**: which child elements are valid, how nesting works, what the attached properties do, and the common patterns for each.

## When to Use This Reference

Use this when you need to:

- Build a compound `LayoutControl` form with nested groups and tabs
- Decorate a POCO for `DataLayoutControl` with the right `[Display]` / `[DataType]` grammar
- Configure `Tile` sizes, groups, click handling
- Arrange flow items with `BreakFlowToFit`, `IsFlowBreak`, `StretchContent`, `MaximizedElement`
- Use `DockLayoutControl.Dock` attached property values
- Build a Visual-Studio-style shell with `DockLayoutManager`

## LayoutControl — Compound Forms

The `LayoutControl` arranges its direct children in a **single row or column** (set via `Orientation`). For non-linear layouts, nest `LayoutGroup` containers — each group also orients its items in a row or column. Items inside the deepest groups are usually `LayoutItem` (a label + content wrapper).

### Anatomy

```
LayoutControl
└── LayoutGroup (Vertical | Horizontal | Tabs)
    ├── LayoutItem (Label + child content)
    ├── LayoutItem
    └── LayoutGroup (nested, opposite orientation)
        └── LayoutItem
        └── LayoutItem
```

### LayoutGroup Views

`LayoutGroup.View` (the `LayoutGroupView` enum) decides how the group renders:

| View | Look |
|---|---|
| `Group` (default) | No header, no border — invisible container. Used for arranging items in a direction. |
| `GroupBox` | Bordered box with header text. Optionally collapsible (`IsCollapsible`). |
| `Tabs` | Tab strip; each child becomes a tab. |

```xaml
<dxlc:LayoutControl Orientation="Vertical">
    <dxlc:LayoutGroup View="GroupBox" Header="Address" Orientation="Vertical">
        <dxlc:LayoutGroup Orientation="Horizontal">
            <dxlc:LayoutItem Label="Country"><dxe:TextEdit/></dxlc:LayoutItem>
            <dxlc:LayoutItem Label="Region"><dxe:TextEdit/></dxlc:LayoutItem>
        </dxlc:LayoutGroup>
        <dxlc:LayoutGroup Orientation="Horizontal">
            <dxlc:LayoutItem Label="City"><dxe:TextEdit/></dxlc:LayoutItem>
            <dxlc:LayoutItem Label="Postal Code"><dxe:TextEdit/></dxlc:LayoutItem>
        </dxlc:LayoutGroup>
    </dxlc:LayoutGroup>

    <dxlc:LayoutGroup View="Tabs">
        <dxlc:LayoutGroup Header="Tab 1" Orientation="Vertical">
            <dxlc:LayoutItem Label="Item A"><dxe:TextEdit/></dxlc:LayoutItem>
        </dxlc:LayoutGroup>
        <dxlc:LayoutGroup Header="Tab 2" Orientation="Vertical">
            <dxlc:LayoutItem Label="Item B"><dxe:TextEdit/></dxlc:LayoutItem>
        </dxlc:LayoutGroup>
    </dxlc:LayoutGroup>
</dxlc:LayoutControl>
```

### LayoutItem

A `LayoutItem` is the label-plus-content wrapper. Its **content auto-aligns against the content of sibling `LayoutItem`s** — even across groups. That's the LayoutControl's superpower.

| Property | Use |
|---|---|
| `Label` | Label text |
| `LabelPosition` | `Left` (default), `Right`, `Top`, `Bottom` |
| `AddColonToLabel` | Auto-append `:` |
| `LabelTemplate` / `LabelStyle` | Custom label rendering |
| `Content` | The wrapped UIElement (or use element-syntax) |

```xaml
<dxlc:LayoutItem Label="Email" AddColonToLabel="True">
    <dxe:TextEdit EditValue="{Binding Email}"/>
</dxlc:LayoutItem>
```

### Nesting Rules

- `LayoutControl` is the root. It arranges children in **one row or column** (its `Orientation`).
- To split the layout into different orientations, **add `LayoutGroup`s** with the desired orientation.
- A `LayoutGroup` can contain `LayoutItem`s and/or other `LayoutGroup`s.
- A `LayoutGroup` must live inside a `LayoutControl` (or `DataLayoutControl`).
- The "alignment of content" applies across all groups in the same column — so `City:` and `Country:` field editors align even when in different sibling groups.

### Resizing Thumbs

Enable user-resizable items via attached properties:

```xaml
<dxlc:LayoutGroup dxlc:LayoutControl.AllowHorizontalSizing="True"
                  dxlc:LayoutControl.AllowVerticalSizing="True">
    ...
</dxlc:LayoutGroup>
```

### Customization Mode

```xaml
<dxlc:LayoutControl IsCustomization="True">
```

Lets end users drag-and-drop items between groups, hide items (which then appear in the Available Items panel), and resize. Persists via `WriteToXML` — see [save-restore-layout.md](save-restore-layout.md).

## DataLayoutControl — Auto-Generated Forms

`DataLayoutControl` inherits from `LayoutControl` but generates its content from a POCO via `DataAnnotations`. Set `CurrentItem` and the control builds the layout.

```xaml
<dxlc:DataLayoutControl CurrentItem="{Binding SelectedPerson}"/>
```

### Editor Selection by Type

| Property type | Editor |
|---|---|
| `string` | `TextEdit` |
| `bool` | `CheckEdit` |
| `DateTime` | `DateEdit` |
| Enum | `ComboBoxEdit` |
| Numeric | `SpinEdit` (with mask based on `[DataType]`) |

### `[Display]` Attribute Grammar

`Display.GroupName` controls the group hierarchy. Group names use a tiny mini-language:

| Notation | Effect |
|---|---|
| `Foo` | Group named "Foo". Default view: `GroupBox`. |
| `<Foo>` | `LayoutGroupView.Group` (titleless, borderless) |
| `[Foo]` | `LayoutGroupView.GroupBox` (with border + header) |
| `{Foo}` | `LayoutGroupView.Tabs` (tabbed) |
| `Foo/Bar` | Nested: `Bar` inside `Foo` |
| `Foo\|` | Items arranged **vertically** inside the group |
| `Foo-` | Items arranged **horizontally** inside the group |

Without orientation markers, the group orients itself perpendicular to its parent (so nested groups create the typical alternating layout).

### Example

```csharp
public class Person {
    [Display(GroupName = "<Name>", Name = "First name", Order = 0)]
    public string FirstName { get; set; } = "";

    [Display(GroupName = "<Name>", Name = "Last name", Order = 1)]
    public string LastName { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact", Order = 2), DataType(DataType.PhoneNumber)]
    public string Phone { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact", Order = 4)]
    public string Email { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact/Address", ShortName = "")]
    public string AddressLine1 { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact/Address", ShortName = "")]
    public string AddressLine2 { get; set; } = "";

    [Display(GroupName = "Personal-", Name = "Birth date")]
    public DateTime BirthDate { get; set; }

    [Display(GroupName = "Personal-", Order = 3)]
    public Gender Gender { get; set; }

    [Display(GroupName = "{Tabs}/Job", Order = 6)]
    public string Group { get; set; } = "";

    [Display(GroupName = "{Tabs}/Job", Name = "Hire date")]
    public DateTime HireDate { get; set; }

    [Display(GroupName = "{Tabs}/Job"), DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    [Display(GroupName = "{Tabs}/Job", Order = 7)]
    public string Title { get; set; } = "";
}

public enum Gender { Male, Female }
```

### Supported Attributes

| Attribute | Parameter | Effect |
|---|---|---|
| `Display` | `GroupName` | Place the field in a group (see grammar above) |
| `Display` | `Name` | Label text (default = property name) |
| `Display` | `ShortName` | Same as `Name` (empty string hides the label) |
| `Display` | `Order` | Position within the group; omit to append |
| `DataType` | `DataType.Currency` | Currency mask, right-aligned |
| `DataType` | `DataType.PhoneNumber` | Text editor with phone mask |
| `DataType` | `DataType.Password` | `PasswordBoxEdit` |
| `DataType` | `DataType.MultilineText` | Multi-line `TextEdit`, min height 50, stretch vertically |

> For the full list, see https://docs.devexpress.com/content/WPF/16863?md=true in DevExpress docs.

## TileLayoutControl — Modern UI Tiles

Tiles have **fixed pixel sizes** controlled by `Tile.Size`. The layout engine packs tiles from top to bottom, then across.

### Tile Sizes (Fixed by Enum)

| `Tile.Size` | Pixels |
|---|---|
| `ExtraSmall` | 70×70 |
| `Small` | 150×150 |
| `Large` | 310×150 (wide) |
| `ExtraLarge` | 310×310 |

> `Width` and `Height` on `Tile` are **ignored** for positioning. Layout order is driven by `Size` only.

### Tile Anatomy

```xaml
<dxlc:Tile Size="Large"
           Header="Sales Today"
           Background="#FF1976D2"
           Foreground="White"
           Padding="12"
           HorizontalHeaderAlignment="Left"
           VerticalHeaderAlignment="Bottom"
           Click="OnSalesClick">
    <dxlc:Tile.Content>
        <TextBlock Text="$42,300" FontSize="36"/>
    </dxlc:Tile.Content>
</dxlc:Tile>
```

| Tile property | Use |
|---|---|
| `Size` | The size enum value |
| `Header` | Caption (set to empty string to hide) |
| `Content` / `ContentTemplate` | Body |
| `ContentSource` / `ContentChangeInterval` | Auto-cycling animation source |
| `Click` / `Command` | Action when clicked |
| `BorderBrush` / `BorderThickness` | Border |

### Tile Groups

Mark a tile as the start of a new group with `FlowLayoutControl.IsFlowBreak="True"` and supply a header via `TileLayoutControl.GroupHeader`:

```xaml
<dxlc:TileLayoutControl>
    <dxlc:Tile Header="Mail"
               dxlc:TileLayoutControl.GroupHeader="Today"
               Size="Small"/>
    <dxlc:Tile Header="Calendar" Size="Large"/>

    <dxlc:Tile Header="Stock"
               dxlc:FlowLayoutControl.IsFlowBreak="True"
               dxlc:TileLayoutControl.GroupHeader="Inventory"
               Size="Small"/>
    <dxlc:Tile Header="Alerts" Size="Small"/>
</dxlc:TileLayoutControl>
```

`FlowLayoutControl.LayerSpace` (set on the tile control) controls the gap between groups.

### Drag-and-Drop

`TileLayoutControl` inherits `FlowLayoutControl.AllowItemMoving`. To disable:

```xaml
<dxlc:TileLayoutControl AllowItemMoving="False">
```

The `ItemPositionChanged` event fires after each move.

## FlowLayoutControl — Wrapping Cards

Items flow into rows (when `Orientation="Horizontal"`) or columns (when `Vertical`). Wrap happens at the control's edge.

### Anatomy

```xaml
<dxlc:FlowLayoutControl Orientation="Horizontal"
                        BreakFlowToFit="True"
                        AllowItemMoving="True"
                        ShowLayerSeparators="True"
                        AllowLayerSizing="True">
    <dxlc:GroupBox Header="Card 1" Width="200" Height="150"/>
    <dxlc:GroupBox Header="Card 2" Width="200" Height="150"/>
    <dxlc:GroupBox Header="Card 3"
                   Width="200" Height="150"
                   dxlc:FlowLayoutControl.IsFlowBreak="True"/>
    <dxlc:GroupBox Header="Card 4" Width="200" Height="150"/>
</dxlc:FlowLayoutControl>
```

### Behavior Properties

| Property | Effect |
|---|---|
| `Orientation` | Flow direction (`Horizontal` = rows, `Vertical` = columns) |
| `BreakFlowToFit` (default `true`) | Auto-wrap to a new row/column |
| `StretchContent` | Stretch items to control width/height (single line) |
| `IsFlowBreak` (attached) | Force a flow break at this item |
| `ShowLayerSeparators` / `AllowLayerSizing` | Show drag-bars between rows/columns |
| `AllowItemMoving` | End-user drag-drop |

### Maximize-One-Take-the-Rest

A `FlowLayoutControl` can dedicate most of its area to one item, arranging the others in a strip alongside:

```xaml
<dxlc:FlowLayoutControl MaximizedElementPosition="Top"
                        AllowMaximizedElementMoving="True">
    <dxlc:GroupBox Header="Detail"
                   MaximizeElementVisibility="Visible"/>
    <dxlc:GroupBox Header="Sidebar 1"/>
    <dxlc:GroupBox Header="Sidebar 2"/>
</dxlc:FlowLayoutControl>
```

The user can click the GroupBox's Maximize button to elevate it; others arrange in a single strip on the specified edge.

To maximize programmatically:

```csharp
flowControl.MaximizedElement = detailGroupBox;
```

## DockLayoutControl — Edge Docking in One Panel

Children dock to one of five positions via the `DockLayoutControl.Dock` attached property.

```xaml
<dxlc:DockLayoutControl>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Top"
                   Height="70"
                   Header="Toolbar"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Right"
                   Width="200"
                   Header="Properties"
                   dxlc:DockLayoutControl.AllowHorizontalSizing="True"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Left"
                   Width="200"
                   Header="Navigation"
                   dxlc:DockLayoutControl.AllowHorizontalSizing="True"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Client"
                   Header="Content"/>
</dxlc:DockLayoutControl>
```

### Dock Values

| `Dock` | Position |
|---|---|
| `Top` | Top edge, full width of remaining space |
| `Bottom` | Bottom edge |
| `Left` | Left edge, full height of remaining space |
| `Right` | Right edge |
| `Client` (default for the last child) | Fills the remaining area |

**Important**: dock order matters. Items dock in the order they appear in the XAML, with each item filling the **available remaining space** (not the whole panel). So the same children in a different order produce different layouts. This works exactly like the WPF `DockPanel.LastChildFill`-style logic.

### Runtime Resizing

```xaml
<dxlc:GroupBox dxlc:DockLayoutControl.Dock="Right"
               dxlc:DockLayoutControl.AllowHorizontalSizing="True"
               dxlc:DockLayoutControl.AllowVerticalSizing="True"/>
```

Shows a sizer next to the docked item.

To temporarily disable all resizers:

```xaml
<dxlc:DockLayoutControl AllowItemSizing="False">
```

## DockLayoutManager — Visual-Studio-Style Shell

`DockLayoutManager` (in the `dxdo:` namespace) is structurally different — items are **`LayoutPanel`** (regular dockable panels), **`DocumentGroup`** (MDI tab strip), and **`DocumentPanel`** (individual document tabs), all arranged via **`LayoutGroup`** containers (in the `dxdo:` namespace — distinct from the `dxlc:LayoutGroup`).

### Structure

```
DockLayoutManager
└── dxdo:LayoutGroup (horizontal or vertical orientation)
    ├── dxdo:LayoutPanel (Caption="Solution Explorer")
    ├── dxdo:DocumentGroup
    │   ├── dxdo:DocumentPanel (Caption="Main.cs")
    │   └── dxdo:DocumentPanel (Caption="App.xaml")
    └── dxdo:LayoutPanel (Caption="Properties")
```

### Snippet

```xaml
<dxdo:DockLayoutManager>
    <dxdo:LayoutGroup Orientation="Horizontal">

        <dxdo:LayoutPanel Caption="Solution Explorer" ItemWidth="250">
            <TreeView/>
        </dxdo:LayoutPanel>

        <dxdo:DocumentGroup ItemWidth="*">
            <dxdo:DocumentPanel Caption="Start Page">
                <TextBlock Text="Welcome"/>
            </dxdo:DocumentPanel>
            <dxdo:DocumentPanel Caption="MainWindow.xaml">
                <TextBlock Text="XAML editor"/>
            </dxdo:DocumentPanel>
        </dxdo:DocumentGroup>

        <dxdo:LayoutGroup Orientation="Vertical" ItemWidth="250">
            <dxdo:LayoutPanel Caption="Properties"/>
            <dxdo:LayoutPanel Caption="Output"/>
        </dxdo:LayoutGroup>

    </dxdo:LayoutGroup>
</dxdo:DockLayoutManager>
```

### Key Element Types

| Element | Role |
|---|---|
| `LayoutPanel` | A single dockable panel — like a Properties window |
| `DocumentGroup` | Tabbed group of documents (MDI) |
| `DocumentPanel` | An individual document tab |
| `LayoutGroup` (dxdo:) | Container that arranges panels horizontally or vertically (NOT the same as `dxlc:LayoutGroup`) |
| `AutoHideGroup` | Pinned/unpinned panels group (auto-hide behavior) |
| `FloatGroup` | Floating window group |

### Sizing

| Property | Use |
|---|---|
| `ItemWidth` / `ItemHeight` | Fixed pixel size, or `*` for star-sizing |
| `Caption` | Panel header text |
| `BindableName` | MVVM-friendly identifier for save/restore |

### Operations

- **Drag to move**: pick up a panel's header and dock it elsewhere
- **Float**: drag a panel out of its dock to make it a floating window
- **Pin / unpin (auto-hide)**: collapse a side-docked panel to a strip
- **Customization window**: right-click a panel for the panel-management menu

### MDI Bar/Ribbon Merging

When document panels host their own `RibbonControl` or bars, the parent `DockLayoutManager` can merge them into the host's command surface. See the `devexpress-wpf-ribbon-and-bars` skill, `merging.md` reference.

### Document Selector

Press `Ctrl+Tab` to bring up the document selector — a list of currently open panels/documents to cycle through.

## Choosing Container vs Item Type

### Inside `LayoutControl` / `DataLayoutControl`

| Container | Items |
|---|---|
| `LayoutControl` | `LayoutGroup`, `LayoutItem`, occasionally raw `UIElement` |
| `LayoutGroup` | `LayoutGroup` (nested), `LayoutItem` |
| `LayoutItem` | Any `UIElement` (typically an editor) |
| `DataLayoutControl` | Auto-generated; or override individual items via XAML |

### Inside `DockLayoutManager`

| Container | Items |
|---|---|
| `DockLayoutManager` | `dxdo:LayoutGroup`, `LayoutPanel`, `DocumentGroup` |
| `dxdo:LayoutGroup` | `LayoutPanel`, `DocumentGroup`, nested `dxdo:LayoutGroup` |
| `LayoutPanel` | Any `UIElement` (its content) |
| `DocumentGroup` | `DocumentPanel`s only |
| `DocumentPanel` | Any `UIElement` |

## Common Issues

- **LayoutControl items don't auto-align across groups** — they always do unless an item has a manual `Margin`. Don't set `Margin` on `LayoutItem`s.
- **`LayoutGroup` resolves to the wrong type** — typed it without the namespace prefix. Use `<dxlc:LayoutGroup>` or `<dxdo:LayoutGroup>` explicitly.
- **DataLayoutControl ignores attributes** — wrong `Display` import. Use `System.ComponentModel.DataAnnotations`.
- **Tile order looks wrong** — Tiles sort by `Size` only. `Width`/`Height` don't influence layout.
- **DockLayoutControl items overlap** — dock order matters. The last child without `Dock` (or with `Dock="Client"`) fills the remainder.
- **DockLayoutManager DocumentPanel content not visible** — `DocumentPanel` must live inside a `DocumentGroup`, not directly inside a `LayoutGroup`.
- **Flow items don't wrap** — `BreakFlowToFit="False"`. Set it to `true` (default) to wrap.
- **Tab content not loaded when tab is inactive** — by default, only the active tab loads. For initialization at startup, set `LayoutGroup.TabContentCacheMode="CacheAllTabs"` (DockLayoutManager) or equivalent on the LayoutControl tabs.

## Source Material

- `articles/controls-and-libraries/layout-management/tile-and-layout/layout-and-data-layout-controls/layout-control.md` (https://docs.devexpress.com/content/WPF/8147?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/layout-and-data-layout-controls/data-layout-control.md` (https://docs.devexpress.com/content/WPF/11540?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/layout-and-data-layout-controls/layout-items-and-groups.md` (https://docs.devexpress.com/content/WPF/8150?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/tile-layout-control.md` (https://docs.devexpress.com/content/WPF/11541?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/flow-layout-control.md` (https://docs.devexpress.com/content/WPF/8148?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/dock-layout-control.md` (https://docs.devexpress.com/content/WPF/8149?md=true)
- `articles/controls-and-libraries/layout-management/dock-windows.md` (https://docs.devexpress.com/content/WPF/6191?md=true)
