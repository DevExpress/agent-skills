// DevExpress AI-Powered Extensions for Office File API — Quick Start
// Demonstrates: Azure OpenAI setup + proofread a Word document + save result
//
// Prerequisites:
//   dotnet add package DevExpress.AIIntegration
//   dotnet add package DevExpress.AIIntegration.Docs
//   dotnet add package DevExpress.Document.Processor
//   dotnet add package Azure.AI.OpenAI --version 2.2.0-beta.5
//   dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
//
// Required: .NET 8+ or .NET Framework 4.7.2
// Required: Active DevExpress Universal or Office File API Subscription

using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Docs;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.Extensions.AI;
using System.Globalization;

// ============================================================
// CONFIGURATION — replace with your actual values
// Use environment variables or a secrets manager in production
// ============================================================

string azureEndpoint = "YOUR_AZURE_OPENAI_ENDPOINT";   // e.g. https://my-resource.openai.azure.com/
string azureApiKey   = "YOUR_AZURE_OPENAI_API_KEY";    // Your Azure OpenAI key
string modelName     = "gpt-4o-mini";                  // Your deployed model name

string inputDocumentPath  = "Documents/input.docx";    // Path to the Word document to proofread
string outputDocumentPath = "Documents/proofread.docx"; // Where to save the result

// ============================================================
// STEP 1 — Build an IChatClient for Azure OpenAI
// ============================================================

IChatClient chatClient = new Azure.AI.OpenAI.AzureOpenAIClient(
        new Uri(azureEndpoint),
        new System.ClientModel.ApiKeyCredential(azureApiKey))
    .GetChatClient(modelName)
    .AsIChatClient();

// ============================================================
// STEP 2 — Create the DevExpress AI extensions container
// ============================================================

AIExtensionsContainerDefault container =
    AIExtensionsContainerConsole.CreateDefaultAIExtensionContainer(chatClient);

// ============================================================
// STEP 3 — Create the document processing service
// ============================================================

IAIDocProcessingService docService = container.CreateAIDocProcessingService();

// ============================================================
// STEP 4 — Load the Word document and proofread it
// ============================================================

Console.WriteLine($"Loading document: {inputDocumentPath}");

using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument(inputDocumentPath);

    Console.WriteLine("Proofreading document (this may take a moment)...");

    // ProofreadAsync reviews spelling, grammar, punctuation, and style.
    // Changes are applied in-place. The culture specifies the language rules.
    await docService.ProofreadAsync(
        wordProcessor,
        new CultureInfo("en-US"));

    // ============================================================
    // STEP 5 — Save the proofread document
    // ============================================================

    string outputDir = Path.GetDirectoryName(outputDocumentPath)!;
    if (!string.IsNullOrEmpty(outputDir))
        Directory.CreateDirectory(outputDir);

    using (var outputStream = new FileStream(outputDocumentPath, FileMode.Create, FileAccess.Write))
    {
        wordProcessor.SaveDocument(outputStream, DocumentFormat.OpenXml);
    }

    Console.WriteLine($"Proofread document saved to: {outputDocumentPath}");
}

// ============================================================
// BONUS — Translate a specific paragraph to German
// ============================================================

Console.WriteLine("\nDemonstrating paragraph translation...");

using (var wordProcessor = new RichEditDocumentServer())
{
    wordProcessor.LoadDocument(inputDocumentPath);

    if (wordProcessor.Document.Paragraphs.Count > 1)
    {
        // Translate the second paragraph (index 1) to German
        Paragraph para = wordProcessor.Document.Paragraphs[1];
        await docService.TranslateAsync(para.Range, new CultureInfo("de-DE"));

        string translatedPath = "Documents/translated_paragraph.docx";
        wordProcessor.SaveDocument(translatedPath, DocumentFormat.Docx);
        Console.WriteLine($"Translated document saved to: {translatedPath}");
    }
    else
    {
        Console.WriteLine("Document has fewer than 2 paragraphs; skipping paragraph translation demo.");
    }
}

Console.WriteLine("\nDone.");
