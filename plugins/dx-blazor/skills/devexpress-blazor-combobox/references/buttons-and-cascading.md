# ComboBox — Buttons & Cascading Comboboxes

**When to use this reference**: Load this reference when adding a Clear button, custom command buttons, or building cascading (dependent) ComboBoxes.

---

## Clear Button and Placeholder

Set `ClearButtonDisplayMode` to `Auto` to show a **Clear** button when the editor has a non-null value. Clicking it sets the value to `null`.

Use `NullText` to display placeholder text when the value is `null`.

```razor
<DxComboBox Data="@Countries"
            @bind-Value="@SelectedCountry"
            TextFieldName="@nameof(Country.Name)"
            NullText="Select a country…"
            ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto" />

@code {
    record Country(string Name);

    IEnumerable<Country> Countries = new List<Country>
    {
        new("Germany"), new("France"), new("USA"), new("Japan")
    };
    Country SelectedCountry { get; set; }
}
```

| `DataEditorClearButtonDisplayMode` value | Behaviour |
|---|---|
| `Auto` | Button appears when value is non-null |
| `Always` | Button always visible |
| `Never` | Button hidden (default) |

> **Note**: If you use `EditBoxDisplayTemplate`, add `<DxInputBox>` inside the template to enable the Clear button.

---

## Hide the Built-In Drop-Down Button

Set `ShowDropDownButton="false"` to hide the toggle button that opens the drop-down:

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@Value"
            ShowDropDownButton="false" />
```

This is useful when you provide a fully custom trigger via command buttons.

---

## Custom Command Buttons

Add `DxEditorButton` components inside a `<Buttons>` tag to display custom icon buttons inside the ComboBox:

```razor
<DxComboBox Data="@Employees"
            TextFieldName="@nameof(Employee.Text)"
            @bind-Value="@SelectedEmployee">
    <Buttons>
        <DxEditorButton IconCssClass="my-icon-add"
                        Tooltip="Add an employee"
                        Click="@(_ => OpenAddEmployeePopup())" />
    </Buttons>
</DxComboBox>

@code {
    record Employee(int Id, string FullName)
    {
        public string Text => FullName;
    }

    IEnumerable<Employee> Employees = new List<Employee>
    {
        new(1, "John Heart"),
        new(2, "Samantha Bright"),
        new(3, "Arthur Miller"),
    };
    Employee SelectedEmployee { get; set; }

    void OpenAddEmployeePopup() { /* show popup */ }

    protected override void OnInitialized()
        => SelectedEmployee = Employees.FirstOrDefault();
}
```

`DxEditorButton` properties:

| Property | Type | Description |
|---|---|---|
| `IconCssClass` | `string` | CSS class for the button icon |
| `Tooltip` | `string` | Tooltip text |
| `Click` | `EventCallback<MouseEventArgs>` | Button click handler |
| `Focusable` | `bool` | Whether the button is in the tab sequence |
| `Enabled` | `bool` | Whether the button is enabled |

---

## Cascading ComboBoxes

Create dependent ComboBoxes by handling `ValueChanged` on the parent and updating the child's `Data` source.

**Do not use `@bind-Value` on the parent** if you need custom logic on change — use separate `Value` and `ValueChanged`:

```razor
<div style="display:flex; gap:1rem;">
    <DxComboBox Data="@Countries"
                TextFieldName="@nameof(Country.Name)"
                Value="@SelectedCountry"
                ValueChanged="@((Country c) => OnCountryChanged(c))"
                NullText="Select country…" />

    <DxComboBox Data="@FilteredCities"
                TextFieldName="@nameof(City.Name)"
                @bind-Value="@SelectedCity"
                NullText="Select city…" />
</div>

@code {
    record Country(int Id, string Name);
    record City(int CountryId, string Name);

    static readonly List<Country> Countries = new()
    {
        new(1, "Germany"), new(2, "France"), new(3, "USA")
    };

    static readonly List<City> AllCities = new()
    {
        new(1, "Berlin"), new(1, "Munich"),
        new(2, "Paris"), new(2, "Lyon"),
        new(3, "New York"), new(3, "Chicago")
    };

    Country SelectedCountry { get; set; } = Countries[0];
    City SelectedCity { get; set; }
    List<City> FilteredCities { get; set; } = new();

    protected override void OnInitialized() => OnCountryChanged(SelectedCountry);

    void OnCountryChanged(Country country)
    {
        SelectedCountry = country;
        FilteredCities = AllCities.Where(c => c.CountryId == country.Id).ToList();
        SelectedCity = FilteredCities.FirstOrDefault();
    }
}
```

> **Key pattern**: Store `ValueChanged` separately (not `@bind-Value`) on the parent ComboBox so you can reset the child's selection when the parent changes.

> **CRITICAL — never use a bare method group for `ValueChanged`.**
> Blazor cannot implicitly convert a method group to `EventCallback<T>`, which causes **CS1503**.
> Always wrap the handler in a typed lambda:
>
> ```razor
> @* WRONG — causes CS1503 *@
> ValueChanged="@OnCountryChanged"
>
> @* CORRECT — explicit lambda with typed parameter *@
> ValueChanged="@((Country c) => OnCountryChanged(c))"
> ```

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| Clear button doesn't show | Set `ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto"` |
| Clear button missing in custom edit box template | Add `<DxInputBox>` inside `EditBoxDisplayTemplate` |
| Child ComboBox doesn't update when parent changes | Use `Value` + `ValueChanged` (not `@bind-Value`) on the parent, and update child `Data` in the handler |
| **CS1503**: cannot convert from 'method group' to 'EventCallback' | Never pass a bare method reference to `ValueChanged`. Use a typed lambda: `ValueChanged="@((Country c) => OnCountryChanged(c))"` |
| Command button not visible | Verify `<Buttons>` is a direct child of `<DxComboBox>`; check icon CSS class is loaded |
