// DevExpress WinForms Layout Management — Quickstart (C#)
// Demonstrates: LayoutControl groups/items, DockManager panels, layout persistence.
// Package: DevExpress.Win.Navigation   Host form: XtraForm

using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraBars.Docking;

// ------------------------------------------------------------------
// 1. Designer-backed LayoutControl with two groups (RECOMMENDED layout)
//    The LayoutControl, its groups, the hosted editors, and the layout
//    items are declared in the *.Designer.cs partial below
//    (InitializeComponent), so the form stays editable in the Visual
//    Studio WinForms designer. Build a layout in runtime code (new
//    LayoutControl + BeginUpdate + Root.AddGroup/AddItem) only when it is
//    genuinely dynamic or data-driven.
// ------------------------------------------------------------------

// --- MainForm.cs — data + behavior only ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                 // builds the LayoutControl, groups, and hosted editors
    }
}

// --- MainForm.Designer.cs — structure the WinForms designer round-trips ---
partial class MainForm {
    private LayoutControl layoutControl1;
    private LayoutControlGroup rootGroup;
    private LayoutControlGroup lcgPersonal;
    private LayoutControlGroup lcgContact;
    private TextEdit edFirst;
    private TextEdit edLast;
    private TextEdit edEmail;
    private TextEdit edPhone;
    private LayoutControlItem lciFirst;
    private LayoutControlItem lciLast;
    private LayoutControlItem lciEmail;
    private LayoutControlItem lciPhone;

    private void InitializeComponent() {
        this.layoutControl1 = new LayoutControl();
        this.rootGroup = new LayoutControlGroup();
        this.lcgPersonal = new LayoutControlGroup();
        this.lcgContact = new LayoutControlGroup();
        this.edFirst = new TextEdit();
        this.edLast = new TextEdit();
        this.edEmail = new TextEdit();
        this.edPhone = new TextEdit();
        this.lciFirst = new LayoutControlItem();
        this.lciLast = new LayoutControlItem();
        this.lciEmail = new LayoutControlItem();
        this.lciPhone = new LayoutControlItem();
        ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
        this.layoutControl1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.rootGroup)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lcgPersonal)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lcgContact)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edFirst.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edLast.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edEmail.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.edPhone.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciFirst)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciLast)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciEmail)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciPhone)).BeginInit();
        //
        // layoutControl1
        //
        this.layoutControl1.Controls.Add(this.edFirst);
        this.layoutControl1.Controls.Add(this.edLast);
        this.layoutControl1.Controls.Add(this.edEmail);
        this.layoutControl1.Controls.Add(this.edPhone);
        this.layoutControl1.Dock = DockStyle.Fill;
        this.layoutControl1.Name = "layoutControl1";
        this.layoutControl1.Root = this.rootGroup;
        //
        // hosted editors (no Dock/Anchor — the LayoutControlItem positions them)
        //
        this.edFirst.Name = "edFirst";
        this.edLast.Name = "edLast";
        this.edEmail.Name = "edEmail";
        this.edPhone.Name = "edPhone";
        //
        // rootGroup — holds the two sub-groups
        //
        this.rootGroup.Name = "Root";
        this.rootGroup.Items.AddRange(new BaseLayoutItem[] { this.lcgPersonal, this.lcgContact });
        //
        // lcgPersonal / lcgContact
        //
        this.lcgPersonal.Name = "lcgPersonal";
        this.lcgPersonal.Text = "Personal Info";
        this.lcgPersonal.Items.AddRange(new BaseLayoutItem[] { this.lciFirst, this.lciLast });
        this.lcgContact.Name = "lcgContact";
        this.lcgContact.Text = "Contact";
        this.lcgContact.Items.AddRange(new BaseLayoutItem[] { this.lciEmail, this.lciPhone });
        //
        // layout items — each hosts one editor
        //
        this.lciFirst.Control = this.edFirst; this.lciFirst.Name = "lciFirst"; this.lciFirst.Text = "First Name";
        this.lciLast.Control  = this.edLast;  this.lciLast.Name  = "lciLast";  this.lciLast.Text  = "Last Name";
        this.lciEmail.Control = this.edEmail; this.lciEmail.Name = "lciEmail"; this.lciEmail.Text = "Email";
        this.lciPhone.Control = this.edPhone; this.lciPhone.Name = "lciPhone"; this.lciPhone.Text = "Phone";
        //
        // MainForm
        //
        this.Controls.Add(this.layoutControl1);
        this.Name = "MainForm";
        this.Text = "Edit";
        ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
        this.layoutControl1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.rootGroup)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lcgPersonal)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lcgContact)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edFirst.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edLast.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edEmail.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.edPhone.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciFirst)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciLast)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciEmail)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lciPhone)).EndInit();
    }
}

// ------------------------------------------------------------------
// 2. DockManager with three panels
// ------------------------------------------------------------------
public partial class DockForm : XtraForm {
    void BuildDock() {
        var dm = new DockManager { Form = this };

        var left  = dm.AddPanel(DockingStyle.Left);
        left.Text = "Explorer"; left.Width = 220; left.Name = "pnlExplorer";

        var right  = dm.AddPanel(DockingStyle.Right);
        right.Text = "Properties"; right.Width = 240; right.Name = "pnlProperties";

        var bottom = dm.AddPanel(DockingStyle.Bottom);
        bottom.Text = "Output"; bottom.Height = 120; bottom.Name = "pnlOutput";

        left.Controls.Add(new TreeView { Dock = DockStyle.Fill });
    }
}

// ------------------------------------------------------------------
// 3. Persist the DockManager layout
// ------------------------------------------------------------------
public partial class PersistForm : XtraForm {
    DockManager dockManager1;

    void Form_Load(object sender, EventArgs e) {
        if (File.Exists("dock.xml")) dockManager1.RestoreLayoutFromXml("dock.xml");
    }

    void Form_FormClosing(object sender, FormClosingEventArgs e) {
        dockManager1.SaveLayoutToXml("dock.xml");
    }
}
