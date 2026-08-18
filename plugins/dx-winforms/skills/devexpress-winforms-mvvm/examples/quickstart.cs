// DevExpress WinForms MVVM — Quickstart (C#)
// Demonstrates: compile-time ViewModel codegen, designer-backed MVVMContext + fluent binding,
//               services (message box), confirmation behavior, messenger.
// Package: DevExpress.Win (MVVM in DevExpress.Utils.MVVM / DevExpress.Mvvm)
// Compile-time codegen requires .NET 6+ and BOTH DevExpress.Mvvm.CodeGenerators + DevExpress.Mvvm.
//
// Section 2 shows the RECOMMENDED split: the controls and the MVVMContext live in
// MainForm.Designer.cs (InitializeComponent) so the form stays editable in the WinForms
// designer; only the ViewModel wiring (bindings, commands, behaviors) lives in MainForm.cs.
// MVVMContext and each editor's .Properties implement ISupportInitialize (wrap in
// BeginInit/EndInit); SimpleButton does not.

using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using DevExpress.Mvvm.POCO;             // this.GetService<T>() extension
using DevExpress.Utils.MVVM;
using DevExpress.XtraEditors;

// ------------------------------------------------------------------
// 1. ViewModel — compile-time generated properties and commands
// ------------------------------------------------------------------
[GenerateViewModel]
partial class MainViewModel {
    [GenerateProperty]
    string userName = string.Empty;

    [GenerateCommand]
    public void Save() { /* persist data */ }   // public: bound from the View via vm => vm.Save
    bool CanSave() => !string.IsNullOrEmpty(UserName);

    [GenerateCommand]
    public async Task LoadAsync() {
        var data = await FetchDataAsync();
        UserName = data.Name;
    }

    Task<(string Name, int _)> FetchDataAsync() => Task.FromResult(("Alice", 0));

    // Service usage from the ViewModel
    protected IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();
    public void Greet() => MessageBoxService.ShowMessage($"Hello, {UserName}!");
}

// ------------------------------------------------------------------
// 2. View — designer-backed controls + MVVMContext; wiring in the code-behind
// ------------------------------------------------------------------

// --- MainForm.cs — ViewModel wiring only (bindings, commands, behaviors) ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                       // creates the controls + MVVMContext
        mvvmContext.ViewModelType = typeof(MainViewModel);
        var fluent = mvvmContext.OfType<MainViewModel>();

        fluent.SetBinding(textEdit1, te => te.Text, vm => vm.UserName);
        fluent.BindCommand(btnSave, vm => vm.Save);
        fluent.BindCommand(btnLoad, vm => vm.LoadAsync);
        fluent.BindCancelCommand(btnCancel, vm => vm.LoadAsync);

        // Confirm before an irreversible action.
        // Confirmation is on the MVVMContext-level WithEvent (NOT the fluent one).
        mvvmContext
            .WithEvent<FormClosingEventArgs>(this, "FormClosing")
            .Confirmation(b => {
                b.Caption = "Exit";
                b.Text    = "Unsaved changes will be lost. Exit anyway?";
            });
    }
}

// --- MainForm.Designer.cs — controls + MVVMContext the designer round-trips ---
partial class MainForm {
    private System.ComponentModel.IContainer components = null;
    private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
    private TextEdit textEdit1;
    private SimpleButton btnSave;
    private SimpleButton btnLoad;
    private SimpleButton btnCancel;

    private void InitializeComponent() {
        this.components  = new System.ComponentModel.Container();
        this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
        this.textEdit1   = new TextEdit();
        this.btnSave     = new SimpleButton();
        this.btnLoad     = new SimpleButton();
        this.btnCancel   = new SimpleButton();
        // MVVMContext and the editor's Properties (RepositoryItem) are ISupportInitialize; buttons are not.
        ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
        this.SuspendLayout();
        //
        // mvvmContext
        //
        this.mvvmContext.ContainerControl = this;
        //
        // textEdit1
        //
        this.textEdit1.Location = new System.Drawing.Point(12, 12);
        this.textEdit1.Name = "textEdit1";
        this.textEdit1.Size = new System.Drawing.Size(220, 20);
        //
        // buttons
        //
        this.btnSave.Location = new System.Drawing.Point(12, 48);
        this.btnSave.Name = "btnSave"; this.btnSave.Text = "Save";
        this.btnLoad.Location = new System.Drawing.Point(96, 48);
        this.btnLoad.Name = "btnLoad"; this.btnLoad.Text = "Load";
        this.btnCancel.Location = new System.Drawing.Point(180, 48);
        this.btnCancel.Name = "btnCancel"; this.btnCancel.Text = "Cancel";
        //
        // MainForm
        //
        this.Controls.Add(this.textEdit1);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.btnLoad);
        this.Controls.Add(this.btnCancel);
        this.Name = "MainForm";
        this.Text = "MVVM Quickstart";
        ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}

// ------------------------------------------------------------------
// 3. Broadcast a notification between ViewModels (Messenger)
// ------------------------------------------------------------------
public class DataRefreshMessage { }

public class ReceiverViewModel {
    public ReceiverViewModel() {
        Messenger.Default.Register<DataRefreshMessage>(this, _ => RefreshGrid());
    }
    void RefreshGrid() { }
    public void Send() => Messenger.Default.Send(new DataRefreshMessage());
    public void Cleanup() => Messenger.Default.Unregister(this);
}
