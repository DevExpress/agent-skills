# AI Service Integration

## General Pattern

Integrating an AI service follows this flow:

1. User submits a message → `onMessageEntered` fires.
2. Set `typingUsers` to show the typing indicator.
3. Call the AI API (streaming or one-shot).
4. Render the AI response; clear `typingUsers`.
5. Handle errors with `alerts` and `disabled`.

> For Markdown in AI responses, see [markdown.md](markdown.md).

---

## One-Shot Response (Fetch API, framework-agnostic)

```ts
// TypeScript utility — call any REST AI endpoint
async function fetchAIResponse(userMessage: string): Promise<string> {
    const response = await fetch('https://api.openai.com/v1/chat/completions', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${import.meta.env.VITE_OPENAI_API_KEY}`
        },
        body: JSON.stringify({
            model: 'gpt-4o',
            messages: [{ role: 'user', content: userMessage }]
        })
    });
    if (!response.ok) throw new Error(`AI error: ${response.status}`);
    const data = await response.json();
    return data.choices[0].message.content as string;
}
```

---

## React — One-Shot Integration

```tsx
import { useCallback, useState } from 'react';
import { Chat, type ChatTypes } from 'devextreme-react/chat';

const currentUser: ChatTypes.User = { id: '1', name: 'You' };
const assistant:   ChatTypes.User = { id: '2', name: 'Assistant' };

function App() {
    const [messages,     setMessages]     = useState<ChatTypes.Message[]>([]);
    const [typingUsers,  setTypingUsers]  = useState<ChatTypes.User[]>([]);
    const [alerts,       setAlerts]       = useState<ChatTypes.Alert[]>([]);
    const [isDisabled,   setIsDisabled]   = useState(false);

    const onMessageEntered = useCallback(async (e: ChatTypes.MessageEnteredEvent) => {
        // Add user message
        const userMsg = e.message;
        setMessages(prev => [...prev, userMsg]);

        // Show typing indicator
        setTypingUsers([assistant]);

        try {
            const text = await fetchAIResponse(userMsg.text ?? '');
            setTypingUsers([]);
            setMessages(prev => [...prev, { author: assistant, timestamp: Date.now(), text }]);
        } catch {
            setTypingUsers([]);
            setAlerts([{ id: Date.now(), message: 'AI service unavailable. Please try again.' }]);
            setIsDisabled(true);
        }
    }, []);

    return (
        <Chat
            user={currentUser}
            items={messages}
            typingUsers={typingUsers}
            alerts={alerts}
            disabled={isDisabled}
            onMessageEntered={onMessageEntered}
            height={500}
        />
    );
}
```

---

## Streaming Responses

AI services like OpenAI support streaming via Server-Sent Events. The pattern:
1. Create a placeholder message in `items` (empty or "…").
2. Receive chunks from the stream; append each chunk to the last message's `text`.
3. Replace the array reference so the component re-renders.

### React — Streaming with OpenAI SDK

```tsx
import OpenAI from 'openai';

const openai = new OpenAI({ apiKey: import.meta.env.VITE_OPENAI_API_KEY, dangerouslyAllowBrowser: true });

const onMessageEntered = useCallback(async (e: ChatTypes.MessageEnteredEvent) => {
    setMessages(prev => [...prev, e.message]);
    setTypingUsers([assistant]);

    // Insert a placeholder for the streaming response
    const placeholder: ChatTypes.Message = { id: 'stream', author: assistant, timestamp: Date.now(), text: '' };
    setMessages(prev => [...prev, placeholder]);
    setTypingUsers([]);

    try {
        const stream = openai.beta.chat.completions.stream({
            model: 'gpt-4o',
            messages: [{ role: 'user', content: e.message.text ?? '' }]
        });

        for await (const chunk of stream) {
            const delta = chunk.choices[0]?.delta?.content ?? '';
            setMessages(prev => {
                const updated = [...prev];
                const last = updated[updated.length - 1];
                updated[updated.length - 1] = { ...last, text: (last.text ?? '') + delta };
                return updated;
            });
        }
    } catch {
        setAlerts([{ id: Date.now(), message: 'Stream error. Please try again.' }]);
    }
}, []);
```

### Angular — Streaming Pattern

```ts
// In Angular, use the OpenAI SDK or raw fetch with ReadableStream
async onMessageEntered(e: DxChatTypes.MessageEnteredEvent) {
    this.messages = [...this.messages, e.message];
    this.typingUsers = [this.assistant];

    const placeholder: DxChatTypes.Message = {
        id: 'stream',
        author: this.assistant,
        timestamp: Date.now(),
        text: ''
    };

    try {
        const stream = this.openai.beta.chat.completions.stream({
            model: 'gpt-4o',
            messages: [{ role: 'user', content: e.message.text ?? '' }]
        });

        this.typingUsers = [];
        this.messages = [...this.messages, placeholder];

        for await (const chunk of stream) {
            const delta = chunk.choices[0]?.delta?.content ?? '';
            const updated = [...this.messages];
            const last = updated[updated.length - 1];
            updated[updated.length - 1] = { ...last, text: (last.text ?? '') + delta };
            this.messages = updated;
        }
    } catch {
        this.typingUsers = [];
        this.alerts = [{ id: Date.now(), message: 'AI error.' }];
    }
}
```

---

## Azure OpenAI

Replace the OpenAI client with Azure's endpoint and API key:

```ts
import OpenAI from 'openai';

const openai = new OpenAI({
    apiKey: import.meta.env.VITE_AZURE_OPENAI_API_KEY,
    baseURL: `${import.meta.env.VITE_AZURE_OPENAI_ENDPOINT}/openai/deployments/${import.meta.env.VITE_AZURE_DEPLOYMENT}`,
    defaultQuery: { 'api-version': '2024-02-01' },
    defaultHeaders: { 'api-key': import.meta.env.VITE_AZURE_OPENAI_API_KEY },
    dangerouslyAllowBrowser: true
});
```

The rest of the pattern (streaming, error handling) is identical to the OpenAI example above.

---

## Popup Embedding

A common pattern for AI chatbots is a floating button that opens a Chat in a popup at the bottom-right corner.

```js
// jQuery
const assistant = { id: '2', name: 'Assistant' };

$('#action').dxSpeedDialAction({
    label: 'Chat',
    icon: 'send',
    onClick() { popup.show(); }
});

const popup = $('#popup').dxPopup({
    position: 'right bottom',
    width: 400,
    height: 650,
    shading: false,
    dragEnabled: false,
    contentTemplate(contentElement) {
        $('<div>').attr('id', 'chat').dxChat({
            width: 400,
            height: 600,
            user: currentUser,
            items: [{ author: assistant, timestamp: Date.now(), text: 'Hello! How can I help?' }],
            onMessageEntered({ component, message }) {
                component.renderMessage(message);
                // call AI here...
            }
        }).appendTo(contentElement);
    }
}).dxPopup('instance');
```

```css
/* Remove inner padding so Chat fills the popup */
#chat { border: none; }
.dx-popup-content-scrollable { padding: 0; overflow: hidden; }
```

For Angular/Vue/React, nest `<dx-chat>` / `<DxChat>` / `<Chat>` inside `<dx-popup>` / `<DxPopup>` / `<Popup>` and control visibility with a bound `visible` property.

---

## Security Reminders

- Never expose AI API keys in client-side code for production. Proxy requests through your own backend.
- Sanitize AI-generated HTML with [DOMPurify](https://github.com/cure53/DOMPurify) before injecting into the DOM via `innerHTML` / `v-html` / `[innerHTML]` / `dangerouslySetInnerHTML`.
