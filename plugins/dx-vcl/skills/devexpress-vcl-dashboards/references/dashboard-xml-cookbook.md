# Dashboard XML cookbook (author layouts without the Designer)

The layout is .NET Dashboard XML. Everything below was exported to PDF on v26.1+. For other items and options
(Gauge, Pivot, maps, TreeView, conditional formatting, calculated fields, expressions) search the MCP with
`technologies: ["Dashboard"]` and copy the XML shape from the "Dashboard XML" / designer-saved samples.

## Skeleton

```xml
<?xml version="1.0" encoding="utf-8"?>
<Dashboard>
  <Title Text="Orders" />
  <DataSources>
    <JsonDataSource Name="Orders" ConnectionName="OrdersConnection" RootElement="Orders" ComponentName="dsOrders" />
  </DataSources>
  <Parameters> ... </Parameters>          <!-- optional -->
  <Items> ... </Items>
  <LayoutTree>
    <LayoutGroup>
      <LayoutItem DashboardItem="gridOrders" />
    </LayoutGroup>
  </LayoutTree>
</Dashboard>
```

Rules:

- `ComponentName` is the identifier: data sources and items are referenced by it (`DataSource="dsOrders"`,
  `DashboardItem="gridOrders"`). Unique across the dashboard.
- `Name` is the caption shown to the user.
- Inside an item, `<DataItems>` declares `Dimension`s (group by) and `Measure`s (aggregate) with `DataMember` = the
  field name and a `DefaultId` unique within that item. The item's sections reference data items by `DefaultId`.
- An item missing from `<LayoutTree>` is not shown. `<LayoutGroup Orientation="Vertical">` stacks items.
- A JSON data source needs no `<Schema>`. Items on a SQL data source also need `DataMember="<query name>"`.
- Measure summary: `SummaryType="Sum"` (default), `Count`, `CountDistinct`, `Average`, `Min`, `Max`.
- Dimension on a date: `DateTimeGroupInterval="Year" | "Quarter" | "Month" | "MonthYear" | "DayMonthYear" | "None"`.
  `MonthYear` shows "February 2026"; `Month` merges the same month of different years.
- Dimension sort: `SortOrder="Descending"`; sort by a measure: `SortByMeasure="DataItem1"`.

## Grid

```xml
<Grid ComponentName="gridOrders" Name="Orders by Country" DataSource="dsOrders">
  <DataItems>
    <Dimension DataMember="Country" DefaultId="DataItem0" />
    <Measure DataMember="Amount" DefaultId="DataItem1" />
  </DataItems>
  <GridColumns>
    <GridDimensionColumn><Dimension DefaultId="DataItem0" /></GridDimensionColumn>
    <GridMeasureColumn><Measure DefaultId="DataItem1" /></GridMeasureColumn>
  </GridColumns>
  <GridOptions />
</Grid>
```

## Chart (bar by default; `SeriesType="Line" | "Spline" | "Area" | ...`)

```xml
<Chart ComponentName="chartSales" Name="Sales by Month" DataSource="sqlShop" DataMember="Sales">
  <DataItems>
    <Dimension DataMember="SaleDate" DateTimeGroupInterval="MonthYear" DefaultId="DataItem0" />
    <Measure DataMember="Amount" DefaultId="DataItem1" />
  </DataItems>
  <Arguments><Argument DefaultId="DataItem0" /></Arguments>
  <Panes>
    <Pane Name="Pane 1">
      <Series>
        <Simple SeriesType="Bar">
          <Value DefaultId="DataItem1" />
          <PointLabelOptions ShowPointLabels="true" ContentType="Value" />
        </Simple>
      </Series>
    </Pane>
  </Panes>
</Chart>
```
A second dimension in `<SeriesDimensions><SeriesDimension DefaultId="..."/></SeriesDimensions>` splits the series
(one per country). `PointLabelOptions ShowPointLabels="true"` puts the values in the export text.

## Pie

```xml
<Pie ComponentName="pieRegions" Name="Sales by Region" DataSource="dsSales" LabelContentType="ArgumentAndValue">
  <DataItems>
    <Dimension DataMember="Region" DefaultId="DataItem0" />
    <Measure DataMember="Amount" DefaultId="DataItem1" />
  </DataItems>
  <Arguments><Argument DefaultId="DataItem0" /></Arguments>
  <Values><Value DefaultId="DataItem1" /></Values>
</Pie>
```
`LabelContentType`: `Argument`, `Value`, `ArgumentAndValue`, `Percent`, `ArgumentValueAndPercent`. Exported text:
"Europe: 1.4K".

## Card

```xml
<Card ComponentName="cardTotal" Name="Total" DataSource="dsOrders">
  <DataItems>
    <Measure DataMember="Amount" DefaultId="DataItem0" />
    <Dimension DataMember="Country" DefaultId="DataItem1" />
  </DataItems>
  <SeriesDimensions><SeriesDimension DefaultId="DataItem1" /></SeriesDimensions>   <!-- one card per country; omit for one total -->
  <Card>
    <ActualValue DefaultId="DataItem0" />
    <AbsoluteVariationNumericFormat />
    <PercentVariationNumericFormat />
  </Card>
</Card>
```

## Number format

The default measure format abbreviates (`1.33K`, `0.9K`). For exact values:

```xml
<Measure DataMember="Amount" DefaultId="DataItem1">
  <NumericFormat FormatType="Number" Precision="2" Unit="Ones" IncludeGroupSeparator="true" />   <!-- 1,334.50 -->
</Measure>
```
`FormatType`: `Auto`, `General`, `Number`, `Currency`, `Scientific`, `Percent`. `Unit`: `Auto`, `Ones`, `Thousands`,
`Millions`, `Billions`. Set exact formats with `NumericFormat` rather than relying on a culture.

## Parameters

```xml
<Parameters>
  <Parameter Name="Country" Value="Germany" />                                 <!-- String by default -->
  <Parameter Name="MinAmount" Type="System.Int32" Value="0" />
  <Parameter Name="Countries" AllowMultiselect="true"><Values><Value>Germany</Value></Values></Parameter>
</Parameters>
```
Use a parameter in an item filter (`?Name`). Each item filters on its own: give **every** item that must follow the
parameter its own `FilterString`, with a data item for the filtered field (it can be a hidden dimension):
```xml
<Grid ComponentName="gridSales" Name="Sales" DataSource="dsSales" FilterString="[DataItem0] = ?Country">
```
- In an item `FilterString`, refer to **data items** by `DefaultId` (`[DataItem0]`), not to fields.
- An item `FilterString` filters **dimensions** only: `[DataItem1] >= ?MinAmount` on a measure shows no rows. To
  filter by a number, add the field as a dimension (it can be hidden: `<HiddenDimensions>`). In XML, write `>=` as
  `&gt;=`.
- Multi-value parameter: `AllowMultiselect="true"` + `FilterString="[DataItem0] In (?Countries)"`; from code
  `Parameters['Countries'].Value := VarArrayOf(['France', 'USA'])` (unit `System.Variants`).
- To filter every item at once, filter the SQL query (`<Filter>` with a query parameter, [data-binding.md](data-binding.md#sql--tdxbackenddatabasesqlconnection)). A `Filter` attribute on `<JsonDataSource>` is ignored.
- Set the value from code **after** the layout is assigned (assigning `Layout` clears parameters):
  `dxDashboardControl1.Parameters['Country'].Value := 'France';` (C++: `->Parameters->ParamByName["Country"]->Value`; `FindParam` returns nil instead of raising).
  The value goes into `State` (`{"Parameters":{"Country":"France"}}`), and the viewer and exports use it.
- A parameter name that is not in the layout raises `EdxBackendParameterException: Parameter "X" not found`.

## Master filtering (click in one item to filter the others)

```xml
<Grid ComponentName="gridCountries" Name="Countries" DataSource="dsSales">
  <InteractivityOptions MasterFilterMode="Multiple" />     <!-- or "Single" -->
  ...
</Grid>
```
- The master item filters every other item **on the same data source**. Other data sources need
  `IsMasterFilterCrossDataSource="true"` on the master item.
- `Single` always has one selected row (the first by default); `Multiple` starts with no selection.
- An item that must not react: `<InteractivityOptions IgnoreMasterFilters="true" />`.
- The user's selection is part of `State`: `{"Items":{"gridCountries":{"MasterFilterValues":[["France"]]}}}`
  (outer array: selected rows; inner: one value per dimension). Exports apply it.

## Filter elements (Combo Box, List Box)

```xml
<ComboBox ComponentName="comboCountry" Name="Country" DataSource="dsSales">
  <DataItems><Dimension DataMember="Country" DefaultId="DataItem0" /></DataItems>
  <FilterDimensions><Dimension DefaultId="DataItem0" /></FilterDimensions>
</ComboBox>
```
`<ListBox>` has the same shape. They filter like a master item (state key: their `ComponentName`); the
selection is stored in `State`.
