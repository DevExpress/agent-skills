# Data Binding — DevExpress WPF Accordion Control

The Accordion can be populated in four ways: **static items in XAML**, **`ChildrenPath`** (homogeneous data), **`ChildrenSelector`** (mixed types), and **`HierarchicalDataTemplate`** (rich per-level templates). Pick based on whether the menu is static, the data is uniform, and how much per-level customization you need.

## When to Use This Reference

Use this when you need to:

- Decide between static XAML items and a data-bound collection
- Bind to a tree where all items are the same class
- Bind to a tree where roots and leaves are different classes
- Use a `HierarchicalDataTemplate` for rich per-level rendering
- Bind to a `CollectionView`

## Population Approach Picker

| Your data | Approach | Notes |
|---|---|---|
| Fixed list of menu entries known at compile time | **Static items in XAML** | Simplest. Header / Glyph defined inline. |
| Tree of one class with a `Children` property | **`ChildrenPath`** | Most common case. Set `ItemsSource` + `ChildrenPath` + `DisplayMemberPath`. |
| Tree with different classes per level (e.g., `Category` → `Item`) | **`ChildrenSelector`** | Implement `IChildrenSelector`. Use `DataTemplate` resources keyed by `DataType`. |
| Need rich per-level templates or want to use standard WPF idioms | **`HierarchicalDataTemplate`** | Set `ItemTemplate`. Familiar to anyone who's used `TreeView`. |

> **Don't mix static items and `ItemsSource`.** If `ItemsSource` is set, inline `<dxa:AccordionItem>` definitions are ignored.

## Static Items in XAML

For sidebars where the menu structure is fixed, defining items in XAML is the cleanest approach.

```xaml
<dxa:AccordionControl>
    <dxa:AccordionItem Header="Root Item"
                       Glyph="{dx:DXImage Image=Image_32x32.png}">
        <dxa:AccordionItem Header="SubItem"
                           Glyph="{dx:DXImage Image=Map_16x16.png}"/>
        <dxa:AccordionItem Header="SubItem"
                           Glyph="{dx:DXImage Image=Map_16x16.png}">
            <dxa:AccordionItem Header="Nested SubItem"/>
        </dxa:AccordionItem>
    </dxa:AccordionItem>
</dxa:AccordionControl>
```

`AccordionItem` is its own container — children go directly inside as elements. Use `Header` for the caption (or any `UIElement`) and `Glyph` for the icon.

Add commands or events as you'd expect:

```xaml
<dxa:AccordionItem Header="Save" Glyph="..."
                   Click="OnSaveClick"/>
```

Items are also reachable from code through `AccordionControl.Items` (root) and `AccordionItem.Items` (children).

## ChildrenPath — Homogeneous Data

When every item in the tree is the same class with a child-collection property:

### Required Properties

| Property | Use |
|---|---|
| `AccordionControl.ItemsSource` | The root collection |
| `AccordionControl.ChildrenPath` | Property name on each item that holds children |
| `AccordionControl.DisplayMemberPath` | Property used as the header text (case-sensitive) |

### Example

```csharp
public class MenuItem {
    public string Caption { get; set; } = "";
    public List<MenuItem> SubItems { get; set; } = new();
}

public class ViewModel {
    public Menu AppMenu { get; } = new();
}

public class Menu {
    public List<MenuItem> MenuItems { get; } = new() {
        new() {
            Caption = "File",
            SubItems = {
                new() { Caption = "New" },
                new() { Caption = "Open" },
            }
        },
        new() { Caption = "Edit" },
    };
}
```

```xaml
<dxa:AccordionControl ItemsSource="{Binding AppMenu.MenuItems}"
                      ChildrenPath="SubItems"
                      DisplayMemberPath="Caption"/>
```

This produces a two-level menu (File → New / Open, Edit).

### When the Header Needs Customization

If `DisplayMemberPath` isn't enough (you need icons, multi-line text, or formatted captions), switch to `ItemTemplate` (HierarchicalDataTemplate). See the section below.

## ChildrenSelector — Mixed Types

When the root items are of one class and their children are of another:

```csharp
public class Category {
    public string CategoryName { get; set; } = "";
    public List<Item> Items { get; set; } = new();
}

public class Item {
    public string ItemName { get; set; } = "";
}
```

### Step 1: Implement `IChildrenSelector`

```csharp
public class MySelector : IChildrenSelector {
    public IEnumerable? SelectChildren(object item) {
        return item switch {
            Category c => c.Items,
            _          => null,
        };
    }
}
```

### Step 2: Wire It Up

```xaml
<Window.Resources>
    <local:MySelector x:Key="mySelector"/>
</Window.Resources>

<dxa:AccordionControl ItemsSource="{Binding MyData.Categories}"
                      ChildrenSelector="{StaticResource mySelector}">
    <dxa:AccordionControl.Resources>
        <DataTemplate DataType="{x:Type local:Category}">
            <TextBlock Text="{Binding CategoryName}"/>
        </DataTemplate>
        <DataTemplate DataType="{x:Type local:Item}">
            <TextBlock Text="{Binding ItemName}"/>
        </DataTemplate>
    </dxa:AccordionControl.Resources>
</dxa:AccordionControl>
```

`DataTemplate` resources keyed by `DataType` apply implicitly — one per item type. No `DisplayMemberPath` needed.

## HierarchicalDataTemplate

The most flexible option — use this when you want full WPF templating across multiple levels.

### Required Properties

| Property | Use |
|---|---|
| `AccordionControl.ItemsSource` | The root collection |
| `AccordionControl.ItemTemplate` | A `HierarchicalDataTemplate` |

### Example

```xaml
<dxa:AccordionControl ItemsSource="{Binding MyData.Categories}">
    <dxa:AccordionControl.ItemTemplate>
        <HierarchicalDataTemplate DataType="{x:Type local:Category}"
                                  ItemsSource="{Binding Items}">
            <TextBlock Text="{Binding CategoryName}"/>
            <HierarchicalDataTemplate.ItemTemplate>
                <DataTemplate DataType="{x:Type local:Item}">
                    <TextBlock Text="{Binding ItemName}"/>
                </DataTemplate>
            </HierarchicalDataTemplate.ItemTemplate>
        </HierarchicalDataTemplate>
    </dxa:AccordionControl.ItemTemplate>
</dxa:AccordionControl>
```

`HierarchicalDataTemplate.ItemsSource` declares how to navigate from a parent to its children. Nesting `HierarchicalDataTemplate` (or `DataTemplate` at leaves) lets each level render differently — icons + caption at root, plain text at leaves, multi-line layouts where needed.

Both `ChildrenSelector` and `HierarchicalDataTemplate` handle mixed-type hierarchies; pick `HierarchicalDataTemplate` when per-level styling is more important, `ChildrenSelector` when the children-navigation logic is custom.

## Binding to CollectionView

The Accordion supports binding to `ICollectionView` (the LINQ-friendly wrapper around collections with filter / sort / group). Enable it:

```xaml
<dxa:AccordionControl AllowCollectionView="True"
                      ItemsSource="{Binding GroupedView}"
                      ChildrenPath="Items"/>
```

`AllowCollectionView` defaults to `false` to preserve direct-collection behavior. When you bind to a `CollectionView` (or a `CollectionViewSource`), set it to `true`.

## Display the Selected Item

For MVVM bidirectional binding:

```xaml
<dxa:AccordionControl ItemsSource="{Binding MenuItems}"
                      ChildrenPath="SubItems"
                      DisplayMemberPath="Caption"
                      SelectedItem="{Binding CurrentItem, Mode=TwoWay}"/>
```

The bound `SelectedItem` is the underlying data object (e.g., a `MenuItem`), not the `AccordionItem` wrapper.

For NavigationPane view mode, also bind `SelectedRootItem` — see [view-modes.md](view-modes.md).

## Comparison

| Approach | Best for | Trade-off |
|---|---|---|
| Static XAML items | Fixed menus, designer-friendly | No data-driven updates |
| `ChildrenPath` | Uniform tree, simple captions | Limited template customization without `ItemTemplate` |
| `ChildrenSelector` | Mixed types, simple per-type templates | More code than `ChildrenPath` |
| `HierarchicalDataTemplate` | Rich per-level UI | More verbose |

## Common Issues

- **Static items don't appear** — `ItemsSource` is set elsewhere (e.g., via designer). `ItemsSource` takes precedence. Remove it to use inline items.
- **`ChildrenPath` does nothing** — name typo. The property must be public and case-sensitive.
- **Headers show the class name** — `DisplayMemberPath` not set and no `DataTemplate` matches. Either set `DisplayMemberPath` or add a `DataTemplate DataType="{x:Type local:YourClass}"` in resources.
- **Items added at runtime don't appear** — bound collection isn't an `ObservableCollection<T>` (or doesn't implement `INotifyCollectionChanged`). Switch to `ObservableCollection<T>`.
- **`HierarchicalDataTemplate.ItemsSource` ignored** — used `ChildrenPath` and `ItemTemplate` together — `ChildrenPath` overrides the template's `ItemsSource`. Pick one approach.
- **`AllowCollectionView` not honored** — must be `true` when binding to a `CollectionView`; default is `false`.

## Source Material

- `articles/controls-and-libraries/navigation-controls/accordion-control/data-binding.md` (`xref:118635`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/data-binding/children-path.md` (`xref:119810`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/data-binding/children-selector.md` (`xref:119809`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/data-binding/hierarchical-data-template.md` (`xref:119794`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items/adding-accordion-items.md` (`xref:119831`)
