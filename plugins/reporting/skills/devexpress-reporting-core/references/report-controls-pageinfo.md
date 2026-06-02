# Page Info Controls

## When to Use This Reference

Use when displaying page numbers, dates, times, and user information: `XRPageInfo`.

## XRPageInfo — Page Number, Date, Time

Page number, date, time:

```csharp
var pageNum = new XRPageInfo();
pageFooter.Controls.Add(pageNum);
pageNum.PageInfo = DevExpress.XtraPrinting.PageInfo.NumberOfTotal; 
pageNum.BoundsF = new RectangleF(300, 0, 100, 18);
pageNum.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
pageNum.Format = "Page {0} of {1}";  // custom format string; {0}=current, {1}=total
```

**Main content properties**: `PageInfo` (the value type - Number, Total, NumberOfTotal, DateTime, UserName).

**Other key properties**: 
- `Format` (custom format string using `{0}` placeholder)
- `StartPageNumber` (first page offset)
- `RunningBand`
