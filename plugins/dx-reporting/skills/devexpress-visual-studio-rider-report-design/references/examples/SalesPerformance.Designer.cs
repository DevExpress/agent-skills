namespace DxWinFormsReportingApp.Reports
{
    partial class SalesPerformance
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            // ── Step 2: instantiate all fields ────────────────────────────────────
            this.TopMargin                  = new DevExpress.XtraReports.UI.TopMarginBand();
            this.ReportHeader               = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLogo                     = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrLabelTitle               = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelPeriodCaption       = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelPeriod              = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanelKpi1                = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabelKpi1Caption         = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelKpi1Value           = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanelKpi2                = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabelKpi2Caption         = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelKpi2Value           = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanelKpi3                = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabelKpi3Caption         = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelKpi3Value           = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanelKpi4                = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabelKpi4Caption         = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelKpi4Value           = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader1               = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabelSectionRep          = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableRepHeader           = new DevExpress.XtraReports.UI.XRTable();
            this.xrRowRepHeader             = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrCellRepHdrName           = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepHdrOrders         = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepHdrRevenue        = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepHdrPercent        = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepHdrTarget         = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepHdrVsTarget       = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail                     = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTableRepDetail           = new DevExpress.XtraReports.UI.XRTable();
            this.xrRowRepDetail             = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrCellRepName              = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepOrders            = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepRevenue           = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepPercent           = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTarget            = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepVsTarget          = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupFooter1               = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrTableRepTotal            = new DevExpress.XtraReports.UI.XRTable();
            this.xrRowRepTotal              = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrCellRepTotalLabel        = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTotalOrders       = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTotalRevenue      = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTotalPercent      = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTotalTarget       = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCellRepTotalVsTarget     = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader2               = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabelSectionCategory     = new DevExpress.XtraReports.UI.XRLabel();
            this.xrChartCategory            = new DevExpress.XtraReports.UI.XRChart();
            this.GroupFooter2               = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.ReportFooter               = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabelSectionTrend        = new DevExpress.XtraReports.UI.XRLabel();
            this.xrChartTrend               = new DevExpress.XtraReports.UI.XRChart();
            this.xrLabelTopPerformers       = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter                 = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrPageInfo                 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrPageInfoGenerated        = new DevExpress.XtraReports.UI.XRPageInfo();
            this.BottomMargin               = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.HeaderStyle                = new DevExpress.XtraReports.UI.XRControlStyle();
            this.SectionStyle               = new DevExpress.XtraReports.UI.XRControlStyle();
            this.EvenStyle                  = new DevExpress.XtraReports.UI.XRControlStyle();
            this.OddStyle                   = new DevExpress.XtraReports.UI.XRControlStyle();
            this.TotalStyle                 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.KpiStyle                   = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PeriodStart                = new DevExpress.XtraReports.Parameters.Parameter();
            this.PeriodEnd                  = new DevExpress.XtraReports.Parameters.Parameter();

            // ── Step 3: BeginInit ─────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChartCategory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChartTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // ── Step 4: configure controls ────────────────────────────────────────

            this.TopMargin.HeightF = 50F;
            this.TopMargin.Name = "TopMargin";

            // ── ReportHeaderBand (145 pt — bottom of KPI panels at y=75+60=135, +10 padding) ──
            this.ReportHeader.HeightF = 145F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLogo, this.xrLabelTitle,
                this.xrLabelPeriodCaption, this.xrLabelPeriod,
                this.xrPanelKpi1, this.xrPanelKpi2,
                this.xrPanelKpi3, this.xrPanelKpi4 });

            this.xrLogo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F);
            this.xrLogo.Name = "xrLogo";
            this.xrLogo.SizeF = new System.Drawing.SizeF(120F, 45F);
            this.xrLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;

            this.xrLabelTitle.Font = new DevExpress.Drawing.DXFont("Arial", 18F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelTitle.LocationFloat = new DevExpress.Utils.PointFloat(125F, 8F);
            this.xrLabelTitle.Name = "xrLabelTitle";
            this.xrLabelTitle.SizeF = new System.Drawing.SizeF(460F, 30F);
            this.xrLabelTitle.StylePriority.UseFont = false;
            this.xrLabelTitle.StylePriority.UseTextAlignment = false;
            this.xrLabelTitle.Text = "SALES PERFORMANCE REPORT";
            this.xrLabelTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // Period label
            this.xrLabelPeriodCaption.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelPeriodCaption.LocationFloat = new DevExpress.Utils.PointFloat(125F, 44F);
            this.xrLabelPeriodCaption.Name = "xrLabelPeriodCaption";
            this.xrLabelPeriodCaption.SizeF = new System.Drawing.SizeF(90F, 16F);
            this.xrLabelPeriodCaption.StylePriority.UseFont = false;
            this.xrLabelPeriodCaption.Text = "Report Period:";

            this.xrLabelPeriod.LocationFloat = new DevExpress.Utils.PointFloat(217F, 44F);
            this.xrLabelPeriod.Name = "xrLabelPeriod";
            this.xrLabelPeriod.SizeF = new System.Drawing.SizeF(300F, 16F);
            this.xrLabelPeriod.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "FormatString('{0:d} \u2013 {1:d}', ?PeriodStart, ?PeriodEnd)") });

            // ── KPI panels (side by side, y=75, each 180 pt wide) ─────────────────
            // KPI 1 — Total Revenue
            this.xrPanelKpi1.BackColor = System.Drawing.Color.FromArgb(230, 243, 255);
            this.xrPanelKpi1.BorderColor = System.Drawing.Color.FromArgb(180, 210, 240);
            this.xrPanelKpi1.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.xrPanelKpi1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 75F);
            this.xrPanelKpi1.Name = "xrPanelKpi1";
            this.xrPanelKpi1.SizeF = new System.Drawing.SizeF(180F, 60F);
            this.xrPanelKpi1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelKpi1Caption, this.xrLabelKpi1Value });

            this.xrLabelKpi1Caption.Font = new DevExpress.Drawing.DXFont("Arial", 7.5F);
            this.xrLabelKpi1Caption.LocationFloat = new DevExpress.Utils.PointFloat(4F, 4F);
            this.xrLabelKpi1Caption.Name = "xrLabelKpi1Caption";
            this.xrLabelKpi1Caption.SizeF = new System.Drawing.SizeF(172F, 14F);
            this.xrLabelKpi1Caption.StylePriority.UseFont = false;
            this.xrLabelKpi1Caption.Text = "TOTAL REVENUE";

            this.xrLabelKpi1Value.Font = new DevExpress.Drawing.DXFont("Arial", 20F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelKpi1Value.LocationFloat = new DevExpress.Utils.PointFloat(4F, 22F);
            this.xrLabelKpi1Value.Name = "xrLabelKpi1Value";
            this.xrLabelKpi1Value.SizeF = new System.Drawing.SizeF(172F, 34F);
            this.xrLabelKpi1Value.StyleName = "KpiStyle";
            this.xrLabelKpi1Value.StylePriority.UseFont = false;
            this.xrLabelKpi1Value.TextFormatString = "{0:c0}";
            this.xrLabelKpi1Value.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "Sum([OrderTotal])") });

            // KPI 2 — Number of Orders
            this.xrPanelKpi2.BackColor = System.Drawing.Color.FromArgb(230, 255, 235);
            this.xrPanelKpi2.BorderColor = System.Drawing.Color.FromArgb(180, 230, 190);
            this.xrPanelKpi2.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.xrPanelKpi2.LocationFloat = new DevExpress.Utils.PointFloat(188F, 75F);
            this.xrPanelKpi2.Name = "xrPanelKpi2";
            this.xrPanelKpi2.SizeF = new System.Drawing.SizeF(180F, 60F);
            this.xrPanelKpi2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelKpi2Caption, this.xrLabelKpi2Value });

            this.xrLabelKpi2Caption.Font = new DevExpress.Drawing.DXFont("Arial", 7.5F);
            this.xrLabelKpi2Caption.LocationFloat = new DevExpress.Utils.PointFloat(4F, 4F);
            this.xrLabelKpi2Caption.Name = "xrLabelKpi2Caption";
            this.xrLabelKpi2Caption.SizeF = new System.Drawing.SizeF(172F, 14F);
            this.xrLabelKpi2Caption.StylePriority.UseFont = false;
            this.xrLabelKpi2Caption.Text = "NUMBER OF ORDERS";

            this.xrLabelKpi2Value.Font = new DevExpress.Drawing.DXFont("Arial", 20F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelKpi2Value.LocationFloat = new DevExpress.Utils.PointFloat(4F, 22F);
            this.xrLabelKpi2Value.Name = "xrLabelKpi2Value";
            this.xrLabelKpi2Value.SizeF = new System.Drawing.SizeF(172F, 34F);
            this.xrLabelKpi2Value.StyleName = "KpiStyle";
            this.xrLabelKpi2Value.StylePriority.UseFont = false;
            this.xrLabelKpi2Value.TextFormatString = "{0:n0}";
            this.xrLabelKpi2Value.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "Count([OrderID])") });

            // KPI 3 — Avg Order Value
            this.xrPanelKpi3.BackColor = System.Drawing.Color.FromArgb(255, 250, 225);
            this.xrPanelKpi3.BorderColor = System.Drawing.Color.FromArgb(230, 210, 160);
            this.xrPanelKpi3.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.xrPanelKpi3.LocationFloat = new DevExpress.Utils.PointFloat(376F, 75F);
            this.xrPanelKpi3.Name = "xrPanelKpi3";
            this.xrPanelKpi3.SizeF = new System.Drawing.SizeF(180F, 60F);
            this.xrPanelKpi3.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelKpi3Caption, this.xrLabelKpi3Value });

            this.xrLabelKpi3Caption.Font = new DevExpress.Drawing.DXFont("Arial", 7.5F);
            this.xrLabelKpi3Caption.LocationFloat = new DevExpress.Utils.PointFloat(4F, 4F);
            this.xrLabelKpi3Caption.Name = "xrLabelKpi3Caption";
            this.xrLabelKpi3Caption.SizeF = new System.Drawing.SizeF(172F, 14F);
            this.xrLabelKpi3Caption.StylePriority.UseFont = false;
            this.xrLabelKpi3Caption.Text = "AVG ORDER VALUE";

            this.xrLabelKpi3Value.Font = new DevExpress.Drawing.DXFont("Arial", 20F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelKpi3Value.LocationFloat = new DevExpress.Utils.PointFloat(4F, 22F);
            this.xrLabelKpi3Value.Name = "xrLabelKpi3Value";
            this.xrLabelKpi3Value.SizeF = new System.Drawing.SizeF(172F, 34F);
            this.xrLabelKpi3Value.StyleName = "KpiStyle";
            this.xrLabelKpi3Value.StylePriority.UseFont = false;
            this.xrLabelKpi3Value.TextFormatString = "{0:c0}";
            this.xrLabelKpi3Value.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "Iif(Count([OrderID]) > 0, Sum([OrderTotal]) / Count([OrderID]), 0)") });

            // KPI 4 — YoY Growth
            this.xrPanelKpi4.BackColor = System.Drawing.Color.FromArgb(245, 230, 255);
            this.xrPanelKpi4.BorderColor = System.Drawing.Color.FromArgb(210, 180, 240);
            this.xrPanelKpi4.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.xrPanelKpi4.LocationFloat = new DevExpress.Utils.PointFloat(564F, 75F);
            this.xrPanelKpi4.Name = "xrPanelKpi4";
            this.xrPanelKpi4.SizeF = new System.Drawing.SizeF(186F, 60F);
            this.xrPanelKpi4.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelKpi4Caption, this.xrLabelKpi4Value });

            this.xrLabelKpi4Caption.Font = new DevExpress.Drawing.DXFont("Arial", 7.5F);
            this.xrLabelKpi4Caption.LocationFloat = new DevExpress.Utils.PointFloat(4F, 4F);
            this.xrLabelKpi4Caption.Name = "xrLabelKpi4Caption";
            this.xrLabelKpi4Caption.SizeF = new System.Drawing.SizeF(178F, 14F);
            this.xrLabelKpi4Caption.StylePriority.UseFont = false;
            this.xrLabelKpi4Caption.Text = "YOY GROWTH";

            this.xrLabelKpi4Value.Font = new DevExpress.Drawing.DXFont("Arial", 20F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabelKpi4Value.LocationFloat = new DevExpress.Utils.PointFloat(4F, 22F);
            this.xrLabelKpi4Value.Name = "xrLabelKpi4Value";
            this.xrLabelKpi4Value.SizeF = new System.Drawing.SizeF(178F, 34F);
            this.xrLabelKpi4Value.StyleName = "KpiStyle";
            this.xrLabelKpi4Value.StylePriority.UseFont = false;
            this.xrLabelKpi4Value.TextFormatString = "{0:p1}";
            this.xrLabelKpi4Value.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[YoYGrowthPercent]") });

            // ── GroupHeaderBand Level 0 — Sales by Rep (40 pt) ───────────────────
            this.GroupHeader1.HeightF = 40F;
            this.GroupHeader1.Level = 0;
            this.GroupHeader1.Name = "GroupHeader1";
            this.GroupHeader1.RepeatEveryPage = true;
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelSectionRep, this.xrTableRepHeader });

            this.xrLabelSectionRep.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabelSectionRep.Name = "xrLabelSectionRep";
            this.xrLabelSectionRep.SizeF = new System.Drawing.SizeF(750F, 18F);
            this.xrLabelSectionRep.StyleName = "SectionStyle";
            this.xrLabelSectionRep.StylePriority.UseFont = false;
            this.xrLabelSectionRep.StylePriority.UseTextAlignment = false;
            this.xrLabelSectionRep.Text = "SALES BY REPRESENTATIVE";
            this.xrLabelSectionRep.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // Rep header table row
            this.xrCellRepHdrName.Name = "xrCellRepHdrName";
            this.xrCellRepHdrName.StyleName = "HeaderStyle";
            this.xrCellRepHdrName.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrName.Text = "Rep Name";
            this.xrCellRepHdrName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrCellRepHdrName.Weight = 2.5D;

            this.xrCellRepHdrOrders.Name = "xrCellRepHdrOrders";
            this.xrCellRepHdrOrders.StyleName = "HeaderStyle";
            this.xrCellRepHdrOrders.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrOrders.Text = "Orders";
            this.xrCellRepHdrOrders.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepHdrOrders.Weight = 1.0D;

            this.xrCellRepHdrRevenue.Name = "xrCellRepHdrRevenue";
            this.xrCellRepHdrRevenue.StyleName = "HeaderStyle";
            this.xrCellRepHdrRevenue.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrRevenue.Text = "Revenue";
            this.xrCellRepHdrRevenue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepHdrRevenue.Weight = 1.5D;

            this.xrCellRepHdrPercent.Name = "xrCellRepHdrPercent";
            this.xrCellRepHdrPercent.StyleName = "HeaderStyle";
            this.xrCellRepHdrPercent.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrPercent.Text = "% of Total";
            this.xrCellRepHdrPercent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepHdrPercent.Weight = 1.0D;

            this.xrCellRepHdrTarget.Name = "xrCellRepHdrTarget";
            this.xrCellRepHdrTarget.StyleName = "HeaderStyle";
            this.xrCellRepHdrTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrTarget.Text = "Target";
            this.xrCellRepHdrTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepHdrTarget.Weight = 1.2D;

            this.xrCellRepHdrVsTarget.Name = "xrCellRepHdrVsTarget";
            this.xrCellRepHdrVsTarget.StyleName = "HeaderStyle";
            this.xrCellRepHdrVsTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepHdrVsTarget.Text = "vs Target";
            this.xrCellRepHdrVsTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepHdrVsTarget.Weight = 1.0D;

            this.xrRowRepHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.xrCellRepHdrName, this.xrCellRepHdrOrders, this.xrCellRepHdrRevenue,
                this.xrCellRepHdrPercent, this.xrCellRepHdrTarget, this.xrCellRepHdrVsTarget });
            this.xrRowRepHeader.Name = "xrRowRepHeader";
            this.xrRowRepHeader.Weight = 1D;

            this.xrTableRepHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 18F);
            this.xrTableRepHeader.Name = "xrTableRepHeader";
            this.xrTableRepHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.xrRowRepHeader });
            this.xrTableRepHeader.SizeF = new System.Drawing.SizeF(750F, 22F);

            // ── DetailBand (25 pt) ────────────────────────────────────────────────
            this.Detail.HeightF = 25F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrTableRepDetail });

            this.xrCellRepName.Name = "xrCellRepName";
            this.xrCellRepName.StylePriority.UseTextAlignment = false;
            this.xrCellRepName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrCellRepName.Weight = 2.5D;
            this.xrCellRepName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[RepName]") });

            this.xrCellRepOrders.Name = "xrCellRepOrders";
            this.xrCellRepOrders.StylePriority.UseTextAlignment = false;
            this.xrCellRepOrders.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepOrders.TextFormatString = "{0:n0}";
            this.xrCellRepOrders.Weight = 1.0D;
            this.xrCellRepOrders.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderCount]") });

            this.xrCellRepRevenue.Name = "xrCellRepRevenue";
            this.xrCellRepRevenue.StylePriority.UseTextAlignment = false;
            this.xrCellRepRevenue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepRevenue.TextFormatString = "{0:c0}";
            this.xrCellRepRevenue.Weight = 1.5D;
            this.xrCellRepRevenue.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Revenue]") });

            this.xrCellRepPercent.Name = "xrCellRepPercent";
            this.xrCellRepPercent.StylePriority.UseTextAlignment = false;
            this.xrCellRepPercent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepPercent.TextFormatString = "{0:p1}";
            this.xrCellRepPercent.Weight = 1.0D;
            this.xrCellRepPercent.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[RevenuePercent]") });

            this.xrCellRepTarget.Name = "xrCellRepTarget";
            this.xrCellRepTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTarget.TextFormatString = "{0:c0}";
            this.xrCellRepTarget.Weight = 1.2D;
            this.xrCellRepTarget.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Target]") });

            // vs Target cell — green if at or above target, red if below
            this.xrCellRepVsTarget.Name = "xrCellRepVsTarget";
            this.xrCellRepVsTarget.StylePriority.UseBackColor = false;
            this.xrCellRepVsTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepVsTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepVsTarget.TextFormatString = "{0:c0}";
            this.xrCellRepVsTarget.Weight = 1.0D;
            this.xrCellRepVsTarget.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "[Revenue] - [Target]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "BackColor",
                    "Iif([Revenue] >= [Target], 'PaleGreen', 'MistyRose')") });

            this.xrRowRepDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.xrCellRepName, this.xrCellRepOrders, this.xrCellRepRevenue,
                this.xrCellRepPercent, this.xrCellRepTarget, this.xrCellRepVsTarget });
            this.xrRowRepDetail.Name = "xrRowRepDetail";
            this.xrRowRepDetail.Weight = 1D;

            this.xrTableRepDetail.EvenStyleName = "EvenStyle";
            this.xrTableRepDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTableRepDetail.Name = "xrTableRepDetail";
            this.xrTableRepDetail.OddStyleName = "OddStyle";
            this.xrTableRepDetail.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.xrRowRepDetail });
            this.xrTableRepDetail.SizeF = new System.Drawing.SizeF(750F, 25F);

            // ── GroupFooterBand Level 0 — Rep Totals (25 pt) ─────────────────────
            this.GroupFooter1.HeightF = 25F;
            this.GroupFooter1.Level = 0;
            this.GroupFooter1.Name = "GroupFooter1";
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrTableRepTotal });

            this.xrCellRepTotalLabel.Name = "xrCellRepTotalLabel";
            this.xrCellRepTotalLabel.StyleName = "TotalStyle";
            this.xrCellRepTotalLabel.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalLabel.Text = "TOTAL";
            this.xrCellRepTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrCellRepTotalLabel.Weight = 2.5D;

            this.xrCellRepTotalOrders.Name = "xrCellRepTotalOrders";
            this.xrCellRepTotalOrders.StyleName = "TotalStyle";
            this.xrCellRepTotalOrders.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalOrders.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTotalOrders.TextFormatString = "{0:n0}";
            this.xrCellRepTotalOrders.Weight = 1.0D;
            this.xrCellRepTotalOrders.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "sumSum([OrderCount])") });

            this.xrCellRepTotalRevenue.Name = "xrCellRepTotalRevenue";
            this.xrCellRepTotalRevenue.StyleName = "TotalStyle";
            this.xrCellRepTotalRevenue.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalRevenue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTotalRevenue.TextFormatString = "{0:c0}";
            this.xrCellRepTotalRevenue.Weight = 1.5D;
            this.xrCellRepTotalRevenue.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "sumSum([Revenue])") });

            this.xrCellRepTotalPercent.Name = "xrCellRepTotalPercent";
            this.xrCellRepTotalPercent.StyleName = "TotalStyle";
            this.xrCellRepTotalPercent.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalPercent.Text = "100%";
            this.xrCellRepTotalPercent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTotalPercent.Weight = 1.0D;

            this.xrCellRepTotalTarget.Name = "xrCellRepTotalTarget";
            this.xrCellRepTotalTarget.StyleName = "TotalStyle";
            this.xrCellRepTotalTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTotalTarget.TextFormatString = "{0:c0}";
            this.xrCellRepTotalTarget.Weight = 1.2D;
            this.xrCellRepTotalTarget.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "sumSum([Target])") });

            this.xrCellRepTotalVsTarget.Name = "xrCellRepTotalVsTarget";
            this.xrCellRepTotalVsTarget.StyleName = "TotalStyle";
            this.xrCellRepTotalVsTarget.StylePriority.UseTextAlignment = false;
            this.xrCellRepTotalVsTarget.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrCellRepTotalVsTarget.TextFormatString = "{0:c0}";
            this.xrCellRepTotalVsTarget.Weight = 1.0D;
            this.xrCellRepTotalVsTarget.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text",
                    "sumSum([Revenue]) - sumSum([Target])") });

            this.xrRowRepTotal.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.xrCellRepTotalLabel, this.xrCellRepTotalOrders, this.xrCellRepTotalRevenue,
                this.xrCellRepTotalPercent, this.xrCellRepTotalTarget, this.xrCellRepTotalVsTarget });
            this.xrRowRepTotal.Name = "xrRowRepTotal";
            this.xrRowRepTotal.Weight = 1D;

            this.xrTableRepTotal.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTableRepTotal.Name = "xrTableRepTotal";
            this.xrTableRepTotal.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.xrRowRepTotal });
            this.xrTableRepTotal.SizeF = new System.Drawing.SizeF(750F, 25F);

            // ── GroupHeaderBand Level 1 — Sales by Category + Chart (250 pt) ─────
            this.GroupHeader2.HeightF = 250F;
            this.GroupHeader2.Level = 1;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelSectionCategory, this.xrChartCategory });

            this.xrLabelSectionCategory.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabelSectionCategory.Name = "xrLabelSectionCategory";
            this.xrLabelSectionCategory.SizeF = new System.Drawing.SizeF(750F, 18F);
            this.xrLabelSectionCategory.StyleName = "SectionStyle";
            this.xrLabelSectionCategory.StylePriority.UseFont = false;
            this.xrLabelSectionCategory.StylePriority.UseTextAlignment = false;
            this.xrLabelSectionCategory.Text = "SALES BY PRODUCT CATEGORY";
            this.xrLabelSectionCategory.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // Bar chart — DataSource assigned at runtime in SalesPerformance.cs
            this.xrChartCategory.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22F);
            this.xrChartCategory.Name = "xrChartCategory";
            this.xrChartCategory.SizeF = new System.Drawing.SizeF(750F, 220F);

            // Add a placeholder bar series — series data bound at runtime
            var barSeriesCategory = new DevExpress.XtraCharts.Series("Category Revenue",
                new DevExpress.XtraCharts.ViewType());
            barSeriesCategory.ArgumentDataMember = "Category";
            barSeriesCategory.ValueDataMembers.AddRange(new string[] { "Revenue" });
            this.xrChartCategory.Series.Add(barSeriesCategory);

            // ── GroupFooterBand Level 1 (0 pt — required pairing) ────────────────
            this.GroupFooter2.HeightF = 0F;
            this.GroupFooter2.Level = 1;
            this.GroupFooter2.Name = "GroupFooter2";

            // ── ReportFooterBand (250 pt) ─────────────────────────────────────────
            this.ReportFooter.HeightF = 250F;
            this.ReportFooter.Name = "ReportFooter";
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabelSectionTrend, this.xrChartTrend, this.xrLabelTopPerformers });

            this.xrLabelSectionTrend.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabelSectionTrend.Name = "xrLabelSectionTrend";
            this.xrLabelSectionTrend.SizeF = new System.Drawing.SizeF(750F, 18F);
            this.xrLabelSectionTrend.StyleName = "SectionStyle";
            this.xrLabelSectionTrend.StylePriority.UseFont = false;
            this.xrLabelSectionTrend.StylePriority.UseTextAlignment = false;
            this.xrLabelSectionTrend.Text = "SALES TREND";
            this.xrLabelSectionTrend.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // Line chart — DataSource assigned at runtime
            this.xrChartTrend.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22F);
            this.xrChartTrend.Name = "xrChartTrend";
            this.xrChartTrend.SizeF = new System.Drawing.SizeF(750F, 180F);

            var lineSeriesTrend = new DevExpress.XtraCharts.Series("Revenue Trend",
                new DevExpress.XtraCharts.ViewType());
            lineSeriesTrend.ArgumentDataMember = "Period";
            lineSeriesTrend.ValueDataMembers.AddRange(new string[] { "Revenue" });
            this.xrChartTrend.Series.Add(lineSeriesTrend);

            this.xrLabelTopPerformers.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.xrLabelTopPerformers.LocationFloat = new DevExpress.Utils.PointFloat(0F, 208F);
            this.xrLabelTopPerformers.Name = "xrLabelTopPerformers";
            this.xrLabelTopPerformers.SizeF = new System.Drawing.SizeF(750F, 36F);
            this.xrLabelTopPerformers.StylePriority.UseFont = false;
            this.xrLabelTopPerformers.Text = "Top 5 Products: [assigned at runtime]  ·  Top 5 Customers: [assigned at runtime]";

            // ── PageFooterBand (40 pt) ────────────────────────────────────────────
            this.PageFooter.HeightF = 40F;
            this.PageFooter.Name = "PageFooter";
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrPageInfoGenerated, this.xrPageInfo });

            this.xrPageInfoGenerated.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10F);
            this.xrPageInfoGenerated.Name = "xrPageInfoGenerated";
            this.xrPageInfoGenerated.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfoGenerated.SizeF = new System.Drawing.SizeF(300F, 18F);
            this.xrPageInfoGenerated.StylePriority.UseTextAlignment = false;
            this.xrPageInfoGenerated.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrPageInfoGenerated.TextFormatString = "Generated: {0:g}";

            this.xrPageInfo.LocationFloat = new DevExpress.Utils.PointFloat(550F, 10F);
            this.xrPageInfo.Name = "xrPageInfo";
            this.xrPageInfo.PageInfo = DevExpress.XtraPrinting.PageInfo.NumberOfTotal;
            this.xrPageInfo.SizeF = new System.Drawing.SizeF(200F, 18F);
            this.xrPageInfo.StylePriority.UseTextAlignment = false;
            this.xrPageInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.xrPageInfo.TextFormatString = "Page {0} of {1}";

            this.BottomMargin.HeightF = 50F;
            this.BottomMargin.Name = "BottomMargin";

            // ── Step 5: styles ────────────────────────────────────────────────────
            this.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.HeaderStyle.BorderColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.HeaderStyle.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.HeaderStyle.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.HeaderStyle.Name = "HeaderStyle";
            this.HeaderStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);

            this.SectionStyle.BackColor = System.Drawing.Color.FromArgb(232, 232, 232);
            this.SectionStyle.BorderColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.SectionStyle.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.SectionStyle.Font = new DevExpress.Drawing.DXFont("Arial", 11F, DevExpress.Drawing.DXFontStyle.Bold);
            this.SectionStyle.Name = "SectionStyle";
            this.SectionStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);

            this.EvenStyle.BackColor = System.Drawing.Color.White;
            this.EvenStyle.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.EvenStyle.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.EvenStyle.Name = "EvenStyle";
            this.EvenStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);

            this.OddStyle.BackColor = System.Drawing.Color.FromArgb(247, 247, 247);
            this.OddStyle.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.OddStyle.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.OddStyle.Name = "OddStyle";
            this.OddStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);

            this.TotalStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.TotalStyle.BorderColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.TotalStyle.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.TotalStyle.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.TotalStyle.Name = "TotalStyle";
            this.TotalStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 0, 0, 100F);

            this.KpiStyle.Font = new DevExpress.Drawing.DXFont("Arial", 20F, DevExpress.Drawing.DXFontStyle.Bold);
            this.KpiStyle.Name = "KpiStyle";

            // ── Step 5: parameters ────────────────────────────────────────────────
            this.PeriodStart.Description = "Period Start";
            this.PeriodStart.Name = "PeriodStart";
            this.PeriodStart.Type = typeof(System.DateTime);
            this.PeriodStart.Value = new System.DateTime(System.DateTime.Today.Year, System.DateTime.Today.Month, 1);

            this.PeriodEnd.Description = "Period End";
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.Type = typeof(System.DateTime);
            this.PeriodEnd.Value = System.DateTime.Today;

            // ── Step 5: report-level setup ────────────────────────────────────────
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.ReportHeader,
                this.GroupHeader1,
                this.Detail,
                this.GroupFooter1,
                this.GroupHeader2,
                this.GroupFooter2,
                this.ReportFooter,
                this.PageFooter,
                this.BottomMargin });
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
            this.PageHeight = 1100;
            this.PageWidth = 850;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
                this.PeriodStart, this.PeriodEnd });
            this.RequestParameters = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
                this.HeaderStyle, this.SectionStyle,
                this.EvenStyle, this.OddStyle,
                this.TotalStyle, this.KpiStyle });
            this.Version = "25.2";

            // ── Step 6: EndInit ───────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRepTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChartCategory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrChartTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        #endregion

        // Bands
        private DevExpress.XtraReports.UI.TopMarginBand    TopMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.GroupHeaderBand  GroupHeader1;
        private DevExpress.XtraReports.UI.DetailBand       Detail;
        private DevExpress.XtraReports.UI.GroupFooterBand  GroupFooter1;
        private DevExpress.XtraReports.UI.GroupHeaderBand  GroupHeader2;
        private DevExpress.XtraReports.UI.GroupFooterBand  GroupFooter2;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.PageFooterBand   PageFooter;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;

        // ReportHeader
        private DevExpress.XtraReports.UI.XRPictureBox xrLogo;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelTitle;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelPeriodCaption;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelPeriod;
        private DevExpress.XtraReports.UI.XRPanel      xrPanelKpi1;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi1Caption;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi1Value;
        private DevExpress.XtraReports.UI.XRPanel      xrPanelKpi2;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi2Caption;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi2Value;
        private DevExpress.XtraReports.UI.XRPanel      xrPanelKpi3;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi3Caption;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi3Value;
        private DevExpress.XtraReports.UI.XRPanel      xrPanelKpi4;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi4Caption;
        private DevExpress.XtraReports.UI.XRLabel      xrLabelKpi4Value;

        // GroupHeader1 — Sales by Rep
        private DevExpress.XtraReports.UI.XRLabel      xrLabelSectionRep;
        private DevExpress.XtraReports.UI.XRTable      xrTableRepHeader;
        private DevExpress.XtraReports.UI.XRTableRow   xrRowRepHeader;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrName;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrOrders;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrRevenue;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrPercent;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrTarget;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepHdrVsTarget;

        // Detail
        private DevExpress.XtraReports.UI.XRTable      xrTableRepDetail;
        private DevExpress.XtraReports.UI.XRTableRow   xrRowRepDetail;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepName;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepOrders;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepRevenue;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepPercent;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTarget;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepVsTarget;

        // GroupFooter1
        private DevExpress.XtraReports.UI.XRTable      xrTableRepTotal;
        private DevExpress.XtraReports.UI.XRTableRow   xrRowRepTotal;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalOrders;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalRevenue;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalPercent;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalTarget;
        private DevExpress.XtraReports.UI.XRTableCell  xrCellRepTotalVsTarget;

        // GroupHeader2 — Category Chart
        private DevExpress.XtraReports.UI.XRLabel  xrLabelSectionCategory;
        private DevExpress.XtraReports.UI.XRChart  xrChartCategory;

        // ReportFooter
        private DevExpress.XtraReports.UI.XRLabel  xrLabelSectionTrend;
        private DevExpress.XtraReports.UI.XRChart  xrChartTrend;
        private DevExpress.XtraReports.UI.XRLabel  xrLabelTopPerformers;

        // PageFooter
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfoGenerated;

        // Styles
        private DevExpress.XtraReports.UI.XRControlStyle HeaderStyle;
        private DevExpress.XtraReports.UI.XRControlStyle SectionStyle;
        private DevExpress.XtraReports.UI.XRControlStyle EvenStyle;
        private DevExpress.XtraReports.UI.XRControlStyle OddStyle;
        private DevExpress.XtraReports.UI.XRControlStyle TotalStyle;
        private DevExpress.XtraReports.UI.XRControlStyle KpiStyle;

        // Parameters
        private DevExpress.XtraReports.Parameters.Parameter PeriodStart;
        private DevExpress.XtraReports.Parameters.Parameter PeriodEnd;
    }
}
