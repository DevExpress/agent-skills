// DevExpress WinForms TreeList (XtraTreeList) — Quickstart (C#)
// Demonstrates: designer-backed bound form, unbound mode, dynamic loading,
//               find/focus/expand, in-place editor, total summary.
// Package: DevExpress.Win.TreeList   Host form: XtraForm
//
// Section 1 shows the RECOMMENDED split for a designer-backed form: the control,
// columns, and in-place editors live in MainForm.Designer.cs (InitializeComponent);
// only data loading and events live in MainForm.cs. Keep that split so the form
// stays editable in the Visual Studio WinForms designer. Sections 2-5 use the
// runtime API — appropriate for code-only controls or dynamically built structure.

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

// ------------------------------------------------------------------
// 1. Designer-backed bound form (RECOMMENDED layout)
//    Control, columns, and the in-place editor are declared in the
//    *.Designer.cs partial below (InitializeComponent). The code-behind
//    only loads data, so the form stays editable in the WinForms designer.
// ------------------------------------------------------------------

// --- MainForm.cs — data + behavior only ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                 // builds treeList + columns + editor
        treeList.DataSource = GetEmployees();  // columns are defined in the designer file
        treeList.ExpandAll();
    }

    List<Employee> GetEmployees() => new() {
        new Employee { ID = 1, ParentID = -1, Name = "Gregory S. Price", Position = "President", Sales = 0m },
        new Employee { ID = 2, ParentID = 1,  Name = "Irma R. Marshall", Position = "VP",        Sales = 120000m },
        new Employee { ID = 3, ParentID = 1,  Name = "John C. Powell",   Position = "VP",        Sales = 98000m },
        new Employee { ID = 4, ParentID = 2,  Name = "Brian C. Cowling", Position = "Manager",   Sales = 54000m },
    };
}

// --- MainForm.Designer.cs — structure the WinForms designer round-trips ---
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private TreeList treeList;
    private TreeListColumn colName;
    private TreeListColumn colPosition;
    private TreeListColumn colSales;
    private RepositoryItemSpinEdit repoSales;

    private void InitializeComponent() {
        this.components  = new System.ComponentModel.Container();
        this.treeList    = new TreeList();
        this.colName     = new TreeListColumn();
        this.colPosition = new TreeListColumn();
        this.colSales    = new TreeListColumn();
        this.repoSales   = new RepositoryItemSpinEdit();
        ((System.ComponentModel.ISupportInitialize)(this.treeList)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.repoSales)).BeginInit();
        this.SuspendLayout();
        //
        // treeList
        //
        this.treeList.Columns.AddRange(new TreeListColumn[] {
            this.colName, this.colPosition, this.colSales});
        this.treeList.Dock = DockStyle.Fill;
        this.treeList.KeyFieldName = "ID";
        this.treeList.Name = "treeList";
        this.treeList.ParentFieldName = "ParentID";
        this.treeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoSales});
        this.treeList.RootValue = -1;                          // records with ParentID == -1 are roots
        this.treeList.OptionsView.ShowSummaryFooter = true;
        //
        // colName / colPosition
        //
        this.colName.Caption = "Name"; this.colName.FieldName = "Name"; this.colName.Name = "colName";
        this.colName.Visible = true; this.colName.VisibleIndex = 0;
        this.colPosition.Caption = "Position"; this.colPosition.FieldName = "Position"; this.colPosition.Name = "colPosition";
        this.colPosition.Visible = true; this.colPosition.VisibleIndex = 1;
        //
        // colSales — in-place editor + total summary, defined in the designer file
        //
        this.colSales.Caption = "Sales"; this.colSales.FieldName = "Sales"; this.colSales.Name = "colSales";
        this.colSales.ColumnEdit = this.repoSales;
        this.colSales.SummaryFooter = SummaryItemType.Sum;
        this.colSales.SummaryFooterStrFormat = "Total={0:c0}";
        this.colSales.Visible = true; this.colSales.VisibleIndex = 2;
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

public class Employee {
    public int ID { get; set; }
    public int ParentID { get; set; }   // RootValue (-1) → root node
    public string Name { get; set; } = "";
    public string Position { get; set; } = "";
    public decimal Sales { get; set; }  // bound "Sales" column (editor + summary configured in the designer file above)
}

// ------------------------------------------------------------------
// 2. Unbound mode — build the tree in code (columns must exist first)
// ------------------------------------------------------------------
public static class UnboundTree {
    public static void Build(TreeList treeList) {
        treeList.BeginUnboundLoad();
        TreeListNode root = treeList.AppendNode(new object[] { "Alfreds Futterkiste", "030-0074321" }, null);
        treeList.AppendNode(new object[] { "Michael Suyama", "030-0074263" }, root);
        treeList.EndUnboundLoad();
    }
}

// ------------------------------------------------------------------
// 3. Dynamic (on-demand) loading
// ------------------------------------------------------------------
public static class DynamicLoading {
    public static void Wire(TreeList treeList) {
        // Mark nodes that may have children so the expand button is visible.
        foreach (TreeListNode root in treeList.Nodes)
            root.HasChildren = true;

        treeList.BeforeExpand += (s, e) => {
            if (e.Node.Nodes.Count == 0) {                  // load once
                foreach (var child in LoadChildren(e.Node)) {
                    TreeListNode n = treeList.AppendNode(child, e.Node);
                    n.HasChildren = true;                   // set based on your data
                }
            }
        };
    }

    static IEnumerable<object[]> LoadChildren(TreeListNode parent) => new List<object[]>();
}

// ------------------------------------------------------------------
// 4. Find, focus, and expand a node
// ------------------------------------------------------------------
public static class NodeOps {
    public static void Demo(TreeList treeList) {
        TreeListNode node = treeList.FindNodeByFieldValue("Name", "John C. Powell");
        treeList.FocusedNode = node;
        node.Expanded = true;
    }
}

// ------------------------------------------------------------------
// 5. In-place editor + total summary in a column footer (RUNTIME API)
//    Section 1 configures the same thing in the designer file (preferred for
//    designer-backed forms). Use this runtime form for code-only controls or
//    when the column/editor is decided at run time.
// ------------------------------------------------------------------
public static class EditorAndSummary {
    public static void Apply(TreeList treeList) {
        var spin = new RepositoryItemSpinEdit();
        spin.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spin.DisplayFormat.FormatString = "c0";
        treeList.RepositoryItems.Add(spin);
        treeList.Columns["Sales"].ColumnEdit = spin;

        treeList.Columns["Sales"].SummaryFooter = SummaryItemType.Sum;
        treeList.Columns["Sales"].SummaryFooterStrFormat = "Total={0:c0}";
        treeList.OptionsView.ShowSummaryFooter = true;
    }
}
