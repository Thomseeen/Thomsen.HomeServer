using System.Windows;

using Thomsen.HomeServer.Widget.ViewModels;
using Thomsen.HomeServer.Widget.Views;

namespace Thomsen.HomeServer.Widget;

/// <summary>
/// Interaction logic for HomeServerWidget.xaml
/// </summary>
public partial class HomeServerWidget : Application {
    public TaskbarIconView TaskbarView { get; }
    public TaskbarIconViewModel TaskbarViewModel { get; }
    public MainWindowViewModel MainWindowViewModel { get; }

    public bool IsMainWindowShown { get; set; }

    public HomeServerWidget(MainWindowViewModel viewModel, TaskbarIconViewModel taskbarViewModel) {
        MainWindowViewModel = viewModel;
        TaskbarViewModel = taskbarViewModel;

        TaskbarView = new TaskbarIconView {
            DataContext = taskbarViewModel
        };

        ShowMainWindow();
    }

    public void ShowMainWindow() {
        if (IsMainWindowShown) {
            return;
        }

        MainWindow = new MainWindowView {
            DataContext = MainWindowViewModel
        };

        MainWindow.Loaded += (sender, e) => IsMainWindowShown = true;
        MainWindow.Closed += (sender, e) => IsMainWindowShown = false;

        MainWindow.Show();
        MainWindow.Focus();
    }

    public void CloseMainWindow() {
        MainWindow.Close();
        MainWindow = null;
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e) {
        base.OnExit(e);
    }
}
