# Getting Started

This reference covers how to add DevExpress WinForms Layout controls to a project: which NuGet packages to install, which assemblies and namespaces to reference, and the minimum code required to get each control on screen.

## NuGet Package

All layout and docking controls ship in a single NuGet package:

```
DevExpress.Win.Navigation
```

Install it with `dotnet add package DevExpress.Win.Navigation`. This package includes all of the following controls and their assemblies:

| Control | Assembly | Namespace |
|---|---|---|
| `LayoutControl` | `DevExpress.XtraLayout.v26.1.dll` | `DevExpress.XtraLayout` |
| `DataLayoutControl` | `DevExpress.XtraLayout.v26.1.dll` | `DevExpress.XtraDataLayout` |
| `DockManager` + `DockPanel` | `DevExpress.XtraBars.v26.1.dll` | `DevExpress.XtraBars.Docking` |
| `StackPanel` / `TablePanel` | `DevExpress.Utils.v26.1.dll` | `DevExpress.Utils.Layout` |

> If your project already references `DevExpress.Win` or `DevExpress.Win.Design`, no additional package is needed — they both include `DevExpress.Win.Navigation` transitively.

## Host Form

Use `DevExpress.XtraEditors.XtraForm` (or `RibbonForm` when pairing with a ribbon) as the host form for consistent skin integration. Plain `System.Windows.Forms.Form` works but appearance may be inconsistent.

## Common Namespaces

```csharp
using DevExpress.XtraLayout;          // LayoutControl, LayoutControlItem, LayoutControlGroup
using DevExpress.XtraLayout.Utils;    // LayoutMode (Flow/Table layout mode for a group)
using DevExpress.XtraDataLayout;      // DataLayoutControl
using DevExpress.XtraBars.Docking;    // DockManager, DockPanel, DockingStyle
using DevExpress.XtraEditors;         // XtraForm, TextEdit, SimpleButton
using DevExpress.Utils.Layout;        // StackPanel, TablePanel
```

---

## LayoutControl — Minimum Setup

Add the control to the form and call `AddItem` for each child control — the `LayoutControl` automatically wraps them in `LayoutControlItem` wrappers:

```csharp
public partial class MainForm : DevExpress.XtraEditors.XtraForm
{
    public MainForm()
    {
        InitializeComponent();

        var lc = new LayoutControl { Dock = DockStyle.Fill };
        Controls.Add(lc);

        lc.BeginUpdate();
        try {
            var nameEdit = new TextEdit { Name = "edName" };
            var emailEdit = new TextEdit { Name = "edEmail" };

            lc.AddItem("Name",  nameEdit);
            lc.AddItem("Email", emailEdit);
        }
        finally {
            lc.EndUpdate();
        }
    }
}
```

---

## DataLayoutControl — Minimum Setup

Set `DataSource`, then call `RetrieveFields()` to auto-generate the layout:

```csharp
var dlc = new DataLayoutControl { Dock = DockStyle.Fill };
dlc.DataSource = myBindingSource;   // BindingSource, DataTable, or business object
Controls.Add(dlc);
dlc.RetrieveFields();               // generate layout from data source fields
```

---

## DockManager — Minimum Setup

`DockManager` is a non-visual component — create it, assign the form, and add panels with `AddPanel`:

```csharp
using DevExpress.XtraBars.Docking;

public partial class MainForm : XtraForm
{
    public MainForm()
    {
        InitializeComponent();

        var dm = new DockManager();
        dm.Form = this;

        // Create panels
        DockPanel leftPanel  = dm.AddPanel(DockingStyle.Left);
        leftPanel.Text = "Explorer";
        leftPanel.Width = 220;

        DockPanel bottomPanel = dm.AddPanel(DockingStyle.Bottom);
        bottomPanel.Text = "Output";
        bottomPanel.Height = 120;

        // Place a control inside a panel
        var tree = new TreeView { Dock = DockStyle.Fill };
        leftPanel.Controls.Add(tree);
    }
}
```

---

## StackPanel / TablePanel — Minimum Setup

Lightweight containers — no NuGet step. `StackPanel`, `TablePanel`, and their helper types (`StackPanelLayoutDirection`, `TablePanelEntityStyle`, `TablePanelRow`, `TablePanelColumn`) live in the `DevExpress.Utils.Layout` namespace — add `using DevExpress.Utils.Layout;`.

```csharp
// StackPanel — controls flow vertically (or horizontally)
var stack = new StackPanel {
    Dock = DockStyle.Fill,
    LayoutDirection = StackPanelLayoutDirection.TopDown
};
stack.Controls.Add(new SimpleButton { Text = "OK",     Width = 80 });
stack.Controls.Add(new SimpleButton { Text = "Cancel", Width = 80 });
Controls.Add(stack);

// TablePanel — grid-like rows and columns
var table = new TablePanel { Dock = DockStyle.Fill };
table.Rows.Add(new TablePanelRow(TablePanelEntityStyle.Relative, 50));
table.Rows.Add(new TablePanelRow(TablePanelEntityStyle.Relative, 50));
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Relative, 50));
table.Columns.Add(new TablePanelColumn(TablePanelEntityStyle.Relative, 50));

var btn = new SimpleButton { Text = "Top-Left" };
table.Controls.Add(btn);
table.SetCell(btn, 0, 0);   // SetCell(control, row, column)
Controls.Add(table);
```

---

## Authoring the `.Designer.cs` File

**This is the default way to generate a layout.** Write it the way the Visual Studio WinForms designer serializes it — declare the `LayoutControl` (or `DataLayoutControl`/`DockManager`), its `LayoutControlGroup`/`LayoutControlItem`s as **fields**, and configure them inside `InitializeComponent()` in `MainForm.Designer.cs`. The `LayoutControl`, its root group, and every item implement `ISupportInitialize`, so each is wrapped in `BeginInit()`/`EndInit()`. Keep only data loading and event handlers in `MainForm.cs`. If you instead `new` the `LayoutControl` and call `AddItem`/`AddGroup` in the form constructor body, the designer file stays empty and the form can no longer be edited in the WinForms designer.

**Rules of thumb — what goes where:**

| `MainForm.Designer.cs` (`InitializeComponent`) | `MainForm.cs` |
| --- | --- |
| `LayoutControl`, `LayoutControlGroup`, `LayoutControlItem`, hosted editors as **fields**; their property setup | Loading data / assigning `DataSource` |
| `Root`/`Items.AddRange`, `Control`, `Text`, `Name`, `MinSize`/`MaxSize` | `RetrieveFields()` for a `DataLayoutControl` |
| `BeginInit`/`EndInit` (control + root group + each item), `Controls.Add` | Event handlers, runtime customization |

**`MainForm.Designer.cs`** — a `LayoutControl` with two labeled editors, as the designer emits it:

```csharp
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.XtraLayout.LayoutControl layoutControl1;
    private DevExpress.XtraLayout.LayoutControlGroup rootGroup;
    private DevExpress.XtraEditors.TextEdit edName;
    private DevExpress.XtraEditors.TextEdit edEmail;
    private DevExpress.XtraLayout.LayoutControlItem lciName;
    private DevExpress.XtraLayout.LayoutControlItem lciEmail;

    private void InitializeComponent() {
        this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
        this.rootGroup = new DevExpress.XtraLayout.LayoutControlGroup();
        this.edName = new DevExpress.XtraEditors.TextEdit();
        this.edEmail = new DevExpress.XtraEditors.TextEdit();
        this.lciName = new DevExpress.XtraLayout.LayoutControlItem();
        this.lciEmail = new DevExpress.XtraLayout.LayoutControlItem();
        ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
        this.layoutControl1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.rootGroup)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edName.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edEmail.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciName)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciEmail)).BeginInit();
        //
        // layoutControl1
        //
        this.layoutControl1.Controls.Add(this.edName);
        this.layoutControl1.Controls.Add(this.edEmail);
        this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layoutControl1.Name = "layoutControl1";
        this.layoutControl1.Root = this.rootGroup;
        //
        // edName / edEmail (hosted editors — no Dock/Anchor; the LayoutControlItem positions them)
        //
        this.edName.Name = "edName";
        this.edEmail.Name = "edEmail";
        //
        // rootGroup
        //
        this.rootGroup.Name = "Root";
        this.rootGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciName, this.lciEmail});
        //
        // lciName / lciEmail
        //
        this.lciName.Control = this.edName;
        this.lciName.Name = "lciName";
        this.lciName.Text = "Name";
        this.lciEmail.Control = this.edEmail;
        this.lciEmail.Name = "lciEmail";
        this.lciEmail.Text = "Email";
        //
        // MainForm
        //
        this.Controls.Add(this.layoutControl1);
        this.Name = "MainForm";
        this.Text = "Edit";
        ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
        this.layoutControl1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.rootGroup)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edName.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edEmail.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciName)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciEmail)).EndInit();
    }
}
```

For a `DataLayoutControl`, declare it as a field and set `DataSource`/`RetrieveFields()` in `MainForm.cs` — let it generate the items rather than adding them by hand.

## Source Material

- `LayoutControl` class: `xref:DevExpress.XtraLayout.LayoutControl`
- `DataLayoutControl` class: `xref:DevExpress.XtraDataLayout.DataLayoutControl`
- `DockManager` class: `xref:DevExpress.XtraBars.Docking.DockManager`
- Form Layout Managers overview: `https://docs.devexpress.com/content/WindowsForms/114577?md=true`
