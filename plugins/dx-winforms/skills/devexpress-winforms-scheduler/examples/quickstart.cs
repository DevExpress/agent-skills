// DevExpress WinForms Scheduler (SchedulerControl) — Quickstart (C#)
// Demonstrates: designer-backed unbound setup, an appointment, DateNavigator
//               pairing, switching views.
// Package: DevExpress.Win.Scheduler   Host form: XtraForm

using System;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.XtraEditors;

// ------------------------------------------------------------------
// 1. Designer-backed unbound setup (RECOMMENDED layout)
//    The control and storage are declared in the *.Designer.cs partial
//    below (InitializeComponent), so the form stays editable in the
//    Visual Studio WinForms designer. The code-behind only creates
//    appointments. Sections 2-3 use the runtime API on an existing control.
// ------------------------------------------------------------------

// --- MainForm.cs — data + behavior only ---
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();                 // builds scheduler + storage and wires them

        // Unbound mode: appointments are created in code. (Field mappings apply
        // only to bound mode — see references/data-binding.md.)
        var apt = storage.CreateAppointment(AppointmentType.Normal,
            DateTime.Today.AddHours(10), DateTime.Today.AddHours(11), "Team Sync");
        storage.Appointments.Add(apt);

        schedulerControl1.Start = DateTime.Today;
    }
}

// --- MainForm.Designer.cs — structure the WinForms designer round-trips ---
partial class MainForm {
    private SchedulerControl schedulerControl1;
    private SchedulerDataStorage storage;

    private void InitializeComponent() {
        this.storage = new SchedulerDataStorage();
        this.schedulerControl1 = new SchedulerControl();
        ((System.ComponentModel.ISupportInitialize)(this.storage)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.schedulerControl1)).BeginInit();
        this.SuspendLayout();
        //
        // schedulerControl1 — bound-mode mappings, if any, would live on storage here
        //
        this.schedulerControl1.DataStorage = this.storage;
        this.schedulerControl1.Dock = DockStyle.Fill;
        this.schedulerControl1.Name = "schedulerControl1";
        this.schedulerControl1.ActiveViewType = SchedulerViewType.WorkWeek;
        //
        // MainForm
        //
        this.Controls.Add(this.schedulerControl1);
        this.Name = "MainForm";
        this.Text = "Schedule";
        ((System.ComponentModel.ISupportInitialize)(this.storage)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.schedulerControl1)).EndInit();
        this.ResumeLayout(false);
    }
}

// ------------------------------------------------------------------
// 2. Pair the scheduler with a DateNavigator
// ------------------------------------------------------------------
public static class NavigatorSetup {
    public static void Pair(DevExpress.XtraScheduler.DateNavigator dateNavigator1,
                            SchedulerControl schedulerControl1) {
        dateNavigator1.SchedulerControl = schedulerControl1;
    }
}

// ------------------------------------------------------------------
// 3. Switch to Month view
// ------------------------------------------------------------------
public static class ViewSetup {
    public static void Month(SchedulerControl schedulerControl1) {
        schedulerControl1.ActiveViewType = SchedulerViewType.Month;
    }
}
