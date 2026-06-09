# Fields & Areas — Blazor Pivot Table

When you need to: configure DxPivotTableField properties; assign fields to Row, Column, Data, or Filter areas; group dates by year, quarter, or month; choose aggregation types; control field ordering.

## Area Overview

| `PivotTableArea` | Purpose |
|---|---|
| `Row` | Fields whose values create the row headers (left side) |
| `Column` | Fields whose values create the column headers (top) |
| `Data` | Numeric value fields — one cell in the cross-tabulation per combination |
| `Filter` | Fields that filter the entire pivot view |

## DxPivotTableField Properties

| Property | Type | Description |
|---|---|---|
| `Field` | `string` | Data source field name. Use `nameof()` |
| `Area` | `PivotTableArea` | Row, Column, Data, or Filter |
| `AreaIndex` | `int` | Display order within the area (0-based) |
| `Caption` | `string` | Label shown in the pivot header and field list |
| `SummaryType` | `PivotTableSummaryType` | Aggregation function (Data area only) |
| `GroupInterval` | `PivotTableGroupInterval` | Date grouping (any area with date fields) |
| `SortOrder` | `PivotTableSortOrder` | Ascending or Descending |
| `Visible` | `bool` | Whether the field appears in the pivot layout |

## PivotTableArea Values

```razor
Area="PivotTableArea.Row"     // Row headers
Area="PivotTableArea.Column"  // Column headers
Area="PivotTableArea.Data"    // Aggregated values (cells)
Area="PivotTableArea.Filter"  // Top-level filter
```

## PivotTableSummaryType Values

| Value | Description |
|---|---|
| `Sum` | Sum of values (default) |
| `Count` | Number of records |
| `Average` | Average of values |
| `Min` | Minimum value |
| `Max` | Maximum value |

```razor
<DxPivotTableField Field="@nameof(Sale.Revenue)"
                   Area="PivotTableArea.Data"
                   SummaryType="PivotTableSummaryType.Sum"
                   Caption="Total Revenue" />
```

## PivotTableGroupInterval Values for Dates

| Value | Description |
|---|---|
| `DateYear` | Groups by year (2022, 2023, 2024) |
| `DateQuarter` | Groups by quarter (Q1, Q2, Q3, Q4) |
| `DateMonth` | Groups by month (January, February, ...) |
| `DateDay` | Groups by day of month |
| `DateHour` | Groups by hour |
| `DateDayOfWeek` | Groups by day of week |

### Multi-level Date Hierarchy

Use the same field multiple times with different `GroupInterval` values and increasing `AreaIndex`:

```razor
<DxPivotTableField Field="@nameof(Sale.OrderDate)"
                   Area="PivotTableArea.Column"
                   AreaIndex="0"
                   GroupInterval="PivotTableGroupInterval.DateYear"
                   Caption="Year" />
<DxPivotTableField Field="@nameof(Sale.OrderDate)"
                   Area="PivotTableArea.Column"
                   AreaIndex="1"
                   GroupInterval="PivotTableGroupInterval.DateQuarter"
                   Caption="Quarter" />
<DxPivotTableField Field="@nameof(Sale.OrderDate)"
                   Area="PivotTableArea.Column"
                   AreaIndex="2"
                   GroupInterval="PivotTableGroupInterval.DateMonth"
                   Caption="Month" />
```

## AreaIndex — Field Ordering

`AreaIndex` controls the display order when multiple fields share the same area:

```razor
<!-- Row area: Country then City (nested) -->
<DxPivotTableField Field="@nameof(Sale.Country)" Area="PivotTableArea.Row" AreaIndex="0" />
<DxPivotTableField Field="@nameof(Sale.City)"    Area="PivotTableArea.Row" AreaIndex="1" />
```

Without `AreaIndex`, fields appear in the order they are declared.

## Multiple Data Fields

You can have multiple fields in the Data area, each with a different aggregation:

```razor
<DxPivotTableField Field="@nameof(Order.Revenue)" Area="PivotTableArea.Data"
                   SummaryType="PivotTableSummaryType.Sum"
                   Caption="Total Revenue" />
<DxPivotTableField Field="@nameof(Order.Quantity)" Area="PivotTableArea.Data"
                   SummaryType="PivotTableSummaryType.Sum"
                   Caption="Units Sold" />
<DxPivotTableField Field="@nameof(Order.Revenue)" Area="PivotTableArea.Data"
                   SummaryType="PivotTableSummaryType.Average"
                   Caption="Avg Revenue" />
```

## Interactive Field List

Allow end-users to drag fields between areas:

```razor
<DxPivotTable Data="@Sales" ShowFieldList="true">
    <Fields>
        ...
    </Fields>
</DxPivotTable>
```

## PivotTableSortOrder

```razor
<DxPivotTableField Field="@nameof(Sale.Country)"
                   Area="PivotTableArea.Row"
                   SortOrder="PivotTableSortOrder.Descending" />
```

## Full Example — Multi-Dimensional Analysis

```razor
<DxPivotTable Data="@Sales">
    <Fields>
        <!-- Row: Country → Category -->
        <DxPivotTableField Field="@nameof(Sale.Country)"
                           Area="PivotTableArea.Row" AreaIndex="0" Caption="Country" />
        <DxPivotTableField Field="@nameof(Sale.Category)"
                           Area="PivotTableArea.Row" AreaIndex="1" Caption="Category" />

        <!-- Column: Year → Quarter -->
        <DxPivotTableField Field="@nameof(Sale.OrderDate)"
                           Area="PivotTableArea.Column" AreaIndex="0"
                           GroupInterval="PivotTableGroupInterval.DateYear" Caption="Year" />
        <DxPivotTableField Field="@nameof(Sale.OrderDate)"
                           Area="PivotTableArea.Column" AreaIndex="1"
                           GroupInterval="PivotTableGroupInterval.DateQuarter" Caption="Quarter" />

        <!-- Data: Sum of Amount, Count of orders -->
        <DxPivotTableField Field="@nameof(Sale.Amount)"
                           Area="PivotTableArea.Data"
                           SummaryType="PivotTableSummaryType.Sum" Caption="Total Sales ($)" />
        <DxPivotTableField Field="@nameof(Sale.Amount)"
                           Area="PivotTableArea.Data"
                           SummaryType="PivotTableSummaryType.Count" Caption="Order Count" />

        <!-- Filter: Region -->
        <DxPivotTableField Field="@nameof(Sale.Region)"
                           Area="PivotTableArea.Filter" Caption="Region" />
    </Fields>
</DxPivotTable>
```
