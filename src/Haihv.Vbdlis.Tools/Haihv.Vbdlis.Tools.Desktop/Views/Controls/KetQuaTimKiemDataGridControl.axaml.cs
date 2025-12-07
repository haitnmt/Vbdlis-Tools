using Avalonia.Controls;
using Haihv.Vbdlis.Tools.Desktop.Models;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop;

public partial class KetQuaTimKiemDataGridControl : UserControl
{
    public KetQuaTimKiemDataGridControl()
    {
        InitializeComponent();
        DataContext ??= new KetQuaTimKiemDataGridViewModel();
    }

    public KetQuaTimKiemDataGridControl(KetQuaTimKiemDataGridViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Gets the KetQuaTimKiemDataGridViewModel instance
    /// </summary>
    public KetQuaTimKiemDataGridViewModel? ViewModel => DataContext as KetQuaTimKiemDataGridViewModel;

    /// <summary>
    /// Cập nhật dữ liệu từ response
    /// </summary>
    public void UpdateData(AdvancedSearchGiayChungNhanResponse? response)
    {
        ViewModel?.UpdateData(response);
    }
}
