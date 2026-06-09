# Custom Validation Context — Code Snippets

## Defining a Custom Context

Custom validation contexts do **not** fire automatically. They must be triggered programmatically via `IRuleSet.ValidateTarget`.

```csharp
// 1. Define a context identifier as a constant
public static class ValidationContexts {
    public const string Export = "Export";
    public const string ChangePassword = "ChangePassword";
}

// 2. Apply the custom context to a rule attribute
[RuleRegularExpression("PasswordComplexity", "ChangePassword",
    @"^(?=.*[a-zA-Z])(?=.*\d).{6,}$",
    CustomMessageTemplate = "Password must contain letters and digits, minimum 6 characters.")]
public virtual string NewPassword { get; set; }

[RuleRequiredField("ExportTitleRequired", "Export",
    CustomMessageTemplate = "Title is required for export.")]
public virtual string Title { get; set; }
```

## Triggering a Custom Context Programmatically

```csharp
using DevExpress.Persistent.Validation;

// In a controller or action handler:
IRuleSet ruleSet = Validator.GetService(Application.ServiceProvider);

// Single object
RuleSetValidationResult result = ruleSet.ValidateTarget(
    objectSpace, currentObject, ValidationContexts.Export);

if (result.ValidationOutcome > ValidationOutcome.Information) {
    // Handle validation failure — errors or warnings present
    foreach (var item in result.Results) {
        if (item.State == ValidationState.Invalid) {
            // item.Rule.Id, item.ErrorMessage
        }
    }
}
```

## Multi-Context Rules

A single rule can fire in multiple contexts. Use semicolon-separated context identifiers:

```csharp
// Fires on both Save and Export
[RuleRequiredField("NameRequired",
    DefaultContexts.Save + ";Export")]
public virtual string Name { get; set; }
```

The `Save` context fires automatically during the Save action. The `Export` context fires only when triggered by code.

## Associating a Custom Context with an Action

To trigger validation when an action executes, call `ValidateTarget` in the action's `Execute` handler:

```csharp
public class ExportController : ViewController {
    public ExportController() {
        var exportAction = new SimpleAction(this, "ExportDocument", PredefinedCategory.Export);
        exportAction.Execute += (s, e) => {
            IRuleSet ruleSet = Validator.GetService(Application.ServiceProvider);
            RuleSetValidationResult result = ruleSet.ValidateTarget(
                View.ObjectSpace, View.CurrentObject, ValidationContexts.Export);

            if (result.ValidationOutcome == ValidationOutcome.Error) {
                throw new ValidationException(result);
            }

            // Proceed with export...
        };
    }
}
```
