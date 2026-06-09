# Features Reference — DevExpress Blazor AI Chat

## When to Use This Reference

- Adding prompt suggestions (hint bubbles)
- Adding a system prompt or preloading conversation history
- Enabling file attachments
- Configuring Markdown rendering
- Handling message events
- Understanding available `IAIChat` methods

## Prompt Suggestions

Prompt suggestions are clickable hint bubbles displayed in an empty chat before the first message. They help users start a conversation.

```razor
@using DevExpress.AIIntegration.Blazor.Chat

<DxAIChat>
    <PromptSuggestions>
        <DxAIChatPromptSuggestion Title="Tell me a joke"
                                  Text="Take a break and enjoy a quick laugh"
                                  PromptMessage="Tell me a joke."
                                  SendOnClick="true" />
        <DxAIChatPromptSuggestion Title="Summarize text"
                                  Text="Extract a quick summary (main ideas)"
                                  PromptMessage="Summarize the following text:"
                                  SendOnClick="false" />
        <DxAIChatPromptSuggestion Title="Write an email"
                                  Text="Make your text look and sound professional"
                                  PromptMessage="Format text as a formal email to a client:"
                                  SendOnClick="false" />
        <DxAIChatPromptSuggestion Title="Brainstorm ideas"
                                  Text="Get creative input for your tasks"
                                  PromptMessage="Help me brainstorm ideas for:"
                                  SendOnClick="false" />
        <DxAIChatPromptSuggestion Title="Fix my writing"
                                  Text="Avoid spelling, grammar, and style errors"
                                  PromptMessage="Proofread the following text:"
                                  SendOnClick="false" />
    </PromptSuggestions>
</DxAIChat>
```

### DxAIChatPromptSuggestion Properties

| Property | Type | Description |
|---|---|---|
| `Title` | `string` | Bold header text on the tile |
| `Text` | `string` | Secondary description |
| `PromptMessage` | `string` | The text sent to the AI model when clicked |
| `SendOnClick` | `bool` | `true` — send immediately on click; `false` (default) — place in the prompt input box for the user to edit before sending |

## System Prompt

Use the `Initialized` event to load a system prompt before the user sends their first message. The `Initialized` callback receives an `IAIChat` instance.

```razor
<DxAIChat Initialized="ChatInitialized" />

@code {
    async Task ChatInitialized(IAIChat chat) {
        chat.LoadMessages(new[] {
            new BlazorChatMessage(Microsoft.Extensions.AI.ChatRole.System,
                @"You are a friendly hiking enthusiast who helps people discover fun hikes.
                  Always ask for: location and desired intensity.
                  Provide three suggestions after receiving that information.")
        });
    }
}
```

> **Important**: `ChatRole.System` messages are not displayed in the UI — they serve as hidden instructions to the AI model.

## Preloading Conversation History

To restore a previous conversation, pass multiple `BlazorChatMessage` objects in `LoadMessages`:

```razor
@code {
    async Task ChatInitialized(IAIChat chat) {
        chat.LoadMessages(new[] {
            new BlazorChatMessage(Microsoft.Extensions.AI.ChatRole.System, "You are a helpful assistant."),
            new BlazorChatMessage(Microsoft.Extensions.AI.ChatRole.User, "Hello!"),
            new BlazorChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, "Hello! How can I help you today?"),
        });
    }
}
```

## File Attachments

Enable the file upload button and configure validation rules:

```razor
<DxAIChat FileUploadEnabled="true">
    <AIChatSettings>
        <DxAIChatFileUploadSettings MaxFileCount="2"
                                    MaxFileSize="20000"
                                    AllowedFileExtensions="@(new List<string> { ".jpg", ".png", ".pdf" })"
                                    FileTypeFilter="@(new List<string> { "image/*", "application/pdf" })" />
    </AIChatSettings>
</DxAIChat>
```

### DxAIChatFileUploadSettings Properties

| Property | Type | Description |
|---|---|---|
| `MaxFileCount` | `int` | Maximum number of files per message |
| `MaxFileSize` | `int` | Maximum file size in bytes |
| `AllowedFileExtensions` | `List<string>` | Allowed file extensions |
| `FileTypeFilter` | `List<string>` | MIME types shown in the browser file picker |

> **Note**: The AI provider must support multimodal input (images/files) for attachments to be processed. Not all models support file input.

## Markdown Rendering

By default, AI responses are displayed as plain text. To render Markdown (headers, bold, code blocks, lists):

```razor
<DxAIChat ResponseContentFormat="ResponseContentFormat.Markdown" />
```

When using Markdown, sanitize the output if you render it as HTML to prevent XSS attacks:

```razor
@using Markdig
@using Ganss.Xss

<DxAIChat ResponseContentFormat="ResponseContentFormat.Markdown">
    <MessageContentTemplate>
        <div>@ToHtml(context.Text)</div>
    </MessageContentTemplate>
</DxAIChat>

@code {
    readonly HtmlSanitizer sanitizer = new HtmlSanitizer();

    MarkupString ToHtml(string markdown) {
        string html = Markdown.ToHtml(markdown);
        return new MarkupString(sanitizer.Sanitize(html));
    }
}
```

> Requires `Markdig` and `HtmlSanitizer` NuGet packages.

## Custom Message Template

Use `MessageContentTemplate` to completely customize how messages are rendered:

```razor
<DxAIChat>
    <MessageContentTemplate>
        <div class="my-message @(context.Role == Microsoft.Extensions.AI.ChatRole.User ? "user" : "ai")">
            @context.Text
        </div>
    </MessageContentTemplate>
</DxAIChat>
```

The template context is a `BlazorChatMessage` with `Text` and `Role` properties.

## IAIChat Methods

The `Initialized` event arg exposes `IAIChat`:

| Method | Description |
|---|---|
| `LoadMessages(IEnumerable<BlazorChatMessage>)` | Loads system prompt or history — call before user interaction |
| `AppendMessageAsync(string, ChatRole, List<IAIChatMessageContextItem>?)` | Appends a message to history without triggering an AI response |
| `ShowLoadingIndicatorAsync(string?)` | Shows a loading spinner with optional text. Useful in `MessageSending` for long async pre-processing. While visible, `SendMessageAsync` has no effect until `HideLoadingIndicatorAsync` is called. |
| `HideLoadingIndicatorAsync()` | Removes the loading indicator and re-enables message sending. |

## MessageSending Event

Handle `MessageSending` to intercept user messages before they reach the AI service:

```razor
<DxAIChat MessageSending="OnMessageSending" />

@code {
    async Task OnMessageSending(MessageSendingEventArgs args) {
        // Read the user's message text
        Console.WriteLine(args.Text);

        // Append a hidden system instruction before sending
        await args.Chat.AppendMessageAsync(
            "Answer in bullet points.", Microsoft.Extensions.AI.ChatRole.System);

        // Set args.Cancel = true to block automatic delivery
        // and call chat.SendMessageAsync() manually for full control
    }
}
```

> Use cases: inject per-message system instructions, sanitize PII, log the message, or cancel and handle delivery manually with `args.Cancel = true`.

## ResponseReceived Event

Handle `ResponseReceived` to process AI responses after they arrive:

```razor
<DxAIChat ResponseReceived="OnResponseReceived" />

@code {
    void OnResponseReceived(ResponseReceivedEventArgs args) {
        // Log the response or inspect tool-call records
        Console.WriteLine($"AI replied: {args.Message.Text}");
    }
}
```

> **Timing**: When `UseStreaming="true"` (default), this event fires after the full streamed response is assembled. When `UseStreaming="false"`, it fires before the message appears in the chat.

## Manual Message Processing (via @ref)

Use `@ref` to access `AppendMessageAsync`, `SendMessageAsync`, and `SaveMessages` directly on the component:

```razor
<DxAIChat @ref="chat" />
<button @onclick="AskAI">Ask AI</button>

@code {
    DxAIChat? chat;

    async Task AskAI() {
        await chat!.SendMessageAsync("Summarize the latest changes.");
    }
}
```

| Method | Description |
|---|---|
| `AppendMessageAsync(string, ChatRole)` | Adds a message to history without triggering an AI response |
| `SendMessageAsync(string, List<IAIChatMessageContextItem>?)` | Sends a message to the AI service programmatically |
| `SaveMessages()` | Returns `IEnumerable<BlazorChatMessage>` — snapshot history for persistence |

> **Combine with MessageSending**: Set `args.Cancel = true` in `MessageSending`, then call `SendMessageAsync` with a modified message for full control over delivery.

## Save and Restore History

Persist conversation history across page navigations using `SaveMessages` and `LoadMessages`:

```razor
<DxAIChat @ref="chat" Initialized="ChatInitialized" />

@code {
    DxAIChat? chat;
    IEnumerable<BlazorChatMessage> savedHistory = Array.Empty<BlazorChatMessage>();

    async Task ChatInitialized(IAIChat iChat) {
        // Restore saved history on load
        iChat.LoadMessages(savedHistory);
    }

    void SaveHistory() {
        // Snapshot before navigating away
        savedHistory = chat!.SaveMessages();
    }
}
```

## Chat Appearance

Customize the header, empty state, and input resize:

```razor
<DxAIChat ShowHeader="true"
          HeaderText="Support Chat"
          EmptyMessageAreaText="How can I help you today?"
          AllowResizeInput="true"
          SizeMode="SizeMode.Medium" />
```

| Property | Type | Description |
|---|---|---|
| `ShowHeader` | `bool` | Shows or hides the header bar |
| `HeaderText` | `string` | Header label text |
| `EmptyMessageAreaText` | `string` | Placeholder when no messages exist |
| `AllowResizeInput` | `bool` | Lets users drag the input box taller |
| `SizeMode` | `SizeMode` | Overall component size |

## AI Resources

Use `Resources` to provide named documents or data that users can optionally attach to their messages via a picker button. Resources are pre-defined by the developer (unlike file upload, which is user-driven).

```razor
@using DevExpress.AIIntegration.Blazor.Chat

<DxAIChat Resources="resources" />

@code {
    List<AIChatResource> resources = new();

    protected override async Task OnInitializedAsync() {
        var instructions = await FileResourceProvider.GetTextResourceAsync(
            "instructions", "Support Agent", "You assist with product support.");
        var doc = await FileResourceProvider.GetTextResourceAsync(
            "api-reference.md", "API Reference", "Full API documentation.");
        resources.Add(instructions);
        resources.Add(doc);
    }
}
```

When at least one resource is defined, the chat input shows a picker. Users select which resources to attach to each message for contextual grounding.
