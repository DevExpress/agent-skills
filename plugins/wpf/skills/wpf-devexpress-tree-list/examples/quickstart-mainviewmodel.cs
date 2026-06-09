// Quickstart ViewModel for the DevExpress WPF TreeList.
// Companion to quickstart-mainview.xaml.
//
// Verified APIs:
//   - DevExpress.Mvvm.ViewModelBase (DevExpress.Mvvm package, bundled with
//     DevExpress.Wpf.Grid.Core)
//
// Tree shape (self-referential):
//   - Root: ID=0 (President), ParentID=null
//   - Level 1: VPs (Marketing, Operations, Production, Finance) report to President
//   - Level 2: Managers report to their VP

using System.Collections.Generic;
using DevExpress.Mvvm;

namespace DevExpressTreeListQuickstart.ViewModels;

public class MainViewModel : ViewModelBase
{
    public List<Employee> Employees { get; }

    public MainViewModel()
    {
        Employees = new List<Employee>
        {
            new Employee { ID = 0,                Name = "Gregory S. Price",     Department = "",            Position = "President" },
            new Employee { ID = 1,  ParentID = 0, Name = "Irma R. Marshall",     Department = "Marketing",   Position = "Vice President" },
            new Employee { ID = 2,  ParentID = 0, Name = "John C. Powell",       Department = "Operations",  Position = "Vice President" },
            new Employee { ID = 3,  ParentID = 0, Name = "Christian P. Laclair", Department = "Production",  Position = "Vice President" },
            new Employee { ID = 4,  ParentID = 0, Name = "Karen J. Kelly",       Department = "Finance",     Position = "Vice President" },

            new Employee { ID = 5,  ParentID = 1, Name = "Brian C. Cowling",     Department = "Marketing",   Position = "Manager" },
            new Employee { ID = 6,  ParentID = 1, Name = "Thomas C. Dawson",     Department = "Marketing",   Position = "Manager" },
            new Employee { ID = 7,  ParentID = 1, Name = "Angel M. Wilson",      Department = "Marketing",   Position = "Manager" },

            new Employee { ID = 9,  ParentID = 2, Name = "Harold S. Brandes",    Department = "Operations",  Position = "Manager" },
            new Employee { ID = 10, ParentID = 2, Name = "Michael S. Blevins",   Department = "Operations",  Position = "Manager" },

            new Employee { ID = 13, ParentID = 3, Name = "James L. Kelsey",      Department = "Production",  Position = "Manager" },
            new Employee { ID = 14, ParentID = 3, Name = "Howard M. Carpenter",  Department = "Production",  Position = "Manager" },

            new Employee { ID = 16, ParentID = 4, Name = "Judith P. Underhill",  Department = "Finance",     Position = "Manager" },
            new Employee { ID = 17, ParentID = 4, Name = "Russell E. Belton",    Department = "Finance",     Position = "Manager" },
        };
    }
}

public class Employee
{
    public int ID { get; set; }

    // Nullable so the President's ParentID is null and unambiguous as the root.
    // Source: see references/data-binding.md § "Self-Referential" for root detection.
    public int? ParentID { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}
