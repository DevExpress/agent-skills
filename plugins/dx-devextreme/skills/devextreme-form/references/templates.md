# Form Templates

## When to Use This Reference

Read this file before generating a Form item template, label template, or framework-specific render/component equivalent. Form template data is item metadata, not the complete `formData` object.

## Choose the Correct Template

| Template | Purpose | Important context |
|---|---|---|
| Simple item `template` | Replaces the item's editor/content area | Item metadata; read values through the Form or explicitly captured application state |
| `label.template` | Replaces a simple item's label | Label data such as `text`; preserve the label text and accessibility |
| Button item `buttonOptions.template` | Replaces Button content | Button template data (`icon`, `text`), not Form item data |
| Tab item `tabTemplate` | Replaces a tab header | Tab-specific data |

Do not assume a field such as `FirstName` is a free variable merely because the item has `dataField: 'FirstName'`.

## Framework Syntax

| Framework | Declaration | Context access |
|---|---|---|
| jQuery | `template(data, itemElement)` | Callback `data` argument |
| Angular | `template="name"` + `*dxTemplate` | The `let` variable |
| Vue | `template="name"` + named slot | Slot `data` |
| React | `render` / `component` on the applicable nested configuration | Function/component argument |
| ASP.NET Core | `.Template(@<text>...</text>)` or `TemplateName` | Documented client-template variables |

## Item Template Examples

### jQuery

```js
$('#employeeForm').dxForm({
    formData: employee,
    items: [{
        dataField: 'Notes',
        template(data, element) {
            $('<div>').dxTextArea({
                value: data.component.option('formData.Notes'),
                onValueChanged(e) {
                    data.component.updateData('Notes', e.value);
                }
            }).appendTo(element);
        }
    }]
});
```

### Angular

```html
<dx-form [formData]="employee">
    <dxi-form-simple-item dataField="Notes" template="notesEditor"></dxi-form-simple-item>
    <div *dxTemplate="let item of 'notesEditor'">
        <dx-text-area [(value)]="employee.Notes"></dx-text-area>
    </div>
</dx-form>
```

### Vue

```vue
<DxForm :form-data="employee">
    <DxSimpleItem data-field="Notes" template="notesEditor" />
    <template #notesEditor="{ data }">
        <DxTextArea v-model:value="employee.Notes" />
    </template>
</DxForm>
```

### React

```tsx
function renderNotes() {
    return <TextArea value={employee.Notes} />;
}

<Form formData={employee}>
    <SimpleItem dataField="Notes" render={renderNotes} />
</Form>
```

### ASP.NET Core Controls

```cshtml
items.AddSimpleFor(model => model.Notes)
    .Template(new TemplateName("notesEditor"));

@using (Html.DevExtreme().NamedTemplate("notesEditor")) {
    @(Html.DevExtreme().TextArea()
        .Value(new JS("component.option('formData.Notes')"))
        .OnValueChanged("function(e) { component.updateData('Notes', e.value); }")
    )
}
```

In ASP.NET Core client templates, `component` is the Form instance. Do not use Razor `@Model.Notes` when the template must reflect client-side Form state after rendering.

## Label Templates

Preserve the supplied label text. In jQuery use `data.text`; Angular/Vue/React receive the label-template data object through their normal template mechanism; ASP.NET Core client markup uses `<%- text %>`.

```cshtml
.Label(label => label.Template(@<text>
    <span><%- text %></span>
    <i class="dx-icon dx-icon-info" aria-hidden="true"></i>
</text>))
```

## Official References

- [Form item customization demo](https://js.devexpress.com/Vue/Demos/WidgetsGallery/Demo/Form/ItemCustomization/)
- [Add tooltips to Form item labels](https://js.devexpress.com/Angular/Documentation/Guide/UI_Components/Form/Configure_Item_Labels/Add_Tooltips_to_Item_Labels/)
- [ASP.NET Core template concepts](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
