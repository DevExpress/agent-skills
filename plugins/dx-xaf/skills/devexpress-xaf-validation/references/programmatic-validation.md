# Programmatic Validation — Code Snippets

## Obtaining IRuleSet

```csharp
using DevExpress.Persistent.Validation;

IRuleSet ruleSet = Validator.GetService(Application.ServiceProvider);
```

## Validate a Single Object (ValidateTarget)

`ValidateTarget` returns a `RuleSetValidationResult` without throwing — inspect the result to decide how to proceed.

```csharp
public class ValidateController : ViewController {
    private void ValidateCurrentObject() {
        IRuleSet ruleSet = Validator.GetService(Application.ServiceProvider);

        RuleSetValidationResult result = ruleSet.ValidateTarget(
            View.ObjectSpace, View.CurrentObject, DefaultContexts.Save);

        if (result.ValidationOutcome == ValidationOutcome.Error) {
            foreach (var item in result.Results) {
                if (item.State == ValidationState.Invalid) {
                    // item.Rule.Id — rule identifier
                    // item.ErrorMessage — human-readable error text
                }
            }
        }
    }
}
```

## Validate Multiple Objects (ValidateAllTargets)

`ValidateAllTargets` validates a collection of objects at once and returns an aggregated `RuleSetValidationResult`.

```csharp
IRuleSet ruleSet = Validator.GetService(Application.ServiceProvider);

var pendingInvoices = objectSpace.GetObjects<Invoice>(
    CriteriaOperator.FromLambda<Invoice>(i => i.Status == "Pending"));

RuleSetValidationResult result = ruleSet.ValidateAllTargets(
    objectSpace, pendingInvoices, "Export");

if (result.ValidationOutcome == ValidationOutcome.Error) {
    foreach (var item in result.Results) {
        if (item.State == ValidationState.Invalid) {
            // item.Rule.Id, item.ErrorMessage
            // item.Rule.UsedProperties — properties involved
        }
    }
}
```

## Validate and Throw (Validate)

`Validate` throws a `ValidationException` if any rule fails with `Error` state:

```csharp
// Throws ValidationException on failure
ruleSet.Validate(View.ObjectSpace, View.CurrentObject, DefaultContexts.Save);
```

## ValidationOutcome Enum

| Value | Meaning |
|-------|---------|
| `ValidationOutcome.Valid` | All rules passed |
| `ValidationOutcome.Error` | At least one rule failed with Error state |
| `ValidationOutcome.Warning` | At least one rule returned Warning; no Error |
| `ValidationOutcome.Information` | At least one rule returned Information; no Error/Warning |
| `ValidationOutcome.Skipped` | All rules were skipped (e.g., via `TargetCriteria` or `OnCustomNeedToValidateRule`) |

## Inspecting RuleSetValidationResult

```csharp
RuleSetValidationResult result = ruleSet.ValidateTarget(
    objectSpace, target, DefaultContexts.Save);

// Overall outcome
if (result.ValidationOutcome == ValidationOutcome.Error) {
    // At least one Error-level rule failed
}

// Per-rule results
foreach (var item in result.Results) {
    string ruleId = item.Rule.Id;
    ValidationState state = item.State;   // Valid, Invalid, Skipped
    string message = item.ErrorMessage;    // Per-rule error text
}
```
