// DevExpress WinForms Editors — Quickstart (C#)
// Demonstrates: EditValue data binding, masks, ButtonEdit with custom buttons,
//               SVG glyphs, button-only editors, immediate post on toggle.
// Packages: DevExpress.Win.Navigation (XtraEditors) + DevExpress.Win.Dialogs
//           (for XtraOpenFileDialog, used in section 2)   Host form: XtraForm

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;                 // DefaultBoolean
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;

// ------------------------------------------------------------------
// 1. Three editors bound via EditValue, with masks
// ------------------------------------------------------------------
public partial class MainForm : XtraForm {
    BindingSource employeeBindingSource = new();
    Employee currentEmployee = new();   // typed model so EditValue bindings resolve

    void BuildEditors() {
        var name = new TextEdit { Width = 240 };
        name.Properties.NullValuePrompt = "Full name";
        name.Properties.MaskSettings.Configure<MaskSettings.RegExp>(s => {
            s.MaskExpression = "[A-Z][a-z]+( [A-Z][a-z]+)+";   // First Last
            s.IsAutoComplete = true;
        });

        var salary = new SpinEdit { Width = 140 };
        salary.Properties.MinValue = 0;
        salary.Properties.MaxValue = 1_000_000;
        salary.Properties.Increment = 100;
        salary.Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => s.MaskExpression = "c0");

        var birth = new DateEdit { Width = 140 };
        birth.Properties.VistaCalendarViewStyle =
            DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
        birth.Properties.MaskSettings.Configure<MaskSettings.DateTime>(s => s.MaskExpression = "d");

        employeeBindingSource.DataSource = currentEmployee;
        name.DataBindings.Add("EditValue", employeeBindingSource, "FullName");
        salary.DataBindings.Add("EditValue", employeeBindingSource, "Salary");
        birth.DataBindings.Add("EditValue", employeeBindingSource, "Birthday");

        Controls.AddRange(new Control[] { name, salary, birth });
    }
}

// Model bound above — the property names match the EditValue bindings.
public class Employee {
    public string FullName { get; set; } = "";
    public decimal Salary { get; set; }
    public DateTime Birthday { get; set; } = DateTime.Today;
}

// ------------------------------------------------------------------
// 2. ButtonEdit with two buttons (browse / clear)
// ------------------------------------------------------------------
public static class ButtonEditDemo {
    public static ButtonEdit Build() {
        var path = new ButtonEdit { Width = 360 };
        path.Properties.Buttons.Clear();
        path.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Ellipsis) { Tag = "browse" });
        path.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Clear)    { Tag = "clear"  });
        path.ButtonClick += (s, e) => {
            var edit = (ButtonEdit)s;
            if ((string)e.Button.Tag == "browse") {
                using var dlg = new XtraOpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK) edit.EditValue = dlg.FileName;
            }
            if (e.Button.Kind == ButtonPredefines.Clear) edit.Clear();
        };
        return path;
    }
}

// ------------------------------------------------------------------
// 3. Currency formatting (mask used as display format)
// ------------------------------------------------------------------
public static class CurrencyEditor {
    public static void Apply(SpinEdit spinEdit1) {
        spinEdit1.Properties.MaskSettings.Configure<MaskSettings.Numeric>(s => s.MaskExpression = "c2");
        spinEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
    }
}

// ------------------------------------------------------------------
// 4. Custom SVG glyph in a ButtonEdit
// ------------------------------------------------------------------
public static class SvgGlyph {
    public static void Add(ButtonEdit buttonEdit1) {
        var send = new EditorButton(ButtonPredefines.Glyph);
        send.ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromFile("send.svg");
        send.ImageOptions.SvgImageSize = new Size(16, 16);
        buttonEdit1.Properties.Buttons.Add(send);
    }
}

// ------------------------------------------------------------------
// 5. Immediate post on toggle / check
// ------------------------------------------------------------------
public static class ImmediatePost {
    public static void Apply(CheckEdit checkEdit1, ToggleSwitch toggleSwitch1) {
        checkEdit1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;   // commits on each tick
        toggleSwitch1.Properties.InplaceModeImmediatePostChanges = DefaultBoolean.True;
    }
}
