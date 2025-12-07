using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;
using System;
using System.Threading.Tasks;

namespace Haihv.Vbdlis.Tools.Desktop.Views
{
    public partial class PlaywrightInstallationWindow : Window
    {
        public PlaywrightInstallationWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Gets the ViewModel for this window
        /// </summary>
        public PlaywrightInstallationViewModel? ViewModel => DataContext as PlaywrightInstallationViewModel;

        /// <summary>
        /// Auto-closes the window after a delay when installation is complete
        /// </summary>
        public async Task AutoCloseAfterDelayAsync(int delayMilliseconds = 3000)
        {
            await Task.Delay(delayMilliseconds);

            // Use dispatcher to close on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Close();
            });
        }

        /// <summary>
        /// Updates the status message on the UI thread
        /// </summary>
        public void UpdateStatus(string message, int? progress = null, bool? isIndeterminate = null)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ViewModel?.UpdateStatus(message, progress, isIndeterminate);
            });
        }

        /// <summary>
        /// Marks installation as started on the UI thread
        /// </summary>
        public void StartInstallation()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ViewModel?.StartInstallation();
            });
        }

        /// <summary>
        /// Marks installation as completed on the UI thread
        /// </summary>
        public void CompleteInstallation()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ViewModel?.CompleteInstallation();
            });
        }

        /// <summary>
        /// Sets error state on the UI thread
        /// </summary>
        public void SetError(string errorMessage)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ViewModel?.SetError(errorMessage);
            });
        }

        /// <summary>
        /// Marks that browsers are already installed on the UI thread
        /// </summary>
        public void SetAlreadyInstalled()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ViewModel?.SetAlreadyInstalled();
            });
        }
    }
}
