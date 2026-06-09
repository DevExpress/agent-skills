# Getting Started — Blazor TreeList

When you need to: set up the TreeList for the first time; configure key fields; display your first hierarchical tree.

## Prerequisites

- .NET 8, 9, or 10
- `DevExpress.Blazor` NuGet package installed from the DevExpress feed
- A valid DevExpress license
- Interactive render mode (`InteractiveServer`, `InteractiveWebAssembly`, or `InteractiveAuto`)

> **Note**: DevExpress Blazor components require .NET 8 or later. .NET Framework is not supported — Blazor itself is a .NET Core/.NET 5+ technology.

## Step 1 — Install NuGet Package

```bash
dotnet add package DevExpress.Blazor --source https://nuget.devexpress.com/free/api
```

## Step 2 — Register Services

In `Program.cs`:

```csharp
builder.Services.AddDevExpressBlazor();
```

## Step 3 — Apply Theme and Scripts

In `App.razor`, inside `<head>`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

## Step 4 — Add Namespace

In `_Imports.razor`:

```razor
@using DevExpress.Blazor
```

## Step 5 — Add the TreeList to a Page

```razor
@page "/tasks"
@rendermode InteractiveServer

<DxTreeList Data="@Tasks"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" Caption="Task" />
        <DxTreeListDataColumn FieldName="AssignedTo" />
        <DxTreeListDataColumn FieldName="DueDate" DisplayFormat="d" />
    </Columns>
</DxTreeList>

@code {
    List<TaskItem> Tasks { get; set; }

    protected override void OnInitialized() {
        Tasks = new List<TaskItem> {
            new TaskItem { Id = 1, ParentId = 0, Name = "Project Alpha", AssignedTo = "Alice", DueDate = DateTime.Today.AddMonths(3) },
            new TaskItem { Id = 2, ParentId = 1, Name = "Design", AssignedTo = "Bob", DueDate = DateTime.Today.AddDays(30) },
            new TaskItem { Id = 3, ParentId = 1, Name = "Development", AssignedTo = "Carol", DueDate = DateTime.Today.AddDays(90) },
        };
    }

    class TaskItem {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
    }
}
```

> **Root nodes**: If your root nodes have `ParentId = 0` instead of `null`, set `RootValue="@((object)0)"` on `DxTreeList`. Note: `@0` alone is invalid Razor syntax (RZ1005) — the cast is required.

## Interactive Render Mode

Most TreeList features require an interactive render mode:

| Feature | Requires Interactive |
|---|---|
| Tree expand/collapse | ✅ |
| Sorting | ✅ |
| Filtering | ✅ |
| Paging | ✅ |
| Editing | ✅ |
| Selection | ✅ |
| Export | ✅ |
| Initial data display | ❌ (static works) |

Apply render mode to the page or the component:

```razor
@rendermode InteractiveServer
```

Or in `App.razor` on the `Routes` component for server-wide:

```razor
<Routes @rendermode="InteractiveServer" />
```

## Key Field Setup

| Scenario | Configuration |
|---|---|
| Simple integer primary key | `KeyFieldName="Id"` |
| Root nodes have `ParentId = null` | `ParentKeyFieldName="ParentId"` (default `RootValue` is null) |
| Root nodes have `ParentId = 0` | `ParentKeyFieldName="ParentId"` + `RootValue="@((object)0)"` |
| Root nodes have `ParentId = -1` | `ParentKeyFieldName="ParentId"` + `RootValue="@(-1)"` |
| GUID primary key | `KeyFieldName="Id"` (works with any comparable type) |
