# Getting Started — WinForms AIChatControl (DevExpress v26.1)

> **.NET Framework?** `AIChatControl` is **.NET 8+ only**. See [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md) for why and for the alternative approach.

## Requirements

| Requirement | Detail |
|---|---|
| Target framework | **.NET 8+** only (no .NET Framework support) |
| Project SDK | `Microsoft.NET.Sdk.Razor` |
| Subscription | Universal, DXperience, or ASP.NET & Blazor |
| Runtime dependency | WebView2 (built into Windows 11; must be distributed for Windows 10 / Server) |

---

## Step 1 — Install DevExpress NuGet Packages

### AI Chat Control packages

```
DevExpress.AIIntegration.WinForms.Chat   ← the chat control itself
DevExpress.Win.Design                    ← design-time support (recommended)
```

Optional — add if you use the **OpenAI Assistants API** (chat with your own data):

```
DevExpress.AIIntegration.OpenAI
```

### Change Project SDK

Open the `.csproj` file and change the first line:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
```

---

## Step 2 — Install the AI Provider NuGet Package

Choose one AI provider and install its NuGet package(s):

### Option A — OpenAI

```
OpenAI (≥ 2.2.0)
Microsoft.Extensions.AI.OpenAI (≥ 9.7.1-preview.1.25365.4)
```

```csharp
using Microsoft.Extensions.AI;
using DevExpress.AIIntegration;

IChatClient chatClient = new OpenAI.OpenAIClient(
    Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
```

### Option B — Azure OpenAI

```
Azure.AI.OpenAI (≥ 2.2.0-beta.5)
Microsoft.Extensions.AI.OpenAI (≥ 9.7.1-preview.1.25365.4)
```

```csharp
using Microsoft.Extensions.AI;
using DevExpress.AIIntegration;

IChatClient chatClient = new Azure.AI.OpenAI.AzureOpenAIClient(
    new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
    new System.ClientModel.ApiKeyCredential(
        Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY")))
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
```

### Option C — Ollama (self-hosted, local)

```
OllamaSharp
```

```csharp
using OllamaSharp;
using Microsoft.Extensions.AI;
using DevExpress.AIIntegration;

IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434/"), "llama3.2");

AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
```

### Option D — Semantic Kernel (Google, Anthropic, etc.)

```
Microsoft.SemanticKernel
Microsoft.SemanticKernel.Connectors.<Provider>
```

```csharp
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using DevExpress.AIIntegration;

var kernel = Kernel.CreateBuilder()
    .AddGoogleAIGeminiChatCompletion("MODEL_ID", "API_KEY", GoogleAIVersion.V1_Beta)
    .Build();

IChatClient chatClient = kernel
    .GetRequiredService<IChatCompletionService>()
    .AsChatClient();

AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
```

---

## Step 3 — Register the AI Client in Program.cs

Registration must happen **before** `Application.Run()`:

```csharp
using Microsoft.Extensions.AI;
using DevExpress.AIIntegration;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // --- Register AI client here ---
        IChatClient chatClient = new OpenAI.OpenAIClient(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
            .GetChatClient("gpt-4o-mini")
            .AsIChatClient();
        AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
        // --------------------------------

        Application.Run(new Form1());
    }
}
```

---

## Step 4 — Add AIChatControl to a Form

```csharp
using DevExpress.AIIntegration.WinForms.Chat;
using DevExpress.XtraEditors;

public partial class ChatForm : XtraForm
{
    public ChatForm()
    {
        InitializeComponent();

        var chat = new AIChatControl
        {
            Name = "aiChatControl1",
            Dock = DockStyle.Fill
        };
        Controls.Add(chat);
    }
}
```

---

## Key Configuration Properties

| Property | Type | Purpose |
|---|---|---|
| `UseStreaming` | `DefaultBoolean` | Stream responses token-by-token instead of waiting for the full reply |
| `ContentFormat` | `ResponseContentFormat` | Set to `Markdown` to render Markdown in messages |
| `EmptyStateText` | `string` | Placeholder shown when the conversation is empty |
| `ShowHeader` | `DefaultBoolean` | Display a header bar with a title and Clear button |
| `HeaderText` | `string` | Title text in the header |
| `AllowResizeInput` | `DefaultBoolean` | Let users resize the input area |
| `FileUploadEnabled` | `DefaultBoolean` | Allow file attachments in messages |
| `ChatResponseProviderServiceKey` | `string` | Select a specific chat response provider when multiple are registered |

---

## Common Configuration Patterns

### Enable Streaming

```csharp
aiChatControl1.UseStreaming = DevExpress.Utils.DefaultBoolean.True;
```

### Show Header with Clear Button

```csharp
aiChatControl1.ShowHeader  = DevExpress.Utils.DefaultBoolean.True;
aiChatControl1.HeaderText  = "AI Assistant";
```

### Enable Markdown Rendering

> Always sanitize AI-generated HTML before rendering to prevent XSS.

```csharp
using DevExpress.AIIntegration.Blazor.Chat;
using DevExpress.AIIntegration.Blazor.Chat.WebView;
using Ganss.Xss;   // HtmlSanitizer NuGet package
using Markdig;     // Markdig NuGet package

aiChatControl1.ContentFormat = ResponseContentFormat.Markdown;
aiChatControl1.MarkdownConvert += (s, e) =>
{
    string html     = Markdown.ToHtml(e.MarkdownText);
    string safeHtml = new HtmlSanitizer().Sanitize(html);
    e.HtmlText      = (Microsoft.AspNetCore.Components.MarkupString)safeHtml;
};
```

### File Upload

```csharp
aiChatControl1.FileUploadEnabled = DevExpress.Utils.DefaultBoolean.True;
aiChatControl1.OptionsFileUpload.AllowedFileExtensions.AddRange(new[] { ".txt", ".pdf", ".png" });
aiChatControl1.OptionsFileUpload.MaxFileCount = 5;
aiChatControl1.OptionsFileUpload.MaxFileSize  = 5 * 1024 * 1024; // 5 MB
```

### Prompt Suggestions

```csharp
using DevExpress.AIIntegration.Blazor.Chat.WebView;

aiChatControl1.SetPromptSuggestions(new List<PromptSuggestion>
{
    new PromptSuggestion(
        title:  "Summarize",
        text:   "Summarize the provided text.",
        prompt: "Please summarize the following text:"),
    new PromptSuggestion(
        "Translate",
        "Translate text to English.",
        "Translate the following text to English:")
});
```

### Handle Messages Manually

Handle `MessageSending` to process a user message before it is added to chat history and sent to
the AI service (for example, to log it, strip PII, or add system instructions). Use
`e.Chat.AppendMessageAsync(text, role)` to add messages, and set `e.Cancel = true` to block the
default AI call when you answer the request yourself.

```csharp
using DevExpress.AIIntegration.Blazor.Chat.WebView;
using Microsoft.Extensions.AI;

aiChatControl1.MessageSending += async (s, e) =>
{
    // Add a system instruction before the user message is sent
    await e.Chat.AppendMessageAsync("Translate text to Spanish", ChatRole.System);

    // To bypass the built-in AI call and reply yourself:
    // e.Cancel = true;
    // await e.Chat.AppendMessageAsync("Echo from my own service", ChatRole.Assistant);
};
```

### Save and Load Chat History

```csharp
using DevExpress.AIIntegration.Blazor.Chat;

// Save
List<BlazorChatMessage> history = (List<BlazorChatMessage>)aiChatControl1.SaveMessages();

// Restore
aiChatControl1.LoadMessages(history);
```

---

## Multiple AI Clients

Register a keyed `IChatResponseProvider` (and matching `IChatClient`) per service, then assign the
provider key to each control. `IChatResponseProvider` and `AsIChatResponseProvider()` are
DevExpress APIs; `IChatClient`, `OllamaApiClient`, etc. come from the provider packages.

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Chat;   // IChatResponseProvider, AsIChatResponseProvider()

IChatClient azureClient = azureOpenAiClient.GetChatClient("gpt-4o").AsIChatClient();
IChatClient ollamaClient = new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2");

var services = new ServiceCollection();

// Azure OpenAI: keyed response provider + chat client
services.AddKeyedScoped<IChatResponseProvider>(
    "azureAiProviderServiceKey", (sp, _) => azureClient.AsIChatResponseProvider());
services.AddKeyedScoped<IChatClient>("azureAiClient", (sp, _) => azureClient);

// Ollama: keyed response provider + chat client
services.AddKeyedScoped<IChatResponseProvider>(
    "ollamaAiProviderServiceKey", (sp, _) => ollamaClient.AsIChatResponseProvider());
services.AddKeyedScoped<IChatClient>("ollamaAiClient", (sp, _) => ollamaClient);

services.AddDevExpressAIDesktop(); // required for AIChatControl with keyed services

// Assign the desired provider key to each control instance
aiChatControl1.ChatResponseProviderServiceKey = "azureAiProviderServiceKey";
aiChatControl2.ChatResponseProviderServiceKey = "ollamaAiProviderServiceKey";
```

---

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `WebView2RuntimeNotFoundException` | WebView2 not installed on the target OS | Windows 11 ships WebView2; for Windows 10 / Server, distribute the WebView2 runtime — see [MS docs](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution) |
| "No registered service of type IChatClient" | AI client not registered, or `ChatResponseProviderServiceKey` mismatch | Verify `AIExtensionsContainerDesktop.Default.RegisterChatClient()` is called before `Application.Run()`, or that `ChatResponseProviderServiceKey` matches a registered provider key |
| Build error: Razor components not found | Project SDK not updated | Change `.csproj` first line to `<Project Sdk="Microsoft.NET.Sdk.Razor">` |
| Control targets .NET Framework | `AIChatControl` requires .NET 8+ | For .NET Framework, use the native chat example with `GridControl` + `HtmlContentControl` — see [GitHub example](https://github.com/DevExpress-Examples/winforms-chat-for-net-framework) |
| Markdown not rendered | `ContentFormat` not set | Set `aiChatControl1.ContentFormat = ResponseContentFormat.Markdown` and handle `MarkdownConvert` |
| XSS risk in Markdown output | AI content injected unsanitized | Always use `HtmlSanitizer` or equivalent before assigning `e.HtmlText` |

---

## Key API Reference

| Member | Description |
|---|---|
| `AIChatControl` | The main chat control class |
| `AIExtensionsContainerDesktop.Default.RegisterChatClient(client)` | Register a default AI client (there is no `(client, key)` overload — for named/multiple providers use the keyed-DI approach below) |
| `AIChatControl.ChatResponseProviderServiceKey` | Select a named chat response provider for this control |
| `AIChatControl.UseStreaming` | Enable token streaming |
| `AIChatControl.ContentFormat` | `ResponseContentFormat.Markdown` / `PlainText` |
| `AIChatControl.MarkdownConvert` | Event to convert Markdown → HTML |
| `AIChatControl.MessageSending` | Event for manual message handling (raised before a message is sent) |
| `AIChatControl.EmptyStateText` | Placeholder when chat is empty |
| `AIChatControl.ShowHeader` / `HeaderText` | Show/configure header |
| `AIChatControl.AllowResizeInput` | Resizable input area |
| `AIChatControl.FileUploadEnabled` | File attachment button |
| `AIChatControl.OptionsFileUpload` | File upload settings (extensions, size, count) |
| `AIChatControl.SetPromptSuggestions(list)` | Display pre-set prompt buttons |
| `AIChatControl.SaveMessages()` | Get current message history |
| `AIChatControl.LoadMessages(messages)` | Restore message history |
| `AIChatControl.SetMessageTemplate(...)` | Custom Razor message template |
| `AIChatControl.SetEmptyMessageAreaTemplate(...)` | Custom empty area Razor template |
| `AIChatControlMessageSendingEventArgs.Cancel` | Set to `true` to block automatic message delivery |
| `AIChatControlMessageSendingEventArgs.Chat.AppendMessageAsync(text, role)` | Append a message (system/assistant/user) from the handler |

---

## Source

- AI Chat Control: https://docs.devexpress.com/content/WindowsForms/405218?md=true
- AI-powered Extensions (register clients, all providers): https://docs.devexpress.com/content/WindowsForms/405151?md=true
- Manage Multiple Chat Clients: https://docs.devexpress.com/content/WindowsForms/405583?md=true
- Chat with Your Own Data: https://docs.devexpress.com/content/WindowsForms/405582?md=true
- GitHub examples: https://github.com/DevExpress-Examples/devexpress-ai-chat-samples
