# AI Columns

## Overview

AI columns add AI-generated insights to DataGrid rows. They are a special column type (`type: 'ai'`) that calls an LLM with per-row data and displays the response. AI columns are read-only — values are not stored in the data source and cannot be sorted, filtered, or exported.

**Requirements:**
- `keyExpr` must be set on the DataGrid (or a key must be configured on the Store). Without a key, the grid cannot match AI responses to rows.
- Import the `dx.ai-integration.js` module (jQuery) or the framework-specific AI integration package.
- Configure an `AIIntegration` object and pass it to `aiIntegration`.

---

## Setup: Import the AI Integration Module

### jQuery

```html
<!-- index.html <head> -->
<script src="node_modules/devextreme/dist/js/dx.ai-integration.js"></script>
<!-- or CDN -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/devextreme-dist/{version}/js/dx.ai-integration.js"></script>
```

### Angular

```ts
import { AIIntegration } from 'devextreme-angular/common/ai-integration';
```

### Vue

```ts
import { AIIntegration } from 'devextreme-vue/common/ai-integration';
```

### React

```ts
import { AIIntegration } from 'devextreme-react/common/ai-integration';
```

---

## Configure `AIIntegration`

`AIIntegration` takes a provider object with a `sendRequest` function. This function calls your AI backend and returns a `{ promise, abort }` object.

```ts
// TypeScript — shared across frameworks
const provider = {
    sendRequest({ prompt }: { prompt: string }) {
        const controller = new AbortController();
        const promise = fetch('https://my-backend.com/ai', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ prompt }),
            signal: controller.signal
        })
            .then((r) => r.json())
            .then((data) => data.output as string);

        return {
            promise,
            abort: () => controller.abort()
        };
    }
};

const aiIntegration = new AIIntegration(provider);
```

> Route AI requests through your own backend. Never expose API keys in client-side code.

---

## Adding an AI Column

### jQuery

```js
$('#grid').dxDataGrid({
    dataSource: products,
    keyExpr: 'id',
    aiIntegration: aiIntegration,
    columns: [
        { dataField: 'name', caption: 'Product' },
        { dataField: 'description', caption: 'Description' },
        {
            type: 'ai',
            name: 'summary',
            caption: 'AI Summary',
            ai: {
                mode: 'auto',                           // 'auto' | 'manual'
                prompt: 'Summarize this product in one sentence.',
                noDataText: 'No summary available'
            },
            width: 250
        }
    ]
});
```

### Angular

```html
<dx-data-grid
    [dataSource]="products"
    keyExpr="id"
    [aiIntegration]="aiIntegration">
    <dxi-data-grid-column dataField="name" caption="Product"></dxi-data-grid-column>
    <dxi-data-grid-column dataField="description" caption="Description"></dxi-data-grid-column>
    <dxi-data-grid-column
        type="ai"
        name="summary"
        caption="AI Summary"
        [ai]="aiColumnOptions"
        [width]="250">
    </dxi-data-grid-column>
</dx-data-grid>
```

```ts
import { Component } from '@angular/core';
import { DxDataGridComponent } from 'devextreme-angular/ui/data-grid';
import { AIIntegration } from 'devextreme-angular/common/ai-integration';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxDataGridComponent]
})
export class AppComponent {
    aiIntegration = new AIIntegration(provider);
    aiColumnOptions = {
        mode: 'auto',
        prompt: 'Summarize this product in one sentence.',
        noDataText: 'No summary available'
    };
}
```

### Vue

```vue
<template>
    <DxDataGrid
        :data-source="products"
        key-expr="id"
        :ai-integration="aiIntegration">
        <DxColumn data-field="name" caption="Product" />
        <DxColumn data-field="description" caption="Description" />
        <DxColumn
            type="ai"
            name="summary"
            caption="AI Summary"
            :ai="aiColumnOptions"
            :width="250"
        />
    </DxDataGrid>
</template>

<script setup lang="ts">
import { DxDataGrid, DxColumn } from 'devextreme-vue/data-grid';
import { AIIntegration } from 'devextreme-vue/common/ai-integration';

const aiIntegration = new AIIntegration(provider);
const aiColumnOptions = {
    mode: 'auto' as const,
    prompt: 'Summarize this product in one sentence.',
    noDataText: 'No summary available'
};
</script>
```

### React

```tsx
import { DataGrid, Column } from 'devextreme-react/data-grid';
import { AIIntegration } from 'devextreme-react/common/ai-integration';

const aiIntegration = new AIIntegration(provider);
const aiColumnOptions = {
    mode: 'auto' as const,
    prompt: 'Summarize this product in one sentence.',
    noDataText: 'No summary available'
};

function App() {
    return (
        <DataGrid dataSource={products} keyExpr="id" aiIntegration={aiIntegration}>
            <Column dataField="name" caption="Product" />
            <Column dataField="description" caption="Description" />
            <Column
                type="ai"
                name="summary"
                caption="AI Summary"
                ai={aiColumnOptions}
                width={250}
            />
        </DataGrid>
    );
}
```

---

## `ai` Column Options

| Option | Type | Description |
|---|---|---|
| `mode` | `'auto' \| 'manual'` | `'auto'`: loads on render. `'manual'`: user triggers per row. |
| `prompt` | string | Instruction sent to the AI along with the row data |
| `noDataText` | string | Text shown when the AI returns an empty response |
| `aiIntegration` | `AIIntegration` | Per-column AI settings (overrides the grid-level `aiIntegration`) |

---

## Limiting Data Sent to the AI (`onAIColumnRequestCreating`)

By default, the AI request includes **all fields from the visible rows** — including hidden columns and unbound fields. Use `onAIColumnRequestCreating` to restrict which fields are sent:

### jQuery

```js
$('#grid').dxDataGrid({
    onAIColumnRequestCreating(e) {
        e.data = e.data.map((item) => ({
            id: item.id,
            name: item.name,
            description: item.description
            // omit price, internalCode, etc.
        }));
    }
});
```

### Angular

```html
<dx-data-grid (onAIColumnRequestCreating)="onAIColumnRequestCreating($event)">
</dx-data-grid>
```

```ts
import type { DxDataGridTypes } from 'devextreme-angular/ui/data-grid';

onAIColumnRequestCreating(e: DxDataGridTypes.AIColumnRequestCreatingEvent): void {
    e.data = e.data.map((item) => ({
        id: item.id,
        name: item.name,
        description: item.description
    }));
}
```

### Vue

```vue
<DxDataGrid @ai-column-request-creating="onAIColumnRequestCreating" />
```

```ts
import type { DxDataGridTypes } from 'devextreme-vue/data-grid';

function onAIColumnRequestCreating(e: DxDataGridTypes.AIColumnRequestCreatingEvent): void {
    e.data = e.data.map((item) => ({
        id: item.id,
        name: item.name,
        description: item.description
    }));
}
```

### React

```tsx
import type { DataGridTypes } from 'devextreme-react/data-grid';

function onAIColumnRequestCreating(e: DataGridTypes.AIColumnRequestCreatingEvent): void {
    e.data = e.data.map((item) => ({
        id: item.id,
        name: item.name,
        description: item.description
    }));
}

<DataGrid onAIColumnRequestCreating={onAIColumnRequestCreating} />
```

---

## Limitations

- AI columns cannot be placed inside band columns.
- AI columns ignore many standard column options: `allowEditing`, `allowFiltering`, `allowSorting`, `allowExporting`, `dataField`, `dataType`, `format`, `groupIndex`, `lookup`, `calculateCellValue`, and others.
- AI-generated values are not persisted to the data source.

---

## Official Resources

- [AI Columns guide](https://js.devexpress.com/Documentation/Guide/UI_Components/DataGrid/Columns/Column_Types/AI_Columns/)
- [aiIntegration setup](https://js.devexpress.com/Documentation/Guide/AI_Features/aiIntegration_Setup/)
- [DataGrid demos — AI Columns](https://js.devexpress.com/Demos/WidgetsGallery/Demo/DataGrid/AIColumns/)
