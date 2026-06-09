# Card View — DevExpress WPF Data Grid

`CardView` is one of the three views of `GridControl` (alongside `TableView` and `TreeListView`). Each data record renders as a **card** — a panel with fields stacked vertically. Cards arrange in **columns** (default — scroll vertically) or **rows** (scroll horizontally). Best for records with many fields per item where a wide tabular layout reads poorly (contact lists, product catalogs, ticket details, dashboards).

## When to Use This Reference

Use this when you need to:

- Switch a `GridControl` from `TableView` to `CardView`
- Arrange cards in columns vs rows (`CardLayout`)
- Control card alignment, max cards per row, card size
- Bind a card's header to a data property (`CardHeaderBinding`)
- Customize the card body via `CardTemplate` (full card layout) or per-field `CellTemplate` (per-field display)
- Customize the card header via `CardHeaderTemplate`
- Allow users to resize cards (`AllowCardResizing`)
- Expand / collapse cards programmatically
- Configure Card View–specific print / export options
- Understand limitations (no Data-Aware export, no master-detail)

## When to Pick Card View vs Table View

| Scenario | Pick |
|---|---|
| Many records, few fields per record, comparison across records important | **Table View** |
| Few records (or focus-one-at-a-time), many fields per record, vertical stacking reads better | **Card View** |
| Each record has a primary "identity" field that should appear as a heading | **Card View** (use `CardHeaderBinding`) |
| Hierarchical data | **TreeListView** (or `TreeListControl`) |

Contact lists, product cards, ticket detail panels, dashboards with KPI cards — all Card View. Tabular reports, order books, inventory lists — Table View.

## Setup

```xaml
<dxg:GridControl ItemsSource="{Binding Customers}">
    <dxg:GridControl.View>
        <dxg:CardView/>
    </dxg:GridControl.View>
    <dxg:GridColumn FieldName="CompanyName"/>
    <dxg:GridColumn FieldName="ContactName"/>
    <dxg:GridColumn FieldName="Country"/>
    <dxg:GridColumn FieldName="Phone"/>
</dxg:GridControl>
```

Columns become **card fields** — they appear vertically stacked inside each card. The same `GridColumn` definitions apply (`FieldName`, `EditSettings`, `CellTemplate`, sorting / filtering / grouping settings).

Source: `articles/controls-and-libraries/data-grid/views/card-view.md`.

## Card Layout — Columns vs Rows

```xaml
<dxg:CardView CardLayout="Columns"/>   <!-- Default: cards fill columns top-to-bottom, scroll vertical -->
<dxg:CardView CardLayout="Rows"/>      <!-- Cards fill rows left-to-right, scroll horizontal -->
```

| Layout | First card position | Subsequent cards | Scroll direction |
|---|---|---|---|
| `Columns` (default) | Top-left | Under previous; new column when current is full | Vertical |
| `Rows` | Top-left | Right of previous; new row when current is full | Horizontal |

### Maximum Cards Per Row / Column

```xaml
<dxg:CardView CardLayout="Rows" MaxCardCountInRow="4"/>
```

In `Rows` layout, no more than 4 cards per row. In `Columns` layout, the same property caps cards per column.

Source: `articles/controls-and-libraries/data-grid/views/card-view/cards-layout.md`.

## Card Alignment

`CardView.CardAlignment` controls where cards align within the view when there are fewer cards than fit:

| Layout | Value | Effect |
|---|---|---|
| Rows | `Near` | Cards align to the left of each row |
| Rows | `Center` | Cards center within each row |
| Rows | `Far` | Cards align to the right of each row |
| Columns | `Near` | Cards align to the top of each column |
| Columns | `Center` | Cards center vertically within each column |
| Columns | `Far` | Cards align to the bottom of each column |

```xaml
<dxg:CardView CardLayout="Rows" CardAlignment="Center"/>
```

## Card Size

By default, each card auto-sizes to its content — different cards can have different widths (in Columns layout) or heights (in Rows layout).

For uniform-size cards:

```xaml
<dxg:CardView FixedSize="240"
              MinFixedSize="180"
              AllowCardResizing="True"/>
```

| Property | Effect |
|---|---|
| `CardView.FixedSize` | Fixed width (Columns layout) or height (Rows layout). All cards same size. |
| `CardView.MinFixedSize` | Minimum size when user resizes via Card Separator |
| `CardView.AllowCardResizing` | Allow user to drag card separators to resize. Requires `FixedSize` to be set. |
| `CardView.CardMargin` | Outer margin around each card (Thickness) |

> Without `FixedSize`, `AllowCardResizing` has no effect — cards auto-size to content and the separator can't resize them.

Source: `articles/controls-and-libraries/data-grid/views/card-view/card-settings.md`.

## Card Header

Each card has a header at the top. By default, the header shows the field name and value of the first column. Bind it to any data property:

```xaml
<dxg:CardView CardHeaderBinding="{Binding CompanyName}"/>
```

The card header now displays the `CompanyName` value (e.g., "Around the Horn"), not the default key/value pair.

### Custom Card Header Template

```xaml
<dxg:CardView CardHeaderBinding="{Binding CompanyName}">
    <dxg:CardView.CardHeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Margin="4">
                <Image Source="/Images/customer.png" Width="16" Height="16"/>
                <TextBlock Text="{Binding}" FontWeight="Bold" Margin="4,0,0,0"/>
            </StackPanel>
        </DataTemplate>
    </dxg:CardView.CardHeaderTemplate>
</dxg:CardView>
```

The template's `DataContext` is whatever `CardHeaderBinding` resolves to (in this case, a string — the `CompanyName` value). Bind to `{Binding}` for the value itself, or change the `CardHeaderBinding` to the full row object and bind to multiple properties:

```xaml
<dxg:CardView CardHeaderBinding="{Binding}">
    <dxg:CardView.CardHeaderTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{Binding CompanyName}" FontWeight="Bold"/>
                <TextBlock Text="{Binding Country}" FontStyle="Italic" Opacity="0.7"/>
            </StackPanel>
        </DataTemplate>
    </dxg:CardView.CardHeaderTemplate>
</dxg:CardView>
```

## Custom Card Template

Replace the entire card body with your own layout:

```xaml
<dxg:CardView CardTemplate="{StaticResource CustomCardTemplate}"/>

<DataTemplate x:Key="CustomCardTemplate">
    <Border BorderBrush="LightGray" BorderThickness="1" Padding="8" Background="White">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <TextBlock Grid.Row="0" Text="{Binding RowData.Row.CompanyName}" FontSize="14" FontWeight="Bold"/>
            <TextBlock Grid.Row="1" Text="{Binding RowData.Row.ContactName}"/>
            <TextBlock Grid.Row="2" Text="{Binding RowData.Row.Notes}" TextWrapping="Wrap"/>
        </Grid>
    </Border>
</DataTemplate>
```

The template's `DataContext` is a `CardData` object — access the underlying row via `RowData.Row.{Field}`.

> `CardTemplate` replaces the default card visual. Card fields (the vertical stack) are NOT auto-rendered — you must lay them out yourself. If you want to keep the default field stack and only add header / footer chrome, use `CardStyle` instead (which works like a `Style` on the implicit card container).

## Card Style

For lighter customization (background, border, padding without replacing the layout):

```xaml
<dxg:CardView.CardStyle>
    <Style TargetType="ContentControl">
        <Setter Property="Background" Value="#FFFAFAFA"/>
        <Setter Property="BorderBrush" Value="#FFE0E0E0"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="Margin" Value="2"/>
    </Style>
</dxg:CardView.CardStyle>
```

Per the styles article, `CardStyle` targets `ContentControl` (the wrapper around each card) with `DataContext` of `CardData`.

## Expand / Collapse Cards

Each card has an expand/collapse button in its header. The button toggles visibility of the card body, leaving just the header visible when collapsed.

### Programmatic

```csharp
view.ExpandCard(rowHandle);
view.CollapseCard(rowHandle);
view.ExpandAllCards();
view.CollapseAllCards();
```

### Keyboard

Users can press <kbd>Ctrl</kbd>+<kbd>+</kbd> / <kbd>Ctrl</kbd>+<kbd>-</kbd> to expand or collapse the focused card.

Source: `articles/controls-and-libraries/data-grid/views/card-view/expanding-and-collapsing-cards.md`.

## Features That Work the Same as Table View

Card View shares most of the `DataViewBase` / `GridViewBase` infrastructure with Table View:

- **Sorting** — clicking a card field's label sorts cards by that field
- **Filtering** — same UI flavors (drop-down filter, auto filter row, filter editor, code-driven)
- **Grouping** — groups appear as group bands; cards within a group share a parent header
- **Total / Group Summaries** — `TotalSummary`, `GroupSummary` collections work identically
- **Conditional Formatting** — works on field values
- **Editing** — in-place editors, `EditSettings`, validation
- **Selection** — `SelectedItem(s)`, multi-card select
- **Focus** — `CurrentItem`, `FocusedRowHandle`, navigation methods
- **Drag-and-Drop** — `AllowDragDrop` on the view
- **Search Panel** — `ShowSearchPanelMode`, `SearchString`

See [columns.md](columns.md), [sorting-filtering-grouping.md](sorting-filtering-grouping.md), [validation.md](validation.md), [focus-and-selection.md](focus-and-selection.md), [drag-and-drop.md](drag-and-drop.md) — same APIs apply.

## Per-Column / Per-Field Display

Within a card, each column from `GridControl.Columns` becomes a vertical field row. Customize per field:

```xaml
<dxg:GridColumn FieldName="Phone">
    <dxg:GridColumn.EditSettings>
        <dxe:TextEditSettings Mask="(000) 000-0000"
                              MaskUseAsDisplayFormat="True"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>

<dxg:GridColumn FieldName="IsVIP">
    <dxg:GridColumn.CellTemplate>
        <DataTemplate>
            <CheckBox IsChecked="{Binding RowData.Row.IsVIP, Mode=TwoWay}"/>
        </DataTemplate>
    </dxg:GridColumn.CellTemplate>
</dxg:GridColumn>
```

To hide a column from cards (but still keep it in code):

```xaml
<dxg:GridColumn FieldName="InternalId" Visible="False"/>
```

To exclude a column from a custom `CardTemplate`: just don't reference it.

## Print and Export Options Specific to Card View

```xaml
<dxg:CardView PrintAutoCardWidth="True"
              PrintCardMargin="8"
              PrintMaximumCardColumns="3"/>
```

| Property | Effect |
|---|---|
| `CardView.PrintAutoCardWidth` | Cards in printed output auto-size to fit page width |
| `CardView.PrintCardMargin` | Card frame thickness in print |
| `CardView.PrintMaximumCardColumns` | Max card columns per print page |

See [printing-exporting.md](printing-exporting.md) for the rest.

## Limitations

- **Data-Aware Export (XLSX/XLS/CSV with grouping / summaries / formulas) is NOT supported.** `CardView.ExportToXlsx` / `ExportToXls` / `ExportToCsv` exist but only work in WYSIWYG mode. To force WYSIWYG (you must):
  ```csharp
  view.ExportToXlsx(@"C:\export.xlsx",
      new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
  ```
- **Master-Detail is NOT supported in Card View.** Use TableView for master views with detail descriptors.
- **TreeListView** features (hierarchical data with `KeyFieldName`/`ParentFieldName`) are NOT available in Card View.
- **Bands** are typically TableView-oriented. CardView can use bands but the visual result differs — verify in your version.

Source: `articles/controls-and-libraries/data-grid/printing-and-exporting/data-aware-export.md` (limitation note) and `master-detail/master-detail-mode-limitations.md`.

## Card View Visual Elements (for Styling / Templating)

| Element | Purpose | Style property |
|---|---|---|
| Card | Container for one record | `CardView.CardStyle`, `CardTemplate` |
| Card Header | Top bar with collapse button + caption | `CardView.CardHeaderTemplate`, `CardHeaderBinding` |
| Card Field | One field row (label + value) | `ColumnBase.CellStyle`, `CellTemplate` |
| Group Header | Header for a group of cards | `GridViewBase.GroupRowStyle` |
| Card Separator | Drag handle between cards | `CardView.AllowCardResizing` |

See `articles/controls-and-libraries/data-grid/visual-elements/card-view-elements.md` for the full list.

## Focus Appearance Specific to Card View

| Property | Effect |
|---|---|
| `CardView.FocusedCardBorderTemplate` | Border around the focused card |
| `CardView.FocusedCellBorderCardViewTemplate` | Border around the focused field within the card |
| `CardView.VerticalFocusedGroupRowBorderTemplate` | Group row border in vertical card arrangement |

## Common Issues

- **Cards all show "FieldName: Value" in header** — set `CardHeaderBinding` to bind to a specific field.
- **`AllowCardResizing="True"` does nothing** — must also set `FixedSize` to enable resizing.
- **CardView export to XLSX is empty / wrong** — default Data-Aware mode is unsupported. Force WYSIWYG via `ExportType` option.
- **Cards too narrow** — set `FixedSize` to a larger value, or remove `MaxCardCountInRow` to allow more cards per row.
- **CardTemplate doesn't show field labels** — `CardTemplate` replaces the entire card body. Either add labels manually or use `CardStyle` instead to keep default field rendering.

## Apply to TreeListControl

Card View is a view of `GridControl`. The standalone `TreeListControl` does NOT support a card layout — it uses `TreeListView` for hierarchical display. To get card-like display of hierarchical data, use `GridControl` with `CardView` and group the data, but this loses the tree expand/collapse semantics.

## Source Material

- `articles/controls-and-libraries/data-grid/views/card-view.md` (root)
- `articles/controls-and-libraries/data-grid/views/card-view/cards-layout.md`
- `articles/controls-and-libraries/data-grid/views/card-view/card-settings.md`
- `articles/controls-and-libraries/data-grid/views/card-view/expanding-and-collapsing-cards.md`
- `articles/controls-and-libraries/data-grid/visual-elements/card-view-elements.md`
- `articles/controls-and-libraries/data-grid/end-user-capabilities/expanding-and-collapsing-cards.md`
- `articles/controls-and-libraries/data-grid/end-user-capabilities/resizing-columns-and-cards.md`
- `articles/controls-and-libraries/data-grid/paging-and-scrolling/card-view-scrolling-in-code.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/rows-and-cards.md`
