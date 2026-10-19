# DataGrid Templates

## When to Use This Reference

Read this file before generating `cellTemplate`/`cellRender`, `editCellTemplate`, `headerCellTemplate`, `groupCellTemplate`, `dataRowTemplate`, or master-detail template code. Each template has its own data contract, and framework wrappers expose that contract differently.

## Choose the Correct Template

| Template | Purpose | Important context |
|---|---|---|
| Column `cellTemplate` / React `cellRender` | Display-mode cell content | Cell-info object: raw value, formatted text, and row data |
| `editCellTemplate` / React `editCellRender` | Cell editor | Cell info plus `setValue(...)`; do not treat it as a display template |
| `headerCellTemplate` | Column header | Header info, including the column configuration; no row data |
| `groupCellTemplate` | Group cells | Group-cell-specific data; not a normal data row |
| `dataRowTemplate` | Complete data-row markup | Row info, columns, values, and row data; some grid features require manual handling |
| `masterDetail.template` | Expanded detail area | Master-row key and data |

Do not copy variable names between these template types without checking the exact API contract.

## Cell Template Context

A column cell template has a **cell-info contract**, not the row object directly. JavaScript and client-framework callbacks receive this contract as an object. ASP.NET Core Razor and named templates expose the contract's documented properties as free variables instead.

| Member | Meaning | Typical use |
|---|---|---|
| `value` | Raw value of the current cell | Numeric comparisons and calculations |
| `text` | Formatted display text after column formatting/custom text | What the user should see |
| `displayValue` | Display value, especially relevant for lookups or `calculateDisplayValue` | Lookup/calculated display values |
| `data` | Complete data object for the current row | Other fields such as `data.ProductName` or `data.Price` |
| `column` | Current column configuration | Column-aware rendering |
| `rowIndex` / `columnIndex` | Current visible indexes | DOM IDs only when appropriate; indexes can change after shaping |

For a Price column:

- Compare `value > 500`, because `value` is the current raw Price.
- Render `text` when column formatting should be displayed.
- Access another row field as `data.ProductName`.
- Do not use `Price` as a free variable. The row is under `data`.

## jQuery

The callback receives `(container, cellInfo)`. Append content to `container` or return a DOM/jQuery element.

```js
{
    dataField: 'Price',
    format: 'currency',
    cellTemplate(container, cellInfo) {
        const colorClass = cellInfo.value > 500 ? 'price-high' : 'price-standard';
        $('<span>').addClass(colorClass).append(
            $('<i>').addClass('dx-icon dx-icon-money').attr('aria-hidden', 'true'),
            document.createTextNode(` ${cellInfo.text}`)
        ).appendTo(container);
    }
}
```

Use `cellInfo.value`, `cellInfo.text`, and `cellInfo.data.Price`; do not read `Price` directly from the callback scope.

## Angular

Assign a template name to the column and declare the matching `*dxTemplate`. The `let` variable is the cell-info object.

```html
<dxi-data-grid-column dataField="Price" format="currency" cellTemplate="priceCell">
</dxi-data-grid-column>

<div *dxTemplate="let cell of 'priceCell'"
     [class.price-high]="cell.value > 500"
     [class.price-standard]="cell.value <= 500">
    <i class="dx-icon dx-icon-money" aria-hidden="true"></i>
    {{ cell.text }}
</div>
```

Use `cell.value`, `cell.text`, and `cell.data.Price`. Angular interpolation does not make row fields free variables.

## Vue

Destructure the named slot's `data` prop into a clearly named `cell` variable. The row is then `cell.data`.

```vue
<DxColumn data-field="Price" format="currency" cell-template="priceCell" />

<template #priceCell="{ data: cell }">
    <div :class="cell.value > 500 ? 'price-high' : 'price-standard'">
        <i class="dx-icon dx-icon-money" aria-hidden="true"></i>
        {{ cell.text }}
    </div>
</template>
```

Use `cell.value`, `cell.text`, and `cell.data.Price`. The outer slot prop named `data` is the cell-info object; its own `data` member is the row.

## React

Use `cellRender` for a rendering function or `cellComponent` for a component type. The render-function argument is the cell-info object.

```tsx
interface Product {
    ProductID: number;
    ProductName: string;
    Price: number;
}

interface PriceCellInfo {
    value: number;
    text: string;
    data: Product;
}

function renderPriceCell(cell: PriceCellInfo) {
    const className = cell.value > 500 ? 'price-high' : 'price-standard';
    return (
        <span className={className}>
            <i className="dx-icon dx-icon-money" aria-hidden="true" />
            {' '}{cell.text}
        </span>
    );
}

<Column dataField="Price" format="currency" cellRender={renderPriceCell} />
```

Define the render function outside JSX unless it needs component state; then use `useCallback` with the required dependencies.

## ASP.NET Core Controls

Use an inline Razor template for short markup or a `NamedTemplate` for reusable or conditional templates. Razor template expressions run on the client and use ERB-style delimiters.

Unlike a JavaScript callback, a Razor-block or named template does not declare a `cellInfo` parameter. Template-data properties are free variables. For a DataGrid edit-cell template, use `value`, `displayValue`, `data`, `component`, and `setValue(...)` directly where the contract provides them.

### Inline template

```cshtml
columns.Add()
    .DataField("Price")
    .Format(format => format.Type(Format.Currency))
    .CellTemplate(@<text>
        <% var priceClass = value > 500 ? "price-high" : "price-standard"; %>
        <span class="<%- priceClass %>">
            <i class="dx-icon dx-icon-money" aria-hidden="true"></i>
            <%- text %>
        </span>
    </text>);
```

### Named template

```cshtml
@(Html.DevExtreme().DataGrid<Product>()
    .ID("productsGrid")
    .DataSource(Model.Products, "ProductID")
    .Columns(columns => {
        columns.AddFor(product => product.Price)
            .Format(format => format.Type(Format.Currency))
            .CellTemplate(new TemplateName("priceCellTemplate"));
    })
)

@using (Html.DevExtreme().NamedTemplate("priceCellTemplate")) {
    <% var priceClass = value > 500 ? "price-high" : "price-standard"; %>
    <span class="<%- priceClass %>" title="<%- data.ProductName %>">
        <i class="dx-icon dx-icon-money" aria-hidden="true"></i>
        <%- text %>
    </span>
}
```

Inside a DataGrid cell template:

```text
<%- value %>          raw current-cell value
<%- text %>           formatted current-cell text
<%- data.Price %>     Price field on the complete row object
```

This is incorrect:

```cshtml
<% if (Price > 500) { %>
```

`Price` is not a cell-info property and is not a free variable in a DataGrid cell template. Use `value` for the current Price cell or `data.Price` when accessing the row explicitly.

Do not use Razor `@if` for conditions based on client-side template parameters. Use `<% if (...) { %>` because the template executes in the browser.

### Edit cell with a nested TagBox

Use `EditCellTemplate` rather than `CellTemplate` to replace the editor. Bind the nested editor to the free `value` variable and call the free `setValue(...)` function when its value changes.

```cshtml
columns.AddFor(model => model.Tags)
    .Caption("Tags")
    .EditCellTemplate(new TemplateName("tagsEditor"));

@using (Html.DevExtreme().NamedTemplate("tagsEditor")) {
    @(Html.DevExtreme().TagBox()
        .DataSource(new[] { "Alpha", "Beta", "Gamma", "Delta" })
        .Value(new JS("value"))
        .ValueExpr("this")
        .DisplayExpr("this")
        .ShowSelectionControls(true)
        .ApplyValueMode(EditorApplyValueMode.UseButtons)
        .OnValueChanged("function(e) { setValue(e.value); }")
    )
}
```

These forms are incorrect in an ASP.NET Core Razor or named edit-cell template because no `cellInfo` variable was declared:

```cshtml
// Incorrect
.Value(new JS("cellInfo.value"))
.OnValueChanged("function(e) { cellInfo.setValue(e.value); }")
```

The equivalent jQuery callback does receive a declared `cellInfo` parameter and therefore uses `cellInfo.value` and `cellInfo.setValue(...)`. Keep the two scopes separate.

### ERB output encoding

Use `<%- expression %>` for ordinary text because it HTML-encodes the result. `<%= expression %>` deliberately emits unencoded HTML and can create an XSS vulnerability; use it only when trusted HTML output is explicitly required. `<% code %>` executes client-side JavaScript without rendering a value.

## Syntax Map

| Framework | Template declaration | Raw value | Formatted text | Complete row |
|---|---|---|---|---|
| jQuery | `cellTemplate(container, cellInfo)` | `cellInfo.value` | `cellInfo.text` | `cellInfo.data` |
| Angular | `cellTemplate="name"` + `*dxTemplate` | `cell.value` | `cell.text` | `cell.data` |
| Vue | `cell-template="name"` + named slot | `cell.value` | `cell.text` | `cell.data` |
| React | `cellRender={renderCell}` | `cell.value` | `cell.text` | `cell.data` |
| ASP.NET Core | `@<text>` or `NamedTemplate` | `value` | `text` | `data` |

## Templates Affect Rendering Only

Cell templates do not change the underlying value used for sorting, filtering, grouping, or summaries. Use `calculateCellValue`, `calculateDisplayValue`, `customizeText`, or the relevant data operation when behavior—not only markup—must change. Template markup is also not automatically reproduced in exported Excel/PDF files; configure the exporter separately.

## Official References

- [Customize DataGrid cells — all client frameworks](https://js.devexpress.com/Vue/Documentation/Guide/UI_Components/DataGrid/Columns/Customize_Cells/)
- [Angular DataGrid column-template demo](https://js.devexpress.com/Angular/Demos/WidgetsGallery/Demo/DataGrid/Column/)
- [ASP.NET Core template concepts and free variables](https://docs.devexpress.com/AspNetCore/401029/devextreme-based-controls/concepts/templates)
- [ASP.NET Core DataGrid custom editors demo](https://demos.devexpress.com/AspNetCore/Demo/DataGrid/CustomEditors/)
- [ASP.NET Core DataGrid row-template demo](https://demos.devexpress.com/aspNetCore/Demo/DataGrid/Row/)
