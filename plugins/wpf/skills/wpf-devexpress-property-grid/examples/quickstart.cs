// DevExpress WPF Property Grid — Quickstart (C#)
// Demonstrates: auto-generate from CLR object, custom definitions, categories, collection

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using DevExpress.Xpf.PropertyGrid;

// ------------------------------------------------------------------
// 1. Data object — auto-generated editors
// ------------------------------------------------------------------
public enum Gender { Male, Female }

public class Customer {
    public int Id { get; set; } = 1;

    [Display(Name = "First Name", GroupName = "Personal")]
    public string FirstName { get; set; } = "Nancy";

    [Display(Name = "Last Name", GroupName = "Personal")]
    public string LastName { get; set; } = "Davolio";

    [Display(GroupName = "Personal")]
    public Gender Gender { get; set; } = Gender.Female;

    [Display(Name = "Birth Date", GroupName = "Personal")]
    public DateTime BirthDate { get; set; } = new DateTime(1948, 8, 12);

    [Display(GroupName = "Contact")]
    public string Phone { get; set; } = "7138638137";

    [Display(GroupName = "Contact")]
    public string Email { get; set; } = "nancy@example.com";
}

// ------------------------------------------------------------------
// 2. Code-behind — bind SelectedObject
//
// XAML:
//   <Window DataContext="{Binding}">
//       <dxprg:PropertyGridControl SelectedObject="{Binding Customer}"/>
//   </Window>
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    public Customer Customer { get; } = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = this;
    }
}

// ------------------------------------------------------------------
// 3. Selective property definitions (hide/reorder/customize editors)
//
// XAML:
//   <dxprg:PropertyGridControl SelectedObject="{Binding Customer}">
//       <dxprg:PropertyGridControl.PropertyDefinitions>
//           <dxprg:PropertyDefinition Name="Id" IsReadOnly="True"/>
//           <dxprg:PropertyDefinition Name="FirstName" ShowName="True"/>
//           <dxprg:PropertyDefinition Name="LastName"  ShowName="True"/>
//           <dxprg:PropertyDefinition Name="BirthDate" ShowName="True"/>
//       </dxprg:PropertyGridControl.PropertyDefinitions>
//   </dxprg:PropertyGridControl>
// ------------------------------------------------------------------

// ------------------------------------------------------------------
// 4. Listen for property value changes
// ------------------------------------------------------------------
public partial class TrackingWindow : Window {
    void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
        System.Diagnostics.Debug.WriteLine($"{e.Property.Name} changed to {e.NewValue}");
    }
}
