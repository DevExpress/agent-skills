# Appearance and Conditional Formatting

Style the tree with appearances, apply rule-based conditional formatting, and owner-draw cells.

## When to Use This Reference

- Applying conditional formatting (color scales, data bars, icon sets, value/expression rules)
- Customizing appearances per element, column, or cell
- Owner-drawing via custom draw events

## Appearances

Appearance settings (colors, fonts, alignment) exist at several levels:

- `treeList.Appearance` — control-wide element appearances (`Row`, `HeaderPanel`, `FocusedRow`, etc.)
- `column.AppearanceCell` / `column.AppearanceHeader` — per-column
- per-cell — via the custom draw events below or node appearance API

```csharp
using System.Drawing;

// Bold a column's cells
var col = treeList.Columns["MarchSalesPrev"];
col.AppearanceCell.Font = new Font(col.AppearanceCell.Font, FontStyle.Italic);
```

HTML formatting in cells/headers:

```csharp
treeList.OptionsView.AllowHtmlDrawHeaders = true;   // headers
// (cell HTML support is controlled by the relevant editor / OptionsView settings)
```

## Conditional Formatting (Format Rules)

Format rules apply visual styles based on cell values — the modern, declarative alternative to custom drawing. Rules are `TreeListFormatRule` objects (with a `Rule` from `DevExpress.XtraTreeList.StyleFormatConditions` / `DevExpress.XtraEditors`) added to `treeList.FormatRules`.

Rule kinds include: `FormatConditionRuleValue` (highlight cells), `FormatConditionRuleDataBar`, `FormatConditionRule2ColorScale` / `3ColorScale`, `FormatConditionRuleIconSet`, `FormatConditionRuleExpression`, `FormatConditionRuleTopBottom`.

### Icon Set Example (verified)

```csharp
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraEditors;   // FormatConditionRuleIconSet, FormatConditionIconSet, ...

FormatConditionRuleIconSet CreateThreeTrianglesRule() {
    var ruleIconSet = new FormatConditionRuleIconSet();
    var iconSet = ruleIconSet.IconSet = new FormatConditionIconSet();
    var icon1 = new FormatConditionIconSetIcon();
    var icon2 = new FormatConditionIconSetIcon();
    var icon3 = new FormatConditionIconSetIcon();
    icon1.PredefinedName = "Triangles3_3.png";
    icon2.PredefinedName = "Triangles3_2.png";
    icon3.PredefinedName = "Triangles3_1.png";
    iconSet.ValueType = FormatConditionValueType.Number;
    icon1.Value = decimal.MinValue; icon1.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;
    icon2.Value = 0;                icon2.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;
    icon3.Value = 0;                icon3.ValueComparison = FormatConditionComparisonType.Greater;
    iconSet.Icons.Add(icon1);
    iconSet.Icons.Add(icon2);
    iconSet.Icons.Add(icon3);
    return ruleIconSet;
}

// Apply: drive the icon from one (often hidden, unbound) column, show it on another
var rule = new TreeListFormatRule {
    Rule = CreateThreeTrianglesRule(),
    Column = treeList.Columns["FromPrevMarchChange"],  // value source
    ColumnApplyTo = treeList.Columns["MarchSales"]     // where the icon shows
};
treeList.FormatRules.Add(rule);
```

`Column` is the column whose value the rule evaluates; `ColumnApplyTo` (optional) is where the formatting is rendered. Omit `ColumnApplyTo` to format the evaluated column itself.

## Custom Drawing

For full owner-drawing, handle the custom draw events and paint with `e.Graphics` / `e.Cache` (set `e.Handled = true` when you draw everything yourself):

- `CustomDrawNodeCell` — individual data cells
- `CustomDrawNodeIndicator`, `CustomDrawNodeImages`, `CustomDrawColumnHeader`, `CustomDrawFooterCell` — other elements

```csharp
treeList.CustomDrawNodeCell += (s, e) => {
    if (e.Column.FieldName == "Status" && Equals(e.CellValue, "Overdue")) {
        e.Appearance.BackColor = System.Drawing.Color.MistyRose;
        // e.Handled = true; // only if you fully paint the cell yourself
    }
};
```

> Prefer **format rules** over custom drawing for value-based styling — they are declarative, serialize with the layout, and require less code. Reserve custom drawing for visuals rules can't express.

## Source Material

- [CustomDrawNodeCell](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.CustomDrawNodeCell?md=true), [FormatRules](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.FormatRules?md=true)
