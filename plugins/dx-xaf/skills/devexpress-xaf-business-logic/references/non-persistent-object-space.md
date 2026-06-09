# NonPersistentObjectSpace

## What It Is

`NonPersistentObjectSpace` is an `IObjectSpace` implementation for objects that are **not mapped to a database**. It manages in-memory objects through the same API as persistent Object Spaces, so standard XAF CRUD actions (New, Save, Delete) work automatically in views bound to non-persistent types.

Namespace: `DevExpress.ExpressApp`

## When to Use It

- **Transient UI objects** — dialog parameters, wizard steps, filter panels
- **Report / dashboard parameters** — parameter objects shown before report generation
- **External API data** — objects populated from a REST service, file, or other non-database source
- **Calculated / aggregated views** — read-only objects assembled at runtime

## Registration

Register the provider at startup so XAF creates a `NonPersistentObjectSpace` for non-persistent types:

```csharp
// Startup.cs — register before the application is built
builder.ObjectSpaceProviders
    .AddNonPersistent();
```

## Declaring a Non-Persistent Class

Non-persistent classes use `[DomainComponent]` instead of ORM mapping. Implement `INotifyPropertyChanged` so the Object Space tracks modifications automatically:

```csharp
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

[DomainComponent]
public class ReportParameters : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private DateTime startDate = DateTime.Today;
    private DateTime endDate = DateTime.Today;

    [ModelDefault("DisplayFormat", "{0:d}")]
    public DateTime StartDate {
        get => startDate;
        set { startDate = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate))); }
    }

    public DateTime EndDate {
        get => endDate;
        set { endDate = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate))); }
    }
}
```

## Key Events

Subscribe to events globally when the Object Space is created:

```csharp
builder.ObjectSpaceProviders.Events.OnObjectSpaceCreated = context => {
    if (context.ObjectSpace is NonPersistentObjectSpace npos) {
        npos.ObjectsGetting += Npos_ObjectsGetting;
        npos.ObjectByKeyGetting += Npos_ObjectByKeyGetting;
        npos.CustomCommitChanges += Npos_CustomCommitChanges;
    }
};
```

### ObjectsGetting — Supply Collection Data

Fires when the Object Space creates a collection (e.g., for a ListView). Populate `e.Objects` with your data:

```csharp
void Npos_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
    if (e.ObjectType == typeof(ExternalContact)) {
        // Fetch from an external API or in-memory cache
        e.Objects = contactService.GetAllContacts()
            .Select(c => new ExternalContact { Id = c.Id, Name = c.Name })
            .ToBindingList();
    }
}
```

### ObjectByKeyGetting — Supply a Single Object by Key

Fires when `GetObjectByKey()` is called:

```csharp
void Npos_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
    if (e.ObjectType == typeof(ExternalContact)) {
        var contact = contactService.GetById(e.Key);
        if (contact != null) {
            e.Object = new ExternalContact { Id = contact.Id, Name = contact.Name };
        }
    }
}
```

### CustomCommitChanges — Persist Changes to an External Store

Fires when `CommitChanges()` is called. Set `e.Handled = true` to replace the default (no-op) commit:

```csharp
void Npos_CustomCommitChanges(object sender, HandledEventArgs e) {
    var npos = (NonPersistentObjectSpace)sender;
    foreach (var obj in npos.ModifiedObjects) {
        if (obj is ExternalContact contact) {
            contactService.Save(contact);
        }
    }
    e.Handled = true;
}
```

## Standard XAF Actions Work Automatically

Because `NonPersistentObjectSpace` implements `IObjectSpace`, the built-in New, Save, Delete, and Refresh actions work out of the box in any view bound to a non-persistent type. No additional controller code is needed for basic CRUD — just handle the events above to wire the data source.

## Accessing Persistent Data from a Non-Persistent Object Space

Use `AdditionalObjectSpaces` (inherited from `CompositeObjectSpace`) to attach a persistent Object Space when your non-persistent objects need to reference persistent entities:

```csharp
if (context.ObjectSpace is NonPersistentObjectSpace npos) {
    var persistentOs = context.Application.CreateObjectSpace(typeof(Employee));
    npos.AdditionalObjectSpaces.Add(persistentOs);
}
```
