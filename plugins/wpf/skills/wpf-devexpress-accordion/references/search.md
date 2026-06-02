# Search

`AccordionControl` has a built-in search box that filters items as the user types. Enable it with one property (`ShowSearchControl`), customize behavior with a small set of additional properties, and hook the `CustomItemFilter` event for arbitrary filter logic. Works for both static XAML items and data-bound items, with a couple of subtle differences.

## When to Use This Reference

Use this when you need to:

- Show the search box (`ShowSearchControl`)
- Add a search hint (`SearchControlNullText`)
- Change the filter condition (`Contains`, `Equals`, etc.)
- Add extra searchable text per item (`SearchTag`)
- Implement custom filter logic (`CustomItemFilter`)
- Understand how search applies to bound items via `DisplayMemberPath` or templates

## Enable the Search Box

```xaml
<dxa:AccordionControl ShowSearchControl="True">
    ...
</dxa:AccordionControl>
```

`ShowSearchControl` defaults to `false`; setting it to `true` adds a search field at the top of the panel. End users can type to filter items live.

> Tip: a smart AI-powered search is also available as an extension — see DevExpress Smart Search documentation (`xref:405385`).

## What's Searched

By default, items are matched against their **header text** (or `Header.ToString()` for non-string headers). The behavior differs slightly between static items and data-bound items:

| Population mode | Default match basis |
|---|---|
| Static `AccordionItem` in XAML | `AccordionItem.Header` (with `SearchTag` as an optional supplement) |
| Bound + `DisplayMemberPath` | The bound property the display member points to |
| Bound + `DataTemplate` | `item.ToString()` on the underlying data object |

`SearchTag` only works for **explicitly defined** (XAML) items — not data-bound ones.

## Search Tags (Static Items Only)

When an item's header is a custom UI (an editor, a complex layout) rather than plain text, users can't find it by typing related words. Add a `SearchTag`:

```xaml
<dxa:AccordionControl ShowSearchControl="True">
    <dxa:AccordionItem Header="Volume" Glyph="..." SearchTag="sound audio"/>
    <dxa:AccordionItem SearchTag="edit input">
        <dxa:AccordionItem.Header>
            <dxe:TrackBarEdit/>
        </dxa:AccordionItem.Header>
    </dxa:AccordionItem>
</dxa:AccordionControl>
```

Items match if their `SearchTag` OR `Header` matches the query.

> `SearchTag` works only for items defined explicitly in XAML. For data-bound items, expose searchable text via the property used as `DisplayMemberPath` or override `ToString()` on the data class.

## Data-Bound Filtering

The bound `AccordionControl` filters based on data source values. Two paths:

| Binding type | What's searched |
|---|---|
| `DisplayMemberPath` | The displayed value (the property `DisplayMemberPath` points to) |
| `DataTemplate` (any binding approach) | `item.ToString()` on the underlying data object |

So:

```xaml
<dxa:AccordionControl ItemsSource="{Binding Sections}"
                      ChildrenPath="Children"
                      DisplayMemberPath="Title"
                      ShowSearchControl="True"/>
```

Search matches against `Section.Title` (and the children's `Title` recursively).

If you use a `DataTemplate` to render items, override `ToString()` on the data class to make search work:

```csharp
public class Section {
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public override string ToString() => $"{Title} {Description}";  // search matches both
}
```

> **Search does not work with `BindingList<T>`.** Use `List<T>`, `ObservableCollection<T>`, or another `IEnumerable` implementation.

## Customize the Filter Condition

By default, the search uses **Contains** matching (substring, case-insensitive). Override:

```xaml
<dxa:AccordionControl ShowSearchControl="True"
                      SearchControlFilterCondition="StartsWith"/>
```

Available `SearchControlFilterCondition` values:

- `Contains` (default)
- `Equals`
- `Like`
- `StartsWith`
- `DoesNotEqual`
- `DoesNotContain`
- `EndsWith`
- ... and other standard filter operators

## Customize Search Box Hint Text

The grey "type to search" placeholder:

```xaml
<dxa:AccordionControl ShowSearchControl="True"
                      SearchControlNullText="Search the menu..."/>
```

## Set Search Text in Code

`SearchText` reads / writes the current query:

```xaml
<dxa:AccordionControl ShowSearchControl="True"
                      SearchText="{Binding ActiveQuery, Mode=TwoWay}"/>
```

Useful for restoring a search session across navigation, or pre-filtering based on context.

## Custom Filter Logic — `CustomItemFilter` Event

When the built-in filter conditions don't fit, hook `CustomItemFilter` for full control:

```xaml
<dxa:AccordionControl ShowSearchControl="True"
                      CustomItemFilter="OnCustomItemFilter">
    <dxa:AccordionItem Header="Item1"/>
    <dxa:AccordionItem Header="Item2"/>
    <dxa:AccordionItem Header="Item3"/>
</dxa:AccordionControl>
```

```csharp
private void OnCustomItemFilter(object sender, AccordionCustomItemFilterEventArgs e) {
    // e.Item is the AccordionItem (or data object if bound)
    // e.SearchText is the current query
    // Set e.Accepted = true/false

    if (e.Item is AccordionItem item && item.Header?.ToString() == "Item1") {
        e.Accepted = false;  // always hide Item1
        return;
    }

    // Custom multi-field matching, fuzzy match, regex, etc.
    e.Accepted = ApplyCustomMatch(e.Item, e.SearchText);
}
```

The event fires once per item per search keystroke. Keep the logic fast for responsive search.

## Combining Multiple Customizations

```xaml
<dxa:AccordionControl ShowSearchControl="True"
                      SearchControlNullText="Search settings..."
                      SearchControlFilterCondition="StartsWith"
                      SearchText="{Binding Query, Mode=TwoWay}"
                      CustomItemFilter="OnCustomFilter"
                      ItemsSource="{Binding Settings}"
                      ChildrenPath="Children"
                      DisplayMemberPath="Caption"/>
```

When both `CustomItemFilter` and the built-in `SearchControlFilterCondition` are set, the custom event runs first; if it sets `e.Accepted` explicitly, the built-in filter is bypassed.

## Search and Hierarchy

When a child matches the query, its parent is also shown (otherwise the child would be unreachable in the tree). Conversely, if a parent matches, all its children remain visible — search expands matches in context.

## Common Patterns

### Quick Settings Search

```xaml
<dxa:AccordionControl ItemsSource="{Binding SettingCategories}"
                      ChildrenPath="Settings"
                      DisplayMemberPath="Name"
                      ShowSearchControl="True"
                      SearchControlNullText="Find a setting..."/>
```

### Searchable Static Sidebar with Custom UI Headers

```xaml
<dxa:AccordionControl ShowSearchControl="True">
    <dxa:AccordionItem Header="Dashboard" Glyph="..." SearchTag="home overview"/>
    <dxa:AccordionItem Header="Reports" Glyph="..." SearchTag="analytics statistics">
        <dxa:AccordionItem Header="Sales" SearchTag="revenue orders"/>
        <dxa:AccordionItem Header="Inventory" SearchTag="stock products"/>
    </dxa:AccordionItem>
</dxa:AccordionControl>
```

### Smart Search via Custom Filter

```csharp
private void OnCustomItemFilter(object sender, AccordionCustomItemFilterEventArgs e) {
    if (string.IsNullOrEmpty(e.SearchText)) {
        e.Accepted = true;
        return;
    }

    var text = e.SearchText.Trim();
    var data = e.Item is AccordionItem ai ? ai.DataContext : e.Item;

    e.Accepted = data switch {
        Setting s => s.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                  || s.Description.Contains(text, StringComparison.OrdinalIgnoreCase)
                  || s.Synonyms.Any(syn => syn.Contains(text, StringComparison.OrdinalIgnoreCase)),
        _ => true,
    };
}
```

This matches against name, description, and synonyms — well beyond what `DisplayMemberPath` alone can do.

## Common Issues

- **Search box doesn't appear** — `ShowSearchControl` defaults to `false`. Set it to `true`.
- **`SearchTag` ignored on data-bound items** — by design. `SearchTag` only applies to explicitly defined XAML items. For bound items, make text searchable via `DisplayMemberPath` or `ToString()`.
- **Search shows nothing for `BindingList<T>`** — not supported. Switch to `List<T>` or `ObservableCollection<T>`.
- **Search matches against class name instead of intended text** — `DisplayMemberPath` not set and no `ToString()` override. Either set `DisplayMemberPath` or override `ToString()` on the data class.
- **`CustomItemFilter` fires but filter doesn't apply** — `e.Accepted` not set explicitly. Set it `true` or `false` for every item; the default keeps the existing visibility.
- **Filter condition ignored** — `SearchControlFilterCondition` doesn't apply when `CustomItemFilter` overrides matching. Pick one approach.
- **Search expands tree unexpectedly** — by design: matching children show their parents to keep the result reachable. Use `CustomItemFilter` if you need flat results.

## Source Material

- `articles/controls-and-libraries/navigation-controls/accordion-control/searching.md` (`xref:118715`)
