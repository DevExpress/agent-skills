# Items and Item Links

A bar/ribbon UI is built from two related kinds of objects: **bar items** (the things you click) and **bar item links** (references to those things, used to make the same item appear in several bars). This page covers the inventory of item types, the item/link distinction, the base properties every item shares, and the two ways to define items: declaratively in XAML or generated from a ViewModel collection (MVVM).

## When to Use This Reference

Use this when you need to:

- Add a button, check button, split button, sub-menu, separator, editor, static text to a Ribbon group or bar
- Decide whether to write `BarButtonItem` or `BarButtonItemLink`
- Set glyphs and the small/medium/large icon variants
- Switch the item between glyph-only, content-only, or both
- Wire an item to a ViewModel command vs an event handler
- Generate items from a `ViewModel` collection

## Bar Item Inventory

All items live in `DevExpress.Xpf.Bars` (namespace prefix `dxb:`). Each item type has a corresponding `*Link` class.

| Item / Link | What it is |
|---|---|
| `BarButtonItem` / `BarButtonItemLink` | Regular click button |
| `BarCheckItem` / `BarCheckItemLink` | Two-state or three-state check button. Use `GroupIndex` to make a radio group. |
| `BarSplitButtonItem` / `BarSplitButtonItemLink` | Button with a separate dropdown arrow that opens a `PopupControl` |
| `BarSplitCheckItem` / `BarSplitCheckItemLink` | Combines check button + dropdown |
| `BarSubItem` / `BarSubItemLink` | Container item that opens a submenu of child items |
| `BarStaticItem` / `BarStaticItemLink` | Non-interactive static text (e.g., status text) |
| `BarItemSeparator` / `BarItemLinkSeparator` | Visual separator between items |
| `BarEditItem` / `BarEditItemLink` | Embeds a DXEditors editor (TextEdit, ComboBoxEdit, etc.) in a bar — uses `EditSettings` |
| `BarItemSelector` / `BarItemSelectorLink` | Container that allows only one selected child item |
| `BarLinkContainerItem` / `BarLinkContainerItemLink` | Logical group of links rendered inline (no submenu) |
| `ToolbarListItem` / `ToolbarListItemLink` | Built-in "show/hide toolbars" check list |
| `LinkListItem` / `LinkListItemLink` | Built-in "show/hide individual links" check list |
| `BarItemMenuHeader` / `BarItemLinkMenuHeader` | Header item shown at the top of a popup menu |
| `BarButtonGroup` / `BarButtonGroupLink` | Ribbon-only: small buttons that never break across rows |
| `RibbonGalleryBarItem` / `RibbonGalleryBarItemLink` | Ribbon-only: in-ribbon gallery |

## Items vs Links — The Core Distinction

- **Bar item** — the definition. Owns `Content`, `Glyph`, `Command`, `ItemClick`, state.
- **Bar item link** — a *reference* to a bar item. A bar/group/menu doesn't directly contain items; it contains links that point at items.

In simple XAML, the link is **created implicitly** when you place an item directly inside its container:

```xaml
<dxr:RibbonPageGroup Caption="Clipboard">
    <dxb:BarButtonItem Content="Paste"
                       Glyph="{dx:DXImage Image=Paste_16x16.png}"
                       Command="{Binding PasteCommand}"/>
</dxr:RibbonPageGroup>
```

You **must** write the link explicitly when:

1. The item is shared across multiple bars/menus
2. The item is defined inside a `BarManager.Items` collection and referenced from various bars

```xaml
<dxb:BarManager>
    <dxb:BarManager.Items>
        <dxb:BarButtonItem x:Name="bPaste" Content="Paste"
                           Glyph="{dx:DXImage Image=Paste_16x16.png}"
                           KeyGesture="Ctrl+V"
                           Command="{Binding PasteCommand}"/>
    </dxb:BarManager.Items>

    <DockPanel>
        <!-- Toolbar -->
        <dxb:ToolBarControl DockPanel.Dock="Top">
            <dxb:BarButtonItemLink BarItemName="bPaste"/>
        </dxb:ToolBarControl>

        <!-- Edit menu -->
        <dxb:MainMenuControl DockPanel.Dock="Top">
            <dxb:BarSubItem Content="Edit">
                <dxb:BarButtonItemLink BarItemName="bPaste"/>
            </dxb:BarSubItem>
        </dxb:MainMenuControl>
    </DockPanel>
</dxb:BarManager>
```

Both link references point at one `bPaste` item — change its content/command/glyph in one place, both locations update.

### Property Ownership

| Property class | Lives on |
|---|---|
| `Content`, `Glyph`, `LargeGlyph`, `MediumGlyph`, `Command`, `ItemClick`, `KeyGesture`, `IsEnabled` | **Item** (`BarItem`) — defined once |
| `RibbonStyle`, `IsVisible`, `MergeType`, `MergeOrder`, `BarItemDisplayMode` | **Link** (`BarItemLinkBase`) — can differ per location |

`BarItemDisplayMode` also has a fallback property on `BarItem` (used when the link doesn't override it); setting it on the item gives a default that every link inherits.

This split lets the same item appear as a large button on one Ribbon page and as a small icon in a popup menu, with different per-location styling.

## Base Properties Every Item Has

```xaml
<dxb:BarButtonItem
    Content="Paste"
    Glyph="{dx:DXImage Image=Paste_16x16.png}"
    LargeGlyph="{dx:DXImage Image=Paste_32x32.png}"
    MediumGlyph="{dx:DXImage Image=Paste_20x20.png}"
    BarItemDisplayMode="ContentAndGlyph"
    GlyphAlignment="Left"
    KeyGesture="Ctrl+V"
    Command="{Binding PasteCommand}"
    CommandParameter="{Binding SelectedItem}"
    ItemClick="OnPasteClick"
    Description="Paste from clipboard"
    ToolTip="Paste"
    IsEnabled="{Binding CanPaste}"/>
```

### Content & Glyph

| Property | Use |
|---|---|
| `Content` | Caption text (or any `UIElement`) |
| `Glyph` | 16×16 small icon |
| `LargeGlyph` | 32×32 large icon (used by Ribbon when group is wide) |
| `MediumGlyph` | 20×20 medium icon (used by Simplified Ribbon mode) |

Use `{dx:DXImage Image=...png}` for raster icons or `{dx:DXImage SvgImages/...svg}` for SVG. The Ribbon **automatically swaps** between small and large glyphs as the page resizes.

### Display Mode (`BarItemDisplayMode`)

| Value | Effect |
|---|---|
| `Default` | Inherits from the parent bar / ribbon |
| `Content` | Caption only |
| `ContentAndGlyph` | Both icon and caption |

Typically set on the **link** so the same item can render differently in different placements; the property also exists on `BarItem` as a fallback default. To hide a caption and keep only the glyph, use `BarItem.RibbonStyle="SmallWithoutText"` (Ribbon items) or assign empty `Content`. For finer control on Ribbon items, `BarItem.RibbonStyle` accepts `SmallWithText`, `SmallWithoutText`, `Large`, `Default`.

### Alignment

`GlyphAlignment` — `Left` (default), `Top`, `Right`, `Bottom`. Top-aligned glyphs above a caption is the classic "large Ribbon button" look.

## Wiring Item Actions

### MVVM: `Command` (preferred)

```xaml
<dxb:BarButtonItem Content="Save"
                   Glyph="{dx:DXImage SvgImages/Save/Save.svg}"
                   Command="{Binding SaveCommand}"
                   CommandParameter="{Binding CurrentDocument}"/>
```

`Command` is bound to a ViewModel command. `IsEnabled` automatically reflects the command's `CanExecute`.

### Code-Behind: `ItemClick` Event

```xaml
<dxb:BarButtonItem Content="Refresh"
                   ItemClick="OnRefreshClick"/>
```

```csharp
private void OnRefreshClick(object sender, ItemClickEventArgs e) {
    // ...
}
```

Use `ItemClick` only when MVVM doesn't fit (code-behind-only apps, prototypes).

### Keyboard Shortcuts

`KeyGesture="Ctrl+V"` registers the shortcut at the `BarManager` / `RibbonControl` level. The gesture is shown in tooltips automatically.

## MVVM — Generate Items from a Collection

For dynamic UI (item lists driven by data, plugin-style commands, role-based menus), bind a collection to one of the `*Source` properties and provide a `DataTemplate`. The template root **must be a `ContentControl`**.

### Per-Container `*Source` Properties

| Container | Source property | Template property |
|---|---|---|
| `ToolBarControl`, `MainMenuControl`, `StatusBarControl` | `ItemsSource` | `ItemTemplate` / `ItemTemplateSelector` |
| `Bar`, `BarSubItem` | `ItemLinksSource` | `ItemTemplate` / `ItemTemplateSelector` |
| `PopupMenu`, `ApplicationMenu` | `ItemLinksSource` | `ItemTemplate` / `ItemTemplateSelector` |
| `BarManager` | `BarsSource` | `BarTemplate` / `BarTemplateSelector` |
| `RibbonControl` | `CategoriesSource` | `CategoryTemplate` |
| `RibbonPageCategory` | `PagesSource` | `PageTemplate` |
| `RibbonPage` | `GroupsSource` | `GroupTemplate` |
| `RibbonPageGroup` | `ItemLinksSource` | `ItemTemplate` |
| `RibbonStatusBarControl` | `LeftItemLinksSource` / `RightItemLinksSource` | `LeftItemTemplate` / `RightItemTemplate` |
| `BackstageViewControl` | `ItemsSource` | `ItemTemplate` |

### Example — Toolbar Generated from ViewModel

ViewModel:

```csharp
public class ActionVM : ViewModelBase {
    public string Caption { get; set; } = "";
    public ImageSource? Glyph { get; set; }
    public ICommand? ExecuteCommand { get; set; }
}

public class MainViewModel : ViewModelBase {
    public ObservableCollection<ActionVM> Actions { get; } = new() {
        new ActionVM { Caption = "Cut",   ExecuteCommand = new DelegateCommand(() => { /*...*/ }) },
        new ActionVM { Caption = "Copy",  ExecuteCommand = new DelegateCommand(() => { /*...*/ }) },
        new ActionVM { Caption = "Paste", ExecuteCommand = new DelegateCommand(() => { /*...*/ }) },
    };
}
```

XAML:

```xaml
<dxb:ToolBarControl ItemsSource="{Binding Actions}">
    <dxb:ToolBarControl.ItemTemplate>
        <DataTemplate>
            <ContentControl>
                <dxb:BarButtonItem Content="{Binding Caption}"
                                   Glyph="{Binding Glyph}"
                                   Command="{Binding ExecuteCommand}"
                                   BarItemDisplayMode="ContentAndGlyph"/>
            </ContentControl>
        </DataTemplate>
    </dxb:ToolBarControl.ItemTemplate>
</dxb:ToolBarControl>
```

> **The `ContentControl` wrapper is required.** Without it, the template won't bind correctly. The actual bar item / link must be the *child* of the `ContentControl`.

### Example — Ribbon Page with ViewModel-Generated Items

```xaml
<dxr:RibbonPageGroup Caption="Actions"
                     ItemLinksSource="{Binding Actions}">
    <dxr:RibbonPageGroup.ItemTemplate>
        <DataTemplate>
            <ContentControl>
                <dxb:BarButtonItem Content="{Binding Caption}"
                                   Glyph="{Binding Glyph}"
                                   LargeGlyph="{Binding LargeGlyph}"
                                   Command="{Binding ExecuteCommand}"/>
            </ContentControl>
        </DataTemplate>
    </dxr:RibbonPageGroup.ItemTemplate>
</dxr:RibbonPageGroup>
```

### Implicit Data Templates (One Template Per Item Type)

If your ViewModel has different classes per item type, use implicit `DataType` templates:

```xaml
<Window.Resources>
    <DataTemplate DataType="{x:Type vm:ButtonItemVM}">
        <ContentControl>
            <dxb:BarButtonItem Content="{Binding Caption}"
                               Glyph="{Binding Glyph}"
                               Command="{Binding ExecuteCommand}"/>
        </ContentControl>
    </DataTemplate>
    <DataTemplate DataType="{x:Type vm:CheckItemVM}">
        <ContentControl>
            <dxb:BarCheckItem Content="{Binding Caption}"
                              IsChecked="{Binding IsChecked, Mode=TwoWay}"/>
        </ContentControl>
    </DataTemplate>
</Window.Resources>

<dxb:ToolBarControl ItemsSource="{Binding MixedActions}"/>
```

No explicit template assignment needed — WPF picks the right `DataTemplate` by item type.

### Template Selector (Conditional Templates)

If items share a class but render differently, implement a `DataTemplateSelector`:

```csharp
public class ActionTemplateSelector : DataTemplateSelector {
    public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
        var ctrl = (Control)container;
        return ((ActionVM)item).IsCheckable
            ? (DataTemplate)ctrl.FindResource("CheckItemTemplate")
            : (DataTemplate)ctrl.FindResource("ButtonItemTemplate");
    }
}
```

Assign to `ItemTemplateSelector`.

## Common Issues

- **`BarItemLink` shows nothing** — `BarItemName` doesn't match any `x:Name`, or the item is outside the scope of the `BarManager` owning the link.
- **Same item appears multiple times when generating** — used `Items` instead of `ItemsSource`, or `ItemLinks` instead of `ItemLinksSource`. The `*Source` properties are MVVM; the non-`*Source` are imperative collections.
- **Glyph not displayed** — `Glyph` set to a wrong path; use `{dx:DXImage Image=...png}` markup extension or a pack URI.
- **Command fires but `IsEnabled` doesn't update** — verify the command implements `ICommand.CanExecuteChanged` (DevExpress `DelegateCommand` does this automatically).
- **Three-state check button doesn't have an indeterminate state** — set `BarCheckItem.IsThreeState="True"`.
- **`MediumGlyph` ignored** — only used in Simplified Ribbon mode (`AllowSimplifiedRibbon="True"` + `IsSimplified="True"`). Always provide `Glyph` and `LargeGlyph` too.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/items-and-links.md` (`xref:118282`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/the-list-of-bar-items-and-links.md` (`xref:6646`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/displaying-glyphs.md` (`xref:117633`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/mvvm-support.md` (`xref:10434`)
