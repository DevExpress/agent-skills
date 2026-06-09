# AI Service Integration — DevExpress Blazor AI Chat

## When to Use This Reference

- Connecting `DxAIChat` to a specific AI provider
- Registering multiple providers and switching between them at runtime
- Building agent-based providers with `DevExpress.AIIntegration.Agents`

## Supported Providers

`DxAIChat` uses `IChatResponseProvider` (from `DevExpress.AIIntegration.Chat`) as its AI abstraction layer. Any `IChatClient` (from `Microsoft.Extensions.AI`) can be adapted to an `IChatResponseProvider` by calling `.AsIChatResponseProvider()`.

> **Breaking change (v26.1)**: `DxAIChat` no longer works with a directly registered `IChatClient`. Registering a keyed `IChatClient` via `AddKeyedChatClient` and referencing it through `ChatClientServiceKey` throws a **runtime exception**. Use `AddKeyedScoped<IChatResponseProvider>` and `ChatResponseProviderServiceKey` instead.

| Provider | NuGet Package |
|---|---|
| Azure OpenAI | `Azure.AI.OpenAI` + `Microsoft.Extensions.AI.OpenAI` |
| OpenAI | `OpenAI` + `Microsoft.Extensions.AI.OpenAI` |
| Ollama (self-hosted) | `OllamaSharp` |
| Google Gemini | `Microsoft.Extensions.AI.Google` (or provider SDK) |
| Custom / proprietary | Implement `IChatResponseProvider` directly |

## Single Provider

Register the `IChatClient` using `AddChatClient()`. `AddDevExpressAI()` automatically creates the default (non-keyed) `IChatResponseProvider` from it — no explicit `IChatResponseProvider` registration is needed:

```csharp
// Program.cs
using Microsoft.Extensions.AI;

builder.Services.AddChatClient(chatClient);
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressAI();
```

The `DxAIChat` component without `ChatResponseProviderServiceKey` uses this default.

## Multiple Providers (Runtime Switching)

Register each provider under a unique string key using `AddKeyedScoped`:

```csharp
// Program.cs
builder.Services.AddKeyedScoped<IChatResponseProvider>(
    "azure",
    (_, _) => azureOpenAiClient.AsIChatResponseProvider());

builder.Services.AddKeyedScoped<IChatResponseProvider>(
    "ollama",
    (_, _) => ollamaClient.AsIChatResponseProvider());
```

Bind the component to a provider key at runtime:

```razor
@using DevExpress.AIIntegration.Blazor.Chat

<select @onchange="ChangeProvider">
    <option value="azure">Azure OpenAI</option>
    <option value="ollama">Ollama (local)</option>
</select>

<DxAIChat ChatResponseProviderServiceKey="@selectedKey" />

@code {
    string selectedKey = "azure";

    void ChangeProvider(ChangeEventArgs e) {
        selectedKey = e.Value?.ToString() ?? "azure";
    }
}
```

## Agent-Based Providers

For chat scenarios that require persistent threads, file search, code interpretation, or vendor-agnostic AI backends, use the `DevExpress.AIIntegration.Agents` package instead of the deprecated OpenAI Assistants API.

Install the package:
```bash
dotnet add package DevExpress.AIIntegration.Agents
```

Create an `IChatResponseProvider` from a chat client using `.AsAIAgent()`, then register it:

```csharp
// Program.cs
using DevExpress.AIIntegration.Chat;
using DevExpress.AIIntegration.Agents;

var azureOpenAIChatClient = new AzureOpenAIClient(azureOpenAiEndpoint, azureOpenAiKey)
    .GetChatClient(azureOpenAiModel)
    .AsIChatClient();

// Keyed agent provider with custom instructions
builder.Services.AddKeyedScoped<IChatResponseProvider>("agentResponseProvider",
    (serviceProvider, _) => azureOpenAIChatClient
        .AsAIAgent(instructions: "You are a helpful assistant.")
        .AsIChatResponseProvider()
);

// Default (non-keyed) provider for DxAIChat without a key
builder.Services.AddChatClient(azureOpenAIChatClient);
builder.Services.AddDevExpressAI();
```

Bind to the keyed provider:

```razor
<DxAIChat ChatResponseProviderServiceKey="agentResponseProvider" />
```

> **Note**: `SetupAssistantAsync(AIAssistantOptions)` overloads are obsolete and will be removed. Do not use them in new code.

## Troubleshooting Provider Issues

| Symptom | Fix |
|---|---|
| `No service for IChatResponseProvider` | Register via `AddChatClient()` + `AddDevExpressAI()` for a single provider, or `AddKeyedScoped<IChatResponseProvider>` for keyed providers |
| `401 Unauthorized` from Azure OpenAI | Verify endpoint URI (must include `/openai/`) and API key |
| Ollama not responding | Ensure Ollama is running locally (`ollama serve`) and the model is pulled |
| Keyed provider not found | Verify the key string in `ChatResponseProviderServiceKey` matches the key used in `AddKeyedScoped` |
