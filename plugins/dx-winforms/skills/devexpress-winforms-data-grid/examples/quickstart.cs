// DevExpress WinForms Data Grid (GridControl) — Quickstart (C#)
// Demonstrates: GridView binding, CardView conversion, read-only + auto-filter,
//               per-cell editor switching, unbound column, web-style multi-select,
//               TreeList unbound with checkboxes.
// Package: DevExpress.Win.Grid (+ DevExpress.Win.TreeList for the tree)   Host: XtraForm

using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

// ------------------------------------------------------------------
// 1. Bind a GridControl + GridView to in-memory data (designer-backed)
//    The control, its view, and the static view options are declared in
//    the *.Designer.cs partial below (InitializeComponent), so the form
//    stays editable in the Visual Studio WinForms designer. Only the data
//    source — which is genuinely runtime — is assigned in the code-behind.
// ------------------------------------------------------------------

// --- MainForm.cs — data only (columns auto-create from the source) ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                 // builds gridControl1 + gridView1 and its options
        gridControl1.DataSource = LoadOrders(); // any IList / BindingList / DataTable
        // Columns auto-create when gridView1.OptionsBehavior.AutoPopulateColumns is true (default).
        // Add explicit columns in MainForm.Designer.cs when you need captions / order / editors.
    }

    System.Collections.IList LoadOrders() => new System.Collections.Generic.List<object>();
}

// --- MainForm.Designer.cs — control, view, and static view options ---
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private GridControl gridControl1;
    private GridView gridView1;

    private void InitializeComponent() {
        this.gridControl1 = new GridControl();
        this.gridView1 = new GridView();
        ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
        this.SuspendLayout();
        //
        // gridControl1
        //
        this.gridControl1.Dock = DockStyle.Fill;
        this.gridControl1.MainView = this.gridView1;
        this.gridControl1.Name = "gridControl1";
        this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
        //
        // gridView1 — static presentation options belong in the designer file
        //
        this.gridView1.GridControl = this.gridControl1;
        this.gridView1.Name = "gridView1";
        this.gridView1.OptionsBehavior.Editable = false;
        this.gridView1.OptionsView.ShowAutoFilterRow = true;
        this.gridView1.OptionsView.ShowGroupPanel = true;
        //
        // MainForm
        //
        this.Controls.Add(this.gridControl1);
        this.Name = "MainForm";
        this.Text = "Orders";
        ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
        this.ResumeLayout(false);
    }
}

// ------------------------------------------------------------------
// 2. Convert a GridView to a CardView
// ------------------------------------------------------------------
public static class CardViewSetup {
    public static void Apply(GridControl grid) {
        var cardView = new CardView(grid);
        grid.ViewCollection.Add(cardView);
        grid.MainView = cardView;
        cardView.CardWidth = 240;
        cardView.OptionsView.ShowQuickCustomizeButton = false;
    }
}

// ------------------------------------------------------------------
// 3. Per-cell editor switching (read-only editor for some rows)
// ------------------------------------------------------------------
public static class PerCellEditor {
    public static void Wire(GridView view, DevExpress.XtraEditors.Repository.RepositoryItem readonlyRepository) {
        view.CustomRowCellEdit += (s, e) => {
            if (e.Column.FieldName == "Value"
                && !(bool)view.GetRowCellValue(e.RowHandle, "AllowEdit"))
                e.RepositoryItem = readonlyRepository;
        };
    }
}

// ------------------------------------------------------------------
// 4. Unbound calculated column
// ------------------------------------------------------------------
public static class UnboundColumn {
    public static void Add(GridView view) {
        GridColumn col = view.Columns.AddVisible("Total");
        col.UnboundDataType = typeof(decimal);
        col.UnboundExpression = "[Quantity] * [UnitPrice] * (1 - [Discount])";
        col.DisplayFormat.FormatType = FormatType.Numeric;
        col.DisplayFormat.FormatString = "c2";
        col.OptionsColumn.AllowEdit = false;
    }
}

// ------------------------------------------------------------------
// 5. Web-style multi-select with a check column
// ------------------------------------------------------------------
public static class MultiSelect {
    public static void Apply(GridView view) {
        view.OptionsSelection.MultiSelect = true;
        view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        view.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = true;
    }
}

// ------------------------------------------------------------------
// 6. TreeList unbound mode with checkboxes
// ------------------------------------------------------------------
public static class TreeUnbound {
    public static void Build(TreeList treeList) {
        treeList.Columns.AddVisible("Name");
        treeList.OptionsView.ShowCheckBoxes = true;
        TreeListNode root = treeList.AppendNode(new object[] { "Headquarters" }, null);
        treeList.AppendNode(new object[] { "Sales" },     root, CheckState.Unchecked, tag: null);
        treeList.AppendNode(new object[] { "Marketing" }, root, CheckState.Unchecked, tag: null);
        treeList.ExpandAll();
    }
}
