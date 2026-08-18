# Getting Started with TreeList

> **.NET Framework?** For .NET Framework 4.6.2+ project setup, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).


`TreeList` (namespace `DevExpress.XtraTreeList`) is a data-aware control that displays data as a tree or a multi-column tree-grid, in bound or unbound mode.

## When to Use This Reference

- Adding `TreeList` to a project for the first time
- Choosing the host form and enabling skins
- Binding a self-referential tree end-to-end

## NuGet Package

```
DevExpress.Win.TreeList
```

This package ships `DevExpress.XtraTreeList.v26.1.dll`. For print preview / export, also add `DevExpress.Win.Printing`.

> **Install the package:**
> ```powershell
> dotnet add package DevExpress.Win.TreeList
> ```

## Required Namespace Imports

```csharp
using DevExpress.XtraTreeList;             // TreeList, options, events
using DevExpress.XtraTreeList.Columns;     // TreeListColumn
using DevExpress.XtraTreeList.Nodes;       // TreeListNode
using DevExpress.XtraEditors;              // XtraForm
using DevExpress.XtraEditors.Repository;   // RepositoryItem* in-place editors
```

## Host Form and Skins

Host on a DevExpress form (`XtraForm`, `RibbonForm`, or `FluentDesignForm`) and enable skins before any form is shown, in `Program.Main`:

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

## Authoring the `.Designer.cs` File

Write the form the way the WinForms designer serializes it: put the control, its columns, and its in-place editors in `InitializeComponent()` inside `MainForm.Designer.cs`, and keep only data loading and event handlers in `MainForm.cs`. This is what keeps the form openable in the Visual Studio WinForms designer — if you instead `new` the `TreeList` and build its columns in the form constructor body, the designer file stays empty and the form can no longer be edited visually.

**Rules of thumb — what goes where:**

| `MainForm.Designer.cs` (`InitializeComponent`) | `MainForm.cs` |
| --- | --- |
| `TreeList`, `TreeListColumn`s, `RepositoryItem*` editors as **fields**; their construction and property setup | `DataSource` assignment / loading data |
| `KeyFieldName`, `ParentFieldName`, `RootValue`, `Columns.AddRange`, `RepositoryItems.AddRange`, `ColumnEdit` | Building unbound nodes (`AppendNode`) |
| `BeginInit`/`EndInit`, `SuspendLayout`/`ResumeLayout`, `Controls.Add` | Event handlers (`BeforeExpand`, `FocusedNodeChanged`, …), `ExpandAll()` |

**`MainForm.Designer.cs`** — explicit `Name` and `Sales` columns with a spin-edit in-place editor, exactly as the designer would emit it:

```csharp
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.XtraTreeList.TreeList treeList;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
    private DevExpress.XtraTreeList.Columns.TreeListColumn colSales;
    private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoSales;

    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.treeList = new DevExpress.XtraTreeList.TreeList();
        this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
        this.colSales = new DevExpress.XtraTreeList.Columns.TreeListColumn();
        this.repoSales = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
        ((System.ComponentModel.ISupportInitialize)(this.treeList)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.repoSales)).BeginInit();
        this.SuspendLayout();
        //
        // treeList
        //
        this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colSales});
        this.treeList.Dock = System.Windows.Forms.DockStyle.Fill;
        this.treeList.KeyFieldName = "ID";
        this.treeList.Name = "treeList";
        this.treeList.ParentFieldName = "ParentID";
        this.treeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoSales});
        this.treeList.RootValue = -1;
        //
        // colName
        //
        this.colName.Caption = "Name";
        this.colName.FieldName = "Name";
        this.colName.Name = "colName";
        this.colName.Visible = true;
        this.colName.VisibleIndex = 0;
        //
        // colSales
        //
        this.colSales.Caption = "Sales";
        this.colSales.ColumnEdit = this.repoSales;
        this.colSales.FieldName = "Sales";
        this.colSales.Name = "colSales";
        this.colSales.Visible = true;
        this.colSales.VisibleIndex = 1;
        //
        // repoSales
        //
        this.repoSales.DisplayFormat.FormatString = "c0";
        this.repoSales.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        this.repoSales.Name = "repoSales";
        //
        // MainForm
        //
        this.Controls.Add(this.treeList);
        this.Name = "MainForm";
        this.Text = "Employees";
        ((System.ComponentModel.ISupportInitialize)(this.treeList)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.repoSales)).EndInit();
        this.ResumeLayout(false);
    }
}
```

**`MainForm.cs`** — only data and behavior:

```csharp
using System.Collections.Generic;
using DevExpress.XtraEditors;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();             // builds the TreeList, its columns, and the editor
        treeList.DataSource = GetData();   // columns are already defined in the designer file
        treeList.ExpandAll();
    }

    List<Employee> GetData() => new() {
        new Employee { ID = 1, ParentID = -1, Name = "Gregory S. Price", Sales = 0m },
        new Employee { ID = 2, ParentID = 1,  Name = "Irma R. Marshall", Sales = 120000m },
        new Employee { ID = 3, ParentID = 1,  Name = "John C. Powell",   Sales = 98000m },
    };
}

public class Employee {
    public int ID { get; set; }
    public int ParentID { get; set; }   // RootValue (-1) → root node
    public string Name { get; set; } = "";
    public decimal Sales { get; set; }
}
```

> Auto-populate matches existing columns by `FieldName`, so the columns you declared above are reused rather than duplicated; to suppress *any* extra auto-generated columns for other data-source fields, set `treeList.OptionsBehavior.AutoPopulateColumns = false` in the designer file as well. The form still opens cleanly in the WinForms designer.

## Minimal Bound Setup in Code

This condensed form (control created in the constructor) is fine for a throwaway prototype or a control hosted in code you never open in the designer. For a form you want to keep editing visually, use the designer-file split shown above instead.

```csharp
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var treeList = new TreeList { Parent = this, Dock = DockStyle.Fill };
        treeList.KeyFieldName    = "ID";
        treeList.ParentFieldName = "ParentID";
        treeList.RootValue       = -1;            // records with ParentID == -1 are roots
        treeList.DataSource      = GetData();     // columns auto-generate
        treeList.ExpandAll();
    }

    List<Region> GetData() => new() {
        new Region { ID = 1, ParentID = -1, Name = "Europe" },
        new Region { ID = 2, ParentID = 1,  Name = "Germany" },
        new Region { ID = 3, ParentID = 1,  Name = "France" },
        new Region { ID = 4, ParentID = -1, Name = "Asia" },
        new Region { ID = 5, ParentID = 4,  Name = "Japan" },
    };
}

public class Region {
    public int ID { get; set; }
    public int ParentID { get; set; }   // RootValue (-1) → root node
    public string Name { get; set; } = "";
}
```

Key points:
- The hierarchy comes from `KeyFieldName` + `ParentFieldName`; root nodes are records whose `ParentFieldName` value equals `RootValue`.
- Columns auto-generate from the data source unless `OptionsBehavior.AutoPopulateColumns` is disabled (then call `PopulateColumns()`).
- Host on `XtraForm` and enable skins for correct theming.

## Source Material

- `articles/controls-and-libraries/tree-list.md` (`xref:2434`) — Tree List overview
- `articles/controls-and-libraries/tree-list/feature-center/data-binding/bound-mode.md` (`xref:116708`)
- `articles/controls-and-libraries/tree-list/examples/data-binding/how-to-create-treelist-at-runtime.md` (`xref:403837`)
- [TreeList](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList?md=true) — class reference
