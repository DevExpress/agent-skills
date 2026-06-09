# ComboBox — Data Binding

**When to use this reference**: Loading this reference when you need to bind `DxComboBox` to data — a simple collection, custom objects, async source, or virtual scrolling.

---

## Strongly Typed Collection (Simple Types)

Use the `Data` property to bind to `IEnumerable<T>` of strings, numbers, or enums. Use `@bind-Value` to keep the selected value in sync.

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@SelectedCity"
            NullText="Select a city…" />

@code {
    IEnumerable<string> Cities = new List<string> { "London", "Berlin", "Paris" };
    string SelectedCity { get; set; }
}
```

Initialize `Data` in `OnInitialized` or before it is invoked. For async loading, use `DataAsync`:

```razor
<DxComboBox DataAsync="@LoadCitiesAsync"
            @bind-Value="@SelectedCity"
            NullText="Select a city…" />

@code {
    string SelectedCity { get; set; }

    async Task<IEnumerable<string>> LoadCitiesAsync(CancellationToken ct)
    {
        // Real-world: call your Web API or database, e.g.:
        //   return await Http.GetFromJsonAsync<IEnumerable<string>>("api/cities", ct);
        await Task.Delay(0, ct);
        return new List<string> { "London", "Berlin", "Paris", "Tokyo", "New York" };
    }
}
```

---

## Custom Object Collection

When `TData` is a custom class:

1. Set `TextFieldName` to the property that returns display text.
2. If `TData == TValue` (binding to the whole object), override `Equals` and `GetHashCode` in the model **so the component can match selected items**.
3. Alternatively, use `KeyFieldName` to bind to a scalar key field (`TValue` = key type), which avoids the equality requirement.

### Binding to the whole object (`TData == TValue`)

```razor
<DxComboBox Data="@Staff.DataSource"
            TextFieldName="@nameof(Person.Text)"
            @bind-Value="@SelectedPerson" />

@code {
    Person SelectedPerson { get; set; } = Staff.DataSource[0];
}
```

```csharp
public class Person {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Department Department { get; set; }
    public string Text => $"{FirstName} {LastName} ({Department} Dept.)";

    public override bool Equals(object obj) =>
        obj is Person p && Id == p.Id && FirstName == p.FirstName
            && LastName == p.LastName && Department == p.Department;

    public override int GetHashCode() => HashCode.Combine(Id, FirstName, LastName);
}

public enum Department { Motors, Electronics, Software }
```

### Binding to a key field (`TData != TValue`)

Use `KeyFieldName` so that the bound `Value` is a scalar key (e.g., `int` Id) while `TData` remains the full object:

```razor
<DxComboBox Data="@Products"
            TextFieldName="@nameof(Product.Name)"
            KeyFieldName="@nameof(Product.Id)"
            @bind-Value="@SelectedProductId" />

@code {
    IEnumerable<Product> Products = new List<Product>
    {
        new() { Id = 1, Name = "Widget A" },
        new() { Id = 2, Name = "Widget B" },
        new() { Id = 3, Name = "Gadget X" },
    };
    int SelectedProductId { get; set; } = 1;
}
```

With `KeyFieldName`, the `Product` class does **not** need custom `Equals`/`GetHashCode`.

---

## Bind to an Enumeration

Pass an `IEnumerable<MyEnum>` and set `Data` to `Enum.GetValues<MyEnum>()`:

```razor
<DxComboBox Data="@Enum.GetValues<Department>()"
            @bind-Value="@SelectedDepartment" />

@code {
    Department SelectedDepartment { get; set; } = Department.Electronics;
}
```

---

## Allow Custom Values (Free-Text Input)

Set `AllowUserInput="true"` to let users type a value that is not in the list. The `Value` property stores the typed text when `TData == string`:

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@SelectedCity"
            AllowUserInput="true" />

@code {
    IEnumerable<string> Cities = new List<string> { "London", "Berlin", "Paris" };
    string SelectedCity { get; set; }
}
```

---

## Virtual Scrolling for Large Lists

Set `ListRenderMode="ListRenderMode.Virtual"` to render only the visible items (improves performance for thousands of items):

```razor
<DxComboBox Data="@LargeList"
            @bind-Value="@SelectedItem"
            ListRenderMode="ListRenderMode.Virtual" />

@code {
    IEnumerable<string> LargeList { get; set; }
    string SelectedItem { get; set; }

    protected override void OnInitialized()
        => LargeList = Enumerable.Range(1, 10000).Select(i => $"Item {i}");
}
```

**Known limitation**: When combining `ListRenderMode.Virtual` with `DataLoadMode.OnDemand`, the component may not scroll to the selected item if it is outside the initial viewport.

---

## Data Load Mode

| Mode | Behaviour |
|---|---|
| `ListDataLoadMode.Auto` (default) | All items are loaded when the drop-down opens |
| `ListDataLoadMode.OnDemand` | Items are loaded as the user scrolls (requires `ListRenderMode.Virtual`) |

```razor
<DxComboBox Data="@LargeList"
            @bind-Value="@SelectedItem"
            DataLoadMode="ListDataLoadMode.OnDemand"
            ListRenderMode="ListRenderMode.Virtual" />
```

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Selected item resets after page re-render | Override `Equals`/`GetHashCode` in your model, or use `KeyFieldName` |
| `DataAsync` never resolves | Ensure the function accepts `CancellationToken` and returns `Task<IEnumerable<TData>>` |
| Items show class name, not text | Set `TextFieldName` to the display property name |
| Virtual scrolling selection wrong | Known limitation with `OnDemand` + selected item outside viewport — use `Auto` mode as a workaround |
