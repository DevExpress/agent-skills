// DevExpress WPF Chart Control — Quickstart (C#)
// Demonstrates: ChartControl basics, series binding, pie chart, secondary axis, crosshair

using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Charts;

// ------------------------------------------------------------------
// 1. Data model
// ------------------------------------------------------------------
public class SalesPoint {
    public string Region { get; set; } = "";
    public double Revenue { get; set; }
    public double Expenses { get; set; }
}

// ------------------------------------------------------------------
// 2. ViewModel with an ObservableCollection data source
//
// XAML (MainWindow.xaml):
//   <Window DataContext="{Binding Source={StaticResource vm}}">
//       <dxc:ChartControl DataSource="{Binding Sales}">
//           <dxc:XYDiagram2D>
//               <dxc:BarSideBySideSeries2D DisplayName="Revenue"
//                                          ArgumentDataMember="Region"
//                                          ValueDataMember="Revenue"/>
//               <dxc:BarSideBySideSeries2D DisplayName="Expenses"
//                                          ArgumentDataMember="Region"
//                                          ValueDataMember="Expenses"/>
//           </dxc:XYDiagram2D>
//       </dxc:ChartControl>
//   </Window>
// ------------------------------------------------------------------
public class MainViewModel {
    public ObservableCollection<SalesPoint> Sales { get; } = new() {
        new() { Region = "Asia",          Revenue = 5.29, Expenses = 2.10 },
        new() { Region = "Europe",        Revenue = 3.73, Expenses = 1.90 },
        new() { Region = "North America", Revenue = 4.18, Expenses = 2.50 },
        new() { Region = "South America", Revenue = 2.12, Expenses = 1.20 },
    };
}

// ------------------------------------------------------------------
// 3. Code-behind: apply data source programmatically
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        chartControl.DataSource = new MainViewModel().Sales;
    }
}

// ------------------------------------------------------------------
// 4. Generate series from a collection — Diagram.SeriesItemsSource
//
// SeriesItemsSource / SeriesItemTemplate live on the DIAGRAM, not on
// ChartControl. Each ViewModel item produces one Series.
//
// XAML:
//   <dxc:ChartControl>
//     <dxc:XYDiagram2D SeriesItemsSource="{Binding SeriesList}">
//       <dxc:XYDiagram2D.SeriesItemTemplate>
//         <DataTemplate>
//           <dxc:LineSeries2D ArgumentDataMember="Month"
//                             ValueDataMember="Value"
//                             DisplayName="{Binding Name}"
//                             DataSource="{Binding Data}"/>
//         </DataTemplate>
//       </dxc:XYDiagram2D.SeriesItemTemplate>
//     </dxc:XYDiagram2D>
//   </dxc:ChartControl>
// ------------------------------------------------------------------
public class SeriesDescriptor {
    public string Name { get; set; } = "";
    public ObservableCollection<DataPoint> Data { get; set; } = new();
}

public class DataPoint {
    public string Month { get; set; } = "";
    public double Value { get; set; }
}

// ------------------------------------------------------------------
// 5. Secondary axis — second Y axis for a different scale
//
// XAML:
//   <dxc:XYDiagram2D>
//     <dxc:XYDiagram2D.SecondaryAxesY>
//       <dxc:SecondaryAxisY2D x:Name="axisPercent">
//         <dxc:SecondaryAxisY2D.Title>
//           <dxc:AxisTitle Content="Share (%)"/>
//         </dxc:SecondaryAxisY2D.Title>
//       </dxc:SecondaryAxisY2D>
//     </dxc:XYDiagram2D.SecondaryAxesY>
//     <dxc:BarSideBySideSeries2D .../>
//     <dxc:LineSeries2D ... AxisY="{Binding ElementName=axisPercent}"/>
//   </dxc:XYDiagram2D>
// ------------------------------------------------------------------

// ------------------------------------------------------------------
// 6. Crosshair and tooltips (code-behind)
// ------------------------------------------------------------------
public partial class ChartInteractionWindow : Window {
    void ConfigureCrosshair() {
        var diagram = (XYDiagram2D)chartControl.Diagram;
        diagram.EnableAxisXNavigation = true;
        chartControl.CrosshairEnabled = true;     // toggle crosshair on the control
        chartControl.CrosshairOptions.ContentShowMode = CrosshairContentShowMode.Label;
    }
}

// ------------------------------------------------------------------
// 7. End-user selection
//
// XAML:
//   <dxc:ChartControl SelectionMode="Single" SelectedItems="{Binding Selected}">
//       ...
//   </dxc:ChartControl>
// ------------------------------------------------------------------
public class SelectionViewModel {
    public ObservableCollection<object> Selected { get; } = new();
}
