# Filtering — CriteriaOperator Syntax

Lambda-based, string-based, and programmatic criteria construction, plus a quick reference table.

---

## Lambda-Based Criteria (CriteriaOperator.FromLambda) — Preferred

Compile-time-safe — renaming a property causes a build error instead of a runtime failure:

```csharp
using DevExpress.Data.Filtering;

// Simple AND condition
CriteriaOperator criteria = CriteriaOperator.FromLambda<Contact>(
    c => c.Country == "France" && c.Age > 30);

// Nested property path
CriteriaOperator criteria = CriteriaOperator.FromLambda<Contact>(
    c => c.Department.Name == "Sales");

// Null check
CriteriaOperator criteria = CriteriaOperator.FromLambda<Contact>(
    c => c.Region == null);

// String method
CriteriaOperator criteria = CriteriaOperator.FromLambda<Contact>(
    c => c.Name.StartsWith("A"));
```

> **Default choice**: Use `FromLambda<T>` whenever property names are known at compile time.

## String-Based Criteria (CriteriaOperator.Parse)

Use only when criteria come from configuration, user input, or require syntax not supported by `FromLambda` (e.g., XAF function operators like `CurrentUserId()`, collection element conditions, `Between`, `In`):

```csharp
using DevExpress.Data.Filtering;

// Simple comparison
CriteriaOperator criteria = CriteriaOperator.Parse("City != 'Chicago'");

// Parameterized (required for user-supplied values)
CriteriaOperator criteria = CriteriaOperator.Parse("Country = ? And Age > ?", "France", 30);

// DateTime with function operators
CriteriaOperator criteria = CriteriaOperator.Parse(
    "[DueDate] >= ADDDAYS(LocalDateTimeToday(), -7)");

// Nested property path
CriteriaOperator criteria = CriteriaOperator.Parse("Department.Name = 'Sales'");

// Collection element check — returns true if any child matches
CriteriaOperator criteria = CriteriaOperator.Parse("[Orders][[Amount] > 1000]");

// In operator
CriteriaOperator criteria = CriteriaOperator.Parse("[Status] In (1, 2, 3)");

// Between
CriteriaOperator criteria = CriteriaOperator.Parse("[Price] Between (10, 100)");
```

> **Security**: When using `Parse` with user-supplied values, always use parameterized criteria (`?` placeholders). Never concatenate user input into criteria strings.

## Programmatic Criteria (Operator Classes)

```csharp
using DevExpress.Data.Filtering;

// BinaryOperator — single comparison
var criteria = new BinaryOperator("City", "Chicago", BinaryOperatorType.NotEqual);

// GroupOperator — AND/OR combinations
var criteria = new GroupOperator(GroupOperatorType.And,
    new BinaryOperator("Country", "France"),
    new BinaryOperator("Age", 30, BinaryOperatorType.Greater));

// FunctionOperator — built-in functions
var criteria = new FunctionOperator(FunctionOperatorType.StartsWith,
    new OperandProperty("Name"), new OperandValue("A"));

// UnaryOperator — NOT, IsNull
var criteria = new UnaryOperator(UnaryOperatorType.IsNull, "Region");

// ContainsOperator — collection element check
var criteria = new ContainsOperator("Orders",
    new BinaryOperator("Amount", 1000, BinaryOperatorType.Greater));

// InOperator
var criteria = new InOperator("Status", new[] { 1, 2, 3 });

// BetweenOperator
var criteria = new BetweenOperator("Price", 10, 100);
```

## Criteria Language Syntax Quick Reference

| Syntax | Example |
|--------|---------|
| Comparison | `[Price] > 100`, `[Name] = 'John'` |
| Logical | `[A] And [B]`, `[A] Or [B]`, `Not [A]` |
| Null check | `[Region] Is Null`, `IsNull([Region])` |
| String | `Contains([Name], 'dev')`, `StartsWith([Name], 'A')`, `Like([Name], 'Jo%')` |
| DateTime | `LocalDateTimeToday()`, `ADDDAYS(LocalDateTimeToday(), -7)`, `GetDate([OrderDate])` |
| Aggregate | `[Items].Sum([Amount])`, `[Items].Count()`, `[Items].Max([Price])` |
| Between | `[Qty] Between (10, 20)` |
| In | `[Country] In ('USA', 'UK')` |
| Parameters | `[Name] = ?` with value array |
| Concat | `Concat([First], ' ', [Last])` |
| Iif | `Iif([Price] > 100, 'High', 'Low')` |
