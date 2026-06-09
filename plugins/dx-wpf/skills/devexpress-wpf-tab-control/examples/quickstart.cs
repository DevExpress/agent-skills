// DevExpress WPF Tab Control — Quickstart (C#)
// Demonstrates: ThemedWindow, static tabs, dynamic ItemsSource, programmatic tabs

using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Core;

// ------------------------------------------------------------------
// 1. ThemedWindow — required host for DXTabControl (no plain Window)
// ------------------------------------------------------------------
public partial class MainWindow : ThemedWindow {
    public MainWindow() {
        InitializeComponent();
    }
}

// ------------------------------------------------------------------
// 2. Data model for dynamic tabs
// ------------------------------------------------------------------
public class TabViewModel {
    public string Header { get; set; } = "";
    public string Content { get; set; } = "";
    public bool CanClose { get; set; } = true;
}

// ------------------------------------------------------------------
// 3. ViewModel with dynamic tab collection
//
// XAML:
//   <dx:ThemedWindow DataContext="{Binding Source={StaticResource vm}}">
//       <dx:DXTabControl ItemsSource="{Binding Tabs}"
//                        SelectedIndex="0">
//           <dx:DXTabControl.ItemHeaderTemplate>
//               <DataTemplate>
//                   <TextBlock Text="{Binding Header}"/>
//               </DataTemplate>
//           </dx:DXTabControl.ItemHeaderTemplate>
//           <dx:DXTabControl.ItemTemplate>
//               <DataTemplate>
//                   <TextBlock Text="{Binding Content}" Margin="8"/>
//               </DataTemplate>
//           </dx:DXTabControl.ItemTemplate>
//       </dx:DXTabControl>
//   </dx:ThemedWindow>
// ------------------------------------------------------------------
public class MainViewModel {
    public ObservableCollection<TabViewModel> Tabs { get; } = new() {
        new() { Header = "Orders",    Content = "Orders list here",    CanClose = false },
        new() { Header = "Customers", Content = "Customers list here", CanClose = false },
        new() { Header = "Reports",   Content = "Reports here",        CanClose = true  },
    };

    public void AddTab() {
        Tabs.Add(new TabViewModel {
            Header = $"New Tab {Tabs.Count + 1}",
            Content = "New content",
        });
    }
}

// ------------------------------------------------------------------
// 4. Add/remove tabs programmatically in code-behind
// ------------------------------------------------------------------
public partial class DynamicTabWindow : ThemedWindow {
    void AddTab() {
        var item = new DevExpress.Xpf.Core.DXTabItem {
            Header = $"Tab {tabControl.Items.Count + 1}",
            Content = new System.Windows.Controls.Label { Content = "Content" }
        };
        tabControl.Items.Add(item);
        tabControl.SelectedItem = item;
    }

    void RemoveSelectedTab() {
        if (tabControl.SelectedItem is DevExpress.Xpf.Core.DXTabItem item)
            tabControl.Items.Remove(item);
    }
}

// ------------------------------------------------------------------
// 5. Handle selection change
// ------------------------------------------------------------------
public partial class SelectionWindow : ThemedWindow {
    void OnSelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e) {
        System.Diagnostics.Debug.WriteLine($"Switched to: {e.NewSelectedIndex}");
    }
}
