# Predefined Users, Roles & Permissions — Code Snippets

In `Updater.cs` (`ModuleUpdater.UpdateDatabaseAfterUpdateSchema`):

## Administrative Role and User

```csharp
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;

public override void UpdateDatabaseAfterUpdateSchema() {
    base.UpdateDatabaseAfterUpdateSchema();

    // Create admin role first
    var adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(
        r => r.Name == "Administrators");
    if (adminRole == null) {
        adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
        adminRole.Name = "Administrators";
        adminRole.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;
        adminRole.AddTypePermission<PermissionPolicyRole>(
            SecurityOperations.FullAccess,
            SecurityPermissionState.Allow);
        adminRole.IsAdministrative = true;
    }

    // Create admin user
    var userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(
        u => u.UserName == "Admin");
    if (userAdmin == null) {
        userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
        userAdmin.UserName = "Admin";
        // Set an empty password for the initial seed.
        // In production, source initial passwords from secure configuration.
        userAdmin.SetPassword("");
        userAdmin.ChangePasswordOnFirstLogon = true;
        ObjectSpace.CommitChanges();
        ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(
            SecurityDefaults.PasswordAuthentication,
            ObjectSpace.GetKeyValueAsString(userAdmin));
    }

    userAdmin.Roles.Add(adminRole);
    ObjectSpace.CommitChanges();
}
```

Use `FirstOrDefault` (or `FindObject`) guards for both users and roles because the updater runs on every startup.

## Non-Administrative Role with Permissions

```csharp
private PermissionPolicyRole CreateDefaultRole() {
    var defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(
        r => r.Name == "Default");
    if (defaultRole == null) {
        defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
        defaultRole.Name = "Default";

        // Secure baseline: deny until granted
        defaultRole.PermissionPolicy = SecurityPermissionPolicy.DenyAllByDefault;

        // Type permissions
        defaultRole.AddTypePermission<Contact>(
            SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

        // Explicit deny example
        defaultRole.AddTypePermission<Contact>(
            SecurityOperations.Delete, SecurityPermissionState.Deny);

        // Object permission (lambda)
        defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(
            SecurityOperations.Read,
            u => u.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
            SecurityPermissionState.Allow);

        // Object permission (criteria string)
        defaultRole.AddObjectPermission<ApplicationUser>(
            SecurityOperations.Read,
            "[ID] = CurrentUserId()",
            SecurityPermissionState.Allow);

        // Member permission (semicolon-delimited member names)
        defaultRole.AddMemberPermission<ApplicationUser>(
            SecurityOperations.ReadWriteAccess,
            "ChangePasswordOnFirstLogon",
            null,
            SecurityPermissionState.Allow);

        // Member permission lambda variant
        defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(
            SecurityOperations.Read,
            nameof(ApplicationUser.UserName),
            u => u.UserName != null,
            SecurityPermissionState.Allow);

        // Navigation permission
        defaultRole.AddNavigationPermission(
            @"Application/NavigationItems/Items/Default/Items/MyDetails",
            SecurityPermissionState.Allow);

        ObjectSpace.CommitChanges();
    }
    return defaultRole;
}
```
