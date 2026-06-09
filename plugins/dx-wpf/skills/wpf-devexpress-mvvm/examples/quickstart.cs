// DevExpress WPF MVVM Framework — Quickstart (C#)
// Demonstrates: [GenerateViewModel], ViewModelBase, services, behaviors, messaging

using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Mvvm.DataAnnotations;

// ------------------------------------------------------------------
// 1. Compile-time view model (recommended — C# 9+)
//    Requires: DevExpress.Mvvm.CodeGenerators NuGet package
//
// XAML — a compile-time view model is instantiated directly (NOT via ViewModelSource,
//        which is the runtime-POCO mechanism):
//   <Window.DataContext>
//       <local:MainViewModel/>
//   </Window.DataContext>
//   <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"/>
//   <Button Content="Save" Command="{Binding SaveCommand}"/>
// ------------------------------------------------------------------
[GenerateViewModel]
partial class MainViewModel {
    // A leading underscore is trimmed by the generator: _name -> Name, _isBusy -> IsBusy.
    [GenerateProperty] string _name = "Alice";
    [GenerateProperty] bool _isBusy;

    [GenerateCommand]
    void Save() {
        IsBusy = true;
        // persist Name ...
        IsBusy = false;
    }

    bool CanSave() => !string.IsNullOrWhiteSpace(Name);
}

// ------------------------------------------------------------------
// 2. Runtime POCO view model (POCOViewModel attribute style)
//    Works with ViewModelSource — no code generator needed
// ------------------------------------------------------------------
[POCOViewModel]
public class OrderViewModel {
    public virtual string OrderNumber { get; set; } = "";
    public virtual decimal Total { get; set; }

    public void Submit() {
        // called via SubmitCommand
    }

    public bool CanSubmit() => Total > 0;
}

// ------------------------------------------------------------------
// 3. ViewModelBase — manual INPC + DelegateCommand
// ------------------------------------------------------------------
public class ProductViewModel : ViewModelBase {
    string _productName = "";
    public string ProductName {
        get => _productName;
        set => SetProperty(ref _productName, value, nameof(ProductName));
    }

    public DelegateCommand DeleteCommand { get; }

    public ProductViewModel() {
        DeleteCommand = new DelegateCommand(Delete, CanDelete);
    }

    void Delete() => ProductName = "";
    bool CanDelete() => !string.IsNullOrEmpty(ProductName);
}

// ------------------------------------------------------------------
// 4. MessageBoxService — show dialogs from a ViewModel (no code-behind)
//
// XAML (DXMessageBoxService lives in the dx: core namespace, not dxmvvm:):
//   <Window>
//       <dxmvvm:Interaction.Behaviors>
//           <dx:DXMessageBoxService/>
//       </dxmvvm:Interaction.Behaviors>
//   </Window>
// ------------------------------------------------------------------
[POCOViewModel]
public class DeleteViewModel {
    IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

    public void ConfirmDelete() {
        var result = MessageBoxService.ShowMessage(
            "Delete this record?", "Confirm", MessageButton.YesNo, MessageIcon.Warning);
        if (result == MessageResult.Yes)
            System.Diagnostics.Debug.WriteLine("Deleted");
    }
}

// ------------------------------------------------------------------
// 5. DialogService — open a child window from ViewModel
//
// XAML:
//   <dxmvvm:Interaction.Behaviors>
//       <dx:DialogService/>
//   </dxmvvm:Interaction.Behaviors>
// ------------------------------------------------------------------
[POCOViewModel]
public class EditorHostViewModel {
    IDialogService DialogService => this.GetService<IDialogService>();

    public void OpenEditor() {
        var result = DialogService.ShowDialog(
            dialogButtons: MessageButton.OKCancel,
            title: "Edit Record",
            viewModel: new CommandsViewModel());
        if (result == MessageResult.OK) {
            // apply changes
        }
    }
}

public class CommandsViewModel { }

// ------------------------------------------------------------------
// 6. Messenger — decouple ViewModels
// ------------------------------------------------------------------
public class SavedMessage { public string Payload { get; set; } = ""; }

[POCOViewModel]
public class SenderViewModel {
    public void Notify() => Messenger.Default.Send(new SavedMessage { Payload = "done" });
}

[POCOViewModel]
public class ReceiverViewModel {
    public ReceiverViewModel() {
        Messenger.Default.Register<SavedMessage>(this, msg =>
            System.Diagnostics.Debug.WriteLine($"Received: {msg.Payload}"));
    }
}
