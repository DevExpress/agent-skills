# Permission API Methods — Code Snippets

All methods are on `PermissionSettingHelper` (extension methods for `IPermissionPolicyRole`):

```csharp
// Type permissions — apply to all objects of type T
role.AddTypePermission<Contact>(SecurityOperations.Read, SecurityPermissionState.Allow);
role.SetTypePermission<Contact>(SecurityOperations.Read, SecurityPermissionState.Deny);

// Object permissions — apply to objects matching criteria
role.AddObjectPermission<Contact>(
    SecurityOperations.Read,
    "[Department.Title] = 'Development'",  // string criteria
    SecurityPermissionState.Allow);

role.AddObjectPermissionFromLambda<Contact>(
    SecurityOperations.Read,
    c => c.Department.Title.Contains("Development"),  // lambda criteria
    SecurityPermissionState.Allow);

// Member permissions — apply to specific properties
role.AddMemberPermission<Employee>(
    SecurityOperations.Write,
    "LastName;FirstName",  // semicolon-separated member names
    "[Department.Title] = 'Development'",  // criteria (null for all objects)
    SecurityPermissionState.Deny);

role.AddMemberPermissionFromLambda<Employee>(
    SecurityOperations.Write,
    nameof(Employee.LastName),
    e => e.Department.Title.Contains("Development"),
    SecurityPermissionState.Deny);

// Navigation permissions
role.AddNavigationPermission(
    @"Application/NavigationItems/Items/Default/Items/Contact_ListView",
    SecurityPermissionState.Allow);
```

## SecurityPermissionState

Use `SecurityPermissionState.Allow` to grant an operation and `SecurityPermissionState.Deny` to block an operation.

```csharp
role.AddTypePermission<Order>(
    SecurityOperations.Read,
    SecurityPermissionState.Allow);

role.AddTypePermission<Order>(
    SecurityOperations.Delete,
    SecurityPermissionState.Deny);
```
