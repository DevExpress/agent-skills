# Axis Titles and Labels

This reference covers axis title configuration (text, alignment, font), axis label formatting via `TextPattern` placeholders, custom formatters, and `CustomAxisLabel` callouts.

For axis configuration in general see [axes.md](axes.md). For aggregation see [aggregation-and-summary.md](aggregation-and-summary.md).

## When to Use This Reference

- Adding an axis title and formatting its appearance.
- Formatting axis labels with `{V}` / `{A}` / `{VP}` placeholders + .NET format specifiers.
- Substituting a custom `IFormatProvider` via `AxisLabel.Formatter`.
- Adding callout labels at specific positions with `CustomAxisLabel`.
- Customizing each label's appearance via the `CustomDrawAxisLabel` event.

## Axis Title

```csharp
var diagram = (XYDiagram)chart.Diagram;

diagram.AxisY.Title.Text       = "Revenue, USD";
diagram.AxisY.Title.Visibility = DefaultBoolean.True;
diagram.AxisY.Title.Alignment  = StringAlignment.Center;
diagram.AxisY.Title.TextColor  = Color.DimGray;
diagram.AxisY.Title.Font       = new Font("Segoe UI", 10, FontStyle.Bold);
diagram.AxisY.Title.Antialiasing = true;
```

| Member | Purpose |
|---|---|
| `Axis.Title.Text` | The text. |
| `Axis.Title.Visibility` | `True` / `False` / `Default`. **Default is `False`** — you must set it to `True` or the title stays hidden. |
| `Axis.Title.Alignment` | `Near` / `Center` / `Far` along the axis. |
| `Axis.Title.Font` | Title font (preferred — used at runtime). |
| `Axis.Title.TextColor` | Text color. |
| `Axis.Title.Antialiasing` | Smooth font rendering. |
| `Axis.Title.MaxLineCount` | Wrap long titles into N lines. |

Tip: `Axis.Title.Visibility = DefaultBoolean.True` is the **most common pitfall** — `Title.Text` alone shows nothing.

### Multi-line and rotated titles

The axis chooses orientation automatically — horizontal for X axes, vertical for Y axes. To rotate or skew the title, use the `CustomDrawAxisTitle` event (rare; usually leave alone).

## Axis Labels — `Axis.Label`

Every axis has an `AxisLabel` instance that controls how tick values are rendered.

```csharp
var lab = diagram.AxisX.Label;
lab.Visible      = true;
lab.TextPattern  = "{A:MMM yyyy}";
lab.Font         = new Font("Segoe UI", 9);
lab.TextColor    = Color.Gray;
lab.Angle        = -30;                 // rotate
lab.Antialiasing = true;
lab.Staggered    = true;                // alternate up/down lines to fit
lab.EnableAntialiasing = DefaultBoolean.True;
```

| Member | Purpose |
|---|---|
| `Visible` | Hide all labels on this axis. |
| `TextPattern` | Format string with placeholders (see below). |
| `Font` / `TextColor` / `Antialiasing` / `EnableAntialiasing` | Appearance. |
| `Angle` | Rotation degrees (positive = counter-clockwise). |
| `Staggered` | Alternate lines so long labels do not collide. |
| `ResolveOverlappingOptions.AllowRotate` / `AllowStagger` / `AllowHide` | Strategy when labels would overlap. |
| `Formatter` | Custom `IFormatProvider` for advanced cases. |

## `TextPattern` Placeholders

`TextPattern` accepts these placeholders, each followed by an optional .NET format specifier after a colon (`{A:MMM yyyy}`):

| Placeholder | Meaning | Available where |
|---|---|---|
| `{A}` | Argument | Argument-axis labels, tooltips, crosshair labels. |
| `{V}` | Value | Value-axis labels, tooltips, crosshair labels, point labels. |
| `{V1}`, `{V2}`, … | Specific value when the series has multiple (range, financial). | OHLC/range series labels & tooltips. |
| `{VP}` | Percentage of total | Pie / stacked. |
| `{S}` | Series name | Legend, tooltip, crosshair. |
| `{HP}` | Hint percent (within stacked series) | Stacked series labels. |
| `{B}` / `{W}` | Beginning / weight (bubble, range) | Range / Bubble labels. |
| `{N}` | Auto-numbered (1, 2, 3…) per legend item | Legend (rare). |

Format-specifier examples:

| Pattern | Renders |
|---|---|
| `{V:c0}` | `$1,234` (currency, no decimals) |
| `{V:n2}` | `1,234.56` |
| `{V:p0}` | `42 %` |
| `{V:0.00}` | `1234.56` |
| `{A:MMM yyyy}` | `Mar 2026` |
| `{A:t}` | `9:30 AM` |
| `{A:hh\\:mm\\:ss}` | `00:05:32` for `TimeSpan` |
| `{V:#,##0;(#,##0)}` | `1,234` / `(1,234)` |

```csharp
diagram.AxisX.Label.TextPattern = "{A:dd MMM}";
diagram.AxisY.Label.TextPattern = "{V:c0}";
```

## Custom Formatter

When `TextPattern` is not flexible enough (custom units like K/M/B; runtime localization; computed labels):

```csharp
public sealed class UnitsFormatter : IFormatProvider, ICustomFormatter {
    public object? GetFormat(Type? formatType) =>
        formatType == typeof(ICustomFormatter) ? this : null;

    public string Format(string? format, object? arg, IFormatProvider? p) {
        if (arg is IFormattable f && format != null) {
            double v = Convert.ToDouble(arg);
            return v switch {
                >= 1_000_000_000 => $"{v / 1e9:0.#}B",
                >= 1_000_000     => $"{v / 1e6:0.#}M",
                >= 1_000         => $"{v / 1e3:0.#}K",
                _                => v.ToString("0")
            };
        }
        return arg?.ToString() ?? string.Empty;
    }
}

diagram.AxisY.Label.TextPattern = "{V}";
diagram.AxisY.Label.Formatter   = new UnitsFormatter();
```

`Formatter` is called *after* `TextPattern` substitution.

## `CustomAxisLabel` — Callout Labels

Add labels at specific positions independent of tick marks:

```csharp
diagram.AxisY.CustomLabels.Add(new CustomAxisLabel("Target", 100) {
    TextColor = Color.DarkOrange,
    Visible   = true
});

// Strategy: keep both auto and custom labels
// LabelVisibilityMode is an Axis (Axis2D) member, NOT AxisLabel.
diagram.AxisY.LabelVisibilityMode = AxisLabelVisibilityMode.AutoGeneratedAndCustom;
```

`Axis2D.LabelVisibilityMode` is of type `AxisLabelVisibilityMode`:

| `AxisLabelVisibilityMode` | Behavior |
|---|---|
| `Default` (default) | If the axis has visible custom labels, only those show; otherwise the auto-generated tick labels show. |
| `AutoGeneratedAndCustom` | Show auto-generated and custom labels together. |

Use `CustomAxisLabel` for thresholds, milestones, named ranges.

## `CustomDrawAxisLabel` — Per-Label Owner Draw

```csharp
chart.CustomDrawAxisLabel += (s, e) => {
    if (e.Item.Axis == diagram.AxisY && Convert.ToDouble(e.Item.AxisValue) > 1000)
        e.Item.TextColor = Color.IndianRed;
};
```

`AxisLabelCustomDrawEventArgs.Item` exposes `Text`, `TextColor`, `Axis`, `AxisValue`, and `EnableAntialiasing`. Setting `Item.Text` replaces the pattern output.

## Label Resolution / Overlap

When labels would overlap, the chart applies `ResolveOverlappingOptions`:

```csharp
var ro = diagram.AxisX.Label.ResolveOverlappingOptions;
ro.AllowRotate = true;          // first try
ro.AllowStagger = true;         // then try
ro.AllowHide = true;            // last resort — hide every other label
ro.MinIndent = 6;               // minimum gap between two labels
```

If all three are `false`, labels overlap as-is.

## Common Patterns

### Pattern 1 — Currency Y axis + month X axis

```csharp
diagram.AxisX.Label.TextPattern = "{A:MMM yyyy}";
diagram.AxisY.Label.TextPattern = "{V:c0}";
diagram.AxisY.Title.Text        = "Revenue";
diagram.AxisY.Title.Visibility  = DefaultBoolean.True;
```

### Pattern 2 — Percentage on Y axis

```csharp
diagram.AxisY.Label.TextPattern = "{V:p0}";        // 42 %
diagram.AxisY.WholeRange.SetMinMaxValues(0, 1);
diagram.AxisY.NumericScaleOptions.ScaleMode   = ScaleMode.Manual;
diagram.AxisY.NumericScaleOptions.GridSpacing = 0.1;
```

### Pattern 3 — Rotate date labels to fit

```csharp
diagram.AxisX.Label.Angle = -30;
diagram.AxisX.Label.ResolveOverlappingOptions.AllowStagger = false;
```

### Pattern 4 — Threshold callout on Y axis

```csharp
diagram.AxisY.LabelVisibilityMode = AxisLabelVisibilityMode.AutoGeneratedAndCustom;
diagram.AxisY.CustomLabels.Add(new CustomAxisLabel("SLA Target", 99.9) {
    TextColor = Color.DarkOrange
});
```

### Pattern 5 — K/M/B units via formatter

```csharp
diagram.AxisY.Label.Formatter = new UnitsFormatter();
diagram.AxisY.Label.TextPattern = "{V}";
```

## Common Issues

- **Title not visible** — `Title.Visibility` defaults to `False`. Set `DefaultBoolean.True`.
- **`TextPattern` placeholder shown literally** — typo, e.g., `{V:c0` (missing closing brace) or unknown placeholder. Verify spelling.
- **Currency wrong locale** — `{V:c0}` uses `CurrentCulture`. Set `Application.CurrentCulture` or use a `Formatter` with explicit `CultureInfo`.
- **`Formatter` ignored** — `TextPattern` must contain a placeholder; if it's empty, the formatter has no value to format.
- **`CustomLabels` invisible** — set `LabelVisibilityMode = AutoGeneratedAndCustom` (or `Custom`).
- **Labels overlap** — enable `ResolveOverlappingOptions.AllowRotate` and/or `AllowStagger`. Reduce label count by aggregating data ([aggregation-and-summary.md](aggregation-and-summary.md)).
- **`CustomDrawAxisLabel` not raised** — disabled when the axis itself is hidden (`Visibility = False`) or labels are hidden.

## Source Material

- `articles/controls-and-libraries/chart-control/diagram/axis-titles.md` (`xref:WindowsForms.5475`).
- `articles/controls-and-libraries/chart-control/diagram/axis-labels.md` (`xref:WindowsForms.5479`).
- `articles/controls-and-libraries/chart-control/diagram/format-strings-in-the-chart-control.md` (`xref:WindowsForms.5450`).
- `articles/controls-and-libraries/chart-control/diagram/custom-axis-labels.md` (`xref:WindowsForms.7390`).
- `api/DevExpress.XtraCharts.AxisTitle.yml`.
- `api/DevExpress.XtraCharts.AxisLabel.yml`.
- `api/DevExpress.XtraCharts.CustomAxisLabel.yml`.
- `api/DevExpress.XtraCharts.ChartControl.CustomDrawAxisLabel.yml`.
