# Chat Templates

## When to Use This Reference

Read this file before generating `messageTemplate`, `emptyViewTemplate`, or React `messageRender`/`emptyViewRender` code. These two templates receive different data shapes.

## Template Contracts

| Template | Purpose | Context |
|---|---|---|
| `messageTemplate` | Message content | `{ component, message }`; message fields are under `message` |
| `emptyViewTemplate` | Content shown when there are no messages | `{ component, texts }`; localized strings are `texts.message` and `texts.prompt` |

Do not copy `texts` into a message template or message fields into an empty-view template. In particular, use `data.message.text`, not `data.text` and not a free `text` variable. If the local package typings disagree with this skill, follow the installed typings or query DxDocs for that version.

## Framework Syntax

| Framework | Declaration | Context access |
|---|---|---|
| jQuery | Template callback | Callback's first argument |
| Angular | Template-name property + `*dxTemplate` | The `let` variable |
| Vue | Template-name property + named slot | Slot `data` |
| React | `messageRender` / `emptyViewRender` | Render-function argument |
| ASP.NET Core | Razor block or `TemplateName` | Documented client-template variables |

## Empty View: Complete Examples

### jQuery

```js
$('#chat').dxChat({
    items: [],
    emptyViewTemplate(data) {
        return $('<div>').addClass('chat-empty').append(
            $('<strong>').text(data.texts.message),
            $('<span>').text(data.texts.prompt)
        );
    }
});
```

### Angular

```html
<dx-chat [items]="[]" emptyViewTemplate="emptyView">
    <div *dxTemplate="let view of 'emptyView'" class="chat-empty">
        <strong>{{ view.texts.message }}</strong>
        <span>{{ view.texts.prompt }}</span>
    </div>
</dx-chat>
```

### Vue

```vue
<DxChat :items="[]" empty-view-template="emptyView">
    <template #emptyView="{ data: view }">
        <div class="chat-empty">
            <strong>{{ view.texts.message }}</strong>
            <span>{{ view.texts.prompt }}</span>
        </div>
    </template>
</DxChat>
```

### React

```tsx
function renderEmptyView(view: ChatTypes.EmptyViewData) {
    return (
        <div className="chat-empty">
            <strong>{view.texts.message}</strong>
            <span>{view.texts.prompt}</span>
        </div>
    );
}

<Chat items={[]} emptyViewRender={renderEmptyView} />
```

### ASP.NET Core Controls

```cshtml
@(Html.DevExtreme().Chat()
    .ID("chat")
    .EmptyViewTemplate(new TemplateName("emptyChat"))
)

@using (Html.DevExtreme().NamedTemplate("emptyChat")) {
    <div class="chat-empty">
        <strong><%- texts.message %></strong>
        <span><%- texts.prompt %></span>
    </div>
}
```

Use `<%- ... %>` for client-template values. Do not use `${...}`, Razor `@if`, Angular interpolation, or JSX inside an ASP.NET Core template.

## Message Context Examples

```js
messageTemplate(data) {
    return $('<span>').text(data.message.text);
}
```

```html
<div *dxTemplate="let data of 'message'">{{ data.message.text }}</div>
```

```vue
<template #message="{ data }">{{ data.message.text }}</template>
```

```tsx
function renderMessage(data: ChatTypes.MessageTemplateData) {
    return <span>{data.message.text}</span>;
}
```

```cshtml
<span><%- message.text %></span>
```

Chat's built-in message fields use names such as `text`, `author`, and `timestamp`. For custom fields, use the casing emitted by the server-side serializer rather than guessing.

## Message Content and Safety

- Use the exact message-template type from the framework package when available.
- Preserve message actions, author information, timestamps, and attachment behavior required by the application; replacing content can remove default presentation.
- Treat message HTML as untrusted. Sanitize user- or model-generated HTML before rendering it with `innerHTML`, `v-html`, or a jQuery `.html(...)` call.
- For Markdown rendering, continue with [markdown.md](markdown.md).

## Official References

- [Chat configuration and template contracts](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxChat/Configuration/)
- [Customize the Chat empty view](https://js.devexpress.com/Angular/Documentation/Guide/UI_Components/Chat/Customize_the_Empty_View/)
- [ASP.NET Core template concepts](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
