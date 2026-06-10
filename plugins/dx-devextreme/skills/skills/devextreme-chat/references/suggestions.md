# Suggestion Buttons

## Overview

Suggestions are quick-reply buttons rendered by a [ButtonGroup](https://js.devexpress.com/Documentation/ApiReference/UI_Components/dxButtonGroup/) above the Chat input field. Configure them via the `suggestions` option.

## Basic Setup

```js
// jQuery
$('#chat').dxChat({
    suggestions: {
        items: [
            { text: 'Tell me more' },
            { text: 'Show examples' },
            { text: 'Summarize' }
        ]
    }
});
```

```html
<!-- Angular -->
<dx-chat>
    <dxo-chat-suggestions [items]="suggestionItems"></dxo-chat-suggestions>
</dx-chat>
```

```ts
// Angular component
suggestionItems = [
    { text: 'Tell me more' },
    { text: 'Show examples' },
    { text: 'Summarize' }
];
```

```vue
<!-- Vue -->
<DxChat :suggestions="{ items: suggestionItems }" />
```

```ts
// Vue script
const suggestionItems = [
    { text: 'Tell me more' },
    { text: 'Show examples' },
    { text: 'Summarize' }
];
```

```tsx
// React
const suggestionsConfig = { items: suggestionItems };

<Chat suggestions={suggestionsConfig} />
```

---

## Filling the Input Field on Click

Store a `message` in a custom field on each suggestion item. When clicked, set `inputFieldText` to that message so the user can review before sending.

```js
// jQuery
const chatInstance = $('#chat').dxChat({
    suggestions: {
        items: [
            { text: 'Tell me more',  message: 'Can you elaborate on that?' },
            { text: 'Summarize',     message: 'Please give me a brief summary.' }
        ],
        onItemClick({ itemData }) {
            chatInstance.option('inputFieldText', itemData.message);
        }
    }
}).dxChat('instance');
```

```html
<!-- Angular -->
<dx-chat [inputFieldText]="inputText">
    <dxo-chat-suggestions
        [items]="suggestionItems"
        (onItemClick)="onSuggestionClick($event)">
    </dxo-chat-suggestions>
</dx-chat>
```

```ts
import { DxChatTypes } from 'devextreme-angular/ui/chat';

interface SuggestionItem { text: string; message: string; }

inputText = '';
suggestionItems: SuggestionItem[] = [
    { text: 'Tell me more',  message: 'Can you elaborate on that?' },
    { text: 'Summarize',     message: 'Please give me a brief summary.' }
];

onSuggestionClick(e: { itemData: SuggestionItem }) {
    this.inputText = e.itemData.message;
}
```

```vue
<!-- Vue -->
<DxChat v-model:input-field-text="inputText">
    <DxSuggestions :items="suggestionItems" @item-click="onSuggestionClick" />
</DxChat>
```

```ts
interface SuggestionItem { text: string; message: string; }

const inputText = ref('');
const suggestionItems: SuggestionItem[] = [
    { text: 'Tell me more',  message: 'Can you elaborate on that?' },
    { text: 'Summarize',     message: 'Please give me a brief summary.' }
];

function onSuggestionClick(e: { itemData: SuggestionItem }) {
    inputText.value = e.itemData.message;
}
```

```tsx
// React
interface SuggestionItem { text: string; message: string; }

const [inputText, setInputText] = useState('');
const suggestionItems: SuggestionItem[] = [
    { text: 'Tell me more',  message: 'Can you elaborate on that?' },
    { text: 'Summarize',     message: 'Please give me a brief summary.' }
];

const handleInputFieldTextChanged = useCallback((e) => setInputText(e.value), []);
const handleSuggestionItemClick = useCallback(
    (e: { itemData: SuggestionItem }) => setInputText(e.itemData.message),
    []
);
const suggestionsConfig = useMemo(
    () => ({ items: suggestionItems, onItemClick: handleSuggestionItemClick }),
    [handleSuggestionItemClick]
);

<Chat
    inputFieldText={inputText}
    onInputFieldTextChanged={handleInputFieldTextChanged}
    suggestions={suggestionsConfig}
/>
```

---

## Auto-Sending on Suggestion Click

To send immediately (without user confirmation), call `renderMessage` / update `items` directly inside `onItemClick`:

```js
// jQuery — send immediately
const chatInstance = $('#chat').dxChat({
    user: currentUser,
    suggestions: {
        items: [{ text: 'Yes', message: 'Yes, please continue.' }],
        onItemClick({ itemData }) {
            chatInstance.renderMessage({
                author: currentUser,
                timestamp: Date.now(),
                text: itemData.message
            });
        }
    }
}).dxChat('instance');
```

---

## Disappearing Suggestions

To hide suggestions after one is clicked, clear `suggestions.items`:

```js
// jQuery
onItemClick({ itemData }) {
    chatInstance.option('suggestions', { items: [] });
    // then send/fill input...
}
```

```ts
// Angular
onSuggestionClick() {
    this.suggestionItems = [];
}
```
