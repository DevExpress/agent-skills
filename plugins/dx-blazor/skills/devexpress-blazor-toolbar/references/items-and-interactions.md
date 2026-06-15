# Items and Interactions — DevExpress Blazor Toolbar

## When to Use This Reference

- Adding drop-down sub-menus to toolbar items
- Creating checked or radio-group items
- Handling click events
- Adding icons, navigation links, and tooltips
- Using custom item templates
- Submitting a form from a toolbar button

## Item Alignment

Use `Alignment="ToolbarItemAlignment.Right"` to push items to the right side of the toolbar:

```razor
<DxToolbar>
    <DxToolbarItem Text="New" />
    <DxToolbarItem Text="Save" />
    <DxToolbarItem Text="Settings"
                   IconCssClass="oi oi-cog"
                   Alignment="ToolbarItemAlignment.Right" />
</DxToolbar>
```

## Visual Separators

Use `BeginGroup="true"` to insert a vertical separator before an item:

```razor
<DxToolbar>
    <DxToolbarItem Text="Cut" />
    <DxToolbarItem Text="Copy" />
    <DxToolbarItem Text="Paste" />
    <DxToolbarItem Text="Undo" BeginGroup="true" />
    <DxToolbarItem Text="Redo" />
</DxToolbar>
```

## Drop-Down Sub-Menus

Nest `DxToolbarItem` components inside a parent item's `Items` tag to create a drop-down:

```razor
<DxToolbar>
    <DxToolbarItem Text="Format">
        <Items>
            <DxToolbarItem Text="Bold" />
            <DxToolbarItem Text="Italic" />
            <DxToolbarItem Text="Underline" />
        </Items>
    </DxToolbarItem>
    <DxToolbarItem Text="Insert">
        <Items>
            <DxToolbarItem Text="Image" />
            <DxToolbarItem Text="Table" />
            <DxToolbarItem Text="Link" />
        </Items>
    </DxToolbarItem>
</DxToolbar>
```

Control how the sub-menu appears with `DropDownDisplayMode` on the toolbar or individual item:

```razor
<!-- Modal dialog for this item only -->
<DxToolbarItem Text="Help"
               DropDownDisplayMode="DropDownDisplayMode.ModalDialog">
    <Items>
        <DxToolbarItem Text="Documentation" />
        <DxToolbarItem Text="About" />
    </Items>
</DxToolbarItem>
```

Available values: `DropDown` (default), `ModalDialog`, `ModalBottomSheet`.

## Click Events

Handle clicks with the `Click` event callback:

```razor
<DxToolbar>
    <DxToolbarItem Text="Delete" Click="OnDeleteClicked" />
</DxToolbar>

@code {
    void OnDeleteClicked(ToolbarItemClickEventArgs e) {
        Console.WriteLine($"Clicked: {e.ItemName}");
    }
}
```

For items without child items, `Click` fires when the user clicks the item.

## Navigation Links

Use `NavigateUrl` for items that should navigate to a URL. These work even without interactive render mode:

```razor
<DxToolbar>
    <DxToolbarItem Text="Home" NavigateUrl="/" />
    <DxToolbarItem Text="Docs" NavigateUrl="https://docs.devexpress.com" />
</DxToolbar>
```

## Icons

Add icons via `IconCssClass`. DevExpress components work with any CSS icon library:

```razor
<!-- Open Iconic (included in Blazor templates) -->
<DxToolbarItem Text="New" IconCssClass="oi oi-file" />

<!-- Bootstrap Icons -->
<DxToolbarItem Text="Save" IconCssClass="bi bi-floppy" />

<!-- DevExpress icons -->
<DxToolbarItem IconCssClass="dx-icon-save" />
```

For icon-only items (no text), omit `Text` or set it empty.

## Checked Items (Toggle)

Use `@bind-Checked` and `GroupName` to create checkable items. Items with a unique `GroupName` act as toggles. Items sharing a `GroupName` act as a radio group — only one can be checked at a time:

```razor
<DxToolbar>
    <!-- Toggle item (on/off) -->
    <DxToolbarItem @bind-Checked="ShowPanel"
                   GroupName="ShowPanel"
                   Text="Show Panel"
                   IconCssClass="oi oi-list" />

    <!-- Radio group (only one checked at a time) -->
    <DxToolbarItem @bind-Checked="ViewList"
                   GroupName="ViewMode"
                   Text="List View"
                   BeginGroup="true" />
    <DxToolbarItem @bind-Checked="ViewGrid"
                   GroupName="ViewMode"
                   Text="Grid View" />
    <DxToolbarItem @bind-Checked="ViewCard"
                   GroupName="ViewMode"
                   Text="Card View" />
</DxToolbar>

@code {
    bool ShowPanel { get; set; } = true;
    bool ViewList  { get; set; } = true;
    bool ViewGrid  { get; set; } = false;
    bool ViewCard  { get; set; } = false;
}
```

> **Radio group rule**: Items with the same `GroupName` are mutually exclusive — selecting one unchecks the others. Items with a unique `GroupName` (or no `GroupName`) toggle independently.

## Tooltips

```razor
<DxToolbarItem IconCssClass="oi oi-cloud-upload"
               Tooltip="Save to cloud"
               Click="SaveToCloud" />
```

## Custom Item Content

Use `ChildContent` when you want to replace only the inner content area and keep the default item border, icon layout, and drop-down button:

```razor
<DxToolbarItem Text="Docs">
    <ChildContent>
        <span class="text-decoration-underline text-primary">Docs</span>
    </ChildContent>
    <Items>
        <DxToolbarItem Text="API Reference" />
        <DxToolbarItem Text="Examples" />
    </Items>
</DxToolbarItem>
```

Use `Template` only when you need to replace the entire item surface with fully custom markup:

```razor
<DxToolbarItem>
    <Template>
        <input type="text" placeholder="Search..." class="form-control form-control-sm"
               style="width: 200px;"
               @bind="searchQuery"
               @bind:event="oninput" />
    </Template>
</DxToolbarItem>
```

`Template` replaces the whole item content, so built-in visuals such as the default border and the drop-down button are not preserved.

## Form Submission

Use `Click` to trigger form submission from a toolbar button:

```razor
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />

    <DxToolbar>
        <DxToolbarItem Text="Submit" Click="SubmitForm" />
    </DxToolbar>

    <InputText @bind-Value="model.Name" />
</EditForm>

@code {
    MyModel model = new();

    void SubmitForm(ToolbarItemClickEventArgs _) {
        // Trigger validation and submission
        // Use a form reference or submit via JavaScript interop
    }
    void HandleSubmit() { }
}
```

> For programmatic form submission, keep a reference to the `EditForm` and call `EditContext.Validate()` manually.

## Item Visibility and Enabled State

```razor
<DxToolbarItem Text="Delete"
               Enabled="@canDelete"
               Visible="@isAdmin" />

@code {
    bool canDelete = false;
    bool isAdmin = true;
}
```

## Data-Bound Toolbar with Item Click

When using `Data`, handle item interactions via the toolbar-level `ItemClick` event:

```razor
<DxToolbar Data="@items" ItemClick="OnItemClick">
    <DataMappings>
        <DxToolbarDataMapping Text="Label" Key="Id" ParentKey="ParentId" />
    </DataMappings>
</DxToolbar>

@code {
    record MenuItem(int Id, int? ParentId, string Label);

    List<MenuItem> items = new() {
        new(1, null, "File"),
        new(2, 1,    "New"),
        new(3, 1,    "Save"),
    };

    void OnItemClick(ToolbarItemClickEventArgs e) {
        Console.WriteLine($"Clicked item key: {e.ItemName}");
    }
}
```

## Global Click Handler (Template-Based Items)

For template-based toolbars (`<Items>` mode), use `DxToolbar.ItemClick` as an alternative to wiring `Click` on each item. Assign a `Name` to each item for identification:

```razor
<DxToolbar ItemClick="OnItemClick">
    <Items>
        <DxToolbarItem Text="New"  Name="new" />
        <DxToolbarItem Text="Open" Name="open" />
        <DxToolbarItem Text="Save" Name="save" />
    </Items>
</DxToolbar>

@code {
    void OnItemClick(ToolbarItemClickEventArgs e) {
        switch (e.ItemName) {
            case "new":  OnNew();  break;
            case "open": OnOpen(); break;
            case "save": OnSave(); break;
        }
    }

    void OnNew()  { }
    void OnOpen() { }
    void OnSave() { }
}
```

## Split Drop-Down Button

Use `SplitDropDownButton="true"` on a parent item to split it into a main button and a separate drop-down arrow. Clicking the main area fires `Click`; clicking the arrow opens the sub-menu:

```razor
<DxToolbar>
    <Items>
        <DxToolbarItem Text="Paste" SplitDropDownButton="true" Click="OnPaste">
            <Items>
                <DxToolbarItem Text="Paste Special" />
                <DxToolbarItem Text="Paste as Plain Text" />
                <DxToolbarItem Text="Paste Formatting Only" />
            </Items>
        </DxToolbarItem>
    </Items>
</DxToolbar>

@code {
    void OnPaste(ToolbarItemClickEventArgs _) { /* main paste action */ }
}
```

> `SplitDropDownButton` requires the parent item to have child `<Items>`. Without children, the button renders normally.
