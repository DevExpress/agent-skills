# Master-Detail — DevExpress WPF Data Grid

The Data Grid supports master-detail layouts where each master row can expand to show one or more **detail panes**. Detail content can be a **nested grid**, **custom UI** (chart, form, anything you can put in a `DataTemplate`), or a **tabbed layout** combining multiple kinds. The detail descriptor selection can also vary per row based on master data.

## When to Use This Reference

Use this when you need to:

- Pick between **Data Grid in Details**, **Custom Content in Details**, **Tabbed View**, or **Data-Dependent Details** (decision matrix below)
- Display nested grids per master row (Orders → OrderItems)
- Display a `ChartControl`, form, or other UI per master row
- Display multiple details for the same master row in tabs
- Show **different detail kinds for different master rows** (`DetailDescriptorSelector`)
- Hide expand buttons conditionally (`IsDetailButtonVisibleBinding`, `ShowDetailButtons`)
- Expand / collapse master rows programmatically
- Bind focused master / detail rows to a ViewModel
- Search across master AND detail data
- Handle the multi-selection / server-mode / fixed-row caveats
- Use `RowDetailsTemplate` for focused-row-only inline details (alternative to master-detail descriptors)

## Decision Matrix — Which Detail Kind?

| Detail kind | Descriptor class | Use when | Limitations |
|---|---|---|---|
| **Nested data grid** | `DataControlDetailDescriptor` | Detail data is tabular (Orders → OrderItems) | Detail grids share column layout — column reorder affects all instances |
| **Custom UI** (chart, form, gauge, user control) | `ContentDetailDescriptor` | Detail data is visual / non-tabular | Cannot export the template content |
| **Multiple tabs** (mix of grids + custom UI) | `TabViewDetailDescriptor` | Single master needs several different views (Orders + Sales Chart + Notes) | First nine tabs via `<kbd>Ctrl</kbd>+<kbd>1..9</kbd>` — only when wrapped in `DataControlDetailDescriptor` |
| **Different details per row** | `DetailDescriptorSelector` + `DetailDescriptorTrigger` | Master rows have different types and need different details (VP → Subordinates, Sales Rep → Customers) | Selector can only bind to master row's properties |
| **Focused-row inline expansion** (alternative to descriptors) | `TableView.RowDetailsTemplate` | Show extra info for the focused row only, inline below it | Single-row; not a per-row collection |

Source: `articles/controls-and-libraries/data-grid/master-detail-data-representation.md` plus subtopics in `master-detail/`.

## Setup: Master Grid

```xaml
<dxg:GridControl ItemsSource="{Binding Customers}">
    <dxg:GridControl.View>
        <dxg:TableView AllowMasterDetail="True"
                       ShowDetailButtons="True"/>
    </dxg:GridControl.View>

    <!-- Detail descriptor goes here (see below) -->
</dxg:GridControl>
```

`TableView.AllowMasterDetail="True"` is the master switch (default `true`). Set to `false` to disable details everywhere.

## Pattern 1: Data Grid in Details

The most common case: each master row expands to a nested grid bound to a collection property of the master.

```xaml
<dxg:GridControl ItemsSource="{Binding Customers}">
    <dxg:GridControl.View>
        <dxg:TableView/>
    </dxg:GridControl.View>
    <dxg:GridColumn FieldName="CustomerId"/>
    <dxg:GridColumn FieldName="CompanyName"/>

    <dxg:GridControl.DetailDescriptor>
        <dxg:DataControlDetailDescriptor ItemsSourcePath="Orders"
                                         ShowHeader="True">
            <dxg:DataControlDetailDescriptor.DataControl>
                <dxg:GridControl AutoGenerateColumns="AddNew"
                                 EnableSmartColumnsGeneration="True">
                    <dxg:GridControl.View>
                        <dxg:TableView AutoWidth="True"
                                       DetailHeaderContent="Orders"/>
                    </dxg:GridControl.View>
                </dxg:GridControl>
            </dxg:DataControlDetailDescriptor.DataControl>
        </dxg:DataControlDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

### Source Property Binding Variants

`DataControlDetailDescriptor` supports three ways to bind the detail's data source:

| Property | Use when |
|---|---|
| `ItemsSourcePath="Orders"` | Master row has a property `Orders` (string field name) |
| `ItemsSourceBinding="{Binding Orders}"` | Need a `Binding` with converter / mode |
| `ItemsSourceValueConverter="{StaticResource MyConverter}"` | Transform the master row into a detail collection |

```xaml
<dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding OrderHistory}">
    <!-- ... -->
</dxg:DataControlDetailDescriptor>
```

Source: `articles/controls-and-libraries/data-grid/master-detail/data-grid-in-details.md`.

### Nested Detail Layouts (Multi-Level)

```xaml
<dxg:GridControl ItemsSource="{Binding Data}"
                 AutoGenerateColumns="AddNew"
                 CurrentItem="{Binding Level1Item}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding Items}">
            <dxg:GridControl AutoGenerateColumns="AddNew"
                             CurrentItem="{Binding Level2Item}">
                <dxg:GridControl.DetailDescriptor>
                    <dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding Items}">
                        <dxg:GridControl AutoGenerateColumns="AddNew"
                                         CurrentItem="{Binding Level3Item}"/>
                    </dxg:DataControlDetailDescriptor>
                </dxg:GridControl.DetailDescriptor>
            </dxg:GridControl>
        </dxg:DataControlDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

The total number of levels is fixed by the XAML tree; you cannot add levels at runtime.

## Pattern 2: Custom Content (ContentDetailDescriptor)

Display any UI per master row — chart, form, gauge, user control:

```xaml
<Window.Resources>
    <DataTemplate x:Key="ordersChartTemplate">
        <dxc:ChartControl DataSource="{Binding Orders}">
            <dxc:ChartControl.Titles>
                <dxc:Title Content="{Binding CompanyName}"/>
            </dxc:ChartControl.Titles>
            <dxc:XYDiagram2D>
                <dxc:LineSeries2D ArgumentDataMember="OrderDate"
                                  ValueDataMember="Freight"/>
            </dxc:XYDiagram2D>
        </dxc:ChartControl>
    </DataTemplate>
</Window.Resources>

<dxg:GridControl ItemsSource="{Binding Customers}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:ContentDetailDescriptor ContentTemplate="{StaticResource ordersChartTemplate}"/>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

The template's `DataContext` is the **master row's bound object** (e.g., `Customer`). Inside the template, bind to master row properties or its child collections.

> **Stacked details**: If you need to show multiple details per master row at once (not in tabs), wrap them in a single `ContentDetailDescriptor` with a UserControl that contains both. The Data Grid does not support stacked detail descriptors directly.

Source: `articles/controls-and-libraries/data-grid/master-detail/custom-content-in-details.md`.

## Pattern 3: Tabbed View (TabViewDetailDescriptor)

Show multiple details per master row organized into tabs:

```xaml
<dxg:GridControl ItemsSource="{Binding Employees}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:TabViewDetailDescriptor>
            <dxg:TabViewDetailDescriptor.DetailDescriptors>

                <!-- Tab 1: nested grid -->
                <dxg:DataControlDetailDescriptor ItemsSourcePath="Orders"
                                                 HeaderContent="Orders">
                    <dxg:DataControlDetailDescriptor.DataControl>
                        <dxg:GridControl AutoGenerateColumns="AddNew">
                            <dxg:GridControl.View>
                                <dxg:TableView DetailHeaderContent="Orders"/>
                            </dxg:GridControl.View>
                        </dxg:GridControl>
                    </dxg:DataControlDetailDescriptor.DataControl>
                </dxg:DataControlDetailDescriptor>

                <!-- Tab 2: chart -->
                <dxg:ContentDetailDescriptor ContentTemplate="{StaticResource ordersChartTemplate}"
                                             HeaderContent="Freights"/>

                <!-- Tab 3: custom form -->
                <dxg:ContentDetailDescriptor ContentTemplate="{StaticResource employeeFormTemplate}"
                                             HeaderContent="Profile"/>

            </dxg:TabViewDetailDescriptor.DetailDescriptors>
        </dxg:TabViewDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

`TabViewDetailDescriptor` inherits from `MultiDetailDescriptor`. The `DetailDescriptors` collection accepts any combination of `DataControlDetailDescriptor`, `ContentDetailDescriptor`, or even nested `TabViewDetailDescriptor`.

### Keyboard Navigation Between Tabs

When detail content is wrapped in `DataControlDetailDescriptor`, users can use <kbd>Ctrl</kbd>+<kbd>1</kbd> through <kbd>Ctrl</kbd>+<kbd>9</kbd> to switch between the first nine tabs.

Source: `articles/controls-and-libraries/data-grid/master-detail/tabbed-view-for-details.md`.

## Pattern 4: Data-Dependent Details (DetailDescriptorSelector)

Different master rows can show different kinds of details. For example: a Vice President shows a list of subordinates, a Sales Representative shows a list of customers:

```xaml
<dxg:GridControl ItemsSource="{Binding Employees}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:DetailDescriptorSelector>

            <!-- Default: show subordinates -->
            <dxg:DetailDescriptorSelector.DefaultValue>
                <dxg:DataControlDetailDescriptor ItemsSourcePath="Employees">
                    <dxg:DataControlDetailDescriptor.DataControl>
                        <dxg:GridControl AutoGenerateColumns="AddNew"/>
                    </dxg:DataControlDetailDescriptor.DataControl>
                </dxg:DataControlDetailDescriptor>
            </dxg:DetailDescriptorSelector.DefaultValue>

            <!-- For Sales Reps (no subordinates), show customers -->
            <dxg:DetailDescriptorTrigger Binding="{Binding Path=Employees.Count}" Value="0">
                <dxg:DataControlDetailDescriptor ItemsSourcePath="Customers">
                    <dxg:DataControlDetailDescriptor.DataControl>
                        <dxg:GridControl AutoGenerateColumns="AddNew"/>
                    </dxg:DataControlDetailDescriptor.DataControl>
                </dxg:DataControlDetailDescriptor>
            </dxg:DetailDescriptorTrigger>

        </dxg:DetailDescriptorSelector>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

`DetailDescriptorTrigger.Binding` evaluates against the **master row's bound object**. If multiple triggers match, the first wins. If none match, `DefaultValue` is used.

The trigger's child can be any descriptor type (`DataControlDetailDescriptor`, `ContentDetailDescriptor`, `TabViewDetailDescriptor`, or even another `DetailDescriptorSelector`).

Source: `articles/controls-and-libraries/data-grid/master-detail/data-dependent-details.md`.

## Pattern 5: Row Details Template (Focused-Row Only)

Alternative to descriptors: show extra content **only for the focused row**, inline below it. No expand buttons — the content appears when a row is selected, disappears when another row is selected.

```xaml
<dxg:TableView>
    <dxg:TableView.RowDetailsTemplate>
        <DataTemplate>
            <Border Padding="8" Background="#FFF4F8FC">
                <StackPanel>
                    <TextBlock Text="{Binding Notes}" TextWrapping="Wrap"/>
                    <TextBlock Text="{Binding LastUpdated, StringFormat='Updated: {0:d}'}"/>
                </StackPanel>
            </Border>
        </DataTemplate>
    </dxg:TableView.RowDetailsTemplate>
</dxg:TableView>
```

Use `RowDetailsTemplate` when:
- Detail belongs to one row at a time (no need to compare details across rows)
- Detail is small and visually subordinate to the row
- You want a less mouse-heavy UI than expand-buttons

Source: `articles/controls-and-libraries/data-grid/master-detail/custom-content-in-details.md` § "Display Custom Content in Row Details".

## Hide Detail Buttons Conditionally

### Hide All Buttons

```xaml
<dxg:TableView ShowDetailButtons="False"/>
```

### Per-Row Visibility (Binding)

Bind `IsDetailButtonVisibleBinding` to a property on the master row:

```xaml
<dxg:TableView IsDetailButtonVisibleBinding="{Binding RowData.Row.HasChildren}"/>
```

The binding evaluates per master row. Useful when rows without children should not show a useless expand affordance.

### Per-Descriptor in DetailDescriptorSelector

Use `DetailDescriptorSelector` with no matching trigger and an empty `DefaultValue` to suppress details for specific rows entirely.

### Detail Button Width

```xaml
<dxg:TableView ExpandDetailButtonWidth="24"/>
```

Adjusts the width of the column showing the expand button. Set to `0` to hide visually while keeping rows expandable programmatically.

## Programmatic Expand / Collapse

```csharp
// Expand the focused row
grid.ExpandMasterRow(grid.View.FocusedRowHandle, descriptor);

// Toggle expand state in one call
grid.SetMasterRowExpanded(rowHandle, expanded: true, descriptor);

// Query state
if (grid.IsMasterRowExpanded(rowHandle, descriptor)) { /* ... */ }

// Collapse
grid.CollapseMasterRow(rowHandle, descriptor);
```

The `descriptor` parameter identifies which detail to expand when a master has multiple detail descriptors (e.g., several tabs). Pass `null` if there's only one descriptor.

End users can press <kbd>Ctrl</kbd>+<kbd>+</kbd> / <kbd>Ctrl</kbd>+<kbd>-</kbd> to expand or collapse the focused master row.

### Events

| Event | Fires |
|---|---|
| `GridControl.MasterRowExpanding` | Before expansion — set `e.Cancel = true` to block |
| `GridControl.MasterRowExpanded` | After expansion |
| `GridControl.MasterRowCollapsing` | Before collapse |
| `GridControl.MasterRowCollapsed` | After collapse |

## Work With Detail Grids in Code

### Get the Detail Grid for a Master Row

```csharp
// For DataControlDetailDescriptor with one detail
var detailControl = (GridControl)grid.GetDetail(masterRowHandle, descriptor);

// Visible detail at a row handle
var visibleDetail = grid.GetVisibleDetail(masterRowHandle);
var visibleDescriptor = grid.GetVisibleDetailDescriptor(masterRowHandle);
```

Returns `null` if no `DataControlDetailDescriptor` is used at this level or if the detail is currently collapsed.

### Detail Event Handlers — Use `e.Source`

The master `GridControl` creates a **copy** of the detail `GridControl` for each expanded master row. In event handlers, use `e.Source` (not the original control reference) to obtain the active detail:

```xaml
<dxg:DataControlDetailDescriptor ItemsSourcePath="Items">
    <dxg:GridControl AutoGenerateColumns="AddNew">
        <dxg:GridControl.View>
            <dxg:TableView CellValueChanging="CellValueChanging"/>
        </dxg:GridControl.View>
    </dxg:GridControl>
</dxg:DataControlDetailDescriptor>
```

```csharp
void CellValueChanging(object sender, CellValueChangedEventArgs e) {
    e.Source.PostEditor();   // use e.Source, the active detail
}
```

> Exception: `GridViewBase.AddingNewRow` uses standard `AddingNewEventArgs` which has no `Source`. Use the `sender` parameter instead.

### Events Detail Grids Do NOT Raise

Per the architecture, detail grids do not raise:
- Keyboard / mouse events
- Clipboard events
- Drag-and-drop events

To handle these, attach to the **master grid's** events and use `DataViewBase.FocusedView` to obtain the active detail view:

```csharp
private void TableView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
    if (e.ChangedButton != MouseButton.Right) return;
    if (e.Source is not TableView masterView) return;

    var activeView = (TableView)masterView.FocusedView;
    var activeGrid = activeView.Grid;
    if (activeGrid.GetMasterGrid() == null) return;   // We're in the master, not a detail

    activeGrid.SetCellValue(activeView.FocusedRowHandle,
                            activeGrid.Columns[nameof(DataItem.Ready)],
                            true);
}
```

Source: `articles/controls-and-libraries/data-grid/master-detail/data-grid-in-details.md` § "Handle Detail Grid Events".

## Focus and Selection Across Master and Detail

### Bind Focused Items Per Level

```xaml
<dxg:GridControl ItemsSource="{Binding Data}"
                 CurrentItem="{Binding Level1CurrentItem, Mode=TwoWay}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding Items}">
            <dxg:GridControl CurrentItem="{Binding Level2CurrentItem, Mode=TwoWay}"/>
        </dxg:DataControlDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

`CurrentItem` is OneWay (read-only from grid → ViewModel) by default; specify `Mode=TwoWay` if the ViewModel will also write.

### TwoWay Selected-Item Binding with `ParentPath`

If the detail data items contain a reference back to the master object, you can do TwoWay binding from the ViewModel to the focused detail row. Set `ParentPath` on the descriptor:

```xaml
<dxg:GridControl ItemsSource="{Binding Items}"
                 CurrentItem="{Binding CurrentMasterItem}">
    <dxg:GridControl.DetailDescriptor>
        <dxg:DataControlDetailDescriptor ItemsSourcePath="DetailItems"
                                         ParentPath="MasterItem">
            <dxg:DataControlDetailDescriptor.DataControl>
                <dxg:GridControl AutoGenerateColumns="AddNew"
                                 CurrentItem="{Binding CurrentDetailItem}"/>
            </dxg:DataControlDetailDescriptor.DataControl>
        </dxg:DataControlDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

The detail items' `MasterItem` property points back to the master record. With this set, the grid can find the master associated with a detail item — enabling the ViewModel to set `CurrentDetailItem` and have the grid expand the right master row.

### Multi-Selection in Master-Detail

`SelectedItems` on the master grid returns **only master rows**. To get selected detail rows, query each detail control individually:

```csharp
var details = (GridControl)masterGrid.GetDetail(masterRowHandle, descriptor);
var selectedDetailHandles = details.GetSelectedRowHandles();
foreach (var handle in selectedDetailHandles) {
    var row = details.GetRow(handle);
    // ...
}
```

Note: detail selection is **independent per master row**. Each expanded detail tracks its own selection.

## Search Across Master AND Detail

By default, the master grid's search panel searches master data only. Enable searching in details:

```xaml
<dxg:GridControl ItemsSource="{Binding Customers}">
    <dxg:GridControl.View>
        <dxg:TableView SearchString="an"
                       SearchPanelAllowFilter="False"
                       SearchPanelHighlightResults="True"/>
    </dxg:GridControl.View>

    <dxg:GridControl.DetailDescriptor>
        <dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding Orders}">
            <dxg:DataControlDetailDescriptor.DataControl>
                <dxg:GridControl AutoGenerateColumns="AddNew">
                    <dxg:GridControl.View>
                        <dxg:TableView SearchPanelAllowFilter="False"
                                       SearchPanelHighlightResults="True"/>
                    </dxg:GridControl.View>
                </dxg:GridControl>
            </dxg:DataControlDetailDescriptor.DataControl>
        </dxg:DataControlDetailDescriptor>
    </dxg:GridControl.DetailDescriptor>
</dxg:GridControl>
```

Configure both:
- `SearchPanelHighlightResults="True"` — visually highlight matches
- `SearchPanelAllowFilter="False"` — keep non-matching rows visible (just highlight)

`SearchPanelFindFilter` lets you specify a different comparison operator for the detail.

### Search Limitations

- The grid searches **only expanded** detail views. Master rows are NOT auto-expanded for search.
- Result Info Panel, Navigation Buttons, and Scrollbar Annotations are **not supported** for detail search results.

Source: `articles/controls-and-libraries/data-grid/master-detail/data-grid-in-details.md` § "Enable Search Operations in Detail Grid".

## Show Detail Header

Display a header above the detail grid:

```xaml
<dxg:DataControlDetailDescriptor ItemsSourcePath="Orders" ShowHeader="True">
    <dxg:DataControlDetailDescriptor.DataControl>
        <dxg:GridControl AutoGenerateColumns="AddNew">
            <dxg:GridControl.View>
                <dxg:TableView DetailHeaderContent="Orders"/>
            </dxg:GridControl.View>
        </dxg:GridControl>
    </dxg:DataControlDetailDescriptor.DataControl>
</dxg:DataControlDetailDescriptor>
```

`DetailHeaderContent` is the caption shown above the detail. It also appears in the master grid's group panel, filter panel, and detail tab headers (when used in `TabViewDetailDescriptor`).

For richer header content, use `DetailDescriptorBase.ContentTemplate` — the template's `DataContext` is the master row object:

```xaml
<dxg:DataControlDetailDescriptor ItemsSourcePath="Orders">
    <dxg:DataControlDetailDescriptor.ContentTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Notes}" TextWrapping="Wrap" Padding="4"/>
        </DataTemplate>
    </dxg:DataControlDetailDescriptor.ContentTemplate>
    <dxg:DataControlDetailDescriptor.DataControl>
        <dxg:GridControl AutoGenerateColumns="AddNew"/>
    </dxg:DataControlDetailDescriptor.DataControl>
</dxg:DataControlDetailDescriptor>
```

## Detail Appearance Properties

| Property | Effect |
|---|---|
| `DetailDescriptorBase.Margin` | Margin around the detail |
| `TableView.ExpandDetailButtonWidth` | Width of the column holding the expand button |
| `TableView.IsDetailButtonVisibleBinding` | Per-row binding for the expand button |
| `TableView.ShowDetailButtons` | Master switch for all expand buttons |
| `DetailDescriptorBase.ShowHeader` | Show / hide the detail header |
| `DetailDescriptorBase.ContentTemplate` | Custom content above the detail grid |
| `DataViewBase.DetailHeaderContent` | The caption text (also used in master UI elements) |

## Detail Rendering Specifics

- Detail descriptors are **factories** — the master grid creates a fresh copy of the detail control for each expanded master row.
- Standard layout customization (column reorder, resize) on one detail grid **affects all detail grids** at the same level simultaneously. They share a single layout factory.
- Master and detail grids share **vertical and horizontal scrollbars** — details are built into the master visual tree.
- You cannot set per-row detail width — it derives from the master grid's width.

## Master-Detail vs TreeListView

| Aspect | Master-Detail (`DataControlDetailDescriptor`) | `TreeListView` (a view of `GridControl`) or `TreeListControl` |
|---|---|---|
| Data shape | Different types per level (Customer → Order → OrderItem) | Same type per level (with `ChildNodesPath`) or self-referential (`KeyFieldName`/`ParentFieldName`) |
| Columns | Different columns per level | Same columns at all levels |
| Sort / filter / group | Independent per level | Across all levels |
| Number of levels | Fixed by XAML | Unlimited (data-driven) |
| End-user grouping | Yes | No |
| Different details for different rows | `DetailDescriptorSelector` | Not directly |

Source: `articles/controls-and-libraries/data-grid/master-detail/data-grid-in-details.md` § "Master-Detail vs TreeListView".

## Limitations

### Unsupported Features

- **Stacked details** — multiple details at the same level NOT supported. Use `TabViewDetailDescriptor` for tabs, or `ContentDetailDescriptor` with a UserControl containing multiple grids for visual stacking.
- **`TableView.KeepViewportOnDataUpdate`** has no effect in master-detail.
- **Exporting details defined in templates** — `DetailDescriptorBase.ContentTemplate` content is NOT exported.

### Design Limitations

- **`ICollectionView`** — cannot be used as `ItemsSource` for both master and detail.
- **`TreeListView` / `CardView`** — cannot be used as the master view OR as a detail view. They lack the integration. (You can put them inside `ContentDetailDescriptor` as custom content, but no native master-detail integration.)
- **Server Mode / Instant Feedback** — cannot be used in master OR detail grids.
- **`TreeListControl` / `CardView` in details** — only as custom content (no native integration).
- **Fixed Rows** — cannot pin rows when master-detail is enabled.
- **Hit-test methods on detail grid** — call the master grid's hit-test methods instead.
- **UI Automation** — not supported for detail grids.
- **Auto Filter Row** — works only in the root master grid, not in details.
- **Search** — only searches expanded details; does not auto-expand master rows for matches. No Result Info Panel / Navigation Buttons / Scrollbar Annotations.
- **Fixed total summary row** — bottom summary rows are only visible when scrolled to the bottom of the detail view. Set the detail view's `TotalSummaryPosition="Top"` to keep visible.

Source: `articles/controls-and-libraries/data-grid/master-detail/master-detail-mode-limitations.md`.

## TreeListControl

`TreeListControl` does **NOT** support master-detail (which is a different relationship between master rows and child collections). For hierarchical data in a single tree, use `TreeListControl` directly. For Customer → Orders style master-detail, use `GridControl` with `DataControlDetailDescriptor`.

## Common Issues

- **Detail is empty** — check `ItemsSourcePath` matches the property name on the master row exactly, OR switch to `ItemsSourceBinding="{Binding YourCollection}"`. The property must be a collection, not a single object.
- **Search highlights nothing in details** — ensure `SearchPanelHighlightResults="True"` is set on **each** detail view, not just the master.
- **Selecting in a detail doesn't update ViewModel** — the binding source for the detail descriptor is per-row. Use `ParentPath` to enable TwoWay binding, or fall back to per-detail-control queries.
- **`grid.GetDetail` returns null** — the master row is collapsed. Expand it first via `ExpandMasterRow`, then query.
- **Detail column reorder affects all instances** — by design. There's no API to make per-master detail column layouts; if you need that, use `ContentDetailDescriptor` with a custom UserControl.

## Source Material

- `articles/controls-and-libraries/data-grid/master-detail-data-representation.md` (root)
- `articles/controls-and-libraries/data-grid/master-detail/data-grid-in-details.md`
- `articles/controls-and-libraries/data-grid/master-detail/custom-content-in-details.md`
- `articles/controls-and-libraries/data-grid/master-detail/tabbed-view-for-details.md`
- `articles/controls-and-libraries/data-grid/master-detail/data-dependent-details.md`
- `articles/controls-and-libraries/data-grid/master-detail/master-detail-mode-limitations.md`
- `articles/controls-and-libraries/data-grid/master-detail/master-detail-member-table.md`
