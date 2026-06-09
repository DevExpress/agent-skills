# Security Setup — Code Snippets

## EF Core Integrated Mode (Blazor/Win)

```csharp
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;

builder.Security
    .UseIntegratedMode(options => {
        options.RoleType = typeof(PermissionPolicyRole);
        options.UserType = typeof(ApplicationUser);
        options.UserLoginInfoType = typeof(ApplicationUserLoginInfo); // EF Core only
    })
    .AddPasswordAuthentication(options => {
        options.IsSupportChangePassword = true;
    })
    .AddWindowsAuthentication(options => {
        options.CreateUserAutomatically();
    });
```

`AddWindowsAuthentication` is typically used in intranet/domain scenarios where users are already authenticated by Active Directory.

## XPO Integrated Mode (Win/Blazor)

```csharp
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

builder.Security
    .UseIntegratedMode(options => {
        options.RoleType = typeof(PermissionPolicyRole);
        options.UserType = typeof(ApplicationUser);
        // No UserLoginInfoType in the XPO integrated setup path
    })
    .AddPasswordAuthentication(options => {
        options.IsSupportChangePassword = true;
    });
```

## OAuth2 in Blazor

```csharp
builder.Security
    .UseIntegratedMode(options => {
        options.RoleType = typeof(PermissionPolicyRole);
        options.UserType = typeof(ApplicationUser);
        options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
    })
    .AddPasswordAuthentication(options => options.IsSupportChangePassword = true);

services.AddAuthentication()
    .AddGoogle(options => {
        options.ClientId = Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
        options.Scope.Add("email");
        options.Scope.Add("profile");
    });
```

`ApplicationUserLoginInfo` must be registered in the EF Core `DbContext` to store external logins.

## Required EF Core DbContext Entities

```csharp
public class MyDbContext : DbContext {
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ApplicationUserLoginInfo> UserLoginInfos { get; set; }
    public DbSet<PermissionPolicyRole> Roles { get; set; }
    public DbSet<ModelDifference> ModelDifferences { get; set; }
    public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
    // ... your business entities
}
```

## Middleware (Blazor)

```csharp
app.UseAuthentication();
app.UseAuthorization();
```
