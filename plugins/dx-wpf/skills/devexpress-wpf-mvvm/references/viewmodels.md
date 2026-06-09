# View Models — Strategies, Properties, Commands

DevExpress MVVM offers **four** ways to build a view model. The difference is mostly about how much boilerplate you write yourself: from "lots" (`BindableBase`) to "almost none" (`[GenerateViewModel]`). Pick **one strategy per project** and stick with it — mixing strategies hurts readability and confuses tooling.

## When to Use This Reference

Use this when you need to:

- Pick a view-model strategy
- Define `INotifyPropertyChanged` properties in each style
- Wire commands (sync `DelegateCommand` or async `AsyncCommand`) in each style
- Understand the per-style requirements (C# version, .NET version, VB support)
- Migrate from one style to another

## The Four Strategies

| Strategy | Boilerplate level | Recommended for |
|---|---|---|
| **`[GenerateViewModel]`** — compile-time source generator | Minimal | New projects, C# 9+ |
| **`ViewModelSource.Create<T>()`** — runtime POCO | Minimal | Older runtimes, VB.NET |
| **`ViewModelBase`** — direct inheritance | Moderate | Existing codebases using it; design-time helpers |
| **`BindableBase`** — direct inheritance | Substantial | Minimal lib; no command auto-discovery |

### Requirements Matrix

|   | Compile-time | Runtime POCO | ViewModelBase | BindableBase |
|---|---|---|---|---|
| Min C# | 9 | 6 | 6 | 6 |
| Min .NET Framework | 4.6.1 | 4.5.2 | 4.5.2 | 4.5.2 |
| Min .NET Core | 3.0 | 3.0 | 3.0 | 3.0 |
| VB.NET supported | No | Yes | Yes | Yes |
| Generated code debuggable | Yes | No (Reflection.Emit) | n/a | n/a |
| Inheritance required | No (partial class) | No (POCO) | Yes | Yes |

## Strategy 1: Compile-Time `[GenerateViewModel]` (Recommended)

Source-generator-based. You declare a `partial class`, mark fields with `[GenerateProperty]` and methods with `[GenerateCommand]`. At compile time the generator emits a second partial that adds INPC and command boilerplate. The output is a real, debuggable `.cs` file you can step into.

### Prereqs

- `DevExpress.Mvvm.CodeGenerators` NuGet package installed
- `<LangVersion>9</LangVersion>` (or higher) in `.csproj`

### Properties

```csharp
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel]
partial class ViewModel {
    [GenerateProperty]
    string username = "";

    [GenerateProperty]
    string status = "";
}
```

What the generator emits:

```csharp
partial class ViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void RaisePropertyChanged(PropertyChangedEventArgs e) =>
        PropertyChanged?.Invoke(this, e);

    public string? Username {
        get => username;
        set {
            if (EqualityComparer<string?>.Default.Equals(username, value)) return;
            username = value;
            RaisePropertyChanged(UsernameChangedEventArgs);
        }
    }
    // (same for Status)

    static readonly PropertyChangedEventArgs UsernameChangedEventArgs = new(nameof(Username));
}
```

### `[GenerateProperty]` Options

| Property | Use |
|---|---|
| `IsVirtual` | Mark the generated property `virtual` |
| `OnChangedMethod` | Method to invoke after value changes (default name: `On[PropertyName]Changed`) |
| `OnChangingMethod` | Method to invoke before value changes (default: `On[PropertyName]Changing`) |
| `SetterAccessModifier` | Restrict the setter (`Private`, `Protected`, `Internal`, `ProtectedInternal`) |

```csharp
[GenerateProperty]
string username = "";

void OnUsernameChanged(string oldValue) {
    // runs after Username is set
}
```

### Commands

Methods marked `[GenerateCommand]` become commands. Return type decides the command type:

- `void` → `DelegateCommand`
- `Task` → `AsyncCommand`

```csharp
[GenerateViewModel]
partial class ViewModel {
    [GenerateProperty]
    string username = "";

    [GenerateCommand]
    void Login() => /* sync */;
    bool CanLogin() => !string.IsNullOrEmpty(Username);

    [GenerateCommand]
    async Task LoadAsync() { /* async */ }
}
```

Emitted:

```csharp
DelegateCommand? loginCommand;
public DelegateCommand LoginCommand =>
    loginCommand ??= new DelegateCommand(Login, CanLogin, true);

AsyncCommand? loadAsyncCommand;
public AsyncCommand LoadAsyncCommand =>
    loadAsyncCommand ??= new AsyncCommand(LoadAsync, null, false, true);
```

### `[GenerateCommand]` Options

| Property | Use |
|---|---|
| `Name` | Custom command property name |
| `CanExecuteMethod` | Custom `CanExecute` method name (default: `Can[ActionName]`) |
| `UseCommandManager` | Hook `CommandManager.RequerySuggested` for re-evaluation (default `true`) |
| `AllowMultipleExecution` | Allow re-entering an async command before the previous run completes (default `false`) |

### Implement Additional Interfaces

`[GenerateViewModel]` accepts flags to add interface implementations:

```csharp
[GenerateViewModel(
    ImplementINotifyPropertyChanging = true,
    ImplementIDataErrorInfo = true,
    ImplementISupportServices = true,
    ImplementISupportParentViewModel = true)]
partial class ViewModel { /* ... */ }
```

| Flag | Adds |
|---|---|
| `ImplementINotifyPropertyChanging` | `INotifyPropertyChanging` + `RaisePropertyChanging` |
| `ImplementIDataErrorInfo` | `IDataErrorInfo` (validation via `DataAnnotations`) |
| `ImplementISupportServices` | `ISupportServices` + `ServiceContainer` + `GetService<T>()` |
| `ImplementISupportParentViewModel` | `ISupportParentViewModel` for parent-child relationships |

## Strategy 2: Runtime POCO

`ViewModelSource.Create<T>()` uses **Reflection.Emit** at runtime to wrap your POCO class in a descendant that:

- Adds INPC raising to **virtual** properties
- Auto-creates command properties from methods
- Implements `ISupportParameter`, `ISupportParentViewModel`

### Requirements

- Class must be `public` and non-sealed
- Properties intended to be bindable must be `virtual`
- Default constructor must exist (typically `protected`)

### Properties

```csharp
public class ViewModel {
    public virtual string Username { get; set; } = "";
    public virtual string Status { get; set; } = "";

    public void Login() => Status = $"Welcome, {Username}";
    public bool CanLogin() => !string.IsNullOrEmpty(Username);

    protected ViewModel() { }
    public static ViewModel Create() =>
        DevExpress.Mvvm.POCO.ViewModelSource.Create(() => new ViewModel());
}
```

`virtual` properties become bindable: setting one fires `PropertyChanged`. `public` methods become commands (`LoginCommand`, with `CanLogin` auto-discovered as the `CanExecute` callback).

### Use from XAML

```xaml
<Window xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        DataContext="{dxmvvm:ViewModelSource Type=vm:ViewModel}">
```

### When to Pick POCO

- VB.NET project (compile-time generator is C#-only)
- Older runtimes where compile-time generator isn't available
- Existing codebase already using POCO

### Trade-offs

- Runtime cost: first instance creation has Reflection.Emit overhead
- Generated wrapper isn't visible — can't step into
- Slightly more magic; non-DevExpress developers may not recognize the pattern

## Strategy 3: `ViewModelBase`

Direct inheritance from `DevExpress.Mvvm.ViewModelBase`. The most "classical" pattern in DX MVVM — explicit base class with helpers.

### Properties

```csharp
using DevExpress.Mvvm;

public class ViewModel : ViewModelBase {
    public string Username {
        get => GetProperty(() => Username);
        set => SetProperty(() => Username, value);
    }

    public string Status {
        get => GetProperty(() => Status);
        set => SetProperty(() => Status, value);
    }
}
```

`GetProperty` / `SetProperty` use expression trees to identify the property name without strings.

### Alternative: Expression-Free

`ViewModelBase` also inherits `GetValue`/`SetValue` from `BindableBase`, which uses `[CallerMemberName]`:

```csharp
public string Username {
    get => GetValue<string>();
    set => SetValue(value);
}
```

Less verbose, similar performance. Either form is idiomatic.

### Commands — `[Command]` Attribute

`ViewModelBase` implements `ICustomTypeDescriptor` and auto-exposes a `LoginCommand` property for any public method marked `[Command]`:

```csharp
using DevExpress.Mvvm.DataAnnotations;

public class ViewModel : ViewModelBase {
    [Command]
    public void Login() => /* sync */;
    public bool CanLogin() => !string.IsNullOrEmpty(Username);

    [Command]
    public async Task LoadAsync() { /* async */ }
}
```

### `[Command]` Attribute Options

| Property | Use |
|---|---|
| `Name` | Custom command property name (default: `[MethodName]Command`) |
| `CanExecuteMethodName` | Custom `CanExecute` name (default: `Can[MethodName]`) |
| `UseCommandManager` | Hook into `CommandManager.RequerySuggested` (default `true`) |
| `AllowMultipleExecution` | For async commands (default `false`) |

### Design-Time Initialization

`ViewModelBase` exposes `OnInitializeInDesignMode()` and `OnInitializeInRuntime()` virtual methods. Override to avoid hitting the database at design time:

```csharp
protected override void OnInitializeInDesignMode() {
    base.OnInitializeInDesignMode();
    Employees = new List<Employee> { new() { Name = "Sample" } };
}

protected override void OnInitializeInRuntime() {
    base.OnInitializeInRuntime();
    Employees = _db.GetEmployees();
}
```

`IsInDesignMode` is exposed as a bindable boolean.

### Services Access

`ViewModelBase` implements `ISupportServices`:

```csharp
public IMessageBoxService MessageBox => GetService<IMessageBoxService>();
```

See [services.md](services.md).

### View-Model Communication

`ViewModelBase` implements `ISupportParameter` and `ISupportParentViewModel`. Override `OnParameterChanged(object)` and `OnParentViewModelChanged(object)`.

See [viewmodel-communication.md](viewmodel-communication.md).

## Strategy 4: `BindableBase`

The bare minimum: only `INotifyPropertyChanged`. Use this for lightweight models (not full view models) or when you don't want command auto-discovery / service support.

```csharp
using DevExpress.Mvvm;

public class ViewModel : BindableBase {
    public string FirstName {
        get => GetValue<string>();
        set => SetValue(value);
    }
}
```

Commands must be manually constructed:

```csharp
public class ViewModel : BindableBase {
    public string FirstName { get => GetValue<string>(); set => SetValue(value); }

    public ICommand SaveCommand { get; }

    public ViewModel() {
        SaveCommand = new DelegateCommand(Save, CanSave);
    }

    void Save() { /* ... */ }
    bool CanSave() => !string.IsNullOrEmpty(FirstName);
}
```

## Commands — Sync vs Async

Both supported in all four strategies.

### DelegateCommand (Sync)

```csharp
public ICommand SaveCommand => new DelegateCommand(Save, CanSave);
public ICommand DeleteCommand => new DelegateCommand<int>(Delete, CanDelete);
```

| Constructor parameter | Use |
|---|---|
| `executeMethod` | The action to run |
| `canExecuteMethod` | (Optional) predicate; if returns `false`, the bound control is disabled |
| `useCommandManager` | Hook `CommandManager.RequerySuggested` for re-evaluation (default `true`) |

To force re-evaluation manually: `command.RaiseCanExecuteChanged()`.

### AsyncCommand

```csharp
public AsyncCommand LoadCommand => new AsyncCommand(LoadAsync);
public AsyncCommand<int> FetchCommand => new AsyncCommand<int>(FetchAsync);

async Task LoadAsync() { await _api.LoadAsync(); }
async Task FetchAsync(int id) { await _api.FetchAsync(id); }
```

| Constructor parameter | Use |
|---|---|
| `executeMethod` | The async task |
| `canExecuteMethod` | (Optional) predicate |
| `allowMultipleExecution` | If `false` (default), the command is disabled while a previous run is in flight |
| `useCommandManager` | Hook `CommandManager.RequerySuggested` |

| `AsyncCommand` member | Use |
|---|---|
| `IsExecuting` | `true` while a run is in progress (bindable) |
| `CancelCommand` | A built-in cancel sub-command |
| `CancellationTokenSource` | Available inside the async method via `command.IsCancellationRequested` |

#### Cancellation in AsyncCommand

```csharp
public AsyncCommand LoadCommand { get; }

public ViewModel() {
    LoadCommand = new AsyncCommand(LoadAsync);
}

async Task LoadAsync() {
    var token = LoadCommand.CancellationTokenSource.Token;
    while (!token.IsCancellationRequested) {
        await Task.Delay(100, token);
        // work
    }
}
```

Bind `LoadCommand.CancelCommand` to a Cancel button.

## Property Examples Across Strategies

Same view model, different strategies:

### Compile-Time

```csharp
[GenerateViewModel]
partial class ProductVM {
    [GenerateProperty] string name = "";
    [GenerateProperty] decimal price;
    [GenerateProperty] int stock;

    [GenerateCommand]
    void Buy() => Stock--;
    bool CanBuy() => Stock > 0;
}
```

### Runtime POCO

```csharp
public class ProductVM {
    public virtual string Name { get; set; } = "";
    public virtual decimal Price { get; set; }
    public virtual int Stock { get; set; }

    public void Buy() => Stock--;
    public bool CanBuy() => Stock > 0;

    protected ProductVM() { }
    public static ProductVM Create() => ViewModelSource.Create(() => new ProductVM());
}
```

### ViewModelBase

```csharp
public class ProductVM : ViewModelBase {
    public string Name { get => GetValue<string>(); set => SetValue(value); }
    public decimal Price { get => GetValue<decimal>(); set => SetValue(value); }
    public int Stock { get => GetValue<int>(); set => SetValue(value); }

    [Command]
    public void Buy() => Stock--;
    public bool CanBuy() => Stock > 0;
}
```

### BindableBase

```csharp
public class ProductVM : BindableBase {
    public string Name { get => GetValue<string>(); set => SetValue(value); }
    public decimal Price { get => GetValue<decimal>(); set => SetValue(value); }
    public int Stock { get => GetValue<int>(); set => SetValue(value); }

    public ICommand BuyCommand { get; }

    public ProductVM() {
        BuyCommand = new DelegateCommand(() => Stock--, () => Stock > 0);
    }
}
```

## Migration

### POCO → Compile-Time

| POCO | Compile-time |
|---|---|
| `public virtual string Name { get; set; }` | `[GenerateProperty] string name;` |
| `public void Login()` (auto-command) | `[GenerateCommand] void Login()` |
| `public bool CanLogin()` (auto-discovered) | `bool CanLogin()` (same name, still auto-discovered) |
| `protected ViewModel()` | (none — partial class doesn't need it) |
| `ViewModelSource.Create(() => new VM())` | `new ViewModel()` directly |

Replace XAML `DataContext="{dxmvvm:ViewModelSource Type=vm:ViewModel}"` with `<vm:ViewModel/>`.

### ViewModelBase → Compile-Time

`ViewModelBase` features that need preserving when migrating:

- `OnInitializeInDesignMode` / `OnInitializeInRuntime` — re-implement manually, or check `IsInDesignMode` at the call site
- `[Command]` attribute — replace with `[GenerateCommand]`
- `GetService<T>()` — set `ImplementISupportServices = true` on `[GenerateViewModel]`
- `ISupportParameter` / `ISupportParentViewModel` — implement manually or set the corresponding `[GenerateViewModel]` flags

## Common Issues

- **`[GenerateViewModel]` produces no output** — `DevExpress.Mvvm.CodeGenerators` package missing, or `<LangVersion>` < 9. Check both.
- **Generated class has a different name than expected** — the generator uses the same name as your partial class. Don't put two partial classes with `[GenerateViewModel]` and the same name in different files unless you want them merged.
- **`virtual` keyword forgotten on POCO property** — without `virtual`, the runtime generator can't intercept the setter; INPC won't fire.
- **`ViewModelSource.Create<T>` fails with "type must be public"** — check the access modifier and that the class is not sealed.
- **`[Command]` method not turning into a command** — only works in `ViewModelBase` (or `POCO`). `BindableBase` doesn't auto-discover commands.
- **`AsyncCommand` runs the same handler multiple times** — `allowMultipleExecution` is `true`. Set `false` to enforce single-run.
- **`AsyncCommand.IsExecuting` stays `false`** — bound to a property of a different command instance; verify the binding source.
- **Property setter doesn't notify** — using `GetValue`/`SetValue` from the wrong base class, or compile-time attribute applied to a public property instead of a private field.

## Source Material

- `articles/mvvm-framework/viewmodels.md` (https://docs.devexpress.com/content/WPF/17439?md=true)
- `articles/mvvm-framework/viewmodels/compile-time-generated-viewmodels.md` (https://docs.devexpress.com/content/WPF/402989?md=true)
- `articles/mvvm-framework/viewmodels/runtime-generated-poco-viewmodels.md` (https://docs.devexpress.com/content/WPF/17352?md=true)
- `articles/mvvm-framework/viewmodels/viewmodelbase.md` (https://docs.devexpress.com/content/WPF/17351?md=true)
- `articles/mvvm-framework/viewmodels/bindablebase.md` (https://docs.devexpress.com/content/WPF/17350?md=true)
- `articles/mvvm-framework/commands/delegate-commands.md`
- `articles/mvvm-framework/commands/asynchronous-commands.md`
