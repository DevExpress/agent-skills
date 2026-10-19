# DevExpress services and the container

| Service | Where it belongs | Why |
|---|---|---|
| Services attached to an element — `DialogService`, `WindowService`, `DocumentManagerService` variants, `NavigationService`, `NotificationService`, `DispatcherService`, `CurrentWindowService`, `OpenFileDialogService` and the like declared in `Interaction.Behaviors` | Stay in XAML; view models keep getting them through the DevExpress service container (`GetService<I...>()`, `ServiceContainer`) | They need the element they are attached to; a container-created instance is attached to nothing |
| Unattached services — message box services | May be registered in the container (singleton) and injected by interface | They need no visual element |
| An attached service the user explicitly wants injected | The help topic's attach-behavior approach: the service instance comes from the container and a behavior attaches it to the element | Only when asked; it adds a custom behavior class |

Do not register `IDialogService` / `IWindowService` implementations in the container as ordinary services. Views that these services create are covered by `view-patterns.md`.

Links for `docs-lookup`: https://docs.devexpress.com/WPF/403514 (Use Services with Dependency Injection), https://docs.devexpress.com/WPF/17444.
