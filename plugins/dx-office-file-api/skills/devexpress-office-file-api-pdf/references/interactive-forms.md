# Interactive Forms — DevExpress PDF Document API

PDF interactive forms (AcroForms) contain fillable fields that users can complete in a PDF viewer. The PDF Document API supports creating, reading, modifying, and flattening AcroForm fields including text boxes, check boxes, combo boxes, list boxes, radio groups, and signature fields.

## When to Use This Reference

Use this when you need to:
- Create new AcroForm fields in a PDF document
- Read or write field values in an existing form
- Change field appearance (font, color, border, background)
- Flatten a form (bake field content into page content, making it non-editable)
- Check for form field name collisions before adding fields
- Import or export AcroForm data (FDF, XFDF)

## Key Classes and Types

| Class | Purpose |
|-------|---------|
| `PdfAcroFormField` | Base class for all field types; factory methods (`CreateTextBox`, etc.) |
| `PdfAcroFormTextBoxField` | Text input field |
| `PdfAcroFormCheckBoxField` | Single check box |
| `PdfAcroFormComboBoxField` | Drop-down list |
| `PdfAcroFormListBoxField` | Scrollable list |
| `PdfAcroFormRadioGroupField` | Group of radio buttons |
| `PdfAcroFormSignatureField` | Digital signature field |
| `PdfAcroFormGroupField` | Logical group for organizing fields |
| `PdfDocumentFacade` | High-level document accessor — no direct inner-model access |
| `PdfAcroFormFacade` | Obtained via `PdfDocumentFacade.AcroForm` — read/write field properties |
| `PdfTextFormFieldFacade` | Properties of a text form field |
| `PdfGraphicsAcroFormTextBoxField` | Text box field created via the Graphics API |
| `PdfGraphicsAcroFormRadioGroupField` | Radio group field created via the Graphics API |

## Create Form Fields (Document Model API)

Use `PdfAcroFormField` factory methods to build fields, then add them with `PdfDocumentProcessor.AddFormFields`.

```csharp
using DevExpress.Pdf;
using System.Drawing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("form_template.pdf");

    // Create a text box
    PdfAcroFormTextBoxField textBox = PdfAcroFormField.CreateTextBox(
        pageNumber: 1,
        name: "FullName",
        bounds: new PdfRectangle(100, 650, 400, 675));  // page coordinate system
    textBox.Text = "Jane Doe";

    // Create a check box
    PdfAcroFormCheckBoxField checkBox = PdfAcroFormField.CreateCheckBox(
        pageNumber: 1,
        name: "Agree",
        bounds: new PdfRectangle(100, 620, 120, 640));
    checkBox.Checked = true;

    // Create a combo box
    PdfAcroFormComboBoxField comboBox = PdfAcroFormField.CreateComboBox(
        pageNumber: 1,
        name: "Country",
        bounds: new PdfRectangle(100, 590, 300, 610));
    comboBox.Items.Add(new PdfAcroFormComboBoxItem("United States", "US"));
    comboBox.Items.Add(new PdfAcroFormComboBoxItem("United Kingdom", "UK"));
    comboBox.SelectedIndex = 0;

    // Create a radio group
    PdfAcroFormRadioGroupField radioGroup = PdfAcroFormField.CreateRadioGroup(
        pageNumber: 1, name: "Gender");
    radioGroup.AddButton("Male",   new PdfRectangle(100, 560, 120, 580));
    radioGroup.AddButton("Female", new PdfRectangle(180, 560, 200, 580));
    radioGroup.SelectedIndex = 0;

    // Add all fields
    processor.AddFormFields(new PdfAcroFormField[]
        { textBox, checkBox, comboBox, radioGroup });
    processor.SaveDocument("filled_form.pdf");
}
```

## Create Form Fields (Graphics API)

Use the Graphics API to create fields as part of a new document workflow.

```csharp
using DevExpress.Pdf;
using System.Drawing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.CreateEmptyDocument("form.pdf");

    using (PdfGraphics graphics = processor.CreateGraphicsPageSystem())
    {
        // Text box
        var textBox = new PdfGraphicsAcroFormTextBoxField("CustomerName",
            new RectangleF(50, 50, 300, 30));
        textBox.Text = "Enter name here";
        textBox.Appearance.FontSize = 12;
        textBox.Appearance.BackgroundColor = Color.AliceBlue;
        graphics.AddFormField(textBox);

        // Radio group
        var radioGroup = new PdfGraphicsAcroFormRadioGroupField("Priority");
        radioGroup.AddButton("Low",    new RectangleF(50, 100, 20, 20));
        radioGroup.AddButton("Medium", new RectangleF(50, 130, 20, 20));
        radioGroup.AddButton("High",   new RectangleF(50, 160, 20, 20));
        radioGroup.SelectedIndex = 1;
        graphics.AddFormField(radioGroup);

        processor.RenderNewPage(PdfPaperSize.Letter, graphics);
    }
}
```

## Read and Modify Existing Field Values

Use `PdfDocumentFacade` and `PdfAcroFormFacade` to access field properties without accessing the inner document model.

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("FormDemo.pdf");
    PdfDocumentFacade facade = processor.DocumentFacade;
    PdfAcroFormFacade acroForm = facade.AcroForm;

    // Get all field names
    IList<string> names = acroForm.GetNames();

    // Read a text field
    PdfTextFormFieldFacade visaField = acroForm.GetTextFormField("VisaNo");
    if (visaField != null)
    {
        visaField.Text = "A1234567";
        visaField.MaxLength = 8;
        visaField.InputType = PdfTextFieldInputType.Comb;  // equal-spaced characters
    }

    // Read a check box
    PdfCheckBoxFormFieldFacade agreeBox = acroForm.GetCheckBoxFormField("AgreeToTerms");
    if (agreeBox != null)
        agreeBox.Checked = true;

    // Read a combo box
    PdfComboBoxFormFieldFacade countryField = acroForm.GetComboBoxFormField("Country");
    if (countryField != null)
        countryField.SelectedIndex = 1;

    processor.SaveDocument("filled.pdf");
}
```

## Flatten a Form

Flattening converts all form fields into static page content (non-editable).

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("form.pdf");

    // Flatten all form fields (converts fields to static content)
    processor.FlattenForm();

    processor.SaveDocument("flattened.pdf");
}
```

## Check for Name Collisions

Before adding fields to an existing document, verify name uniqueness:

```csharp
var newFields = new List<PdfAcroFormField> { textBox, radioGroup };

IList<PdfAcroFormFieldNameCollision> collisions =
    processor.CheckFormFieldNameCollisions(newFields);

foreach (var collision in collisions)
{
    // Rename the conflicting field to a GUID
    while (collision.ForbiddenNames.Contains(collision.Field.Name))
        collision.Field.Name = Guid.NewGuid().ToString();
}

processor.AddFormFields(newFields);
processor.SaveDocument("Result.pdf");
```

## Configuration Options

| Property | Description |
|----------|-------------|
| `PdfAcroFormTextBoxField.Text` | Current text value |
| `PdfAcroFormTextBoxField.MaxLength` | Maximum number of characters |
| `PdfAcroFormCheckBoxField.Checked` | Checked state |
| `PdfAcroFormComboBoxField.SelectedIndex` | Selected item (0-based) |
| `PdfAcroFormRadioGroupField.SelectedIndex` | Selected button (0-based) |
| `PdfGraphicsAcroFormField.Appearance.FontSize` | Field text font size |
| `PdfGraphicsAcroFormField.Appearance.BackgroundColor` | Field background fill color |
| `PdfGraphicsAcroFormBorderAppearance.Color` | Border stroke color |
| `PdfGraphicsAcroFormBorderAppearance.Width` | Border width |
| `PdfTextFieldInputType.Comb` | Equal-spaced character boxes (like a postal code field) |

## Troubleshooting

- **Field does not appear in viewer**: Ensure `AddFormFields` is called and `SaveDocument` is called after. Verify page number (1-based) is correct.
- **Field name collision exception**: Use `CheckFormFieldNameCollisions` before `AddFormFields`, and rename conflicting fields.
- **Combo box has no items**: Add items to `comboBox.Items` (or via `PdfComboBoxFormFieldFacade.Items`) before saving.
- **Flattened document still shows fields**: Ensure `processor.FlattenForm()` is called on the `PdfDocumentProcessor` instance, not on individual field facades.
- **Signature field not visible**: Signature fields require a visual bounds rectangle (`SignatureBounds`). See [document-security.md](document-security.md) for digital signature field setup.
