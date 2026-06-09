// DevExpress WPF Pivot Grid — Quickstart (C#)
// Demonstrates: code-behind setup, MVVM binding, field configuration, OLAP

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.PivotGrid;

// ------------------------------------------------------------------
// 1. Data model
// ------------------------------------------------------------------
public class Sale {
    public string Country { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
}

public static class SalesData {
    public static List<Sale> Build() => new() {
        new() { Country = "USA",    Category = "Beverages", OrderDate = new(2024, 1, 15), Amount = 1200m },
        new() { Country = "USA",    Category = "Produce",   OrderDate = new(2024, 3,  5), Amount =  450m },
        new() { Country = "Canada", Category = "Beverages", OrderDate = new(2024, 2, 20), Amount =  800m },
        new() { Country = "France", Category = "Produce",   OrderDate = new(2025, 7, 22), Amount =  700m },
    };
}

// ------------------------------------------------------------------
// 2. Code-behind setup — add fields programmatically
//
// XAML:
//   <dxpg:PivotGridControl x:Name="pivotGridControl1"
//                          DataProcessingEngine="Optimized"
//                          Loaded="Window_Loaded"/>
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        pivotGridControl1.DataSource = SalesData.Build();

        pivotGridControl1.BeginUpdate();
        AddField("Country",  FieldArea.RowArea,    "Country");
        AddField("Category", FieldArea.RowArea,    "Category",   areaIndex: 1);
        AddField("Year",     FieldArea.ColumnArea, "OrderDate",  interval: FieldGroupInterval.DateYear);
        AddField("Sales",    FieldArea.DataArea,   "Amount");
        pivotGridControl1.EndUpdate();
    }

    void AddField(string caption, FieldArea area, string columnName,
                  int areaIndex = 0,
                  FieldGroupInterval interval = FieldGroupInterval.Default) {
        var field = pivotGridControl1.Fields.Add();
        field.Caption = caption;
        field.Area = area;
        field.DataBinding = new DataSourceColumnBinding(columnName) { GroupInterval = interval };
        field.AreaIndex = areaIndex;
    }
}

// ------------------------------------------------------------------
// 3. MVVM ViewModel with field collection
//
// XAML:
//   <dxpg:PivotGridControl DataSource="{Binding Sales}"
//                          DataProcessingEngine="Optimized">
//       <dxpg:PivotGridControl.Fields>
//           <dxpg:PivotGridField Caption="Country"  FieldName="Country"   Area="RowArea"/>
//           <dxpg:PivotGridField Caption="Category" FieldName="Category"  Area="RowArea"   AreaIndex="1"/>
//           <dxpg:PivotGridField Caption="Year"     FieldName="OrderDate" Area="ColumnArea"
//                                GroupInterval="DateYear"/>
//           <dxpg:PivotGridField Caption="Sales"    FieldName="Amount"    Area="DataArea"
//                                SummaryType="Sum"/>
//       </dxpg:PivotGridControl.Fields>
//   </dxpg:PivotGridControl>
// ------------------------------------------------------------------
[POCOViewModel]
public class PivotViewModel {
    public List<Sale> Sales { get; } = SalesData.Build();
}

// ------------------------------------------------------------------
// 4. Export to XLSX
// ------------------------------------------------------------------
public partial class ExportWindow : Window {
    void ExportToExcel() {
        pivotGridControl1.ExportToXlsx("pivot-report.xlsx");
    }
}
