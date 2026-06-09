# Markdown Support

## Overview

Chat's `text` field renders HTML. To support Markdown (from users or AI responses), convert Markdown to HTML before rendering using a third-party library.

The recommended approach: convert inside `messageTemplate` so every message is processed on render.

**Recommended libraries:**

| Library | Use case |
|---|---|
| `micromark` | Lightweight; CommonMark only |
| `unified` + `remark` + `rehype` | Full-featured; supports extensions (GFM, syntax highlighting) |

Install:
```bash
npm install micromark
# or
npm install unified remark-parse remark-rehype rehype-stringify
```

---

## Using `micromark` in `messageTemplate`

### jQuery

```html
<!-- index.html — load as ES module -->
<script type="module">
    import { micromark } from 'https://esm.sh/micromark@3?bundle';
    window.micromark = micromark;
</script>
```

```js
// index.js
$('#chat').dxChat({
    onMessageEntered({ component, message }) {
        component.renderMessage(message);
    },
    messageTemplate(data, element) {
        const html = micromark(data.message.text ?? '');
        $('<div>').html(html).appendTo(element);
    }
});
```

### Angular

```bash
npm install micromark
```

```html
<!-- app.html -->
<dx-chat
    [items]="messages"
    (onMessageEntered)="onMessageEntered($event)"
    messageTemplate="messageTemplate">
    <div *dxTemplate="let data of 'messageTemplate'">
        <div [innerHTML]="toHtml(data.message.text)"></div>
    </div>
</dx-chat>
```

```ts
// app.ts
import { DxChatTypes } from 'devextreme-angular/ui/chat';
import { micromark } from 'micromark';

messages: DxChatTypes.Message[] = [];

onMessageEntered(e: DxChatTypes.MessageEnteredEvent) {
    this.messages = [...this.messages, e.message];
}

toHtml(text: string): string {
    // Strip wrapping <p> tags for inline rendering
    return micromark(text ?? '').replace(/^<p>/, '').replace(/<\/p>\n?$/, '');
}
```

> **Security note**: `[innerHTML]` bypasses Angular's default sanitization. Pipe through `DomSanitizer.bypassSecurityTrustHtml()` only if the source is trusted (AI output). For user-generated content, sanitize with `DOMPurify` first.

### Vue (Composition API + TypeScript)

```bash
npm install micromark
```

```vue
<template>
    <DxChat
        :items="messages"
        @message-entered="onMessageEntered"
        message-template="messageTemplate"
    >
        <template #messageTemplate="{ data }">
            <div v-html="toHtml(data.message.text)" />
        </template>
    </DxChat>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { DxChat, type DxChatTypes } from 'devextreme-vue/chat';
import { micromark } from 'micromark';

const messages = ref<DxChatTypes.Message[]>([]);

function onMessageEntered(e: DxChatTypes.MessageEnteredEvent) {
    messages.value = [...messages.value, e.message];
}

function toHtml(text: string): string {
    return micromark(text ?? '').replace(/^<p>/, '').replace(/<\/p>\n?$/, '');
}
</script>
```

### React (TSX)

```bash
npm install micromark
```

```tsx
import { useCallback, useState } from 'react';
import { Chat, type ChatTypes } from 'devextreme-react/chat';
import { micromark } from 'micromark';

function MessageBubble({ data }: { data: { message: ChatTypes.Message } }) {
    const html = micromark(data.message.text ?? '').replace(/^<p>/, '').replace(/<\/p>\n?$/, '');
    return <div dangerouslySetInnerHTML={{ __html: html }} />;
}

const renderMessage = (data: { message: ChatTypes.Message }) => <MessageBubble data={data} />;

function App() {
    const [messages, setMessages] = useState<ChatTypes.Message[]>([]);

    const onMessageEntered = useCallback((e: ChatTypes.MessageEnteredEvent) => {
        setMessages(prev => [...prev, e.message]);
    }, []);

    return (
        <Chat
            items={messages}
            onMessageEntered={onMessageEntered}
            messageRender={renderMessage}
        />
    );
}
```

> **Security note**: `dangerouslySetInnerHTML` is XSS-prone. Pass AI-generated HTML through [DOMPurify](https://github.com/cure53/DOMPurify) before setting it.

---

## Using `unified` + `remark` + `rehype` (full-featured)

```bash
npm install unified remark-parse remark-rehype rehype-stringify
```

```ts
// Utility function (framework-agnostic)
import { unified } from 'unified';
import remarkParse from 'remark-parse';
import remarkRehype from 'remark-rehype';
import rehypeStringify from 'rehype-stringify';

export async function markdownToHtml(markdown: string): Promise<string> {
    const file = await unified()
        .use(remarkParse)
        .use(remarkRehype)
        .use(rehypeStringify)
        .process(markdown);
    return String(file);
}
```

Use this inside `messageTemplate` or a custom rendering function just like the `micromark` examples above, but call it asynchronously and update the rendered element once the Promise resolves.
