# Presentation Extensions — DevExpress AI-Powered Extensions

The Presentation extensions enable AI-powered proofreading, translation, summarization, and contextual Q&A on PowerPoint presentations (`.pptx`) processed by the `Presentation` class from `DevExpress.Docs.Presentation`. Operations can target an entire presentation or a specific `Slide`. All proofreading and translation changes are applied in-place while preserving formatting.

## When to Use This Reference

Use this when you need to:
- Proofread an entire PowerPoint presentation for grammar, spelling, and style
- Proofread a specific slide without touching other slides
- Translate an entire presentation or a single slide to a target language/culture
- Summarize the content of a presentation
- Ask a contextual question about presentation content using RAG
- Understand how to load, save, and access slides via the `Presentation` API
- Preserve text formatting (fonts, sizes, colors) during AI-powered translation

## Key Classes and Types

| Class/Interface | Namespace | Purpose |
|----------------|-----------|---------|
| `IAIDocProcessingService` | `DevExpress.AIIntegration.Docs` | Main service interface with all AI extension methods |
| `Presentation` | `DevExpress.Docs.Presentation` | Loads, edits, and saves PPTX presentations |
| `Slide` | `DevExpress.Docs.Presentation` | A single slide in the presentation |
| `DocumentFormat` | `DevExpress.Docs.Presentation` | Enum for save format: `Pptx` |
| `SummarizationMode` | `DevExpress.AIIntegration.Docs` | `Abstractive` or `Extractive` |
| `RagOptions` | `DevExpress.AIIntegration.Docs` | Fine-tunes RAG behavior for `AskAIAsync` |

## Required `using` Directives

```csharp
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Docs;
using DevExpress.Docs.Presentation;
using System.Globalization;
using System.Threading;
```

## Loading a Presentation

The `Presentation` class is instantiated from raw bytes:

```csharp
// Load from file
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

// Alternatively, from a stream
byte[] bytes;
using (var fs = File.OpenRead("Documents/input.pptx"))
{
    bytes = new byte[fs.Length];
    fs.Read(bytes, 0, bytes.Length);
}
var presentation = new Presentation(bytes);
```

## Saving a Presentation

```csharp
// Save to file via FileStream
using (var outputStream = File.OpenWrite("Documents/output.pptx"))
{
    presentation.SaveDocument(outputStream, DocumentFormat.Pptx);
    outputStream.Close();
}

// Or use a MemoryStream for in-memory scenarios
using (var ms = new MemoryStream())
{
    presentation.SaveDocument(ms, DocumentFormat.Pptx);
    File.WriteAllBytes("Documents/output.pptx", ms.ToArray());
}
```

## Common Scenarios

### Proofread an Entire Presentation

Reviews all slide text for spelling, grammar, punctuation, and style. Changes are applied in-place.

```csharp
using (var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx")))
{
    // Proofread entire presentation in en-US
    await docService.ProofreadAsync(presentation, new CultureInfo("en-US"));

    // Save the corrected presentation
    string outputPath = Path.Combine(@"C:\Output", "proofread.pptx");
    Directory.CreateDirectory(@"C:\Output");
    using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    {
        presentation.SaveDocument(fs, DocumentFormat.Pptx);
    }
}
```

### Proofread a Specific Slide

Slides are accessed via `presentation.Slides[index]` (zero-based).

```csharp
using (var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx")))
{
    // Proofread only the second slide (index 1)
    Slide slide = presentation.Slides[1];
    await docService.ProofreadAsync(slide, new CultureInfo("en-US"));

    using (var fs = File.OpenWrite("Documents/proofread_slide2.pptx"))
    {
        presentation.SaveDocument(fs, DocumentFormat.Pptx);
        fs.Close();
    }
}
```

### Translate an Entire Presentation

`TranslateAsync(Presentation, CultureInfo)` translates all slide content in-place and returns `Task`.

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

await docService.TranslateAsync(presentation, new CultureInfo("de-DE"));

using (var fs = File.OpenWrite("Documents/translated_full.pptx"))
{
    presentation.SaveDocument(fs, DocumentFormat.Pptx);
    fs.Close();
}
```

### Translate a Specific Slide

`TranslateAsync(Slide, CultureInfo)` translates only the specified slide in-place.

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

// Translate the first slide (index 0) to German
await docService.TranslateAsync(presentation.Slides[0], new CultureInfo("de-DE"));

using (var outputStream = File.OpenWrite(
    Path.Combine(Environment.CurrentDirectory, "presentation_slide1_de.pptx")))
{
    presentation.SaveDocument(outputStream, DocumentFormat.Pptx);
    outputStream.Close();
}
```

### Translate Multiple Specific Slides

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

// Translate slides 0 and 2 to French
int[] slideIndexes = { 0, 2 };
foreach (int index in slideIndexes)
{
    await docService.TranslateAsync(presentation.Slides[index], new CultureInfo("fr-FR"));
}

using (var fs = File.OpenWrite("Documents/partial_translation.pptx"))
{
    presentation.SaveDocument(fs, DocumentFormat.Pptx);
    fs.Close();
}
```

### Proofread, Then Translate (Combined Workflow)

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

// Step 1: Proofread the entire presentation
await docService.ProofreadAsync(presentation, new CultureInfo("en-US"));

// Step 2: Translate the first slide to Japanese
await docService.TranslateAsync(presentation.Slides[0], new CultureInfo("ja-JP"));

using (var fs = File.OpenWrite("Documents/proofread_and_translated.pptx"))
{
    presentation.SaveDocument(fs, DocumentFormat.Pptx);
    fs.Close();
}
```

### Summarize a Presentation

`SummarizeAsync` returns the summary as a string. It does not modify the presentation.

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

string summary = await docService.SummarizeAsync(
    presentation,
    SummarizationMode.Extractive,
    CancellationToken.None);

Console.WriteLine("Presentation Summary:");
Console.WriteLine(summary);
```

**SummarizationMode options:**
- `SummarizationMode.Extractive` — selects key sentences from the original text verbatim
- `SummarizationMode.Abstractive` — rephrases content in a new concise form (AI generates new sentences)

### Ask a Contextual Question (RAG)

`AskAIAsync` uses retrieval-augmented generation to answer a natural language question about the presentation content. Returns a `string`. Requires an embedding generator and vector store registered in the AI container.

```csharp
var presentation = new Presentation(File.ReadAllBytes("Documents/input.pptx"));

string answer = await docService.AskAIAsync(
    presentation,
    "What are the main talking points in this presentation?");

Console.WriteLine(answer);
```

**With RagOptions for fine-tuning:**

```csharp
var ragOptions = new RagOptions
{
    ChunkSize = 1000,
    AugmentationChunkCount = 5,
    RebuildEmbeddings = true
};

string answer = await docService.AskAIAsync(
    presentation,
    "Which slide discusses the budget?",
    ragOptions);
```

## Accessing Slides

```csharp
// Access by zero-based index
Slide firstSlide = presentation.Slides[0];
Slide secondSlide = presentation.Slides[1];

// Get total slide count
int slideCount = presentation.Slides.Count;

// Iterate all slides
foreach (Slide slide in presentation.Slides)
{
    // Process each slide
}
```

## Formatting Preservation

Translation preserves text formatting — font family, size, color, bold, italic, and other text properties are maintained. You do not need to reapply formatting after calling `TranslateAsync`.

## Configuration Options (RagOptions for AskAI)

| Property | Default | Description |
|----------|---------|-------------|
| `ChunkSize` | `1000` | Max characters per content chunk for embedding |
| `AugmentationChunkCount` | `5` | Number of most-relevant chunks sent to the model |
| `RebuildEmbeddings` | `true` | Regenerate vector embeddings; set to `false` to reuse existing embeddings for the same unchanged presentation |
| `VectorDimensions` | `1536` | Dimensionality of vector embeddings |
| `VectorCollectionName` | `"default_collection"` | Logical name of the vector collection (index) |

## Troubleshooting

- **`Slides[n]` throws `ArgumentOutOfRangeException`**: Check `presentation.Slides.Count` before accessing by index. Slide indexing is zero-based.
- **Formatting is changed after translation**: This should not occur — `TranslateAsync` preserves formatting. If it does, verify the DevExpress package version is 25.2 or later.
- **Proofread has no visible effect on some slides**: Slides with only images or shapes containing no text will not be modified. The extension processes text content only.
- **`AskAIAsync` returns vague answers**: Ensure an embedding generator and vector store are registered in the AI container. Without them, RAG cannot retrieve relevant content chunks.
- **Operation times out on large presentations**: Process individual slides rather than the entire presentation, or increase the `CancellationToken` timeout.
- **`Presentation` constructor throws on corrupted file**: Ensure the source `.pptx` file is valid and not password-protected. Protected presentations are not supported by the AI extensions.
