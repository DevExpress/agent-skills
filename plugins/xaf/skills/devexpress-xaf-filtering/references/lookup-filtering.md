# Filtering — Lookup Property Filtering

Code snippets for DataSourcePropertyAttribute, DataSourceCriteriaAttribute, DataSourceCriteriaPropertyAttribute, and Current Object Parameter.

---

## DataSourcePropertyAttribute

Populates a Lookup with objects from a specific collection property:

```csharp
using DevExpress.Persistent.Base;

[DefaultClassOptions]
public class Order : BaseObject {
    public virtual Product Product { get; set; }

    // Lookup shows only accessories linked to the selected Product
    [DataSourceProperty("Product.Accessories")]
    public virtual Accessory Accessory { get; set; }
}
```

With fallback criteria when the source property is null:

```csharp
[DataSourceProperty("Product.Accessories",
    DataSourcePropertyIsNullMode.CustomCriteria, "IsGlobal = true")]
public virtual Accessory Accessory { get; set; }
```

## DataSourceCriteriaAttribute

Applies a static criteria string to filter the lookup:

```csharp
[DataSourceCriteria("IsGlobal = true")]
public virtual Accessory Accessory { get; set; }
```

## DataSourceCriteriaPropertyAttribute

Uses a property that returns a `CriteriaOperator` for dynamic lookup filtering:

```csharp
[DataSourceCriteriaProperty(nameof(AccessoryCriteria))]
public virtual Accessory Accessory { get; set; }

[Browsable(false)]
public CriteriaOperator AccessoryCriteria {
    get {
        return Product != null
            ? CriteriaOperator.FromLambda<Accessory>(
                a => a.Product.Oid == Product.ID)
            : null;
    }
}
```

## Current Object Parameter (@This)

Use `@This` in `DataSourceCriteria` to reference the current object's properties:

```csharp
// Show only Positions in the same Department as the Employee
[DataSourceCriteria("Department = '@This.Department'")]
public virtual Position Position { get; set; }
```

> **Limitation**: Current Object Parameter works only in Lookup Property Editor scenarios.
