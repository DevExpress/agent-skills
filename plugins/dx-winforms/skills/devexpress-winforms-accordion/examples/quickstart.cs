// DevExpress WinForms AccordionControl — Quickstart (C#)
// Demonstrates: designer-backed side navigation, NavigationFrame, Hamburger Menu,
//               data-driven items, embedded control, search panel.
// Package: DevExpress.Win.Navigation   Host form: XtraForm
//
// Sections 1-2 show the RECOMMENDED split for a designer-backed form: the control,
// its groups/items, and the NavigationFrame live in *.Designer.cs (InitializeComponent),
// so the form stays editable in the Visual Studio WinForms designer; only events live
// in the code-behind. Sections 3-5 use the runtime API — appropriate for hamburger
// configuration, data-driven structure, and code-only content containers.
//
// ISupportInitialize (wrap in BeginInit/EndInit) — AccordionControl and NavigationFrame YES;
// AccordionControlElement, AccordionContentContainer, and NavigationPage NO (a NavigationPage
// is a panel: use SuspendLayout/ResumeLayout). Wrapping a page throws InvalidCastException.

using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

// ------------------------------------------------------------------
// 1. Minimal side navigation — groups and items (designer-backed)
// ------------------------------------------------------------------

// --- MainForm.cs — behavior only ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                 // builds the accordion, groups, and items
        accordion.ElementClick += (s, e) => {
            if (e.Element.Style == ElementStyle.Item)
                NavigateTo(e.Element.Tag as string);
        };
    }

    void NavigateTo(string route) { /* update content area */ }
}

// --- MainForm.Designer.cs — structure the WinForms designer round-trips ---
partial class MainForm {
    private AccordionControl accordion;
    private AccordionControlElement grpMain;
    private AccordionControlElement itmDashboard;
    private AccordionControlElement itmReports;
    private AccordionControlElement grpSettings;
    private AccordionControlElement itmProfile;

    private void InitializeComponent() {
        this.accordion    = new AccordionControl();
        this.grpMain      = new AccordionControlElement(ElementStyle.Group);
        this.itmDashboard = new AccordionControlElement(ElementStyle.Item);
        this.itmReports   = new AccordionControlElement(ElementStyle.Item);
        this.grpSettings  = new AccordionControlElement(ElementStyle.Group);
        this.itmProfile   = new AccordionControlElement(ElementStyle.Item);
        // Only AccordionControl implements ISupportInitialize; the elements do not.
        ((System.ComponentModel.ISupportInitialize)(this.accordion)).BeginInit();
        this.SuspendLayout();
        //
        // accordion
        //
        this.accordion.Dock = DockStyle.Left;
        this.accordion.Width = 220;
        this.accordion.Name = "accordion";
        this.accordion.ShowFilterControl = ShowFilterControl.Auto;   // Ctrl+F search
        this.accordion.AllowItemSelection = true;
        this.accordion.Elements.AddRange(new AccordionControlElement[] { this.grpMain, this.grpSettings });
        //
        // grpMain
        //
        this.grpMain.Text = "Main";
        this.grpMain.Expanded = true;
        this.grpMain.Elements.AddRange(new AccordionControlElement[] { this.itmDashboard, this.itmReports });
        //
        // items
        //
        this.itmDashboard.Text = "Dashboard"; this.itmDashboard.Tag = "dashboard";
        this.itmReports.Text = "Reports";     this.itmReports.Tag = "reports";
        this.grpSettings.Text = "Settings";
        this.grpSettings.Elements.Add(this.itmProfile);
        this.itmProfile.Text = "Profile"; this.itmProfile.Tag = "profile";
        //
        // MainForm
        //
        this.Controls.Add(this.accordion);
        this.Name = "MainForm";
        this.Text = "Side Navigation";
        ((System.ComponentModel.ISupportInitialize)(this.accordion)).EndInit();
        this.ResumeLayout(false);
    }
}

// ------------------------------------------------------------------
// 2. Side navigation driving a NavigationFrame (designer-backed)
// ------------------------------------------------------------------

// --- NavFrameForm.cs — behavior only ---
public partial class NavFrameForm : XtraForm {
    public NavFrameForm() {
        InitializeComponent();
        accordion.ElementClick += (s, e) => {
            if (e.Element.Tag is NavigationPage page)
                navFrame.SelectedPage = page;
        };
    }
}

// --- NavFrameForm.Designer.cs ---
partial class NavFrameForm {
    private NavigationFrame navFrame;
    private NavigationPage page1;
    private AccordionControl accordion;
    private AccordionControlElement item1;

    private void InitializeComponent() {
        this.navFrame  = new NavigationFrame();
        this.page1     = new NavigationPage();
        this.accordion = new AccordionControl();
        this.item1     = new AccordionControlElement(ElementStyle.Item);
        // AccordionControl AND NavigationFrame are ISupportInitialize; NavigationPage is NOT
        // (it is a panel — SuspendLayout/ResumeLayout, never BeginInit/EndInit).
        ((System.ComponentModel.ISupportInitialize)(this.navFrame)).BeginInit();
        this.navFrame.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.accordion)).BeginInit();
        this.page1.SuspendLayout();
        this.SuspendLayout();
        //
        // navFrame
        //
        this.navFrame.Dock = DockStyle.Fill;
        this.navFrame.Name = "navFrame";
        this.navFrame.Pages.Add(this.page1);
        this.navFrame.SelectedPage = this.page1;
        //
        // page1
        //
        this.page1.Caption = "Dashboard";
        this.page1.Name = "page1";
        //
        // accordion
        //
        this.accordion.Dock = DockStyle.Left;
        this.accordion.Name = "accordion";
        this.accordion.Elements.Add(this.item1);
        //
        // item1
        //
        this.item1.Text = "Dashboard";
        this.item1.Tag = this.page1;    // the element's Tag references the page to navigate to
        //
        // NavFrameForm
        //
        this.Controls.Add(this.navFrame);
        this.Controls.Add(this.accordion);
        this.accordion.SendToBack();
        this.Name = "NavFrameForm";
        this.Text = "Navigation Frame";
        ((System.ComponentModel.ISupportInitialize)(this.accordion)).EndInit();
        this.page1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.navFrame)).EndInit();
        this.navFrame.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}

// ------------------------------------------------------------------
// 3. Hamburger Menu (overlay display mode) — runtime configuration
// ------------------------------------------------------------------
public static class HamburgerSetup {
    public static void Apply(AccordionControl accordion) {
        accordion.ViewType = AccordionControlViewType.HamburgerMenu;
        accordion.OptionsHamburgerMenu.DisplayMode = AccordionControlDisplayMode.Overlay;
    }
}

// ------------------------------------------------------------------
// 4. Data-driven items — runtime (structure is not known at compile time)
// ------------------------------------------------------------------
public static class DataDriven {
    public static void Populate(AccordionControl accordion, System.Collections.Generic.IEnumerable<Section> sections) {
        accordion.BeginUpdate();
        accordion.Elements.Clear();
        foreach (var section in sections) {
            var grp = new AccordionControlElement(ElementStyle.Group) { Text = section.Name, Expanded = true };
            foreach (var item in section.Items)
                grp.Elements.Add(new AccordionControlElement(ElementStyle.Item) { Text = item.Label, Tag = item.Id });
            accordion.Elements.Add(grp);
        }
        accordion.EndUpdate();
    }
}

public class Section {
    public string Name { get; set; } = "";
    public System.Collections.Generic.List<(string Label, string Id)> Items { get; set; } = new();
}

// ------------------------------------------------------------------
// 5. Item with an embedded control (ContentContainer) — runtime
// ------------------------------------------------------------------
public static class EmbeddedControl {
    public static void Add(AccordionControl accordion, AccordionControlElement settingsItem) {
        var container = new AccordionContentContainer { Padding = new Padding(-1) };  // skin-aware padding
        container.Controls.Add(new ToggleSwitch {
            Dock = DockStyle.Fill,
            Properties = { OnText = "Enabled", OffText = "Disabled" }
        });
        accordion.Controls.Add(container);          // required in addition to assigning ContentContainer
        settingsItem.ContentContainer = container;
    }
}
