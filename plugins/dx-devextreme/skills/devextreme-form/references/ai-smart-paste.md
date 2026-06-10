# AI Smart Paste

## Overview

Smart Paste lets users paste unstructured text (an email, a note, a description) into the form and have an LLM automatically populate the matching fields. A "Smart Paste" button is added to the form's `items[]` array; when clicked, it reads the clipboard, calls the AI service, and fills in the fields.

**Requirements:**
- Import the `dx.ai-integration.js` module (jQuery) or the framework-specific AI integration package.
- Configure an `AIIntegration` object and pass it to the Form's `aiIntegration` option.
- Add a `ButtonItem` with `name: 'smartPaste'` to `items[]`.

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

```ts
// TypeScript — shared provider pattern
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

        return { promise, abort: () => controller.abort() };
    }
};

const aiIntegration = new AIIntegration(provider);
```

> Route AI requests through your own backend. Never expose API keys in client-side code.

---

## Adding Smart Paste to the Form

### jQuery

```js
const aiIntegration = new DevExpress.aiIntegration.AIIntegration(provider);

$('#form').dxForm({
    formData: { name: '', email: '', phone: '', company: '' },
    aiIntegration: aiIntegration,
    items: [
        { dataField: 'name',    label: { text: 'Name' } },
        { dataField: 'email',   label: { text: 'Email' } },
        { dataField: 'phone',   label: { text: 'Phone' } },
        { dataField: 'company', label: { text: 'Company' } },
        {
            itemType: 'button',
            name: 'smartPaste'  // reserved name — renders the Smart Paste button
        }
    ]
});
```

### Angular

```html
<!-- app.html -->
<dx-form
    [formData]="formData"
    [aiIntegration]="aiIntegration"
    (onSmartPasted)="onSmartPasted($event)">
    <dxi-form-simple-item dataField="name"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="email"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="phone"></dxi-form-simple-item>
    <dxi-form-simple-item dataField="company"></dxi-form-simple-item>
    <dxi-form-button-item name="smartPaste"></dxi-form-button-item>
</dx-form>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { AIIntegration } from 'devextreme-angular/common/ai-integration';
import {
    DxFormComponent,
    DxiFormSimpleItemComponent,
    DxiFormButtonItemComponent,
    DxFormTypes
} from 'devextreme-angular/ui/form';
import notify from 'devextreme/ui/notify';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    standalone: true,
    imports: [DxFormComponent, DxiFormSimpleItemComponent, DxiFormButtonItemComponent]
})
export class AppComponent {
    formData = { name: '', email: '', phone: '', company: '' };
    aiIntegration = new AIIntegration(provider);

    onSmartPasted(e: DxFormTypes.SmartPastedEvent): void {
        if (Object.keys(e.aiResult).length > 0) {
            notify('Smart Paste completed successfully', 'success', 2000);
        } else {
            notify('No data could be extracted', 'warning', 2000);
        }
    }
}
```

### Vue

```vue
<template>
    <DxForm
        :form-data="formData"
        :ai-integration="aiIntegration"
        @smart-pasted="onSmartPasted">
        <DxSimpleItem data-field="name" />
        <DxSimpleItem data-field="email" />
        <DxSimpleItem data-field="phone" />
        <DxSimpleItem data-field="company" />
        <DxButtonItem name="smartPaste" />
    </DxForm>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import { DxForm, DxSimpleItem, DxButtonItem } from 'devextreme-vue/form';
import { AIIntegration } from 'devextreme-vue/common/ai-integration';
import type { DxFormTypes } from 'devextreme-vue/form';
import notify from 'devextreme/ui/notify';

const formData = reactive({ name: '', email: '', phone: '', company: '' });
const aiIntegration = new AIIntegration(provider);

function onSmartPasted(e: DxFormTypes.SmartPastedEvent): void {
    if (Object.keys(e.aiResult).length > 0) {
        notify('Smart Paste completed successfully', 'success', 2000);
    } else {
        notify('No data could be extracted', 'warning', 2000);
    }
}
</script>
```

### React

```tsx
import { useRef } from 'react';
import Form, { SimpleItem, ButtonItem } from 'devextreme-react/form';
import { AIIntegration } from 'devextreme-react/common/ai-integration';
import type { FormTypes } from 'devextreme-react/form';
import notify from 'devextreme/ui/notify';

const aiIntegration = new AIIntegration(provider);

const formData = { name: '', email: '', phone: '', company: '' };

function App() {
    const onSmartPasted = (e: FormTypes.SmartPastedEvent) => {
        if (Object.keys(e.aiResult).length > 0) {
            notify('Smart Paste completed successfully', 'success', 2000);
        } else {
            notify('No data could be extracted', 'warning', 2000);
        }
    };

    return (
        <Form
            formData={formData}
            aiIntegration={aiIntegration}
            onSmartPasted={onSmartPasted}>
            <SimpleItem dataField="name" />
            <SimpleItem dataField="email" />
            <SimpleItem dataField="phone" />
            <SimpleItem dataField="company" />
            <ButtonItem name="smartPaste" />
        </Form>
    );
}
```

---

## Smart Paste Events

| Event | Description |
|---|---|
| `onSmartPasting` | Fires before the AI populates the form. Set `e.cancel = true` to abort. |
| `onSmartPasted` | Fires after the AI populates the form. `e.aiResult` contains the populated field data. |

### Inspecting the Result (`onSmartPasted`)

```ts
onSmartPasted(e: any): void {
    console.log('AI populated:', e.aiResult);
    // e.aiResult is a plain object: { name: 'Jane Doe', email: 'jane@example.com', ... }
}
```

### Invoking Smart Paste Programmatically

You can trigger Smart Paste without the button by calling `smartPaste(text)` on the Form instance:

```ts
// jQuery
const form = $('#form').dxForm('instance');
form.smartPaste('Jane Doe, jane@example.com, Acme Corp');
```

```ts
// Angular
@ViewChild(DxFormComponent) formRef!: DxFormComponent;
this.formRef.instance.smartPaste('Jane Doe, jane@example.com, Acme Corp');
```

```ts
// Vue
const formRef = ref();
formRef.value.instance.smartPaste('Jane Doe, jane@example.com, Acme Corp');
```

```ts
// React — via onInitialized
import { useRef, useCallback } from 'react';

function App() {
    const formInstance = useRef<any>(null);
    const handleInitialized = useCallback((e) => { formInstance.current = e.component; }, []);

    function triggerSmartPaste() {
        formInstance.current?.smartPaste('Jane Doe, jane@example.com, Acme Corp');
    }

    return <Form onInitialized={handleInitialized} />;
}
```

---

## Official Resources

- [Smart Paste demo](https://js.devexpress.com/Demos/WidgetsGallery/Demo/Form/SmartPaste/)
- [dxForm `aiIntegration` API](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxForm/Configuration/#aiIntegration)
- [AIIntegration setup](https://js.devexpress.com/Documentation/Guide/AI_Features/aiIntegration_Setup/)
