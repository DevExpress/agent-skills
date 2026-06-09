# Business Model — XPO-Specific Patterns

XPO base class choices and the `SetPropertyValue` property pattern.

---

## XPO Base Class Choices

| Base Class | Key Type | Optimistic Locking | Deferred Deletion |
|---|---|---|---|
| `XPLiteObject` | Manual (you define) | No | No |
| `XPBaseObject` | Manual | Yes | No |
| `XPCustomObject` | Manual | Yes | Yes |
| `XPObject` | `int` (auto) | Yes | Yes |
| `BaseObject` | `Guid` (auto) | Yes | Yes |

- **`BaseObject`** is recommended for most XAF applications (Guid key, full feature set).
- **`XPObject`** provides auto-increment integer keys.
- **`XPLiteObject`** is the lightest base — define your own key property with `[Key(AutoGenerate = true)]`.

## XPO Property with SetPropertyValue

XPO requires `SetPropertyValue` for change tracking and UI notifications. Auto-properties do **not** work:

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

    string fLastName;
    public string LastName {
        get { return fLastName; }
        set { SetPropertyValue(nameof(LastName), ref fLastName, value); }
    }
    // SetPropertyValue fires PropertyChanged and marks the object as modified
}
```

## XPO GetPropertyValue — Lazy-Loaded and Delayed Properties

`GetPropertyValue<T>(propertyName)` is the counterpart to `SetPropertyValue`. Use it for properties loaded on demand (e.g., large strings or BLOBs marked with `[Delayed]`):

```csharp
[DefaultClassOptions]
public class Document : BaseObject {
    public Document(Session session) : base(session) { }

    // Large content loaded only when accessed
    [Delayed(true)]
    public string Content {
        get => GetPropertyValue<string>(nameof(Content));
        set => SetPropertyValue(nameof(Content), value);
    }
}
```

> `[Delayed]` properties are excluded from initial object loading and fetched from the database only when the getter is called. Use `GetPropertyValue<T>` / `SetPropertyValue` (without the `ref` backing field) for delayed properties.

## XPO Constructor Requirement

Every XPO persistent class **must** have a public constructor accepting `Session`:

```csharp
public class MyEntity : BaseObject {
    public MyEntity(Session session) : base(session) { }
}
```

Without this constructor, XPO cannot instantiate the object when loading from the database.

## XPO Default Values with AfterConstruction

Override `AfterConstruction()` to set default property values when a new XPO object is created. This is the XPO equivalent of `OnCreated()` in EF Core entities:

```csharp
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class Order : BaseObject {
    public Order(Session session) : base(session) { }

    public override void AfterConstruction() {
        base.AfterConstruction();
        SetPropertyValue(nameof(OrderDate), ref orderDate, DateTime.Today);
        SetPropertyValue(nameof(Status), ref status, OrderStatus.New);
    }

    private DateTime orderDate;
    public DateTime OrderDate {
        get => orderDate;
        set => SetPropertyValue(nameof(OrderDate), ref orderDate, value);
    }

    private OrderStatus status;
    public OrderStatus Status {
        get => status;
        set => SetPropertyValue(nameof(Status), ref status, value);
    }
}
```

> `AfterConstruction()` runs once when the object is first created — it does **not** run when an existing object is loaded from the database.
