# SelectBox Templates

## When to Use This Reference

Read this file before generating code that uses `itemTemplate`, `fieldTemplate`, `groupTemplate`, `dropDownButtonTemplate`, or framework-specific equivalents such as React's `itemRender`. Template declaration and data-access syntax differ by framework.

## Template Contracts

Choose the template by what it renders:

| Template | Purpose | Data context |
|---|---|---|
| `itemTemplate` | Every item in the open drop-down | Current item object |
| Per-item `template` | One specifically configured item | Current item object |
| `fieldTemplate` | Selected value in the editor field | Selected item; the template must render a DevExtreme TextBox |
| `groupTemplate` | Group headers when `grouped` is enabled | Current group object |
| `dropDownButtonTemplate` | Drop-down button appearance | Button-template-specific context |

Do not assume variables from one template are available in another. Use the documented contract for the exact component and template.

The examples below render this item shape:

```ts
interface FoodItem {
    id: number;
    name: string;
    type: 'rich' | 'poor';
}
```

## jQuery

Set `itemTemplate` to a callback. Its first argument is the current item. Build markup with DOM/jQuery APIs and use `.text(...)` for data-derived text instead of concatenating untrusted HTML.

```html
<div id="foodSelectBox"></div>
```

```js
$('#foodSelectBox').dxSelectBox({
    dataSource: foods,
    valueExpr: 'id',
    displayExpr: 'name',
    itemTemplate(item) {
        return $('<div>').addClass('selectbox-item').append(
            $('<i>').addClass('dx-icon dx-icon-info').attr('aria-hidden', 'true'),
            $('<span>').attr('data-type', item.type).text(item.name)
        );
    }
});
```

Use `item.name` and `item.type`; EJS delimiters and framework template syntax do not belong in this callback.

## Angular

Assign a template name to `itemTemplate`, then declare the matching `*dxTemplate`. The variable introduced by `let` is the current item.

```html
<dx-select-box
    [dataSource]="foods"
    valueExpr="id"
    displayExpr="name"
    itemTemplate="foodItem">
    <div *dxTemplate="let item of 'foodItem'" class="selectbox-item">
        <i class="dx-icon dx-icon-info" aria-hidden="true"></i>
        <span [attr.data-type]="item.type">{{ item.name }}</span>
    </div>
</dx-select-box>
```

Use Angular binding and interpolation. Do not use EJS `<%- ... %>` delimiters or React/Vue syntax.

## Vue

Assign a name through `item-template`, then declare a slot with the same name. Destructure `data` from the slot props; `data` is the current item.

```vue
<template>
    <DxSelectBox
        :data-source="foods"
        value-expr="id"
        display-expr="name"
        item-template="foodItem">
        <template #foodItem="{ data }">
            <div class="selectbox-item">
                <i class="dx-icon dx-icon-info" aria-hidden="true"></i>
                <span :data-type="data.type">{{ data.name }}</span>
            </div>
        </template>
    </DxSelectBox>
</template>

<script setup lang="ts">
import { DxSelectBox } from 'devextreme-vue/select-box';
</script>
```

The slot variable is `data` because the binding explicitly names it. Do not treat item fields as free variables.

## React

Use `itemRender` for a render function or `itemComponent` for a component type. The render function argument is the current item. Define it outside JSX rather than creating an inline callback on every render.

```tsx
import { SelectBox } from 'devextreme-react/select-box';

interface FoodItem {
    id: number;
    name: string;
    type: 'rich' | 'poor';
}

function renderFoodItem(item: FoodItem) {
    return (
        <div className="selectbox-item">
            <i className="dx-icon dx-icon-info" aria-hidden="true" />
            <span data-type={item.type}>{item.name}</span>
        </div>
    );
}

<SelectBox
    dataSource={foods}
    valueExpr="id"
    displayExpr="name"
    itemRender={renderFoodItem}
/>
```

Use JSX expressions such as `{item.name}`. Do not pass a template-name string or use EJS delimiters.

## ASP.NET Core Controls

Use a Razor template block or a `NamedTemplate`. Do not translate a JavaScript template literal into an `.ItemTemplate("...")` string.

For a C# object data source, item properties are available by their serialized names in DevExtreme template markup. Preserve their casing.

### Inline Razor block

```cshtml
@(Html.DevExtreme().SelectBox()
    .ID("foodSelectBox")
    .DataSource(Model.Foods)
    .ValueExpr("ID")
    .DisplayExpr("Name")
    .ItemTemplate(@<text>
        <div class="selectbox-item">
            <i class="dx-icon dx-icon-info" aria-hidden="true"></i>
            <span data-type="<%- Type %>"><%- Name %></span>
        </div>
    </text>)
)
```

Here, `ID`, `Name`, and `Type` are serialized item properties. `<%- ... %>` HTML-encodes the rendered value.

### Named template

Use a named template when multiple SelectBoxes share the same markup or when the template is too large to keep inside the builder chain.

```cshtml
@(Html.DevExtreme().SelectBox()
    .ID("foodSelectBox")
    .DataSource(Model.Foods)
    .ValueExpr("ID")
    .DisplayExpr("Name")
    .ItemTemplate(new TemplateName("foodItemTemplate"))
)

@using (Html.DevExtreme().NamedTemplate("foodItemTemplate")) {
    <div class="selectbox-item">
        <i class="dx-icon dx-icon-info" aria-hidden="true"></i>
        <span data-type="<%- Type %>"><%- Name %></span>
    </div>
}
```

The following is invalid because `${...}` is JavaScript template-literal syntax and the Razor wrapper string is not a JavaScript template literal:

```cshtml
// Incorrect
.ItemTemplate("<span>${name}</span>")
```

Do not escape markup into strings such as `"\<div>...\</div>"`. Keep markup inside `@<text>...</text>` or `NamedTemplate`.

## Syntax Boundaries

| Framework | Template declaration | Current item access |
|---|---|---|
| jQuery | `itemTemplate(item) { ... }` | `item.name` |
| Angular | `itemTemplate="foodItem"` + `*dxTemplate` | `item.name` from the `let` variable |
| Vue | `item-template="foodItem"` + named slot | `data.name` from slot props |
| React | `itemRender={renderFoodItem}` | Render-function argument |
| ASP.NET Core | `@<text>...</text>` or `NamedTemplate` | `<%- Name %>` using serialized property casing |

Never copy a template expression from one framework into another.

## Presentation Versus Item State

An item template controls markup; it does not make an item selectable, disabled, or hidden. SelectBox items support standard `disabled` and `visible` fields. Put interaction state in the data and use the template only to visualize it. When application rules change those fields at runtime, update or reassign the data through the component's documented data-source mechanism.

## Icons

DevExtreme built-in icons use both classes:

```html
<i class="dx-icon dx-icon-info" aria-hidden="true"></i>
```

Do not invent an icon class from a domain noun. Verify the icon name in the DevExtreme icon library or DxDocs before generating it. If no matching built-in icon exists, ask whether the developer wants a custom SVG, image, or external icon library.

## Official References

- [Customize SelectBox item appearance — all client frameworks](https://js.devexpress.com/Angular/Documentation/Guide/UI_Components/SelectBox/Customize_Item_Appearance/)
- [SelectBox items — `disabled`, `visible`, and per-item templates](https://js.devexpress.com/jQuery/Documentation/ApiReference/UI_Components/dxSelectBox/Configuration/items/)
- [ASP.NET Core SelectBox Overview demo — inline item template](https://demos.devexpress.com/aspNetCore/Demo/SelectBox/Overview/)
- [ASP.NET Core `ItemTemplate(TemplateName)`](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.Builders.SelectBoxBuilder.ItemTemplate%28DevExtreme.AspNet.Mvc.TemplateName%29)
- [ASP.NET Core `ItemTemplate` overloads](https://docs.devexpress.com/AspNetCore/DevExtreme.AspNet.Mvc.Builders.SelectBoxBuilder.ItemTemplate.overloads)
