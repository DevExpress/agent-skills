# Getting Started — DevExpress Blazor AI Chat

## When to Use This Reference

- Setting up `DxAIChat` for the first time
- Understanding NuGet packages and DI registration
- Creating a complete minimal working example

## Prerequisites

- .NET 8.0, 9.0, or 10.0
- Interactive render mode (InteractiveServer, InteractiveWebAssembly, or InteractiveAuto)
- An AI provider account (Azure OpenAI, OpenAI, Ollama, etc.)
- DevExpress license and access to NuGet.org

## Step 0: Obtain Provider Credentials (BYOK)

`DxAIChat` follows a **Bring Your Own Key** model. Before writing any code, obtain credentials for your chosen AI provider and decide how to store them.

### Azure OpenAI
1. Open [Azure Portal](https://portal.azure.com) → create or open an **Azure OpenAI** resource.
2. Go to **Keys and Endpoint** — copy the endpoint URL and one of the keys.
3. Open **Azure OpenAI Studio** (or the Deployments section) — note the **deployment name** you want to use.

Three values are required:
```
AZURE_OPENAI_ENDPOINT   = https://<resource-name>.openai.azure.com/
AZURE_OPENAI_KEY        = <key>
AZURE_OPENAI_DEPLOYMENT = <deployment name, e.g. gpt-4o>
```

### OpenAI (non-Azure)
1. Go to [platform.openai.com](https://platform.openai.com) → **API keys** → create a key.

One value is required:
```
OPENAI_KEY = sk-<your key>
```
The model name (e.g., `"gpt-4o"`) is set directly in code.

### Ollama (self-hosted, no cloud account)
```bash
# Install Ollama on Windows
winget install Ollama.Ollama

# Pull a model
ollama pull llama3

# Ollama starts automatically; default endpoint: http://localhost:11434
# No API key required
```

### Storing Secrets Safely

Never put keys in source code or commit them to a repository.

| Approach | Setup |
|---|---|
| **User secrets (dev)** | `dotnet user-secrets init` then `dotnet user-secrets set "KEY" "value"` |
| **Environment variables** | Set in OS, CI/CD platform, or container environment |
| **Azure Key Vault** | `builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential())` |

**User secrets quick setup:**
```bash
dotnet user-secrets init

# For Azure OpenAI:
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://..."
dotnet user-secrets set "AZURE_OPENAI_KEY" "..."
dotnet user-secrets set "AZURE_OPENAI_DEPLOYMENT" "gpt-4o"

# For OpenAI:
dotnet user-secrets set "OPENAI_KEY" "sk-..."
```

## Step 1: Install NuGet Packages

Install the core packages from NuGet.org:

```bash
dotnet add package DevExpress.AIIntegration.Blazor.Chat
dotnet add package DevExpress.Blazor
```

> **Version requirement**: Use `26.1.*` or later. `IChatResponseProvider` and `.AsIChatResponseProvider()` were introduced in v26.1 and are not available in 25.2.

For Azure OpenAI:

```bash
dotnet add package Microsoft.Extensions.AI --version 9.7.1
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
dotnet add package Azure.AI.OpenAI --version 2.3.0
```

For OpenAI (non-Azure):

```bash
dotnet add package Microsoft.Extensions.AI --version 9.7.1
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.7.1-preview.1.25365.4
dotnet add package OpenAI --version 2.3.0
```

For Ollama (self-hosted):

```bash
dotnet add package Microsoft.Extensions.AI --version 9.7.1
dotnet add package OllamaSharp
```

## Step 2: Register Services in Program.cs

### Azure OpenAI

```csharp
using Azure.AI.OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

var azureClient = new AzureOpenAIClient(
    new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!),
    new ApiKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!));

IChatClient chatClient = azureClient
    .GetChatClient(Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")!)
    .AsIChatClient();

// AddChatClient registers the IChatClient.
// AddDevExpressAI() automatically creates the default IChatResponseProvider from it.
builder.Services.AddChatClient(chatClient);
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressAI();

// ... other service registrations
var app = builder.Build();
```

### OpenAI (Non-Azure)

```csharp
using OpenAI;
using Microsoft.Extensions.AI;

IChatClient chatClient = new OpenAIClient(
    Environment.GetEnvironmentVariable("OPENAI_KEY")!)
    .GetChatClient("gpt-4o")
    .AsIChatClient();

builder.Services.AddChatClient(chatClient);
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressAI();
```

### Ollama (Self-Hosted)

> **Ask the developer which model to use** before generating this code. Run `ollama list` to see installed models. Common options: `llama3`, `phi4`, `mistral`, `gemma`.

```csharp
using OllamaSharp;
using Microsoft.Extensions.AI;

// Replace "<model-name>" with the model the developer confirmed (e.g., "llama3", "phi4")
IChatClient ollamaClient = new OllamaApiClient("http://localhost:11434", "<model-name>");

builder.Services.AddChatClient(ollamaClient);
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressAI();
```

## Step 3: Register DevExpress Resources in App.razor and _Imports.razor

### App.razor

Add theme and script registration inside the `<head>` section of `Components/App.razor`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

> Without these calls, DevExpress components will appear unstyled and client-side interactivity will not work.

### _Imports.razor

Add the namespace to `Components/_Imports.razor` to avoid repeating `@using` on every page:

```razor
@using DevExpress.AIIntegration.Blazor.Chat
```

## Step 4: Add DxAIChat to a Page

> **Important**: The `DxAIChat` component requires an interactive render mode. Add `@rendermode` to the page or the component itself.

```razor
@page "/chat"
@rendermode InteractiveServer
@using DevExpress.AIIntegration.Blazor.Chat

<h1>AI Chat</h1>
<DxAIChat />
```

## Step 5: Verify the Build

```bash
dotnet build
```

Resolve any errors before proceeding. Common issues:

- Missing `@rendermode` directive → add `@rendermode InteractiveServer`
- AI does not respond → ensure `AddChatClient()` is called before `AddDevExpressAI()` in Program.cs
- License error → verify DevExpress license is registered

## Security Notes

> **Never hardcode API keys or endpoints in source code.**

Use one of the following approaches:
- **Development**: `dotnet user-secrets set "AZURE_OPENAI_KEY" "your-key"`
- **Production**: Azure Key Vault, AWS Secrets Manager, or environment variables injected by your hosting platform
- **Configuration**: `appsettings.json` with values supplied at deploy time via environment overrides

## GitHub Examples

- [Add DxAIChat to Blazor, MAUI, WPF, and WinForms](https://github.com/DevExpress-Examples/devexpress-ai-chat-samples)
- [Multi-LLM Chat with Conversation History](https://github.com/DevExpress-Examples/blazor-ai-chat-with-multiple-llm-services)
- [Function/Tool Calling](https://github.com/DevExpress-Examples/blazor-ai-chat-function-calling)
- [Tool Call Confirmation](https://github.com/DevExpress-Examples/blazor-ai-chat-confirm-tool-calls)
- [Integrate AI Chat with Model Context Protocol](https://github.com/DevExpress-Examples/blazor-ai-chat-mcp-resources)
