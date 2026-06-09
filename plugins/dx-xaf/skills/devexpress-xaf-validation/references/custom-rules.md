# Custom Rule Implementation — Code Snippets

## Inherit from RuleBase&lt;T&gt;

`RuleBase<T>` is the generic base class for custom validation rules. The type parameter `T` restricts which object types the rule can validate.

```csharp
using DevExpress.Persistent.Validation;

public class RuleInvoiceTotalMatchesItems : RuleBase<Invoice> {
    // Constructor accepts IRuleBaseProperties
    public RuleInvoiceTotalMatchesItems(IRuleBaseProperties properties)
        : base(properties) {
    }

    // Override IsValidInternal with the custom validation logic
    protected override bool IsValidInternal(
        Invoice target, out string errorMessageTemplate) {

        decimal itemSum = target.Items?.Sum(i => i.LineTotal) ?? 0;

        if (target.TotalAmount != itemSum) {
            errorMessageTemplate =
                $"Total ({target.TotalAmount}) does not match sum of line items ({itemSum}).";
            return false;
        }

        errorMessageTemplate = string.Empty;
        return true;
    }
}
```

Set `Properties.CustomMessageTemplate` in the constructor to override the default error message:

```csharp
public RuleInvoiceTotalMatchesItems(IRuleBaseProperties properties)
    : base(properties) {
    Properties.CustomMessageTemplate = "Invoice total must equal the sum of its line items.";
}
```

## Implement IRuleSource for Attribute-Driven Rules

`IRuleSource` allows injecting rules programmatically without decorating properties with built-in rule attributes.

```csharp
using DevExpress.Persistent.Validation;

public class MaxCollectionCountAttribute : RuleBaseAttribute, IRuleSource {
    public int MaxCount { get; set; } = 10;

    // IRuleSource.Name identifies this rule source
    public string Name => "MaxCollectionCountSource";

    // IRuleSource.CreateRules returns rules to register
    public ICollection<IRule> CreateRules(IRuleBaseProperties properties) {
        var rule = new RuleMaxCollectionCount(properties) {
            MaxCount = this.MaxCount
        };
        return new IRule[] { rule };
    }
}
```

Classes implementing `IRuleSource` on a business class (as an attribute) are auto-discovered by XAF. No explicit registration in `ModuleBase` is needed.

## Custom Rule Example: Collection Count

```csharp
public class RuleMaxCollectionCount : RuleBase<IList> {
    public int MaxCount { get; set; } = 10;

    public RuleMaxCollectionCount(IRuleBaseProperties properties)
        : base(properties) {
    }

    protected override bool IsValidInternal(
        IList target, out string errorMessageTemplate) {
        errorMessageTemplate = $"Collection exceeds maximum count of {MaxCount}.";
        return target == null || target.Count <= MaxCount;
    }
}
```
