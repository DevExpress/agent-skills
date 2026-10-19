# Search and Filtering

`AccordionControl` ships with a built-in search panel and also integrates with the standalone `SearchControl` editor. When search is active, only elements whose text matches the query are shown; parent groups of matching items are also shown to preserve context.

## When to Use This Reference

- Enabling the built-in search / filter panel inside the accordion
- Connecting an external `SearchControl` on the form
- Configuring search behavior (visibility, delay, custom filter logic)
- Using the AI-powered Smart Search extension

## Built-in Search Panel

The `AccordionControl.ShowFilterControl` property (`ShowFilterControl` enum) controls the embedded search box:

| Value | Behavior |
|---|---|
| `Never` | Search panel is hidden (default) |
| `Always` | Search panel is permanently visible below the accordion header |
| `Auto` | Hidden; end users press **Ctrl+F** to show it, **Esc** to hide it |

```csharp
// Always-visible search bar
accordionControl1.ShowFilterControl = ShowFilterControl.Always;

// Keyboard-activated (Ctrl+F / Esc)
accordionControl1.ShowFilterControl = ShowFilterControl.Auto;
```

### How It Filters

When the user types in the search box:
- Elements whose `Text` contains the query string are shown.
- If a matching item belongs to a group, that group is also shown (even if the query does not match the group's own text).
- If a group matches, it is shown along with all its children.
- Non-matching elements are hidden.

The control restores the original element visibility when the search box is cleared.

### Filter Delay

To reduce the number of filter operations while the user is still typing, set a delay (in milliseconds):

```csharp
// Wait 300 ms after the last keystroke before applying the filter
accordionControl1.FilterDelay = 300;
```

> The `FilterDelay` property applies only to the **built-in** search panel. It has no effect when you assign a custom external `FilterControl`.

## External SearchControl

Place a `SearchControl` anywhere on the form and link it to the accordion. This lets you position the search box in the toolbar, above the accordion, or anywhere else:

```csharp
// Drop SearchControl onto the form, then link it to the accordion
searchControl1.Client = accordionControl1;
```

The `SearchControl` then drives the accordion's filter — the same filter logic applies (text match, parent-group visibility). Setting `FilterDelay` has no effect when an external `SearchControl` is the active client.

## Custom Filter Control

To replace the built-in filter UI with your own control, assign an object that implements `DevExpress.XtraBars.Navigation.IFilterContent` to the `AccordionControl.FilterControl` property:

```csharp
// FilterControl is typed as IFilterContent.
accordionControl1.FilterControl = myCustomFilterControl;
```

> The `AccordionControl.FilterText` property (which gets/sets the active filter text) is only in effect when your custom control implements `DevExpress.XtraBars.Navigation.IFilterContentEx` rather than the base `IFilterContent`. With a plain `IFilterContent`, `FilterText` is not supported.

The exact member surface of `IFilterContent` / `IFilterContentEx` is not part of the public reference pages — verify it before implementing. A worked sample is published as the DevExpress example *"How to create a custom filter control for the AccordionControl."* Search MCP for the current contract: `devexpress_docs_search(technologies=["WindowsForms"], question="AccordionControl IFilterContentEx custom filter control")`.

### Filtering an element regardless of the query

If you only need to force-show or force-hide individual elements (rather than replace the whole filter UI), handle the `AccordionControl.FilterContent` event. The `FilterValue` argument carries the query, `Element` is the current element, set `Visible` to show/hide it, and set `Handled = true` for your decision to take effect:

```csharp
accordionControl1.FilterContent += (s, e) => {
    if (e.Element.Text?.ToLower() == "help") {
        e.Visible = true;   // always show "Help"
        e.Handled = true;
    }
};
```

## AI-Powered Smart Search

DevExpress ships an optional AI extension that enhances accordion search with semantic understanding — it handles synonyms, misspellings, and intent beyond exact keyword matches.

When the user pauses typing, the control sends the query to an AI service (`IChatClient`). When results arrive, the accordion filters its items accordingly.

> There is no `UseAISmartSearch` property on `AccordionControl`. Smart Search is enabled by attaching a `DevExpress.AIIntegration.WinForms.SmartSearchBehavior` to the accordion (via a `BehaviorManager`), after an `IChatClient` has been registered with the AI container.

### Requirements

- The `DevExpress.AIIntegration.WinForms` package (and `DevExpress.Win.Design` for design-time support).
- A registered `IChatClient` (OpenAI, Azure OpenAI, Ollama, etc.).

### Setup

Step 1 — register the AI client once at application startup (before any form is shown):

```csharp
using Microsoft.Extensions.AI;
using DevExpress.AIIntegration;

// Example: Azure OpenAI
IChatClient chatClient = new Azure.AI.OpenAI.AzureOpenAIClient(
        new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey))
    .GetChatClient(modelId).AsIChatClient();
AIExtensionsContainerDesktop.Default.RegisterChatClient(chatClient);
// On .NET Framework, when creating AI behaviors in code, also call:
// DevExpress.AIIntegration.WinForms.BehaviorInitializer.Initialize();
```

Step 2 — attach a `SmartSearchBehavior` to the accordion. Drop a `BehaviorManager` onto the form (or create one), then attach the behavior. Item descriptions are optional but improve accuracy (each `AIItemDescription` pairs an `AccordionControlElement` with a description string):

```csharp
using DevExpress.AIIntegration.WinForms;

accordionControl1.ShowFilterControl = ShowFilterControl.Always;

behaviorManager1.Attach<SmartSearchBehavior>(accordionControl1, behavior => {
    behavior.Properties.ItemDescriptions.AddRange(new AIItemDescription[] {
        new AIItemDescription(itmDashboard, "Opens the main analytics dashboard."),
        new AIItemDescription(itmReports,   "Generates and views reports."),
    });
});
```

> Verify the current `SmartSearchBehavior` / `AIItemDescription` member surface via MCP before use: `devexpress_docs_search(technologies=["WindowsForms"], question="Accordion SmartSearchBehavior attach BehaviorManager ItemDescriptions")`. Refer to the `devexpress-ai-smart-search` skill or the official AI integration docs for full setup instructions.

## Keyboard Shortcuts

| Key | Action |
|---|---|
| **Ctrl+F** | Open search panel (when `ShowFilterControl = Auto`) |
| **Esc** | Close search panel and clear the filter |
| **Up/Down arrow** | Navigate filtered results |

## Key API Summary

| API | Description |
|---|---|
| `AccordionControl.ShowFilterControl` | `Never` / `Always` / `Auto` |
| `AccordionControl.FilterDelay` | Milliseconds delay before filter applies (built-in panel only) |
| `AccordionControl.FilterControl` | Custom `IFilterContent` implementation (use `IFilterContentEx` for `FilterText`) |
| `AccordionControl.FilterText` | Active filter text (effective with an `IFilterContentEx` filter control) |
| `AccordionControl.FilterContent` (event) | Show/hide an element regardless of the query |
| `SmartSearchBehavior` (`DevExpress.AIIntegration.WinForms`) | Attach via `BehaviorManager` to enable AI-powered Smart Search |
| `SearchControl.Client` | Link external `SearchControl` to the accordion |

## Common Issues

| Symptom | Cause | Solution |
|---|---|---|
| Search panel not appearing | `ShowFilterControl = Never` | Set to `Always` or `Auto` |
| Filter delay ignored | Using external `SearchControl` | `FilterDelay` only affects built-in panel |
| Groups disappear even though a child matches | Custom `IFilterContent` returns `false` for groups | Return `true` for `ElementStyle.Group` elements always, or check children |
| AI search not triggering | `IChatClient` not registered, or behavior not attached | Call `AIExtensionsContainerDesktop.Default.RegisterChatClient(...)` at startup and attach a `SmartSearchBehavior` to the accordion |
| Clearing search does not restore all items | Custom filter not resetting its text | Clear the search box (or reset `FilterText` when using an `IFilterContentEx` control) |
