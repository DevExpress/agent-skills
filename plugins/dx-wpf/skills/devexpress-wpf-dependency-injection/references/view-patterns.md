# View patterns

What each creation site from `detection.md` becomes. DevExpress 26.1+ unless noted; before 26.1 read "custom extension" wherever `{dxmvvm:Ioc}` appears.

| Creation site | Becomes |
|---|---|
| `code-behind-new` in a window | Remove the `DataContext = new ...` line; the window's XAML sets `DataContext="{dxmvvm:Ioc Type={x:Type vm:X}}"`, or the window is resolved from the container and receives `X` in its constructor. One of the two, not both |
| `xaml-element` (`<vm:X/>` in `.DataContext`) | Replace the property element with `DataContext="{dxmvvm:Ioc Type={x:Type vm:X}}"`; `X` gets its dependencies through its constructor |
| `child-view` (UserControl inside a window with its own view model) | The UserControl sets its own `DataContext` with `{dxmvvm:Ioc}` — it is created by the XAML parser, so it cannot receive constructor parameters. If the child is meant to inherit the parent's view model, leave it without a `DataContext` |
| Two views that share state through a static object | Register the state as a singleton and inject it into both view models; delete the static accessor |
| `view-created-by-service` (dialog, window, document, navigation by view name) | Leave the service call as it is. The view is created by DevExpress `ViewLocator`, which resolves the view type from the container first (26.1+) and otherwise uses its parameterless constructor. Give the view its view model with `{dxmvvm:Ioc}` in its XAML, or pass a view model the calling view model received by injection (for example a factory). Do not create views with `new` inside view models |
| View registered in the container | Only needed when the view itself needs constructor injection; register it transient. The locator uses the registration when present |
| `poco-runtime` | See `pitfalls.md` → POCO view models |
| `scaffolded` (POCO with a `protected` constructor and a static `Create(...)`) | Register `sp => X.Create(sp.GetRequiredService<...>())`; in XAML, `{dxmvvm:Ioc Type={x:Type vm:X}}` replaces `{dxmvvm:ViewModelSource vm:X}`. See `pitfalls.md` → Scaffolded view model, and → Dependency is `null` in an override for shell view models derived from `DocumentsViewModel` |
| `compile-time` (`[GenerateViewModel]`) | A normal class: register it and resolve it like any other view model |
| `service-new` inside a view model | Constructor parameter of an interface or the concrete type; registration per `lifetimes.md` |
| `static-singleton`, `service-locator` | Constructor parameters; delete the accessor or helper once unused |

Links for `docs-lookup`: https://docs.devexpress.com/WPF/403514 (view model binding, POCO), https://docs.devexpress.com/WPF/DevExpress.Mvvm.UI.IocExtension.
