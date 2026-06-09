# Contextual and Advanced — DevExpress Blazor Ribbon

## When to Use This Reference

- Showing or hiding contextual tabs based on selected content
- Programmatically switching the active tab
- Implementing split drop-down buttons
- Binding ribbon item states to application data
- Handling Application tab click and sub-item navigation

## Contextual Tabs

Contextual tabs appear in the ribbon only when a specific type of content is selected (e.g., an image, a table cell, or a chart). They are hidden otherwise.

Use `Contextual="true"` on `DxRibbonTab` and bind `Visible` to a boolean property in your component state:

```razor
<DxRibbon>
    <DxRibbonTab Text="Home">
        <!-- Always-visible tab content -->
        <DxRibbonGroup>
            <DxRibbonItem Text="Select Image" Click="SelectImage" />
        </DxRibbonGroup>
    </DxRibbonTab>

    <!-- Appears only when an image is selected -->
    <DxRibbonTab Text="Picture Format" Contextual="true" Visible="@ImageSelected">
        <DxRibbonGroup>
            <DxRibbonItem Text="Crop" />
            <DxRibbonItem Text="Rotate" />
            <DxRibbonItem Text="Flip Horizontal" />
        </DxRibbonGroup>
        <DxRibbonGroup>
            <DxRibbonItem Text="Brightness" />
            <DxRibbonItem Text="Contrast" />
        </DxRibbonGroup>
    </DxRibbonTab>

    <!-- Appears only when a table cell is selected -->
    <DxRibbonTab Text="Table Tools" Contextual="true" Visible="@TableCellSelected">
        <DxRibbonGroup>
            <DxRibbonItem Text="Merge Cells" />
            <DxRibbonItem Text="Split Cell" />
            <DxRibbonItem Text="Delete Row" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

@code {
    bool ImageSelected     { get; set; } = false;
    bool TableCellSelected { get; set; } = false;

    void SelectImage() {
        ImageSelected     = true;
        TableCellSelected = false;
    }
}
```

> **Key rule**: Set `Contextual="true"` on the tab AND bind `Visible` to a boolean. Changing `Visible` at runtime shows or hides the tab automatically. The tab is not shown in the tab strip when `Visible="false"`.

## Multiple Contextual Tabs

You can define multiple contextual tab groups for different content types:

```razor
<DxRibbon>
    <DxRibbonTab Text="Home">...</DxRibbonTab>
    <DxRibbonTab Text="Insert">...</DxRibbonTab>

    <!-- Image tools -->
    <DxRibbonTab Text="Picture Format" Contextual="true" Visible="@IsImageSelected">...</DxRibbonTab>

    <!-- Chart tools (multiple contextual tabs at once) -->
    <DxRibbonTab Text="Chart Design"   Contextual="true" Visible="@IsChartSelected">...</DxRibbonTab>
    <DxRibbonTab Text="Chart Format"   Contextual="true" Visible="@IsChartSelected">...</DxRibbonTab>
</DxRibbon>
```

## Application Tab (File Menu)

`DxRibbonApplicationTab` renders a special button at the left of the tab strip. It can open a dropdown menu of `DxRibbonApplicationTabItem` components. Items support nesting for sub-menus:

```razor
<DxRibbon>
    <DxRibbonApplicationTab Text="File" Click="@(() => FileMenuOpen = !FileMenuOpen)">
        <DxRibbonApplicationTabItem Text="New" />
        <DxRibbonApplicationTabItem Text="Open" />
        <DxRibbonApplicationTabItem Text="Save" />
        <DxRibbonApplicationTabItem Text="Save As">
            <DxRibbonApplicationTabItem Text="Word Document (.docx)" />
            <DxRibbonApplicationTabItem Text="PDF (.pdf)" />
            <DxRibbonApplicationTabItem Text="Plain Text (.txt)" />
        </DxRibbonApplicationTabItem>
        <DxRibbonApplicationTabItem Text="Print" />
    </DxRibbonApplicationTab>
    <DxRibbonTab Text="Home">...</DxRibbonTab>
</DxRibbon>

@code {
    bool FileMenuOpen { get; set; } = false;
}
```

> `DxRibbonApplicationTab.Click` fires when the button is clicked. Handle additional logic (opening a side panel, dialog, or overlay) from this event.

## Programmatically Switching Tabs

Use `ActiveTabIndex` to select a tab by its zero-based index:

```razor
<DxRibbon @bind-ActiveTabIndex="activeTab">
    <DxRibbonTab Text="Home">...</DxRibbonTab>
    <DxRibbonTab Text="Insert">...</DxRibbonTab>
    <DxRibbonTab Text="View">...</DxRibbonTab>
</DxRibbon>

<button @onclick="() => activeTab = 1">Go to Insert Tab</button>

@code {
    int activeTab = 0;
}
```

## Split Drop-Down Button

Use `SplitDropDownButton="true"` on `DxRibbonItem` to render a button with a separate drop-down portion:

```razor
<DxRibbonGroup>
    <DxRibbonItem Text="Paste" SplitDropDownButton="true" Click="OnPaste">
        <Items>
            <DxRibbonItem Text="Paste Special" Click="OnPasteSpecial" />
            <DxRibbonItem Text="Paste as Plain Text" Click="OnPastePlain" />
        </Items>
    </DxRibbonItem>
</DxRibbonGroup>

@code {
    void OnPaste()        { }
    void OnPasteSpecial() { }
    void OnPastePlain()   { }
}
```

Clicking the main button area fires `Click`. Clicking the drop-down arrow opens the sub-menu.

## Binding Item State to Application Data

Connect ribbon item states to your component's fields or services for a live-synchronized UI:

```razor
<DxRibbon>
    <DxRibbonTab Text="Home">
        <DxRibbonGroup>
            <DxRibbonToggleItem Text="Bold"   @bind-Checked="editor.IsBold" />
            <DxRibbonToggleItem Text="Italic" @bind-Checked="editor.IsItalic" />
        </DxRibbonGroup>
        <DxRibbonGroup>
            <DxRibbonComboBoxItem Data="@fontSizes"
                                  @bind-Value="editor.FontSize"
                                  NullText="Size"
                                  Width="80px" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

@code {
    EditorState editor = new();

    List<int> fontSizes = new() { 8, 10, 12, 14, 16, 18, 24, 36 };

    class EditorState {
        public bool IsBold   { get; set; }
        public bool IsItalic { get; set; }
        public int? FontSize { get; set; } = 12;
    }
}
```

## Global Item Click Handler

Use `DxRibbon.ItemClick` to handle all ribbon button clicks in a single event — the recommended pattern when you have many items and want to avoid wiring `Click` to each one:

```razor
<DxRibbon ItemClick="OnItemClick">
    <DxRibbonTab Text="Home">
        <DxRibbonGroup Text="Clipboard">
            <DxRibbonItem Text="Cut" />
            <DxRibbonItem Text="Copy" />
            <DxRibbonItem Text="Paste" />
        </DxRibbonGroup>
        <DxRibbonGroup Text="Font">
            <DxRibbonItem Text="Bold" />
            <DxRibbonItem Text="Italic" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

@code {
    void OnItemClick(RibbonItemClickEventArgs args) {
        switch (args.Item.Text) {
            case "Cut":   OnCut();   break;
            case "Copy":  OnCopy();  break;
            case "Paste": OnPaste(); break;
            case "Bold":  OnBold();  break;
            case "Italic": OnItalic(); break;
        }
    }

    void OnCut()   { }
    void OnCopy()  { }
    void OnPaste() { }
    void OnBold()  { }
    void OnItalic(){ }
}
```

> **Note**: `DxRibbon.ItemClick` fires for `DxRibbonItem` buttons only. Application tab items, color palettes, and data editors (`DxRibbonComboBoxItem`, `DxRibbonSpinEditItem`) fire their own events.

## Hide Tab Captions (ShowTabs)

Set `ShowTabs="false"` to hide all tab captions while keeping all groups and items visible. This creates a compact toolbar-like layout using ribbon content:

```razor
<DxRibbon ShowTabs="false" @bind-ActiveTabIndex="activeTab">
    <DxRibbonTab Text="Home">
        <DxRibbonGroup Text="Font">
            <DxRibbonItem Text="Bold" />
            <DxRibbonItem Text="Italic" />
        </DxRibbonGroup>
    </DxRibbonTab>
    <DxRibbonTab Text="Insert">
        <DxRibbonGroup Text="Media">
            <DxRibbonItem Text="Image" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

<!-- Switch tabs programmatically even with captions hidden -->
<button @onclick="() => activeTab = 1">Go to Insert</button>
@code { int activeTab = 0; }
```

## Adaptive Group Collapse Mode

`AdaptivityAutoCollapseItemsToGroups` (default: `true`) controls how the ribbon collapses on narrow screens:

| Value | Behavior |
|---|---|
| `true` (default) | Each group collapses into its own labeled drop-down button |
| `false` | All overflowing content collapses into a single shared overflow menu |

```razor
<!-- Each group gets its own overflow button (default) -->
<DxRibbon AdaptivityAutoCollapseItemsToGroups="true">
    <DxRibbonTab Text="Home">
        <DxRibbonGroup Text="Clipboard" IconCssClass="dx-icon-copy">...</DxRibbonGroup>
        <DxRibbonGroup Text="Font"      IconCssClass="dx-icon-bold">...</DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

<!-- Single shared overflow menu -->
<DxRibbon AdaptivityAutoCollapseItemsToGroups="false">
    <DxRibbonTab Text="Home">
        <DxRibbonGroup Text="Clipboard">...</DxRibbonGroup>
        <DxRibbonGroup Text="Font">...</DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>
```

> **Requires**: Set `DxRibbonGroup.Text` and/or `DxRibbonGroup.IconCssClass` to label collapsed group buttons when `AdaptivityAutoCollapseItemsToGroups="true"`.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Contextual tab always visible | `Visible` not bound or always `true` | Bind `Visible` to a boolean that reflects selection state |
| Contextual tab never appears | `Contextual="true"` missing | Add `Contextual="true"` to the tab |
| Split button opens no drop-down | `Items` child not defined | Add `<Items>` with `DxRibbonItem` children |
| Active tab does not change | `@bind-ActiveTabIndex` not used | Use `@bind-ActiveTabIndex` for two-way binding |
| Application tab sub-items not showing | Missing `DxRibbonApplicationTabItem` | Add child `DxRibbonApplicationTabItem` inside `DxRibbonApplicationTab` |
