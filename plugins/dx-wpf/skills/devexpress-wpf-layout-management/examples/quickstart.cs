// DevExpress WPF Layout Management — Quickstart (C#)
// Demonstrates: DockLayoutManager shell (runtime DocumentPanel), DataLayoutControl form
// (Layout persistence — SaveLayoutToXml/RestoreLayoutFromXml — is covered in
//  references/save-restore-layout.md)

using System.Windows;
using DevExpress.Xpf.Docking;

// ------------------------------------------------------------------
// 1. Add a DocumentPanel at runtime
// ------------------------------------------------------------------
public partial class DynamicDocsWindow : Window {
    int _docCounter;

    void AddDocument() {
        var panel = new DocumentPanel {
            Caption = $"Document {++_docCounter}",
            Content = new System.Windows.Controls.TextBox { Text = "New document content" }
        };
        documentGroup.Items.Add(panel);
        dockManager.DockController.Activate(panel);
    }
}

// ------------------------------------------------------------------
// 2. DataLayoutControl — auto-generate a form from a data object
//    (No XAML needed for field definitions — uses data annotations)
// ------------------------------------------------------------------
public class PersonForm {
    [System.ComponentModel.DataAnnotations.Display(Name = "First Name", GroupName = "Personal")]
    public string FirstName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Display(Name = "Last Name", GroupName = "Personal")]
    public string LastName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Display(GroupName = "Contact")]
    public string Email { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Display(GroupName = "Contact")]
    public string Phone { get; set; } = "";
}

// Code-behind for auto-form window:
//   dataLayoutControl.CurrentItem = new PersonForm();
