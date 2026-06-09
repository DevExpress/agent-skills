# Adaptivity and Appearance — DevExpress Blazor Toolbar

## When to Use This Reference

- Configuring adaptive layout for narrow containers or mobile screens
- Applying render styles (Contained, Plain) to the toolbar or individual items
- Setting size mode (Small, Medium, Large)
- Controlling drop-down display modes
- Customizing the toolbar title
- Binding the toolbar to a data collection

## Adaptivity

`DxToolbar` can automatically hide item text or move items to an overflow submenu when the container is too narrow to show everything.

### Collapse Items to Icons

When the container narrows, items that have an `IconCssClass` collapse to icon-only display (text hidden):

```razor
<DxToolbar AdaptivityAutoCollapseItemsToIcons="true">
    <DxToolbarItem Text="Bold"   IconCssClass="oi oi-bold" />
    <DxToolbarItem Text="Italic" IconCssClass="oi oi-italic" />
    <DxToolbarItem Text="Save"   IconCssClass="oi oi-cloud-upload" />
</DxToolbar>
```

### Hide Root Items to Overflow Submenu

When there is insufficient space, root-level items without enough room are moved to an overflow drop-down button:

```razor
<DxToolbar AdaptivityAutoHideRootItems="true"
           AdaptivityMinRootItemCount="2">
    <DxToolbarItem Text="Bold"   IconCssClass="oi oi-bold"   AdaptivePriority="1" AdaptiveText="Bold" />
    <DxToolbarItem Text="Italic" IconCssClass="oi oi-italic" AdaptivePriority="1" AdaptiveText="Italic" />
    <DxToolbarItem Text="Undo"   IconCssClass="oi oi-action-undo" AdaptivePriority="2" AdaptiveText="Undo" />
    <DxToolbarItem Text="Redo"   IconCssClass="oi oi-action-redo" AdaptivePriority="2" AdaptiveText="Redo" />
</DxToolbar>
```

### Adaptivity Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `AdaptivityAutoCollapseItemsToIcons` | `bool` | `false` | Collapses item text when space is limited |
| `AdaptivityAutoHideRootItems` | `bool` | `false` | Moves root items to overflow submenu |
| `AdaptivityMinRootItemCount` | `int` | `1` | Keep at least N root items visible |

### Per-Item Adaptive Properties

| Property | Type | Description |
|---|---|---|
| `AdaptivePriority` | `int` | Items with lower priority are hidden first (default: 0 = last to hide) |
| `AdaptiveText` | `string` | Text used when the item appears in the overflow submenu |

## Render Styles

Control the visual fill style of toolbar items.

### Toolbar-Level Style

Applies the same style to all items:

```razor
<!-- Contained (filled background) -->
<DxToolbar ItemRenderStyleMode="ToolbarRenderStyleMode.Contained">
    <DxToolbarItem Text="Save" RenderStyle="ButtonRenderStyle.Success" />
    <DxToolbarItem Text="Delete" RenderStyle="ButtonRenderStyle.Danger" />
</DxToolbar>

<!-- Plain (no background, bordered) -->
<DxToolbar ItemRenderStyleMode="ToolbarRenderStyleMode.Plain">
    <DxToolbarItem Text="Action" />
</DxToolbar>
```

### Per-Item Style

Override the style for a specific item:

```razor
<DxToolbar>
    <DxToolbarItem Text="Bold" />
    <DxToolbarItem Text="Delete"
                   RenderStyle="ButtonRenderStyle.Danger"
                   RenderStyleMode="ButtonRenderStyleMode.Contained" />
</DxToolbar>
```

### Available Render Styles (`ButtonRenderStyle`)

`Primary`, `Secondary`, `Success`, `Info`, `Warning`, `Danger`, `Dark`, `Light`, `Link`

## Size Mode

Control the size of toolbar items. Applies to the component and all its children:

```razor
<DxToolbar SizeMode="SizeMode.Small">
    <DxToolbarItem Text="Compact Action" />
</DxToolbar>
```

Available: `Small`, `Medium` (default), `Large`.

## Drop-Down Display Mode

Control how sub-menus are shown. Set at the toolbar level or per item:

```razor
<!-- All drop-downs show as modal dialogs -->
<DxToolbar DropDownDisplayMode="DropDownDisplayMode.ModalDialog">
    <DxToolbarItem Text="Options">
        <Items>
            <DxToolbarItem Text="Settings" />
            <DxToolbarItem Text="Preferences" />
        </Items>
    </DxToolbarItem>
</DxToolbar>

<!-- Bottom sheet for mobile UX -->
<DxToolbar DropDownDisplayMode="DropDownDisplayMode.ModalBottomSheet">
    <DxToolbarItem Text="Share">
        <Items>
            <DxToolbarItem Text="Copy Link" />
            <DxToolbarItem Text="Send by Email" />
        </Items>
    </DxToolbarItem>
</DxToolbar>
```

## Toolbar Title

Display a static title or use a custom template:

```razor
<!-- Static title text -->
<DxToolbar Title="My Document">
    <DxToolbarItem Text="Save" />
</DxToolbar>

<!-- Custom title template (e.g., editable field) -->
<DxToolbar>
    <TitleTemplate>
        <InputText @bind-Value="documentTitle"
                   class="form-control form-control-sm"
                   style="width: 200px;" />
    </TitleTemplate>
    <Items>
        <DxToolbarItem Text="Save" />
    </Items>
</DxToolbar>

@code {
    string documentTitle = "Untitled";
}
```

## Data Binding

Bind the toolbar to a flat or hierarchical data collection. Use `DxToolbarDataMapping` to map data properties to toolbar item properties:

```razor
<DxToolbar Data="@menuItems">
    <DataMappings>
        <DxToolbarDataMapping Text="Label" Key="Id" ParentKey="ParentId" />
    </DataMappings>
</DxToolbar>

@code {
    record MenuItem(int Id, int? ParentId, string Label);

    List<MenuItem> menuItems = new() {
        new(1, null, "File"),
        new(2, 1,    "New"),
        new(3, 1,    "Open"),
        new(4, 1,    "Save"),
        new(5, null, "Edit"),
        new(6, 5,    "Cut"),
        new(7, 5,    "Copy"),
        new(8, 5,    "Paste"),
    };
}
```

### DxToolbarDataMapping Properties

| Property | Description |
|---|---|
| `Text` | Data field mapped to item label |
| `Key` | Data field mapped to the item's unique key |
| `ParentKey` | Data field mapped to the parent item's key (for hierarchical data) |

Top-level items have `ParentKey` value = `null` (or the default of the key type).
