// DevExpress WPF Scheduler — Quickstart (C#)
// Demonstrates: unbound mode, bound mode with mappings, ViewModel, view switching

using System;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Scheduling;

// ------------------------------------------------------------------
// 1. Data model for bound mode
// ------------------------------------------------------------------
public class MedicalAppointment {
    public int Id { get; set; }
    public bool AllDay { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Subject { get; set; } = "";
    public string Location { get; set; } = "";
    public int? DoctorId { get; set; }
    public int Type { get; set; }
    public string RecurrenceInfo { get; set; } = "";
    public string ReminderInfo { get; set; } = "";   // serialized ReminderItem(s)
}

public class Doctor {
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

// ------------------------------------------------------------------
// 2. ViewModel
//
// XAML (MainWindow.xaml):
//   <Window DataContext="{dxmvvm:ViewModelSource Type=local:MainViewModel}">
//       <dxsch:SchedulerControl GroupType="Resource">
//           <dxsch:SchedulerControl.DataSource>
//               <dxsch:DataSource AppointmentsSource="{Binding Appointments}"
//                                 ResourcesSource="{Binding Doctors}">
//                   <dxsch:DataSource.AppointmentMappings>
//                       <dxsch:AppointmentMappings Id="Id"
//                                                  Type="Type"
//                                                  Start="StartTime"
//                                                  End="EndTime"
//                                                  Subject="Subject"
//                                                  Location="Location"
//                                                  ResourceId="DoctorId"
//                                                  RecurrenceInfo="RecurrenceInfo"
//                                                  Reminder="ReminderInfo"/>
//                   </dxsch:DataSource.AppointmentMappings>
//                   <dxsch:DataSource.ResourceMappings>
//                       <dxsch:ResourceMappings Id="Id" Caption="Name"/>
//                   </dxsch:DataSource.ResourceMappings>
//               </dxsch:DataSource>
//           </dxsch:SchedulerControl.DataSource>
//           <dxsch:SchedulerControl.Views>
//               <dxsch:WeekView/>
//           </dxsch:SchedulerControl.Views>
//       </dxsch:SchedulerControl>
//   </Window>
// ------------------------------------------------------------------
[POCOViewModel]
public class MainViewModel {
    public ObservableCollection<MedicalAppointment> Appointments { get; }
    public ObservableCollection<Doctor> Doctors { get; }

    public MainViewModel() {
        Doctors = new ObservableCollection<Doctor> {
            new() { Id = 1, Name = "Dr. Smith" },
            new() { Id = 2, Name = "Dr. Jones" },
        };
        Appointments = new ObservableCollection<MedicalAppointment> {
            new() {
                Id = 1, StartTime = DateTime.Today.AddHours(9),
                EndTime = DateTime.Today.AddHours(10),
                Subject = "Check-up", DoctorId = 1, Location = "Room 101"
            },
            new() {
                Id = 2, StartTime = DateTime.Today.AddHours(11),
                EndTime = DateTime.Today.AddHours(12),
                Subject = "Consultation", DoctorId = 2, Location = "Room 202"
            },
        };
    }
}

// ------------------------------------------------------------------
// 3. Unbound mode — quick prototyping, no persistence
// ------------------------------------------------------------------
public partial class UnboundWindow : Window {
    void AddAppointments() {
        schedulerControl.AppointmentItems.Add(new AppointmentItem {
            Start = DateTime.Today.AddHours(9),
            End = DateTime.Today.AddHours(10),
            Subject = "Team standup",
        });
        schedulerControl.AppointmentItems.Add(new AppointmentItem {
            Start = DateTime.Today.AddHours(14),
            End = DateTime.Today.AddHours(15),
            Subject = "Code review",
        });
    }
}

// ------------------------------------------------------------------
// 4. Change view at runtime
//
// All seven view types ship enabled by default. Switch by setting
// ActiveViewIndex to the index of the desired view in
// SchedulerControl.Views. The default order is:
//   0 = Day, 1 = Work Week, 2 = Week, 3 = Month,
//   4 = Timeline, 5 = Agenda, 6 = List
// (If you declared a custom <SchedulerControl.Views> set in XAML,
//  indexes follow the declaration order in that collection.)
// ------------------------------------------------------------------
public partial class ViewSwitchWindow : Window {
    void SetDayView()      => schedulerControl.ActiveViewIndex = 0;
    void SetWorkWeekView() => schedulerControl.ActiveViewIndex = 1;
    void SetWeekView()     => schedulerControl.ActiveViewIndex = 2;
    void SetMonthView()    => schedulerControl.ActiveViewIndex = 3;
    void SetTimelineView() => schedulerControl.ActiveViewIndex = 4;
    void SetAgendaView()   => schedulerControl.ActiveViewIndex = 5;
    void SetListView()     => schedulerControl.ActiveViewIndex = 6;
}
