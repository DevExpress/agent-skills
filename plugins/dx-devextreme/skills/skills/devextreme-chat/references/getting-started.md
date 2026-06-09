# Getting Started with Chat

## Create a Chat (all frameworks)

### jQuery

```html
<!-- index.html -->
<div id="chat"></div>
```

```js
// index.js
$(function() {
    $('#chat').dxChat({
        width: 400,
        height: 500
    });
});
```

### Angular

```html
<!-- app.html -->
<dx-chat [width]="400" [height]="500"></dx-chat>
```

```ts
// app.ts
import { Component } from '@angular/core';
import { DxChatComponent } from 'devextreme-angular/ui/chat';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [DxChatComponent],
    templateUrl: './app.html'
})
export class AppComponent {}
```

### Vue (Composition API + TypeScript)

```vue
<!-- App.vue -->
<template>
    <DxChat :width="400" :height="500" />
</template>

<script setup lang="ts">
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { DxChat } from 'devextreme-vue/chat';
</script>
```

### React (TSX + hooks)

```tsx
// App.tsx
import 'devextreme/dist/css/dx.fluent.blue.light.css';
import { Chat } from 'devextreme-react/chat';

function App() {
    return <Chat width={400} height={500} />;
}

export default App;
```

---

## Configure Users

The `user` property identifies the **current user** — their messages appear on the right side of the chat. Other authors appear on the left. Avatars fall back to name initials when `avatarUrl` is not provided.

```ts
// User shape (framework-agnostic)
const currentUser = {
    id: '1',
    name: 'Alice',
    avatarUrl: '/images/alice.png',
    avatarAlt: 'Alice avatar'
};

const bot = {
    id: '2',
    name: 'Support Bot',
    avatarUrl: '/images/bot.png',
    avatarAlt: 'Bot avatar'
};
```

### jQuery

```js
$('#chat').dxChat({ user: currentUser });
```

### Angular

```html
<dx-chat [user]="currentUser"></dx-chat>
```

```ts
import { DxChatTypes } from 'devextreme-angular/ui/chat';

currentUser: DxChatTypes.User = { id: '1', name: 'Alice' };
```

### Vue

```vue
<DxChat :user="currentUser" />
```

```ts
import { type DxChatTypes } from 'devextreme-vue/chat';
const currentUser: DxChatTypes.User = { id: '1', name: 'Alice' };
```

### React

```tsx
import { type ChatTypes } from 'devextreme-react/chat';
const currentUser: ChatTypes.User = { id: '1', name: 'Alice' };

<Chat user={currentUser} />
```

---

## Seed Initial Messages

Pass a `Message[]` array to `items` to pre-populate the chat history.

```ts
// Message shape
const initialMessages = [
    {
        timestamp: Date.now(),
        author: bot,
        text: 'Hello! How can I help you today?'
    }
];
```

### jQuery

```js
$('#chat').dxChat({
    user: currentUser,
    items: initialMessages
});
```

### Angular

```html
<dx-chat [user]="currentUser" [items]="messages"></dx-chat>
```

```ts
import { DxChatTypes } from 'devextreme-angular/ui/chat';

messages: DxChatTypes.Message[] = [
    { timestamp: Date.now(), author: bot, text: 'Hello! How can I help you today?' }
];
```

### Vue

```vue
<DxChat :user="currentUser" :items="messages" />
```

```ts
import { ref } from 'vue';
import { type DxChatTypes } from 'devextreme-vue/chat';

const messages = ref<DxChatTypes.Message[]>([
    { timestamp: Date.now(), author: bot, text: 'Hello! How can I help you today?' }
]);
```

### React

```tsx
import { useState } from 'react';
import { type ChatTypes } from 'devextreme-react/chat';

const initialMessages: ChatTypes.Message[] = [
    { timestamp: Date.now(), author: bot, text: 'Hello! How can I help you today?' }
];

const [messages, setMessages] = useState<ChatTypes.Message[]>(initialMessages);

<Chat user={currentUser} items={messages} />
```

---

## Appearance Options

| Option | Default | Description |
|---|---|---|
| `showAvatar` | `true` | Show/hide user avatar thumbnails |
| `showUserName` | `true` | Show/hide the author name above messages |
| `showDayHeaders` | `true` | Show/hide date separator rows |
| `showMessageTimestamp` | `true` | Show/hide per-message timestamps |
| `messageTimestampFormat` | `'shortTime'` | Format string or object for timestamps |
| `dayHeaderFormat` | `'longDate'` | Format string or object for day separators |

```js
// jQuery — minimal UI
$('#chat').dxChat({
    showAvatar: false,
    showUserName: false,
    showDayHeaders: false
});
```
