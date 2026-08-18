# Getting Started with AccordionControl

The `AccordionControl` is a side-navigation control from the `DevExpress.XtraBars.Navigation` namespace. It renders a hierarchical, expandable menu of groups and items and supports both a classic accordion layout and a Hamburger Menu view style.

## When to Use This Reference

- Setting up `AccordionControl` in a new project for the first time
- Understanding which NuGet package and assembly to reference
- Adding the control to a form in code or in the designer file
- Choosing the right host form type

## NuGet Package

```
DevExpress.Win.Navigation
```

This package ships `DevExpress.XtraBars.v26.1.dll`, which contains the entire `DevExpress.XtraBars.Navigation` namespace.

> **Install the package:**
> ```powershell
> dotnet add package DevExpress.Win.Navigation
> ```

## Required Namespace Imports

```csharp
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;       // XtraForm
using DevExpress.XtraBars.Ribbon;   // RibbonForm (optional)
```

## Host Form Requirements

`AccordionControl` works on any `Form`, but DevExpress recommends hosting it on:

| Host Form | When to Use |
|---|---|
| `XtraForm` | Standard side-navigation layout — the accordion docks to the left, the content area fills the rest of the form. |
| `RibbonForm` | When the app also has a `RibbonControl`. The accordion can be connected via `RibbonForm.NavigationControl` so it stretches alongside the Ribbon title bar. |
| `FluentDesignForm` | When you need Acrylic Material / Reveal Highlight effects and an adaptive Hamburger Menu that automatically switches display modes on resize. |

Using a plain `Form` loses skin-aware rendering. Always derive your form from `XtraForm` (or `RibbonForm` / `FluentDesignForm`) to ensure correct theming.

## Minimal Setup in Code

```csharp
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System.Windows.Forms;

public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();

        var accordion = new AccordionControl { Dock = DockStyle.Left };
        Controls.Add(accordion);

        // Root group
        var grpContacts = new AccordionControlElement(ElementStyle.Group) {
            Text = "Contacts",
            Expanded = true
        };

        // Child items
        var itemCustomers = new AccordionControlElement(ElementStyle.Item) { Text = "Customers" };
        var itemEmployees = new AccordionControlElement(ElementStyle.Item) { Text = "Employees" };

        grpContacts.Elements.AddRange(new[] { itemCustomers, itemEmployees });
        accordion.Elements.Add(grpContacts);

        // Handle item clicks
        accordion.ElementClick += (s, e) => {
            if (e.Element.Style == ElementStyle.Item)
                MessageBox.Show(e.Element.Text + " clicked");
        };
    }
}
```

Key points:
- `ElementStyle.Group` creates a collapsible group; `ElementStyle.Item` creates a clickable leaf.
- Root-level elements go into `AccordionControl.Elements`; nested elements go into `AccordionControlElement.Elements`.
- Wrap bulk additions in `BeginUpdate()` / `EndUpdate()` to suppress intermediate redraws.

## Integration with RibbonForm (optional)

```csharp
// In a RibbonForm constructor
accordion = new AccordionControl { Dock = DockStyle.Left };
Controls.Add(accordion);
this.NavigationControl = accordion;
this.NavigationControlLayoutMode =
    RibbonFormNavigationControlLayoutMode.StretchToFormTitle;
```

This stretches the accordion alongside the Ribbon title bar, giving an Outlook-style layout.

## Common Issues

| Symptom | Cause | Solution |
|---|---|---|
| Control looks unstyled / flat | Hosting on plain `Form` | Derive from `XtraForm` |
| Items not visible at runtime | `Elements` collection empty | Populate inside `Form_Load` or constructor after `InitializeComponent` |
| Excessive flicker on bulk add | Adding elements one by one | Wrap with `BeginUpdate()` / `EndUpdate()` |
| Accordion overlaps content area | `Dock` not set | Set `Dock = DockStyle.Left` (or Right/Top) |

## Source Material

- `articles/114553` (`xref:DevExpress.XtraBars.Navigation.AccordionControl`) — Accordion Control overview
- `articles/120496` — How To: Create AccordionControl in code
- `articles/DevExpress.XtraBars.Navigation.AccordionControl` — AccordionControl class reference
