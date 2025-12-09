using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Haihv.Vbdlis.Tools.Desktop.Views;
using Haihv.Vbdlis.Tools.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Serilog;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private bool _isShuttingDown = false;

        public static IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Configure dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            Services = _serviceProvider;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                // Set ShutdownMode to OnExplicitShutdown to prevent auto-shutdown when windows close
                desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;

                // Initialize MainWindow asynchronously after ensuring Playwright is ready
                _ = InitializeMainWindowAsync(desktop);

                // Handle application exit to cleanup resources
                desktop.ShutdownRequested += OnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Initializes the main window after ensuring Playwright browsers are installed.
        /// This ensures the application is fully ready before showing the UI.
        /// </summary>
        private async Task InitializeMainWindowAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            Log.Information("Starting main window initialization...");

            // First, ensure Playwright browsers are installed
            await EnsurePlaywrightBrowsersAsync();

            Log.Information("Playwright check completed, preparing to show main window...");

            // Check if app is shutting down before proceeding
            if (_isShuttingDown)
            {
                Log.Information("Application is shutting down, skipping main window initialization");
                return;
            }

            // Then create and show the main window on UI thread
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (_serviceProvider == null || _isShuttingDown)
                    {
                        Log.Error("Service provider is null or app is shutting down, cannot initialize main window");
                        return;
                    }

                    Log.Information("Creating MainWindow and ViewModel...");

                    // Get MainWindowViewModel from DI container
                    var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();

                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = mainViewModel,
                        MinWidth = 1100,
                        MinHeight = 880,
                        WindowState = WindowState.Maximized,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    // Show the main window
                    desktop.MainWindow.Show();

                    // Now that MainWindow is shown, change shutdown mode to close when main window closes
                    desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

                    Log.Information("Main window initialized and shown");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize main window");
                throw;
            }

            // Check for updates after MainWindow is shown (non-blocking)
            _ = CheckForUpdatesAsync();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Register Playwright installer service
            services.AddSingleton<IPlaywrightInstallerService, PlaywrightInstallerService>();

            // Register Playwright service as singleton to maintain browser context
            services.AddSingleton<IPlaywrightService, PlaywrightService>();

            // Register Credential service
            services.AddSingleton<ICredentialService, CredentialService>();

            // Register Update service
            services.AddSingleton<IUpdateService, UpdateService>();

            // Register ViewModels
            services.AddSingleton<MainWindowViewModel>();

            // Add other services here as needed
        }

        /// <summary>
        /// Checks if Playwright browsers are available; attempts install if missing.
        /// </summary>
        private async Task EnsurePlaywrightBrowsersAsync()
        {
            if (_serviceProvider == null)
            {
                Log.Warning("Service provider is not initialized");
                return;
            }

            var installer = _serviceProvider.GetService<IPlaywrightInstallerService>();
            if (installer == null)
            {
                Log.Warning("PlaywrightInstallerService is not registered");
                return;
            }

            var os = installer.GetOperatingSystem();
            Log.Information("Checking Playwright browsers on {OS}...", os);

            // If already installed, we're done
            if (installer.IsBrowsersInstalled())
            {
                Log.Information("Playwright browsers are ready.");
                return;
            }

            // Show installation window with progress/status
            var installViewModel = new PlaywrightInstallationViewModel
            {
                OperatingSystem = os
            };
            var installWindow = new PlaywrightInstallationWindow
            {
                DataContext = installViewModel,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            installWindow.Show();
            installWindow.StartInstallation();

            bool ready;
            try
            {
                ready = await installer.EnsureBrowsersInstalledAsync(message =>
                {
                    installWindow.UpdateStatus(message);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while ensuring Playwright browsers");
                ready = false;
                installWindow.SetError(ex.Message);
            }

            if (ready)
            {
                installWindow.CompleteInstallation();
                _ = installWindow.AutoCloseAfterDelayAsync();
                Log.Information("Playwright browsers are ready.");
            }
            else
            {
                installWindow.SetError("Không thể cài đặt Playwright. Vui lòng kiểm tra kết nối mạng hoặc cài thủ công.");
                Log.Warning("Playwright browsers still missing after attempted install.");
            }
        }

        /// <summary>
        /// Checks for application updates in the background
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Wait a bit after app startup before checking
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (_serviceProvider == null)
                    return;

                var updateService = _serviceProvider.GetService<IUpdateService>();
                if (updateService == null)
                    return;

                Log.Information("Checking for updates...");
                var updateInfo = await updateService.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    Log.Information("Update available: {Version}", updateInfo.Version);

                    // Show update notification on UI thread
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        var result = await ShowUpdateDialogAsync(updateInfo);

                        if (result)
                        {
                            // User wants to update
                            Log.Information("User accepted update");
                            await updateService.DownloadAndInstallUpdateAsync(updateInfo, progress =>
                            {
                                Log.Information("Download progress: {Progress}%", progress);
                            });

                            // Installer will launch and close this app
                        }
                    });
                }
                else
                {
                    Log.Information("No updates available");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking for updates");
            }
        }

        /// <summary>
        /// Shows update dialog to user
        /// </summary>
        private async Task<bool> ShowUpdateDialogAsync(UpdateInfo updateInfo)
        {
            // Simple message box for now - can be replaced with custom UI
            try
            {
                var messageBox = new Window
                {
                    Title = "Cập nhật mới",
                    Width = 450,
                    Height = 250,
                    CanResize = false,
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                };

                var stackPanel = new StackPanel
                {
                    Margin = new Thickness(20),
                    Spacing = 15
                };

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Phiên bản mới {updateInfo.Version} đã sẵn sàng!",
                    FontSize = 16,
                    FontWeight = Avalonia.Media.FontWeight.Bold
                });

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Phiên bản hiện tại: {updateService?.CurrentVersion ?? "N/A"}",
                    FontSize = 12,
                    Foreground = Avalonia.Media.Brushes.Gray
                });

                if (!string.IsNullOrEmpty(updateInfo.ReleaseNotes))
                {
                    var scrollViewer = new ScrollViewer
                    {
                        Height = 80,
                        Content = new TextBlock
                        {
                            Text = updateInfo.ReleaseNotes,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            FontSize = 11
                        }
                    };
                    stackPanel.Children.Add(scrollViewer);
                }

                var buttonPanel = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Spacing = 10
                };

                bool result = false;

                var updateButton = new Button
                {
                    Content = "Cập nhật ngay",
                    Padding = new Thickness(20, 8),
                    Background = Avalonia.Media.Brushes.Green,
                    Foreground = Avalonia.Media.Brushes.White
                };
                updateButton.Click += (s, e) => { result = true; messageBox.Close(); };

                var laterButton = new Button
                {
                    Content = "Để sau",
                    Padding = new Thickness(20, 8)
                };
                laterButton.Click += (s, e) => { result = false; messageBox.Close(); };

                buttonPanel.Children.Add(updateButton);
                buttonPanel.Children.Add(laterButton);
                stackPanel.Children.Add(buttonPanel);

                messageBox.Content = stackPanel;

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
                {
                    await messageBox.ShowDialog(desktop.MainWindow);
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        private IUpdateService? updateService => _serviceProvider?.GetService<IUpdateService>();

        private async void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            _isShuttingDown = true;

            // Cleanup Playwright service
            if (_serviceProvider != null)
            {
                var playwrightService = _serviceProvider.GetService<IPlaywrightService>();
                if (playwrightService != null)
                {
                    await playwrightService.CloseAsync();
                }

                await _serviceProvider.DisposeAsync();
            }
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}
