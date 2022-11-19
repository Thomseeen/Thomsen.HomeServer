using System.Windows;
using System.Windows.Input;

namespace Thomsen.HomeServer.Widget.ViewModels;

public class TaskbarIconViewModel : BaseViewModel {
    private readonly MainWindowViewModel _mainViewModel;

    private ICommand? _showMainWindowCmd;
    private ICommand? _hideMainWindowCmd;
    private ICommand? _exitCmd;

    private static HomeServerWidget App => (HomeServerWidget)Application.Current;

    public ICommand ShowMainWindowCmd => _showMainWindowCmd ??=
        new CommandHandler(param => App.ShowMainWindow(), () => !App.IsMainWindowShown);

    public ICommand HideMainWindowCmd => _hideMainWindowCmd ??=
        new CommandHandler(param => App.CloseMainWindow(), () => App.IsMainWindowShown);

    public ICommand ExitCmd => _exitCmd ??=
        new CommandHandler(param => Application.Current.Shutdown(), () => true);

    public MainWindowViewModel MainViewModel => _mainViewModel;

    public TaskbarIconViewModel(MainWindowViewModel mainViewModel) {
        _mainViewModel = mainViewModel;
    }
}
