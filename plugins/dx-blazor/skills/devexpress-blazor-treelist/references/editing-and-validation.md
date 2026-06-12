# Editing & Validation — Blazor TreeList

When you need to: enable CRUD for tree nodes; add, edit, or delete items in the hierarchy; handle save and delete events; validate user input.

## Edit Modes

Set `EditMode` on `DxTreeList`:

| Mode | Description |
|---|---|
| `TreeListEditMode.EditRow` | Inline row editors |
| `TreeListEditMode.EditForm` | Inline form below the edited row |
| `TreeListEditMode.PopupEditForm` | Modal popup edit form |
| `TreeListEditMode.EditCell` | Click any cell to edit in-place |

## Required Events

| Event | Args Type | When It Fires |
|---|---|---|
| `EditModelSaving` | `TreeListEditModelSavingEventArgs` | User saves an edited or new row |
| `DataItemDeleting` | `TreeListDataItemDeletingEventArgs` | User clicks Delete |
| `CustomizeEditModel` | `TreeListCustomizeEditModelEventArgs` | Before the edit form opens |

## Minimal Editing Setup

```razor
<DxTreeList Data="@Tasks"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            RootValue="@((object)0)"
            EditMode="TreeListEditMode.EditRow"
            EditModelSaving="OnEditModelSaving"
            DataItemDeleting="OnDataItemDeleting">
    <Columns>
        <DxTreeListCommandColumn />
        <DxTreeListDataColumn FieldName="Name" Caption="Task" />
        <DxTreeListDataColumn FieldName="Status" />
    </Columns>
</DxTreeList>

@code {
    List<TaskItem> Tasks { get; set; }

    Task OnEditModelSaving(TreeListEditModelSavingEventArgs e) {
        var model = (TaskItem)e.EditModel;
        if (e.IsNew) {
            model.Id = Tasks.Max(t => t.Id) + 1;
            Tasks.Add(model);
        } else {
            e.CopyChangesToDataItem();
        }
        return Task.CompletedTask;
    }

    Task OnDataItemDeleting(TreeListDataItemDeletingEventArgs e) {
        var item = (TaskItem)e.DataItem;
        RemoveWithDescendants(item.Id);
        return Task.CompletedTask;
    }

    void RemoveWithDescendants(int id) {
        var children = Tasks.Where(t => t.ParentId == id).Select(t => t.Id).ToList();
        foreach (var childId in children)
            RemoveWithDescendants(childId);
        Tasks.RemoveAll(t => t.Id == id);
    }
}
```

## Setting Parent ID for New Child Nodes

When a user adds a new node from within a row's context, set the `ParentId` automatically using `CustomizeEditModel`:

```razor
<DxTreeList CustomizeEditModel="OnCustomizeEditModel" ...>

@code {
    void OnCustomizeEditModel(TreeListCustomizeEditModelEventArgs e) {
        if (e.IsNew) {
            var newTask = (TaskItem)e.EditModel;
            // Parent data item is null only when adding a root node
            if (e.ParentDataItem != null)
                newTask.ParentId = ((TaskItem)e.ParentDataItem).Id;
            else
                newTask.ParentId = 0; // root
        }
    }
}
```

> `TreeListCustomizeEditModelEventArgs.ParentDataItem` contains the parent node when creating a child row.

## Typed Edit Models in EditFormTemplate

When you use `EditFormTemplate` or handle `EditModelSaving`, cast `EditModel` to your model type
before you access custom properties:

```razor
<EditFormTemplate Context="editFormContext">
    @{
        var editModel = (TaskItem)editFormContext.EditModel;
    }

    <p>Editing @editModel.Name</p>
    @editFormContext.GetEditor("Name")
</EditFormTemplate>
```

The TreeList exposes `EditModel` as `object`. Accessing `Name`, `AssignedTo`, or other model
members without a cast causes compilation errors.

## Event Args Members

### TreeListEditModelSavingEventArgs

| Member | Type | Description |
|---|---|---|
| `EditModel` | `object` | The edit model — cast to your type |
| `DataItem` | `object` | Original data item (null when `IsNew` is true) |
| `IsNew` | `bool` | True when a new row is being created |
| `CopyChangesToDataItem()` | `void` | Copies edit model changes back to the data item |

### TreeListDataItemDeletingEventArgs

| Member | Type | Description |
|---|---|---|
| `DataItem` | `object` | The item to delete — cast to your type |

### TreeListCustomizeEditModelEventArgs

| Member | Type | Description |
|---|---|---|
| `EditModel` | `object` | The edit model |
| `IsNew` | `bool` | True when creating a new row |
| `ParentDataItem` | `object` | The parent data item (for new rows only) |

## Validation

Apply DataAnnotation attributes to the data model:

```csharp
public class TaskItem {
    public int Id { get; set; }
    public int ParentId { get; set; }

    [Required(ErrorMessage = "Task name is required")]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    public string AssignedTo { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}
```

The TreeList reads DataAnnotation attributes automatically and highlights invalid fields.

## Programmatic Editing

```csharp
// Start editing the focused row
await TreeList.StartEditRowAsync(TreeList.GetFocusedRowIndex());

// Add a new root node
await TreeList.StartEditNewRowAsync();

// Add a child for the focused row
await TreeList.StartEditNewRowAsync(TreeList.GetFocusedRowIndex());

// Save the current edit
await TreeList.SaveChangesAsync();

// Cancel editing
await TreeList.CancelEditAsync();
```

For toolbar or external edit buttons, enable `FocusedRowEnabled="true"` and call
`StartEditRowAsync(TreeList.GetFocusedRowIndex())`. If you also need a visible selected row state,
combine focused row behavior with single selection.
