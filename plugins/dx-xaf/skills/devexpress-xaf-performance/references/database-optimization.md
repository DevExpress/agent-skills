# Database Optimization — Code Snippets

## Indexing

Add indices for columns used in WHERE, ORDER BY, GROUP BY:

```csharp
// XPO
using DevExpress.Xpo;

[Indices("LastName;FirstName", "Department")]
public class Employee : BaseObject {
    [Indexed]
    public string Email {
        get => fEmail;
        set => SetPropertyValue(nameof(Email), ref fEmail, value);
    }
    string fEmail;
}
```

## Server-Side Filtering

For tables with >100K records, add a filter instead of loading all data:

```csharp
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;

public class LargeDataFilterController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        if (View.ObjectTypeInfo.Type == typeof(AuditLogEntry)) {
            View.CollectionSource.Criteria["RecentOnly"] =
                CriteriaOperator.Parse("CreatedOn > AddDays(Now(), -30)");
        }
    }
}
```

## Denormalization with Database Views

Map complex objects to database views for read-only scenarios:

```csharp
// XPO: Map to a database view
[Persistent("vw_OrderSummary")]
public class OrderSummary : XPLiteObject {
    [Key]
    public int OrderId {
        get => fOrderId;
        set => SetPropertyValue(nameof(OrderId), ref fOrderId, value);
    }
    int fOrderId;
    // Only columns from the view, no JOINs needed
}
```

```csharp
// EF Core: Map to a database view
using System.ComponentModel.DataAnnotations.Schema;

[Table("vw_OrderSummary")]
public class OrderSummary {
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    // Only columns from the view, no JOINs needed
}

// In DbContext.OnModelCreating:
protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<OrderSummary>().HasNoKey();
}
```
