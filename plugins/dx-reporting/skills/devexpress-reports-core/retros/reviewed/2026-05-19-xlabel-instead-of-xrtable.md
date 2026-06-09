# Retro — XRLabel used instead of XRTable for tabular data

## Mistake 3

### Task context
The user asked to display product name and price in a detail band in a tabular layout.

### What the skill said (or didn't say)
The skill's Core Classes table lists `XRTable / XRTableRow / XRTableCell` as the correct control for tabular layout. However, the *Quick Start* code example in the skill uses `XRLabel` for field display without any note warning against using multiple side-by-side labels to simulate a table.

### What you did wrong
I placed two `XRLabel` controls side-by-side inside the `DetailBand` to show `Name` and `Price`, simulating a table with absolute coordinates instead of using `XRTable / XRTableRow / XRTableCell`.

### Why you made the mistake
The skill's *Quick Start* example uses `XRLabel` and the skill is **silent** on when `XRTable` must be preferred over multiple `XRLabel` controls. The constraint is absent — there is no rule or warning saying "do not simulate table structure with adjacent labels".

### What the correct behavior should have been
Any time two or more data-bound fields are laid out side-by-side in the same band to form columns, use `XRTable` with `XRTableRow` and `XRTableCell` instead of multiple `XRLabel` controls with absolute positions.

### Proposed skill fix
**New rule** — add directly below the Core Classes table:

> **CRITICAL — Tabular layout**: When displaying two or more fields side-by-side as columns inside a band, **always use `XRTable` / `XRTableRow` / `XRTableCell`**. Never simulate a table by placing multiple `XRLabel` controls at absolute X positions. Side-by-side labels produce misaligned columns, poor border rendering, and break the report designer's table editing features.

**New example** :

```csharp
    XtraReport report = new XtraReport();
    report.DataSource = sqlDataSource;
    report.DataMember = "selectQuery";

    // Creates a detail band and adds it to the report.
    DetailBand detailBand = new DetailBand();
    report.Bands.Add(detailBand);

    // Creates a table and adds it to the detail band.
    XRTable table = new XRTable();
    detailBand.Controls.Add(table);

    // Creates a row and adds the product name and product price cells to the row.
    table.BeginInit();

    XRTableRow row = new XRTableRow();
    table.Rows.Add(row);

    XRTableCell productName = new XRTableCell();
    XRTableCell productPrice = new XRTableCell();

    row.Cells.Add(productName);
    row.Cells.Add(productPrice);

    // Binds the table cells to the data fields.
    productName.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
    productPrice.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"));

    // Adjust the table width.
    table.BeforePrint += Table_BeforePrint;

    table.EndInit();
```
