# ComboBox — Data Shaping

**When to use this reference**: Load this reference when configuring search/filter, grouping, or disabled items in `DxComboBox`.

---

## Search and Filter

When a user types in the ComboBox input, the component automatically searches item text and filters the drop-down list. All visible columns participate in search by default.

### Key Search Properties

| Property | Type | Description |
|---|---|---|
| `SearchMode` | `ListSearchMode` | `AutoSearch` (default) — filter as you type; `Disabled` — no filtering |
| `SearchFilterCondition` | `ListSearchFilterCondition` | `Contains` / `Default`, `StartsWith`, `Equals` |
| `SearchTextParseMode` | `ListSearchTextParseMode` | `GroupWordsByAnd`, `GroupWordsByOr`, `ExactMatch` |
| `SearchDelay` | `int` | Debounce in milliseconds before filtering fires |
| `DxListEditorColumn.SearchEnabled` | `bool` | Exclude a column from search (multi-column mode) |

> **OBSOLETE — Do NOT use `FilteringMode`**: The `FilteringMode` property (type `DataGridFilteringMode`) is marked `[Obsolete]` and must never be generated. Use `SearchMode` + `SearchFilterCondition` instead.

### Example — Search with Contains Condition

```razor
<DxComboBox Data="@Staff.DataSource"
            @bind-Value="@SelectedPerson"
            SearchMode="ListSearchMode.AutoSearch"
            SearchFilterCondition="ListSearchFilterCondition.Contains">
    <Columns>
        <DxListEditorColumn FieldName="FirstName" />
        <DxListEditorColumn FieldName="LastName" />
        <DxListEditorColumn FieldName="Department" SearchEnabled="false" />
    </Columns>
</DxComboBox>

@code {
    Person SelectedPerson { get; set; }
}
```

> **Note**: If you use `EditBoxDisplayTemplate` and need filter mode, add a `DxInputBox` inside the template markup.

---

## Group Data

Use `GroupFieldName` to display items organised by a group. The drop-down shows a group header row for each distinct value of the specified field.

```razor
<DxComboBox Data="@Customers"
            @bind-Value="@SelectedCustomer"
            TextFieldName="@nameof(Customer.ContactName)"
            GroupFieldName="@nameof(Customer.Country)"
            SearchMode="ListSearchMode.AutoSearch"
            SearchFilterCondition="ListSearchFilterCondition.Contains"
            InputId="cbGrouping" />

@code {
    record Customer(string ContactName, string Country);

    IEnumerable<Customer> Customers = new List<Customer>
    {
        new("Maria Anders",   "Germany"),
        new("Thomas Hardy",   "UK"),
        new("Ana Trujillo",   "Argentina"),
        new("Antonio Moreno", "Argentina"),
        new("Pierre Dupont",  "France"),
    };
    Customer SelectedCustomer { get; set; }

    protected override void OnInitialized()
        => SelectedCustomer = Customers.FirstOrDefault(x => x.Country == "Argentina");
}
```

---

## Disabled Items

Use `DisabledFieldName` to point to a Boolean property on your data item. When the property is `true`, the item is greyed out and cannot be selected.

```razor
<DxComboBox Data="@Products"
            TextFieldName="@nameof(Product.Name)"
            DisabledFieldName="@nameof(Product.IsDiscontinued)"
            @bind-Value="@SelectedProduct" />

@code {
    record Product(string Name, bool IsDiscontinued);

    IEnumerable<Product> Products = new List<Product>
    {
        new("Widget A",  false),
        new("Widget B",  true),   // disabled — discontinued
        new("Gadget X",  false),
        new("Gadget Y",  true),   // disabled — discontinued
    };
    Product SelectedProduct { get; set; }
}
```

> **Note**: `DisabledFieldName` is inherited from `DxListEditorBase<TData, TValue>`. Disabled items are still visible in the list but cannot be clicked.

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Search not filtering | Confirm `SearchMode="ListSearchMode.AutoSearch"` is set |
| All items visible even with filter | Check that `AllowUserInput` is not interfering with the search flow; add `DxInputBox` if using `EditBoxDisplayTemplate` |
| Group header not shown | Confirm `GroupFieldName` matches an exact property name in the data item type |
| Disabled items are selectable | Verify `DisabledFieldName` points to a `bool` field and that the field returns `true` for items to be disabled |
