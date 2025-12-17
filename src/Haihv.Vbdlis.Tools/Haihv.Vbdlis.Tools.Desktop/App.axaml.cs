using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Haihv.Vbdlis.Tools.Desktop.Views;
using Haihv.Vbdlis.Tools.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private bool _isShuttingDown = false;
        private SplashWindow? _splashWindow;
        private SplashWindowViewModel? _splashViewModel;

        private bool _forceUpdateDialogTest = false;

        public static IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            // TEST ONLY: Uncomment to force show update dialog at startup (no "Để sau").
            // _forceUpdateDialogTest = true;
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

                // Show splash screen
                _splashViewModel = new SplashWindowViewModel();
                _splashWindow = new SplashWindow
                {
                    DataContext = _splashViewModel
                };
                _splashWindow.Show();

                // Initialize MainWindow asynchronously after ensuring Playwright is ready
                _ = InitializeMainWindowAsync(desktop);

                // Handle application exit to cleanup resources
                desktop.ShutdownRequested += OnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Initializes the main window after checking for updates and ensuring Playwright browsers are installed.
        /// This ensures the application is fully ready before showing the UI.
        /// </summary>
        private async Task InitializeMainWindowAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            Log.Information("Starting main window initialization...");

            try
            {
                // Update splash screen: Checking for updates
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _splashViewModel?.SetCheckingUpdates();
                });

                if (_forceUpdateDialogTest)
                {
                    var currentVersion = updateService?.CurrentVersion ?? "N/A";

                    var releaseNotes = await FetchLatestReleaseNotesFromGithubAsync();
                    if (string.IsNullOrWhiteSpace(releaseNotes))
                    {
                        releaseNotes = "### ✨ Có gì mới trong phiên bản này?\n" +
                                       "- Sửa lỗi hiển thị ghi chú phát hành\n" +
                                       "- Cải thiện **hiệu suất**\n" +
                                       "\n" +
                                       "### 📝 Ghi chú\n" +
                                       "- Đây là dữ liệu test (force update).";
                    }

                    var sample = new UpdateInfo
                    {
                        Version = currentVersion,
                        FileSize = 0,
                        PublishedAt = DateTime.Now,
                        IsRequired = true,
                        DownloadUrl = string.Empty,
                        ReleaseNotes = releaseNotes
                    };

                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        await ShowUpdateDialogAsync(sample, allowLater: false);
                        _splashWindow?.Close();
                        desktop.Shutdown();
                    });
                    return;
                }

                // First, check for updates BEFORE starting the app
                await CheckForUpdatesAsync();

                // Check if app is shutting down after update check (user may have chosen to update)
                if (_isShuttingDown)
                {
                    Log.Information("Application is shutting down for update, skipping main window initialization");
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _splashWindow?.Close();
                    });
                    return;
                }

                // Update splash screen: Checking Playwright
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _splashViewModel?.SetCheckingPlaywright();
                });

                // Then, ensure Playwright browsers are installed
                await EnsurePlaywrightBrowsersAsync();

                Log.Information("Playwright check completed, preparing to show main window...");

                // Check if app is shutting down before proceeding
                if (_isShuttingDown)
                {
                    Log.Information("Application is shutting down, skipping main window initialization");
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _splashWindow?.Close();
                    });
                    return;
                }

                // Update splash screen: Initializing main window
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _splashViewModel?.SetInitializingMainWindow();
                });

                // Small delay to show the status
                await Task.Delay(300);

                // Then create and show the main window on UI thread
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

                    // Update splash screen: Complete
                    _splashViewModel?.SetComplete();

                    // Show the main window
                    desktop.MainWindow.Show();

                    // Close splash window
                    _splashWindow?.Close();

                    // Now that MainWindow is shown, change shutdown mode to close when main window closes
                    desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

                    Log.Information("Main window initialized and shown");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize main window");

                // Close splash window on error
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _splashWindow?.Close();
                });

                throw;
            }
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

            // Update splash screen for installing Playwright
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _splashViewModel?.UpdateStatus("Cần cài đặt Playwright browsers...", 65);
            });

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
        /// Checks for application updates before app starts
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            Log.Information("[AUTO-UPDATE] Bắt đầu kiểm tra cập nhật tự động");

            try
            {
                if (_serviceProvider == null)
                {
                    Log.Warning("[AUTO-UPDATE] ServiceProvider chưa khởi tạo");
                    return;
                }

                var updateService = _serviceProvider.GetService<IUpdateService>();
                if (updateService == null)
                {
                    Log.Warning("[AUTO-UPDATE] UpdateService không được đăng ký");
                    return;
                }

                var updateInfo = await updateService.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    Log.Information("[AUTO-UPDATE] Hiển thị dialog cập nhật: {Version}", updateInfo.Version);

                    // Show update notification on UI thread
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        var result = await ShowUpdateDialogAsync(updateInfo);

                        if (result)
                        {
                            Log.Information("[AUTO-UPDATE] User chọn: CẬP NHẬT NGAY");

                            var (progressWindow, progressBar, progressText) = CreateUpdateProgressWindow();
                            progressWindow.Show();

                            var success = await updateService.DownloadAndInstallUpdateAsync(updateInfo,
                                progress =>
                                {
                                    Log.Debug("[AUTO-UPDATE] Progress: {Progress}%", progress);

                                    Dispatcher.UIThread.Post(() =>
                                    {
                                        progressBar.Value = progress;
                                        progressText.Text = $"Đang tải bản cập nhật... {progress}%";
                                    });
                                },
                                async () =>
                                {
                                    // Before restart, show final message and wait a moment
                                    for (int i = 3; i >= 1; i--)
                                    {
                                        Dispatcher.UIThread.Post(() =>
                                        {
                                            progressBar.Value = 100;
                                            progressText.Text = $"Tải xong. Ứng dụng sẽ khởi động lại sau {i} giây...";
                                        });
                                        await Task.Delay(TimeSpan.FromSeconds(1));
                                    }
                                });

                            if (success)
                            {
                                Log.Information("[AUTO-UPDATE] Thành công - App sẽ restart");
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    progressText.Text = "Tải/cài đặt thất bại. Vui lòng thử lại.";
                                    progressWindow.Close();
                                });
                                Log.Error("[AUTO-UPDATE] Thất bại - Vui lòng thử lại");
                            }
                        }
                        else
                        {
                            Log.Information("[AUTO-UPDATE] User chọn: ĐỂ SAU");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AUTO-UPDATE] Lỗi: {Message}", ex.Message);
            }

            Log.Information("[AUTO-UPDATE] Kết thúc kiểm tra");
        }

        /// <summary>
        /// Shows update dialog to user
        /// </summary>
        private async Task<bool> ShowUpdateDialogAsync(UpdateInfo updateInfo, bool allowLater = true)
        {
            try
            {
                var messageBox = new Window
                {
                    Title = "Cập nhật mới",
                    Width = 520,
                    Height = 340,
                    CanResize = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var container = new Grid
                {
                    RowDefinitions = new RowDefinitions("Auto,*,Auto"),
                    Margin = new Thickness(20)
                };

                var headerPanel = new Grid
                {
                    RowDefinitions = new RowDefinitions("*,*"),
                };
                var phienBanHienTai = new TextBlock
                {
                    Text = $"Phiên bản hiện tại: {updateService?.CurrentVersion ?? "N/A"}",
                    FontSize = 14,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    TextAlignment = Avalonia.Media.TextAlignment.Left
                };
                Grid.SetRow(phienBanHienTai, 0);
                headerPanel.Children.Add(phienBanHienTai);
                var textPhienBanMoi = updateInfo.FileSize > 0
                    ? $"Phiên bản mới: {updateInfo.Version} (Dung lượng ước tính: {updateInfo.FileSize / 1024.0 / 1024.0:F1} MB)"
                    : $"Phiên bản mới: {updateInfo.Version}";
                var phienBanMoi = new TextBlock
                {
                    Text = textPhienBanMoi,
                    FontSize = 14,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    TextAlignment = Avalonia.Media.TextAlignment.Left,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                };
                Grid.SetRow(phienBanMoi, 1);
                headerPanel.Children.Add(phienBanMoi);

                Grid.SetRow(headerPanel, 0);
                container.Children.Add(headerPanel);

                var releaseNotesText = string.IsNullOrWhiteSpace(updateInfo.ReleaseNotes)
                    ? "Không có ghi chú phát hành."
                    : updateInfo.ReleaseNotes;

                var infoPanel = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Avalonia.Media.Brushes.LightGray,
                    Background = Avalonia.Media.Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 12, 0, 12),
                    Padding = new Thickness(14)
                };

                var infoStack = new StackPanel { Spacing = 10 };

                infoStack.Children.Add(new TextBlock
                {
                    Text = "Nội dung cập nhật",
                    FontSize = 13,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                });

                infoStack.Children.Add(new ScrollViewer
                {
                    Height = 150,
                    Content = CreateRichReleaseNotesBlock(releaseNotesText)
                });
                infoPanel.Child = infoStack;
                Grid.SetRow(infoPanel, 1);
                container.Children.Add(infoPanel);

                var buttonPanel = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("*,*,*"),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                    ColumnSpacing = 12,
                };

                bool result = false;

                if (!allowLater)
                {
                    messageBox.Closing += (s, e) =>
                    {
                        // Force update in test mode: prevent closing unless user clicked update
                        if (!result)
                        {
                            e.Cancel = true;
                        }
                    };
                }

                var updateButton = new Button
                {
                    Content = "Cập nhật ngay",
                    Padding = new Thickness(16, 10),
                    Background = Avalonia.Media.Brushes.ForestGreen,
                    Foreground = Avalonia.Media.Brushes.White,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                updateButton.Click += (s, e) =>
                {
                    result = true;
                    messageBox.Close();
                };
                if (allowLater)
                {
                    Grid.SetColumn(updateButton, 1);
                }
                else
                {
                    Grid.SetColumn(updateButton, 0);
                    Grid.SetColumnSpan(updateButton, 3);
                }

                var laterButton = new Button
                {
                    Content = "Để sau",
                    Padding = new Thickness(16, 10),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                laterButton.Click += (s, e) =>
                {
                    result = false;
                    messageBox.Close();
                };
                Grid.SetColumn(laterButton, 2);

                buttonPanel.Children.Add(updateButton);
                if (allowLater)
                {
                    buttonPanel.Children.Add(laterButton);
                }
                Grid.SetRow(buttonPanel, 2);
                container.Children.Add(buttonPanel);

                messageBox.Content = container;

                // Show as standalone window and wait for it to close
                var tcs = new TaskCompletionSource<bool>();
                messageBox.Closed += (s, e) => tcs.SetResult(result);
                messageBox.Show();

                return await tcs.Task;
            }
            catch
            {
                return false;
            }
        }

        private static Control CreateRichReleaseNotesBlock(string text)
        {
            var block = new TextBlock
            {
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                FontSize = 12
            };

            foreach (var inline in ParseSimpleMarkdownToInlines(text))
            {
                block.Inlines!.Add(inline);
            }

            return block;
        }

        private static async Task<string> FetchLatestReleaseNotesFromGithubAsync()
        {
            try
            {
                // Keep this local to the app for test purposes (no extra permissions needed).
                const string owner = "haitnmt";
                const string repo = "Vbdlis-Tools";
                var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Vbdlis-Tools-UpdateDialogTest");

                var json = await client.GetStringAsync(apiUrl);
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("body", out var bodyElement))
                {
                    var body = bodyElement.GetString() ?? string.Empty;
                    return ExtractAppUpdateNotesFromReleaseBody(body);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[TEST] Failed to fetch GitHub release notes");
                return string.Empty;
            }
        }

        private static string ExtractAppUpdateNotesFromReleaseBody(string fullReleaseNotes)
        {
            const string startMarker = "<!-- APP_UPDATE_NOTES_START -->";
            const string endMarker = "<!-- APP_UPDATE_NOTES_END -->";

            if (string.IsNullOrWhiteSpace(fullReleaseNotes))
            {
                return string.Empty;
            }

            var startIndex = fullReleaseNotes.IndexOf(startMarker, StringComparison.Ordinal);
            var endIndex = fullReleaseNotes.IndexOf(endMarker, StringComparison.Ordinal);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                startIndex += startMarker.Length;
                return fullReleaseNotes.Substring(startIndex, endIndex - startIndex).Trim();
            }

            // If markers are missing, fall back to the full body.
            return fullReleaseNotes.Trim();
        }

        private static IEnumerable<Inline> ParseSimpleMarkdownToInlines(string input)
        {
            // Minimal markdown-like parser for update notes:
            // - Headings: ### Title
            // - Bullets: - item
            // - Bold: **text**
            // - Blank lines
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            var normalized = input.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = normalized.Split('\n');

            var firstLine = true;
            foreach (var raw in lines)
            {
                var line = (raw ?? string.Empty).TrimEnd();

                if (!firstLine)
                {
                    yield return new LineBreak();
                }
                firstLine = false;

                if (string.IsNullOrWhiteSpace(line))
                {
                    // Keep a blank line for readability
                    yield return new LineBreak();
                    continue;
                }

                // Heading (e.g. ### ...)
                if (Regex.IsMatch(line, @"^#{1,6}\s+"))
                {
                    var headingText = Regex.Replace(line, @"^#{1,6}\s+", string.Empty).Trim();
                    var headingSpan = new Span
                    {
                        FontWeight = Avalonia.Media.FontWeight.SemiBold,
                        FontSize = 13
                    };
                    foreach (var i in ParseBoldSegments(headingText))
                        headingSpan.Inlines!.Add(i);
                    yield return headingSpan;
                    continue;
                }

                // Bullet (- item)
                if (Regex.IsMatch(line, @"^[-*]\s+"))
                {
                    yield return new Run("• ");
                    var bulletText = Regex.Replace(line, @"^[-*]\s+", string.Empty);
                    foreach (var i in ParseBoldSegments(bulletText))
                        yield return i;
                    continue;
                }

                // Plain line (may contain **bold**)
                foreach (var i in ParseBoldSegments(line))
                    yield return i;
            }
        }

        private static IEnumerable<Inline> ParseBoldSegments(string line)
        {
            // Split by **bold** segments. This is intentionally simple and safe.
            // Example: "abc **bold** def" -> Run("abc ") + Bold(Run("bold")) + Run(" def")
            if (string.IsNullOrEmpty(line))
                yield break;

            var parts = Regex.Split(line, @"(\*\*[^*]+\*\*)");
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                if (part.StartsWith("**") && part.EndsWith("**") && part.Length >= 4)
                {
                    var boldText = part.Substring(2, part.Length - 4);
                    var bold = new Bold();
                    bold.Inlines!.Add(new Run(boldText));
                    yield return bold;
                }
                else
                {
                    yield return new Run(part);
                }
            }
        }

        /// <summary>
        /// Creates a small progress window for update download/install steps
        /// </summary>
        private (Window window, ProgressBar progressBar, TextBlock statusText) CreateUpdateProgressWindow()
        {
            var progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Height = 14
            };

            var statusText = new TextBlock
            {
                Text = "Đang chuẩn bị...",
                FontSize = 12,
                Foreground = Avalonia.Media.Brushes.Gray
            };

            var stackPanel = new StackPanel
            {
                Spacing = 12,
                Margin = new Thickness(20),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Đang tải bản cập nhật",
                FontSize = 16,
                FontWeight = Avalonia.Media.FontWeight.SemiBold
            });

            stackPanel.Children.Add(progressBar);
            stackPanel.Children.Add(statusText);

            var window = new Window
            {
                Title = "Đang cập nhật",
                Width = 420,
                Height = 133,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = stackPanel
            };

            return (window, progressBar, statusText);
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
