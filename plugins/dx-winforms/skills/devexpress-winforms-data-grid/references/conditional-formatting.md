# Conditional Formatting

This reference covers conditional formatting (a.k.a. format rules) for `GridView`, `BandedGridView`, `AdvBandedGridView`, and `TreeList`. Rules attach to a column or band, evaluate a condition per row, and apply an appearance (color, font, icon, data bar) when the condition is true. End users can also create and manage rules at runtime via the Conditional Formatting context menu.

## When to Use This Reference

- Highlighting overdue / out-of-range / negative values without writing custom-draw code.
- Visualizing magnitude with data bars or icon sets (Excel-style).
- Combining multiple rules on the same column and deciding precedence (`StopIfTrue`).
- Animating cells that change value at runtime.
- Letting end users add and edit rules from the column context menu.

## How Rules Work

A format rule is a triplet:

1. **Target** — the column whose cells are evaluated, plus optional `ColumnApplyTo` to choose which columns get styled (e.g., "highlight the whole row, decision based on Status column").
2. **Rule** (`FormatConditionRuleBase` descendant) — the condition (e.g., "value > 1000") plus the style (`Appearance` or `PredefinedName`).
3. **Rule container** (`GridFormatRule` for the grid, `TreeListFormatRule` for the TreeList) — wraps the rule and registers it with the view.

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;

var rule = new FormatConditionRuleValue {
    Condition = FormatCondition.Greater,
    Value1    = 1000m,
    PredefinedName = "Red Text"
};
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["Total"],
    Rule   = rule
});
```

For `TreeList`, use `TreeListFormatRule`:

```csharp
treeList1.FormatRules.Add(new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule {
    Column = treeList1.Columns["Budget"],
    Rule   = rule
});
```

## Rule Types

| Rule class | Purpose | UI Equivalent |
|---|---|---|
| `FormatConditionRuleValue` | Compare cell value to one or two constants. | "Cell Value Is …" |
| `FormatConditionRuleExpression` | Boolean expression in DevExpress criteria syntax. | "Use a formula to determine …" |
| `FormatConditionRuleContains` | Cell contains a substring or any value from a list. | "Text that contains …" |
| `FormatConditionRuleAboveBelowAverage` | Compare to column average. | "Above/Below average" |
| `FormatConditionRuleDateOccuring` | Compare a date to a relative period (today, last week, …). | "A date occurring" |
| `FormatConditionRuleTopBottom` | Highlight top/bottom N or N % rows. | "Top/Bottom N" |
| `FormatConditionRule2ColorScale` | Two-color gradient based on value magnitude. | "Color Scales (2)" |
| `FormatConditionRule3ColorScale` | Three-color gradient. | "Color Scales (3)" |
| `FormatConditionRuleDataBar` | In-cell horizontal bar. Supports positive/negative bars. | "Data Bars" |
| `FormatConditionRuleIconSet` | Map value ranges to icons (arrows, traffic lights, ratings…). | "Icon Sets" |
| `FormatConditionRuleDataUpdate` | Animate cell when value changes (highlight, icon flash). | (programmatic only) |
| `FormatConditionRuleUniqueDuplicate` | Highlight unique or duplicate values. | "Unique/Duplicate" |

## Predefined Styles

Most rules expose a `PredefinedName` property to pick a built-in style without specifying colors manually:

- Text/Fill colors: `"Red Text"`, `"Yellow Fill with Dark Yellow Text"`, `"Green Fill"`, `"Light Red Fill"`, etc.
- Color scales: `"Blue White Red"`, `"Green Yellow Red"`, etc.
- Data bars: `"Coral"`, `"Blue Gradient"`, `"Green Gradient"`, etc.
- Icon sets: `"Arrows3_2"`, `"Symbols3Uncircled"`, `"Ratings4"`, `"Flags3_1"`, etc.

You can override or fully customize via `rule.Appearance.BackColor`, `rule.Appearance.ForeColor`, `rule.Appearance.Font`, etc.

## Examples

### Value comparison

```csharp
var rule = new FormatConditionRuleValue {
    Condition = FormatCondition.Less,
    Value1    = 0,
    PredefinedName = "Light Red Fill with Dark Red Text"
};
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["Profit"],
    Rule   = rule
});
```

### Expression that references multiple columns

```csharp
var rule = new FormatConditionRuleExpression {
    Expression = "[Quantity] > 10 And [Discount] > 0.1",
    PredefinedName = "Yellow Fill"
};
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["OrderID"],
    Rule   = rule
});
```

### Data bar — positive and negative

```csharp
var bar = new FormatConditionRuleDataBar {
    PredefinedName    = "Blue Gradient",
    AllowNegative     = true,
    ShowAxis          = true,
    GradientFill      = true
};
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["Change"],
    Rule   = bar
});
```

### Icon set — three-arrow trend

```csharp
var icons = new FormatConditionRuleIconSet {
    IconSet = new FormatConditionIconSet()
};
icons.IconSet.Icons.Add(new FormatConditionIconSetIcon {
    PredefinedName = "Arrows3_1.png", Value = 0,   ValueComparison = FormatConditionComparisonType.GreaterOrEqual
});
icons.IconSet.Icons.Add(new FormatConditionIconSetIcon {
    PredefinedName = "Arrows3_2.png", Value = 15,  ValueComparison = FormatConditionComparisonType.GreaterOrEqual
});
icons.IconSet.Icons.Add(new FormatConditionIconSetIcon {
    PredefinedName = "Arrows3_3.png", Value = 25,  ValueComparison = FormatConditionComparisonType.GreaterOrEqual
});
icons.IconSet.ValueType = FormatConditionValueType.Number;
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["MarketShare"],
    Rule   = icons
});
```

### Animation on value change

```csharp
var update = new FormatConditionRuleDataUpdate {
    HighlightTime = 500,
    PredefinedName = "Green Fill",
    Trigger        = FormatConditionDataUpdateTrigger.ValueIncreased
};
update.Icon.PredefinedName = "Flags3_1.png";
gridView1.FormatRules.Add(new GridFormatRule {
    Column = gridView1.Columns["Price"],
    Rule   = update
});
```

`FormatConditionRuleDataUpdate` is supported only in `GridView`, `BandedGridView`, `AdvBandedGridView`.

## Apply Style to a Different Column or to the Entire Row

A rule evaluates against one column but can style other columns. Use `ColumnApplyTo` to set the styled columns:

```csharp
var rule = new FormatConditionRuleValue {
    Condition = FormatCondition.Equal,
    Value1    = "Overdue",
    PredefinedName = "Light Red Fill"
};
var grid = new GridFormatRule {
    Column = gridView1.Columns["Status"],   // condition column
    Rule   = rule
};
grid.ColumnApplyTo.Add(gridView1.Columns["OrderID"]);
grid.ColumnApplyTo.Add(gridView1.Columns["Customer"]);
grid.ApplyToRow = true;                     // style the whole row
gridView1.FormatRules.Add(grid);
```

> `ColumnApplyTo` is not supported for `FormatConditionRuleIconSet` (icon rules apply to the rule's `Column`).

## Combine Multiple Rules — `StopIfTrue`

Multiple rules on the same column combine by default — later rules add to earlier appearances. Set `FormatRuleBase.StopIfTrue = true` to short-circuit evaluation when the rule matches.

## End-User Rule Manager

Show the Conditional Formatting submenu in the column header context menu:

```csharp
gridView1.OptionsMenu.ShowConditionalFormattingItem = true;
```

Users can pick predefined rules from the menu or open *Manage Rules…* for a full editor that supports custom expressions. Rules created at runtime live in `view.FormatRules` like programmatic rules and persist via layout save/restore.

## Conditional Formatting Filters

Once rules exist, users can filter rows that match a rule via the column header menu. See [filtering-and-search.md](filtering-and-search.md) for the *Conditional Formatting Filters* feature.

## Common Issues

- **Rule does not fire**: confirm the column's `FieldName` matches the data. Expressions use `[FieldName]` syntax.
- **Icon set rule does not style the entire row**: not supported. Use a value or expression rule and `ApplyToRow`.
- **Animation does not play**: animation only runs in `GridView` family; ensure `FormatConditionRuleBase.AllowAnimation = true` for rules that support it.
- **Rules persist but appearance does not**: default `SaveLayoutTo…` skips appearance. Pass `new OptionsLayoutGrid { StoreAppearance = true }` — see [saving-and-restoring-layout.md](saving-and-restoring-layout.md).
- **Multiple rules clash**: order them and use `StopIfTrue` to express priority.
