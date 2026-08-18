// DevExpress WinForms Ribbon and Bars — Quickstart (C#)
// Demonstrates: Ribbon UI (pages/groups, item+links, QAT, status bar) and the
//               classic BarManager menu/toolbar stack.
// Package: DevExpress.Win.Navigation   Host form: RibbonForm (for the Ribbon)

using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

// ------------------------------------------------------------------
// 1. Designer-backed Ribbon UI (RECOMMENDED layout)
//    The ribbon, its page/group, the item + links, and the status bar are
//    declared in the *.Designer.cs partial below (InitializeComponent), so
//    the form stays editable in the Visual Studio WinForms designer. Only the
//    ItemClick handler lives in the code-behind. Section 2 (classic Bars) shows
//    the equivalent BarManager stack.
// ------------------------------------------------------------------

// --- MainForm.cs — only behavior/events ---
public partial class MainForm : RibbonForm {
    public MainForm() {
        InitializeComponent();                 // builds ribbon + page/group + item + QAT + status bar
        itemNew.ItemClick += (s, e) => MessageBox.Show("New!");
    }
}

// --- MainForm.Designer.cs — structure the WinForms designer round-trips ---
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.Utils.SvgImageCollection svgImageCollection1;
    private RibbonControl ribbon;
    private RibbonPage pageHome;
    private RibbonPageGroup groupFile;
    private BarButtonItem itemNew;
    private RibbonStatusBar ribbonStatusBar1;
    private BarStaticItem statusLabel;

    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        this.svgImageCollection1 = new DevExpress.Utils.SvgImageCollection(this.components);
        this.ribbon = new RibbonControl();
        this.pageHome = new RibbonPage();
        this.groupFile = new RibbonPageGroup();
        this.itemNew = new BarButtonItem();
        this.ribbonStatusBar1 = new RibbonStatusBar();
        this.statusLabel = new BarStaticItem();
        ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
        this.SuspendLayout();
        //
        // svgImageCollection1 — populate via the SVG Image Collection editor (serialized to .resx)
        //
        // this.svgImageCollection1.Add("new", "image://svgimages/actions/new.svg");
        //
        // ribbon
        //
        this.ribbon.Images = this.svgImageCollection1;
        this.ribbon.Items.AddRange(new BarItem[] { this.itemNew, this.statusLabel });
        this.ribbon.MaxItemId = 2;
        this.ribbon.Name = "ribbon";
        this.ribbon.Pages.AddRange(new RibbonPage[] { this.pageHome });
        this.ribbon.Toolbar.ItemLinks.Add(this.itemNew);       // also in the Quick Access Toolbar
        //
        // itemNew — create the item once; link it wherever it should appear
        //
        this.itemNew.Caption = "New";
        this.itemNew.Id = 1;                                    // unique id — required for layout (de)serialization
        this.itemNew.ImageOptions.SvgImage = this.svgImageCollection1["new"];
        this.itemNew.Name = "itemNew";
        this.itemNew.RibbonStyle = RibbonItemStyles.Large;
        //
        // pageHome / groupFile
        //
        this.pageHome.Groups.AddRange(new RibbonPageGroup[] { this.groupFile });
        this.pageHome.Name = "pageHome";
        this.pageHome.Text = "Home";
        this.groupFile.ItemLinks.Add(this.itemNew);
        this.groupFile.Name = "groupFile";
        this.groupFile.Text = "File";
        //
        // ribbonStatusBar1 / statusLabel
        //
        this.ribbonStatusBar1.Name = "ribbonStatusBar1";
        this.ribbonStatusBar1.Ribbon = this.ribbon;
        this.ribbonStatusBar1.ItemLinks.Add(this.statusLabel);
        this.statusLabel.Alignment = BarItemLinkAlignment.Right;
        this.statusLabel.Caption = "Ready";
        this.statusLabel.Id = 2;
        this.statusLabel.Name = "statusLabel";
        //
        // MainForm
        //
        this.Controls.Add(this.ribbonStatusBar1);
        this.Controls.Add(this.ribbon);
        this.Name = "MainForm";
        this.Ribbon = this.ribbon;              // link the ribbon to the RibbonForm
        this.Text = "My Application";
        ((System.ComponentModel.ISupportInitialize)(this.svgImageCollection1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}

// ------------------------------------------------------------------
// 2. Classic Bars UI — BarManager with a menu bar and shortcuts
// ------------------------------------------------------------------
public partial class ClassicForm : DevExpress.XtraEditors.XtraForm {
    void BuildBars() {
        var bars = new BarManager { Form = this };
        bars.BeginUpdate();

        var menu  = new Bar(bars, "Main")  { DockStyle = BarDockStyle.Top, DockRow = 0 };
        var tools = new Bar(bars, "Tools") { DockStyle = BarDockStyle.Top, DockRow = 1 };
        var status = new Bar(bars, "Status") {
            DockStyle = BarDockStyle.Bottom,
            OptionsBar = { AllowQuickCustomization = false, DrawDragBorder = false, UseWholeRow = true }
        };
        bars.MainMenu = menu;
        bars.StatusBar = status;

        var fileMenu = new BarSubItem(bars, "&File");
        var newCmd   = new BarButtonItem(bars, "&New") { ItemShortcut = new BarShortcut(Keys.Control | Keys.N) };
        newCmd.ItemClick += (_, _) => CreateNewDocument();
        fileMenu.AddItem(newCmd);
        menu.AddItem(fileMenu);

        bars.EndUpdate();
    }

    void CreateNewDocument() { }
}
