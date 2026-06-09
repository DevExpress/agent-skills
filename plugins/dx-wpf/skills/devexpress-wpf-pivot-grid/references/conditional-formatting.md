# Conditional Formatting

Conditional formatting changes the appearance of data cells based on their values — Excel-inspired data bars, color scales, icon sets, top/bottom rules, and arbitrary value-based or expression-based formats. Rules live in `PivotGridControl.FormatConditions` and reference fields by `Name`.

## When to Use This Reference

Use this when you need to:

- Add data bars / color scales / icon sets / top-bottom rules to data cells
- Apply formatting only at a specific row × column intersection vs. across all data cells
- Use a predefined format (`PredefinedFormatName`) or build a custom one
- Let users add and manage rules at runtime

## Required Setup — Set `Name` on Every Field

**Every field referenced by a format condition (data, row, column) must have its `Name` set**. Without `Name`, conditions can't bind to fields.

```xaml
<dxpg:PivotGridField Name="fieldSales"     Area="DataArea"    FieldName="ExtendedPrice"
                     CellFormat="c0"/>
<dxpg:PivotGridField Name="fieldQuarter"   Area="ColumnArea">
    <dxpg:PivotGridField.DataBinding>
        <dxpg:DataSourceColumnBinding ColumnName="OrderDate" GroupInterval="DateQuarter"/>
    </dxpg:PivotGridField.DataBinding>
</dxpg:PivotGridField>
<dxpg:PivotGridField Name="fieldSalesman"  Area="RowArea"     FieldName="SalesPerson"/>
```

## Format Condition Types

| Class | Format |
|---|---|
| `ColorScaleFormatCondition` | 2- or 3-point color gradient across the value range |
| `DataBarFormatCondition` | Excel-style data bars; bar length proportional to value |
| `IconSetFormatCondition` | Icons (3-arrows, 4-rating, 5-quarters, ...) per value range |
| `TopBottomRuleFormatCondition` | Top-N, bottom-N, above-average, below-average |
| `FormatCondition` | Value-based (`Equal`, `Less`, `Between`, expression) |

All inherit from `FormatConditionBase`.

> **Predefined vs. custom formats.** `PredefinedFormatName` accepts a built-in format name — this reference uses `"OrangeGradientDataBar"` (from the official example). Discover the full list of valid names in the design-time **Conditional Formatting Rules Manager** / **Format Condition Collection Editor** (or via IntelliSense). When you need exact control, assign a custom `Format` object instead (shown below for data bars and icon sets). `Format` and `PredefinedFormatName` are mutually exclusive.

## Key Properties — All Conditions

| `FormatConditionBase` member | Use |
|---|---|
| `MeasureName` | The data field whose values are formatted (by `PivotGridField.Name`) |
| `ApplyToSpecificLevel` | If `true`, apply only at the row/column intersection; if `false` (default), all data cells of the measure |
| `RowName` | When `ApplyToSpecificLevel=true`, the row field name |
| `ColumnName` | When `ApplyToSpecificLevel=true`, the column field name |
| `Format` | Custom format object (`dx:Format`, `dx:IconSetFormat`, etc.) |
| `PredefinedFormatName` | Use a built-in preset instead of `Format` (mutually exclusive) |

> **`TopBottomRuleFormatCondition` is an exception** — it always requires `ApplyToSpecificLevel="True"` with both `RowName` and `ColumnName`.

## Data Bars

```xaml
<dxpg:PivotGridControl.FormatConditions>
    <dxpg:DataBarFormatCondition
            MeasureName="fieldSales"
            RowName="fieldSalesman"
            ColumnName="fieldQuarter"
            ApplyToSpecificLevel="True"
            PredefinedFormatName="OrangeGradientDataBar"/>
</dxpg:PivotGridControl.FormatConditions>
```

`OrangeGradientDataBar` is one of the built-in data-bar formats. Pick the exact name for other built-in formats from the **Conditional Formatting Rules Manager** / Collection Editor (or IntelliSense), or define a custom `DataBarFormat` (below).

### Custom Data Bar Format

```xaml
<dxpg:DataBarFormatCondition MeasureName="fieldSales" ApplyToSpecificLevel="False">
    <dxpg:DataBarFormatCondition.Format>
        <dx:DataBarFormat>
            <dx:DataBarFormat.Fill>
                <LinearGradientBrush EndPoint="1,0">
                    <GradientStop Color="#FF63C384" Offset="0"/>
                    <GradientStop Color="White"     Offset="1"/>
                </LinearGradientBrush>
            </dx:DataBarFormat.Fill>
        </dx:DataBarFormat>
    </dxpg:DataBarFormatCondition.Format>
</dxpg:DataBarFormatCondition>
```

## Color Scales

```xaml
<dxpg:ColorScaleFormatCondition MeasureName="fieldSales"
                                PredefinedFormatName="..."/>
```

Set `PredefinedFormatName` to a built-in color-scale format — pick the exact name from the **Conditional Formatting Rules Manager** / Collection Editor (or IntelliSense) — or assign a custom `Format`.

## Icon Sets

```xaml
<dxpg:IconSetFormatCondition
        MeasureName="fieldSales"
        RowName="fieldSalesman" ColumnName="fieldYear"
        ApplyToSpecificLevel="True"
        PredefinedFormatName="..."/>
```

Set `PredefinedFormatName` to a built-in icon set — pick the exact name from the **Conditional Formatting Rules Manager** / Collection Editor (or IntelliSense) — or define a custom `IconSetFormat` (below).

### Custom Icon Set

```xaml
<dxpg:IconSetFormatCondition MeasureName="fieldSales" ApplyToSpecificLevel="False">
    <dxpg:IconSetFormatCondition.Format>
        <dx:IconSetFormat>
            <dx:IconSetElement Threshold="66.66"  ThresholdComparisonType="GreaterOrEqual">
                <dx:IconSetElement.Icon>
                    <BitmapImage UriSource="pack://application:,,,/MyAssembly;component/Icons/up.png"/>
                </dx:IconSetElement.Icon>
            </dx:IconSetElement>
            <dx:IconSetElement Threshold="33.33"  ThresholdComparisonType="GreaterOrEqual">
                <dx:IconSetElement.Icon>
                    <BitmapImage UriSource="pack://application:,,,/MyAssembly;component/Icons/flat.png"/>
                </dx:IconSetElement.Icon>
            </dx:IconSetElement>
            <dx:IconSetElement Threshold="0"      ThresholdComparisonType="GreaterOrEqual">
                <dx:IconSetElement.Icon>
                    <BitmapImage UriSource="pack://application:,,,/MyAssembly;component/Icons/down.png"/>
                </dx:IconSetElement.Icon>
            </dx:IconSetElement>
        </dx:IconSetFormat>
    </dxpg:IconSetFormatCondition.Format>
</dxpg:IconSetFormatCondition>
```

Thresholds work top-down; the first matching element wins. `ThresholdComparisonType`: `Greater`, `GreaterOrEqual`.

## Top / Bottom Rules

```xaml
<dxpg:TopBottomRuleFormatCondition
        MeasureName="fieldQuantity"
        RowName="fieldSalesman"
        ColumnName="fieldQuarter"
        ApplyToSpecificLevel="True"
        Rule="TopItems"
        Threshold="3">
    <dxpg:TopBottomRuleFormatCondition.Format>
        <dx:Format Background="LightGreen" Foreground="Green"/>
    </dxpg:TopBottomRuleFormatCondition.Format>
</dxpg:TopBottomRuleFormatCondition>
```

`Rule` values:

| Rule | Effect |
|---|---|
| `TopItems` | Top N items |
| `BottomItems` | Bottom N items |
| `TopPercent` | Top N % |
| `BottomPercent` | Bottom N % |
| `AboveAverage` | Values above the average |
| `BelowAverage` | Values below the average |

`Threshold` sets N (count or percent).

> Reminder: **`TopBottomRuleFormatCondition` requires `ApplyToSpecificLevel="True"`** with `RowName` and `ColumnName`.

## Value / Expression-Based — `FormatCondition`

```xaml
<dxpg:FormatCondition MeasureName="fieldSales"
                      Expression="[fieldSales] &gt; 100000">
    <dxpg:FormatCondition.Format>
        <dx:Format Background="LightYellow" Foreground="DarkOrange" FontWeight="Bold"/>
    </dxpg:FormatCondition.Format>
</dxpg:FormatCondition>
```

Or by comparison:

```xaml
<dxpg:FormatCondition MeasureName="fieldSales"
                      ValueRule="Between" Value1="50000" Value2="100000">
    <dxpg:FormatCondition.Format>
        <dx:Format Background="LightBlue"/>
    </dxpg:FormatCondition.Format>
</dxpg:FormatCondition>
```

`ValueRule` values: `Equal`, `NotEqual`, `Greater`, `GreaterOrEqual`, `Less`, `LessOrEqual`, `Between`, `NotBetween`, `Expression`

## Adding Conditions in Code

```csharp
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Core.ConditionalFormatting;

var bar = new DataBarFormatCondition {
    MeasureName       = "fieldSales",
    RowName           = "fieldSalesman",
    ColumnName        = "fieldQuarter",
    ApplyToSpecificLevel = true,
    PredefinedFormatName = "OrangeGradientDataBar"
};
pivotGrid.AddFormatCondition(bar);
```

Or `pivotGrid.FormatConditions.Add(bar)`.

To remove:

```csharp
pivotGrid.FormatConditions.Remove(bar);
pivotGrid.FormatConditions.Clear();
```

## Apply Scope — `ApplyToSpecificLevel`

| Setting | Scope |
|---|---|
| `ApplyToSpecificLevel="False"` (default) | All data cells for the measure. `RowName` / `ColumnName` ignored. |
| `ApplyToSpecificLevel="True"` | Only the cells at the row × column intersection. Both `RowName` and `ColumnName` required. |

Use `False` for measure-wide rules (e.g., color scale across all sales); use `True` to scope a rule to a specific slice (e.g., Q1 vs. salesperson).

## End-User Conditional Formatting

```xaml
<dxpg:PivotGridControl AllowConditionalFormattingMenu="True"
                       AllowConditionalFormattingManager="True"
                       .../>
```

| Property | Use |
|---|---|
| `AllowConditionalFormattingMenu` | Right-click a data cell → "Conditional Formatting" → add a rule |
| `AllowConditionalFormattingManager` | "Manage Rules…" — full CRUD UI for existing rules |

User-created rules are persisted with `SaveLayoutTo*` / `RestoreLayoutFrom*`.

## Common Patterns

### Pattern 1: Top-3 Performers Highlight

```xaml
<dxpg:TopBottomRuleFormatCondition
        MeasureName="fieldSales"
        RowName="fieldSalesman" ColumnName="fieldQuarter"
        ApplyToSpecificLevel="True"
        Rule="TopItems" Threshold="3">
    <dxpg:TopBottomRuleFormatCondition.Format>
        <dx:Format Background="LightGreen" Foreground="DarkGreen" FontWeight="Bold"/>
    </dxpg:TopBottomRuleFormatCondition.Format>
</dxpg:TopBottomRuleFormatCondition>
```

### Pattern 2: KPI Traffic-Light

```xaml
<dxpg:IconSetFormatCondition
        MeasureName="fieldMargin"
        PredefinedFormatName="..."
        ApplyToSpecificLevel="False"/>
<!-- Pick a built-in traffic-light (3-icon) set name from the Rules Manager,
     or define a custom IconSetFormat as shown above. -->
```

### Pattern 3: Per-Quarter Data Bar

```xaml
<dxpg:DataBarFormatCondition
        MeasureName="fieldSales"
        RowName="fieldSalesman" ColumnName="fieldQuarter"
        ApplyToSpecificLevel="True"
        PredefinedFormatName="OrangeGradientDataBar"/>
```

Each quarter's column has its own data-bar scale (max/min computed within that quarter), making cross-salesperson comparisons within a quarter clearer.

### Pattern 4: Threshold Highlight

```xaml
<dxpg:FormatCondition MeasureName="fieldProfit"
                      ValueRule="Less" Value1="0">
    <dxpg:FormatCondition.Format>
        <dx:Format Foreground="Red" FontWeight="Bold"/>
    </dxpg:FormatCondition.Format>
</dxpg:FormatCondition>
```

Highlight negative profits in red.

## Common Issues

- **Rule doesn't apply** — a referenced field has no `Name`. Set `Name` on every measure/row/column field.
- **`TopBottomRuleFormatCondition` does nothing** — `ApplyToSpecificLevel="False"`. Set it to `True` with both `RowName` and `ColumnName`.
- **`PredefinedFormatName` doesn't match anything** — typo, or it belongs to a different condition class (e.g., trying a data-bar preset on a color-scale rule). Each class has its own valid preset names.
- **Custom `Format` is ignored** — both `Format` and `PredefinedFormatName` set; the predefined name wins. Pick one.
- **Rule lost after restart** — user-created rules need persisting via the layout save/restore APIs.
- **Data bar / icons missing from PDF export** — conditional formatting using **icons** and **data bars** is **not** rendered in PDF / HTML / MHT / RTF / XLS(X) exports. Use a different rule type for printable reports.
- **Wrong cell highlighted** — `ApplyToSpecificLevel="True"` but only one of `RowName` / `ColumnName` set; both are required.

## Source Material

- `articles/controls-and-libraries/pivot-grid/conditional-formatting.md` (https://docs.devexpress.com/content/WPF/114038?md=true)
- `articles/controls-and-libraries/pivot-grid/end-user-capabilities/applying-conditional-formatting.md` (https://docs.devexpress.com/content/WPF/114395?md=true)
- `articles/controls-and-libraries/pivot-grid/examples/appearance/how-to-apply-conditional-formatting-to-data-cells.md` (https://docs.devexpress.com/content/WPF/8035?md=true)
