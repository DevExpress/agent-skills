# MVVM Behaviors

Behaviors attach reusable logic to UI controls without subclassing. They react to control events and either confirm user actions, route them to ViewModel commands, or run custom code.

Attach behaviors using `mvvmContext.AttachBehavior<TBehavior>(control)` or the Fluent API.

---

## Built-in Behavior Types

| Behavior | Purpose |
|---|---|
| `ConfirmationBehavior<TEventArgs>` | Intercepts an event and shows a confirmation dialog before allowing it to proceed |
| `EventToCommandBehavior<TViewModel, TEventArgs>` | Routes a control event to a ViewModel command |
| Key-to-Command (fluent) | Routes a keyboard shortcut to a ViewModel command |
| `EventTriggerBase<TEventArgs>` | Base class for custom behaviors |

---

## 1. ConfirmationBehavior

Shows a dialog when an event fires. If the user cancels, the event is cancelled (sets `Cancel = true` on cancelable event args).

### Inline (no subclass needed)

```csharp
// Prompt before CheckEdit state changes
mvvmContext1.AttachBehavior<ConfirmationBehavior<ChangingEventArgs>>(
    checkEdit1,
    behavior => {
        behavior.Caption = "Confirm";
        behavior.Text    = "Are you sure you want to change this setting?";
        behavior.Buttons = ConfirmationButtons.YesNo;
        behavior.ShowQuestionIcon = true;
    },
    "EditValueChanging"   // event name on the control
);
```

### Fluent API (shortest)

```csharp
mvvmContext1
    .WithEvent<ChangingEventArgs>(checkEdit1, "EditValueChanging")
    .Confirmation(b => {
        b.Caption = "Confirm";
        b.Text    = "Are you sure?";
    });
```

### Class-based (for complex logic or reuse)

```csharp
public class CloseConfirmBehavior : ConfirmationBehavior<FormClosingEventArgs> {
    public CloseConfirmBehavior() : base("FormClosing") { }

    protected override string GetConfirmationCaption() => "Exit?";
    protected override string GetConfirmationText()    => "Unsaved changes will be lost.";
}

// Attach to form
mvvmContext1.AttachBehavior<CloseConfirmBehavior>(this);
```

---

## 2. EventToCommandBehavior

Maps any control event to a ViewModel command. Useful for third-party controls or events that don't natively support `Command` binding.

### Fluent API (inline — no subclass needed)

```csharp
var fluent = mvvmContext1.OfType<MainViewModel>();

// "DoubleClick" on listBox1 → calls vm.ViewDetails()
fluent.WithEvent<EventArgs>(listBox1, "DoubleClick")
      .EventToCommand(vm => vm.ViewDetails());
```

### Class-based

```csharp
public class DoubleClickToViewDetails : EventToCommandBehavior<MainViewModel, EventArgs> {
    public DoubleClickToViewDetails() : base("DoubleClick", x => x.ViewDetails()) { }
}

mvvmContext1.AttachBehavior<DoubleClickToViewDetails>(listBox1);
```

### Passing Event Args as Command Parameter

```csharp
fluent.WithEvent<MouseEventArgs>(panel1, "MouseDown")
      .EventToCommand(vm => vm.OnMouseDown(null), args => args);
// ViewModel: public void OnMouseDown(MouseEventArgs args) { ... }
```

---

## 3. Key-to-Command (Keyboard Shortcuts)

Route keyboard shortcuts to ViewModel commands without event handlers.

```csharp
var fluent = mvvmContext1.OfType<DocumentViewModel>();

// Ctrl+S → Save
fluent.WithKey(this, Keys.S | Keys.Control)
      .KeyToCommand(vm => vm.Save);

// Delete key in grid → DeleteSelected
fluent.WithKey(gridView1, Keys.Delete)
      .KeyToCommand(vm => vm.DeleteSelected);

// F5 → Refresh
fluent.WithKey(this, Keys.F5)
      .KeyToCommand(vm => vm.Refresh);
```

The ViewModel's `CanXxx()` method is respected — if `CanSave()` returns `false`, the shortcut does nothing.

---

## 4. Custom Behavior

Inherit from `EventTriggerBase<TEventArgs>` to create fully custom behaviors.

```csharp
// Paste from clipboard when Ctrl+V is pressed in a grid
public class GridCtrlVBehavior : EventTriggerBase<KeyEventArgs> {
    public GridCtrlVBehavior() : base("KeyDown") { }

    protected override void OnEvent() {
        if (Args.Control && Args.KeyCode == Keys.V) {
            var svc = this.GetService<IClipboardService>();
            svc?.PasteToGrid();
            Args.Handled = true;
        }
    }
}

// Register service and attach:
mvvmContext1.RegisterService(new ClipboardService());
mvvmContext1.AttachBehavior<GridCtrlVBehavior>(gridControl1);
```

`this.GetService<T>()` inside a behavior resolves from the behavior's owner `MVVMContext`.

---

## Attaching Behaviors — API Summary

| Syntax | When to use |
|---|---|
| `mvvmContext.AttachBehavior<TBehavior>(control)` | Class-based behavior, no initialization |
| `mvvmContext.AttachBehavior<TBehavior>(control, init, eventName)` | Class-based with inline initialization |
| `mvvmContext.WithEvent<TArgs>(control, "EventName").Confirmation(...)` | Inline confirmation, one line — `Confirmation` is on the **MVVMContext-level** `WithEvent`, **not** the Fluent-API `fluent.WithEvent` (which has no `.Confirmation`) |
| `fluent.WithEvent<TArgs>(control, "EventName").EventToCommand(vm => vm.Method)` | Inline event-to-command, one line |
| `fluent.WithKey(control, Keys.X).KeyToCommand(vm => vm.Method)` | Keyboard shortcut |

---

## Source Material

- Behaviors concept: `https://docs.devexpress.com/content/WindowsForms/113975?md=true`
