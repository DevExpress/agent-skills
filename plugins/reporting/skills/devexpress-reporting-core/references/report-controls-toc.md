# Table of Contents Controls

## When to Use This Reference

Use when auto-generating a table of contents from report bookmarks: `XRTableOfContents`.

## XRTableOfContents — Auto-Generated TOC

Auto-generated TOC built from the report's bookmark hierarchy:

```csharp
// Assign Bookmark property on controls/bands to build the hierarchy.
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Bookmark", "[CategoryName]"));
// The TOC can only be placed into the ReportHeaderBand
var reportHeaderBand = new ReportHeaderBand();
report.Bands.Add(reportHeaderBand);
var toc = new XRTableOfContents();
reportHeaderBand.Controls.Add(toc);
toc.LevelTitle.Text = "Contents";
toc.LevelTitle.Font = new DXFont("Arial", 14f, DXFontStyle.Bold);
reportHeaderBand.Controls.Add(toc);
```

**Key properties**: 
- `LevelTitle` (`XRTableOfContentsTitle`) — title text and font
- `Levels` (`XRTableOfContentsLevelCollection`) — per-depth formatting; each `XRTableOfContentsLevel` has `Indent`, `Font`, `ForeColor`, `LeaderSymbol`

**Limitations**:
- Always occupies the entire page. `PageHeaderBand` and `PageFooterBand` are **not** printed on TOC pages. Use `TopMarginBand`/`BottomMarginBand` to add page-level headers/footers to TOC pages.
