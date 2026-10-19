# Button Templates

## When to Use This Reference

Read this file before generating a custom Button `template` or its React `render`/`component` equivalent. A Button template replaces the complete default content; use `icon` and `text` options when custom markup is unnecessary.

## Template Contract

The template data contains the configured `icon` and `text`. Do not treat these fields as free variables outside ASP.NET Core client-template markup.

| Framework | Declaration | Data access |
|---|---|---|
| jQuery | `template(data, contentElement)` | `data.icon`, `data.text` |
| Angular | `template="buttonContent"` + `*dxTemplate` | The `let` variable |
| Vue | `template="buttonContent"` + named slot | Slot `data` |
| React | `render={renderButton}` or `component={ButtonContent}` | Function/component argument |
| ASP.NET Core | `.Template(@<text>...</text>)` or `TemplateName` | `<%- icon %>`, `<%- text %>` |

## Framework Examples

### jQuery

```js
$('#saveButton').dxButton({
    text: 'Save',
    icon: 'save',
    template(data) {
        return $('<span>').addClass('button-content').append(
            $('<i>').addClass(`dx-icon dx-icon-${data.icon}`).attr('aria-hidden', 'true'),
            $('<span>').text(data.text)
        );
    }
});
```

### Angular

```html
<dx-button text="Save" icon="save" template="buttonContent">
    <span *dxTemplate="let data of 'buttonContent'" class="button-content">
        <i class="dx-icon dx-icon-save" aria-hidden="true"></i>
        <span>{{ data.text }}</span>
    </span>
</dx-button>
```

### Vue

```vue
<DxButton text="Save" icon="save" template="buttonContent">
    <template #buttonContent="{ data }">
        <span class="button-content">
            <i class="dx-icon dx-icon-save" aria-hidden="true" />
            <span>{{ data.text }}</span>
        </span>
    </template>
</DxButton>
```

### React

```tsx
function renderButton(data: { icon?: string; text?: string }) {
    return (
        <span className="button-content">
            <i className="dx-icon dx-icon-save" aria-hidden="true" />
            <span>{data.text}</span>
        </span>
    );
}

<Button text="Save" icon="save" render={renderButton} />
```

### ASP.NET Core Controls

```cshtml
@(Html.DevExtreme().Button()
    .ID("saveButton")
    .Text("Save")
    .Icon("save")
    .Template(@<text>
        <span class="button-content">
            <i class="dx-icon dx-icon-<%- icon %>" aria-hidden="true"></i>
            <span><%- text %></span>
        </span>
    </text>)
)
```

Do not put `${text}`, JSX, or Angular interpolation in a Razor template. Use `<%- text %>` for the client-side template value.

## Accessibility

A custom template must preserve an understandable accessible name. Keep `text`, add suitable visible text, or configure an accessible label. Mark decorative icons with `aria-hidden="true"`.

## Official References

- [Button configuration and template contract](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxButton/Configuration/#template)
- [ASP.NET Core template concepts](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
