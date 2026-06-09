# Message Editing and Deletion

## Enabling Edit and Delete

Set `editing.allowUpdating` and `editing.allowDeleting` to `true` to let users edit and delete their own messages. A context menu appears on hover/long-press when these are enabled.

### jQuery

```js
$('#chat').dxChat({
    editing: {
        allowUpdating: true,
        allowDeleting: true
    }
});
```

### Angular

```html
<dx-chat>
    <dxo-chat-editing [allowUpdating]="true" [allowDeleting]="true"></dxo-chat-editing>
</dx-chat>
```

### Vue (Composition API + TypeScript)

```vue
<DxChat>
    <DxEditing :allow-updating="true" :allow-deleting="true" />
</DxChat>
```

```ts
import { DxChat, DxEditing } from 'devextreme-vue/chat';
```

### React (TSX)

```tsx
import { Chat, Editing } from 'devextreme-react/chat';

<Chat>
    <Editing allowUpdating={true} allowDeleting={true} />
</Chat>
```

---

## Conditional Editing (Function-Based)

Pass a function to `allowUpdating` or `allowDeleting` to control edit/delete access per message. The function receives `{ message }` and must return a `Boolean`.

A common use case is preventing edits on messages older than a time threshold:

### jQuery

```js
function canEdit({ message }) {
    return Date.now() - message.timestamp < 5 * 60 * 1000; // 5 minutes
}

$('#chat').dxChat({
    editing: {
        allowUpdating: canEdit,
        allowDeleting: canEdit
    }
});
```

### Angular

```html
<dx-chat>
    <dxo-chat-editing
        [allowUpdating]="canEdit"
        [allowDeleting]="canEdit">
    </dxo-chat-editing>
</dx-chat>
```

```ts
import { DxChatTypes } from 'devextreme-angular/ui/chat';

canEdit = (e: { message: DxChatTypes.Message }) => {
    return Date.now() - (e.message.timestamp as number) < 5 * 60 * 1000;
};
```

### Vue (Composition API + TypeScript)

```vue
<DxChat>
    <DxEditing :allow-updating="canEdit" :allow-deleting="canEdit" />
</DxChat>
```

```ts
import { type DxChatTypes } from 'devextreme-vue/chat';

function canEdit({ message }: { message: DxChatTypes.Message }): boolean {
    return Date.now() - (message.timestamp as number) < 5 * 60 * 1000;
}
```

### React (TSX)

```tsx
import { type ChatTypes } from 'devextreme-react/chat';

function canEdit({ message }: { message: ChatTypes.Message }): boolean {
    return Date.now() - (message.timestamp as number) < 5 * 60 * 1000;
}

<Chat>
    <Editing allowUpdating={canEdit} allowDeleting={canEdit} />
</Chat>
```

---

## Handling Edit/Delete Events

| Event | Fires |
|---|---|
| `onMessageEditingStart` | When the user opens a message for editing |
| `onMessageEditCanceled` | When the user cancels an edit |
| `onMessageUpdating` | Before a message update is applied |
| `onMessageUpdated` | After a message has been updated |
| `onMessageDeleting` | Before a message is deleted |
| `onMessageDeleted` | After a message has been deleted |

```js
// jQuery — intercept before deletion to confirm
$('#chat').dxChat({
    onMessageDeleting({ message, cancel }) {
        if (!confirm(`Delete "${message.text}"?`)) {
            cancel = true;
        }
    }
});
```

```ts
// Angular — listen for updates
import { DxChatTypes } from 'devextreme-angular/ui/chat';

onMessageUpdated(e: DxChatTypes.MessageUpdatedEvent) {
    console.log('Message updated:', e.message);
    // sync to backend
}
```
