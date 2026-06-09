# Mail Merge — DevExpress Word Processing Document API

Mail merge generates personalized documents or reports by combining a template with data from a data source. The Word Processing Document API supports plain (flat) and master-detail (hierarchical) merge, and can output directly to DOCX, PDF, HTML, or a stream.

## When to Use This Reference

Use this when you need to:
- Generate personalized letters, invoices, or contracts from a template
- Build tabular reports from a `DataTable`, `DataSet`, or object collection
- Create master-detail reports (e.g., orders with line items)
- Insert images from a database into merged documents
- Insert dynamically computed content (formatted text, tables) via DOCVARIABLE fields
- Control the merge process through events

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `RichEditDocumentServer` | Hosts the template and executes the merge |
| `Document.MailMergeDataSource` | Property to set the flat data source |
| `MailMergeOptions` | Configures the merge operation (output format, range, region tags) |
| `Field` | Represents a document field (MERGEFIELD, DOCVARIABLE, etc.) |
| `FieldCollection.Create()` | Programmatically inserts fields into a template |
| `MailMergeRegionInfo` | Describes the master-detail region hierarchy |
| `CalculateDocumentVariableEventArgs` | Event args for dynamically computing field values |

## Step 1: Load a Template

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using var server = new RichEditDocumentServer();

// Load a pre-built template from disk
server.LoadDocument("template.docx", DocumentFormat.Docx);

// Or load as a template (prevents accidental overwrite on save)
// server.LoadDocumentTemplate("template.docx");
```

## Step 2: Add Fields to a Template Programmatically

```csharp
Document doc = server.Document;

// Insert MERGEFIELD at the start of the first paragraph
doc.Fields.Create(doc.Paragraphs[0].Range.Start, "MERGEFIELD FirstName");
doc.Fields.Create(doc.Paragraphs[0].Range.End, " ");
doc.Fields.Create(doc.Paragraphs[0].Range.End, "MERGEFIELD LastName");

// Insert a date field with formatting
doc.Fields.Create(doc.Paragraphs[1].Range.Start, @"DATE \@ ""MMMM d, yyyy""");
```

## Step 3: Set the Data Source

### Flat Data Source (DataTable)

```csharp
using System.Data;

DataTable customers = new DataTable("Customers");
customers.Columns.Add("FirstName");
customers.Columns.Add("LastName");
customers.Columns.Add("Email");
customers.Rows.Add("Alice", "Smith", "alice@example.com");
customers.Rows.Add("Bob", "Jones", "bob@example.com");

// Assign data source
server.Document.MailMergeDataSource = customers;
```

### Object Collection

```csharp
List<Customer> customers = GetCustomers(); // IEnumerable<T>
server.Document.MailMergeDataSource = customers;
```

### DataSet (for Master-Detail)

For master-detail reports, pass the `DataSet` directly as the data source. The template region names must match the `DataTable` names in the `DataSet`.

```csharp
DataSet orderData = LoadOrderData(); // Contains "Orders" and "OrderDetails" tables
// Add relation: orderData.Relations.Add("Orders_OrderDetails", ...)
server.Document.MailMergeDataSource = orderData;
```

## Step 4: Define Master-Detail Regions

Master-detail regions use special MERGEFIELD fields as markers. Region start/end fields must be inside a table row.

Template structure:
```
TableStart:Orders       <- MERGEFIELD in cell (marks start of Orders region)
{OrderDate} {Customer}  <- Regular MERGEFIELDs
  TableStart:OrderDetails   <- nested region start
  {ProductName} {Qty} {Price}
  TableEnd:OrderDetails     <- nested region end
TableEnd:Orders         <- MERGEFIELD in cell (marks end of Orders region)
```

Insert region markers in code:

```csharp
Table orderTable = doc.Tables[0]; // The table that holds per-order data

// Insert TableStart marker in the first cell of the first data row
doc.Fields.Create(orderTable.Rows[1].Cells[0].Range.Start, "MERGEFIELD TableStart:Orders");

// Insert TableEnd marker in the last cell of the last data row
doc.Fields.Create(orderTable.Rows[1].Cells[3].Range.End, "MERGEFIELD TableEnd:Orders");
```

Inspect the region hierarchy:

```csharp
var regionInfo = doc.GetRegionHierarchy();
Console.WriteLine($"Regions found: {regionInfo.Regions.Count}");
```

## Step 5: Execute the Merge

### Merge to a New File

```csharp
MailMergeOptions mergeOptions = server.CreateMailMergeOptions();
mergeOptions.DataSource = customers; // DataTable or collection
mergeOptions.MergeMode = MergeMode.NewSection; // One section per record

// Merge and save
server.MailMerge(mergeOptions, "merged_output.docx", DocumentFormat.Docx);
```

### Merge to a Stream

```csharp
using (FileStream outputStream = new FileStream("output.docx", FileMode.Create))
{
    server.MailMerge(mergeOptions, outputStream, DocumentFormat.Docx);
}
```

### Merge to PDF

```csharp
server.MailMerge(mergeOptions, "output.pdf", DocumentFormat.Pdf);
// Or export via ExportToPdf after merging to a new server instance
```

### Merge a Single Record

```csharp
mergeOptions.FirstRecordIndex = 2; // Zero-based
mergeOptions.LastRecordIndex = 2;
server.MailMerge(mergeOptions, "single_record.docx", DocumentFormat.Docx);
```

## Insert Images from a Database

Use a `DOCVARIABLE` field and handle the `CalculateDocumentVariable` event:

```csharp
// In the template: { DOCVARIABLE Photo "EmployeeId" }

server.CalculateDocumentVariable += (sender, e) =>
{
    if (e.VariableName == "Photo")
    {
        string employeeId = e.Arguments[0].Value.ToString();
        byte[] imageBytes = GetImageFromDatabase(employeeId);

        var imageServer = new RichEditDocumentServer();
        Shape image = imageServer.Document.Shapes.InsertPicture(
            imageServer.Document.Range.Start,
            DocumentImageSource.FromStream(new MemoryStream(imageBytes))
        );
        image.TextWrapping = TextWrappingType.InLineWithText;

        e.Value = imageServer;
        e.Handled = true;
    }
};
```

## Insert Dynamic Formatted Content

`DOCVARIABLE` can insert an entire `RichEditDocumentServer` as content (paragraphs, tables, formatted runs):

```csharp
server.CalculateDocumentVariable += (sender, e) =>
{
    if (e.VariableName == "DynamicContent")
    {
        var contentServer = new RichEditDocumentServer();
        contentServer.Document.AppendText("Computed at: " + DateTime.Now);
        // Apply formatting ...
        e.Value = contentServer;
        e.Handled = true;
    }
};
```

## Custom Region Tags

If the template uses custom start/end tag names other than `TableStart`/`TableEnd`:

```csharp
MailMergeOptions opts = server.CreateMailMergeOptions();
opts.RegionStartTag = "Begin";
opts.RegionEndTag = "End";
// Now the template uses MERGEFIELD Begin:RegionName and MERGEFIELD End:RegionName
```

## Supported Field Types for Mail Merge

| Field | Description |
|-------|-------------|
| `MERGEFIELD FieldName` | Plain text replacement from data source column |
| `MERGEFIELD TableStart:RegionName` | Marks start of a detail region |
| `MERGEFIELD TableEnd:RegionName` | Marks end of a detail region |
| `DOCVARIABLE VarName "arg"` | Dynamic content (handled by `CalculateDocumentVariable` event) |
| `INCLUDEPICTURE "path"` | Inserts an image from a path or URI |

## Troubleshooting

- **Blank merged fields**: MERGEFIELD name must exactly match the column name in the data source (case-sensitive).
- **`Compare` exception on merge result**: Do not call `Compare` on a document that still has merge fields — execute the merge first.
- **Region exception**: The `TableStart` and `TableEnd` fields must be in the same table, and the region cannot span multiple paragraphs outside a table.
- **`CalculateDocumentVariable` not fired**: Ensure `e.Handled = true` is set; also ensure the field is a `DOCVARIABLE` field and it exists in the template.
- **Image not appearing**: The `RichEditDocumentServer` assigned to `e.Value` must contain the image as an inline shape with `TextWrapping = TextWrappingType.InLineWithText`.
- **Output is all records in one section**: Use `MergeMode.NewSection` or `MergeMode.JoinTables` depending on desired layout.
