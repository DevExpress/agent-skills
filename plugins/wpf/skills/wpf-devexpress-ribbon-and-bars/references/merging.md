# Merging

Bar and Ribbon merging combines the items of a *child* window (or document panel) with those of the *parent* — typical in MDI apps where each document brings its own commands but they should appear in the main window's toolbars / ribbon. DevExpress supports this **automatically** in `DockLayoutManager` scenarios, and **manually** through `Bar.Merge` / `RibbonControl.UnMerge` and related methods.

## When to Use This Reference

Use this when you need to:

- Show child-document commands inside the parent window's bars / ribbon
- Auto-merge when a `DocumentPanel` is maximized (regular MDI) or activated (tabbed MDI)
- Control which items merge / replace / hide via `MergeType` and `MergeOrder`
- Trigger merge / unmerge manually from code
- Disable merging for a specific panel
- Merge a `ThemedWindow`'s `ToolbarItems` / `HeaderItems` with a child panel's bars

## Which Controls Merge Automatically

Automatic merging happens when bars/ribbons are hosted inside DevExpress docking containers — `DockLayoutManager` with `DocumentGroup` + `DocumentPanel`. The triggers:

| Container layout | When merging fires |
|---|---|
| Regular MDI mode | A `DocumentPanel` is maximized |
| Tabbed MDI mode | A tab (`DocumentPanel`) is activated |

The reverse (unmerge) happens when:

- The maximized `DocumentPanel` is restored
- A tab is deactivated (e.g., user switches to another tab)

The following controls participate in automatic merging:

- **Bars/menus**: `BarManager`, standalone `MainMenuControl` / `ToolBarControl` / `StatusBarControl`, `Bar`
- **Ribbon**: `RibbonControl` (merges categories, pages, groups, items) and `RibbonStatusBarControl`
- **ThemedWindow**: `ThemedWindow.ToolbarItems` / `ThemedWindow.HeaderItems` (the panel's bars merge with these)

Bars and ribbons *not* inside a `DocumentPanel` (e.g., directly in a `LayoutPanel`) are **not merged by default** — see [Layout Panel Specifics](#layout-panel-specifics) below.

## Element Matching — How Parent and Child Items Are Paired

The merge mechanism pairs items between parent and child using these properties, in priority order:

1. **`MergingProperties.Name`** (attached property) — explicit, highest priority
2. **`x:Name`** (mapped to `BarItemLink.BarItemName`)
3. **`BarItem.Content`** (caption fallback)

Items that don't match are simply added to the parent.

## `MergeType` — What Happens to Matched Items

Set **`MergeType` on the CHILD's items / links** (not the parent's). Available values:

| `BarItemMergeType` value | Effect |
|---|---|
| `Add` (default) | Add the child item to the parent — both shown side by side |
| `MergeItems` | For `BarSubItem` only: merge the child's sub-menu items into the matching parent sub-menu's items |
| `Remove` | Remove the matched parent item — child's command effectively hides the parent's |
| `Replace` | Replace the parent item with the child item — child wins, parent is hidden |

### Ribbon Variant (`RibbonMergeType`)

The same values apply to ribbon elements via `RibbonMergeType` on `RibbonPageCategoryBase.MergeType`, `RibbonPage.MergeType`, `RibbonPageGroup.MergeType`. Bar items inside ribbon groups use `BarItemLinkBase.MergeType` (`BarItemMergeType`).

### Example — `MergeItems` for Sub-Menus

```xaml
<!-- Parent: a File submenu with two items -->
<dxb:MainMenuControl>
    <dxb:BarSubItem Content="File">
        <dxb:BarButtonItem Content="Open" MergeOrder="0" ItemClick="parent_bar_open"/>
        <dxb:BarButtonItem Content="Save" MergeOrder="1" ItemClick="parent_bar_save"/>
    </dxb:BarSubItem>
</dxb:MainMenuControl>

<!-- Child: a File submenu — MergeType="MergeItems" combines children inline -->
<dxb:MainMenuControl>
    <dxb:BarSubItem Content="File" MergeType="MergeItems">
        <dxb:BarButtonItem Content="Open" MergeOrder="0" MergeType="Replace" ItemClick="child_bar_open"/>
        <dxb:BarButtonItem Content="Save" MergeOrder="1" MergeType="Replace" ItemClick="child_bar_save"/>
        <dxb:BarButtonItem Content="Close" MergeOrder="2"/>
    </dxb:BarSubItem>
</dxb:MainMenuControl>
```

Result: a single `File` submenu showing Open / Save (with child's click handlers replacing parent's) plus the child-only Close item.

## `MergeOrder` — Display Order After Merging

Each merged item / page / group has a `MergeOrder` integer. Lower values display first; equal values keep parent → child ordering (parent items first).

```xaml
<!-- Parent Ribbon -->
<dxr:RibbonPage MergeOrder="0" Caption="Home"/>
<dxr:RibbonPage MergeOrder="2" Caption="View"/>

<!-- Child Ribbon -->
<dxr:RibbonPage MergeOrder="1" Caption="Insert"/>
<dxr:RibbonPage MergeOrder="3" Caption="Test"/>
<dxr:RibbonPage MergeOrder="4" Caption="Add-ins"/>
```

Merged order: **Home (0) → Insert (1) → View (2) → Test (3) → Add-ins (4)**.

Merge processing order, for both bars and ribbons:

1. `RibbonPageCategory`
2. `RibbonPage`
3. `RibbonPageGroup`
4. Bar items / commands inside groups

Each level has its own independent `MergeType` and `MergeOrder` properties.

## Default MDI Merge Behavior — `MDIMergeStyle`

`DockLayoutManager.MDIMergeStyle` controls the default trigger for all child panels:

| `MDIMergeStyle` | When merging happens |
|---|---|
| `WhenChildActivated` | Tabbed MDI: when a tab activates. Regular MDI: when a panel maximizes. |
| `WhenLoadedOrChildActivated` | Same as above, plus when the parent first loads |
| `Always` | Merge all child panels at all times |
| `Never` | Never auto-merge |

```xaml
<dxdo:DockLayoutManager MDIMergeStyle="Always">
    ...
</dxdo:DockLayoutManager>
```

### Per-Panel Override — `DocumentPanel.MDIMergeStyle`

Attached property on a single panel; takes priority over the `DockLayoutManager` setting:

```xaml
<dxdo:DocumentPanel dxdo:DocumentPanel.MDIMergeStyle="Never">
    ...
</dxdo:DocumentPanel>
```

### Block a Ribbon from Being Merged

To prevent a specific `RibbonControl` or `RibbonStatusBarControl` from ever being merged:

```xaml
<dxr:RibbonControl MDIMergeStyle="Never">
    ...
</dxr:RibbonControl>
```

## Disable Merging for a Single Panel

```xaml
<dxdo:DocumentPanel Caption="Document1"
                    dxb:MergingProperties.ElementMergingBehavior="None">
    ...
</dxdo:DocumentPanel>
```

`ElementMergingBehavior.None` opts the panel out entirely; merging logic skips it.

## Manual Merging (No DockLayoutManager)

Outside MDI scenarios — or when you populate bars from a ViewModel and the timing matters — you can drive merges yourself. The classic pattern: disable auto-merge, then merge after the bars finish loading.

### Merge / Unmerge Two Bars

```csharp
// Both bars must exist; usually the parent's Bar is referenced by x:Name.
parentBar.Merge(childBar);

// Later:
parentBar.UnMerge();
```

### Merge / Unmerge Sub-Menus

Both `BarSubItem` and `PopupMenu` implement `ILinksHolder`:

```csharp
parentSubItem.Merge(childSubItem);
parentSubItem.UnMerge();
```

### Merge / Unmerge Ribbons

```csharp
parentRibbon.Merge(childRibbon);

// Later — unmerge a specific child:
parentRibbon.UnMerge(childRibbon);

// Or reset to the original layout (no arguments):
parentRibbon.UnMerge();
```

`RibbonStatusBarControl` has the same `Merge` / `UnMerge` methods.

### Listen to DockLayoutManager Events

For MDI scenarios where you need to intercept the auto-merge (e.g., to add cross-cutting logic), handle:

| Event | Fires |
|---|---|
| `DockLayoutManager.Merge` | Just before automatic merge — you can perform custom merge logic here |
| `DockLayoutManager.UnMerge` | Just before automatic unmerge — also handles undoing any manual `Bar.Merge` you performed in the `Merge` event |

## Merge a Panel's Bars with `ThemedWindow.ToolbarItems` / `HeaderItems`

If a `ThemedWindow` defines bar items in `ThemedWindow.ToolbarItems` or `HeaderItems`, an activated child panel's bars merge with these collections automatically. The control-box buttons (minimize, maximize, close) end up on the same bar as the window's items.

Rule: when both `ToolbarItems` and `HeaderItems` are populated, the panel merges into **`HeaderItems`** preferentially.

## Layout Panel Specifics

Bars/ribbons inside `LayoutPanel` (not `DocumentPanel`) **do NOT merge by default**. To opt in:

```xaml
<Style TargetType="dxdo:LayoutPanel">
    <Setter Property="dxb:MergingProperties.ElementMergingBehavior"
            Value="InternalWithExternal"/>
</Style>
```

`InternalWithExternal` lets the `LayoutPanel`'s bars participate in merging with the parent container's bars.

## Common Issues

- **Items duplicated after merge** — both parent and child items present and no `MergeType` set on the child → defaults to `Add`. Use `Replace` or `MergeItems` if you want the child to take over.
- **Items appear in unexpected order** — `MergeOrder` not set explicitly; defaults to `0` everywhere, falling back to source order. Set explicit `MergeOrder` per item.
- **No merge despite `DockLayoutManager`** — child bars/ribbons are inside a `LayoutPanel` (not a `DocumentPanel`), or `MDIMergeStyle` is `Never`.
- **Sub-menu items don't combine** — used `MergeType="Add"` on a child `BarSubItem` instead of `MergeType="MergeItems"`. `Add` makes a *second* "File" sub-menu next to the parent's.
- **`MergingProperties.Name` ignored** — must be set on **both** parent and child items, with the same value.
- **Manual `Bar.Merge` call lost on tab switch** — `DockLayoutManager.UnMerge` event undoes manual merges. Re-merge in the next `Merge` event.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/merging.md` (`xref:118293`)
- `articles/controls-and-libraries/layout-management/dock-windows/mdi-bar-merging.md` (`xref:9155`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/features/mdi-ribbon-merging.md` (`xref:10587`)
