# Getting Started with the DevExpress Excel Export Library (.NET Framework)

This guide covers setting up the Excel Export Library in a .NET Framework 4.6.2+ project using assembly references rather than NuGet packages.

## When to Use This Reference

Use this when you need to:
- Set up the Excel Export Library in a .NET Framework 4.6.2+ project
- Understand which DLL assemblies are required
- Handle the DevExpress installer vs. NuGet installation paths
- Know about framework-specific API differences (e.g., `Process.Start` overload)

## System Requirements

- .NET Framework 4.6.2 or later
- Visual Studio 2019+ (recommended)
- DevExpress installation (v25.2) or the DevExpress NuGet feed
- Valid DevExpress license (Office File API or Universal Subscription)

## Installation Option A: DevExpress Installer (Recommended for .NET Framework)

After running the DevExpress installer, the assemblies are available locally. Add references to your project:

1. In **Solution Explorer**, right-click **References** and select **Add Reference**.
2. In the **Reference Manager**, browse to the DevExpress installation folder (typically `C:\Program Files (x86)\DevExpress 25.2\Components\Bin\Framework\`).
3. Add references to these assemblies:

| Assembly | Purpose |
|----------|---------|
| `DevExpress.Data.v25.2.dll` | Core data layer |
| `DevExpress.Drawing.v25.2.dll` | Drawing primitives |
| `DevExpress.Printing.v25.2.Core.dll` | Excel Export Library core (contains `DevExpress.Export.Xl`) |

> **String formulas only**: If you need to parse text-based formulas with `XlFormulaParser`, also reference:
> - `DevExpress.Spreadsheet.v25.2.Core.dll`

## Installation Option B: NuGet Package

If you prefer NuGet in a .NET Framework project, add the DevExpress feed and install:

```
Install-Package DevExpress.Document.Processor
```

The NuGet package pulls the correct DLLs automatically.

**Important**: All DevExpress packages must use the **same version**. Do not mix, for example, `25.2.x` with `25.1.x`.

## Namespace Import

```csharp
using DevExpress.Export.Xl;
using System.IO;
using System.Diagnostics; // For Process.Start
```

## Key Difference: Launching the Generated File

In .NET Framework, use the older `Process.Start` overload (no `ProcessStartInfo` object is required):

```csharp
// .NET Framework — simpler overload
System.Diagnostics.Process.Start("Document.xlsx");
```

In .NET 6+, use `ProcessStartInfo` with `UseShellExecute = true`:

```csharp
// .NET 6+ — required overload
Process.Start(new ProcessStartInfo("Document.xlsx") { UseShellExecute = true });
```

## Complete .NET Framework Example

The API usage is identical to .NET — only the project setup differs:

```csharp
using DevExpress.Export.Xl;
using System.IO;

namespace XLExportFrameworkExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an exporter for XLSX format
            IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

            using (FileStream stream = new FileStream("Report.xlsx", FileMode.Create, FileAccess.ReadWrite))
            using (IXlDocument document = exporter.CreateDocument(stream))
            {
                using (IXlSheet sheet = document.CreateSheet())
                {
                    sheet.Name = "Data";

                    // Column definitions
                    using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 200; }
                    using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 100; }

                    // Header row
                    XlCellFormatting headerFmt = new XlCellFormatting();
                    headerFmt.Font = new XlFont();
                    headerFmt.Font.Bold = true;
                    headerFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent2, 0.0));

                    using (IXlRow row = sheet.CreateRow())
                    {
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "Item";
                            cell.ApplyFormatting(headerFmt);
                        }
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "Value";
                            cell.ApplyFormatting(headerFmt);
                        }
                    }

                    // Data rows
                    string[] items = { "Alpha", "Beta", "Gamma" };
                    double[] values = { 1500.0, 2300.5, 870.25 };

                    for (int i = 0; i < items.Length; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            using (IXlCell cell = row.CreateCell()) { cell.Value = items[i]; }
                            using (IXlCell cell = row.CreateCell()) { cell.Value = values[i]; }
                        }
                    }
                }
            }

            // Open the file with the default application (.NET Framework style)
            System.Diagnostics.Process.Start("Report.xlsx");
        }
    }
}
```

## Using String Formulas in .NET Framework

Reference `DevExpress.Spreadsheet.v25.2.Core.dll`, then:

```csharp
using DevExpress.Spreadsheet; // for XlFormulaParser

IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx, new XlFormulaParser());
```

After that, formula strings work exactly as in .NET:

```csharp
cell.SetFormula("=SUM(B2:B10)");
```

## Supported .NET Framework Versions

| Framework | Supported |
|-----------|-----------|
| .NET Framework 4.6.2 | Yes |
| .NET Framework 4.7 / 4.7.1 / 4.7.2 | Yes |
| .NET Framework 4.8 | Yes |
| .NET Framework 4.5 or earlier | Not supported |

## Troubleshooting

- **`FileNotFoundException` for DevExpress assembly**: Verify the assembly path and version match. Check that the `Copy Local` property of each reference is set to `true`.
- **`TypeLoadException` or binding redirect errors**: Add binding redirects in `app.config` if multiple DevExpress versions are present. Prefer using only one version.
- **`XlFormulaParser` not found**: Add a reference to `DevExpress.Spreadsheet.v25.2.Core.dll`.
- **License error at startup**: Register the DevExpress license or ensure the project is running in an environment where the DevExpress installer has activated the license.

## What to Learn Next

- [getting-started.md](getting-started.md): .NET 6/7/8+ setup with NuGet
- [cells-and-formatting.md](cells-and-formatting.md): Cell values, fonts, colors, borders, number formats
- [advanced-features.md](advanced-features.md): Tables, formulas, sparklines, printing, data validation
