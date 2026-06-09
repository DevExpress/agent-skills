# Data Binding — Blazor TreeList

When you need to: bind the TreeList to flat or hierarchical data; configure parent-child relationships; load child nodes on demand.

## Flat Data with Parent-Child Keys (Most Common)

Bind `Data` to any `IEnumerable<T>`. The TreeList builds the tree structure from `KeyFieldName` and `ParentKeyFieldName`:

```razor
<DxTreeList Data="@Departments"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            RootValue="@((object)0)">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" Caption="Department" />
        <DxTreeListDataColumn FieldName="HeadCount" />
    </Columns>
</DxTreeList>

@code {
    List<Department> Departments { get; set; }

    protected override void OnInitialized() {
        Departments = new List<Department> {
            new Department { Id = 1, ParentId = 0, Name = "Company", HeadCount = 250 },
            new Department { Id = 2, ParentId = 1, Name = "Engineering", HeadCount = 100 },
            new Department { Id = 3, ParentId = 2, Name = "Backend", HeadCount = 45 },
            new Department { Id = 4, ParentId = 2, Name = "Frontend", HeadCount = 35 },
            new Department { Id = 5, ParentId = 1, Name = "Sales", HeadCount = 80 },
        };
    }
}
```

## RootValue Configuration

`RootValue` specifies the parent key value that identifies root-level nodes. The default is `null`.

| Root node `ParentId` value | Configuration |
|---|---|
| `null` | `RootValue` not required (default) |
| `0` (int) | `RootValue="@((object)0)"` (cast required — `@0` alone is invalid Razor syntax, RZ1005) |
| `""` (empty string) | `RootValue="@string.Empty"` |
| `-1` | `RootValue="@(-1)"` |
| `Guid.Empty` | `RootValue="@Guid.Empty"` |

## EF Core Binding

```razor
@inject IDbContextFactory<AppDbContext> DbFactory
@implements IDisposable

<DxTreeList Data="@Categories"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" />
        <DxTreeListDataColumn FieldName="Description" />
    </Columns>
</DxTreeList>

@code {
    AppDbContext Db;
    List<Category> Categories;

    protected override async Task OnInitializedAsync() {
        Db = DbFactory.CreateDbContext();
        Categories = await Db.Categories.ToListAsync();
    }

    public void Dispose() => Db?.Dispose();
}
```

## Load Children on Demand

For large trees where loading all nodes upfront is expensive, use the `ChildrenLoaded` event. Provide `Data` with root nodes only; child nodes are loaded when the user expands a row:

```razor
<DxTreeList Data="@RootCategories"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            ChildrenLoaded="LoadChildren">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" />
    </Columns>
</DxTreeList>

@code {
    List<Category> RootCategories { get; set; }

    protected override async Task OnInitializedAsync() {
        RootCategories = await DataService.GetRootCategoriesAsync();
    }

    async Task LoadChildren(TreeListChildrenLoadingEventArgs e) {
        var parentId = ((Category)e.DataItem).Id;
        e.Children = await DataService.GetChildrenAsync(parentId);
    }
}
```

> **Important**: When using load-on-demand, only root-level items should be in the initial `Data` collection. Returning children via `e.Children` integrates them into the tree state automatically.

## Data Structure Requirements

| Requirement | Details |
|---|---|
| Primary key field | Each item must have a unique value in `KeyFieldName`. The type can be `int`, `Guid`, `string`, etc. |
| Parent key field | Root nodes must have the `ParentKeyFieldName` field equal to `RootValue`. |
| Circular references | Not supported. Ensure your data has no cycles. |
| Duplicate keys | Not supported. Each key must be unique across the whole dataset. |
