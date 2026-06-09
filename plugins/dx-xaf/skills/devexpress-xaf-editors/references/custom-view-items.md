# Editors — Custom View Items

Code snippets for ViewItemAttribute-based custom View Items and BlazorControlViewItem for Razor components.

---

## ViewItem with ViewItemAttribute (Blazor Button)

```csharp
public interface IModelButtonViewItem : IModelViewItem { }

[ViewItem(typeof(IModelButtonViewItem))]
public class ButtonViewItem : ViewItem, IComponentContentHolder, IComplexViewItem {
    private ButtonModel componentModel;
    private XafApplication application;

    public ButtonViewItem(IModelViewItem model, Type objectType)
        : base(objectType, model.Id) { }

    RenderFragment IComponentContentHolder.ComponentContent =>
        ComponentModelObserver.Create(componentModel, componentModel.GetComponentContent());

    void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
        this.application = application;
    }

    protected override object CreateControlCore() {
        componentModel = new ButtonModel {
            Text = "Click me!",
            Click = EventCallback.Factory.Create<MouseEventArgs>(this,
                () => application.ShowViewStrategy.ShowMessage("Clicked!"))
        };
        return componentModel;
    }
}
```

After creating a custom ViewItem, add it to a View layout in the Application Model via code using a `ModelNodesGeneratorUpdater`:

```csharp
// Register the custom IModelViewItem interface in your Module
public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
    extenders.Add<IModelViewItem, IModelMyCustomViewItem>();
}

public interface IModelMyCustomViewItem : IModelViewItem {
    string CustomProperty { get; set; }
}
```

```csharp
// Add the ViewItem to a DetailView layout via a generator updater
public class MyViewItemUpdater : ModelNodesGeneratorUpdater<ModelDetailViewLayoutNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        var detailView = node.Parent as IModelDetailView;
        if (detailView?.ModelClass?.TypeInfo.Type == typeof(Order)) {
            var viewItems = detailView.Items;
            var item = viewItems.AddNode<IModelMyCustomViewItem>("MyCustomItem");
            item.CustomProperty = "value";
        }
    }
}
```

## BlazorControlViewItem for Razor Components in Dashboard Views

Use `BlazorControlViewItem` with `CascadingParameter` in Razor components to embed custom controls that access `ObjectSpace`:

```razor
@code {
    [CascadingParameter] public BlazorControlViewItem ViewItem { get; set; }

    protected override void OnInitialized() {
        base.OnInitialized();
        var data = ViewItem.ObjectSpace.GetObjects<MyEntity>();
        ViewItem.ObjectSpace.Reloaded += (s, e) => { /* refresh */ };
    }
}
```
