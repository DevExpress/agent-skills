// DevExpress WPF Data Editors — Quickstart (C#)
// Demonstrates: common editors, masks, validation, in-place grid editors

using System;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Editors;

// ------------------------------------------------------------------
// 1. Data model with common property types
// ------------------------------------------------------------------
public class Employee {
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-30);
    public decimal Salary { get; set; } = 50_000m;
    public string Phone { get; set; } = "";
    public string Department { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

// ------------------------------------------------------------------
// 2. ViewModel
//
// XAML (MainWindow.xaml) — see quickstart.xaml for editor declarations:
//   <dxe:TextEdit EditValue="{Binding Employee.FirstName}"/>
//   <dxe:DateEdit EditValue="{Binding Employee.BirthDate}"/>
//   <dxe:SpinEdit EditValue="{Binding Employee.Salary}" Mask="c2"/>
// ------------------------------------------------------------------
public class MainViewModel {
    public Employee Employee { get; } = new();
    public ObservableCollection<string> Departments { get; } = new() {
        "Engineering", "Marketing", "Operations", "HR"
    };
}

// ------------------------------------------------------------------
// 3. Validate event — catch invalid input before it's committed
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    void OnPhoneValidate(object sender, ValidationEventArgs e) {
        if (e.Value is string s && !string.IsNullOrEmpty(s) && s.Length < 10) {
            e.IsValid = false;
            e.ErrorContent = "Phone number must be at least 10 digits.";
        }
    }
}

// ------------------------------------------------------------------
// 4. In-place grid editor (TextEditSettings inside GridColumn)
//
// XAML:
//   <dxg:GridColumn FieldName="Phone">
//       <dxg:GridColumn.EditSettings>
//           <dxe:TextEditSettings MaskType="Simple" Mask="+0 (000) 000-0000"/>
//       </dxg:GridColumn.EditSettings>
//   </dxg:GridColumn>
// ------------------------------------------------------------------
