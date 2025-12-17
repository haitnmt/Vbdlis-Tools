using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

namespace Haihv.Vbdlis.Tools.Desktop.Services
{
    /// <summary>
    /// Service interface for application initialization logic
    /// </summary>
    public interface IApplicationInitializer
    {
        /// <summary>
        /// Initializes the main window after checking for updates and ensuring dependencies are ready
        /// </summary>
        Task InitializeAsync(IClassicDesktopStyleApplicationLifetime desktop);

        /// <summary>
        /// Gets whether the application is shutting down
        /// </summary>
        bool IsShuttingDown { get; }
    }
}