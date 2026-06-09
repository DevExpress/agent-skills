# Conditional Formatting — DevExpress WPF Data Grid

Conditional formatting changes a cell's or row's appearance based on its value. The engine is Excel-compatible: same metaphors (color scales, data bars, icon sets, comparison rules, top/bottom) and same export round-trip. Five format-condition classes cover every common case; XAML and code APIs are interchangeable.

## When to Use This Reference

Use this when you need to:

- Highlight cells or rows when a value compares to a threshold (greater, less, between, equal)
- Apply 2- or 3-color scales (heat-map style)
- Show data bars inside cells (mini bar chart per row)
- Show icon sets (arrows, traffic lights, stars) based on value buckets
- Highlight top N or bottom N values (absolute count or percent)
- Use custom criteria expressions (`[Birthday] > #1980-01-01#`)
- Apply formatting to the entire row (not just the value cell)
- Use predefined or custom format styles
- Define conditional formatting in a ViewModel via `FormatConditionsSource`

## The Five Format-Condition Classes

All live in `DevExpress.Xpf.Grid` (within the `DevExpress.Xpf.Grid.ConditionalFormatting` namespace, but accessible via the `dxg:` prefix). All derive from `FormatConditionBase`.

| Class | Use for | Distinctive properties |
|---|---|---|
| `FormatCondition` | Comparison rules (Greater / Less / Between / Equal / Contains) + custom expressions | `ValueRule` (`ConditionRule` enum), `Value1`, `Value2`, or `Expression` |
| `ColorScaleFormatCondition` | 2- / 3-color scale (heat map) | `Format` (`ColorScaleFormat`), `MinValue`, `MaxValue` |
| `DataBarFormatCondition` | Bar visualizing magnitude inside cell | `Format` (`DataBarFormat`), `MinValue`, `MaxValue` |
| `IconSetFormatCondition` | Icons (arrows, traffic lights, stars) per value bucket | `Format` (`IconSetFormat`), `MinValue`, `MaxValue` |
| `TopBottomRuleFormatCondition` | Highlight top / bottom N items or percent | `Rule` (`TopBottomRule` enum), `Threshold` |

**Container collection**: `TableView.FormatConditions` (or `TreeListView.FormatConditions` for trees).

All format conditions share:
- `FieldName` — the column to apply the rule to
- `Expression` — custom criteria expression (alternative to `FieldName` for multi-column logic)
- `PredefinedFormatName` — built-in format style
- `Format` — fully custom format (overrides `PredefinedFormatName`)

For indicator types (`ColorScaleFormatCondition`, `DataBarFormatCondition`, `IconSetFormatCondition`), there's also:
- `SelectiveExpression` — restrict which rows the indicator applies to
- `MinValue` / `MaxValue` (inherited from `IndicatorFormatConditionBase`) — range bounds (default: auto-detect from data)

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/`.

## Pattern 1: Comparison Rule (`FormatCondition`)

```xaml
<dxg:TableView.FormatConditions>
    <dxg:FormatCondition ValueRule="Greater"
                         Value1="10000000"
                         FieldName="Profit"
                         PredefinedFormatName="LightRedFillWithDarkRedText"/>
</dxg:TableView.FormatConditions>
```

```csharp
var condition = new FormatCondition {
    ValueRule = ConditionRule.Greater,
    Value1 = 10000000,
    FieldName = "Profit",
    PredefinedFormatName = "LightRedFillWithDarkRedText"
};
view.FormatConditions.Add(condition);
```

### `ConditionRule` Enum Values

- **Comparison**: `Greater`, `GreaterOrEqual`, `Less`, `LessOrEqual`, `Equal`, `NotEqual`
- **Range**: `Between`, `NotBetween` — use both `Value1` and `Value2`
- **Text**: `Contains`, `NotContains`, `BeginsWith`, `EndsWith`
- **Blank checks**: `IsBlank`, `IsNotBlank` — no Value1/Value2 needed
- **Unique**: `Unique`, `Duplicate`

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-comparison-rules.md`.

## Pattern 2: Custom Criteria Expression

For rules that depend on multiple columns or use complex logic, use `Expression` instead of `ValueRule` / `Value1`:

```xaml
<dxg:FormatCondition Expression="[Birthday] &gt; #1980-01-01# And [Birthday] &lt; #1990-01-01#"
                     FieldName="Birthday"
                     PredefinedFormatName="LightRedFillWithDarkRedText"/>
```

```csharp
new FormatCondition {
    Expression = "[Birthday] > #1980-01-01# And [Birthday] < #1990-01-01#",
    FieldName = "Birthday",
    PredefinedFormatName = "LightRedFillWithDarkRedText"
};
```

Expression uses the [DevExpress Criteria Language Syntax](https://docs.devexpress.com/content/CoreLibraries/4928?md=true) — fields in brackets, dates in `#...#`, operators `And`, `Or`, `Not`, etc.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-custom-conditions.md`.

## Pattern 3: Color Scale (`ColorScaleFormatCondition`)

```xaml
<dxg:TableView.FormatConditions>
    <dxg:ColorScaleFormatCondition FieldName="Profit"
                                   PredefinedFormatName="RedWhiteBlueColorScale"/>
</dxg:TableView.FormatConditions>
```

```csharp
view.FormatConditions.Add(new ColorScaleFormatCondition {
    FieldName = "Profit",
    PredefinedFormatName = "RedWhiteBlueColorScale"
});
```

### Custom Color Scale Format

```xaml
<dxg:ColorScaleFormatCondition FieldName="Profit">
    <dxg:ColorScaleFormatCondition.Format>
        <dx:ColorScaleFormat ElementThresholdType="Percent">
            <dx:ColorScaleElement Threshold="0"   Color="Red"/>
            <dx:ColorScaleElement Threshold="50"  Color="Yellow"/>
            <dx:ColorScaleElement Threshold="100" Color="Green"/>
        </dx:ColorScaleFormat>
    </dxg:ColorScaleFormatCondition.Format>
</dxg:ColorScaleFormatCondition>
```

`dx:` is `xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"`.

`ColorScaleElement.Threshold` + `Color` define each stop. `ElementThresholdType` is `Number` (raw value) or `Percent` (percent of data range).

> Verify exact `ColorScaleElement` / `ColorScaleFormat` schema against `apidoc/DevExpress.Xpf.Grid.ConditionalFormatting/ColorScaleFormat/` and `DevExpress.Xpf.Core.ConditionalFormatting` namespace before shipping. The pattern matches `IconSetFormat` which is documented; `ColorScaleFormat` follows the same shape.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-color-scales.md`.

## Pattern 4: Data Bars (`DataBarFormatCondition`)

```xaml
<dxg:TableView.FormatConditions>
    <dxg:DataBarFormatCondition FieldName="Profit"
                                PredefinedFormatName="BlueGradientDataBar"/>
</dxg:TableView.FormatConditions>
```

```csharp
view.FormatConditions.Add(new DataBarFormatCondition {
    FieldName = "Profit",
    PredefinedFormatName = "BlueGradientDataBar"
});
```

### Custom Data Bar Format

```xaml
<dxg:DataBarFormatCondition FieldName="Profit"
                            MinValue="0"
                            MaxValue="10000">
    <dxg:DataBarFormatCondition.Format>
        <dx:DataBarFormat PositiveFill="LightGreen"
                          NegativeFill="LightCoral"
                          ShowValue="True"/>
    </dxg:DataBarFormatCondition.Format>
</dxg:DataBarFormatCondition>
```

`PositiveFill`, `NegativeFill`, `ShowValue` are paraphrased — verify exact `DataBarFormat` property names against `apidoc/DevExpress.Xpf.Grid.ConditionalFormatting/DataBarFormat/` if needed.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-data-bars.md`.

## Pattern 5: Icon Sets (`IconSetFormatCondition`)

```xaml
<dxg:TableView.FormatConditions>
    <dxg:IconSetFormatCondition FieldName="Profit"
                                PredefinedFormatName="Stars3IconSet"/>
</dxg:TableView.FormatConditions>
```

```csharp
view.FormatConditions.Add(new IconSetFormatCondition {
    FieldName = "Profit",
    PredefinedFormatName = "Stars3IconSet"
});
```

### Custom Icon Set with Thresholds

```xaml
<dxg:IconSetFormatCondition FieldName="Profit">
    <dxg:IconSetFormatCondition.Format>
        <dx:IconSetFormat ElementThresholdType="Number">
            <dx:IconSetElement Threshold="-20" Icon="{dx:IconSet Name=RedToBlack4_1}"/>
            <dx:IconSetElement Threshold="0"   Icon="{dx:IconSet Name=Stars3_2}"/>
            <dx:IconSetElement Threshold="10"  Icon="{dx:IconSet Name=Stars3_1}"/>
        </dx:IconSetFormat>
    </dxg:IconSetFormatCondition.Format>
</dxg:IconSetFormatCondition>
```

`IconSetElement.Threshold` is the **minimum value required** to display this icon (so the first matching threshold from highest to lowest wins). `ElementThresholdType`: `Number` (raw) or `Percent` (relative to MinValue/MaxValue range).

`{dx:IconSet Name=...}` — markup extension referencing a built-in icon. The Name catalogue includes `RedToBlack4_1`, `Stars3_1`, `Stars3_2`, `Arrows3_1`, `Arrows3_2`, `Arrows5_1`, `Quarters5_1`, `Boxes5_1`, `Ratings5_1`, etc. See https://docs.devexpress.com/content/WPF/118922?md=true for the full catalogue.

### Min/Max Range

```xaml
<dxg:IconSetFormatCondition FieldName="SatisfactionScore"
                            PredefinedFormatName="Quarters5IconSet"
                            MinValue="0"
                            MaxValue="1"/>
```

When `ElementThresholdType="Percent"`, `MinValue`/`MaxValue` define the 0–100% range. Defaults (when omitted): the min/max of the data column.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-icon-sets.md`.

## Pattern 6: Top / Bottom (`TopBottomRuleFormatCondition`)

```xaml
<dxg:TableView.FormatConditions>
    <dxg:TopBottomRuleFormatCondition Rule="TopItems"
                                      Threshold="20"
                                      FieldName="Profit"
                                      PredefinedFormatName="GreenFillWithDarkGreenText"/>
</dxg:TableView.FormatConditions>
```

```csharp
view.FormatConditions.Add(new TopBottomRuleFormatCondition {
    Rule = TopBottomRule.TopItems,
    Threshold = 20,
    FieldName = "Profit",
    PredefinedFormatName = "GreenFillWithDarkGreenText"
});
```

### `TopBottomRule` Enum Values

- `TopItems` — top N items (absolute count)
- `TopPercent` — top N percent
- `BottomItems` — bottom N items
- `BottomPercent` — bottom N percent
- `AboveAverage` — items above the column's average
- `BelowAverage` — items below the column's average

For `AboveAverage` / `BelowAverage`, `Threshold` is ignored.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-top-and-bottom-values.md`.

## Apply Formatting to the Entire Row

By default, the format applies only to the column's cell. To highlight the whole row:

```xaml
<dxg:FormatCondition ValueRule="Greater"
                     Value1="6"
                     FieldName="Visits"
                     ApplyToRow="True">
    <dxg:Format Background="Aqua"/>
</dxg:FormatCondition>
```

`ApplyToRow="True"` extends the highlight across all cells of the matching row.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats.md` and example in `printing-and-exporting/data-aware-export.md` § "Excel Style Conditional Formatting".

## Custom `Format` Object (No Predefined Style)

```xaml
<dxg:FormatCondition ValueRule="Less" Value1="0" FieldName="Profit">
    <dxg:Format Background="#FFFCDDDD"
                Foreground="DarkRed"
                FontWeight="Bold"/>
</dxg:FormatCondition>
```

`dxg:Format` has the basic appearance properties:
- `Background` (Brush)
- `Foreground` (Brush)
- `FontWeight`, `FontStyle`, `FontSize`, `FontFamily`
- `BorderBrush`, `BorderThickness`
- `HorizontalAlignment`, `VerticalAlignment`

> Verify exact `Format` properties against `apidoc/DevExpress.Xpf.Core.ConditionalFormatting/Format/` if you need esoteric properties. Common ones above are reliable.

## `SelectiveExpression` — Restrict Indicator Rules

For Color Scale, Data Bar, and Icon Set conditions: apply the visual indicator only to certain rows (others get no indicator):

```xaml
<dxg:IconSetFormatCondition FieldName="Profit"
                            PredefinedFormatName="Stars3IconSet"
                            SelectiveExpression="[Department] = 'Sales'"/>
```

Only rows where Department is Sales show the stars; other rows display the raw value.

## Predefined Format Catalogues

| Catalogue | Where | Used by |
|---|---|---|
| `TableView.PredefinedFormats` | View | `FormatCondition`, `TopBottomRuleFormatCondition` |
| `TableView.PredefinedColorScaleFormats` | View | `ColorScaleFormatCondition` |
| `TableView.PredefinedDataBarFormats` | View | `DataBarFormatCondition` |
| `TableView.PredefinedIconSetFormats` | View | `IconSetFormatCondition` |

Common predefined names (use as `PredefinedFormatName="..."`):
- **Cell formats**: `LightRedFillWithDarkRedText`, `YellowFillWithDarkYellowText`, `GreenFillWithDarkGreenText`, `LightRedFill`, `RedText`, `RedBorder`, `BoldText`, `ItalicText`
- **Color scales**: `RedWhiteBlueColorScale`, `RedYellowGreenColorScale`, `RedWhiteColorScale`, `WhiteRedColorScale`, `GreenWhiteColorScale`, `BlueWhiteRedColorScale`, plus 3-color variants
- **Data bars**: `BlueGradientDataBar`, `GreenGradientDataBar`, `RedGradientDataBar`, `OrangeGradientDataBar`, `PurpleGradientDataBar`, plus `*SolidDataBar` variants
- **Icon sets**: `Arrows3IconSet`, `Arrows5IconSet`, `Stars3IconSet`, `Quarters5IconSet`, `Boxes5IconSet`, `Ratings5IconSet`, `Indicators3IconSet`, `Flags3IconSet`

The same names are usable across XAML and code.

## Multiple Conditions on the Same Column

`FormatConditions` is a collection — add as many as you need. When multiple conditions match the same cell, the **last matching condition wins** (latest in the collection order):

```xaml
<dxg:TableView.FormatConditions>
    <dxg:FormatCondition ValueRule="Less" Value1="0" FieldName="Profit"
                         PredefinedFormatName="LightRedFill"/>
    <dxg:FormatCondition ValueRule="Greater" Value1="100000" FieldName="Profit"
                         PredefinedFormatName="GreenFillWithDarkGreenText"/>
    <dxg:DataBarFormatCondition FieldName="Profit" PredefinedFormatName="BlueGradientDataBar"/>
</dxg:TableView.FormatConditions>
```

The data bar appears under the colored background (data bars compose with cell colors, not replace).

## End-User Conditional Formatting

Users can add rules at runtime via:

- **Conditional Formatting Menu** — column header context menu → "Conditional Formatting" → choose rule type. Limited to predefined formats from the `Predefined*` catalogues.
- **Conditional Formatting Rules Manager** — full UI for creating / editing / reordering / deleting rules. Supports custom expressions and the **Format Cells Dialog** for custom appearance.

```xaml
<dxg:TableView AllowConditionalFormattingMenu="True"
               AllowConditionalFormattingManager="True"/>
```

> Verify exact property names (`AllowConditionalFormattingMenu`, `AllowConditionalFormattingManager`) via DxDocs MCP if your version differs.

Source: `articles/controls-and-libraries/data-grid/conditional-formatting/creating-conditional-formatting-rules/conditional-formatting-menu.md` and `conditional-formatting-rules-manager.md`.

## MVVM: Bind Conditional Formats from a ViewModel

Use `TableView.FormatConditionsSource` + `FormatConditionGeneratorTemplate`:

```xaml
<dxg:TableView FormatConditionsSource="{Binding FormatRules}">
    <dxg:TableView.FormatConditionGeneratorTemplate>
        <DataTemplate>
            <dxg:FormatCondition FieldName="{Binding FieldName}"
                                 ValueRule="{Binding Rule}"
                                 Value1="{Binding Value}"
                                 PredefinedFormatName="{Binding Format}"/>
        </DataTemplate>
    </dxg:TableView.FormatConditionGeneratorTemplate>
</dxg:TableView>
```

`FormatRules` is an `ObservableCollection<FormatRuleDefinition>` on the ViewModel. See https://docs.devexpress.com/content/WPF/117884?md=true and `articles/controls-and-libraries/data-grid/mvvm-enhancements/bind-to-collections-specified-in-the-viewmodel.md`.

## Mode Compatibility

- **Optimized mode**: required for conditional formatting. Set via `TableView.DataProcessingEngine` or `GridControl` default.
- **Hierarchical Data Templates** (in `TreeListView`): conditional formatting does NOT work.
- **Server Mode / Virtual Sources**: most conditional formats work, but **top/bottom rules** (TopItems, TopPercent, BottomItems, BottomPercent, AboveAverage, BelowAverage) are NOT supported.
- **Custom `CellTemplate`**: works only if the template contains a `BaseEdit` descendant named `PART_Editor`. See [cell-display-and-editing.md](cell-display-and-editing.md) § "CellTemplate vs CellDisplayTemplate".

Source: `articles/controls-and-libraries/data-grid/conditional-formatting.md` § "Usage Notes".

## Conditional Formatting Filters

Filter rows to show only those matching a conditional format:

```csharp
grid.FilterString = "[@TopItems]([Profit], 5)";   // Show only rows in Top 5 by Profit
```

The `@` prefix signals "matches the named conditional-formatting rule on this column". See [sorting-filtering-grouping.md](sorting-filtering-grouping.md) § "Conditional Formatting Filters".

Source: `articles/controls-and-libraries/data-grid/filtering-and-searching/conditional-formatting-filters.md`.

## Print and Export

- **Data-Aware export** (XLSX/CSV/XLS): only `FormatCondition` (regular comparison rules) survive. Color scales, data bars, icon sets, and top/bottom rules are NOT exported as native Excel rules in Data-Aware mode. Data values are exported with the underlying values intact.
- **WYSIWYG export** (PDF/HTML/RTF): all visual indicators export as rendered pixels.

If you need conditional formatting to round-trip to Excel as live rules, stick with `FormatCondition` + `PredefinedFormatName`.

Source: [printing-exporting.md](printing-exporting.md) § "Data-Aware Limitations".

## Apply to TreeListControl

| GridControl member | TreeListControl equivalent |
|---|---|
| `TableView.FormatConditions` | `TreeListView.FormatConditions` |
| `TableView.PredefinedFormats` etc. | `TreeListView.PredefinedFormats` etc. |
| `FormatCondition`, `ColorScaleFormatCondition`, etc. | identical (same classes) |
| `TableView.FormatConditionsSource` | `TreeListView.FormatConditionsSource` |

All concepts and APIs are identical.

## Common Issues

- **`FormatCondition` doesn't fire** — column uses custom `CellTemplate` without a `PART_Editor`-named `BaseEdit` inside. Add it, or use `EditSettings` instead.
- **Top / Bottom rule shows nothing** — running in Server Mode / Virtual Source. These rules are unsupported there.
- **Custom `Format` overrides predefined** — by design. Set only one of `PredefinedFormatName` or `Format`.
- **Data bars don't show negative values correctly** — set `MinValue` to a negative number; otherwise the bar's zero point is at `MinValue` (default is min of data).
- **Icon set picks wrong icon** — thresholds are evaluated from highest to lowest; the first matching threshold wins. Order matters in XAML (high values listed last).
- **Multiple rules conflict** — last-matching wins. Reorder the `FormatConditions` collection or use `SelectiveExpression` for exclusivity.
- **ViewModel `FormatConditionsSource` doesn't update** — collection must implement `INotifyCollectionChanged` (use `ObservableCollection<T>`).
- **Excel-export round-trip loses scales / bars / icons** — Data-Aware export only preserves `FormatCondition`. Use WYSIWYG mode for visual fidelity.

## Source Material

- `articles/controls-and-libraries/data-grid/conditional-formatting.md` (root)
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-comparison-rules.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-custom-conditions.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-color-scales.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-data-bars.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-values-using-icon-sets.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-top-and-bottom-values.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-date-time-values.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-above-or-below-average-values.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-unique-and-duplicate-values.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-changing-values.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-focused-cells-and-rows.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/creating-conditional-formatting-rules/conditional-formatting-menu.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/creating-conditional-formatting-rules/conditional-formatting-rules-manager.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/conditional-formatting-filters.md`
- `apidoc/DevExpress.Xpf.Grid.ConditionalFormatting/` (apidoc namespace)
