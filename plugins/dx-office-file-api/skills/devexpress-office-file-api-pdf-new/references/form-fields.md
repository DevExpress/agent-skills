# Interactive Form Fields

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Create a fillable AcroForm PDF with text boxes, checkboxes, radio buttons, list/combo boxes, or signature fields
- Read or fill field values programmatically
- Import/export form data (FDF, XFDF, XML, TXT)
- Group fields logically
- Search for fields by name or predicate

## Field Architecture

Each form field is a non-visual data container. To display it on a page, bind it to a widget annotation and add the widget to `page.Annotations`.

**Pattern**: `Create field → Add to pdfDocument.Fields → Create widget(field) → Add widget to page.Annotations`

Set `field.ReadOnly = true` to make a field non-editable (users can view but not change its value).

## Text Box Field

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using System.Drawing;

using (PdfDocument pdfDocument = new PdfDocument()) {
    Page page = pdfDocument.Pages.Add(DXPaperKind.A4);

    TextBoxField loginField = new TextBoxField("Login");
    pdfDocument.Fields.Add(loginField);

    TextBoxWidgetAnnotation loginWidget = new TextBoxWidgetAnnotation(loginField) {
        Bounds = new RectangleF(120, 730, 220, 20)
    };
    page.Annotations.Add(loginWidget);
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports System.Drawing

Using pdfDocument As New PdfDocument()
    Dim page As Page = pdfDocument.Pages.Add(DXPaperKind.A4)

    Dim loginField As New TextBoxField("Login")
    pdfDocument.Fields.Add(loginField)

    Dim loginWidget As New TextBoxWidgetAnnotation(loginField) With {
        .Bounds = New RectangleF(120, 730, 220, 20)
    }
    page.Annotations.Add(loginWidget)
End Using
```

## Checkbox Field

```csharp
CheckBoxField agreeField = new CheckBoxField("AgreeToTerms");
pdfDocument.Fields.Add(agreeField);

CheckBoxWidgetAnnotation agreeWidget = new CheckBoxWidgetAnnotation(agreeField) {
    Bounds = new RectangleF(50, 730, 18, 18)
};
page.Annotations.Add(agreeWidget);
```

## Radio Button Group

```csharp
RadioGroupField radioGroupField = new RadioGroupField("radioGroup");
pdfDocument.Fields.Add(radioGroupField);

// Each item is a separate widget with a unique value string
RadioGroupItemWidgetAnnotation item1 = new RadioGroupItemWidgetAnnotation(
    radioGroupField, "Option1", new RectangleF(50, 710, 18, 18));
RadioGroupItemWidgetAnnotation item2 = new RadioGroupItemWidgetAnnotation(
    radioGroupField, "Option2", new RectangleF(50, 685, 18, 18));
RadioGroupItemWidgetAnnotation item3 = new RadioGroupItemWidgetAnnotation(
    radioGroupField, "Option3", new RectangleF(50, 660, 18, 18));

page.Annotations.Add(item1);
page.Annotations.Add(item2);
page.Annotations.Add(item3);

// Pre-select a value
radioGroupField.Value = "Option3";
```

## List Box Field

```csharp
ListBoxField listBoxField = new ListBoxField("listBox");
listBoxField.Items.Add(new ChoiceFieldItem("Option #1"));
listBoxField.Items.Add(new ChoiceFieldItem("Option #2"));
listBoxField.Items.Add(new ChoiceFieldItem("Option #3"));
pdfDocument.Fields.Add(listBoxField);

ListBoxWidgetAnnotation listBoxWidget = new ListBoxWidgetAnnotation(listBoxField) {
    Bounds = new RectangleF(20, 500, 100, 80)
};
listBoxWidget.BackgroundColor = PdfColor.White;
listBoxWidget.BorderColor = PdfColor.Red;
page.Annotations.Add(listBoxWidget);

// Get selected values / set selection
// listBoxField.SelectedValues
// listBoxField.SelectValue("Option #2");
// listBoxField.SetSelected(0, true);
```

## Combo Box Field

```csharp
ComboBoxField comboBoxField = new ComboBoxField("comboBox");
comboBoxField.Items.Add(new ChoiceFieldItem("Option #1"));
comboBoxField.Items.Add(new ChoiceFieldItem("Option #2"));
comboBoxField.Items.Add(new ChoiceFieldItem("Option #3"));
comboBoxField.Value = "Option #2";    // pre-select
pdfDocument.Fields.Add(comboBoxField);

ComboBoxWidgetAnnotation comboBoxWidget = new ComboBoxWidgetAnnotation(comboBoxField) {
    Bounds = new RectangleF(20, 680, 100, 30)
};
page.Annotations.Add(comboBoxWidget);
```

## Signature Field

```csharp
SignatureField signatureField = new SignatureField("signature");
signatureField.ShowDate = true;
pdfDocument.Fields.Add(signatureField);

SignatureWidgetAnnotation signatureWidget = new SignatureWidgetAnnotation(signatureField) {
    Bounds = new RectangleF(20, 400, 200, 100)
};
signatureWidget.BorderColor = PdfColor.Black;
page.Annotations.Add(signatureWidget);
```

`SignatureField` info properties: `ContactInfo`, `Reason`, `SignerName`, `SigningTime`.  
Display toggle properties: `ShowDate`, `ShowLabels`, `ShowLocation`, `ShowReason`, `ShowSignerName`.

## Field Types Quick Reference

| Field Class | Widget Class | Notes |
|-------------|-------------|-------|
| `TextBoxField` | `TextBoxWidgetAnnotation` | Single/multi-line text input |
| `CheckBoxField` | `CheckBoxWidgetAnnotation` | Boolean toggle |
| `RadioGroupField` | `RadioGroupItemWidgetAnnotation` | One selection from multiple items |
| `ListBoxField` | `ListBoxWidgetAnnotation` | Scrollable multi-item list |
| `ComboBoxField` | `ComboBoxWidgetAnnotation` | Dropdown with optional items |
| `SignatureField` | `SignatureWidgetAnnotation` | Digital signature placeholder |

## Fill and Read Field Values

```csharp
// Fill by name (cast to concrete type)
FormField field = pdfDocument.Fields.FindByName("FirstName");
((TextBoxField)field).Value = "Alice";

// Read all fields
foreach (FormField f in pdfDocument.Fields) {
    if (f is TextBoxField tb)
        Console.WriteLine($"{tb.Name} = {tb.Value}");
}
```

## Search for Fields

```csharp
FormField field = pdfDocument.Fields.Find(f => f.Name.StartsWith("First"));
List<FormField> fields = pdfDocument.Fields.FindAll(f => f.Name.Contains("Name"));
FormField byName = pdfDocument.Fields.FindByName("Login");
```

## Import / Export Form Data

Supported formats: `ExportDataFormat.Fdf`, `ExportDataFormat.Xfdf`, `ExportDataFormat.Xml`, `ExportDataFormat.Txt`.

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

// Import
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("EmptyForm.pdf", FileMode.Open, FileAccess.Read))) {
    pdfDocument.ImportFormData(
        new FileStream("FormData.xml", FileMode.Open, FileAccess.Read),
        ExportDataFormat.Xml);
    pdfDocument.Save(new FileStream("FilledForm.pdf", FileMode.Create, FileAccess.Write));
}

// Export
pdfDocument.ExportFormData(
    new FileStream("ExportedData.xml", FileMode.OpenOrCreate),
    ExportDataFormat.Xml);
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.IO

Using pdfDocument As New PdfDocument(New FileStream("EmptyForm.pdf", FileMode.Open, FileAccess.Read))
    pdfDocument.ImportFormData(
        New FileStream("FormData.xml", FileMode.Open, FileAccess.Read),
        ExportDataFormat.Xml)
    pdfDocument.Save(New FileStream("FilledForm.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## Group Fields

`GroupField` organizes fields logically without visual representation.

```csharp
TextBoxField firstNField = new TextBoxField("FirstName");
TextBoxField lastNField = new TextBoxField("LastName");

GroupField groupField = new GroupField("PersonalInfo");
groupField.Kids.Add(firstNField);
groupField.Kids.Add(lastNField);
pdfDocument.Fields.Add(groupField);

// Find a field's parent group
GroupField parent = pdfDocument.Fields.FindParent(firstNField);
```

## Value Formatting

```csharp
// Date format
TextBoxField dateField = new TextBoxField("Date");
dateField.ValueFormat = FormFieldValueFormat.CreateDateTimeFormat("mmmm/dd/yyyy");

// Available factory methods:
// FormFieldValueFormat.CreateNumberFormat(...)
// FormFieldValueFormat.CreatePercentFormat(...)
// FormFieldValueFormat.CreateTimeFormat(...)
// FormFieldValueFormat.CreateSpecialFormat(...)
```
