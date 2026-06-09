# XPO Filtering — Programmatic Criteria Construction

Code snippets for building criteria with BinaryOperator, GroupOperator, FunctionOperator, ContainsOperator, InOperator, BetweenOperator, and AggregateOperand.

---

## Building Criteria Programmatically

```csharp
using DevExpress.Data.Filtering;

// BinaryOperator — single comparison
var byCity = new BinaryOperator("City", "Chicago", BinaryOperatorType.Equal);

// GroupOperator — combine with AND
var combined = new GroupOperator(GroupOperatorType.And,
    new BinaryOperator("City", "Chicago"),
    new BinaryOperator("Age", 30, BinaryOperatorType.GreaterOrEqual));

// GroupOperator — combine with OR
var either = new GroupOperator(GroupOperatorType.Or,
    new BinaryOperator("Status", "Active"),
    new BinaryOperator("Status", "Pending"));

// FunctionOperator — use built-in functions
var startsWithA = new FunctionOperator(FunctionOperatorType.StartsWith,
    new OperandProperty("LastName"), new OperandValue("A"));

var todayOrLater = new BinaryOperator(
    new OperandProperty("DueDate"),
    new FunctionOperator(FunctionOperatorType.LocalDateTimeToday),
    BinaryOperatorType.GreaterOrEqual);

// ContainsOperator — check collection elements
var hasLargeOrder = new ContainsOperator("Orders",
    new BinaryOperator("Amount", 10000, BinaryOperatorType.Greater));

// InOperator — value in a set
var inCountries = new InOperator("Country",
    new OperandValue[] {
        new OperandValue("USA"),
        new OperandValue("UK"),
        new OperandValue("Germany")
    });

// BetweenOperator
var priceRange = new BetweenOperator("Price", 10m, 100m);

// Nested property path
var byDeptName = new BinaryOperator("Department.Name", "Sales");

// AggregateOperand — aggregate over a collection
// Constructor: new AggregateOperand(collectionProperty, aggregatedExpression, aggregateType, filterCriteria)
var totalOver1000 = new AggregateOperand(
    "OrderItems", "Amount",
    Aggregate.Sum,
    new BinaryOperator("IsActive", true));
```

**`Aggregate` enum values:**

| Value | Description |
|-------|-------------|
| `Count` | Number of matching objects (aggregatedExpression is ignored — pass `""` or `null`) |
| `Sum` | Sum of the aggregated expression values |
| `Min` | Minimum value of the aggregated expression |
| `Max` | Maximum value of the aggregated expression |
| `Avg` | Average value of the aggregated expression |
| `Exists` | Returns `true` if at least one matching object exists (no value computed) |
| `Single` | Returns the single matching object; throws if zero or more than one match |
| `Custom` | Delegates to a registered custom aggregate function |

```csharp

// AggregateOperand — Aggregate.Count with a date-range condition and threshold
// Find customers with more than 5 orders in the last 30 days:
var dateCriteria = new BinaryOperator(
    "OrderDate", DateTime.Today.AddDays(-30), BinaryOperatorType.GreaterOrEqual);
var recentOrderCount = new AggregateOperand(
    "Orders", "", Aggregate.Count, dateCriteria);
var threshold = new BinaryOperator(
    recentOrderCount, new OperandValue(5), BinaryOperatorType.Greater);
// threshold alone is the complete filter; combine with other criteria via GroupOperator:
var activeCriteria = new BinaryOperator("IsActive", true);
var finalFilter = GroupOperator.And(activeCriteria, threshold);

// Combine string-parsed and programmatic criteria
var parsed = CriteriaOperator.Parse("[Status] = ?", "Active");
var finalCriteria = GroupOperator.And(parsed, byCity);
```

## JoinOperand — Cross-Type Aggregates

Use `JoinOperand` to aggregate or filter across a persistent type that has **no XPO `[Association]`** with the parent type. The `^.` (caret-dot) prefix references the parent object's properties inside the join condition:

```csharp
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

// Find employees who have a matching User record with "Issue Refunds" permission
// (Employee and User are separate types — no [Association] between them)
var joinCondition = new BinaryOperator(
    new OperandProperty("Oid"),
    new OperandProperty("^.Oid"),
    BinaryOperatorType.Equal);
var permissionFilter = new ContainsOperator(
    new OperandProperty("Permissions"),
    new BinaryOperator("Action", "Issue Refunds"));

var filter = new JoinOperand(
    joinTypeName: "User",
    condition: GroupOperator.And(joinCondition, permissionFilter));

var employees = new XPCollection<Employee>(Session, filter);

// Aggregate example — count AuditLogEntry records per object (no association):
var auditCount = new JoinOperand(
    joinTypeName: "AuditLogEntry",
    condition: new BinaryOperator("ObjectOid", new OperandProperty("^.Oid")),
    aggregateType: Aggregate.Count,
    aggregatedExpression: null);
var hasAudits = new BinaryOperator(auditCount, new OperandValue(0), BinaryOperatorType.Greater);
```

> **`JoinOperand` vs `AggregateOperand`:** Prefer `AggregateOperand` when filtering or aggregating over an associated collection (e.g., `"Orders"` on a Customer with `[Association]`). Use `JoinOperand` for cross-type queries where no XPO association exists — it generates a SQL JOIN on an arbitrary condition. `JoinOperand` works only with XPO persistent objects.

## GroupOperator Helpers

```csharp
// Static helpers for combining criteria
var combined = GroupOperator.And(criteria1, criteria2, criteria3);
var either = GroupOperator.Or(criteria1, criteria2);

// Combine nullable criteria safely (skips null operands)
var criteria = CriteriaOperator.And(maybeCriteria1, maybeCriteria2);
var criteria = CriteriaOperator.Or(maybeCriteria1, maybeCriteria2);
```

> `CriteriaOperator.And()` / `CriteriaOperator.Or()` with **zero** non-null operands returns `null`; with **one** non-null operand returns that operand unchanged. This makes them safe for conditional criteria composition — you can pass a variable number of optional filters without special-casing empty or single-element lists.
