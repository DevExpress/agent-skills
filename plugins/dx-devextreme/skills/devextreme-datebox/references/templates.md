# DateBox Templates

## When to Use This Reference

Read this file before customizing the DateBox drop-down button. DateBox exposes `dropDownButtonTemplate`; it does not expose an item template for calendar cells. Calendar customization belongs to the Calendar component configured through `calendarOptions` and must be verified against that API.

## Template Contract

`dropDownButtonTemplate` receives Button-style data containing `icon` and `text`. The template changes markup only; it does not open the drop-down by itself or change date parsing.

| Framework | Declaration | Data access |
|---|---|---|
| jQuery | `dropDownButtonTemplate(data)` | `data.icon`, `data.text` |
| Angular | `dropDownButtonTemplate="name"` + `*dxTemplate` | The `let` variable |
| Vue | `drop-down-button-template="name"` + named slot | Slot `data` |
| React | `dropDownButtonRender={fn}` | Render-function argument |
| ASP.NET Core | `.DropDownButtonTemplate(@<text>...</text>)` or `TemplateName` | Client-template variables |

## Framework Examples

### jQuery

```js
$('#dateBox').dxDateBox({
    type: 'date',
    dropDownButtonTemplate(data) {
        return $('<i>').addClass('dx-icon dx-icon-event').attr('aria-hidden', 'true');
    }
});
```

### Angular

```html
<dx-date-box type="date" dropDownButtonTemplate="dateButton">
    <i *dxTemplate="let data of 'dateButton'"
       class="dx-icon dx-icon-event"
       aria-hidden="true"></i>
</dx-date-box>
```

### Vue

```vue
<DxDateBox type="date" drop-down-button-template="dateButton">
    <template #dateButton="{ data }">
        <i class="dx-icon dx-icon-event" aria-hidden="true" />
    </template>
</DxDateBox>
```

### React

```tsx
function renderDateButton() {
    return <i className="dx-icon dx-icon-event" aria-hidden="true" />;
}

<DateBox type="date" dropDownButtonRender={renderDateButton} />
```

### ASP.NET Core Controls

```cshtml
@(Html.DevExtreme().DateBox()
    .ID("dateBox")
    .Type(DateBoxType.Date)
    .DropDownButtonTemplate(@<text>
        <i class="dx-icon dx-icon-event" aria-hidden="true"></i>
    </text>)
)
```

Do not use an `itemTemplate` copied from SelectBox. DateBox's direct template is specifically for its drop-down button.

## Official References

- [DateBox `dropDownButtonTemplate`](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxDateBox/Configuration/#dropDownButtonTemplate)
- [ASP.NET Core template concepts](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
