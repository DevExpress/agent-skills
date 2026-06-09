# CreatedBy / UpdatedBy Owner Pattern — Code Snippets

## Business Object with Owner Tracking

```csharp
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.EFCore;
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.Extensions.DependencyInjection;

public class Document : BaseObject {
    public virtual string Title { get; set; }

    [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "False")]
    public virtual ApplicationUser CreatedBy { get; set; }

    [ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "False")]
    public virtual ApplicationUser UpdatedBy { get; set; }

    ApplicationUser GetCurrentUser() {
        return ObjectSpace.GetObjectByKey<ApplicationUser>(
            ObjectSpace.ServiceProvider.GetRequiredService<ISecurityStrategyBase>().UserId);
    }

    public override void OnSaving() {
        base.OnSaving();
        if (ObjectSpace.IsNewObject(this)) {
            // Available for EF Core and XPO BaseObject descendants
            SetPropertyValueWithSecurityBypass(nameof(CreatedBy), GetCurrentUser());
        } else {
            SetPropertyValueWithSecurityBypass(nameof(UpdatedBy), GetCurrentUser());
        }
    }
}
```

For non-`BaseObject` EF Core entities, use the static helper:

```csharp
SecuredPropertySetter.SetPropertyValueWithSecurityBypass(
    this,
    nameof(CreatedBy),
    GetCurrentUser());
```

In Middle Tier mode, call bypass setters from `OnSaving`.

## Filter by Owner in Permissions

```csharp
defaultRole.AddTypePermission<Document>(
    SecurityOperations.Read, SecurityPermissionState.Deny);
defaultRole.AddObjectPermissionFromLambda<Document>(
    SecurityOperations.Read,
    d => d.CreatedBy.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
    SecurityPermissionState.Allow);
```
