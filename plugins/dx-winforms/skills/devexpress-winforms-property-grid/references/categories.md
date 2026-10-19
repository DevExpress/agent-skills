# Categories

Categories group related property rows under collapsible header rows. The Property Grid creates category rows automatically from `[Category]` attributes, or you can define them manually in code or the designer.

## When to Use This Reference

- Grouping properties automatically with the `[Category]` attribute
- Creating `CategoryRow` objects in code
- Expanding/collapsing or pinning (fixing) categories, and configuring indent style
- Hiding root categories (`ShowRootCategories`)
- Organizing properties into Office View tabs via `TabPanelCustomize`

---

## Attribute-Based Categories (Automatic)

Apply `[Category]` to model properties. The Property Grid creates a `CategoryRow` for each distinct category name and places the matching properties beneath it.

```csharp
using System.ComponentModel;

public class ConnectionSettings {
    [Category("Connection")]
    [DisplayName("Host")]
    public string Host { get; set; } = "localhost";

    [Category("Connection")]
    [DisplayName("Port")]
    public int Port { get; set; } = 5432;

    [Category("Security")]
    [DisplayName("Use TLS")]
    public bool UseTls { get; set; }

    [Category("Security")]
    [DisplayName("Certificate Path")]
    public string CertificatePath { get; set; } = string.Empty;

    // No [Category] → placed under "Misc" category
    public string Notes { get; set; } = string.Empty;
}
```

Properties without `[Category]` appear under a **Misc** category row.

To hide the root category headers entirely:

```csharp
propertyGridControl1.OptionsView.ShowRootCategories = false;
```

---

## Expand/Collapse Categories

```csharp
// All rows start expanded; collapse a specific category
propertyGridControl1.GetRowByCaption("Security").Expanded = false;

// Expand by caption
propertyGridControl1.GetRowByCaption("Connection").Expanded = true;

// Expand all rows
foreach (BaseRow row in propertyGridControl1.Rows)
    row.Expanded = true;
```

To collapse all category rows on load:

```csharp
void propertyGridControl1_DataSourceChanged(object sender, EventArgs e) {
    foreach (BaseRow row in propertyGridControl1.Rows)
        if (row is CategoryRow cat)
            cat.Expanded = false;
}
```

---

## Creating Category Rows in Code

If you build rows manually, create `CategoryRow` objects and add `EditorRow` children:

```csharp
// Create rows first
var hostRow = new EditorRow { };
hostRow.Properties.FieldName = "Host";
hostRow.Properties.Caption   = "Host Name";

var portRow = new EditorRow { };
portRow.Properties.FieldName = "Port";
portRow.Properties.Caption   = "Port";

// Create category and add children
var connCategory = new CategoryRow();
connCategory.Properties.Caption = "Connection";
connCategory.ChildRows.AddRange(new BaseRow[] { hostRow, portRow });
connCategory.Expanded = true;

propertyGridControl1.Rows.Add(connCategory);
propertyGridControl1.SelectedObject = new ConnectionSettings();
```

---

## Nested Rows (Row Hierarchy Without CategoryRow)

To create a parent-child relationship between editor rows (no category header, just indented nesting):

```csharp
var parentRow = new EditorRow();
parentRow.Properties.FieldName = "Timeout";
parentRow.Properties.Caption   = "Timeout";

var childRow = new EditorRow();
childRow.Properties.FieldName = "RetryCount";
childRow.Properties.Caption   = "Retry Count";

parentRow.ChildRows.Add(childRow);
propertyGridControl1.Rows.Add(parentRow);
```

---

## Fixed (Pinned) Categories

A category can be pinned to the top or bottom so it stays visible while scrolling:

```csharp
propertyGridControl1.GetRowByCaption("General").Fixed = DevExpress.XtraVerticalGrid.Rows.FixedStyle.Top;
```

---

## Category Indentation Options

```csharp
// Indent style for category levels
propertyGridControl1.OptionsView.CategoryLevelIndentStyle =
    DevExpress.XtraVerticalGrid.CategoryLevelIndentStyle.Vertical;

// Remove left indent at the root level
propertyGridControl1.OptionsView.ShowRootLevelIndent = false;
```

---

## Expand Button Style

```csharp
// Default, ExplorerBar, or TreeView
propertyGridControl1.TreeButtonStyle =
    DevExpress.XtraVerticalGrid.TreeButtonStyle.Default;
```

---

## Office View: Organize into Tabs (Instead of Categories)

In Office view, categories can be replaced by tabs. Properties not assigned to any tab are not shown.

```csharp
propertyGridControl1.ActiveViewType = PropertyGridView.Office;
propertyGridControl1.TabPanelCustomize += OnTabPanelCustomize;
propertyGridControl1.SelectedObject = new ConnectionSettings();

void OnTabPanelCustomize(object sender,
        DevExpress.XtraVerticalGrid.Events.TabPanelCustomizeEventArgs e) {
    var tab1 = new Tab { Caption = "Connection" };
    tab1.CategoryNames.Add("Connection");   // include entire category
    tab1.FieldNames.Add("Notes");           // include individual property

    var tab2 = new Tab { Caption = "Security" };
    tab2.CategoryNames.Add("Security");

    e.Tabs.Add(tab1);
    e.Tabs.Add(tab2);
    propertyGridControl1.SelectedTab = propertyGridControl1.Tabs[0];
}
```

If a `[Category]` name contains spaces, replace them with underscores in `CategoryNames.Add`:
e.g. `"Window Style"` → `tab.CategoryNames.Add("Window_Style")`.

---

## Source Material

- Rows (categories section): `https://docs.devexpress.com/content/WindowsForms/401851?md=true`
- Office View (tabs): `https://docs.devexpress.com/content/WindowsForms/120262?md=true`
