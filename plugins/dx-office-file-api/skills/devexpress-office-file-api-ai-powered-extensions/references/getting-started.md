# Getting Started with DevExpress AI-Powered Extensions for Office File API

This guide walks you through setting up the DevExpress AI-Powered Extensions, registering an AI provider, and running your first document operation.

## When to Use This Reference

Use this when you need to:
- Install all required NuGet packages for Azure OpenAI, OpenAI, Ollama, or another provider
- Understand what runtime is required (.NET 8 or .NET Framework 4.7.2)
- Register an `IChatClient` and create the `AIExtensionsContainerDefault`
- Create the `IAIDocProcessingService` in a console app or ASP.NET Core / Blazor app
- Run your first proofread or translate operation
- Understand async patterns and `CancellationToken` usage

## System Requirements

- **.NET 8.0+** or **.NET Framework 4.7.2** (earlier .NET Framework versions are not supported)
- Visual Studio 2022+ or JetBrains Rider
- An active Universal Subscription or Office File API Subscription
- An AI provider account/endpoint (see provider options below)

## Step 1: Install NuGet Packages

### Core DevExpress Packages (required for all scenarios)

```bash
dotnet add package DevExpress.AIIntegration
dotnet add package DevExpress.AIIntegration.Docs
dotnet add package DevExpress.Document.Processor       # Word Processing + Spreadsheet
dotnet add package DevExpress.Docs.Presentation        # PowerPoint (required only for .pptx)
```

### AI Provider Packages (choose one)

**Azure OpenAI** (primary example in this guide):
```bash
dotnet add package Azure.AI.OpenAI --version 2.2.0-beta.5
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
```

**OpenAI** (non-Azure):
```bash
dotnet add package OpenAI --version 2.2.0
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
```

**Ollama** (self-hosted local model):
```bash
dotnet add package OllamaSharp
```

**Google Gemini, Claude, or other providers via Semantic Kernel**:
```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.<ProviderName>
```

**AI Foundry Local** (on-device, cross-platform):
```bash
dotnet add package Microsoft.AI.Foundry.Local --version 0.8.2.1
dotnet add package OpenAI --version 2.2.0
dotnet add package Microsoft.Extensions.AI --version 9.7.1
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
```

**ONNX Runtime** (local ONNX model):
```bash
dotnet add package Microsoft.ML.OnnxRuntimeGenAI
```

> **Important**: All `DevExpress.*` packages in a project must use the exact same version number. Do not mix versions.

### Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

AI-powered document operations (proofread, translate, summarize) run through the underlying `DevExpress.Document.Processor` (Word/Spreadsheet) and `DevExpress.Docs.Presentation` engines, which use a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. The SkiaSharp-based engine is enabled **automatically** on non-Windows platforms. Add `DevExpress.Drawing.Skia` for non-Windows deployment.

On Linux, also install the required native libraries: `apt-get install -y libc6 libicu-dev libfontconfig1` (Debian/Ubuntu) or `yum install -y glibc-devel libicu fontconfig` (RHEL/CentOS). If fonts render incorrectly, register them explicitly via `DXFontRepository`. See the platform-specific guides for [macOS](https://docs.devexpress.com/OfficeFileAPI/401532), [Linux](https://docs.devexpress.com/OfficeFileAPI/401441), and [Docker](https://docs.devexpress.com/OfficeFileAPI/401528).

## Step 2: Register Your AI Provider (Console App)

Create a helper method that builds an `IChatClient` for your provider and wraps it in a `AIExtensionsContainerDefault`:

### Azure OpenAI

```csharp
using DevExpress.AIIntegration;
using Microsoft.Extensions.AI;

// Read credentials from environment variables (never hardcode secrets)
string azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!;
string azureApiKey   = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY")!;
string modelName     = "gpt-4o-mini";

IChatClient client = new Azure.AI.OpenAI.AzureOpenAIClient(
        new Uri(azureEndpoint),
        new System.ClientModel.ApiKeyCredential(azureApiKey))
    .GetChatClient(modelName)
    .AsIChatClient();

AIExtensionsContainerDefault container =
    AIExtensionsContainerConsole.CreateDefaultAIExtensionContainer(client);
```

### OpenAI (non-Azure)

```csharp
using Microsoft.Extensions.AI;

string apiKey    = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
string modelName = "gpt-4o-mini";

IChatClient client = new OpenAI.OpenAIClient(apiKey)
    .GetChatClient(modelName)
    .AsIChatClient();

AIExtensionsContainerDefault container =
    AIExtensionsContainerConsole.CreateDefaultAIExtensionContainer(client);
```

### Ollama (local self-hosted)

```csharp
using OllamaSharp;
using Microsoft.Extensions.AI;

IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"))
    .AsChatClient("llama3");

AIExtensionsContainerDefault container =
    AIExtensionsContainerConsole.CreateDefaultAIExtensionContainer(client);
```

## Step 3: Register in ASP.NET Core / Blazor

For web applications, use the DI-based registration pattern instead of the console pattern:

```csharp
// Program.cs
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Docs;
using Microsoft.Extensions.AI;

var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!;
var azureApiKey   = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY")!;
var modelName     = "gpt-4o-mini";

var builder = WebApplication.CreateBuilder(args);

// Create and register the IChatClient as a singleton
IChatClient chatClient = new Azure.AI.OpenAI.AzureOpenAIClient(
        new Uri(azureEndpoint),
        new System.ClientModel.ApiKeyCredential(azureApiKey))
    .GetChatClient(modelName)
    .AsIChatClient();

builder.Services.AddSingleton(chatClient);

// Register DevExpress AI services including the document-processing extensions
builder.Services.AddDevExpressAIConsole((config) => {
    config.RegisterAIDocProcessingService();
});

var app = builder.Build();
// ...
```

Then inject `IAIDocProcessingService` into your controllers or Razor components:

```csharp
public class MyDocumentController : ControllerBase
{
    private readonly IAIDocProcessingService _docService;

    public MyDocumentController(IAIDocProcessingService docService)
    {
        _docService = docService;
    }
}
```

## Step 4: Create the Document Processing Service (Console)

```csharp
using DevExpress.AIIntegration.Docs;

// container was created in Step 3
IAIDocProcessingService docService = container.CreateAIDocProcessingService();
```

Alternatively, instantiate `AIDocProcessingService` directly:

```csharp
IAIDocProcessingService docService = new AIDocProcessingService(container);
```

## Step 5: Proofread a Word Document

```csharp
using DevExpress.AIIntegration.Docs;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Globalization;

using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Proofread entire document in en-US
    await docService.ProofreadAsync(wordProcessor, new CultureInfo("en-US"));

    // Save the proofread result
    wordProcessor.SaveDocument("Documents/proofread.docx", DocumentFormat.OpenXml);
}
```

To proofread only a specific paragraph instead:

```csharp
// Replace the ProofreadAsync call above with:
Paragraph para = wordProcessor.Document.Paragraphs[1];
await docService.ProofreadAsync(para.Range, new CultureInfo("en-US"));
```

## Step 6: Translate a Word Document

```csharp
using DevExpress.AIIntegration.Docs;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Globalization;

using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument("Documents/input.docx");

    // Translate the second paragraph to German
    Paragraph para = wordProcessor.Document.Paragraphs[1];
    await docService.TranslateAsync(para.Range, new CultureInfo("de-DE"));

    wordProcessor.SaveDocument("Documents/translated.docx", DocumentFormat.Docx);
}
```

## Notes on Async Patterns and Culture Codes

**All AI operations are async.** Every method on `IAIDocProcessingService` returns a `Task` or `Task<string>`. Always `await` them in an `async` method. For console apps on .NET 8, use a top-level `await` or mark `Main` as `async Task`.

**CancellationToken**: All methods accept an optional `CancellationToken` as the last parameter. Pass one to support graceful cancellation in long-running operations:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
await docService.ProofreadAsync(wordProcessor, new CultureInfo("en-US"), cts.Token);
```

**Culture codes** use the standard BCP 47 format. Common examples:
- English: `"en-US"`
- German: `"de-DE"`
- Spanish: `"es-ES"`
- French: `"fr-FR"`
- Japanese: `"ja-JP"`
- Chinese (Simplified): `"zh-CN"`

## What to Learn Next

- [word-processing-extensions.md](word-processing-extensions.md): All Word document operations, paragraph targeting, summarization, Ask AI
- [pdf-extensions.md](pdf-extensions.md): PDF translation (returns text), page area targeting, summarization, Ask AI
- [presentation-extensions.md](presentation-extensions.md): PowerPoint proofreading and translation by slide
