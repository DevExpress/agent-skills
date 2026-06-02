# XRCrossTab

## What Is XRCrossTab

`XRCrossTab` is a report control that renders a **pivot matrix**: it groups rows and columns by field values and aggregates data at every intersection. Unlike a table with a fixed number of rows, a crosstab generates its rows, columns, and cells dynamically from its own data source.

### Anatomy of a Crosstab


```text
                    Country A  Country A  A Total    Country B   B Total   Grand
                    Person 1   Person 2              Person 3              Total
┌─────────────────┬──────────┬──────────┬──────────┬──────────┬──────────┬──────────┐
│ Year  Category  │    12    │    8     │    20    │    15    │    5     │    40    │
├─────────────────┼──────────┼──────────┼──────────┼──────────┼──────────┼──────────┤
│ 1998  Beverages │   $720   │  $480    │ $1,200   │  $900    │  $300    │ $2,400   │
│       Condiments│   $360   │  $240    │  $600    │  $540    │  $120    │ $1,260   │
├─────────────────┼──────────┼──────────┼──────────┼──────────┼──────────┼──────────┤
│ 1998  Tot       │ $1,080   │  $720    │ $1,800   │ $1,440   │  $420    │ $3,660   │
├─────────────────┼──────────┼──────────┼──────────┼──────────┼──────────┼──────────┤
│ 1999  Beverages │    ...   │   ...    │   ...    │   ...    │   ...    │   ...    │
├─────────────────┼──────────┼──────────┼──────────┼──────────┼──────────┼──────────┤
│ Grand Total     │   ...    │   ...    │   ...    │   ...    │   ...    │   ...    │
└─────────────────┴──────────┴──────────┴──────────┴──────────┴──────────┴──────────┘

Row axis:    [Year] → [Category] → [data label]
Column axis: [Country] → [Person/Salesperson]```

**Core building blocks:**

| Concept | Class | Purpose |
|---------|-------|---------|
| **Row field** | `CrossTabRowField` | Groups data into rows (vertical axis). One field per hierarchy level. |
| **Column field** | `CrossTabColumnField` | Groups data into columns (horizontal axis). One field per hierarchy level. |
| **Data field** | `CrossTabDataField` | Defines the metric to aggregate at each row × column intersection. |
| **Row definition** | `CrossTabRowDefinition` | Controls height and visibility of a generated row. |
| **Column definition** | `CrossTabColumnDefinition` | Controls width and visibility of a generated column. |
| **Cell** | `XRCrossTabCell` | A single cell in the matrix. Identified by `RowIndex` + `ColumnIndex`. |

**`GenerateLayout()` vs. manual cells:**
- **Simple pattern** (no column fields, single data field): define `ColumnDefinitions`, `RowDefinitions`, and `Cells` by hand. Every cell is explicit.
- **Advanced pattern** (column fields or multiple data fields): call `GenerateLayout()` after adding all fields. The control creates all definitions and cells automatically. Do not add cells manually — customize them via the `crossTab.Cells` collection after generation.

---

## Pattern Selection

| Requirement | Simple | Advanced |
|-------------|:------:|:--------:|
| Single row grouping (e.g., by Salesperson) | ✓ | ✓ |
| Multiple row hierarchy levels (Year → Category) | ✗ | ✓ |
| Column fields (data-driven column axis) | ✗ | ✓ |
| Multiple data metrics (Quantity + Price) | ✗ | ✓ |
| Parameter-driven filtering | ✗ | ✓ |
| Manual cell definitions | ✓ | ✗ |

---

## Simple Pattern — Manual Cells

**When to use**: one row-grouping field, one data metric, fixed grid size.
**Output**: Salesperson × ExtendedPrice summary, sorted by sales descending.

```
┌──────────────────┬────────────────────┐
│ Salesperson      │ Sales, $           │   ← Row 0: header
├──────────────────┼────────────────────┤
│ Top Salesperson  │ $1,234.50          │   ← Row 1: data (repeats per unique Salesperson)
│ Second Person    │ $1,100.25          │
├──────────────────┼────────────────────┤
│ Grand Total      │ $15,678.90         │   ← Row 2: total
└──────────────────┴────────────────────┘
	 Column 0            Column 1
```

```csharp
private void CreateSimpleCrosstab(SqlDataSource dataSource, string dataMember) {
	var crossTab = new XRCrossTab();

	// Attach to the Detail band (must exist on the report)
	this.Detail.Controls.Add(crossTab);

	// Position and size — use full available page width
	float availableWidth = this.PageWidth - this.Margins.Left - this.Margins.Right;
	crossTab.LocationF = new PointF(0F, 0F);
	crossTab.SizeF = new SizeF(availableWidth, 75F);

	// Data binding
	crossTab.DataSource = dataSource;
	crossTab.DataMember = dataMember;

	// Column widths: 1/3 for row labels, 2/3 for values
	float col0 = availableWidth / 3F;
	crossTab.ColumnDefinitions.AddRange(new CrossTabColumnDefinition[] {
		new CrossTabColumnDefinition(col0),
		new CrossTabColumnDefinition(availableWidth - col0)
	});

	// Row heights: header / data / total — must match the number of cell rows defined below
	crossTab.RowDefinitions.AddRange(new CrossTabRowDefinition[] {
		new CrossTabRowDefinition(25F),   // Row 0 — header
		new CrossTabRowDefinition(25F),   // Row 1 — data (one per unique Salesperson value)
		new CrossTabRowDefinition(25F)    // Row 2 — grand total
	});

	// Row field: group data by Salesperson, sort by aggregated ExtendedPrice descending
	var rowField = new CrossTabRowField { FieldName = "Salesperson" };
	rowField.SortBySummaryInfo.FieldName = "ExtendedPrice";
	rowField.SortOrder = XRColumnSortOrder.Descending;
	crossTab.RowFields.AddRange(new CrossTabRowField[] { rowField });

	// Data field: aggregate ExtendedPrice — SummaryType.Sum is the default
	var dataField = new CrossTabDataField { FieldName = "ExtendedPrice" };
	crossTab.DataFields.AddRange(new CrossTabDataField[] { dataField });

	// Cells — all six must be created explicitly; order in AddRange does not matter
	// Row 0: column header labels
	var rowHeaderCell  = new XRCrossTabCell { RowIndex = 0, ColumnIndex = 0, Text = "Salesperson" };
	var colHeaderCell  = new XRCrossTabCell { RowIndex = 0, ColumnIndex = 1, Text = "Sales, $" };
	// Row 1: data cells — Text is auto-populated from field values at runtime
	var rowFieldCell   = new XRCrossTabCell { RowIndex = 1, ColumnIndex = 0 };
	var dataFieldCell  = new XRCrossTabCell { RowIndex = 1, ColumnIndex = 1, TextFormatString = "{0:C2}" };
	// Row 2: grand total
	var totalLabelCell = new XRCrossTabCell { RowIndex = 2, ColumnIndex = 0, Text = "Grand Total" };
	var totalValueCell = new XRCrossTabCell { RowIndex = 2, ColumnIndex = 1, TextFormatString = "{0:C2}" };

	crossTab.Cells.AddRange(new XRControl[] {
		rowHeaderCell, colHeaderCell, rowFieldCell, dataFieldCell, totalLabelCell, totalValueCell
	});

	// Styles — define all four zones for consistent output
	crossTab.CrossTabStyles.GeneralStyle = new XRControlStyle {
		Name = "crossTabGeneralStyle",
		BackColor = Color.White, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F), ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.HeaderAreaStyle = new XRControlStyle {
		Name = "crossTabHeaderStyle",
		BackColor = Color.LightGray, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold),
		ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.DataAreaStyle = new XRControlStyle {
		Name = "crossTabDataStyle",
		BackColor = Color.White, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F), ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.TotalAreaStyle = new XRControlStyle {
		Name = "crossTabTotalStyle",
		BackColor = Color.LightYellow, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold),
		ForeColor = Color.Black
	};
}
```

**Checklist**
- [ ] `DataSource` and `DataMember` set
- [ ] `LocationF` and `SizeF` set
- [ ] CrossTab added to a report band
- [ ] `ColumnDefinitions` count and `RowDefinitions` count match the cell grid exactly
- [ ] `RowField(s)` configured — `SortBySummaryInfo` and `SortOrder` if needed
- [ ] `DataField(s)` with correct `SummaryType`
- [ ] Every cell created — `RowIndex` × `ColumnIndex` must cover the full grid with no gaps
- [ ] `TextFormatString` on data and total value cells
- [ ] All four `CrossTabStyles` zones defined

---

## Advanced Pattern — GenerateLayout()

**When to use**: column fields present, multiple data fields, or hierarchical row/column grouping.

**Example Output**: Year × Category rows vs. Country × Salesperson columns, showing avg Quantity and sum ExtendedPrice, filtered by a multi-value report parameter.

```
┌──────┬─────────────┬───────┬──────────┬──────────┬───────────┬──────────┬─────────┬───────────┐
│      │             │       │ USA                 │           │ UK                 │           │
│      │             │       ├──────────┬──────────┤USA Total  ├──────────┬─────────┤UK Total   │
│ Year │ Category    │       │ Person1  │ Person2  │           │ Person3  │ Person4 │           │
├──────┼─────────────┼───────┼──────────┼──────────┼───────────┼──────────┼─────────┼───────────┤
│ 1998 │ Beverages   │ Qty   │ 12       │ 8        │ 20        │ 15       │ 5       │ 20        │
│      │             │ Price │ $720.00  │ $480.00  │$1,200.00  │ $900.00  │ $300.00 │$1,200.00  │
│      ├─────────────┼───────┼──────────┼──────────┼───────────┼──────────┼─────────┼───────────┤
│      │ Condiments  │ Qty   │ 6        │ 4        │ 10        │ 9        │ 2       │ 11        │
│      │             │ Price │ $360.00  │ $240.00  │ $600.00   │ $540.00  │ $120.00 │ $660.00   │
│      ├─────────────┼───────┴──────────┴──────────┴───────────┴──────────┴─────────┴───────────┤
│      │ 1998 Total  │ Qty/Price aggregated across all countries                                  │
└──────┴─────────────┴────────────────────────────────────────────────────────────────────────────┘
Row axis:    [Year (DateYear grouped)] → [CategoryName (sorted by price desc)] → [data field label]
Column axis: [Country] → [Salesperson] → [Country Total]
DataFieldLayout = InColumn: each metric occupies its own row, not its own column
```

```csharp
private void CreateAdvancedCrosstab(SqlDataSource dataSource, string dataMember) {
	var crossTab = new XRCrossTab();

	// Attach to the Detail band (must exist on the report)
	this.Detail.Controls.Add(crossTab);

	// Position and size
	crossTab.LocationF = new PointF(0F, 0F);
	crossTab.SizeF = new SizeF(650F, 75F);

	// Print options — MUST be set before GenerateLayout()
	crossTab.PrintOptions.RepeatColumnHeaders = true;
	crossTab.PrintOptions.RepeatRowHeaders = true;
	crossTab.PrintOptions.PrintLayout = PrintLayout.AcrossThenDown;

	// Data binding
	crossTab.DataSource = dataSource;
	crossTab.DataMember = dataMember;

	// Parameter binding: bind a report-level parameter to a named crosstab parameter slot.
	// "crossTabParameter" must match the ?placeholder used in FilterString below.
	// this.Parameters["Salesperson"] is a MultiValue parameter defined on the report.
	crossTab.Parameters.Add(
		new XRControlParameter("crossTabParameter", this.Parameters["Salesperson"])
	);

	// Filter applied to the crosstab's data before aggregation.
	// ?crossTabParameter resolves via the XRControlParameter added above.
	crossTab.FilterString =
		"[Salesperson] In (?crossTabParameter) And [OrderDate] > #1998-01-01# And [Quantity] > 0s";

	// Row fields — first entry = outermost (root) level, last entry = innermost level
	var yearRowField = new CrossTabRowField {
		FieldName     = "OrderDate",
		GroupInterval = GroupInterval.DateYear   // bucket DateTime values by year
	};
	var categoryRowField = new CrossTabRowField {
		FieldName = "CategoryName",
		SortOrder = XRColumnSortOrder.Descending
	};
	categoryRowField.SortBySummaryInfo.FieldName = "ExtendedPrice"; // sort by aggregated metric
	crossTab.RowFields.AddRange(new CrossTabRowField[] { yearRowField, categoryRowField });

	// Column fields — first entry = outermost level, last entry = innermost level
	crossTab.ColumnFields.AddRange(new CrossTabColumnField[] {
		new CrossTabColumnField { FieldName = "Country" },
		new CrossTabColumnField { FieldName = "Salesperson" }
	});

	// Data fields — each gets its own label row because DataFieldLayout = InColumn
	crossTab.DataFields.AddRange(new CrossTabDataField[] {
		new CrossTabDataField { FieldName = "Quantity",      SummaryType = SummaryType.Average },
		new CrossTabDataField { FieldName = "ExtendedPrice", SummaryType = SummaryType.Sum }
	});

	// Stack data fields vertically: each metric occupies its own row (not its own column)
	crossTab.LayoutOptions.DataFieldLayout = DataFieldLayout.InColumn;

	// GenerateLayout() — MUST be called after all fields are added
	// Creates all RowDefinitions, ColumnDefinitions, and Cells automatically
	crossTab.GenerateLayout();

	// ── Post-generation: visibility ──────────────────────────────────────────────
	// Hide the grand total row and grand total column using Count-1 (dynamic — avoids
	// hardcoded indices that break when field count or hierarchy depth changes).
	crossTab.RowDefinitions[crossTab.RowDefinitions.Count - 1].Visible = false;
	crossTab.ColumnDefinitions[crossTab.ColumnDefinitions.Count - 1].Visible = false;

	// ── Post-generation: auto-width ──────────────────────────────────────────────
	// Only apply to visible columns. Use fully-qualified AutoSizeMode to avoid CS0104
	// ambiguity between DevExpress.XtraReports.UI.AutoSizeMode and System.Windows.Forms.AutoSizeMode.
	foreach (var col in crossTab.ColumnDefinitions) {
		if (col.Visible)
			col.AutoWidthMode = DevExpress.XtraReports.UI.AutoSizeMode.GrowOnly;
	}

	// ── Post-generation: cell formatting ─────────────────────────────────────────
	// Iterate crossTab.Cells after GenerateLayout(); identify cells by DataLevel:
	//   DataLevel ==  0  → first data field  (Quantity / Average)
	//   DataLevel ==  1  → second data field (ExtendedPrice / Sum)
	//   DataLevel == -1  → header or total cell (not a data value cell)
	foreach (XRCrossTabCell cell in crossTab.Cells) {
		if (cell.DataLevel == 0) {
			cell.TextFormatString = "{0:N0}";    // integer average: 12
			cell.NullValueText    = "-";
			// Optional: alternating row background via expression binding
			cell.ExpressionBindings.Add(new ExpressionBinding(
				"BeforePrint", "BackColor",
				"Iif(([Arguments.GroupRowIndex] % 2 == 0), 'AliceBlue', ?)"));
		} else if (cell.DataLevel == 1) {
			cell.TextFormatString = "{0:C2}";    // currency: $1,234.50
			cell.NullValueText    = "-";
		}
	}

	// ── Styles ───────────────────────────────────────────────────────────────────
	crossTab.CrossTabStyles.GeneralStyle = new XRControlStyle {
		Name = "crossTabGeneralStyle",
		BackColor = Color.White, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F), ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.HeaderAreaStyle = new XRControlStyle {
		Name = "crossTabHeaderStyle",
		BackColor = Color.LightGray, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold),
		ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.DataAreaStyle = new XRControlStyle {
		Name = "crossTabDataStyle",
		BackColor = Color.White, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F), ForeColor = Color.Black
	};
	crossTab.CrossTabStyles.TotalAreaStyle = new XRControlStyle {
		Name = "crossTabTotalStyle",
		BackColor = Color.LightYellow, BorderColor = Color.Gray, Borders = BorderSide.All,
		Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold),
		ForeColor = Color.Black
	};
}
```

**Required report-level setup** (in the report constructor, before calling this method):

```csharp
// Prevents wide/tall matrices from being clipped at page boundaries
this.VerticalContentSplitting   = VerticalContentSplitting.Smart;
this.HorizontalContentSplitting = HorizontalContentSplitting.Smart;
```

**Checklist**
- [ ] Report `VerticalContentSplitting` and `HorizontalContentSplitting` set to `Smart`
- [ ] `DataSource` and `DataMember` set
- [ ] CrossTab added to a report band
- [ ] `LocationF` and `SizeF` set
- [ ] Print options set **before** `GenerateLayout()`
- [ ] If neccessary, parameters bound via `crossTab.Parameters.Add(new XRControlParameter("name", this.Parameters["Name"]))` and are used in `FilterString`
- [ ] Row fields added outermost-first with `GroupInterval` / `SortBySummaryInfo` as needed
- [ ] Column fields added outermost-first
- [ ] Data fields with correct `SummaryType` per metric
- [ ] `DataFieldLayout` set before `GenerateLayout()`
- [ ] `GenerateLayout()` called **after** all fields and options, **before** any post-processing
- [ ] `RowDefinitions` / `ColumnDefinitions` visibility set using `Count - 1` (not hardcoded indices)
- [ ] `AutoWidthMode` set only on `col.Visible == true` columns using `DevExpress.XtraReports.UI.AutoSizeMode`
- [ ] `TextFormatString` and `NullValueText` applied per `DataLevel` in post-generation cell loop
- [ ] All four `CrossTabStyles` zones defined

---

## API Reference

### CrossTabRowField

```csharp
var field = new CrossTabRowField {
	FieldName     = "CategoryName",
	GroupInterval = GroupInterval.Default,        // see GroupInterval table below
	SortOrder     = XRColumnSortOrder.Ascending   // Ascending | Descending
};
// Sort rows by an aggregated metric (e.g., highest-selling category first):
field.SortBySummaryInfo.FieldName = "ExtendedPrice";
field.SortBySummaryInfo.SortOrder = XRColumnSortOrder.Descending;
```

### CrossTabColumnField

```csharp
new CrossTabColumnField {
	FieldName     = "Country",
	GroupInterval = GroupInterval.Default
}
```

### CrossTabDataField

```csharp
new CrossTabDataField {
	FieldName   = "ExtendedPrice",
	SummaryType = SummaryType.Sum    // see SummaryType table below
}
```

### XRControlParameter — parameter binding

```csharp
// "paramSlotName" must match the ?placeholder in FilterString exactly
// this.Parameters["ReportParamName"] is the report-level XtraReport parameter
crossTab.Parameters.Add(
	new XRControlParameter("paramSlotName", this.Parameters["ReportParamName"])
);
crossTab.FilterString = "[Field] In (?paramSlotName)";
```

Report parameters that accept multiple values must have `MultiValue = true`.

### GenerateLayout()

```csharp
// Call after all RowFields, ColumnFields, DataFields, LayoutOptions are configured:
crossTab.GenerateLayout();
// After this call, the following collections are populated and ready for post-processing:
//   crossTab.RowDefinitions     — one entry per generated row
//   crossTab.ColumnDefinitions  — one entry per generated column
//   crossTab.Cells              — all cells, iterable for formatting
```

### RowDefinitions / ColumnDefinitions — post-generation access

```csharp
// Hide definitions using Count-1 (dynamic — safe when field count may vary):
crossTab.RowDefinitions[crossTab.RowDefinitions.Count - 1].Visible = false;
crossTab.ColumnDefinitions[crossTab.ColumnDefinitions.Count - 1].Visible = false;

// Auto-width — only on visible columns; fully-qualify to avoid CS0104:
foreach (var col in crossTab.ColumnDefinitions)
	if (col.Visible)
		col.AutoWidthMode = DevExpress.XtraReports.UI.AutoSizeMode.GrowOnly;
```

### CrossTabStyles — four independent zones

```csharp
crossTab.CrossTabStyles.GeneralStyle    = new XRControlStyle { ... }; // baseline for all cells
crossTab.CrossTabStyles.HeaderAreaStyle = new XRControlStyle { ... }; // row and column headers
crossTab.CrossTabStyles.DataAreaStyle   = new XRControlStyle { ... }; // data value cells
crossTab.CrossTabStyles.TotalAreaStyle  = new XRControlStyle { ... }; // subtotal and grand total cells
```

### XRControlStyle properties

```csharp
new XRControlStyle {
	Name        = "uniqueName",           // required — must be unique per report
	BackColor   = Color.White,
	ForeColor   = Color.Black,
	BorderColor = Color.Gray,
	Borders     = BorderSide.All,
	Font        = new DevExpress.Drawing.DXFont("Arial", 9.75F),
	// Font with style:
	Font        = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold),
	TextAlignment = TextAlignment.TopRight,
	Padding     = new PaddingInfo { All = 2 }
}
```

### ExpressionBinding — conditional cell appearance (advanced only)

```csharp
// Apply after GenerateLayout(); Arguments.GroupRowIndex gives the 0-based data row index
cell.ExpressionBindings.Add(new ExpressionBinding(
	"BeforePrint",    // event name
	"BackColor",      // property to set
	"Iif(([Arguments.GroupRowIndex] % 2 == 0), 'AliceBlue', ?)"  // ? = inherit from style
));
```

---

## GroupInterval

Use on `CrossTabRowField.GroupInterval` or `CrossTabColumnField.GroupInterval` to bucket continuous values.

| Value | Type | Buckets |
|-------|------|---------|
| `Default` | Any | Unique field values (no bucketing) |
| `Date` | DateTime | Date part only, time ignored |
| `DateDay` | DateTime | Day of month (1–31) |
| `DateDayOfWeek` | DateTime | Day name (Monday, Tuesday…) |
| `DateDayOfYear` | DateTime | Day of year (1–366) |
| `DateWeekOfMonth` | DateTime | Week in month (1–6) |
| `DateWeekOfYear` | DateTime | Week in year (1–53) |
| `DateMonth` | DateTime | Month name (January–December) |
| `DateQuarter` | DateTime | Quarter (1–4) |
| `DateYear` | DateTime | Year value (2022, 2023…) |
| `YearAge` | DateTime | Full years elapsed; range via `GroupIntervalNumericRange` |
| `MonthAge` | DateTime | Full months elapsed |
| `WeekAge` | DateTime | Full weeks elapsed |
| `DayAge` | DateTime | Full days elapsed |
| `Alphabetical` | String | First character of value |
| `Numeric` | Numeric | Ranges defined by `GroupIntervalNumericRange` |
| `Hour` | DateTime | Hour part (0–23) |
| `Minute` | DateTime | Minute part (0–59) |
| `Second` | DateTime | Second part (0–59) |

---

## SummaryType

Use on `CrossTabDataField.SummaryType`.

| Value | Description |
|-------|-------------|
| `Sum` | Sum of all values **(default)** |
| `Count` | Number of records |
| `Average` | Mean of all values |
| `Min` | Smallest value |
| `Max` | Largest value |
| `StdDev` | Sample standard deviation |
| `StdDevp` | Population standard deviation |
| `Var` | Sample variance |
| `Varp` | Population variance |
| `CountDistinct` | Number of unique values |
| `Median` | Middle value in ordered list |
| `Mode` | Most frequently appearing value |

---

## LayoutOptions

| Property | Values | Effect |
|----------|--------|--------|
| `DataFieldLayout` | `InColumn` | Each data field occupies its own **row**; label appears in the row header area. Use when showing multiple metrics per data row. |
| `DataFieldLayout` | `InRow` | Each data field occupies its own **column**; label appears in the column header area. |
| `CornerHeaderDisplayMode` | `RowFieldNames` / `ColumnFieldNames` / `None` | Content of the top-left corner cell |
| `ColumnTotalsPosition` | `AfterData` / `BeforeData` | Placement of column subtotals |
| `RowTotalsPosition` | `AfterData` / `BeforeData` | Placement of row subtotals |
| `HierarchicalRowLayout` | `true` / `false` | Tree-style vs. flat row headers |

---

## PrintOptions

| Property | Effect |
|----------|--------|
| `RepeatColumnHeaders = true` | Reprints column headers on every horizontal page break — **set before `GenerateLayout()`** |
| `RepeatRowHeaders = true` | Reprints row headers on every vertical page break — **set before `GenerateLayout()`** |
| `PrintLayout = PrintLayout.AcrossThenDown` | Continues wide content in the next band row rather than starting a new page |

---

## AutoWidthMode

Use `DevExpress.XtraReports.UI.AutoSizeMode` (fully qualified to avoid CS0104 with `System.Windows.Forms.AutoSizeMode`).

| Value | Behavior |
|-------|----------|
| `GrowOnly` | Expands to fit content; never narrower than defined width |
| `ShrinkOnly` | Shrinks to fit content; never wider than defined width |
| `GrowAndShrink` | Fully auto-sized to content |
| `None` | Fixed width **(default)** |

---

## Key Rules

| Rule | Detail |
|------|--------|
| **`GenerateLayout()` timing** | Call **after** all fields and `LayoutOptions`; **before** accessing `RowDefinitions`, `ColumnDefinitions`, or `Cells`. |
| **Print options timing** | Set `RepeatColumnHeaders` / `RepeatRowHeaders` **before** `GenerateLayout()` — they have no effect if set after. |
| **Dynamic index for visibility** | Use `Count - 1` to reference the last definition. Hardcoded indices break when field count or hierarchy depth changes. |
| **Fully-qualified AutoSizeMode** | Always write `DevExpress.XtraReports.UI.AutoSizeMode.GrowOnly` to prevent CS0104. |
| **DataLevel is 0-based** | `0` = first data field, `1` = second data field, `-1` = header or total cell. |
| **Field order = hierarchy depth** | First entry in `RowFields` / `ColumnFields` is the root (outermost) grouping level. |
| **SortBySummaryInfo** | Sorts by the **aggregated** metric value, not the raw field value. |
| **Responsive width** | `float w = this.PageWidth - this.Margins.Left - this.Margins.Right;` |
| **Parameter placeholder match** | The `?name` in `FilterString` must exactly match the first argument of `XRControlParameter`. |
| **Simple pattern: no GenerateLayout()** | Define every cell manually; calling `GenerateLayout()` will override them. |

---

## Common Issues & Solutions

| Symptom | Cause | Fix |
|---------|-------|-----|
| Data cells show blank instead of `-` | `NullValueText` not set | `cell.NullValueText = "-"` in the post-generation cell loop |
| Wrong cells receive formatting | `DataLevel` index is off | `0` = first field, `1` = second; verify order in `DataFields.AddRange` |
| CS0104 ambiguity on `AutoSizeMode` | Two assemblies expose the same name | Fully qualify: `DevExpress.XtraReports.UI.AutoSizeMode` |
| Filter has no effect | `?name` mismatch in `FilterString` vs `XRControlParameter` | First arg of `XRControlParameter` must exactly match the `?name` |
