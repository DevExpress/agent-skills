# Safe Spreadsheet Processing — Spreadsheet Document API

**v26.1+** — Available in `DevExpress.Spreadsheet` namespace.

## When You Need to:
- Reject oversized or structurally abnormal workbooks before full parsing (DoS protection)
- Strip macros, OLE objects, ActiveX, external connections, and pivot caches during load
- Remove personal metadata, comments, and hidden rows/columns/sheets before sharing (GDPR, HIPAA, SOX)
- Inspect a workbook to discover what sensitive content it contains before sanitizing

## Security Loading Limits

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook()) {
    WorkbookSecurityLoadingLimits securityLimits =
        workbook.Options.SecurityLoadingLimits;

    securityLimits.MaxFileSize = 50 * 1024 * 1024; // 50 MB
    securityLimits.MaxSheetColumnCount = 100;
    securityLimits.MaxSheetRowCount = 50;
    securityLimits.MaxWorksheetCount = 10;

    workbook.LoadDocument("Documents\\Sample.xlsx");
}
```

```vb
Imports DevExpress.Spreadsheet

Using workbook As New Workbook()
    Dim securityLimits As WorkbookSecurityLoadingLimits =
        workbook.Options.SecurityLoadingLimits

    securityLimits.MaxFileSize = 50 * 1024 * 1024 ' 50 MB
    securityLimits.MaxSheetColumnCount = 100
    securityLimits.MaxSheetRowCount = 50
    securityLimits.MaxWorksheetCount = 10

    workbook.LoadDocument("Documents\Sample.xlsx")
End Using
```

**Handle violations** — `SecurityLoadingLimitExceeded` fires when a limit is exceeded. Set `e.Handled = false` to abort; `e.Handled = true` to continue (log-only mode):

```csharp
workbook.SecurityLoadingLimitExceeded += (sender, e) => {
    Console.WriteLine($"Limit exceeded: {e.PropertyName}");
    e.Handled = false; // abort loading
};
```

### Configurable Limits

| Property | Description |
|----------|-------------|
| `MaxFileSize` | Maximum file size in bytes |
| `MaxWorksheetCount` | Maximum worksheets in the workbook |
| `MaxSheetRowCount` | Maximum rows per worksheet |
| `MaxSheetColumnCount` | Maximum columns per worksheet |
| `MaxCellCount` | Maximum cells per worksheet |
| `MaxChartCount` | Maximum charts in the workbook |
| `MaxXmlElementCount` | Maximum total XML elements |
| `MaxXmlElementDepth` | Maximum XML nesting depth |

## Remove Dangerous Content

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook()) {
    WorkbookSecurityLoadingOptions securityLoadingOptions =
        workbook.Options.SecurityLoadingOptions;

    securityLoadingOptions.RemoveMacros = true;
    securityLoadingOptions.RemoveActiveXContent = true;
    securityLoadingOptions.RemoveOleObjects = true;
    securityLoadingOptions.RemoveRestrictedFormulas = true;
    securityLoadingOptions.RemoveExternalWorkbooks = true;
    securityLoadingOptions.RemoveExternalConnections = true;
    securityLoadingOptions.RemovePivotCaches = true;
    securityLoadingOptions.RemoveCustomXMLParts = true;

    workbook.SecurityLoadingOptionsViolation += (_, e) => {
        Console.WriteLine($"Dangerous content found: {e.PropertyName}");
        e.Handled = false; // false = remove the content
    };

    workbook.LoadDocument("Documents\\Sample.xlsx");
}
```

```vb
Imports DevExpress.Spreadsheet

Using workbook As New Workbook()
    Dim securityLoadingOptions As WorkbookSecurityLoadingOptions =
        workbook.Options.SecurityLoadingOptions

    securityLoadingOptions.RemoveMacros = True
    securityLoadingOptions.RemoveActiveXContent = True
    securityLoadingOptions.RemoveOleObjects = True
    securityLoadingOptions.RemoveRestrictedFormulas = True
    securityLoadingOptions.RemoveExternalWorkbooks = True
    securityLoadingOptions.RemoveExternalConnections = True
    securityLoadingOptions.RemovePivotCaches = True
    securityLoadingOptions.RemoveCustomXMLParts = True

    AddHandler workbook.SecurityLoadingOptionsViolation, Sub(_, e)
        Console.WriteLine($"Dangerous content found: {e.PropertyName}")
        e.Handled = False ' False = remove the content
    End Sub

    workbook.LoadDocument("Documents\Sample.xlsx")
End Using
```

## Sanitize Private Information

```csharp
using DevExpress.Office;
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook()) {
    workbook.LoadDocument("Documents\\Sample.xlsx");

    WorkbookSanitizeOptions sanitizeOptions = new WorkbookSanitizeOptions {
        RemoveThreadedComments = true,
        HiddenSheets = HiddenContentSanitizeMode.MakeVisible
    };

    var results = workbook.Sanitize(sanitizeOptions);

    foreach (var result in results)
        Console.WriteLine($"Content type: {result.Type}, Action taken: {result.Action}");
}
```

```vb
Imports DevExpress.Office
Imports DevExpress.Spreadsheet

Using workbook As New Workbook()
    workbook.LoadDocument("Documents\Sample.xlsx")

    Dim sanitizeOptions As New WorkbookSanitizeOptions With {
        .RemoveThreadedComments = True,
        .HiddenSheets = HiddenContentSanitizeMode.MakeVisible
    }

    Dim results = workbook.Sanitize(sanitizeOptions)

    For Each result In results
        Console.WriteLine($"Content type: {result.Type}, Action taken: {result.Action}")
    Next
End Using
```

### WorkbookSanitizeOptions Properties

| Property | Type | Description |
|----------|------|-------------|
| `Metadata` | `MetadataRemovalScope` | `All` = clear all document properties |
| `RemoveComments` | `bool` | Remove all legacy comments |
| `RemoveThreadedComments` | `bool` | Remove all threaded comment threads |
| `InvisibleCellText` | `InvisibleContentSanitizeMode` | Handle text with matching foreground/background |
| `HiddenRows` | `HiddenContentSanitizeMode` | `Remove` / `MakeVisible` hidden rows |
| `HiddenColumns` | `HiddenContentSanitizeMode` | `Remove` / `MakeVisible` hidden columns |
| `HiddenSheets` | `HiddenContentSanitizeMode` | `Remove` / `MakeVisible` hidden worksheets |

## Inspect Before Sanitizing

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook()) {
    workbook.LoadDocument("Documents\\Sample.xlsx");

    WorkbookInspectResult inspectResult = workbook.Inspect(new WorkbookInspectOptions());
    WorkbookSanitizeOptions sanitizeOptions = inspectResult.CreateSanitizeOptions();

    var results = workbook.Sanitize(sanitizeOptions);

    foreach (var result in results)
        Console.WriteLine($"Content type: {result.Type}, Action taken: {result.Action}");
}
```

## Compliance Context

| Regulation | Applicable Feature |
|------------|-------------------|
| GDPR Art. 5 | `Metadata = MetadataRemovalScope.All` removes author names, paths, timestamps |
| HIPAA | `RemoveMacros`, `RemoveOleObjects`, `RemoveActiveXContent` prevent malware vectors |
| SOX § 404 | `Sanitize()` returns a structured findings list suitable for audit trail |
