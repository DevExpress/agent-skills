# Wiring decision

Work top-down; each question is answered from the inspection facts.

## 1. How DevExpress MVVM reaches the container

| Facts | Wiring | Links for `docs-lookup` |
|---|---|---|
| `DevExpress.Wpf.*` 26.1 or later | `IocServiceProvider.Default.ConfigureServices(provider)`; views resolve view models with `{dxmvvm:Ioc Type=...}`; DevExpress `ViewLocator` / `ViewModelLocator` (used by dialog, window, document and navigation services) try the provider first | https://docs.devexpress.com/WPF/403514, https://docs.devexpress.com/WPF/DevExpress.Mvvm.UI.IocExtension |
| DevExpress before 26.1, or only the free `DevExpress.Mvvm` package | A small custom markup extension that resolves a type from the container, as in the help topic below. DevExpress locators create views with their parameterless constructor, so views created by DevExpress services get their view model from that extension. Tell the user that 26.1+ has this built in; do not upgrade on your own | https://docs.devexpress.com/WPF/17352 |

### Verified API facts (DevExpress 26.1+)

Checked against the DevExpress sources; use them without a documentation lookup. Anything not in this list comes from `docs-lookup`.

| Fact | Value |
|---|---|
| Hand the provider to DevExpress | `DevExpress.Mvvm.IocServiceProvider.Default.ConfigureServices(IServiceProvider)` — namespace `DevExpress.Mvvm` |
| XAML resolution | `{dxmvvm:Ioc Type={x:Type vm:X}}` — markup extension `DevExpress.Mvvm.UI.IocExtension`, `xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"`; returns `null` at design time, throws at run time when the provider is not configured, returns `null` for an unregistered type |
| Generated POCO type | `DevExpress.Mvvm.POCO.ViewModelSource.GetPOCOType(Type)` |
| Locators | DevExpress `ViewLocator` / `ViewModelLocator` ask the configured provider first (skipping runtime-generated POCO types) and fall back to the parameterless constructor |
| Create a POCO instance | `ViewModelSource.Create(() => new X(...))` — the lambda body must be a `new` expression; anything else throws `ViewModelSourceException` at run time |
| Scaffolded `DocumentsViewModel<TModule, TUnitOfWork>` | Its constructor `(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory)` stores the factory in the protected field `unitOfWorkFactory`, then calls the virtual `CreateModules()` |

## 2. Which container

| Facts | Decision |
|---|---|
| A `ServiceCollection` or Host already exists | Register into it and hand its provider to DevExpress. Do not create a second container |
| The request or the project needs configuration files, logging, `IOptions`, hosted/background services, `HttpClient` factory | Generic Host: `Host.CreateApplicationBuilder` (.NET 8+) |
| Otherwise | `new ServiceCollection()` … `BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true })` |

## 3. Composition root

- Lives in `App` (`OnStartup`, or the `App` constructor when `App.xaml` resources resolve from the container).
- The provider is built and handed to DevExpress before anything that resolves from it loads:
  - `StartupUri`: WPF loads it after `OnStartup` returns, so configuring anywhere in `OnStartup` is early enough. Removing it and showing the main window from `OnStartup` is clearer and lets the main window come from the container.
  - `App.xaml` resources that use `{dxmvvm:Ioc}` load inside `InitializeComponent()` of `App` — configure in the `App` constructor before it, or move those resources.
- On exit: dispose the provider; with a Host, `StopAsync` then `Dispose`.
- Generic Host in WPF: set the content root to the application directory (`AppContext.BaseDirectory`) so `appsettings.json` is found when the app is started from another working directory; mark `appsettings.json` `CopyToOutputDirectory`.
- The main window: resolve it from the container (register it transient) or create it and let `{dxmvvm:Ioc}` give it its view model — not both.

## 4. Packages

- `Microsoft.Extensions.DependencyInjection` (or `Microsoft.Extensions.Hosting` for the Host, which includes it).
- `Microsoft.Extensions.Http` when the plan registers `AddHttpClient` — neither package above includes it.
- Match the major version already used by other `Microsoft.Extensions.*` packages in the solution; otherwise pick one supported by the target framework.
