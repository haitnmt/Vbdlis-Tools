using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Haihv.Vbdlis.Tools.Desktop.ViewModels;

public partial class HomeViewModel(MainWindowViewModel mainWindowViewModel) : ViewModelBase
{
    [ObservableProperty]
    private string _loggedInUsername = mainWindowViewModel.LoggedInUsername;

    [ObservableProperty]
    private string _loggedInServer = mainWindowViewModel.LoggedInServer;

    public ICommand LogoutCommand { get; } = mainWindowViewModel.LogoutCommand;
}
