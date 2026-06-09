# ComboBox — Validation

**When to use this reference**: Load this reference when integrating `DxComboBox` with Blazor's `<EditForm>` for input validation.

---

## Overview

`DxComboBox` integrates with standard Blazor [EditForm](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/validation) and [Data Annotations](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation). Place `DxComboBox` inside an `<EditForm>` and use `@bind-Value` linked to a model property decorated with validation attributes.

**Validated properties**:
- `DxComboBox.Value` — the selected value
- `DxComboBox.Text` — the text typed by the user (when `AllowUserInput="true"`)

Control which property is validated with `ValidateBy`:

| `ComboBoxValidateBy` | Description |
|---|---|
| `Value` (default) | Validates the `Value` property |
| `Text` | Validates the `Text` property (useful with `AllowUserInput`) |

---

## Basic Validation Example

```csharp
// Model.cs
using System.ComponentModel.DataAnnotations;

public class OrderModel {
    [Required(ErrorMessage = "Please select a product.")]
    public Product SelectedProduct { get; set; }
}
```

```razor
@* OrderForm.razor *@
<EditForm Model="@Model" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <DxComboBox Data="@Products"
                TextFieldName="@nameof(Product.Name)"
                @bind-Value="@Model.SelectedProduct"
                NullText="Select a product…" />
    <ValidationMessage For="@(() => Model.SelectedProduct)" />

    <DxButton SubmitFormOnClick="true" Text="Submit" RenderStyle="ButtonRenderStyle.Primary" />
</EditForm>

@code {
    OrderModel Model { get; set; } = new();
    IEnumerable<Product> Products = new List<Product>
    {
        new() { Id = 1, Name = "Widget A" },
        new() { Id = 2, Name = "Widget B" },
        new() { Id = 3, Name = "Gadget X" },
    };

    void HandleValidSubmit() { /* save data */ }
}
```

---

## Validating a Key Field

When `TData != TValue` (binding to a key field), validate the `Value` property which holds the key:

```csharp
public class OrderModel {
    [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
    public int ProductId { get; set; }
}
```

```razor
<DxComboBox Data="@Products"
            TextFieldName="@nameof(Product.Name)"
            KeyFieldName="@nameof(Product.Id)"
            @bind-Value="@Model.ProductId"
            NullText="Select a product…" />
<ValidationMessage For="@(() => Model.ProductId)" />
```

---

## Validating Free-Text Input

When `AllowUserInput="true"`, the user can type a custom value. To validate the typed text, set `ValidateBy="ComboBoxValidateBy.Text"`:

```csharp
public class SearchModel {
    [Required, MinLength(2, ErrorMessage = "Enter at least 2 characters.")]
    public string SearchText { get; set; }
}
```

```razor
<EditForm Model="@Model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    <DxComboBox Data="@Cities"
                @bind-Value="@Model.SearchText"
                AllowUserInput="true"
                ValidateBy="ComboBoxValidateBy.Text" />
    <ValidationMessage For="@(() => Model.SearchText)" />
    <button type="submit">Search</button>
</EditForm>
```

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| `ValidationMessage` never shows | Ensure `<DataAnnotationsValidator />` is inside `<EditForm>` |
| Validation fires on page load | This is standard Blazor EditForm behaviour — use `OnValidSubmit` or `EditContext` |
| Free-text input not validated | Set `ValidateBy="ComboBoxValidateBy.Text"` and ensure `AllowUserInput="true"` |
| Required attribute passes when nothing is selected | For reference types, ensure `[Required]` is used; for value types, use `[Range]` to exclude the default (e.g., 0 for int) |
