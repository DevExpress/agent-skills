# Word Processing Extensions — DevExpress AI-Powered Extensions

The Word Processing extensions enable AI-powered proofreading, translation, summarization, and contextual Q&A on Word documents (`.docx`, `.doc`, `.rtf`) processed by `RichEditDocumentServer`. Operations can target an entire document or a specific `DocumentRange` (paragraph, selection, or custom range). All changes are applied in-place before saving.

## When to Use This Reference

Use this when you need to:
- Proofread an entire Word document for grammar, spelling, and style errors
- Proofread a specific paragraph or `DocumentRange` without touching the rest of the document
- Translate an entire Word document or a specific range to a target language/culture
- Summarize a Word document (abstractive or extractive mode)
- Ask a natural language question about the content of a Word document (RAG)
- Understand how to access paragraphs and ranges via the `RichEditDocumentServer` API
- Preserve formatting during AI-powered transformations

## Key Classes and Types

| Class/Interface | Namespace | Purpose |
|----------------|-----------|---------|
| `IAIDocProcessingService` | `DevExpress.AIIntegration.Docs` | Main service interface with all AI extension methods |
| `AIDocProcessingService` | `DevExpress.AIIntegration.Docs` | Concrete implementation; use `CreateAIDocProcessingService()` to obtain |
| `RichEditDocumentServer` | `DevExpress.XtraRichEdit` | Headless Word document server (load, edit, save) |
| `DocumentRange` | `DevExpress.XtraRichEdit.API.Native` | Represents a range of content within a document |
| `Paragraph` | `DevExpress.XtraRichEdit.API.Native` | A paragraph in the document; exposes `.Range` |
| `DocumentFormat` | `DevExpress.XtraRichEdit.API.Native` | Enum for save format: `OpenXml`, `Docx`, `Rtf`, etc. |
| `SummarizationMode` | `DevExpress.AIIntegration.Docs` | `Abstractive` or `Extractive` |
| `RagOptions` | `DevExpress.AIIntegration.Docs` | Fine-tunes RAG behavior for `AskAIAsync` |

## Required `using` Directives

```csharp
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Docs;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Globalization;
using System.Threading;
```

## Common Scenarios

### Proofread an Entire Document

Reviews the full document text for spelling, grammar, punctuation, and style. Changes are applied in-place.

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Proofread entire document in en-US
    await docService.ProofreadAsync(wordProcessor, new CultureInfo("en-US"));

    // Save the corrected document
    using (var fs = new FileStream("Documents/proofread.docx", FileMode.Create, FileAccess.Write))
    {
        wordProcessor.SaveDocument(fs, DocumentFormat.OpenXml);
    }
}
```

### Proofread a Specific Paragraph

Use `wordProcessor.Document.Paragraphs[index].Range` to target a single paragraph. Indexing is zero-based.

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Proofread only the second paragraph (index 1)
    Paragraph para = wordProcessor.Document.Paragraphs[1];
    await docService.ProofreadAsync(para.Range, new CultureInfo("en-US"));

    wordProcessor.SaveDocument("Documents/proofread_para.docx", DocumentFormat.Docx);
}
```

### Translate a Specific Paragraph

`TranslateAsync(DocumentRange, CultureInfo)` translates the specified range in-place and returns `Task` (not a string).

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    Paragraph para = wordProcessor.Document.Paragraphs[1];
    await docService.TranslateAsync(para.Range, new CultureInfo("de-DE"));

    wordProcessor.SaveDocument("Documents/translated.docx", DocumentFormat.Docx);
}
```

### Translate an Entire Document

There is no single-call overload for translating an entire `RichEditDocumentServer` in one step. Translate the full document range by iterating paragraphs, or target the whole content range:

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Translate all paragraphs sequentially
    foreach (Paragraph para in wordProcessor.Document.Paragraphs)
    {
        await docService.TranslateAsync(para.Range, new CultureInfo("de-DE"));
    }

    wordProcessor.SaveDocument("Documents/translated_full.docx", DocumentFormat.Docx);
}
```

> **Note**: Translating paragraph by paragraph may produce better results than a single large range, depending on the model. Test both approaches with your content.

### Proofread and Then Translate (Combined)

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Step 1: Proofread the source text
    await docService.ProofreadAsync(wordProcessor, new CultureInfo("en-US"));

    // Step 2: Translate a specific paragraph to German
    Paragraph para = wordProcessor.Document.Paragraphs[0];
    await docService.TranslateAsync(para.Range, new CultureInfo("de-DE"));

    wordProcessor.SaveDocument("Documents/proofread_and_translated.docx", DocumentFormat.Docx);
}
```

### Summarize a Word Document

`SummarizeAsync` returns a `string` containing the AI-generated summary. It does not modify the document.

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Get extractive summary (uses original sentences)
    string summary = await docService.SummarizeAsync(
        wordProcessor,
        SummarizationMode.Extractive,
        CancellationToken.None);

    // Optionally insert the summary at the start of the document
    wordProcessor.Document.Paragraphs.Insert(wordProcessor.Document.Paragraphs[0].Range.Start);
    wordProcessor.Document.InsertText(
        wordProcessor.Document.Paragraphs[0].Range.Start,
        "Summary:\n" + summary);

    wordProcessor.SaveDocument("Documents/summarized.docx", DocumentFormat.Docx);
}
```

**SummarizationMode options:**
- `SummarizationMode.Extractive` — selects and extracts key sentences from the original text verbatim
- `SummarizationMode.Abstractive` — rephrases content in a new, concise form (AI generates new sentences)

### Ask a Contextual Question (RAG)

`AskAIAsync` uses retrieval-augmented generation (RAG) to answer a natural language question about the document. Returns a `string`. Requires an embedding generator and vector store registered in the container (see the main article for full setup).

```csharp
using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    string answer = await docService.AskAIAsync(
        wordProcessor,
        "What are the key terms defined in this document?");

    Console.WriteLine(answer);
}
```

**Configuring RAG behavior with `RagOptions`:**

```csharp
var ragOptions = new RagOptions
{
    ChunkSize = 1000,               // Max characters per content chunk (default: 1000)
    AugmentationChunkCount = 5,     // Number of most-relevant chunks to include (default: 5)
    RebuildEmbeddings = true        // Regenerate embeddings (set false to reuse after first call)
};

string answer = await docService.AskAIAsync(
    wordProcessor,
    "What is the main conclusion of this document?",
    ragOptions);
```

## Accessing Paragraphs and Ranges

```csharp
// Access by index (zero-based)
Paragraph first  = wordProcessor.Document.Paragraphs[0];
Paragraph second = wordProcessor.Document.Paragraphs[1];

// Get total paragraph count
int count = wordProcessor.Document.Paragraphs.Count;

// Get the range of a paragraph
DocumentRange range = para.Range;

// Get the document's full range
DocumentRange fullRange = wordProcessor.Document.Range;
```

## Saving Documents

`DocumentFormat` values for Word documents:

| Value | File Format |
|-------|-------------|
| `DocumentFormat.OpenXml` | `.docx` (OOXML) |
| `DocumentFormat.Docx` | `.docx` (same as OpenXml) |
| `DocumentFormat.Rtf` | `.rtf` |
| `DocumentFormat.Doc` | `.doc` (legacy) |
| `DocumentFormat.Txt` | Plain text |

```csharp
// Save to file path
wordProcessor.SaveDocument("output.docx", DocumentFormat.Docx);

// Save to stream
using (var fs = new FileStream("output.docx", FileMode.Create, FileAccess.Write))
{
    wordProcessor.SaveDocument(fs, DocumentFormat.OpenXml);
}
```

## Configuration Options (RagOptions for AskAI)

| Property | Default | Description |
|----------|---------|-------------|
| `ChunkSize` | `1000` | Max characters per content chunk for embedding |
| `AugmentationChunkCount` | `5` | Number of most-relevant chunks sent to the model |
| `RebuildEmbeddings` | `true` | Whether to regenerate vector embeddings; set to `false` after first call for the same unchanged document |
| `VectorDimensions` | `1536` | Dimensionality of vector embeddings |
| `VectorCollectionName` | `"default_collection"` | Logical name of the vector collection (index) |

## Troubleshooting

- **Operation times out on large documents**: Increase the `CancellationToken` timeout, or process the document in smaller ranges/paragraphs.
- **Formatting is lost after translation**: This should not happen — `TranslateAsync` preserves formatting. If it occurs, check that you are not re-creating the document from the string result.
- **`Paragraphs[n]` throws `ArgumentOutOfRangeException`**: Check `wordProcessor.Document.Paragraphs.Count` before accessing by index.
- **Proofread has no visible effect**: The model may consider the text already correct, or the endpoint may be rate-limiting. Check the model response directly if needed.
- **`AskAIAsync` returns generic answers**: Ensure an embedding generator and vector store are registered in the AI container — RAG requires both to function.
