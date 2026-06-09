# Customizing Validation Behavior — Code Snippets

## ValidationOptions.Events in Startup

Subscribe to events on `ValidationOptions.Events` inside `AddValidation` to customize rule behavior at application startup.

```csharp
// In Startup.cs / Program.cs
builder.Modules.AddValidation(options => {

    // OnCustomNeedToValidateRule: skip a rule conditionally
    // Fires for every rule during validation — check rule ID before suppressing.
    options.Events.OnCustomNeedToValidateRule += context => {
        if (context.Rule.Id == "SkippableRule" &&
            context.TargetObject is Order order &&
            order.Status == "Draft") {
            context.NeedToValidateRule = false;
        }
    };

    // OnCustomValidateRule: override validation logic for a specific rule
    options.Events.OnCustomValidateRule += context => {
        if (context.Rule.Id == "CustomLogicRule") {
            context.RuleValidationResult = new RuleValidationResult(
                context.Rule, ValidationState.Valid, "Overridden: OK");
        }
    };

    // OnRuleValidated: fires after each individual rule has been evaluated
    options.Events.OnRuleValidated += context => {
        if (context.RuleValidationResult.State == ValidationState.Invalid) {
            // Log, audit, or adjust the result after evaluation
        }
    };

    // OnValidationCompleted: fires after all rules for a given context have been evaluated
    options.Events.OnValidationCompleted += context => {
        // context.ValidationOutcome contains the aggregated result
        if (context.ValidationOutcome == ValidationOutcome.Error) {
            // Log or notify
        }
    };
});
```

## Event Summary

| Event | When It Fires | Typical Use |
|-------|---------------|-------------|
| `OnCustomNeedToValidateRule` | Before each rule is evaluated | Skip rules conditionally |
| `OnCustomValidateRule` | During rule evaluation | Override validation result |
| `OnRuleValidated` | After each rule is evaluated | Logging, auditing, adjusting results |
| `OnValidationCompleted` | After all rules for a context finish | Aggregated logging, notifications |
