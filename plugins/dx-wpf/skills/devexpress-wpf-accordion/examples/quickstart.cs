// DevExpress WPF Accordion — Quickstart (C#)
// Demonstrates: hierarchical binding, search, view modes, item templates

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Accordion;

// ------------------------------------------------------------------
// 1. Data model
// ------------------------------------------------------------------
public class Employee {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Position { get; set; } = "";
    public string Department { get; set; } = "";
}

public class EmployeeDepartment {
    public string Name { get; set; } = "";
    public ObservableCollection<Employee> Employees { get; set; } = new();
}

// ------------------------------------------------------------------
// 2. ViewModel
//
// XAML (MainWindow.xaml):
//   <Window DataContext="{Binding Source={StaticResource vm}}">
//       <dxa:AccordionControl ItemsSource="{Binding EmployeeDepartments}"
//                             ChildrenPath="Employees"
//                             DisplayMemberPath="Name"/>
//   </Window>
// ------------------------------------------------------------------
public class MainViewModel {
    public ObservableCollection<EmployeeDepartment> EmployeeDepartments { get; }

    public MainViewModel() {
        var staff = new[] {
            new Employee { Id = 1, Name = "Gregory S. Price",  Department = "Management", Position = "President" },
            new Employee { Id = 2, Name = "Irma R. Marshall",  Department = "Marketing",  Position = "Vice President" },
            new Employee { Id = 3, Name = "Brian C. Cowling",  Department = "Marketing",  Position = "Manager" },
            new Employee { Id = 4, Name = "John C. Powell",    Department = "Operations", Position = "Vice President" },
            new Employee { Id = 5, Name = "Harold S. Brandes", Department = "Operations", Position = "Manager" },
        };
        EmployeeDepartments = new ObservableCollection<EmployeeDepartment>(
            staff.GroupBy(e => e.Department)
                 .Select(g => new EmployeeDepartment {
                     Name = g.Key,
                     Employees = new ObservableCollection<Employee>(g)
                 }));
    }
}

// ------------------------------------------------------------------
// 3. Handle item selection
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    void OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e) {
        if (e.NewItem is Employee emp)
            System.Diagnostics.Debug.WriteLine($"Selected: {emp.Name}");
    }
}
