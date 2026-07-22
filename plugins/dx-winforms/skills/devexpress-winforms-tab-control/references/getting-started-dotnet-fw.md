# Getting Started with XtraTabControl (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all `XtraTabControl` code (pages, headers, events, appearance) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

You can add DevExpress to a .NET Framework project in two ways:

1. **Unified Component Installer** (recommended for designer-first workflows). Run the installer, then drag `XtraTabControl` from the Visual Studio Toolbox onto an `XtraForm`. The IDE adds the required references automatically.
2. **NuGet packages** (recommended for source control and CI builds). Install `DevExpress.Win.Navigation` from nuget.org.

## Path 1: Unified Component Installer + Toolbox

1. Install the DevExpress Universal/WinForms Component Installer.
2. In Visual Studio, create or open a **.NET Framework** Windows Forms App, or use the **DevExpress Template Gallery** to scaffold a skinned WinForms application.
3. Make the main form derive from `XtraForm` (or `RibbonForm` / `FluentDesignForm`).
4. Drag `XtraTabControl` from the Toolbox onto the form; set `Dock = Fill`. The control starts with two pages.
5. Open the control's **smart tag** → **"Tab Pages…"** to add, reorder, and configure `XtraTabPage` pages, then drag controls onto each page.

## Path 2: NuGet Package

Install via the Package Manager Console:

```powershell
Install-Package DevExpress.Win.Navigation
```

> If your project uses the older `packages.config` instead of `PackageReference`, the same command works — Visual Studio installs the package and adds the required assembly references.

This package ships `DevExpress.XtraEditors.v<version>.dll`, which contains the `DevExpress.XtraTab` namespace (`XtraTabControl`, `XtraTabPage`, and `DevExpress.XtraTab.ViewInfo` / `DevExpress.XtraTab.Buttons`).

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.Data.v<version>.dll`
- `DevExpress.Utils.v<version>.dll`
- `DevExpress.XtraEditors.v<version>.dll`

Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Enable Skins at Startup

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

The control API is identical to the .NET 8+ guide:

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

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraEditors.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([pages-and-content.md](pages-and-content.md), [headers-and-layout.md](headers-and-layout.md), [events-and-closing.md](events-and-closing.md), [appearance-and-customization.md](appearance-and-customization.md)) apply identically to both .NET and .NET Framework once the project is configured.

## Source Material

- `articles/controls-and-libraries/form-layout-managers/tab-control.md` (`xref:114576`) — Tab Control overview
- [XtraTabControl](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabControl?md=true) — `XtraTabControl` class reference
- [XtraTabPage](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTab.XtraTabPage?md=true) — `XtraTabPage` class reference
