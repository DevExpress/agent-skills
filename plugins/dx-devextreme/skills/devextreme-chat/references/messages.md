# Rendering Messages, Typing Indicators, and Alerts

## Rendering a Sent Message

When the user submits a message, `onMessageEntered` fires with the new `Message` object at `e.message`. You must explicitly add it to the displayed messages — the Chat does not do this automatically.

**Pattern differences by framework:**

| Framework | How to render |
|---|---|
| jQuery | Call `e.component.renderMessage(e.message)` |
| Angular | Use `DataSource` + `CustomStore` and push via `store.push([{ type: 'insert', data }])` |
| Vue | Spread `e.message` into a new reactive array |
| React | Call `setState(prev => [...prev, e.message])` |

### jQuery

```js
$('#chat').dxChat({
    user: currentUser,
    onMessageEntered({ component, message }) {
        component.renderMessage(message);
    }
});
```

> In jQuery, use `renderMessage()` — updating `items` after initialization does not work reliably.

### Angular

> ⚠️ **Do NOT use `[items]` with a plain array in Angular.** DevExtreme event callbacks (including `onMessageEntered`) fire outside Angular's `NgZone`, so mutating an array and rebinding `[items]` does not reliably trigger change detection. Messages will appear delayed or not at all.
>
> **Use `[dataSource]` backed by a `CustomStore` instead.** Calling `store.push()` updates the widget directly — no `NgZone`, no `ChangeDetectorRef`, no `instance.option()` hacks needed. This is the pattern used in the official DevExtreme MessageStreaming demo.

```html
<dx-chat
    [user]="currentUser"
    [dataSource]="chatDataSource"
    [reloadOnChange]="false"
    (onMessageEntered)="onMessageEntered($event)">
</dx-chat>
```

> **`[reloadOnChange]="false"` is required** when using `store.push()` as the sole write mechanism.
> Without it, DxChat reloads the DataSource after every push — calling `load()` which returns
> the unmodified in-memory store, wiping the just-rendered message.

```ts
import { DataSource, CustomStore } from 'devextreme-angular/common/data';
import type { DxChatTypes } from 'devextreme-angular/ui/chat';

// Set up once (e.g. as class fields):
private store: DxChatTypes.Message[] = [];

// ⚠️ Do NOT define 'insert' in the CustomStore.
// DxChat automatically calls store.insert() when the user submits a message.
// If you also insert in onMessageEntered, every message renders twice.
// Own all writes exclusively via store.push() instead.
private customStore = new CustomStore({
    key: 'id',
    load: () => Promise.resolve([...this.store]),
});

chatDataSource = new DataSource({ store: this.customStore, paginate: false });

// Helper used by all message inserts:
private insertMessage(data: DxChatTypes.Message) {
    this.chatDataSource.store().push([{ type: 'insert', data }]);
}

onMessageEntered(e: DxChatTypes.MessageEnteredEvent) {
    this.insertMessage(e.message);
}
```

> To update an existing message (e.g. streaming): `store.push([{ type: 'update', key: id, data: { text } }])`.
> To delete a message: `store.push([{ type: 'remove', key: id }])`.

### Vue (Composition API + TypeScript)

```vue
<template>
    <DxChat
        :user="currentUser"
        :items="messages"
        @message-entered="onMessageEntered"
    />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { DxChat, type DxChatTypes } from 'devextreme-vue/chat';

const messages = ref<DxChatTypes.Message[]>([]);

function onMessageEntered(e: DxChatTypes.MessageEnteredEvent) {
    messages.value = [...messages.value, e.message];
}
</script>
```

### React (TSX + hooks)

```tsx
import { useCallback, useState } from 'react';
import { Chat, type ChatTypes } from 'devextreme-react/chat';

function App() {
    const [messages, setMessages] = useState<ChatTypes.Message[]>([]);

    const onMessageEntered = useCallback((e: ChatTypes.MessageEnteredEvent) => {
        setMessages(prev => [...prev, e.message]);
    }, []);

    return (
        <Chat
            user={currentUser}
            items={messages}
            onMessageEntered={onMessageEntered}
        />
    );
}
```

---

## Typing Indicator

Set `typingUsers` to an array of `User` objects to show a "... is typing" indicator. Clear it (empty array) when the response is ready.

### jQuery

```js
const chat = $('#chat').dxChat({ /* ... */ }).dxChat('instance');

function simulateResponse() {
    // Show indicator
    chat.option('typingUsers', [bot]);

    setTimeout(() => {
        // Clear indicator and render response
        chat.option('typingUsers', []);
        chat.renderMessage({
            author: bot,
            timestamp: Date.now(),
            text: 'Here is my response!'
        });
    }, 1500);
}
```

### Angular

```ts
typingUsers: DxChatTypes.User[] = [];

showTyping() {
    this.typingUsers = [this.bot];
    setTimeout(() => {
        this.typingUsers = [];
        this.insertMessage({
            author: this.bot,
            timestamp: Date.now(),
            text: 'Here is my response!'
        });
    }, 1500);
}
```

```html
<dx-chat
    [(typingUsers)]="typingUsers"
    [dataSource]="chatDataSource"
    [reloadOnChange]="false">
</dx-chat>
```

> Use the same `chatDataSource` / `insertMessage()` helper from the [Rendering a Sent Message — Angular](#angular) section above. Do **not** use `[items]` here — array replacement does not reliably trigger change detection when called from a `setTimeout` outside `NgZone`.

### Vue

```vue
<DxChat v-model:typing-users="typingUsers" :items="messages" />
```

```ts
const typingUsers = ref<User[]>([]);

function showTyping() {
    typingUsers.value = [bot];
    setTimeout(() => {
        typingUsers.value = [];
        messages.value = [...messages.value, { author: bot, timestamp: Date.now(), text: 'Response!' }];
    }, 1500);
}
```

### React

```tsx
const [typingUsers, setTypingUsers] = useState<User[]>([]);

function showTyping() {
    setTypingUsers([bot]);
    setTimeout(() => {
        setTypingUsers([]);
        setMessages(prev => [...prev, { author: bot, timestamp: Date.now(), text: 'Response!' }]);
    }, 1500);
}

<Chat typingUsers={typingUsers} items={messages} />
```

---

## Alerts

`alerts` are system-level notifications displayed at the bottom of the message list (e.g., "Session expired", "Connection lost"). They do not come from a user.

```ts
// Alert shape
interface Alert {
    id: string | number;
    message: string;
}
```

### jQuery

```js
chat.option('alerts', [{ id: 1, message: 'Session expired. Please refresh.' }]);

// Clear alerts
chat.option('alerts', []);
```

### Angular

```html
<dx-chat [(alerts)]="alerts" [(disabled)]="isDisabled"></dx-chat>
```

```ts
alerts: DxChatTypes.Alert[] = [];
isDisabled = false;

expireSession() {
    this.alerts = [{ id: 1, message: 'Session expired.' }];
    this.isDisabled = true;
}
```

### Vue

```vue
<DxChat v-model:alerts="alerts" v-model:disabled="isDisabled" />
```

```ts
const alerts = ref<Alert[]>([]);
const isDisabled = ref(false);

function expireSession() {
    alerts.value = [{ id: 1, message: 'Session expired.' }];
    isDisabled.value = true;
}
```

### React

```tsx
const [alerts, setAlerts] = useState<Alert[]>([]);
const [isDisabled, setIsDisabled] = useState(false);

function expireSession() {
    setAlerts([{ id: 1, message: 'Session expired.' }]);
    setIsDisabled(true);
}

<Chat alerts={alerts} disabled={isDisabled} />
```

---

## Using `dataSource` Instead of `items`

For store-backed data, pass a `DataSource` or `Store` instance to `dataSource` instead of `items`.

> **Do not use both `items` and `dataSource` on the same instance.**

```ts
import DataSource from 'devextreme/data/data_source';
import ArrayStore from 'devextreme/data/array_store';

const chatDataSource = new DataSource({
    store: new ArrayStore({ key: 'id', data: messageHistory }),
    reshapeOnPush: true
});
```

```js
// jQuery
$('#chat').dxChat({ dataSource: chatDataSource });
```
