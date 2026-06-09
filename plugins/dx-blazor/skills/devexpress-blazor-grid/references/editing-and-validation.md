# Editing & Validation — Blazor Grid

When you need to: enable CRUD operations; choose an edit mode; handle save and delete events; validate user input; customize the edit form.

## Edit Modes

Set `EditMode` on `DxGrid`:

| Mode | Description |
|---|---|
| `GridEditMode.EditRow` | In-place editors appear in all cells of the edited row |
| `GridEditMode.EditForm` | An inline form replaces the edited row |
| `GridEditMode.PopupEditForm` | A popup dialog contains the edit form |
| `GridEditMode.EditCell` | Click any cell to open an in-place editor for that cell alone (batch editing) |

## Required Events

| Event | Args | When It Fires |
|---|---|---|
| `EditModelSaving` | `GridEditModelSavingEventArgs` | User clicks Save after editing a row (new or existing) |
| `DataItemDeleting` | `GridDataItemDeletingEventArgs` | User clicks Delete |
| `CustomizeEditModel` | `GridCustomizeEditModelEventArgs` | Before the edit form opens — use to initialize new-row defaults |

## Minimal Editing Setup

```razor
<DxGrid Data="@Items"
        KeyFieldName="Id"
        EditMode="GridEditMode.EditRow"
        EditModelSaving="OnSave"
        DataItemDeleting="OnDelete">
    <Columns>
        <DxGridCommandColumn />
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Price" />
    </Columns>
</DxGrid>

@code {
    List<Product> Items { get; set; } = new();

    void OnSave(GridEditModelSavingEventArgs e) {
        var model = (Product)e.EditModel;
        if (e.IsNew)
            Items.Add(model);
        else
            e.CopyChangesToDataItem();  // copies edit model changes to the data item
    }

    void OnDelete(GridDataItemDeletingEventArgs e) {
        Items.Remove((Product)e.DataItem);
    }
}
```

## EF Core Editing Pattern

```razor
@inject IDbContextFactory<AppDbContext> DbFactory
@implements IDisposable

<DxGrid Data="@Products"
        KeyFieldName="Id"
        EditMode="GridEditMode.EditForm"
        CustomizeEditModel="OnCustomizeEditModel"
        EditModelSaving="OnSave"
        DataItemDeleting="OnDelete">
    <Columns>
        <DxGridCommandColumn />
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Price" />
    </Columns>
</DxGrid>

@code {
    IEnumerable<Product> Products;
    AppDbContext Db;

    protected override async Task OnInitializedAsync() {
        Db = DbFactory.CreateDbContext();
        Products = await Db.Products.ToListAsync();
    }

    void OnCustomizeEditModel(GridCustomizeEditModelEventArgs e) {
        // Initialize defaults for new rows
        if (e.IsNew)
            ((Product)e.EditModel).CreatedAt = DateTime.UtcNow;
    }

    async Task OnSave(GridEditModelSavingEventArgs e) {
        var model = (Product)e.EditModel;
        if (e.IsNew)
            await Db.Products.AddAsync(model);
        else
            e.CopyChangesToDataItem();
        await Db.SaveChangesAsync();
        Products = await Db.Products.ToListAsync();
    }

    async Task OnDelete(GridDataItemDeletingEventArgs e) {
        Db.Products.Remove((Product)e.DataItem);
        await Db.SaveChangesAsync();
        Products = await Db.Products.ToListAsync();
    }

    public void Dispose() => Db?.Dispose();
}
```

## GridEditModelSavingEventArgs Members

| Member | Type | Description |
|---|---|---|
| `EditModel` | `object` | The edit model (a copy of the data item) — cast to your type |
| `DataItem` | `object` | The original data item (null when `IsNew` is true) |
| `IsNew` | `bool` | True when a new row is being created |
| `CopyChangesToDataItem()` | `void` | Copies edit model property values to the original data item |
| `Reload` | `bool` | Set to `true` to trigger a grid data reload after the handler completes — alternative to calling `Grid.Reload()` when you do not hold a `@ref` |

## GridDataItemDeletingEventArgs Members

| Member | Type | Description |
|---|---|---|
| `DataItem` | `object` | The data item to delete — cast to your type |
| `Reload` | `bool` | Set to `true` to trigger a grid data reload after the handler completes — alternative to calling `Grid.Reload()` when you do not hold a `@ref` |

### Using e.Reload Instead of Grid.Reload()

When the component that handles the event does not hold a `@ref="Grid"`, set `e.Reload = true` to request a data reload without needing a reference:

```csharp
// No @ref needed on DxGrid
void OnEditModelSaving(GridEditModelSavingEventArgs e) {
    var model = (Product)e.EditModel;
    if (e.IsNew)
        Items.Add(model);
    else
        e.CopyChangesToDataItem();
    e.Reload = true; // triggers reload automatically after handler returns
}

void OnDataItemDeleting(GridDataItemDeletingEventArgs e) {
    Items.Remove((Product)e.DataItem);
    e.Reload = true;
}
```

**`Grid.Reload()` vs `e.Reload`**:
- Use `Grid.Reload()` when you hold `@ref="Grid"` and need to reload at an arbitrary moment.
- Use `e.Reload = true` inside `EditModelSaving` / `DataItemDeleting` handlers when no `@ref` is held — cleaner and avoids the need for `IGrid` injection.
- `Grid.Reload()` returns `void` — do **not** `await` it.

## Input Validation

The Grid validates using DataAnnotation attributes on the edit model:

```csharp
public class Product {
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99999.99")]
    public decimal Price { get; set; }
}
```

No additional Grid configuration is needed — the Grid reads DataAnnotation attributes automatically and highlights invalid fields.

## Programmatic Editing

```csharp
// Start editing a row
await Grid.StartEditDataItemAsync(dataItem);

// Start creating a new row
await Grid.StartEditNewRowAsync();

// Save the current edit (EditCell mode)
await Grid.SaveChangesAsync();

// Cancel editing
await Grid.CancelEditAsync();
```

## Popup Edit Form Size

```razor
<DxGrid EditMode="GridEditMode.PopupEditForm" ...>
    <CustomizeElement>
        @{ /* Use CSS to control popup width */ }
    </CustomizeElement>
</DxGrid>
```

To control popup size, apply CSS to the `.dxbs-popup` class or use `PopupEditFormCssClass`:

```razor
<DxGrid EditMode="GridEditMode.PopupEditForm"
        PopupEditFormCssClass="my-popup">
```
