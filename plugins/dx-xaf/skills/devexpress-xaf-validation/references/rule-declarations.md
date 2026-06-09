# Rule Declarations — Code Snippets

## All 11 Built-in Rule Attributes

Every rule attribute requires a unique **rule ID** (first string parameter). The rule ID identifies the rule in validation results, events, and troubleshooting. It must be unique within the module.

```csharp
using DevExpress.Persistent.Validation;

// --- RuleRequiredField: property must have a non-null/non-empty value ---
[RuleRequiredField("CustomerNameRequired", DefaultContexts.Save,
    CustomMessageTemplate = "Customer name is required.")]
public virtual string Name { get; set; }

// --- RuleCriteria: object must satisfy a criteria expression (class-level or property-level) ---
[RuleCriteria("EndDateAfterStart", DefaultContexts.Save,
    "EndDate > StartDate",
    CustomMessageTemplate = "End date must be after start date.")]
public class Project : BaseObject {
    public virtual DateTime StartDate { get; set; }
    public virtual DateTime EndDate { get; set; }
}

// --- RuleRange: value must be within min/max inclusive bounds ---
[RuleRange("DiscountRange", DefaultContexts.Save, 0, 100)]
public virtual int DiscountPercentage { get; set; }

// --- RuleRegularExpression: value must match a .NET regex pattern ---
[RuleRegularExpression("EmailFormat", DefaultContexts.Save,
    @"^[\w.-]+@[\w.-]+\.\w+$")]
public virtual string Email { get; set; }

// --- RuleValueComparison: value compared against a constant ---
[RuleValueComparison("PositiveAmount", DefaultContexts.Save,
    ValueComparisonType.GreaterThan, 0)]
public virtual decimal Amount { get; set; }

// --- RuleStringComparison: string compared against a constant ---
[RuleStringComparison("CodeStartsWithA", DefaultContexts.Save,
    StringComparisonType.StartsWith, "A")]
public virtual string Code { get; set; }

// --- RuleUniqueValue: value must be unique across all persisted objects ---
// NOTE: Executes a database query. New unsaved objects in the same session
// are not visible to this check. Add a DB unique index alongside this rule.
[RuleUniqueValue("UniqueEmployeeCode", DefaultContexts.Save,
    CustomMessageTemplate = "Employee code must be unique.")]
public virtual string EmployeeCode { get; set; }

// --- RuleCombinationOfPropertiesIsUnique: combination of properties must be unique ---
[RuleCombinationOfPropertiesIsUnique("UniqueDeptAndTitle", DefaultContexts.Save,
    "Department;Title")]
public class Position : BaseObject {
    public virtual string Department { get; set; }
    public virtual string Title { get; set; }
}

// --- RuleFromBoolProperty: boolean property must be true ---
[RuleFromBoolProperty("AcceptedTerms", DefaultContexts.Save,
    CustomMessageTemplate = "You must accept the terms and conditions.")]
public virtual bool IsTermsAccepted { get; set; }

// --- RuleIsReferenced: object must be referenced (Delete context) ---
// Prevents deleting a Department that still has Employees referencing it.
[RuleIsReferenced("DepartmentHasEmployees", DefaultContexts.Delete,
    typeof(Employee), "Department",
    MessageTemplateMustBeReferenced = "Cannot delete: employees reference this department.")]
public class Department : BaseObject {
    public virtual string Name { get; set; }
}

// --- RuleObjectExists: at least one object matching criteria must exist ---
[RuleObjectExists("ManagerMustExist", DefaultContexts.Save,
    "IsManager = true",
    MessageTemplateMustExist = "At least one manager must exist.")]
public class Team : BaseObject {
    public virtual string TeamName { get; set; }
}
```

## Declaring Rules on Business Classes

```csharp
using DevExpress.Persistent.Validation;

[DefaultClassOptions]
[RuleCriteria("IncidentMustHaveAssignee", DefaultContexts.Save,
    "AssignedTo is not null", SkipNullOrEmptyValues = false)]
public class Incident : BaseObject {

    [RuleRequiredField("SubjectRequired", DefaultContexts.Save)]
    public virtual string Subject { get; set; }

    public virtual Person AssignedTo { get; set; }

    [RuleRange("PriorityRange", DefaultContexts.Save, 1, 5)]
    public virtual int Priority { get; set; }

    [RuleRegularExpression("EmailFormat", DefaultContexts.Save,
        @"^[\w.-]+@[\w.-]+\.\w+$")]
    public virtual string ContactEmail { get; set; }
}
```

## Conditional Validation (TargetCriteria)

Validate only when a condition is met:

```csharp
public class Contact : BaseObject {
    public virtual bool IsMarried { get; set; }

    // Single condition
    [RuleRequiredField("SpouseRequired", DefaultContexts.Save,
        TargetCriteria = "[IsMarried] = true")]
    public virtual string SpouseName { get; set; }

    // Multi-condition with CriteriaOperator syntax
    [RuleRequiredField("VATNumberRequired", DefaultContexts.Save,
        TargetCriteria = "CountryCode = 'DE' Or CountryCode = 'AT'")]
    public virtual string VATNumber { get; set; }

    public virtual string CountryCode { get; set; }

    // Function operators in TargetCriteria
    [RuleRequiredField("ReviewRequired", DefaultContexts.Save,
        TargetCriteria = "CreatedDate < AddDays(LocalDateTimeToday(), -7)")]
    public virtual string ReviewedBy { get; set; }

    public virtual DateTime CreatedDate { get; set; }
}
```

When `TargetCriteria` evaluates to `false`, the rule is skipped entirely — it is not counted as a pass, it is simply not evaluated.

## Multi-Context Rules

Apply one rule to multiple contexts using semicolon-separated context identifiers:

```csharp
[RuleRequiredField("NameRequiredSaveAndExport",
    DefaultContexts.Save + ";Export")]
public virtual string Name { get; set; }
```

Both contexts evaluate the same rule independently. The custom context (`"Export"`) must still be triggered programmatically.

## Soft Validation (Warnings and Information)

### ValidationResultType Enum

| Value | Blocking? | Behavior |
|-------|-----------|----------|
| `ValidationResultType.Error` | Yes | Blocks Save/Delete; user must fix before proceeding |
| `ValidationResultType.Warning` | No | Displayed in validation UI but Save proceeds |
| `ValidationResultType.Information` | No | Informational message only; Save proceeds |

```csharp
// Warning: advisory, does not block save
[RuleRequiredField("RecommendedNotes", DefaultContexts.Save,
    CustomMessageTemplate = "Notes are recommended but not required.",
    ResultType = ValidationResultType.Warning)]
public virtual string Notes { get; set; }

// Information: purely informational
[RuleRequiredField("OptionalBio", DefaultContexts.Save,
    CustomMessageTemplate = "Consider adding a biography.",
    ResultType = ValidationResultType.Information)]
public virtual string Biography { get; set; }
```

A `Warning` result does **not** prevent the Save action from completing. Only `Error` blocks save.
