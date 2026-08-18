# Getting Started with XtraTabControl

`XtraTabControl` (namespace `DevExpress.XtraTab`) is a container that organizes other controls into tab pages. Each page is an `XtraTabPage` with a clickable header.

## When to Use This Reference

- Adding `XtraTabControl` to a project for the first time
- Choosing the host form and enabling skins
- Creating a first control with two pages (designer or code)

## NuGet Package

```
DevExpress.Win.Navigation
```

This package ships `DevExpress.XtraEditors.v26.1.dll`, which contains the `DevExpress.XtraTab` namespace (`XtraTabControl`, `XtraTabPage`, the event-args classes, and `DevExpress.XtraTab.ViewInfo` / `DevExpress.XtraTab.Buttons`).

> **Install the package:**
> ```powershell
> dotnet add package DevExpress.Win.Navigation
> ```

## Required Namespace Imports

```csharp
using DevExpress.XtraTab;            // XtraTabControl, XtraTabPage, event args
using DevExpress.XtraTab.ViewInfo;   // ClosePageButtonEventArgs, CustomHeaderButtonEventArgs
using DevExpress.XtraEditors;        // XtraForm, SimpleButton
```

## Host Form and Skins

`XtraTabControl` renders correctly when hosted on a DevExpress form and skins are enabled:

| Host Form | When to Use |
|---|---|
| `XtraForm` | Standard case — a tab control docked into a form area. |
| `RibbonForm` | App has a `RibbonControl`. |
| `FluentDesignForm` | Fluent / Acrylic UI. |

Enable skins once, before any form is shown, in `Program.Main`:

```csharp
static class Program {
    [STAThread]
    static void Main() {
        DevExpress.XtraEditors.WindowsFormsSettings.LoadApplicationSettings();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("WXI");
        Application.Run(new MainForm());
    }
}
```

## Minimal Setup in Code

```csharp
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var tabControl = new XtraTabControl { Dock = DockStyle.Fill };
        Controls.Add(tabControl);

        var page1 = new XtraTabPage { Text = "Page 1" };
        page1.Controls.Add(new SimpleButton {
            Text = "Button #1", Size = new Size(160, 32), Location = new Point(12, 12)
        });

        var page2 = new XtraTabPage { Text = "Page 2" };

        tabControl.TabPages.AddRange(new[] { page1, page2 });
    }
}
```

You can also add pages by caption directly — the collection creates the `XtraTabPage` for you:

```csharp
tabControl.TabPages.Add("Page 1");
tabControl.TabPages.Add("Page 2");
```

Key points:
- Root pages go into `XtraTabControl.TabPages`; page content goes into `XtraTabPage.Controls`.
- The first page added is the initial `SelectedTabPage`.
- Host on `XtraForm` and enable skins for correct theming.

## Source Material

- `articles/controls-and-libraries/form-layout-managers/tab-control.md` (`xref:114576`) — Tab Control overview
- [XtraTabControl](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabControl?md=true) — `XtraTabControl` class reference
- [XtraTabPage](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabPage?md=true) — `XtraTabPage` class reference
