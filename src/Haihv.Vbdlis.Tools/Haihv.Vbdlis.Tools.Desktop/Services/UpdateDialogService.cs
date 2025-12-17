using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Haihv.Vbdlis.Tools.Desktop.Helpers;

namespace Haihv.Vbdlis.Tools.Desktop.Services
{
    public class UpdateDialogService : IUpdateDialogService
    {
        /// <summary>
        /// Shows update dialog to user and returns their choice
        /// </summary>
        public async Task<bool> ShowUpdateDialogAsync(UpdateInfo updateInfo, string currentVersion,
            bool allowLater = true)
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

                // Header panel with version info
                var headerPanel = CreateHeaderPanel(updateInfo, currentVersion);
                Grid.SetRow(headerPanel, 0);
                container.Children.Add(headerPanel);

                // Release notes panel
                var infoPanel = CreateReleaseNotesPanel(updateInfo.ReleaseNotes);
                Grid.SetRow(infoPanel, 1);
                container.Children.Add(infoPanel);

                // Button panel
                var result = false;
                var buttonPanel = CreateButtonPanel(allowLater, () =>
                {
                    result = true;
                    messageBox.Close();
                }, () =>
                {
                    result = false;
                    messageBox.Close();
                });
                Grid.SetRow(buttonPanel, 2);
                container.Children.Add(buttonPanel);

                messageBox.Content = container;

                // If force update (not allowing later), prevent closing
                if (!allowLater)
                {
                    messageBox.Closing += (_, e) =>
                    {
                        if (!result)
                        {
                            e.Cancel = true;
                        }
                    };
                }

                // Show as standalone window and wait for it to close
                var tcs = new TaskCompletionSource<bool>();
                messageBox.Closed += (_, _) => tcs.SetResult(result);
                messageBox.Show();

                return await tcs.Task;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Shows progress window for update download/installation
        /// </summary>
        public (Action<int> UpdateProgress, Action<string> UpdateStatus, Action Close) ShowProgressWindow()
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
                Foreground = Brushes.Gray
            };

            var stackPanel = new StackPanel
            {
                Spacing = 12,
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Đang tải bản cập nhật",
                FontSize = 16,
                FontWeight = FontWeight.SemiBold
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

            window.Show();

            return (
                progress => progressBar.Value = progress,
                status => statusText.Text = status,
                () => window.Close()
            );
        }

        /// <summary>
        /// Creates the header panel with version information
        /// </summary>
        private Grid CreateHeaderPanel(UpdateInfo updateInfo, string currentVersion)
        {
            var headerPanel = new Grid
            {
                RowDefinitions = new RowDefinitions("*,*"),
            };

            var phienBanHienTai = new TextBlock
            {
                Text = $"Phiên bản hiện tại: {currentVersion}",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Left
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
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Left,
                FontWeight = FontWeight.SemiBold
            };
            Grid.SetRow(phienBanMoi, 1);
            headerPanel.Children.Add(phienBanMoi);

            return headerPanel;
        }

        /// <summary>
        /// Creates the release notes panel
        /// </summary>
        private Border CreateReleaseNotesPanel(string releaseNotes)
        {
            var releaseNotesText = string.IsNullOrWhiteSpace(releaseNotes)
                ? "Không có ghi chú phát hành."
                : releaseNotes;

            var infoPanel = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.LightGray,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 12, 0, 12),
                Padding = new Thickness(14)
            };

            var infoStack = new StackPanel { Spacing = 10 };

            infoStack.Children.Add(new TextBlock
            {
                Text = "Nội dung cập nhật",
                FontSize = 13,
                FontWeight = FontWeight.SemiBold
            });

            infoStack.Children.Add(new ScrollViewer
            {
                Height = 150,
                Content = CreateRichReleaseNotesBlock(releaseNotesText)
            });

            infoPanel.Child = infoStack;
            return infoPanel;
        }

        /// <summary>
        /// Creates rich text block from markdown
        /// </summary>
        private static Control CreateRichReleaseNotesBlock(string text)
        {
            var block = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12
            };

            foreach (var inline in MarkdownHelper.ParseToInlines(text))
            {
                block.Inlines!.Add(inline);
            }

            return block;
        }

        /// <summary>
        /// Creates button panel with update/later buttons
        /// </summary>
        private Grid CreateButtonPanel(bool allowLater, Action onUpdate, Action onLater)
        {
            var buttonPanel = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,*,*"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                ColumnSpacing = 12,
            };

            var updateButton = new Button
            {
                Content = "Cập nhật ngay",
                Padding = new Thickness(16, 10),
                Background = Brushes.ForestGreen,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            updateButton.Click += (_, _) => onUpdate();

            if (allowLater)
            {
                Grid.SetColumn(updateButton, 1);
            }
            else
            {
                Grid.SetColumn(updateButton, 0);
                Grid.SetColumnSpan(updateButton, 3);
            }

            buttonPanel.Children.Add(updateButton);

            if (!allowLater) return buttonPanel;
            {
                var laterButton = new Button
                {
                    Content = "Để sau",
                    Padding = new Thickness(16, 10),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                laterButton.Click += (_, _) => onLater();
                Grid.SetColumn(laterButton, 2);
                buttonPanel.Children.Add(laterButton);
            }

            return buttonPanel;
        }
    }
}