# Data Binding and Populating Items

`AccordionControl` is **not a data-bound control** in the traditional WinForms sense — it has no `DataSource` / `DataMember` properties. All elements (`AccordionControlElement`) are added explicitly to the `Elements` collection. This page covers the two patterns for populating the control and explains when to use each.

## When to Use This Reference

- Deciding how to populate the accordion (static vs runtime vs data-driven)
- Building items from a collection or database query
- Refreshing items when the underlying data changes

## Overview of Population Strategies

| Strategy | Best For | API |
|---|---|---|
| **Code — static items** | Fixed menus defined in code at startup | `accordion.Elements.Add(...)` at form init |
| **Code — data-driven** | Items generated from a collection, config file, or database | Iterate collection → create `AccordionControlElement` per record |

There is no `ItemsSource`/`DataSource` equivalent. Both strategies ultimately write to `AccordionControl.Elements` or `AccordionControlElement.Elements`.

## Strategy 1 — Code, Static Items

Populate in the constructor or `Form_Load` when the menu is fixed but defined in code:

```csharp
void BuildMenu(AccordionControl accordion) {
    accordion.BeginUpdate();
    try {
        // Group
        var grpFile = new AccordionControlElement(ElementStyle.Group) {
            Text = "File",
            Expanded = true,
            ImageOptions = { ImageUri = { Uri = "Open;Office2013" } }
        };

        // Items
        var itmNew  = new AccordionControlElement(ElementStyle.Item) { Text = "New",  Name = "itmNew" };
        var itmOpen = new AccordionControlElement(ElementStyle.Item) { Text = "Open", Name = "itmOpen" };
        var itmSave = new AccordionControlElement(ElementStyle.Item) { Text = "Save", Name = "itmSave" };

        grpFile.Elements.AddRange(new[] { itmNew, itmOpen, itmSave });
        accordion.Elements.Add(grpFile);
    }
    finally {
        accordion.EndUpdate();
    }
}
```

Always wrap bulk additions in `BeginUpdate()` / `EndUpdate()` — it suppresses per-item layout passes and is significantly faster for large menus.

## Strategy 2 — Code, Data-Driven

Generate items programmatically from a collection (list, query result, config section). This is the closest equivalent to XAML's `ItemsSource`.

### Example: Single-Level List from Database/Service

```csharp
public class NavigationItem {
    public string Title   { get; set; }
    public string Tag     { get; set; }
    public string IconUri { get; set; }
}

void BindFlatMenu(AccordionControl accordion, IEnumerable<NavigationItem> items) {
    accordion.BeginUpdate();
    accordion.Elements.Clear();

    foreach (var item in items) {
        var elem = new AccordionControlElement(ElementStyle.Item) {
            Text = item.Title,
            Tag  = item.Tag
        };
        elem.ImageOptions.ImageUri.Uri = item.IconUri;
        accordion.Elements.Add(elem);
    }

    accordion.EndUpdate();
}
```

### Example: Two-Level Grouped Menu

```csharp
public class MenuGroup {
    public string                  GroupTitle { get; set; }
    public List<NavigationItem>    Items      { get; set; }
}

void BindGroupedMenu(AccordionControl accordion, IEnumerable<MenuGroup> groups) {
    accordion.BeginUpdate();
    accordion.Elements.Clear();

    foreach (var g in groups) {
        var grp = new AccordionControlElement(ElementStyle.Group) {
            Text     = g.GroupTitle,
            Expanded = true
        };

        foreach (var item in g.Items) {
            var elem = new AccordionControlElement(ElementStyle.Item) {
                Text = item.Title,
                Tag  = item.Tag
            };
            grp.Elements.Add(elem);
        }

        accordion.Elements.Add(grp);
    }

    accordion.EndUpdate();
}
```

### Refreshing Items at Runtime

To refresh after the data source changes, clear and repopulate:

```csharp
void RefreshMenu(AccordionControl accordion, IEnumerable<MenuGroup> newData) {
    accordion.BeginUpdate();
    accordion.Elements.Clear();      // removes all existing elements
    // ... re-add elements from newData
    accordion.EndUpdate();
}
```

There is no incremental update API — a full clear + repopulate is the standard approach.

## Using `Tag` to Associate Data with Items

`AccordionControlElement.Tag` is the idiomatic way to carry a payload (ID, route, model object) alongside a menu item:

```csharp
var item = new AccordionControlElement(ElementStyle.Item) {
    Text = "Customers",
    Tag  = "/customers"      // or: Tag = customersViewModel
};

accordion.ElementClick += (s, e) => {
    if (e.Element.Style == ElementStyle.Group) return;
    var route = e.Element.Tag as string;
    Navigate(route);
};
```

## Reading the Selected Element

```csharp
// Programmatic selection (requires AllowItemSelection = true)
accordion.AllowItemSelection = true;
accordion.SelectedElement = targetItem;

// Read back
AccordionControlElement selected = accordion.SelectedElement;
```

## Common Issues

| Symptom | Cause | Solution |
|---|---|---|
| Items appear in wrong order | `Add` called after `Elements.Clear` without `BeginUpdate` | Wrap in `BeginUpdate` / `EndUpdate` |
| Group header missing after rebind | `Expanded` not set | Set `Expanded = true` on root groups |
| Data changes not reflected | No refresh call | Call `Elements.Clear()` + repopulate inside `BeginUpdate/EndUpdate` |
| Cannot find item by ID at runtime | No identifier stored | Store ID/route in `Tag`; search with LINQ over `Elements` |

## Source Material

- `articles/114553` — Accordion Control overview (Items and Groups section)
- `articles/120496` — How To: Create AccordionControl in code
- `articles/DevExpress.XtraBars.Navigation.AccordionControl` — AccordionControl class reference
- `articles/DevExpress.XtraBars.Navigation.AccordionControlElement` — AccordionControlElement class reference
