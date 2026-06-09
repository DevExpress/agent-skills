# Retro — 2026-05-15 — data-binding-expression

**Task summary:** Add data binding to an existing `XtraReport` subclass, then (on user correction) rewrite to the recommended expression-based API.

---

## Mistake 1

### Task context
The user asked to bind the existing `SampleReport` to a sample array of objects.

### What the skill said (or didn't say)
The Quick Start example and Pattern 2 both show `ExpressionBindings` as the only binding form:

```csharp
// Quick Start
var nameLabel = new XRLabel {
    ExpressionBindings = {
        new ExpressionBinding("Text", "[ProductName]")
    }, ...
};

// Pattern 2
label.ExpressionBindings.Add(new ExpressionBinding("Text", "[UnitPrice] * [Quantity]"));
```

The skill does **not** mention `DataBindings` at all — neither as a deprecated API nor as something to avoid.

### What you did wrong
Generated data-bound labels using the legacy `DataBindings` API:

```csharp
idLabel.DataBindings.Add("Text", null, "Id");
nameLabel.DataBindings.Add("Text", null, "Name");
```

This forced a follow-up correction request from the user ("It is a legacy data binding mode").

### Why you made the mistake
The skill was **silent** on `DataBindings`. Without an explicit prohibition the model fell back on its general training knowledge, which includes the older `DataBindings` API. The skill shows only `ExpressionBindings` but never flags `DataBindings` as wrong.

### What the correct behavior should have been
Generate expression bindings from the start:

```csharp
idLabel.ExpressionBindings.Add(new ExpressionBinding("Text", "[Id]"));
```

### Proposed skill fix
**New rule** — add to the *Constraints & Rules* section:

> **9. Never use `DataBindings`**: `DataBindings` is the legacy binding mode and is deprecated. Always use `ExpressionBindings` with `ExpressionBinding(propertyName, expression)`. Generating `DataBindings.Add(...)` is always wrong regardless of DevExpress version targeted.

Also add a troubleshooting row:

| Symptom | Cause | Fix |
|---------|-------|-----|
| Code uses `DataBindings.Add("Text", null, "Field")` | Legacy binding API applied | Replace with `ExpressionBindings.Add(new ExpressionBinding("Text", "[Field]"))` |

---

## Mistake 2

### Task context
The user asked to rewrite the `DataBindings` calls to use expressions instead.

### What the skill said (or didn't say)
Both the Quick Start example and Pattern 2 use the **2-argument** constructor:

```csharp
new ExpressionBinding("Text", "[ProductName]")
```

The skill shows no usage of the 3-argument constructor.

### What you did wrong
Generated the 3-argument form `ExpressionBinding("BeforePrint", "Text", "[Field]")`:

```csharp
idLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Id]"));
```

While this compiles and runs (the `"BeforePrint"` event is the default when the 2-arg overload is used), it differs from the canonical form shown by the skill and adds unnecessary noise.

### Why you made the mistake
The skill showed only the 2-arg form but did not state that it is the **preferred** or **only** form to use. General training data contains both overloads; without a constraint, the model chose the more explicit 3-arg form.

### What the correct behavior should have been
Use the 2-argument constructor consistently:

```csharp
idLabel.ExpressionBindings.Add(new ExpressionBinding("Text", "[Id]"));
```

### Proposed skill fix
**Clarification** — add a note directly under Pattern 2:

```markdown
**Pattern 2 — Expression binding:**
```csharp
label.ExpressionBindings.Add(new ExpressionBinding("Text", "[UnitPrice] * [Quantity]"));
```
> **Always use the 2-argument constructor** `ExpressionBinding(propertyName, expression)`.  
> The 3-argument overload `ExpressionBinding(eventName, propertyName, expression)` exists but is not needed; the default event (`BeforePrint`) is applied automatically by the 2-argument form.
```
