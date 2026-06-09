// Quickstart ViewModel for the DevExpress WPF Data Grid.
// Companion to quickstart-mainview.xaml.
//
// Verified APIs:
//   - DevExpress.Mvvm.ViewModelBase (from DevExpress.Mvvm package, bundled
//     with DevExpress.Wpf.Grid.Core)

using System;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;

namespace DevExpressGridQuickstart.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<Order> Orders { get; }

    public MainViewModel()
    {
        Orders = new ObservableCollection<Order>
        {
            new Order { OrderId = 10248, OrderDate = new DateTime(1996, 7, 4), ShipCity = "Reims",          Freight = 32.38m },
            new Order { OrderId = 10249, OrderDate = new DateTime(1996, 7, 5), ShipCity = "Munster",        Freight = 11.61m },
            new Order { OrderId = 10250, OrderDate = new DateTime(1996, 7, 8), ShipCity = "Rio de Janeiro", Freight = 65.83m },
            new Order { OrderId = 10251, OrderDate = new DateTime(1996, 7, 8), ShipCity = "Lyon",           Freight = 41.34m },
            new Order { OrderId = 10252, OrderDate = new DateTime(1996, 7, 9), ShipCity = "Charleroi",      Freight = 51.30m },
        };
    }
}

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipCity { get; set; } = string.Empty;
    public decimal Freight { get; set; }
}
