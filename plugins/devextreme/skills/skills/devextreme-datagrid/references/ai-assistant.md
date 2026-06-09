# AI Assistant

## Overview

The AI Assistant is a chat-based panel that lets users ask natural-language questions about the grid data. It is configured via the `aiAssistant` option and is distinct from [AI Columns](ai-columns.md):

| Feature | AI Columns | AI Assistant |
|---|---|---|
| Purpose | Per-row LLM-generated insights | Chat panel for data exploration |
| Column type | `type: 'ai'` | N/A — toolbar button opens the panel |
| Event | `onAIColumnRequestCreating` | `onAIAssistantRequestCreating` |
| Output | Read-only column cells | Chat responses in a popup/panel |

**Requirements** (same as AI columns):
- `keyExpr` must be set (or a store key configured).
- Import the AI integration module and configure an `AIIntegration` object (see [ai-columns.md](ai-columns.md#setup-import-the-ai-integration-module)).
- Set `aiIntegration` at the component level or inside `aiAssistant`.

---

## Basic Setup

```js
// jQuery
$('#data-grid').dxDataGrid({
    keyExpr: 'id',
    aiIntegration: aiIntegration,   // or set inside aiAssistant
    aiAssistant: {
        enabled: true
    }
});
```

```html
<!-- Angular -->
<dx-data-grid key-expr="id" [ai-integration]="aiIntegration">
    <dxo-data-grid-ai-assistant [enabled]="true"></dxo-data-grid-ai-assistant>
</dx-data-grid>
```

```vue
<!-- Vue -->
<DxDataGrid key-expr="id" :ai-integration="aiIntegration">
    <DxAiAssistant :enabled="true" />
</DxDataGrid>
```

```tsx
// React
<DataGrid keyExpr="id" aiIntegration={aiIntegration}>
    <AiAssistant enabled={true} />
</DataGrid>
```

When enabled, a toolbar button opens the AI Assistant panel. The panel embeds a `dxChat` component.

---

## `aiAssistant` Options

| Option | Type | Description |
|---|---|---|
| `enabled` | `Boolean` | Enables the AI Assistant panel and toolbar button |
| `title` | `String` | Panel title (default: `'AI Assistant'`) |
| `aiIntegration` | `AIIntegration` | AI service binding — overrides the component-level `aiIntegration` for assistant requests |
| `popup` | `Object` | Options passed to the underlying dxPopup (e.g. `width`, `height`, `position`) |
| `chat` | `Object` | Options passed to the embedded dxChat component |
| `customizeResponseTitle` | `Function` | `(info) => string` — customize the title shown above each AI response |
| `customizeResponseText` | `Function` | `(info) => string` — post-process the AI response text before display |

---

## Controlling Context Sent to the AI

Use `onAIAssistantRequestCreating` to inspect and modify the context object included in the request. This is the primary way to limit or augment what data the AI service receives.

```ts
// TypeScript — shared pattern across frameworks
function onAIAssistantRequestCreating(e: DataGridTypes.AIAssistantRequestCreatingEvent) {
    e.context = {
        ...e.context,
        // Add extra context the AI should know about
        currentFilter: 'Active employees only',
        // Remove sensitive fields by omitting them from the spread
    };
}
```

`e.context` is the object sent to the AI service alongside the user's question. `e.responseSchema` is the JSON schema of the expected response shape (read-only).

### Angular

```html
<dx-data-grid (onAIAssistantRequestCreating)="onAIAssistantRequestCreating($event)">
    <dxo-data-grid-ai-assistant [enabled]="true"></dxo-data-grid-ai-assistant>
</dx-data-grid>
```

```ts
import { DxDataGridTypes } from 'devextreme-angular/ui/data-grid';

onAIAssistantRequestCreating(e: DxDataGridTypes.AIAssistantRequestCreatingEvent) {
    e.context = { ...e.context, department: this.selectedDepartment };
}
```

### Vue

```vue
<DxDataGrid @ai-assistant-request-creating="onAIAssistantRequestCreating">
    <DxAiAssistant :enabled="true" />
</DxDataGrid>
```

```ts
import { DxDataGridTypes } from 'devextreme-vue/data-grid';

function onAIAssistantRequestCreating(e: DxDataGridTypes.AIAssistantRequestCreatingEvent) {
    e.context = { ...e.context, department: selectedDepartment.value };
}
```

### React

```tsx
import { DataGrid, AiAssistant, type DataGridTypes } from 'devextreme-react/data-grid';

function onAIAssistantRequestCreating(e: DataGridTypes.AIAssistantRequestCreatingEvent) {
    e.context = { ...e.context, department: selectedDepartment };
}

function App() {
    return (
        <DataGrid onAIAssistantRequestCreating={onAIAssistantRequestCreating}>
            <AiAssistant enabled={true} />
        </DataGrid>
    );
}
```

---

## Customizing Responses

Use `customizeResponseTitle` and `customizeResponseText` to post-process what the AI returns before it is rendered in the Chat panel.

```ts
// React — module-level constants
const aiAssistantConfig = {
    enabled: true,
    customizeResponseTitle: () => 'AI Insight',
    customizeResponseText: (info) => info.responseText.trim(),
};

<DataGrid aiIntegration={aiIntegration}>
    <AiAssistant {...aiAssistantConfig} />
</DataGrid>
```

---

## Constraints

- **`aiIntegration` is required**: Without it, error E1068 is thrown. Set it either at the DataGrid level or inside `aiAssistant`.
- **`keyExpr` is required**: The AI Assistant uses row keys to build context. Without a key the panel will not work correctly.
- **React**: Define `aiAssistant` config objects and `customizeResponse*` callbacks as module-level constants or `useMemo`/`useCallback` values. Never pass them as inline JSX literals.
- **`onAIAssistantRequestCreating` vs `onAIColumnRequestCreating`**: These are separate events. `onAIColumnRequestCreating` controls per-row AI Column requests (`e.data`); `onAIAssistantRequestCreating` controls the chat panel request (`e.context`).
