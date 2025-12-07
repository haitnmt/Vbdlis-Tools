using Avalonia.Controls;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop.Views.Controls;

public partial class DvhcSelectorControl : UserControl
{
    public DvhcSelectorControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Khởi tạo UserControl với ViewModel
    /// </summary>
    public void Initialize(DvhcSelectorViewModel viewModel)
    {
        DataContext = viewModel;
    }
}
