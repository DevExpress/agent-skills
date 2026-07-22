# Getting Started with the DevExpress WinForms TreeList (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, every other reference in this skill applies identically to both target frameworks.

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- DevExpress WinForms subscription via the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (designer-first). Run the installer, then create the project from the **DevExpress Template Gallery** (File → New → Project → DevExpress v26.1 Template Gallery → *WinForms Application*), or drag `TreeList` from the toolbox onto a form. References are added automatically.
2. **NuGet package** (recommended for source control / CI):

   ```powershell
   Install-Package DevExpress.Win.TreeList
   ```

## Designer Workflow (.NET Framework)

1. Drop a `TreeList` on an `XtraForm`; set `Dock = Fill`.
2. Open the smart tag → **Run Designer** → add columns (FieldName, Caption, in-place editors) and, for unbound mode, nodes.
3. For bound mode, set `DataSource` via the smart tag, then `KeyFieldName`, `ParentFieldName`, and `RootValue`.
4. **Data Source Configuration Wizard / typed DataSets** are available in .NET Framework projects (they are *not* in .NET SDK projects) — convenient when binding to a database.

## Required Assemblies (Manual Reference)

Prefer the NuGet package (it pulls all dependencies). If you reference assemblies directly, add (replace `26.1` with your version):

- `DevExpress.XtraTreeList.v26.1.dll` (the control)
- `DevExpress.XtraEditors.v26.1.dll`, `DevExpress.Utils.v26.1.dll`, `DevExpress.Data.v26.1.dll` (core dependencies)
- `DevExpress.Printing.v26.1.Core.dll` (only for print / export)

## Minimal Bound Example

The snippet below is condensed (control created in the constructor) to show the binding fields only. **For a designer-backed form, declare the `TreeList`, its columns, and editors in `InitializeComponent()` in `MainForm.Designer.cs`** so the form stays editable in the WinForms designer — `.NET Framework` projects serialize the designer file exactly the same way as SDK-style projects. See the worked [`.Designer.cs` example in getting-started.md](getting-started.md#authoring-the-designercs-file).

```csharp
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                  // designer-backed form: control + columns live here
        treeList.DataSource = GetData();        // any IList / BindingList / DataTable
        treeList.ExpandAll();
    }
}
```

See [getting-started.md](getting-started.md) for the full data model and the full designer-file split, and [data-binding.md](data-binding.md) for bound/unbound/dynamic modes.
