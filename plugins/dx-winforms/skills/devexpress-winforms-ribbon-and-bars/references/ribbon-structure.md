# Ribbon Structure

This reference is the structural map of `RibbonControl` — every nesting layer (categories → pages → groups → item links), every auxiliary area (QAT, page header, caption bar, status bar, application menu / Backstage View, contextual tabs), and one end-to-end code sample that wires all of them together. Use it as the index when assembling a complete Office-style ribbon UI.

For items themselves, see [items-and-settings.md](items-and-settings.md). For host-form setup and skin, see [getting-started.md](getting-started.md). For the merging model, see [merging.md](merging.md).

## When to Use This Reference

- Assembling a full ribbon with multiple pages, groups, and contextual tabs.
- Adding the Quick Access Toolbar (QAT) and items to the caption bar.
- Configuring the application menu — picking `ApplicationMenu` (Office 2007) vs `BackstageViewControl` (Office 2010+).
- Adding a `RibbonStatusBar`.
- Showing a contextual page on demand and activating it programmatically.

## Anatomy at a Glance

```text
RibbonForm
├── RibbonControl
│   ├── ApplicationButton                          → ApplicationButtonDropDownControl
│   │                                               (ApplicationMenu | BackstageViewControl)
│   ├── CaptionBarItemLinks         (form title bar, right of caption)
│   ├── Toolbar  (Quick Access Toolbar = QAT)
│   ├── PageHeaderItemLinks         (tab strip area)
│   ├── PageCategories  ─────────────────────► RibbonPageCategory  (regular or contextual)
│   │                                               └── Pages  ──► RibbonPage  (tab)
│   │                                                                  └── Groups  ──► RibbonPageGroup
│   │                                                                                       └── ItemLinks  ──► BarItemLink
│   ├── Pages       (top-level pages — outside any category)
│   ├── Items       (every BarItem — items live here once)
│   ├── RepositoryItems   (editor templates for BarEditItem)
│   ├── Images / LargeImages / SvgImages          (icon collections)
│   └── StatusBar   ← optional RibbonStatusBar reference
└── RibbonStatusBar
    └── ItemLinks
```

A `RibbonPage` belongs either to `RibbonControl.Pages` (top-level) or to `RibbonPageCategory.Pages` (inside a category). Groups belong to pages. Items live once in `RibbonControl.Items`, and any number of links point at them.

## Categories

`RibbonPageCategory` groups one or more `RibbonPage`s under a colored highlight in the tab strip. Two flavors:

- **Regular** — always visible.
- **Contextual** — hidden by default; you set `Visible = true` when relevant (e.g., when the user selects an image / a table cell).

```csharp
var picCat = new RibbonPageCategory("Picture Tools", Color.OrangeRed, true);
ribbon.PageCategories.Add(picCat);
picCat.Pages.Add(new RibbonPage("Format"));
picCat.Visible = true;          // show when an image is selected
ribbon.SelectedPage = picCat.Pages[0];
```

The `RibbonPageCategory(string text, Color color, bool visible)` constructor's third parameter is the category's initial **`Visible`** state — pass `false` to create the category hidden and reveal it later. The `Color` argument tints the contextual category (the exact appearance is skin-dependent).

## Pages and Groups

```csharp
var pageHome = new RibbonPage("Home");
ribbon.Pages.Add(pageHome);

var groupFile = new RibbonPageGroup("File") {
    AllowTextClipping = false,
    ShowCaptionButton = true,           // show the "expander" caption button
};
pageHome.Groups.Add(groupFile);

// Add bar items as links
groupFile.ItemLinks.Add(itemNew);
groupFile.ItemLinks.Add(itemOpen);
```

Group features:

- `ShowCaptionButton = true` displays a small button in the lower-right corner; subscribe to `RibbonPageGroup.CaptionButtonClick` (or the ribbon's `PageGroupCaptionButtonClick`) for handling.
- `AllowTextClipping` controls whether captions can be clipped when space is tight.
- `State` (`Auto`, `Expanded`, `Collapsed`) — collapse a group into a single dropdown button if needed.

## Items inside Groups

A `RibbonPageGroup` holds **links**, not items. Items live in `ribbon.Items` (one copy each). Use `group.ItemLinks.Add(item)` to add a link.

```csharp
var itemNew  = ribbon.Items.CreateButton("New");
itemNew.Id   = ribbon.Manager.GetNewItemId();
itemNew.RibbonStyle = RibbonItemStyles.Large;
itemNew.ImageOptions.SvgImage = svgImageCollection1["new"];
itemNew.ItemClick += (_, _) => CreateNew();

groupFile.ItemLinks.Add(itemNew);
```

For ordering, add in order or use `ItemLinks.Insert(index, item)`.

## Quick Access Toolbar (QAT)

`RibbonControl.Toolbar` is the QAT — the strip of small buttons next to the application button (or in the title bar when hosted in `RibbonForm`).

```csharp
ribbon.Toolbar.ItemLinks.Add(itemNew);
ribbon.Toolbar.ItemLinks.Add(itemSave);
ribbon.Toolbar.ItemLinks.Add(itemUndo);

// Place QAT above or below the ribbon
ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Below;
```

End users can right-click any ribbon item → *Add to Quick Access Toolbar*.

The QAT is **unavailable** when `RibbonStyle = OfficeUniversal`.

## Page Header Items

Items shown in the same row as the tabs — typically to the right of all tabs.

```csharp
ribbon.PageHeaderItemLinks.Add(itemHelp);
ribbon.PageHeaderItemLinks.Add(itemFeedback);
```

## Caption Bar Items

Items shown in the **form title bar**, on the right side of the title text. Requires `RibbonForm`.

```csharp
ribbon.CaptionBarItemLinks.Add(new SkinPaletteDropDownButtonItem());
ribbon.CaptionBarItemLinks.Add(new SkinDropDownButtonItem());
```

## Ribbon Status Bar

A `RibbonStatusBar` is the bottom strip — a "bar" in the same family as classic `Bar`, but tied to the ribbon for skin and merge consistency.

```csharp
var status = new RibbonStatusBar(ribbon) { Parent = this };
status.ItemLinks.Add(new BarStaticItem { Caption = "Ready" });
ribbon.StatusBar = status;        // optional; sets the convenience pointer

var zoomItem = new BarEditItem(ribbon.Manager, new RepositoryItemZoomTrackBar { Minimum = 0, Maximum = 200 }) {
    EditWidth = 150, EditValue = 100
};
ribbon.Items.Add(zoomItem);
status.ItemLinks.Add(zoomItem);
```

Status-bar links can be left- or right-aligned via `BarItemLink.UserAlignment`.

## Application Menu — Two Choices

The "File" button (or the round Office Pearl in the 2007 style) opens whatever is assigned to `RibbonControl.ApplicationButtonDropDownControl`. Two component choices:

### Option A — `ApplicationMenu` (Office 2007 look)

```csharp
var appMenu = new ApplicationMenu();
appMenu.ItemLinks.Add(itemNew);
appMenu.ItemLinks.Add(itemOpen);
appMenu.ItemLinks.Add(itemSave);
appMenu.ItemLinks.Add(itemExit);

// Bottom-edge buttons live in a PopupControlContainer assigned to BottomPaneControlContainer
var bottomPane = new PopupControlContainer();
// add your custom buttons/controls to bottomPane.Controls …
appMenu.BottomPaneControlContainer = bottomPane;

ribbon.ApplicationButtonDropDownControl = appMenu;
ribbon.ShowApplicationButton            = DefaultBoolean.True;
```

A simple panel-style menu sized to its contents. Suits small command sets.

### Option B — `BackstageViewControl` (Office 2010+ full-screen menu)

```csharp
var backstage = new BackstageViewControl { Ribbon = ribbon };

// Left rail items — tabs and buttons
var tabInfo = new BackstageViewTabItem  { Caption = "Info" };
var tabNew  = new BackstageViewTabItem  { Caption = "New" };
var tabOpen = new BackstageViewTabItem  { Caption = "Open" };
var btnExit = new BackstageViewButtonItem { Caption = "Exit" };
btnExit.ItemClick += (_, _) => Close();

// BackstageViewControlItemCollection has no AddRange — add items one at a time
backstage.Items.Add(tabInfo);
backstage.Items.Add(tabNew);
backstage.Items.Add(tabOpen);
backstage.Items.Add(new BackstageViewItemSeparator());
backstage.Items.Add(btnExit);

// Right pane content per tab — a BackstageViewClientControl you author
tabInfo.ContentControl = new InfoPanel();          // InfoPanel : BackstageViewClientControl
tabNew.ContentControl  = new NewDocumentPanel();   // NewDocumentPanel : BackstageViewClientControl
tabOpen.ContentControl = new OpenPanel();          // OpenPanel : BackstageViewClientControl

ribbon.ApplicationButtonDropDownControl = backstage;
```

`BackstageViewTabItem.ContentControl` is a `BackstageViewClientControl` — your content panel must **derive from `BackstageViewClientControl`**, not a plain `Control`/`UserControl`. For recent-files lists, use `RecentItemControl` as a child of that content panel.

### Pick based on `RibbonStyle`

- `Office2007` → `ApplicationMenu`.
- `Office2010` / `Office2013` / `Office2019` / `OfficeUniversal` → `BackstageViewControl`.
- `TabletOffice` → either works; Backstage is recommended.

## Contextual Tabs (Selection-Driven)

Contextual tabs are pages inside a `RibbonPageCategory.Visible = false` category. Show/hide on selection events from your editor / document area.

```csharp
// Setup once
var pictureCategory = new RibbonPageCategory("Picture Tools", Color.OrangeRed);
pictureCategory.Visible = false;
ribbon.PageCategories.Add(pictureCategory);
var pictureFormat = new RibbonPage("Format");
pictureCategory.Pages.Add(pictureFormat);

// When the user selects an image
pictureCategory.Visible = true;
ribbon.SelectedPage     = pictureFormat;

// When the selection clears
pictureCategory.Visible = false;
```

## Integrated Search Menu

`RibbonControl.SearchItemShortcut` enables the search box. Users can find a command by caption or by `BarItem.SearchTags`.

```csharp
ribbon.SearchItemShortcut = new BarShortcut(Keys.Alt | Keys.Q);
itemFormatCells.SearchTags = "format,cell,number,currency";
```

## End-to-End Sample — Full Ribbon UI

```csharp
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;

public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();
        InitRibbon();
    }

    void InitRibbon() {
        var ribbon = new RibbonControl {
            Images       = svgImageCollection1,
            RibbonStyle  = RibbonControlStyle.Office2019,
            ToolbarLocation = RibbonQuickAccessToolbarLocation.Above
        };
        Controls.Add(ribbon);
        ribbon.BeginInit();

        // ── Items ──────────────────────────────────────────────────────
        var iNew    = MakeBtn(ribbon, "New",    "new");
        var iOpen   = MakeBtn(ribbon, "Open",   "open");
        var iSave   = MakeBtn(ribbon, "Save",   "save");
        var iCut    = MakeBtn(ribbon, "Cut",    "cut");
        var iCopy   = MakeBtn(ribbon, "Copy",   "copy");
        var iPaste  = MakeBtn(ribbon, "Paste",  "paste");
        var iUndo   = MakeBtn(ribbon, "Undo",   "undo");
        var iHelp   = MakeBtn(ribbon, "Help",   "help");

        // ── Pages and groups ──────────────────────────────────────────
        var pageHome = new RibbonPage("Home");
        ribbon.Pages.Add(pageHome);

        var gFile = new RibbonPageGroup("File");
        pageHome.Groups.Add(gFile);
        gFile.ItemLinks.AddRange(new BarItem[] { iNew, iOpen, iSave });

        var gEdit = new RibbonPageGroup("Edit");
        pageHome.Groups.Add(gEdit);
        gEdit.ItemLinks.AddRange(new BarItem[] { iCut, iCopy, iPaste });

        // ── Contextual category ───────────────────────────────────────
        var catPic = new RibbonPageCategory("Picture Tools", Color.OrangeRed);
        ribbon.PageCategories.Add(catPic);
        var pageFormat = new RibbonPage("Format");
        catPic.Pages.Add(pageFormat);
        var gColor = new RibbonPageGroup("Color");
        pageFormat.Groups.Add(gColor);
        gColor.ItemLinks.Add(iPaste);                  // any items
        catPic.Visible = true;
        ribbon.SelectedPage = pageFormat;

        // ── Quick Access Toolbar ──────────────────────────────────────
        ribbon.Toolbar.ItemLinks.Add(iSave);
        ribbon.Toolbar.ItemLinks.Add(iUndo);

        // ── Page header ───────────────────────────────────────────────
        ribbon.PageHeaderItemLinks.Add(iHelp);

        // ── Caption bar (form title) ──────────────────────────────────
        ribbon.CaptionBarItemLinks.Add(new SkinPaletteDropDownButtonItem());
        ribbon.CaptionBarItemLinks.Add(new SkinDropDownButtonItem());

        // ── Application captions ──────────────────────────────────────
        ribbon.ApplicationCaption         = "My Application";
        ribbon.ApplicationDocumentCaption = "Untitled";

        // ── Application menu / Backstage ──────────────────────────────
        var backstage = new BackstageViewControl { Ribbon = ribbon };
        backstage.Items.Add(new BackstageViewTabItem  { Caption = "Info" });
        backstage.Items.Add(new BackstageViewTabItem  { Caption = "New"  });
        backstage.Items.Add(new BackstageViewTabItem  { Caption = "Open" });
        backstage.Items.Add(new BackstageViewItemSeparator());
        backstage.Items.Add(new BackstageViewButtonItem { Caption = "Exit" });
        ribbon.ApplicationButtonDropDownControl = backstage;

        // ── Status bar ────────────────────────────────────────────────
        var status = new RibbonStatusBar(ribbon) { Parent = this };
        var ready  = new BarStaticItem { Caption = "Ready" };
        ribbon.Items.Add(ready);
        status.ItemLinks.Add(ready);

        var zoom = new BarEditItem(ribbon.Manager,
                       new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar { Minimum = 0, Maximum = 200 }) {
            EditWidth = 150, EditValue = 100
        };
        ribbon.Items.Add(zoom);
        var zoomLink = status.ItemLinks.Add(zoom);
        zoomLink.UserAlignment = BarItemLinkAlignment.Right;

        // ── Search ────────────────────────────────────────────────────
        ribbon.SearchItemShortcut = new BarShortcut(Keys.Alt | Keys.Q);
        iSave.SearchTags = "save,write,persist";

        ribbon.EndInit();
    }

    static BarButtonItem MakeBtn(RibbonControl r, string text, string svgKey) {
        var b = r.Items.CreateButton(text);
        b.Id = r.Manager.GetNewItemId();
        b.ImageOptions.SvgImage = ((DevExpress.Utils.SvgImageCollection)r.Images)[svgKey];
        b.RibbonStyle = RibbonItemStyles.Default;
        return b;
    }
}
```

What the sample wires up:

- One `RibbonControl` with Office 2019 style.
- Page `Home` with two groups (`File`, `Edit`).
- Contextual category `Picture Tools` with a `Format` page (visible, selected on startup for demo).
- QAT items, page header items, caption bar items.
- A `BackstageViewControl` opens on the application button.
- A status bar with a left-aligned "Ready" label and a right-aligned zoom track bar.
- Alt+Q opens the ribbon's search field.

## Common Issues

- **Application Button doesn't show** — `ShowApplicationButton` is `DefaultBoolean.False`. Set it to `True`, or use a `RibbonStyle` that always shows the button (Office 2007/2010/2013/Tablet).
- **Backstage tab clicked but content area empty** — `BackstageViewTabItem.ContentControl` not assigned.
- **QAT items disappear after restart** — `BarItem.Id` not set; layout deserialization fails.
- **Caption Bar items invisible** — host form is not `RibbonForm`; or `RibbonStyle = OfficeUniversal` (which does not draw the title bar).
- **Contextual tab does not show** — `RibbonPageCategory.Visible = false`. Set it to `true` when activating.
- **Group expander button missing** — `RibbonPageGroup.ShowCaptionButton = false`. Enable it and handle `CaptionButtonClick`.
- **Application menu won't close** — adding controls to `BackstageViewClientControl` without proper disposal traps focus. Use `BackstageViewTabItem.ContentControl` properties.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/index.md` — Ribbon overview (`xref:WindowsForms.2500`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/index.md` — Visual Elements (`xref:WindowsForms.2524`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/ribbon-page.md` (`xref:WindowsForms.2494`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/ribbon-page-group.md` (`xref:WindowsForms.2493`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/quick-access-toolbar.md` (`xref:WindowsForms.2496`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/page-header-items.md` (`xref:WindowsForms.3452`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/caption-bar-items.md` (`xref:WindowsForms.402552`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/categories-and-contextual-tabs.md` (`xref:WindowsForms.3327`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/the-ribbon-ui/ribbon-status-bar.md` (`xref:WindowsForms.2498`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/main-menus/application-menu.md` (`xref:WindowsForms.116775`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/main-menus/backstageview-control.md` (`xref:WindowsForms.11017`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/main-menus/recent-item-control.md` (`xref:WindowsForms.114825`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/search-menu.md` (`xref:WindowsForms.400621`).
- `api/DevExpress.XtraBars.Ribbon.RibbonControl.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonPageCategory.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonPage.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonPageGroup.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonStatusBar.yml`.
- `api/DevExpress.XtraBars.Ribbon.ApplicationMenu.yml`.
- `api/DevExpress.XtraBars.Ribbon.BackstageViewControl.yml`.
