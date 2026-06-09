// Quickstart MainWindow code-behind for the DevExpress WPF Pivot Grid.
// Companion to quickstart-mainview.xaml.
//
// Verified APIs (Lesson 1 of getting-started + apidoc):
//   - DevExpress.Xpf.PivotGrid.PivotGridControl.DataSource (object)
//   - DevExpress.Xpf.PivotGrid.PivotGridControl.Fields (PivotGridFieldCollection)
//   - DevExpress.Xpf.PivotGrid.PivotGridControl.BeginUpdate() / EndUpdate()
//   - DevExpress.Xpf.PivotGrid.PivotGridField (Caption, Area, AreaIndex, DataBinding)
//   - DevExpress.Xpf.PivotGrid.FieldArea (RowArea, ColumnArea, DataArea, FilterArea)
//   - DevExpress.Xpf.PivotGrid.DataSourceColumnBinding (ColumnName, GroupInterval)
//   - DevExpress.Xpf.PivotGrid.FieldGroupInterval (DateYear, etc.)
//
// Note the System.Windows.RoutedEventArgs qualification to avoid the
// `RoutedEventArgs` -> `System.Windows.Forms.PaintEventArgs` style ambiguity
// that ImplicitUsings can introduce. The Window base class is also qualified.

using DevExpress.Xpf.PivotGrid;

namespace DevExpressPivotGridQuickstart;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow() => InitializeComponent();

    private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        // Step 1: Set DataSource BEFORE adding fields.
        pivotGridControl1.DataSource = SalesData.Build();

        // Step 2: Batch field additions (avoids intermediate layout recalcs).
        pivotGridControl1.BeginUpdate();

        // Row area: two-level hierarchy — Country → Category.
        AddField("Country",  FieldArea.RowArea,    "Country",  areaIndex: 0);
        AddField("Category", FieldArea.RowArea,    "Category", areaIndex: 1);

        // Column area: roll OrderDate up to Year.
        AddField("Year", FieldArea.ColumnArea, "OrderDate",
                 areaIndex: 0,
                 interval: FieldGroupInterval.DateYear);

        // Data area: Sum of Amount per cell.
        AddField("Sales", FieldArea.DataArea, "Amount", areaIndex: 0);

        pivotGridControl1.EndUpdate();
    }

    private void AddField(string caption,
                          FieldArea area,
                          string columnName,
                          int areaIndex = 0,
                          FieldGroupInterval interval = FieldGroupInterval.Default)
    {
        var field = pivotGridControl1.Fields.Add();
        field.Caption = caption;
        field.Area = area;
        field.DataBinding = new DataSourceColumnBinding(columnName) { GroupInterval = interval };
        field.AreaIndex = areaIndex;   // MUST be set AFTER adding to Fields.
    }
}
