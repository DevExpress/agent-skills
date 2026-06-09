// DevExpress WPF TreeList — Quickstart (C#)
// Demonstrates: self-referential binding, hierarchical binding, unbound mode, editing

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;

// ------------------------------------------------------------------
// 1. Self-referential data model
// ------------------------------------------------------------------
public class Employee {
    public int ID { get; set; }
    public int ParentID { get; set; }
    public string Name { get; set; } = "";
    public string Department { get; set; } = "";
    public string Position { get; set; } = "";
}

// ------------------------------------------------------------------
// 2. ViewModel — self-referential tree
//
// XAML (MainWindow.xaml):
//   <Window DataContext="{Binding Source={StaticResource vm}}">
//       <dxg:TreeListControl ItemsSource="{Binding Employees}">
//           <dxg:TreeListControl.View>
//               <dxg:TreeListView KeyFieldName="ID"
//                                 ParentFieldName="ParentID"
//                                 AutoWidth="True"
//                                 AutoExpandAllNodes="True"/>
//           </dxg:TreeListControl.View>
//           <dxg:TreeListColumn FieldName="Name"       Header="Name"/>
//           <dxg:TreeListColumn FieldName="Department" Header="Department"/>
//           <dxg:TreeListColumn FieldName="Position"   Header="Position"/>
//       </dxg:TreeListControl>
//   </Window>
// ------------------------------------------------------------------
public class MainViewModel : ViewModelBase {
    public List<Employee> Employees { get; }

    public MainViewModel() {
        Employees = new List<Employee> {
            new() { ID = 0,              Name = "Gregory S. Price",     Position = "President" },
            new() { ID = 1, ParentID=0,  Name = "Irma R. Marshall",     Department = "Marketing",  Position = "Vice President" },
            new() { ID = 2, ParentID=0,  Name = "John C. Powell",       Department = "Operations", Position = "Vice President" },
            new() { ID = 5, ParentID=1,  Name = "Brian C. Cowling",     Department = "Marketing",  Position = "Manager" },
            new() { ID = 6, ParentID=1,  Name = "Thomas C. Dawson",     Department = "Marketing",  Position = "Manager" },
            new() { ID = 9, ParentID=2,  Name = "Harold S. Brandes",    Department = "Operations", Position = "Manager" },
        };
    }
}

// ------------------------------------------------------------------
// 3. Hierarchical binding — ChildNodesPath (children collection on item)
//
// XAML:
//   <dxg:TreeListControl ItemsSource="{Binding Departments}">
//       <dxg:TreeListControl.View>
//           <dxg:TreeListView ChildNodesPath="SubDepartments"
//                             TreeDerivationMode="ChildNodesSelector"/>
//       </dxg:TreeListControl.View>
//       <dxg:TreeListColumn FieldName="Name"/>
//   </dxg:TreeListControl>
// ------------------------------------------------------------------
public class Department {
    public string Name { get; set; } = "";
    public ObservableCollection<Department> SubDepartments { get; set; } = new();
}

// ------------------------------------------------------------------
// 4. Unbound mode — build tree programmatically (no ItemsSource)
// ------------------------------------------------------------------
public partial class UnboundWindow : Window {
    void BuildTree() {
        var view = (TreeListView)treeList.View;
        var root = view.Nodes.Add();
        root.SetValue("Name", "Root");

        var child1 = root.Add();
        child1.SetValue("Name", "Child 1");

        var child2 = root.Add();
        child2.SetValue("Name", "Child 2");
        root.IsExpanded = true;
    }
}

// ------------------------------------------------------------------
// 5. Row validation
// ------------------------------------------------------------------
public partial class ValidatingWindow : Window {
    void OnValidateRow(object sender, TreeListRowValidationEventArgs e) {
        var emp = (Employee)e.Row;
        if (string.IsNullOrWhiteSpace(emp.Name)) {
            e.IsValid = false;
            e.ErrorContent = "Name is required.";
        }
    }
}
