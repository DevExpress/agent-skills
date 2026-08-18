// DevExpress WinForms Pivot Grid (PivotGridControl) — Quickstart (C#)
// Demonstrates: field areas, data binding, date hierarchy grouping, currency format.
// Package: DevExpress.Win.PivotGrid   Host form: XtraForm

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

// ------------------------------------------------------------------
// 1. Minimal pivot — fixed fields in the designer file, bind data at runtime
//    The control and its four fields (fixed columns) are declared in the
//    *.Designer.cs partial below (InitializeComponent), so the form stays
//    editable in the Visual Studio WinForms designer. Only the data source
//    is assigned in the code-behind. For fields discovered from the data at
//    run time, use the AddDataSourceColumn helper (see Section 2).
// ------------------------------------------------------------------

// --- Form1.cs — data only ---
public partial class Form1 : XtraForm {
    public Form1() {
        InitializeComponent();                 // builds pivotGridControl1 + its fields/areas
        pivotGridControl1.DataSource = GetSalesData();   // IList / DataTable / BindingSource
        pivotGridControl1.BestFit();
    }

    List<SalesRecord> GetSalesData() => new() {
        new SalesRecord { Country = "USA", Category = "Bikes", Year = 2025, Sales = 12000m },
        new SalesRecord { Country = "USA", Category = "Bikes", Year = 2026, Sales = 15000m },
        new SalesRecord { Country = "DE",  Category = "Parts", Year = 2026, Sales = 8000m  },
    };
}

// --- Form1.Designer.cs — control + fixed fields the WinForms designer round-trips ---
partial class Form1 {
    private System.ComponentModel.IContainer components = null;
    private PivotGridControl pivotGridControl1;
    private PivotGridField fieldCountry;
    private PivotGridField fieldCategory;
    private PivotGridField fieldYear;
    private PivotGridField fieldSales;

    private void InitializeComponent() {
        this.pivotGridControl1 = new PivotGridControl();
        this.fieldCountry = new PivotGridField();
        this.fieldCategory = new PivotGridField();
        this.fieldYear = new PivotGridField();
        this.fieldSales = new PivotGridField();
        ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).BeginInit();
        this.SuspendLayout();
        //
        // pivotGridControl1
        //
        this.pivotGridControl1.Dock = DockStyle.Fill;
        this.pivotGridControl1.Name = "pivotGridControl1";
        this.pivotGridControl1.OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized;
        this.pivotGridControl1.Fields.AddRange(new PivotGridField[] {
            this.fieldCountry, this.fieldCategory, this.fieldYear, this.fieldSales});
        //
        // fields — FieldName binds to a data-source column; Area places it
        //
        this.fieldCountry.Area = PivotArea.FilterArea;
        this.fieldCountry.Caption = "Country";
        this.fieldCountry.FieldName = "Country";
        this.fieldCountry.Name = "fieldCountry";
        this.fieldCategory.Area = PivotArea.RowArea;
        this.fieldCategory.Caption = "Category";
        this.fieldCategory.FieldName = "Category";
        this.fieldCategory.Name = "fieldCategory";
        this.fieldYear.Area = PivotArea.ColumnArea;
        this.fieldYear.Caption = "Year";
        this.fieldYear.FieldName = "Year";
        this.fieldYear.Name = "fieldYear";
        this.fieldSales.Area = PivotArea.DataArea;
        this.fieldSales.Caption = "Sales";
        this.fieldSales.FieldName = "Sales";
        this.fieldSales.Name = "fieldSales";
        //
        // Form1
        //
        this.Controls.Add(this.pivotGridControl1);
        this.Name = "Form1";
        this.Text = "Sales";
        ((System.ComponentModel.ISupportInitialize)(this.pivotGridControl1)).EndInit();
        this.ResumeLayout(false);
    }
}

public class SalesRecord {
    public string Country { get; set; } = "";
    public string Category { get; set; } = "";
    public int Year { get; set; }
    public decimal Sales { get; set; }
}

// ------------------------------------------------------------------
// 2. Date hierarchy (Year → Quarter) in the column area
// ------------------------------------------------------------------
public static class DateHierarchy {
    public static void Build(PivotGridControl pivot) {
        var fY = pivot.Fields.AddDataSourceColumn("OrderDate", PivotArea.ColumnArea);
        fY.Caption = "Year";
        ((DataSourceColumnBinding)fY.DataBinding).GroupInterval = PivotGroupInterval.DateYear;
        fY.AreaIndex = 0;

        var fQ = pivot.Fields.AddDataSourceColumn("OrderDate", PivotArea.ColumnArea);
        fQ.Caption = "Quarter";
        ((DataSourceColumnBinding)fQ.DataBinding).GroupInterval = PivotGroupInterval.DateQuarter;
        fQ.AreaIndex = 1;

        // Group them so they move together
        var g = new PivotGridGroup();
        g.AddRange(new[] { fY, fQ });
        pivot.Groups.Add(g);
    }
}

// ------------------------------------------------------------------
// 3. Currency format on a data field
// ------------------------------------------------------------------
public static class FieldFormat {
    public static void Currency(PivotGridField fieldSales) {
        fieldSales.CellFormat.FormatType   = DevExpress.Utils.FormatType.Numeric;
        fieldSales.CellFormat.FormatString = "c2";
    }
}
