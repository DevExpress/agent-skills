# Getting Started (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all ribbon/bars code (pages, groups, items, links, merging, appearance) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Install the NuGet Package

```powershell
# SDK-style project
dotnet add package DevExpress.Win.Navigation

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.Navigation
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the package is also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
</ItemGroup>
```

> Both commands above add the required assembly references.

This package ships everything in `DevExpress.XtraBars` and `DevExpress.XtraBars.Ribbon` (in `DevExpress.XtraBars.v<version>.dll`).

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.XtraBars.v<version>.dll` (ribbon, bars, items)
- `DevExpress.XtraEditors.v<version>.dll`, `DevExpress.Utils.v<version>.dll`, `DevExpress.Data.v<version>.dll` (core dependencies)

Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Common Imports

```csharp
using DevExpress.XtraBars;            // BarManager, Bar, BarItem, BarButtonItem, ItemClickEventArgs
using DevExpress.XtraBars.Ribbon;     // RibbonControl, RibbonForm, RibbonPage, RibbonPageGroup, RibbonStatusBar
using DevExpress.Utils;               // SvgImageCollection
using DevExpress.Utils.Svg;           // SvgImage
```

## Host Form and Skins

The host form for a `RibbonControl` must inherit from `DevExpress.XtraBars.Ribbon.RibbonForm`; for the classic Bars UI, `XtraForm` (or a plain `Form`) is fine. Apply a skin once, before any form is shown, in `Program.Main`:

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

## Minimal Ribbon in Code

The ribbon API is identical to the .NET 8+ guide:

```csharp
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();

        var ribbon = new RibbonControl();
        Controls.Add(ribbon);

        var pageHome  = new RibbonPage("Home");
        ribbon.Pages.Add(pageHome);
        var groupFile = new RibbonPageGroup("File");
        pageHome.Groups.Add(groupFile);

        var itemNew = ribbon.Items.CreateButton("New");
        itemNew.Id = ribbon.Manager.GetNewItemId();   // required for SaveLayoutToXml round-trip
        itemNew.RibbonStyle = RibbonItemStyles.Large;
        itemNew.ItemClick += (s, e) => MessageBox.Show("New clicked");
        groupFile.ItemLinks.Add(itemNew);
    }
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraBars.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([items-and-settings.md](items-and-settings.md), [ribbon-structure.md](ribbon-structure.md), [bars-layout.md](bars-layout.md), [merging.md](merging.md), [appearance-customization.md](appearance-customization.md)) apply identically to both .NET and .NET Framework once the project is configured.
