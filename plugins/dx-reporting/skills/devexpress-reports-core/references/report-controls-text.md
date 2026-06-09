# Text Controls

## When to Use This Reference

Use when adding text and formatted text controls to report bands: `XRLabel`, `XRRichText`, `XRCharacterComb`.

## XRLabel — Text and Data Fields

Most common text/field control useful for standalone text:

```csharp
var label = new XRLabel();
detail.Controls.Add(label);
label.LocationF = new System.Drawing.PointF(0, 0);
label.SizeF = new System.Drawing.SizeF(150, 25);
label.Font = new DevExpress.Drawing.DXFont("Arial", 10f, DevExpress.Drawing.DXFontStyle.Bold);
label.ForeColor = Color.Black;
label.TextFormatString = "{0:C2}"; // format string for numeric/date fields
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
```

**Main content property**: `Text`.

**Key properties**: `TextFormatString`, `Font`, `ForeColor`, `BackColor`, `Borders`, `TextAlignment`, `Multiline`, `WordWrap`, `CanGrow`, `CanShrink`, `Angle` (rotation in degrees; `CanGrow`/`CanShrink` have no effect when non-zero), `LineSpacing`, `AutoWidth`, `TextFitMode` (None/GrowOnly/ShrinkOnly/ShrinkAndGrow).

`AllowMarkupText = true` enables HTML-style markup tags in `Text` (e.g., `<b>`, `<color=red>`, `<href=...>`, `<image id="...">`). **Limitations when enabled**: `Justify` text alignment values have no effect; `Angle` has no effect; `LineSpacing` has no effect; content exports as image to HTML/MHT/RTF formats.

**Tabular layout**: When displaying two or more fields side-by-side as columns inside a band, use `XRTable` / `XRTableRow` / `XRTableCell`. See [report-controls-table.md](report-controls-table.md) and the SKILL.md **Antipatterns** section (AP3).

## XRRichText — HTML/RTF/DOCX Formatted Content

HTML/RTF/DOCX formatted content:

```csharp
var richText = new XRRichText();
detail.Controls.Add(richText);
richText.BoundsF = new RectangleF(0, 0, 300, 100);
richText.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Rtf", "[Notes]"));
```

**Main content properties**: `Rtf` (RTF/DOCX string), `Html` (HTML string). Set one or the other — not both. Do not use `Text` property with `XRRichText`.

> **Font/style caveat**: When content is supplied via expression bindings (from a data field), the control renders the content's own formatting. Setting `Font`, `ForeColor`, or other style properties on the control itself has **no effect** on data-bound content. To modify fonts/styles of data-bound RTF content, handle `BeforePrint` and use `RichEditDocumentServer` to manipulate the `Rtf` string before rendering.

## XRCharacterComb — Fixed-Cell Character Grid

Fixed-cell character grid (for forms):

```csharp
var comb = new XRCharacterComb();
detail.Controls.Add(comb);
comb.BoundsF = new RectangleF(0, 0, 200, 25);
comb.CellWidth = 15;
comb.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Code]"));
```

**Main content property**: `Text`.

**Key properties**: `CellWidth` (fixed pixel width per character cell), `CellHeight`, `CellVerticalSpacing`, `CellHorizontalSpacing`.
