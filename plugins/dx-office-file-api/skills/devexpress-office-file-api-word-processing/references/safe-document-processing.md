# Safer Document Processing — Word Processing Document API

**v26.1+** — Available in `DevExpress.XtraRichEdit` namespace.

## When You Need to:
- Reject oversized or structurally abnormal documents before full parsing (DoS protection)
- Strip macros, OLE objects, ActiveX, and dangerous hyperlinks during document load
- Remove personal metadata, revision history, and hidden content before sharing (GDPR, HIPAA, SOX)
- Inspect a document to discover what sensitive content it contains before sanitizing

## Security Loading Limits

Protect against document-based denial-of-service attacks. Limits take effect before full parsing.

```csharp
using DevExpress.XtraRichEdit;

using (RichEditDocumentServer wordProcessor = new RichEditDocumentServer()) {
    WordProcessingSecurityLoadingLimits securityLimits =
        wordProcessor.Options.SecurityLoadingLimits;

    securityLimits.MaxFileSize = 50 * 1024 * 1024; // 50 MB
    securityLimits.MaxParagraphCount = 100_000;
    securityLimits.MaxTableCount = 1_000;
    securityLimits.MaxXmlElementDepth = 128;

    wordProcessor.LoadDocument("Documents\\Sample.docx");
}
```

```vb
Imports DevExpress.XtraRichEdit

Using wordProcessor As New RichEditDocumentServer()
    Dim securityLimits As WordProcessingSecurityLoadingLimits =
        wordProcessor.Options.SecurityLoadingLimits

    securityLimits.MaxFileSize = 50 * 1024 * 1024 ' 50 MB
    securityLimits.MaxParagraphCount = 100000
    securityLimits.MaxTableCount = 1000
    securityLimits.MaxXmlElementDepth = 128

    wordProcessor.LoadDocument("Documents\Sample.docx")
End Using
```

**Handle violations** — the `SecurityLoadingLimitExceeded` event fires when a limit is exceeded. Set `e.Handled = false` (default) to abort loading; `e.Handled = true` to continue (log-only mode):

```csharp
wordProcessor.SecurityLoadingLimitExceeded += (sender, e) => {
    Console.WriteLine($"Limit exceeded: {e.PropertyName}");
    e.Handled = false; // abort loading
};
```

### Configurable Limits

| Property | Description |
|----------|-------------|
| `MaxFileSize` | Maximum file size in bytes |
| `MaxParagraphCount` | Maximum number of paragraphs |
| `MaxTableCount` | Maximum number of tables |
| `MaxXmlElementDepth` | Maximum XML nesting depth |
| `MaxXmlElementCount` | Maximum total XML elements |
| `MaxTableRowCount` | Maximum rows across all tables |
| `MaxSectionCount` | Maximum sections |
| `MaxSubDocumentCount` | Maximum embedded sub-documents |

## Remove Dangerous Content

Strip macros, OLE objects, ActiveX, external images, and dangerous hyperlinks during loading. Set `e.Handled = false` in the violation event handler to remove detected content.

```csharp
using DevExpress.XtraRichEdit;

var wordProcessor = new RichEditDocumentServer();
WordProcessingSecurityLoadingOptions securityLoadingOptions =
    wordProcessor.Options.SecurityLoadingOptions;

securityLoadingOptions.RestrictedHyperlinkRemovalMode = RestrictedHyperlinkRemovalMode.Full;
securityLoadingOptions.RemoveRestrictedLinks = true;
securityLoadingOptions.RemoveExternalImages = true;
securityLoadingOptions.RemoveOleObjects = true;
securityLoadingOptions.RemoveActiveXContent = true;
securityLoadingOptions.RemoveMacros = true;
securityLoadingOptions.RemoveDDEFields = true;
securityLoadingOptions.RemoveIncludePictureFields = true;
securityLoadingOptions.RemoveCustomXMLParts = true;

wordProcessor.SecurityLoadingOptionsViolation += (sender, e) => {
    Console.WriteLine($"Dangerous content found: {e.PropertyName}");
    e.Handled = false; // false = remove the content
};

wordProcessor.LoadDocument("external_submission.docm");
```

```vb
Imports DevExpress.XtraRichEdit

Dim wordProcessor As New RichEditDocumentServer()
Dim securityLoadingOptions As WordProcessingSecurityLoadingOptions =
    wordProcessor.Options.SecurityLoadingOptions

securityLoadingOptions.RestrictedHyperlinkRemovalMode = RestrictedHyperlinkRemovalMode.Full
securityLoadingOptions.RemoveRestrictedLinks = True
securityLoadingOptions.RemoveExternalImages = True
securityLoadingOptions.RemoveOleObjects = True
securityLoadingOptions.RemoveActiveXContent = True
securityLoadingOptions.RemoveMacros = True
securityLoadingOptions.RemoveDDEFields = True
securityLoadingOptions.RemoveIncludePictureFields = True
securityLoadingOptions.RemoveCustomXMLParts = True

AddHandler wordProcessor.SecurityLoadingOptionsViolation, Sub(sender, e)
    Console.WriteLine($"Dangerous content found: {e.PropertyName}")
    e.Handled = False ' False = remove the content
End Sub

wordProcessor.LoadDocument("external_submission.docm")
```

## Sanitize Private Information

Remove personal metadata, revision history, and hidden content from an already-loaded document before sharing or archiving.

```csharp
using DevExpress.XtraRichEdit;

var wordProcessor = new RichEditDocumentServer();
wordProcessor.LoadDocument("submission.docx");

WordProcessingSanitizeOptions sanitizeOptions = new WordProcessingSanitizeOptions() {
    Metadata = MetadataRemovalScope.All,
    TrackedChanges = TrackedChangesSanitizeMode.Accept,
};

IList<WordProcessingSanitizeResult> findings = wordProcessor.Sanitize(sanitizeOptions);
Console.WriteLine($"{findings.Count} finding(s) removed.");
wordProcessor.SaveDocument("submission_clean.docx", DocumentFormat.OpenXml);
```

```vb
Imports DevExpress.XtraRichEdit

Dim wordProcessor As New RichEditDocumentServer()
wordProcessor.LoadDocument("submission.docx")

Dim sanitizeOptions As New WordProcessingSanitizeOptions() With {
    .Metadata = MetadataRemovalScope.All,
    .TrackedChanges = TrackedChangesSanitizeMode.Accept
}

Dim findings As IList(Of WordProcessingSanitizeResult) = wordProcessor.Sanitize(sanitizeOptions)
Console.WriteLine($"{findings.Count} finding(s) removed.")
wordProcessor.SaveDocument("submission_clean.docx", DocumentFormat.OpenXml)
```

### WordProcessingSanitizeOptions Properties

| Property | Type | Description |
|----------|------|-------------|
| `Metadata` | `MetadataRemovalScope` | `All` = clear all document properties; `None` = skip |
| `TrackedChanges` | `TrackedChangesSanitizeMode` | `Accept` / `Reject` all revisions and strip markup |
| `RemoveComments` | `bool` | Remove all comments including threaded replies |
| `HiddenText` | `InvisibleContentSanitizeMode` | `Remove` hidden text runs; `MakeVisible` to expose them |
| `InvisibleText` | `InvisibleContentSanitizeMode` | Handle text with matching foreground/background color |

## Inspect Before Sanitizing

Inspect to discover what is present without modifying anything. Use results to build targeted sanitize options.

```csharp
var wordProcessor = new RichEditDocumentServer();
wordProcessor.LoadDocument("submission.docm");

WordProcessingInspectResult inspectResult =
    wordProcessor.Inspect(WordProcessingInspectOptions.All);
Console.WriteLine($"Detected: {string.Join(", ", inspectResult.ContentTypes)}");

// Build options targeting only what was found
WordProcessingSanitizeOptions sanitizeOptions = inspectResult.CreateSanitizeOptions();
var findings = wordProcessor.Sanitize(sanitizeOptions);
Console.WriteLine($"{findings.Count} finding(s) removed.");
wordProcessor.SaveDocument("submission_clean.docx", DocumentFormat.OpenXml);
```

## Compliance Context

| Regulation | Applicable Feature |
|------------|-------------------|
| GDPR Art. 5 | `Metadata = MetadataRemovalScope.All` removes author names, paths, timestamps |
| HIPAA | `RemoveMacros`, `RemoveOleObjects` prevent malware; sanitize removes PHI in tracked changes |
| SOX § 404 | `Sanitize()` returns a structured findings list suitable for audit trail requirements |
