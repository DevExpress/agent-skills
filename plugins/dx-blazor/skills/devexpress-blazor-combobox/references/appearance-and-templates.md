# ComboBox — Appearance Customization & Templates

**When to use this reference**: Load this reference when customising the size, styling, drop-down width/direction, or item/edit-box templates of `DxComboBox`.

---

## Size Modes

Use `SizeMode` to apply a predefined size to the ComboBox:

```razor
<DxComboBox Data="@Cities" @bind-Value="@Value" SizeMode="SizeMode.Small" NullText="Small" />
<DxComboBox Data="@Cities" @bind-Value="@Value" SizeMode="SizeMode.Medium" NullText="Medium" />
<DxComboBox Data="@Cities" @bind-Value="@Value" SizeMode="SizeMode.Large" NullText="Large" />

@code {
    IEnumerable<string> Cities = new List<string> { "London", "Berlin", "Paris" };
    string Value { get; set; }
}
```

| Value | Description |
|---|---|
| `SizeMode.Small` | Compact input |
| `SizeMode.Medium` | Default size |
| `SizeMode.Large` | Enlarged input |

---

## Input Appearance

Apply a custom CSS class to the input element with `InputCssClass`:

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@Value"
            InputCssClass="my-combobox-input" />
```

```css
.my-combobox-input {
    border: 2px dotted orange !important;
}
```

---

## Drop-Down List Width

Control the drop-down window width with `DropDownWidthMode`:

| Value | Description |
|---|---|
| `ContentOrEditorWidth` (default) | Width fits the longest item text; minimum width equals the editor |
| `ContentWidth` | Width equals the longest item text only |
| `EditorWidth` | Width matches the editor exactly; long items wrap to multiple lines |

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@Value"
            DropDownWidthMode="DropDownWidthMode.EditorWidth" />
```

---

## Drop-Down Window Direction

Use `DropDownDirection` to control which direction the drop-down opens:

```razor
<DxComboBox Data="@Cities"
            @bind-Value="@Value"
            NullText="Select City …"
            DropDownDirection="DropDownDirection.Up" />

@code {
    IEnumerable<string> Cities = new List<string> { "London", "Berlin", "Paris" };
    string Value { get; set; }
}
```

---

## Item Display Template

Use `ItemDisplayTemplate` to completely replace how each item is rendered in the drop-down list. The template receives a `ComboBoxItemDisplayTemplateContext<TData>` as `context`:

| Context Member | Description |
|---|---|
| `context.DataItem` | The raw data item object |
| `context.DisplayText` | The text that would be shown by default |
| `context.Value` | The item value |
| `context.VisibleIndex` | Zero-based position in the visible list |

```razor
<DxComboBox Data="@Employees"
            @bind-Value="@SelectedEmployee"
            InputId="cbItemTemplate">
    <ItemDisplayTemplate>
        <div style="display:flex; align-items:center; gap:0.5rem">
            <div style="width:2rem; height:2rem; border-radius:50%; background:#6c757d;
                        color:white; display:flex; align-items:center; justify-content:center;
                        font-size:0.8rem; flex-shrink:0;">
                @context.DataItem.FullName[0]
            </div>
            <span>@context.DataItem.FullName — @context.DataItem.Phone</span>
        </div>
    </ItemDisplayTemplate>
</DxComboBox>

@code {
    record Employee(string FullName, string Phone);

    IEnumerable<Employee> Employees = new List<Employee>
    {
        new("John Heart",      "+1 (213) 555-9392"),
        new("Samantha Bright", "+1 (818) 555-2655"),
        new("Arthur Miller",   "+1 (310) 555-8244"),
    };
    Employee SelectedEmployee { get; set; }

    protected override void OnInitialized()
        => SelectedEmployee = Employees.FirstOrDefault();
}
```

---

## Edit Box Display Template

Use `EditBoxDisplayTemplate` to customise the value shown in the edit box when an item is selected. The template receives a `ComboBoxEditBoxDisplayTemplateContext<TData, TValue>` as `context`.

> **Note**: If you need the Clear button or search/filter functionality inside the edit box display template, add a `DxInputBox` component to the template markup.

```razor
<DxComboBox Data="@Employees"
            @bind-Value="@SelectedEmployee">
    <EditBoxDisplayTemplate>
        <span>@context.DataItem?.FullName (@context.DataItem?.Phone)</span>
    </EditBoxDisplayTemplate>
</DxComboBox>
```

---

## Empty Data Area Template

Show custom content when the drop-down list has no items matching the search text:

```razor
<DxComboBox Data="@Cities" @bind-Value="@Value">
    @* Use DxComboBoxSettings to access EmptyDataAreaTemplate if needed *@
</DxComboBox>
```

> Use `DxComboBoxSettings` with `EmptyDataAreaTemplate` when the ComboBox is embedded in a grid cell editor context. For standalone use, the ComboBox displays a built-in "No data to display" message automatically.

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| `ItemDisplayTemplate` context is null | Ensure `Data` is populated before the drop-down opens |
| Clear button doesn't appear in custom `EditBoxDisplayTemplate` | Add `<DxInputBox>` inside the template markup |
| Drop-down opens in the wrong direction | Set `DropDownDirection` explicitly |
| Custom CSS not applied | Check CSS specificity; use `!important` if overriding DevExpress defaults |
