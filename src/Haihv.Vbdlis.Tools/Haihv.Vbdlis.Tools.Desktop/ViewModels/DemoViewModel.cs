using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Haihv.Vbdlis.Tools.Desktop.ViewModels;

public partial class DemoViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Demo View";

    [ObservableProperty]
    private string _description = "Đây là một view demo để minh họa cách thêm view mới vào ứng dụng.";

    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private string _message = "Nhấn nút để tăng bộ đếm!";

    [RelayCommand]
    private void IncrementCounter()
    {
        Counter++;
        Message = $"Bạn đã nhấn {Counter} lần!";
    }

    [RelayCommand]
    private void ResetCounter()
    {
        Counter = 0;
        Message = "Bộ đếm đã được reset!";
    }
}
