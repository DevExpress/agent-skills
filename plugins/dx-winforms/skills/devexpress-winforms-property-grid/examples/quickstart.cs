// DevExpress WinForms Property Grid (PropertyGridControl) — Quickstart (C#)
// Demonstrates: SelectedObject, attributes/categories, nested expandable props,
//               DX collection editor, expanding a row, Office view with tabs.
// Package: DevExpress.Win.Navigation   Host form: XtraForm

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid;

// ------------------------------------------------------------------
// 1. Show an object's properties (designer-backed)
//    The control is declared in the *.Designer.cs partial below
//    (InitializeComponent), so the form stays editable in the Visual
//    Studio WinForms designer. Only SelectedObject — the runtime data —
//    is assigned in the code-behind; rows auto-generate from its public
//    properties.
// ------------------------------------------------------------------

// --- SettingsForm.cs — data only ---
public partial class SettingsForm : XtraForm {
    public SettingsForm() {
        InitializeComponent();                 // builds propertyGridControl1
        propertyGridControl1.SelectedObject = new AppSettings();
    }
}

// --- SettingsForm.Designer.cs — the control the WinForms designer round-trips ---
partial class SettingsForm {
    private System.ComponentModel.IContainer components = null;
    private PropertyGridControl propertyGridControl1;

    private void InitializeComponent() {
        this.propertyGridControl1 = new PropertyGridControl();
        ((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).BeginInit();
        this.SuspendLayout();
        //
        // propertyGridControl1
        //
        this.propertyGridControl1.Dock = DockStyle.Fill;
        this.propertyGridControl1.Name = "propertyGridControl1";
        //
        // SettingsForm
        //
        this.Controls.Add(this.propertyGridControl1);
        this.Name = "SettingsForm";
        this.Text = "Settings";
        ((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).EndInit();
        this.ResumeLayout(false);
    }
}

public class AppSettings {
    [Category("General")]
    [DisplayName("Application Title")]
    [Description("Shown in the title bar.")]
    public string Title { get; set; } = "My App";

    [Category("General")]
    public bool StartMinimized { get; set; }

    [Category("Data")]
    [DisplayName("Database")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public DatabaseSettings Database { get; set; } = new DatabaseSettings();

    [Category("Data")]
    [DisplayName("Allowed Paths")]
    public List<string> AllowedPaths { get; set; } = new();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class DatabaseSettings {
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public override string ToString() => $"{Host}:{Port}";
}

// ------------------------------------------------------------------
// 2. Use the DevExpress collection editor; expand a row on load
// ------------------------------------------------------------------
public static class GridSetup {
    public static void Apply(PropertyGridControl grid) {
        grid.OptionsCollectionEditor.UseDXCollectionEditor = true;

        grid.DataSourceChanged += (s, e) =>
            grid.GetRowByFieldName("Database").Expanded = true;
    }
}

// ------------------------------------------------------------------
// 3. Office view with category tabs
// ------------------------------------------------------------------
public static class OfficeView {
    public static void Apply(PropertyGridControl grid) {
        grid.ActiveViewType = PropertyGridView.Office;
        grid.TabPanelCustomize += (s, e) => {
            var tab1 = new Tab { Caption = "General" };
            tab1.CategoryNames.Add("General");

            var tab2 = new Tab { Caption = "Data" };
            tab2.CategoryNames.Add("Data");

            e.Tabs.Add(tab1);
            e.Tabs.Add(tab2);
        };
    }
}
