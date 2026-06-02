# Report Bands

## When to Use This Reference

Use when you need to understand which band type to use for a layout requirement: repeating rows, grouped sections, page headers/footers, master-detail nesting.

## Band Types

| Band Class | Printed | Use For |
|------------|---------|---------|
| `TopMarginBand` | Once per page (top) | Top margin area |
| `BottomMarginBand` | Once per page (bottom) | Bottom margin area |
| `ReportHeaderBand` | Once at report start | Report title, logo |
| `ReportFooterBand` | Once at report end | Grand totals, sign-off |
| `PageHeaderBand` | Top of every page | Column headers, page title |
| `PageFooterBand` | Bottom of every page | Page numbers, date |
| `GroupHeaderBand` | Before each group | Group caption, group header row |
| `GroupFooterBand` | After each group | Group subtotals |
| `DetailBand` | Once per data record | **Mandatory** — the repeating row |
| `DetailReportBand` | Per master record | Nested (master-detail) data set |
| `SubBand` | After its parent band | Additional rows after any band |
| `VerticalHeaderBand` | Vertical layout header | Column header in vertical report |
| `VerticalDetailBand` | Vertical layout row | Repeating column in vertical report |
| `VerticalTotalBand` | Vertical layout footer | Total column in vertical report |

## Required Band

**`DetailBand` is mandatory.** A report without a `DetailBand` throws an exception at render time.

## Grouping

```csharp
var groupHeader = new GroupHeaderBand {
    GroupFields = { new GroupField("Category", XRColumnSortOrder.Ascending) }
};
var groupFooter = new GroupFooterBand();
report.Bands.Add(groupHeader);
report.Bands.Add(groupFooter);
groupHeader.HeightF = 25;
groupFooter.HeightF = 25;

// Group header label
var catLabel = new XRLabel { Font = new DevExpress.Drawing.DXFont("Arial", 10, DevExpress.Drawing.DXFontStyle.Bold) };
catLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Category]"));
groupHeader.Controls.Add(catLabel);
catLabel.BoundsF = new RectangleF(0, 0, 150, 20);

// Group footer summary
var sumLabel = new XRLabel();
groupFooter.Controls.Add(sumLabel);
sumLabel.BoundsF = new RectangleF(100, 0, 80, 20);
sumLabel.Summary = new XRSummary { Running = SummaryRunning.Group };
sumLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
```

## Page Header and Footer

```csharp
var pageHeader = new PageHeaderBand();
report.Bands.Add(pageHeader);
pageHeader.HeightF = 20;
var colHeader = new XRLabel { Text = "Product" };
pageHeader.Controls.Add(colHeader);
colHeader.BoundsF = new RectangleF(0, 0, 150, 18);

var pageFooter = new PageFooterBand();
report.Bands.Add(pageFooter);
pageFooter.HeightF = 20;
var pageNum = new XRPageInfo {
    PageInfo = PageInfo.NumberOfTotal
};
pageFooter.Controls.Add(pageNum);
pageNum.BoundsF = new RectangleF(0, 0, 80, 18);
```

## Master-Detail (Nested Data)

```csharp
var masterDetail = new DetailReportBand {
    DataSource = detailList,
    DataMember = "OrderItems"
};
report.Bands.Add(masterDetail);

var nestedDetail = new DetailBand();
masterDetail.Bands.Add(nestedDetail);
nestedDetail.HeightF = 20;
```

## VB.NET

```vb
Dim groupHeader As New GroupHeaderBand()
groupHeader.GroupFields.Add(New GroupField("Category", XRColumnSortOrder.Ascending))
report.Bands.Add(groupHeader)
groupHeader.HeightF = 25

Dim detail As New DetailBand()
report.Bands.Add(detail)
detail.HeightF = 25

Dim groupFooter As New GroupFooterBand()
report.Bands.Add(groupFooter)
groupFooter.HeightF = 25
```

## Band Print Order

For a data-bound report with grouping, bands print in this order per page:
1. `TopMarginBand`
2. `ReportHeaderBand` (first page only)
3. `PageHeaderBand`
4. For each group: `GroupHeaderBand` → `DetailBand` (×N) → `GroupFooterBand`
5. `PageFooterBand`
6. `ReportFooterBand` (last page only)
7. `BottomMarginBand`
