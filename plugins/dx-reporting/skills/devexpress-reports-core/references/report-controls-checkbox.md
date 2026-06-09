# Checkbox Controls

## When to Use This Reference

Use when displaying a checkbox glyph with optional label text: `XRCheckBox`.

## XRCheckBox — Checkbox with Customizable Glyph

Displays a checkbox glyph with optional label text:

```csharp
var check = new XRCheckBox();
detail.Controls.Add(check);
check.BoundsF = new RectangleF(0, 0, 150, 20);
check.Text = "In Stock";
// CheckBoxState accepts 'Checked', 'Unchecked', or 'Indeterminate'
check.ExpressionBindings.Add(
    new ExpressionBinding("BeforePrint", "CheckBoxState",
        "Iif([UnitsInStock] >= 1, 'Checked', 'Unchecked')"));

// Customize glyph appearance
check.GlyphOptions.Style = DevExpress.XtraPrinting.GlyphStyle.Thumb;
check.GlyphOptions.Size = new SizeF(16, 16);
check.GlyphOptions.Alignment = DevExpress.Utils.HorzAlignment.Near;

// Optional: use custom SVG glyphs per state
check.GlyphOptions.CustomGlyphs[CheckBoxState.Checked] = ImageSource.FromFile("checked.svg");
check.GlyphOptions.CustomGlyphs[CheckBoxState.Unchecked] = ImageSource.FromFile("unchecked.svg");
```

**Main content properties**: 
- `CheckBoxState` (the main property, the checkbox state: `Checked`, `Unchecked`, or `Indeterminate`)
- `Text` (label text displayed next to the glyph)

**Other key properties**:
- `GlyphOptions.Style` — predefined glyph style from `GlyphStyle` enum (e.g., StandardBox1, StandardBox2, YesNoBox, YesNoSolidBox, YesNo, RadioButton, Smiley, Thumb, Toggle, Star, Heart).
- `GlyphOptions.Alignment` — horizontal glyph position within the control: `Near` (left), `Center`, `Far` (right).
- `GlyphOptions.Size` — glyph pixel dimensions (`SizeF`).
