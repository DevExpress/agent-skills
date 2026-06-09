# Getting Current User & Checking Permissions — Code Snippets

## Getting the Current User

### In a Controller

```csharp
using DevExpress.ExpressApp.Security;

public class MyController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        var currentUser = SecuritySystem.CurrentUser as ApplicationUser;

        // For updates, use an ObjectSpace-tracked instance
        var trackedUser = ObjectSpace.GetObjectByKey<ApplicationUser>(
            (Guid)SecuritySystem.CurrentUserId);

        ISecurityStrategyBase security = Application.GetSecurityStrategy();
        var userId = security.UserId;
        var currentUserViaSecurity = ObjectSpace.GetObjectByKey<ApplicationUser>((Guid)userId);
    }
}
```

### Via Dependency Injection (Blazor / Web API)

```csharp
// In a controller, middleware, or service:
public class MyService {
    private readonly ISecurityProvider securityProvider;
    private readonly ISecurityStrategyBase security;

    public MyService(ISecurityProvider securityProvider, ISecurityStrategyBase security) {
        this.securityProvider = securityProvider;
        this.security = security;
    }

    public void DoWork() {
        var strategy = securityProvider.GetSecurity();
        var user = (ISecurityUserWithRoles)security.User;
        bool isAdmin = user.IsUserInRole("Administrators");
    }
}
```

### In Criteria (Filtering)

```csharp
// CurrentUserId() function operator:
CriteriaOperator.Parse("[CreatedBy.ID] = CurrentUserId()");

// IsCurrentUserInRole() function:
CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')");
```

## Checking Permissions in Code

Use `IsGrantedExtensions` methods:

```csharp
ISecurityStrategyBase security = Application.GetSecurityStrategy();

// Check type-level permissions
bool canRead = security.CanRead<Contact>();
bool canCreate = security.CanCreate<Contact>();
bool canWrite = security.CanWrite<Contact>();
bool canDelete = security.CanDelete<Contact>();

// Equivalent explicit checks with SecurityOperations constants
bool canCreateInvoice = security.IsGranted(
    new PermissionRequest(typeof(Invoice), SecurityOperations.Create));
bool canReadInvoice = security.IsGranted(
    new PermissionRequest(typeof(Invoice), SecurityOperations.Read));

// Check member-level permissions
bool canEditName = security.CanWrite<Contact>(ObjectSpace, nameof(Contact.Name));

// Check object-level permissions
bool canReadObj = security.CanRead(contactInstance);

// Check role membership
var user = (ISecurityUserWithRoles)security.User;
bool isManager = user.IsUserInRole("Manager");
```

Pre-check permissions before executing actions to avoid `SecurityException`. If you call operations directly without checks, XAF can throw when access is denied.
