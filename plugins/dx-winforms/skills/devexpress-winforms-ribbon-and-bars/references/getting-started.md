# Getting Started

This reference covers what is required to host a `RibbonControl` or `BarManager` in a WinForms app: the NuGet package, the host-form rules (`RibbonForm` for ribbons), skin application, and a minimal first-ribbon example you can paste into an empty project.

## When to Use This Reference

- A new WinForms project needs a ribbon or bars for the first time.
- The ribbon appears in the form's client area instead of integrating with the title bar.
- The ribbon renders unskinned or with the OS-default chrome.
- You need a one-page paste-ready first example.

## NuGet Package

`DevExpress.Win.Navigation` ships everything in `DevExpress.XtraBars` and `DevExpress.XtraBars.Ribbon`:

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
</ItemGroup>
```

Or via CLI:

```
dotnet add package DevExpress.Win.Navigation
```

If you want one umbrella that covers most WinForms controls, use `DevExpress.Win` instead. Keep DevExpress NuGet versions aligned across the solution.

## Common Imports

```csharp
using DevExpress.XtraBars;            // BarManager, Bar, BarItem, BarButtonItem, BarCheckItem, BarSubItem, BarEditItem, ItemClickEventArgs
using DevExpress.XtraBars.Ribbon;     // RibbonControl, RibbonForm, RibbonPage, RibbonPageGroup, RibbonPageCategory, RibbonStatusBar, ApplicationMenu, BackstageViewControl, RibbonItemStyles
using DevExpress.Utils;               // SvgImageCollection
using DevExpress.Utils.Svg;           // SvgImage
```

## Host Form Requirement (Ribbon UI)

The host form for a `RibbonControl` must inherit from `DevExpress.XtraBars.Ribbon.RibbonForm`. This is the **WinForms equivalent of WPF's `ThemedWindow`** — without it, the ribbon cannot integrate the Quick Access Toolbar and Caption Bar Items into the form's title bar, the Application Button cannot overlap the title bar in Office-2007 style, and skins won't paint the form non-client area properly.

```csharp
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();
    }
}
```

`RibbonForm` derives from `XtraForm` and additionally implements `ISupportGlassRegions`, `IBarObjectContainer`, and `ISupportFormShadow`. To convert an existing form, change its base class to `DevExpress.XtraBars.Ribbon.RibbonForm` in both the `.cs` and `.Designer.cs` partial declarations.

For the **classic Bars UI** (no ribbon), `XtraForm` (or a plain `Form`) is fine — `BarManager` does not require a special host. `XtraForm` is recommended so skins paint the title bar.

## Minimal Ribbon — Code Path

```csharp
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();

        var ribbon = new RibbonControl();
        Controls.Add(ribbon);

        // Optional: SVG icon collection shared by all items
        // ribbon.Images = svgImageCollection1;

        var pageHome = new RibbonPage("Home");
        ribbon.Pages.Add(pageHome);

        var groupFile = new RibbonPageGroup("File");
        pageHome.Groups.Add(groupFile);

        var itemNew = ribbon.Items.CreateButton("New");
        itemNew.Id = ribbon.Manager.GetNewItemId();   // required for SaveLayoutToXml round-trip
        itemNew.RibbonStyle = RibbonItemStyles.Large;
        itemNew.ItemClick += (s, e) => MessageBox.Show("New clicked");

        groupFile.ItemLinks.Add(itemNew);

        // Status bar at the bottom of the form
        var status = new RibbonStatusBar(ribbon) { Parent = this };
        var ready = new BarStaticItem { Caption = "Ready", Alignment = BarItemLinkAlignment.Right };
        ribbon.Items.Add(ready);
        status.ItemLinks.Add(ready);

        // Application caption (left of the form title, separated by " - ")
        ribbon.ApplicationCaption         = "My Application";
        ribbon.ApplicationDocumentCaption = "Untitled";
    }
}
```

After running:

- The ribbon integrates into the form's title bar.
- "My Application - Untitled" appears as the form caption.
- Clicking *New* shows the message box.
- The status bar at the bottom shows "Ready" right-aligned.

> The Code Path above is condensed for reading — it builds the ribbon in the constructor. For a form you want to keep editing in the WinForms designer, use the `.Designer.cs` split shown next instead.

## Authoring the `.Designer.cs` File

When you generate the form **in code**, write it the way the WinForms designer serializes it: declare the `RibbonControl`, its `RibbonPage`/`RibbonPageGroup`, and the `BarItem`s as fields, and build them — pages, groups, items, and `ItemLinks` — inside `InitializeComponent()` in `MainForm.Designer.cs`. Keep only `ItemClick` handlers and runtime data in `MainForm.cs`. If you instead `new` the ribbon and add pages/items in the form constructor body, the designer file stays empty and the form can no longer be opened in the Visual Studio WinForms designer.

**Rules of thumb — what goes where:**

| `MainForm.Designer.cs` (`InitializeComponent`) | `MainForm.cs` |
| --- | --- |
| `RibbonControl`, `RibbonPage`, `RibbonPageGroup`, `BarItem*` as **fields**; their property setup | `ItemClick` / event handlers |
| `Pages.AddRange`, `Groups.AddRange`, `ItemLinks.Add`, item `Id`/`Caption`/`ImageOptions`/`RibbonStyle` | Loading runtime data, populating editors |
| `BeginInit`/`EndInit`, `Controls.Add`, `this.Ribbon = ribbonControl1` | Wiring commands, opening documents |

**`MainForm.Designer.cs`** — one page, one group, one button, as the designer would emit it:

```csharp
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
    private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPageHome;
    private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonGroupFile;
    private DevExpress.XtraBars.BarButtonItem itemNew;

    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
        this.ribbonPageHome = new DevExpress.XtraBars.Ribbon.RibbonPage();
        this.ribbonGroupFile = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
        this.itemNew = new DevExpress.XtraBars.BarButtonItem();
        ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
        this.SuspendLayout();
        //
        // ribbonControl1
        //
        this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { this.itemNew });
        this.ribbonControl1.MaxItemId = 1;
        this.ribbonControl1.Name = "ribbonControl1";
        this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { this.ribbonPageHome });
        //
        // itemNew
        //
        this.itemNew.Caption = "New";
        this.itemNew.Id = 1;                      // unique id — required for layout (de)serialization
        this.itemNew.Name = "itemNew";
        this.itemNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
        //
        // ribbonPageHome
        //
        this.ribbonPageHome.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { this.ribbonGroupFile });
        this.ribbonPageHome.Name = "ribbonPageHome";
        this.ribbonPageHome.Text = "Home";
        //
        // ribbonGroupFile
        //
        this.ribbonGroupFile.ItemLinks.Add(this.itemNew);
        this.ribbonGroupFile.Name = "ribbonGroupFile";
        this.ribbonGroupFile.Text = "File";
        //
        // MainForm
        //
        this.Controls.Add(this.ribbonControl1);
        this.Name = "MainForm";
        this.Ribbon = this.ribbonControl1;        // link the ribbon to the RibbonForm
        this.Text = "My Application";
        ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
```

**`MainForm.cs`** — only behavior:

```csharp
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();                          // builds the ribbon, page, group, and button
        itemNew.ItemClick += (s, e) => MessageBox.Show("New clicked");
    }
}
```

## Minimal Bars (Classic) — Code Path

```csharp
using DevExpress.XtraBars;

var bars = new BarManager { Form = this };
bars.BeginUpdate();

var menuBar   = new Bar(bars, "Main Menu") { DockStyle = BarDockStyle.Top, DockRow = 0 };
var statusBar = new Bar(bars, "Status")    { DockStyle = BarDockStyle.Bottom };
bars.MainMenu  = menuBar;     // makes this bar the application's main menu
bars.StatusBar = statusBar;

var fileMenu = new BarSubItem(bars, "&File");
var newCmd   = new BarButtonItem(bars, "&New") { ItemShortcut = new BarShortcut(Keys.Control | Keys.N) };
newCmd.ItemClick += (_, _) => MessageBox.Show("New");
fileMenu.AddItem(newCmd);
menuBar.AddItem(fileMenu);

statusBar.AddItem(new BarStaticItem { Caption = "Ready" });

bars.EndUpdate();
```

`BarManager` automatically creates four `BarDockControl`s docked to all four sides of the form. A `Bar` with `DockStyle = Top/Bottom/Left/Right` lives inside the corresponding `BarDockControl`. `DockRow` orders multiple bars within one side.

## Pick One Per Form

`BarManager` and `RibbonControl` may **conflict** when used in the same form or user control. Pick one stack per form:

- Office-style app → `RibbonControl` + `RibbonForm` + `RibbonStatusBar` (+ optional `BackstageViewControl` or `ApplicationMenu`).
- Visual-Studio-style toolbars + main menu + status bar → `BarManager`.

If you need ribbon-style commands inside a window that uses bars, host them as bar items inside a `Bar` — do not embed a `RibbonControl` next to a `BarManager`.

## Verify the Setup

After a successful build and run:

- The form title bar uses the DevExpress skin chrome.
- The ribbon (if any) is integrated with the title bar — QAT is in the title bar, not below it.
- The application menu / Backstage View opens when you click the Application Button.
- Pressing `Ctrl+N` (if configured) raises the bound item's `ItemClick` even when the ribbon does not have focus — bar shortcuts are form-scoped.

## Common Issues

- **Ribbon below the title bar instead of integrated**: host form is not `RibbonForm`. Change the base class.
- **`BarManager` + `RibbonControl` together** produce strange focus behavior, drag-and-drop glitches, and double menu. Remove one.
- **`SaveLayoutToXml` writes nothing useful**: bar items created in code without `Id`. Call `manager.GetNewItemId()` for every item.
- **Skin not applied**: `SetSkinStyle` called after the form is shown. Move it to the top of `Main`.
- **Bar items invisible**: created but never added to a `Bar.ItemLinks` / `RibbonPageGroup.ItemLinks` / `RibbonControl.Toolbar.ItemLinks` — items are non-visual until linked.
- **Title bar overlay broken in .NET 8+**: confirm the project targets `net*-windows` and `<UseWindowsForms>true</UseWindowsForms>` is set; non-Windows targets do not support `RibbonForm`'s title-bar painting.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/index.md` — Ribbon, Bars, and Menus (`xref:WindowsForms.1199`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/the-ribbon-ui/index.md` — Ribbon UI (`xref:WindowsForms.118333`).
- `articles/controls-and-libraries/forms-and-user-controls/ribbon-form.md` — RibbonForm (`xref:WindowsForms.114562`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/get-started.md` — Ribbon get-started (`xref:WindowsForms.402959`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/index.md` — Bars overview (`xref:WindowsForms.5361`).
- `api/DevExpress.XtraBars.Ribbon.RibbonForm.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonControl.yml`.
- `api/DevExpress.XtraBars.BarManager.yml`.
