# Designer File Patterns

## InitializeComponent — Complete Skeleton

```csharp
partial class SalesReport {
    private void InitializeComponent() {
        // ── Step 1: Local inline objects ─────────────────────────────────────
        // Declare objects used directly in property assignments below.
        // XRSummary, SelectQuery, Column, QueryParameter, MasterDetailInfo,
        // RelationColumnInfo all go here as local variables (not fields).
        DevExpress.XtraReports.UI.XRSummary xrSummary1 =
            new DevExpress.XtraReports.UI.XRSummary();

        // ── Step 2: Instantiate all fields ───────────────────────────────────
        // All bands, controls, styles, data sources, parameters that are
        // declared as 'private' fields at the bottom of the file.
        this.TopMargin      = new DevExpress.XtraReports.UI.TopMarginBand();
        this.BottomMargin   = new DevExpress.XtraReports.UI.BottomMarginBand();
        this.Detail         = new DevExpress.XtraReports.UI.DetailBand();
        this.ReportHeader   = new DevExpress.XtraReports.UI.ReportHeaderBand();
        this.PageFooter     = new DevExpress.XtraReports.UI.PageFooterBand();
        this.xrTable1       = new DevExpress.XtraReports.UI.XRTable();
        this.xrRow1         = new DevExpress.XtraReports.UI.XRTableRow();
        this.xrCell1        = new DevExpress.XtraReports.UI.XRTableCell();
        this.xrLabel1       = new DevExpress.XtraReports.UI.XRLabel();
        this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
        this.EvenStyle      = new DevExpress.XtraReports.UI.XRControlStyle();
        this.OddStyle       = new DevExpress.XtraReports.UI.XRControlStyle();
        this.OrderIdParam   = new DevExpress.XtraReports.Parameters.Parameter();

        // ── Step 3: BeginInit — ALL at once before any property assignments ──
        // Every XRTable and the report itself must be wrapped.
        // All BeginInit calls happen here, not scattered around.
        ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

        // ── Step 4: Property assignments and AddRange calls ──────────────────
        this.xrCell1.ExpressionBindings.AddRange(
            new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding(
                    "BeforePrint", "Text", "[ProductName]") });
        this.xrCell1.Weight    = 1.5D;
        this.xrCell1.Multiline = true;
        this.xrCell1.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
        this.xrCell1.StylePriority.UseFont = false; // explicit font wins over style

        this.xrRow1.Cells.AddRange(
            new DevExpress.XtraReports.UI.XRTableCell[] { this.xrCell1 });
        this.xrRow1.OddStyleName = "OddStyle"; // per-row override if needed

        this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
        this.xrTable1.SizeF         = new System.Drawing.SizeF(650F, 25F);
        this.xrTable1.EvenStyleName = "EvenStyle";
        this.xrTable1.OddStyleName  = "OddStyle";
        this.xrTable1.Rows.AddRange(
            new DevExpress.XtraReports.UI.XRTableRow[] { this.xrRow1 });

        this.Detail.Controls.AddRange(
            new DevExpress.XtraReports.UI.XRControl[] { this.xrTable1 });
        this.Detail.HeightF       = 25F;
        this.Detail.KeepTogether  = true; // prevent row splitting across pages

        this.EvenStyle.Name       = "EvenStyle";
        this.EvenStyle.BackColor  = System.Drawing.Color.White;
        this.OddStyle.Name        = "OddStyle";
        this.OddStyle.BackColor   = System.Drawing.Color.FromArgb(234, 245, 255);

        this.OrderIdParam.Name  = "OrderIdParam";
        this.OrderIdParam.Type  = typeof(int);
        this.OrderIdParam.Value = 0;

        // ── Step 5: Report-level setup ───────────────────────────────────────
        this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin, this.BottomMargin, this.Detail,
            this.ReportHeader, this.PageFooter });
        this.DataSource  = this.sqlDataSource1;
        this.DataMember  = "Orders"; // required when DataSource has multiple queries
        this.StyleSheet.AddRange(
            new DevExpress.XtraReports.UI.XRControlStyle[] {
                this.EvenStyle, this.OddStyle });
        this.Parameters.AddRange(
            new DevExpress.XtraReports.Parameters.Parameter[] { this.OrderIdParam });
        // ComponentStorage is MANDATORY — registers data sources for designer serialization
        this.ComponentStorage.AddRange(
            new System.ComponentModel.IComponent[] { this.sqlDataSource1 });
        this.RequestParameters = false;
        this.Version           = "25.1";

        // ── Step 6: EndInit — same order as BeginInit ────────────────────────
        ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
    }

    // ── Field declarations ───────────────────────────────────────────────────
    private DevExpress.XtraReports.UI.TopMarginBand   TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.XtraReports.UI.DetailBand       Detail;
    private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
    private DevExpress.XtraReports.UI.PageFooterBand   PageFooter;
    private DevExpress.XtraReports.UI.XRTable          xrTable1;
    private DevExpress.XtraReports.UI.XRTableRow       xrRow1;
    private DevExpress.XtraReports.UI.XRTableCell      xrCell1;
    private DevExpress.XtraReports.UI.XRLabel          xrLabel1;
    private DevExpress.DataAccess.Sql.SqlDataSource     sqlDataSource1;
    private DevExpress.XtraReports.UI.XRControlStyle   EvenStyle;
    private DevExpress.XtraReports.UI.XRControlStyle   OddStyle;
    private DevExpress.XtraReports.Parameters.Parameter OrderIdParam;
}
```

## XRTableCell Sizing

```csharp
this.xrCell1.CanGrow    = false;                          // fixed height
this.xrCell1.TextFitMode = TextFitMode.ShrinkOnly;        // shrink to fit
this.xrCell1.RowSpan    = 2;                              // span 2 rows
this.xrCell1.Multiline  = true;                           // always set for wrapping text
// Default: CanGrow = true, cell height expands with content
```

## XRPictureBox with Embedded ImageResources

```csharp
// Embed named SVG/bitmap assets at the report level
this.ImageResources.AddRange(new DevExpress.XtraPrinting.Drawing.ImageItem[] {
    new DevExpress.XtraPrinting.Drawing.ImageItem(
        "DeliveredIcon",
        new DevExpress.XtraPrinting.Drawing.ImageSource(
            "svg", resources.GetString("$this.ImageResources"))),
    new DevExpress.XtraPrinting.Drawing.ImageItem(
        "PendingIcon",
        new DevExpress.XtraPrinting.Drawing.ImageSource(
            "svg", resources.GetString("$this.ImageResources1")))
});

// Reference by name in an expression
this.xrPictureBox1.ExpressionBindings.AddRange(
    new DevExpress.XtraReports.UI.ExpressionBinding[] {
        new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ImageSource",
            "Iif([ShipmentStatus]=2, [Images.DeliveredIcon], [Images.PendingIcon])")
    });
this.xrPictureBox1.ImageAlignment = DevExpress.XtraPrinting.ImageAlignment.MiddleCenter;
```

## What Belongs in the Main .cs File

```csharp
public partial class SalesReport : DevExpress.XtraReports.UI.XtraReport {
    public SalesReport() {
        InitializeComponent();
    }

    // Runtime data source wiring
    public void SetDataSource(IEnumerable<SalesRecord> data) {
        this.DataSource = data;
    }

    // Event handlers
    private void Detail_BeforePrint(object sender, CancelEventArgs e) {
        // conditional logic based on data values
    }
}
```

Never add controls, set `LocationFloat`, or call `Controls.Add()` here.
