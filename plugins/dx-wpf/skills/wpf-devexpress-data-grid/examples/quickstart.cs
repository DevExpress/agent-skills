// DevExpress WPF Data Grid — Quickstart (C#)
// Demonstrates: basic binding, columns, sorting/filtering/grouping, editing, master-detail

using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Grid;

// ------------------------------------------------------------------
// 1. Data model
// ------------------------------------------------------------------
public class Order {
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public string Product { get; set; } = "";
    public double Total { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "";
}

public class OrderDetail {
    public int OrderId { get; set; }
    public string Item { get; set; } = "";
    public int Quantity { get; set; }
    public double Price { get; set; }
}

// ------------------------------------------------------------------
// 2. ViewModel
//
// XAML (MainWindow.xaml):
//   <Window DataContext="{dxmvvm:ViewModelSource Type=local:MainViewModel}">
//       <dxg:GridControl ItemsSource="{Binding Orders}"
//                        AutoGenerateColumns="AddNew">
//           <dxg:GridControl.View>
//               <dxg:TableView AllowEditing="True"/>
//           </dxg:GridControl.View>
//       </dxg:GridControl>
//   </Window>
// ------------------------------------------------------------------
[POCOViewModel]
public class MainViewModel {
    public ObservableCollection<Order> Orders { get; }

    public MainViewModel() {
        Orders = new ObservableCollection<Order> {
            new() { Id = 1, Customer = "Alfreds",    Product = "Widget", Total = 320,  OrderDate = new(2025,1,10), Status = "Shipped" },
            new() { Id = 2, Customer = "Contoso",    Product = "Gadget", Total = 180,  OrderDate = new(2025,2,14), Status = "Pending" },
            new() { Id = 3, Customer = "Northwind",  Product = "Widget", Total = 540,  OrderDate = new(2025,3,5),  Status = "Shipped" },
        };
    }
}

// ------------------------------------------------------------------
// 3. Explicit column definitions (XAML)
//
// <dxg:GridControl ItemsSource="{Binding Orders}">
//     <dxg:GridControl.Columns>
//         <dxg:GridColumn FieldName="Id"        Header="ID"     Width="50"/>
//         <dxg:GridColumn FieldName="Customer"  Header="Customer"/>
//         <dxg:GridColumn FieldName="Total"     Header="Total"  DisplayFormat="c2"/>
//         <dxg:GridColumn FieldName="OrderDate" Header="Date"   DisplayFormat="d"/>
//         <dxg:GridColumn FieldName="Status"    Header="Status"/>
//     </dxg:GridControl.Columns>
//     <dxg:GridControl.View>
//         <dxg:TableView AllowEditing="True" ShowGroupPanel="True"/>
//     </dxg:GridControl.View>
// </dxg:GridControl>
// ------------------------------------------------------------------

// ------------------------------------------------------------------
// 4. Sorting, filtering, grouping (code-behind)
// ------------------------------------------------------------------
public partial class GridWindow : Window {
    void ConfigureGrid() {
        gridControl.SortBy(gridControl.Columns["Total"], DevExpress.Data.ColumnSortOrder.Descending);
        gridControl.GroupBy(gridControl.Columns["Status"]);
        ((TableView)gridControl.View).SearchPanelAllowFilter = true;
    }
}

// ------------------------------------------------------------------
// 5. Master-detail (XAML)
//
// <dxg:GridControl ItemsSource="{Binding Orders}">
//     <dxg:GridControl.DetailDescriptor>
//         <dxg:DataControlDetailDescriptor ItemsSourceBinding="{Binding Details}">
//             <dxg:DataControlDetailDescriptor.DetailControl>
//                 <dxg:GridControl AutoGenerateColumns="AddNew">
//                     <dxg:GridControl.View>
//                         <dxg:TableView/>
//                     </dxg:GridControl.View>
//                 </dxg:GridControl>
//             </dxg:DataControlDetailDescriptor.DetailControl>
//         </dxg:DataControlDetailDescriptor>
//     </dxg:GridControl.DetailDescriptor>
//     <dxg:GridControl.View>
//         <dxg:TableView/>
//     </dxg:GridControl.View>
// </dxg:GridControl>
// ------------------------------------------------------------------
public class OrderWithDetails : Order {
    public ObservableCollection<OrderDetail> Details { get; set; } = new();
}

// ------------------------------------------------------------------
// 6. Row validation
// ------------------------------------------------------------------
public partial class ValidatingGridWindow : Window {
    void OnValidateRow(object sender, GridRowValidationEventArgs e) {
        var order = (Order)e.Row;
        if (order.Total < 0) {
            e.IsValid = false;
            e.ErrorContent = "Total must be non-negative.";
        }
    }
}

// ------------------------------------------------------------------
// 7. Export to XLSX (requires DevExpress.Wpf.Grid reference)
// ------------------------------------------------------------------
public partial class ExportWindow : Window {
    void ExportToExcel() {
        ((TableView)gridControl.View).ExportToXlsx("orders.xlsx");
    }
}
