# XPO Performance — Code Snippets

## Delayed Loading for Large Properties

Defer loading of BLOBs and complex references not shown in List Views:

```csharp
using DevExpress.Xpo;

public class Document : BaseObject {
    [Delayed(true)]
    public byte[] FileContent {
        get => GetDelayedPropertyValue<byte[]>(nameof(FileContent));
        set => SetDelayedPropertyValue(nameof(FileContent), value);
    }
}
```

## Explicit Loading for Reference Properties

Include references in the main SELECT instead of separate queries:

```csharp
using DevExpress.Xpo;

public class Order : BaseObject {
    [ExplicitLoading]
    public Customer Customer {
        get => fCustomer;
        set => SetPropertyValue(nameof(Customer), ref fCustomer, value);
    }
    Customer fCustomer;
}
```

## Connection Pooling (WinForms)

Prevent repeated connection creation in InstantFeedback mode:

```csharp
builder.ObjectSpaceProviders
    .AddSecuredXpo((application, options) => {
        options.EnablePoolingInConnectionString = true;
    });
```

## XPO Profiling

Enable XPO logging by uncommenting the XPO switch in the application configuration file, or use the XPO Profiler tool.
