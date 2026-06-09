# Structure Tree (Tagged PDF / PDF/UA)

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Create an accessible tagged PDF with a logical structure tree
- Map custom structure tags to standard PDF roles
- Edit the structure tree of an existing document
- Remove elements from a structure tree
- Validate a structure tree for PDF/UA compliance

## Concept

A tagged PDF defines a structure tree — a logical hierarchy of elements (Document > Section > Heading, Paragraph, Table, etc.). Assistive technologies (screen readers) use this tree to improve accessibility. Use `PdfDocument.StructureTree` to build or edit it.

## Create a Structure Tree for a New Document

Use `StructureTree.AddChildElement(Pdf17StructureType)` to build the tree top-down. Each `StructureElement` adds fragments to a page via `element.AddFragment(page, fragment)`.

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

using (PdfDocument document = new PdfDocument()) {
    Page page = document.Pages.Add(DXPaperKind.A4);

    // Build structure tree: Document > Section > Heading / Table / Paragraph
    StructureElement doc = document.StructureTree
        .AddChildElement(Pdf17StructureType.Document);
    StructureElement section = doc.AddChildElement(Pdf17StructureType.Sect);

    // Heading
    StructureElement heading = section.AddChildElement(Pdf17StructureType.H1);
    heading.AddFragment(page, new TextFragment {
        Text = "Invoice",
        Location = new PointF(50, 800),
        Font = new TextFont("Arial", TextFontStyle.Bold),
        FontSize = 24
    });

    // Table with layout attributes
    StructureElement table = section.AddChildElement(Pdf17StructureType.Table);
    table.Attributes.Add(new LayoutAttribute {
        Placement = LayoutPlacement.Block,
        WritingMode = WritingMode.LeftToRight
    });
    table.Attributes.Add(new TableAttribute { Summary = "Invoice items" });

    // Table header row
    StructureElement thead = table.AddChildElement(Pdf17StructureType.THead);
    StructureElement headerRow = thead.AddChildElement(Pdf17StructureType.TR);

    StructureElement th1 = headerRow.AddChildElement(Pdf17StructureType.TH);
    th1.Attributes.Add(new TableAttribute { Scope = TableScope.Column });
    th1.AddFragment(page, new TextFragment {
        Text = "Item", Location = new PointF(50, 700),
        Font = new TextFont("Arial", TextFontStyle.Bold), FontSize = 12
    });

    // Table body row
    StructureElement tbody = table.AddChildElement(Pdf17StructureType.TBody);
    StructureElement dataRow = tbody.AddChildElement(Pdf17StructureType.TR);
    StructureElement td1 = dataRow.AddChildElement(Pdf17StructureType.TD);
    td1.AddFragment(page, new TextFragment {
        Text = "Product A", Location = new PointF(50, 670)
    });

    // Footer paragraph
    StructureElement pTotal = section.AddChildElement(Pdf17StructureType.P);
    pTotal.AddFragment(page, new TextFragment {
        Text = "Total: $50.00", Location = new PointF(50, 620), Bold = true
    });

    document.Save(File.Create("TaggedInvoice.pdf"));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing.Printing
Imports System.Drawing
Imports System.IO

Using document As New PdfDocument()
    Dim page As Page = document.Pages.Add(DXPaperKind.A4)

    Dim doc As StructureElement = document.StructureTree _
        .AddChildElement(Pdf17StructureType.Document)
    Dim section As StructureElement = doc.AddChildElement(Pdf17StructureType.Sect)

    Dim heading As StructureElement = section.AddChildElement(Pdf17StructureType.H1)
    heading.AddFragment(page, New TextFragment With {
        .Text = "Invoice",
        .Location = New PointF(50, 800),
        .Font = New TextFont("Arial", TextFontStyle.Bold),
        .FontSize = 24
    })

    document.Save(File.Create("TaggedInvoice.pdf"))
End Using
```

## Common PDF 1.7 Structure Type Descriptors

| Descriptor | Description |
|------------|-------------|
| `Pdf17StructureType.Document` | Root document element |
| `Pdf17StructureType.Sect` | Section |
| `Pdf17StructureType.H1`–`H6` | Heading levels |
| `Pdf17StructureType.P` | Paragraph |
| `Pdf17StructureType.Table` | Table |
| `Pdf17StructureType.THead` / `TBody` / `TFoot` | Table head/body/foot |
| `Pdf17StructureType.TR` | Table row |
| `Pdf17StructureType.TH` / `TD` | Header cell / data cell |
| `Pdf17StructureType.Div` | Division (generic block) |
| `Pdf17StructureType.Figure` | Figure/illustration |
| `Pdf17StructureType.Span` | Inline element |

Use `Pdf20StructureType` for PDF 2.0 types.

## Map Custom Tags to Standard PDF Roles

```csharp
StructureTree structureTree = document.StructureTree;

// Map custom tag names to standard PDF structure roles
structureTree.RoleMap["CompanyTitle"] = "H1";
structureTree.RoleMap["ContentBlock"] = "P";
structureTree.RoleMap["Disclaimer"] = "Span";

// Use a custom type descriptor
var customHeading = new Pdf17StructureTypeDescriptor("CompanyTitle");
StructureElement docElem = structureTree.AddChildElement(Pdf17StructureType.Document);
StructureElement heading = docElem.AddChildElement(customHeading);
heading.AddFragment(page, new TextFragment {
    Text = "Acme Corporation",
    Location = new PointF(50, 800),
    FontSize = 24
});
```

## Edit an Existing Document's Structure Tree

```csharp
using (PdfDocument pdfDocument = new PdfDocument(File.OpenRead("Document.pdf"))) {
    StructureTree structureTree = pdfDocument.StructureTree;

    // Find a section element
    StructureElement section = structureTree.Elements.Find(
        e => e is StructureElement se && se.Type == "Sect") as StructureElement;

    if (section != null) {
        StructureElement heading = section.AddChildElement(Pdf17StructureType.H2);
        Page page = pdfDocument.Pages[0];
        heading.AddFragment(page, new TextFragment {
            Text = "New Heading",
            Location = new PointF(50, 700),
            Bold = true
        });
    }

    pdfDocument.Save(File.Create("UpdatedDocument.pdf"));
}
```

## Remove Elements

```csharp
StructureItem target = structureTree.Elements.Find(
    e => e is StructureElement se && se.Descriptor.Value == "Private",
    out StructureElement parent);

if (target != null)
    parent.Elements.Remove(target);
```

## Validate a Structure Tree

```csharp
using (PdfDocument pdfDocument = new PdfDocument(File.OpenRead("Document.pdf"))) {
    StructureTreeValidationResult result = pdfDocument.StructureTree.Validate();

    if (!result.IsValid) {
        foreach (StructureTreeValidationError error in result.Errors) {
            Console.WriteLine($"Error: {error.Message}");
            StructureElement parent = error.Parent;
            StructureItem invalidChild = error.InvalidChild;
            parent.Elements.Remove(invalidChild);
            // Optionally replace with a valid type:
            parent.AddChildElement(Pdf17StructureType.Div);
        }
    }

    pdfDocument.Save(File.Create("ValidatedDocument.pdf"));
}
```

## Key API

| Member | Description |
|--------|-------------|
| `PdfDocument.StructureTree` | `StructureTree` — access or build the tree |
| `StructureTree.AddChildElement(StructureTypeDescriptor)` | Add root-level element |
| `StructureElement.AddChildElement(StructureTypeDescriptor)` | Add child element |
| `StructureElement.AddFragment(page, PageFragment)` | Bind content to a structure element |
| `StructureElement.Attributes` | Collection of `LayoutAttribute`, `TableAttribute`, etc. |
| `StructureTree.RoleMap` | Dictionary mapping custom tag names to standard roles |
| `StructureTree.Elements` | Collection of root structure items |
| `StructureElementCollection.Find(predicate)` | Find element by predicate |
| `StructureTree.Validate()` | Returns `StructureTreeValidationResult` |
| `StructureTreeValidationResult.IsValid` | `true` if tree is PDF/UA compliant |
| `StructureTreeValidationResult.Errors` | List of `StructureTreeValidationError` |
