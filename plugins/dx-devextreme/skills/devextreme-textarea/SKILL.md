---
name: devextreme-textarea
description: >
  Help developers use the DevExtreme TextArea component (dxTextArea) in Angular, React, Vue, and jQuery.
  Use when someone asks about TextArea configuration, multiline input, auto-resize, height limits,
  value binding, value change events, or any scenario involving dxTextArea or DxTextArea.
  Trigger phrases: "DevExtreme TextArea", "dxTextArea", "DxTextArea", "multiline input",
  "multiline text field", "textarea", "auto resize textarea", "expandable text input".
compatibility: DevExtreme 26.1+. Supports Angular 20+, React 18+, Vue 3, jQuery 3.x or 4.x.
metadata:
  author: DevExpress
  version: "26.1"
---

# DevExtreme TextArea Skill

A skill for building and configuring the DevExtreme TextArea UI component (`dxTextArea`) across Angular, React, Vue, and jQuery. TextArea is the multiline counterpart of TextBox — it shares most of its API and adds height/resize control.

## When to Use This Skill

- Creating a multiline text input (description, notes, address, comments)
- Setting a fixed height or enabling auto-resize
- Clamping auto-resize with min/max height
- Binding and reading the value
- Handling value change events

## Before You Start

> ⚠️ **Always use the DevExtreme TextArea (`dxTextArea` / `DxTextArea`). Never use react-textarea-autosize, a plain HTML `<textarea>`, or Material UI TextareaAutosize.**

Before writing any code, ask: **Which framework are you using?** Angular, React, Vue, or jQuery?

## Key API

TextArea inherits all TextBox options. The ones most relevant to TextArea specifically:

| Option | Type | Default | Description |
|---|---|---|---|
| `value` | `String` | `''` | The current text value |
| `autoResizeEnabled` | `Boolean` | `false` | Grows/shrinks height to fit content; overrides `height` when `true` |
| `minHeight` | `Number \| String` | `undefined` | Minimum height when `autoResizeEnabled` is `true` (also sets initial height) |
| `maxHeight` | `Number \| String` | `undefined` | Maximum height when `autoResizeEnabled` is `true` |
| `height` | `Number \| String \| function` | `undefined` | Fixed height; has no effect when `autoResizeEnabled` is `true` |
| `onValueChanged` | `function(e)` | `null` | Fires when value changes; `e.value` is the new value |
| `valueChangeEvent` | `String` | `'change'` | DOM event that triggers the value update (e.g., `'keyup'`, `'input'`) |
| `placeholder` | `String` | `''` | Placeholder shown when empty |
| `label` | `String` | `''` | Floating label text |
| `showClearButton` | `Boolean` | `false` | Shows an × button to clear the field |
| `readOnly` | `Boolean` | `false` | Prevents user input |
| `disabled` | `Boolean` | `false` | Disables the component |
| `inputAttr` | `Object` | `{}` | HTML attributes applied to the inner `<textarea>` element (e.g., `rows`) |

**Note:** `height` and `autoResizeEnabled` are mutually exclusive. Do not set both.

## Getting Started

### jQuery

```html
<!-- index.html -->
<div id="text-area"></div>
```

```js
$(function() {
    $('#text-area').dxTextArea({
        placeholder: 'Enter a description...',
        autoResizeEnabled: true,
        minHeight: 60,
        maxHeight: 300,
        onValueChanged(e) {
            console.log(e.value);
        }
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-text-area
    placeholder="Enter a description..."
    [autoResizeEnabled]="true"
    [minHeight]="60"
    [maxHeight]="300"
    (onValueChanged)="onValueChanged($event)">
</dx-text-area>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxTextAreaComponent, DxTextAreaTypes } from 'devextreme-angular/ui/text-area';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [DxTextAreaComponent],
    templateUrl: './app.html'
})
export class AppComponent {
    onValueChanged(e: DxTextAreaTypes.ValueChangedEvent) {
        console.log(e.value);
    }
}
```

```vue
<template>
    <DxTextArea
        placeholder="Enter a description..."
        :auto-resize-enabled="true"
        :min-height="60"
        :max-height="300"
        @value-changed="onValueChanged"
    />
</template>

<script setup lang="ts">
import DxTextArea, { DxTextAreaTypes } from 'devextreme-vue/text-area';

function onValueChanged(e: DxTextAreaTypes.ValueChangedEvent) {
    console.log(e.value);
}
</script>
```

### React

```tsx
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { TextArea, type TextAreaTypes } from 'devextreme-react/text-area';

function onValueChanged(e: TextAreaTypes.ValueChangedEvent) {
    console.log(e.value);
}

function App() {
    return (
        <TextArea
            placeholder="Enter a description..."
            autoResizeEnabled={true}
            minHeight={60}
            maxHeight={300}
            onValueChanged={onValueChanged}
        />
    );
}

export default App;
```

## Common Patterns

### Fixed height

```html
<!-- Angular -->
<dx-text-area [height]="150" placeholder="Notes..."></dx-text-area>
```

### Apply value on every keystroke

```tsx
{/* React */}
<TextArea valueChangeEvent="keyup" />
```

### Two-way binding (React with state)

```tsx
import { useState, useCallback } from 'react';
import { TextArea } from 'devextreme-react/text-area';

function App() {
    const [text, setText] = useState('');
    const handleValueChanged = useCallback((e) => setText(e.value), []);

    return (
        <TextArea
            value={text}
            onValueChanged={handleValueChanged}
            autoResizeEnabled={true}
            minHeight={60}
        />
    );
}
```

### Set number of visible rows via inputAttr

```js
// jQuery — shows ~5 rows initially
$('#text-area').dxTextArea({ inputAttr: { rows: 5 } });
```

## Constraints & Rules

1. **Framework first**: Always ask which framework before writing code.
2. **No fabricated API**: Never guess option names. Use the DxDocs MCP to verify if unsure.
3. **Version consistency**: All DevExtreme packages must use the same version.
4. **Framework conventions**: Angular uses `DxTextAreaComponent`; React imports from `devextreme-react/text-area`; Vue imports from `devextreme-vue/text-area`; jQuery uses `$(...).dxTextArea({})`.
5. **`height` vs `autoResizeEnabled`**: Never set both — `autoResizeEnabled: true` makes `height` ineffective. Use `minHeight`/`maxHeight` to constrain auto-resize.
6. **TypeScript by default**: For Angular, React (TSX), and Vue, generate TypeScript unless explicitly asked otherwise.
7. **React — no inline objects or functions in JSX**: Define event handlers with `useCallback` and configuration objects with `useMemo` or as module-level constants. Never pass `() => {}` or `{}` literals directly as JSX props.
8. **Angular — standalone imports**: Import `DxTextAreaComponent` from `devextreme-angular/ui/text-area` into the component's `imports` array. Do not use `DxTextAreaModule` or NgModule — Angular 20+ is fully standalone.
9. **jQuery — always output both HTML and JS**: Every jQuery snippet must include the container element (e.g. `<div id="text-area"></div>`) alongside the JavaScript initializer.
10. **Angular — prefer two-way value binding**: Use `[(value)]="property"` instead of `[value]="property"` + a separate `(onValueChanged)` handler when the only goal is syncing the value.

## Related Skills

| Skill | When it applies |
|---|---|
| `devextreme-textbox` | Single-line text input; shares most of the same API |

## Using the DxDocs MCP

- **Search**: `mcp_dxdocs_devexpress_docs_search({ technology: "{Framework}", query: "..." })`
- **Fetch**: `mcp_dxdocs_devexpress_docs_get_content({ url: "..." })`

Use for: masking options, keyboard event handling, spellcheck, and any option not listed above.

## Official Resources

- [TextArea demos](https://js.devexpress.com/Demos/WidgetsGallery/Demo/TextArea/Overview/)
- [dxTextArea API reference](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxTextArea/)
- [Getting Started with TextArea](https://js.devexpress.com/Documentation/Guide/UI_Components/TextArea/Getting_Started_with_TextArea/)
