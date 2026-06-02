// DevExpress WPF Ribbon and Bars — Quickstart (C#)
// Demonstrates: ThemedWindow + RibbonControl, command binding, bars-only UI

using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;

// ------------------------------------------------------------------
// 1. ThemedWindow with Ribbon — required base class
// ------------------------------------------------------------------
public partial class MainWindow : ThemedWindow {
    public MainWindow() {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}

// ------------------------------------------------------------------
// 2. ViewModel with commands for Ribbon items
//
// XAML (MainWindow.xaml) — see quickstart.xaml for the full Ribbon structure:
//   <dxr:RibbonControl RibbonStyle="Office2019">
//       <dxr:RibbonDefaultPageCategory>
//           <dxr:RibbonPage Caption="Home">
//               <dxr:RibbonPageGroup Caption="File">
//                   <dxb:BarButtonItem Content="New"  Command="{Binding NewCommand}"/>
//                   <dxb:BarButtonItem Content="Open" Command="{Binding OpenCommand}"/>
//                   <dxb:BarButtonItem Content="Save" Command="{Binding SaveCommand}"/>
//               </dxr:RibbonPageGroup>
//           </dxr:RibbonPage>
//       </dxr:RibbonDefaultPageCategory>
//   </dxr:RibbonControl>
// ------------------------------------------------------------------
[POCOViewModel]
public class MainViewModel {
    public void New()  => System.Diagnostics.Debug.WriteLine("New document");
    public void Open() => System.Diagnostics.Debug.WriteLine("Open document");
    public void Save() => System.Diagnostics.Debug.WriteLine("Save document");
}

// ------------------------------------------------------------------
// 3. Show/hide a contextual category (e.g., "Table Tools" in Word)
// ------------------------------------------------------------------
public partial class ContextualRibbonWindow : ThemedWindow {
    void ShowTableTools(bool visible) {
        // ribbonControl and tableToolsCategory are declared in XAML
        tableToolsCategory.IsVisible = visible;
    }
}

// ------------------------------------------------------------------
// 4. Bars-only UI (no Ribbon) — MainMenu + ToolBar + StatusBar
//
// XAML:
//   <dx:ThemedWindow ...>
//       <DockPanel>
//           <dxb:MainMenuControl DockPanel.Dock="Top">
//               <dxb:BarSubItem Content="File">
//                   <dxb:BarButtonItem Content="New"/>
//                   <dxb:BarButtonItem Content="Open"/>
//               </dxb:BarSubItem>
//               <dxb:BarSubItem Content="Edit">
//                   <dxb:BarButtonItem Content="Undo"/>
//                   <dxb:BarButtonItem Content="Redo"/>
//               </dxb:BarSubItem>
//           </dxb:MainMenuControl>
//           <dxb:ToolBarControl DockPanel.Dock="Top">
//               <dxb:BarButtonItem Content="Save" Glyph="..."/>
//           </dxb:ToolBarControl>
//           <dxb:StatusBarControl DockPanel.Dock="Bottom">
//               <dxb:BarStaticItem Content="Ready"/>
//           </dxb:StatusBarControl>
//           <ContentControl/>
//       </DockPanel>
//   </dx:ThemedWindow>
// ------------------------------------------------------------------
