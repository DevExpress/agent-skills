# Business Model — Entity Examples

Complete starter templates for EF Core and XPO business classes.

---

## Minimal EF Core Entity

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.Collections.ObjectModel;
using System.ComponentModel;

[DefaultClassOptions]
public class Contact : BaseObject {
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual Department Department { get; set; }
}

// Register in DbContext:
// public DbSet<Contact> Contacts { get; set; }
// Make sure that you use options.UseChangeTrackingProxies() in your DbContext settings.
```

## Minimal XPO Entity

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Contact : BaseObject {
    public Contact(Session session) : base(session) { }
    string fFirstName;
    public string FirstName {
        get { return fFirstName; }
        set { SetPropertyValue(nameof(FirstName), ref fFirstName, value); }
    }
}
```

## Full EF Core Entity with Relationships and Defaults

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;

[DefaultClassOptions]
[XafDisplayName("Employee")]
public class Employee : BaseObject {
    [RuleRequiredField("FirstNameRequired", DefaultContexts.Save)]
    public virtual string FirstName { get; set; }

    [RuleRequiredField("LastNameRequired", DefaultContexts.Save)]
    public virtual string LastName { get; set; }

    [Browsable(false)]
    public string FullName => $"{FirstName} {LastName}";

    public virtual DateTime? HireDate { get; set; }
    public virtual Department Department { get; set; }

    [Aggregated]
    public virtual IList<Note> Notes { get; set; } = new ObservableCollection<Note>();

    public override void OnCreated() {
        HireDate = DateTime.Today;
    }
}

[DefaultClassOptions]
public class Department : BaseObject {
    public virtual string Name { get; set; }
    public virtual IList<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
}

public class Note : BaseObject {
    public virtual string Text { get; set; }
    public virtual Employee Employee { get; set; }
}

// Add to DbContext:
// public DbSet<Employee> Employees { get; set; }
// public DbSet<Department> Departments { get; set; }
// public DbSet<Note> Notes { get; set; }
```

### What This Does

Creates an Employee class with a required name, a one-to-many relationship to Department, an aggregated Notes collection, and a default HireDate set on creation. The `[DefaultClassOptions]` attribute adds the class to navigation and makes it visible in List/Detail views.
