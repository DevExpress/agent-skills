# Blazor Memory & Tab Management — Code Snippets

## Limiting Open Tabs

Set static properties in `Program.cs` before the application starts:

```csharp
// Program.cs — set before application startup
BlazorMdiShowViewStrategy.MaxTabLimit = 5; // default: 10
BlazorMdiShowViewStrategy.TabOverflowStrategy = TabOverflowStrategy.CloseLeastRecentTab;
```

### TabOverflowStrategy Enum Values

| Value | Behavior |
|-------|----------|
| `BlockNewTab` | Default. Prevents opening new tabs; notifies user to close tabs manually |
| `CloseLeastRecentTab` | Closes the least recently used tab (tabs with unsaved changes are kept) |
| `UnloadLeastRecentTab` | Unloads tab content from memory but keeps the tab visible in the UI |
| `NoLimit` | No limit enforced |

`BlazorMdiShowViewStrategy.MaxTabLimit` and `BlazorMdiShowViewStrategy.TabOverflowStrategy` are static properties on `DevExpress.ExpressApp.Blazor.BlazorMdiShowViewStrategy`.

## Tab Layout Persistence

`IModelOptionsBlazor.RestoreTabbedMdiLayout` saves tab arrangement across sessions.

> **Manual step required — Model Editor:**
> 1. Open the `SolutionName.Blazor.Server\Model.xafml` file in the Model Editor.
> 2. Navigate to the **Options** node in the model tree.
> 3. Set `RestoreTabbedMdiLayout` to `True` (or `False` to disable).
>
> The Model Editor is a design-time GUI tool that this skill cannot automate — instruct the developer to perform these steps manually.
