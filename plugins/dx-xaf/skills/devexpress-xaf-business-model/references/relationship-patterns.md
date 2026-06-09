# Business Model — Relationship Patterns

Code snippets for one-to-many, many-to-many, and aggregated relationships in EF Core and XPO.

---

## EF Core One-to-Many

```csharp
[DefaultClassOptions]
public class Department : BaseObject {
    public virtual string Name { get; set; }
    public virtual IList<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
}

[DefaultClassOptions]
public class Contact : BaseObject {
    public virtual string Name { get; set; }
    public virtual Department Department { get; set; }
}
// Register both in DbContext. Use options.UseChangeTrackingProxies().
```

## XPO One-to-Many

Use `[Association]` with the same name on both sides. The "one" side uses a reference property with `SetPropertyValue`; the "many" side exposes an `XPCollection<T>` via `GetCollection<T>()`:

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Department : XPObject {
    public Department(Session session) : base(session) { }

    private string name;
    public string Name {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    [Association("Department-Employees")]
    public XPCollection<Employee> Employees => GetCollection<Employee>(nameof(Employees));
}

[DefaultClassOptions]
public class Employee : XPObject {
    public Employee(Session session) : base(session) { }

    private string name;
    public string Name {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    private Department department;
    [Association("Department-Employees")]
    public Department Department {
        get => department;
        set => SetPropertyValue(nameof(Department), ref department, value);
    }
}
```

## EF Core Many-to-Many

```csharp
[DefaultClassOptions]
public class Contact : BaseObject {
    public virtual string Name { get; set; }
    public virtual IList<TaskItem> Tasks { get; set; } = new ObservableCollection<TaskItem>();
}

[DefaultClassOptions]
public class TaskItem : BaseObject {
    public virtual string Subject { get; set; }
    public virtual IList<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
}
// Both sides must declare the collection. EF Core creates the join table automatically.
```

## XPO Many-to-Many

Use `[Association]` with the same name on both sides. Both sides expose an `XPCollection<T>` via `GetCollection<T>()`. XPO creates the intermediate join table automatically:

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Contact : XPObject {
    public Contact(Session session) : base(session) { }

    private string name;
    public string Name {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    [Association("Contact-Tasks")]
    public XPCollection<TaskItem> Tasks => GetCollection<TaskItem>(nameof(Tasks));
}

[DefaultClassOptions]
public class TaskItem : XPObject {
    public TaskItem(Session session) : base(session) { }

    private string subject;
    public string Subject {
        get => subject;
        set => SetPropertyValue(nameof(Subject), ref subject, value);
    }

    [Association("Contact-Tasks")]
    public XPCollection<Contact> Contacts => GetCollection<Contact>(nameof(Contacts));
}
```

## EF Core Aggregated Collection

Add `[Aggregated]` to mark a collection as parent-owned (cascade delete — children are deleted when the parent is deleted):

```csharp
[DefaultClassOptions]
public class Order : BaseObject {
    public virtual string Number { get; set; }

    [Aggregated]
    public virtual IList<OrderLine> Lines { get; set; } = new ObservableCollection<OrderLine>();
}

public class OrderLine : BaseObject {
    public virtual string ProductName { get; set; }
    public virtual int Quantity { get; set; }
    public virtual Order Order { get; set; }
}
```

## XPO Aggregated Collection

Add `[Aggregated]` alongside `[Association]` on the `XPCollection<T>` property. Children are deleted when the parent is deleted:

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Order : XPObject {
    public Order(Session session) : base(session) { }

    private string number;
    public string Number {
        get => number;
        set => SetPropertyValue(nameof(Number), ref number, value);
    }

    [Association("Order-Lines"), Aggregated]
    public XPCollection<OrderLine> Lines => GetCollection<OrderLine>(nameof(Lines));
}

public class OrderLine : XPObject {
    public OrderLine(Session session) : base(session) { }

    private string productName;
    public string ProductName {
        get => productName;
        set => SetPropertyValue(nameof(ProductName), ref productName, value);
    }

    private Order order;
    [Association("Order-Lines")]
    public Order Order {
        get => order;
        set => SetPropertyValue(nameof(Order), ref order, value);
    }
}
```

## EF Core One-to-One

Use `HasOne(...).WithOne(...)` in the `DbContext` fluent API to configure a one-to-one relationship. One side holds the foreign key:

```csharp
[DefaultClassOptions]
public class Employee : BaseObject {
    public virtual string Name { get; set; }
    public virtual EmployeeProfile Profile { get; set; }
}

public class EmployeeProfile : BaseObject {
    public virtual string Bio { get; set; }
    public virtual Employee Employee { get; set; }
}

// In DbContext.OnModelCreating:
protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Profile)
        .WithOne(p => p.Employee)
        .HasForeignKey<EmployeeProfile>("EmployeeId");
}
```

## XPO One-to-One

XPO has no native one-to-one enforcement. Model it with reference properties on both sides and synchronize them manually:

```csharp
[DefaultClassOptions]
public class Employee : XPObject {
    public Employee(Session session) : base(session) { }

    private EmployeeProfile profile;
    public EmployeeProfile Profile {
        get => profile;
        set {
            if (SetPropertyValue(nameof(Profile), ref profile, value)) {
                if (!IsLoading && value != null && value.Employee != this)
                    value.Employee = this;
            }
        }
    }
}

public class EmployeeProfile : XPObject {
    public EmployeeProfile(Session session) : base(session) { }

    private Employee employee;
    public Employee Employee {
        get => employee;
        set {
            if (SetPropertyValue(nameof(Employee), ref employee, value)) {
                if (!IsLoading && value != null && value.Profile != this)
                    value.Profile = this;
            }
        }
    }
}
```

> **Note:** Because XPO does not enforce one-to-one at the ORM level, application logic or a unique database constraint is required to prevent multiple profiles per employee.

## Key Rules

- **EF Core**: Properties must be `public virtual` for change-tracking proxies. Collections must be initialized as `new ObservableCollection<T>()`.
- **XPO**: Use `[Association("Name")]` on both ends. Collection side uses `XPCollection<T>` via `GetCollection<T>()`. Reference side uses `SetPropertyValue`.
- **Aggregated**: Use `[Aggregated]` only when the parent owns the children (e.g., Order → OrderLines). Without it, deleting a parent leaves children as orphans.
