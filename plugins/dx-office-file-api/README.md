# AI Agent Skills for DevExpress Office & PDF File API

This plugin contains AI agent skills for [DevExpress Office & PDF File API](https://www.devexpress.com/products/net/office-file-api/) (AI-powered .NET library that allows you to generate, edit, and convert spreadsheets, rich text documents, presentations, PDF files, Barcodes, and ZIP archives).

---

## Prerequisites

- .NET 8+ or .NET Framework 4.6.2+ application
- DevExpress v26.1+ NuGet packages: unified package with all APIs (`DevExpress.Document.Processor`) or individual API packages (such as `DevExpress.Docs.Pdf`, `DevExpress.Docs.Presentation`, `DevExpress.Docs.Barcode`)
- [DevExpress subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/) (Office File API Subscription or Universal Subscription)
- AI provider credentials - Azure OpenAI, OpenAI, Ollama, Google Gemini, or another supported provider (if using AI-powered extensions skill `devexpress-office-file-api-ai-powered-extensions`)

---

## Included AI Agent Skills

| Skill | Capabilities | Docs |
| --- | --- | --- |
| [devexpress-office-file-api-spreadsheet](skills/devexpress-office-file-api-spreadsheet/) | Spreadsheet Document API: worksheets, cells, formulas, formatting, charts, pivot tables, import/export, protection | [Overview](https://docs.devexpress.com/OfficeFileAPI/14912) |
| [devexpress-office-file-api-word-processing](skills/devexpress-office-file-api-word-processing/) | Word Processing API: paragraphs, styles, tables, mail merge, export | [Overview](https://docs.devexpress.com/OfficeFileAPI/17488) |
| [devexpress-office-file-api-pdf](skills/devexpress-office-file-api-pdf/) | Legacy PDF File API (`PdfDocumentProcessor`): content, forms, annotations, security | [Overview](https://docs.devexpress.com/OfficeFileAPI/16491) |
| [devexpress-office-file-api-pdf-new](skills/devexpress-office-file-api-pdf-new/) | New PDF File API (`DevExpress.Docs.Pdf`) CTP: content, forms, annotations, security | [Overview](https://docs.devexpress.com/OfficeFileAPI/405907/pdf-document-api-new/overview?v=26.1) |
| [devexpress-office-file-api-presentation](skills/devexpress-office-file-api-presentation/) | Presentation API: slides, shapes, text, charts, export, merge | [Overview](https://docs.devexpress.com/OfficeFileAPI/405405) |
| [devexpress-office-file-api-barcode](skills/devexpress-office-file-api-barcode/) | Barcode API: generate barcodes with various supported symbologies (QR code, Data Matrix, Code 128, Aztec, and others) | [Overview](https://docs.devexpress.com/OfficeFileAPI/15094) |
| [devexpress-office-file-api-unit-conversion](skills/devexpress-office-file-api-unit-conversion/) | Unit Conversion API: metric/imperial conversions across common unit families | [Overview](https://docs.devexpress.com/OfficeFileAPI/15095) |
| [devexpress-office-file-api-excel-export](skills/devexpress-office-file-api-excel-export/) | Excel Export Library: streaming XLSX generation based on large datasets | [Overview](https://docs.devexpress.com/OfficeFileAPI/114031) |
| [devexpress-office-file-api-zip](skills/devexpress-office-file-api-zip/) | ZIP Archive API: archival, extraction, encryption, secure extraction policies | [Overview](https://docs.devexpress.com/OfficeFileAPI/15093) |
| [devexpress-office-file-api-ai-powered-extensions](skills/devexpress-office-file-api-ai-powered-extensions/) | AI-powered Extensions: Use AI providers to summarize, translate, or proofread documents | [Overview](https://docs.devexpress.com/OfficeFileAPI/405645/ai-powered-extensions) |

---

## Skill Folder Content

Each skill folder contains the following files and sub-directories:

- `SKILL.md` - activators, rules, and navigation guidance
- `references/` - feature-specific implementation guides
- `examples/quickstart.cs` - getting-started samples

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
