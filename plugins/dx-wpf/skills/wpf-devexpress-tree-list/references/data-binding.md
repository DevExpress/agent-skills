# Data Binding — DevExpress WPF TreeList

`TreeListControl` supports four ways to build a tree:

1. **Self-Referential** — flat data with Key + Parent fields (default)
2. **Hierarchical with `ChildNodesPath`** — same type per level, with a children property
3. **Hierarchical with `ChildNodesSelector` or `HierarchicalDataTemplate`** — different types per level
4. **Unbound Mode** — programmatic `TreeListNode` tree, no `ItemsSource`

## When to Use This Reference

Use this when you need to:

- Pick the right binding strategy for your data shape
- Build a tree from a flat list of records with Parent-ID fields
- Build a tree from nested object collections
- Build a tree where each level is a different type
- Fetch child nodes asynchronously
- Build a tree programmatically without a data source
- Bind expand state to a ViewModel property

## Key Properties (`DevExpress.Xpf.Grid.TreeListView`)

| Property | Mode | Description |
|---|---|---|
| `KeyFieldName` | Self-referential | Field that uniquely identifies each record. |
| `ParentFieldName` | Self-referential | Field that points to the parent record's key. |
| `ChildNodesPath` | Hierarchical (same type) | The property name on the data item that holds its children. |
| `ChildNodesSelector` | Hierarchical (different types) | An `IChildNodesSelector` implementation. |
| `TreeDerivationMode` | Mode switch | `Selfreference` (default), `ChildNodesSelector`, or `HierarchicalDataTemplate`. (There is **no** `ChildNodesPath` mode — for a children-field, use `ChildNodesSelector` mode + the `ChildNodesPath` property.) |
| `Nodes` | Unbound | Root `TreeListNodeCollection` populated in XAML or code. |

Source: `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`.

## Self-Referential

Most common pattern. Flat list of records, each with an `Id` and `ParentId`.

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView KeyFieldName="ID"
                          ParentFieldName="ParentID"
                          AutoWidth="True"
                          AutoExpandAllNodes="False"/>
    </dxg:TreeListControl.View>
    <dxg:TreeListColumn FieldName="Name"/>
    <dxg:TreeListColumn FieldName="Position"/>
</dxg:TreeListControl>
```

```csharp
public class Employee {
    public int ID { get; set; }
    public int? ParentID { get; set; }    // null for the root
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}
```

**Root detection**: a record is a root when its `ParentID` value doesn't match any other record's `ID`. Using a nullable key type (e.g., `int?`) and setting the root's `ParentID` to `null` is the safest pattern.

Source: `articles/controls-and-libraries/tree-list/getting-started/lesson-1-add-a-treelistcontrol-to-a-project.md`.

## Hierarchical with `ChildNodesPath` (Same Type)

Each item has a `Children` collection of the same type. Set the `ChildNodesPath` property to the
children-field name **and** `TreeDerivationMode="ChildNodesSelector"` (there is no `ChildNodesPath`
mode value).

```xaml
<dxg:TreeListControl ItemsSource="{Binding Departments}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView TreeDerivationMode="ChildNodesSelector"
                          ChildNodesPath="Children"
                          AutoWidth="True"/>
    </dxg:TreeListControl.View>
    <dxg:TreeListColumn FieldName="Name"/>
    <dxg:TreeListColumn FieldName="HeadCount"/>
</dxg:TreeListControl>
```

```csharp
public class Department {
    public string Name { get; set; } = string.Empty;
    public int HeadCount { get; set; }
    public List<Department> Children { get; set; } = new();
}
```

The grid walks `Children` recursively. `ItemsSource` should be the top-level (root) collection.

## Hierarchical with `ChildNodesSelector` (Different Types)

Use when child levels have different .NET types. Implement `DevExpress.Xpf.Grid.IChildNodesSelector`:

```csharp
using DevExpress.Xpf.Grid;
using System.Collections;

public class OrgChartChildSelector : IChildNodesSelector {
    public IEnumerable SelectChildren(object item) => item switch {
        Department d => d.Teams,
        Team       t => t.Members,
        _          => null
    };
}
```

```xaml
<Window.Resources>
    <local:OrgChartChildSelector x:Key="ChildSelector"/>
</Window.Resources>
<dxg:TreeListControl ItemsSource="{Binding RootDepartments}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView TreeDerivationMode="ChildNodesSelector"
                          ChildNodesSelector="{StaticResource ChildSelector}"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

> The interface `IChildNodesSelector` and the exact selector pattern are described in `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`. If the precise member signatures differ in your DevExpress version, verify via DxDocs MCP:
> `devexpress_docs_search(technology="WPF TreeList", query="IChildNodesSelector SelectChildren")`

## Asynchronous Child-Nodes Selector

For trees backed by remote services or DB queries, fetch children on demand so the UI stays responsive. The article (https://docs.devexpress.com/content/WPF/10366?md=true#fetch-nodes-asynchronously) covers this pattern. The interface is `IAsyncChildNodesSelector` (or similar — verify):

```csharp
// TODO: Verify exact interface name (IAsyncChildNodesSelector vs IChildNodesSelectorAsync).
// devexpress_docs_search(technology="WPF TreeList",
//                        query="asynchronous child nodes selector fetch on demand")
```

## Hierarchical Data Templates

For type-specific rendering (e.g., a Department row vs. an Employee row should look different):

```xaml
<dxg:TreeListView TreeDerivationMode="HierarchicalDataTemplate">
    <dxg:TreeListView.Resources>
        <HierarchicalDataTemplate DataType="{x:Type local:Department}"
                                  ItemsSource="{Binding Teams}">
            <TextBlock Text="{Binding Name}" FontWeight="Bold"/>
        </HierarchicalDataTemplate>
        <HierarchicalDataTemplate DataType="{x:Type local:Team}"
                                  ItemsSource="{Binding Members}">
            <TextBlock Text="{Binding Name}"/>
        </HierarchicalDataTemplate>
        <DataTemplate DataType="{x:Type local:Employee}">
            <TextBlock Text="{Binding FullName}"/>
        </DataTemplate>
    </dxg:TreeListView.Resources>
</dxg:TreeListView>
```

> Hierarchical Data Templates are incompatible with **Optimized Mode** and **Conditional Formatting** — use only when type-per-level rendering is essential.

## Unbound Mode (No `ItemsSource`)

Build the tree programmatically. Useful for static trees, in-memory structures not modeled as collections, or one-off views.

### XAML

```xaml
<dxg:TreeListControl>
    <dxg:TreeListControl.Columns>
        <dxg:TreeListColumn FieldName="Name"/>
        <dxg:TreeListColumn FieldName="Executor"/>
    </dxg:TreeListControl.Columns>
    <dxg:TreeListControl.View>
        <dxg:TreeListView>
            <dxg:TreeListView.Nodes>
                <dxg:TreeListNode>
                    <dxg:TreeListNode.Content>
                        <local:ProjectObject Name="Project: Betaron" Executor="Destiny Tabisola"/>
                    </dxg:TreeListNode.Content>
                    <dxg:TreeListNode.Nodes>
                        <dxg:TreeListNode>
                            <dxg:TreeListNode.Content>
                                <local:ProjectObject Name="Development" Executor="Kairra Hogg"/>
                            </dxg:TreeListNode.Content>
                        </dxg:TreeListNode>
                    </dxg:TreeListNode.Nodes>
                </dxg:TreeListNode>
            </dxg:TreeListView.Nodes>
        </dxg:TreeListView>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

### Code

```csharp
using DevExpress.Xpf.Grid;

void BuildTree() {
    var root = new TreeListNode(new ProjectObject { Name = "Project: Stanton", Executor = "N. Llams" });
    treeListView1.Nodes.Add(root);

    var info = new TreeListNode(new ProjectObject { Name = "Information Gathering", Executor = "A. Galva" });
    root.Nodes.Add(info);

    var design = new TreeListNode(new ProjectObject { Name = "Design", Executor = "R. Felton" });
    info.Nodes.Add(design);
}
```

Source: `articles/controls-and-libraries/tree-list/getting-started/lesson-2-build-a-tree-in-unbound-mode.md`.

## Bind Expand State

To restore expansion across sessions, bind each `TreeListNode.ExpandStateBinding` to a property on the data item:

```xaml
<dxg:TreeListNode ExpandStateBinding="{Binding IsExpanded}">
    <dxg:TreeListNode.Content>
        <local:Item Name="Root"/>
    </dxg:TreeListNode.Content>
</dxg:TreeListNode>
```

Or set it programmatically via `Binding`. For bound mode, `TreeListView` has expand-state events (verify exact names via DxDocs MCP).

## Choosing the Right Mode

| Data shape | Mode | Property |
|---|---|---|
| Flat list with `Id` / `ParentId` | `Selfreference` (default) | `KeyFieldName`, `ParentFieldName` |
| Nested same-type objects with `Children` | `ChildNodesSelector` | `ChildNodesPath` |
| Nested different-type objects | `ChildNodesSelector` | implement `IChildNodesSelector` |
| Remote data, lazy loading | Async selector | (verify exact interface) |
| Type-specific row visuals | `HierarchicalDataTemplate` | `<HierarchicalDataTemplate>` resources |
| No data source / programmatic | Unbound | `TreeListView.Nodes` |

## Source Material

- `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`
- `articles/controls-and-libraries/tree-list/getting-started/lesson-1-add-a-treelistcontrol-to-a-project.md`
- `articles/controls-and-libraries/tree-list/getting-started/lesson-2-build-a-tree-in-unbound-mode.md`
