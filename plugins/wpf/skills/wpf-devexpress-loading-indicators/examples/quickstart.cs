// DevExpress WPF Loading Indicators — Quickstart (C#)
// Demonstrates: SplashScreenManager, LoadingDecorator, WaitIndicator
// NOTE: SplashScreenManager.CreateThemed() requires ApplicationThemeHelper.ApplicationThemeName
//       to be set first — the splash derives its colors from the active theme.

using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;

// ------------------------------------------------------------------
// 1. App.xaml.cs — themed startup splash (auto-closes when main window loads)
// ------------------------------------------------------------------
public partial class App : System.Windows.Application {
    public App() {
        // Theme must be set before CreateThemed — splash derives colors from it.
        // Lightweight Themes require the DevExpress.Wpf.ThemesLW NuGet package.
        CompatibilitySettings.UseLightweightThemes = true;
        ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
        SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
            Title = "MyApp",
            Subtitle = "Loading…",
            IsIndeterminate = true,
        }).ShowOnStartup();
    }
}

// ------------------------------------------------------------------
// 2. SplashScreenManager — on-demand (long operation in code-behind)
//
// XAML: nothing needed — SplashScreenManager is code-only.
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    async void LoadData() {
        var manager = SplashScreenManager.CreateWaitIndicator(
            new DXSplashScreenViewModel { Status = "Loading data…" });
        manager.Show();
        try {
            await Task.Run(() => System.Threading.Thread.Sleep(2000)); // simulate work
        } finally {
            manager.Close();
        }
    }

    // Progress updates — update via manager.ViewModel (cross-thread marshalling is internal).
    async void LoadWithProgress() {
        var manager = SplashScreenManager.CreateThemed(
            new DXSplashScreenViewModel {
                Title = "Importing…",
                IsIndeterminate = false,
            });
        manager.Show();
        for (int i = 0; i <= 100; i += 10) {
            manager.ViewModel.Progress = i;
            manager.ViewModel.Status = $"Step {i / 10} of 10…";
            await Task.Delay(200);
        }
        manager.Close();
    }
}

// ------------------------------------------------------------------
// 3. LoadingDecorator — wraps any content; shows spinner via IsSplashScreenShown
//
// XAML:
//   <dx:LoadingDecorator IsSplashScreenShown="{Binding IsLoading}"
//                        OwnerLock="LoadingContent">
//       <DataGrid ItemsSource="{Binding Orders}"/>
//   </dx:LoadingDecorator>
// ------------------------------------------------------------------
public class DataViewModel : System.ComponentModel.INotifyPropertyChanged {
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    bool isLoading;
    public bool IsLoading {
        get => isLoading;
        set {
            isLoading = value;
            PropertyChanged?.Invoke(this,
                new System.ComponentModel.PropertyChangedEventArgs(nameof(IsLoading)));
        }
    }

    public async Task LoadAsync() {
        IsLoading = true;
        try { await Task.Run(() => System.Threading.Thread.Sleep(1500)); }
        finally { IsLoading = false; }
    }
}

// ------------------------------------------------------------------
// 4. WaitIndicator — inline spinner, always visible until hidden
//
// XAML:
//   <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"
//                     Content="Saving…"/>
// ------------------------------------------------------------------
