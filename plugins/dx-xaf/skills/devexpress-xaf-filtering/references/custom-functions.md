# Filtering — Custom Function Criteria Operators

Code snippets for ICustomFunctionOperator registration and Web API / DI-aware custom functions.

---

## ICustomFunctionOperator — Basic Registration

Register custom functions for reuse in criteria throughout the application:

```csharp
using DevExpress.Data.Filtering;

public class WeekAgoOperator : ICustomFunctionOperator {
    static WeekAgoOperator() {
        var instance = new WeekAgoOperator();
        if (CriteriaOperator.GetCustomFunction(instance.Name) == null) {
            CriteriaOperator.RegisterCustomFunction(instance);
        }
    }
    public static void Register() { }

    public string Name => "WeekAgo";
    public object Evaluate(params object[] operands) => DateTime.Today.AddDays(-7);
    public Type ResultType(params Type[] operands) => typeof(DateTime);
}

// Register in module's static constructor:
public sealed partial class MyModule : ModuleBase {
    static MyModule() {
        WeekAgoOperator.Register();
    }
}

// Usage in criteria:
// "ShippingDate > WeekAgo()"
```

For server-side filtering, also implement `ICustomFunctionOperatorFormattable`.

## Web API / DI-Aware Custom Functions

For Web API services where `SecuritySystem.Instance` is unavailable, use `OnCustomizeSecurityCriteriaOperator`:

```csharp
builder.Security
    .UseIntegratedMode(options => {
        options.Events.OnCustomizeSecurityCriteriaOperator = context => {
            if (context.Operator is FunctionOperator functionOperator) {
                var funcName = (functionOperator.Operands[0] as ConstantValue)?.Value?.ToString();
                if ("CurrentOrgId".Equals(funcName, StringComparison.OrdinalIgnoreCase)) {
                    var user = (ApplicationUser)context.Security.User;
                    context.Result = new ConstantValue(user?.Organization?.Oid ?? Guid.NewGuid());
                }
            }
        };
    });
```

### Combining Criteria with GroupOperator.And

When adding an extra condition inside `OnCustomizeSecurityCriteriaOperator`, combine it with the existing criteria using `GroupOperator.And` — never replace the incoming criteria outright:

```csharp
options.Events.OnCustomizeSecurityCriteriaOperator = context => {
    // Add a tenant filter alongside the existing security criteria
    if (context.ObjectType == typeof(Order)) {
        var user = (ApplicationUser)context.Security.User;
        var tenantFilter = CriteriaOperator.FromLambda<Order>(
            o => o.TenantId == user.TenantId);

        // AND-combine with the existing criteria to preserve security rules
        context.Result = GroupOperator.And(context.Criteria, tenantFilter);
    }
};
```

> Always AND-combine with `context.Criteria` rather than replacing it. Replacing discards existing security criteria and can open data access to unauthorized records.
