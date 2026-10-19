# Merging (MDI)

This reference covers ribbon and bars merging in MDI applications — what merges automatically, what must be merged manually, the merge-style options, and the merge customization properties (`MergeType`, `MergeOrder`).

In an MDI app, when a child form is activated (or maximized, depending on settings), the child's ribbon/bars hide and their items move into the parent's ribbon/bars. When the child becomes inactive, items unmerge and the child's UI reappears. This skinned MDI experience is the WinForms way to keep the active document's commands visible at the top of the application.

## When to Use This Reference

- Building an MDI app where each child form contributes commands to the parent ribbon.
- Configuring when merging triggers (only when maximized, always when active, never).
- Manually merging status bars (which never auto-merge) and arbitrary toolbars.
- Controlling how items combine (replace vs append vs merge submenus) with `MergeType`.
- Using `DocumentManager`/`XtraTabbedMdiManager` for tabbed MDI plus ribbon merging.

## Auto-Merge Matrix

| What | Ribbon | Bars |
|---|---|---|
| Ribbon pages | **auto** by caption | n/a |
| Ribbon page groups | **auto** by caption (inside merged pages) | n/a |
| Page-header items (`PageHeaderItemLinks`) | **auto** | n/a |
| Quick Access Toolbar (`Toolbar`) | **auto** | n/a |
| Caption Bar Items (`CaptionBarItemLinks`) | **auto** | n/a |
| Application Menu / Backstage View | **not merged** (child menus are hidden) | n/a |
| Status Bar (`RibbonStatusBar`) | **manual** — handle `Merge`/`UnMerge` | **manual** |
| Main Menu (`BarManager.MainMenu`) | n/a | **auto** |
| Toolbars (other `Bar`s) | n/a | **manual** — handle `BarManager.Merge`/`UnMerge` |

## Ribbon Merging

### Pick a merge style

```csharp
parentRibbon.MdiMergeStyle = RibbonMdiMergeStyle.OnlyWhenMaximized;
```

| Value | Behavior |
|---|---|
| `Default` | Same as `OnlyWhenMaximized`. |
| `OnlyWhenMaximized` | Ribbons merge when a child MDI form is maximized; unmerge when restored. |
| `Always` | Merge as soon as a child form becomes active. |
| `Never` | Disable automatic merging. Call `MergeRibbon` / `UnMergeRibbon` manually. |

### What auto-merge does (in order)

1. Copy child `Pages` / `PageGroups` to the parent. Pages with matching captions are merged; missing pages are appended.
2. Copy `PageHeaderItemLinks`.
3. Copy `Toolbar` (QAT) `ItemLinks`.
4. Hide the child ribbon.
5. Raise `RibbonControl.Merge` on the parent. Handle this to do anything else (merge status bars, set custom captions, etc.).

Unmerge is the reverse plus `RibbonControl.UnMerge`.

### Manually merge status bars on `Merge`

Status bars never auto-merge — child status bar items are not moved. Wire them up yourself:

```csharp
parentRibbon.Merge += (s, e) => {
    var parent = (RibbonControl)s;
    var child  = e.MergedChild;
    parent.StatusBar.MergeStatusBar(child.StatusBar);
};

parentRibbon.UnMerge += (s, e) => {
    var parent = (RibbonControl)s;
    parent.StatusBar.UnMergeStatusBar();
};
```

### Manual merge (when `MdiMergeStyle = Never`)

```csharp
// On child activation
parentRibbon.MergeRibbon(childRibbon);

// On child deactivation
parentRibbon.UnMergeRibbon();
```

`MergeRibbon` has an overload that takes a `RibbonStatusBar` if you want to merge status bars at the same call site.

### Customize how individual items merge — `MergeType` and `MergeOrder`

`BarItem.MergeType` controls what happens when a child item meets a parent item with the same caption (case-sensitive, including the hidden `&` accelerator marker).

`BarItem.MergeType` is of type `BarMenuMerge`, whose members are `Add`, `MergeItems`, `Remove`, and `Replace` (there is no `Default` value):

| `MergeType` | Behavior |
|---|---|
| `Add` | Append the child item after the parent items (the default behavior). |
| `Remove` | Hide the parent item when the child is active. |
| `Replace` | Replace the parent item with the child item. |
| `MergeItems` | For `BarSubItem`s: merge the child's submenu into the parent's submenu (combines two *Help* menus into one with all entries). |

```csharp
// Combine the two "Help" submenus
parentHelpMenu.MergeType = BarMenuMerge.MergeItems;
childHelpMenu.MergeType  = BarMenuMerge.MergeItems;

// Replace parent "Print" with child's specialized version
childPrintItem.MergeType = BarMenuMerge.Replace;
```

`BarItem.MergeOrder` (int, default 0) reorders merged items. Items are sorted by `MergeOrder` ascending; ties keep insertion order.

```csharp
childZoomOut.MergeOrder = 10;
childZoomIn.MergeOrder  = 20;
```

## Bars Merging (`BarManager`)

### Auto-merge

Only the **main menus** auto-merge (`BarManager.MainMenu` of parent and child). Other toolbars and the status bar do not auto-merge.

### Merge style for bars

```csharp
parentBars.MdiMenuMergeStyle = BarMdiMenuMergeStyle.Always;
// or .Never / .OnlyWhenChildMaximized / .WhenChildActivated
```

> `MdiMenuMergeStyle` is of type `BarMdiMenuMergeStyle` and controls *when* the main menus merge: `Always`, `Never`, `OnlyWhenChildMaximized`, or `WhenChildActivated`. (It is not the `BarMenuMerge` enum — that one configures per-item `MergeType`.) Most apps leave it at `Always`.

### Manual merge for toolbars and status bars

```csharp
parentBars.Merge += (s, e) => {
    var parent = (BarManager)s;
    var child  = e.ChildManager;

    var parentEditBar = parent.Bars["Edit"];
    var childEditBar  = child.Bars["Edit"];
    if (parentEditBar != null && childEditBar != null)
        parentEditBar.Merge(childEditBar);

    parent.StatusBar.Merge(child.StatusBar);
};

parentBars.UnMerge += (s, e) => {
    var parent = (BarManager)s;
    parent.Bars["Edit"].UnMerge();
    parent.StatusBar.UnMerge();
};
```

Inside `Merge` you can also add *dynamic* toolbars to the parent that exist only while the child is active, then dispose them in `UnMerge`.

## Wiring an MDI Scenario

```csharp
// Parent form
public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();
        IsMdiContainer = true;
        ribbonControl.MdiMergeStyle = RibbonMdiMergeStyle.OnlyWhenMaximized;

        ribbonControl.Merge += (s, e) => {
            ribbonControl.StatusBar.MergeStatusBar(e.MergedChild.StatusBar);
        };
        ribbonControl.UnMerge += (_, _) => {
            ribbonControl.StatusBar.UnMergeStatusBar();
        };
    }

    void OpenDocument() {
        var child = new DocumentForm { MdiParent = this };
        child.Show();
    }
}

// Child form — also a RibbonForm
public partial class DocumentForm : RibbonForm {
    public DocumentForm() {
        InitializeComponent();
        // Add child ribbon pages with the SAME captions as the parent
        // (e.g., "Home") to merge into existing parent pages.
        // Different captions become new tabs in the merged parent ribbon.
    }
}
```

The child `RibbonControl` is **required** even when the child contributes only a few extra commands — without it, automatic merge has nothing to copy.

## Page Merge Semantics

- **Same caption** ⇒ pages merge: child groups are appended to the parent page.
- **Different caption** ⇒ child page is added as a new tab in the parent ribbon.
- **Empty parent page exists** ⇒ child fills it.
- **Contextual page** in the child ⇒ merges into the parent as another contextual page if the parent has the same category.

## DocumentManager Merging Specifics

The `DocumentManager` (App UI Manager) component supports merging via `RibbonAndBarsMergeStyle`:

| Value | Behavior |
|---|---|
| `Default` | Defer to `BarManager.MdiMenuMergeStyle` / `RibbonControl.MdiMergeStyle`. |
| `AlwaysMergeRibbonsAndBars` | Merge whenever a document is active. |
| `WhenChildFormMaximized` | Only on maximization. |
| `NeverMergeRibbonsAndBars` | Disable. |

For Tabbed / NativeMDI / Widget views, the active document's ribbon/bars merge as if its host form were the active MDI child.

```csharp
documentManager1.RibbonAndBarsMergeStyle = RibbonAndBarsMergeStyle.AlwaysMergeRibbonsAndBars;
```

## Common Patterns

### Pattern 1 — Suppress an item from merging

```csharp
childOnlyItem.MergeType = BarMenuMerge.Remove;   // hidden in parent when merged
```

### Pattern 2 — Replace a parent button with the child's specialized variant

```csharp
parentPrintItem.Caption = "Print";
childPrintItem.Caption  = "Print";              // captions must match
childPrintItem.MergeType = BarMenuMerge.Replace;
```

### Pattern 3 — Combine two submenus

```csharp
parentToolsMenu.Caption = childToolsMenu.Caption = "&Tools";
parentToolsMenu.MergeType = childToolsMenu.MergeType = BarMenuMerge.MergeItems;
```

### Pattern 4 — Add a temporary toolbar only while a specific child is active

```csharp
parentBars.Merge += (s, e) => {
    var parent = (BarManager)s;
    var temp = new Bar(parent, "temp-edit") { DockStyle = BarDockStyle.Top, DockRow = 1 };
    temp.AddItem(e.ChildManager.Items["FindItem"]);
};

parentBars.UnMerge += (s, e) => {
    var parent = (BarManager)s;
    var temp = parent.Bars["temp-edit"];
    if (temp != null) {
        temp.UnMerge();
        temp.Dispose();
    }
};
```

## Common Issues

- **Two child "Help" menus after merge** — `MergeType` defaults to `Add`. Set `MergeType = MergeItems` on *both* sides with matching captions.
- **Status bar empty in merged state** — status bars never auto-merge. Handle `RibbonControl.Merge` and call `parent.StatusBar.MergeStatusBar(child.StatusBar)`.
- **Toolbar items lost after merge** — only `MainMenu` auto-merges in bars. Subscribe to `BarManager.Merge` and call `parentBar.Merge(childBar)`.
- **Child caption changes break merging** — pages merge by *caption*. If the child's caption is localized or changes at runtime, the parent merges into a different page.
- **`MergeRibbon` throws "already merged"** — call `UnMergeRibbon` first, or check `MergedChild` before re-merging.
- **`UnMerge` event not fired** — `MdiMergeStyle = Never` doesn't dispatch the event. Trigger your own cleanup before `UnMergeRibbon()`.
