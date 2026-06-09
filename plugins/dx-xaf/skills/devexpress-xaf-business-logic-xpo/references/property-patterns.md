# XPO Business Logic — Property & Association Patterns

Code snippets for SetPropertyValue, calculated properties, and Association patterns.

---

## SetPropertyValue / GetPropertyValue Pattern

XPO requires `SetPropertyValue` for change tracking and `GetPropertyValue` for delayed loading:

```csharp
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Contact : BaseObject {
    public Contact(Session session) : base(session) { }

    string fFirstName;
    public string FirstName {
        get { return GetPropertyValue(nameof(FirstName), ref fFirstName); }
        set { SetPropertyValue(nameof(FirstName), ref fFirstName, value); }
    }

    string fLastName;
    public string LastName {
        get { return GetPropertyValue(nameof(LastName), ref fLastName); }
        set { SetPropertyValue(nameof(LastName), ref fLastName, value); }
    }

    // Calculated (non-persistent) property:
    [PersistentAlias("Concat([FirstName], ' ', [LastName])")]
    public string FullName => (string)EvaluateAlias(nameof(FullName));
}
```

> `GetPropertyValue(nameof(Prop), ref _field)` is the XPO-native getter counterpart to `SetPropertyValue`. It ensures delayed (deferred-loaded) properties are fetched from the session before being returned. For simple non-delayed properties, `return _field` also works, but `GetPropertyValue` is the safer, consistent pattern.

**Why not auto-properties?** XPO needs `SetPropertyValue` to:
1. Fire `PropertyChanged` notifications for UI binding
2. Track modifications for optimistic locking
3. Mark objects as dirty for `CommitChanges`

## One-to-Many Association

XPO uses `[Association]` attributes (not EF Core navigation conventions):

```csharp
[DefaultClassOptions]
public class Department : BaseObject {
    public Department(Session session) : base(session) { }

    string fName;
    public string Name {
        get => fName;
        set => SetPropertyValue(nameof(Name), ref fName, value);
    }

    [Association("Dept-Employees")]
    public XPCollection<Employee> Employees => GetCollection<Employee>(nameof(Employees));
}

[DefaultClassOptions]
public class Employee : BaseObject {
    public Employee(Session session) : base(session) { }

    Department fDepartment;
    [Association("Dept-Employees")]
    public Department Department {
        get => fDepartment;
        set => SetPropertyValue(nameof(Department), ref fDepartment, value);
    }
}
```

## Association Rules

- Both ends must have the same `[Association("name")]` string
- The "many" side uses `XPCollection<T>` via `GetCollection<T>()`
- The "one" side uses a reference property with `SetPropertyValue`
- Add `[Aggregated]` to the collection side for cascade delete (parent owns children)

## PersistentAlias for Server-Side Calculations

Use `PersistentAlias` for properties that can be evaluated as SQL — enables server-mode sorting/filtering:

```csharp
[PersistentAlias("Amount * Quantity")]
public decimal Total => (decimal)EvaluateAlias(nameof(Total));

[PersistentAlias("Iif([Status] = 'Active', 1, 0)")]
public int IsActiveInt => (int)EvaluateAlias(nameof(IsActiveInt));
```
