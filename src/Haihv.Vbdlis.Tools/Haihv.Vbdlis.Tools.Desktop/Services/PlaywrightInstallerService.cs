using System;
using System.IO;
using System.Runtime.InteropServices;
using Serilog;

namespace Haihv.Vbdlis.Tools.Desktop.Services
{
    /// <summary>
    /// Service for managing Playwright browser detection.
    /// Detects bundled browsers and copies them to the system cache when needed.
    /// </summary>
    public class PlaywrightInstallerService : IPlaywrightInstallerService
    {
        private readonly ILogger _logger;

        public PlaywrightInstallerService()
        {
            _logger = Log.ForContext<PlaywrightInstallerService>();
        }

        /// <summary>
        /// Gets the current operating system name
        /// </summary>
        public string GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "MacOS";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";

            return "Unknown";
        }

        /// <summary>
        /// Gets the Playwright browsers directory path based on OS
        /// </summary>
        private string GetPlaywrightBrowsersPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: %USERPROFILE%\AppData\Local\ms-playwright
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppData, "ms-playwright");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // MacOS: ~/Library/Caches/ms-playwright
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(home, "Library", "Caches", "ms-playwright");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: ~/.cache/ms-playwright
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(home, ".cache", "ms-playwright");
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks if Playwright browsers are installed.
        /// Prefer bundled browsers (no copy), fallback to system cache.
        /// </summary>
        public bool IsBrowsersInstalled()
        {
            try
            {
                var appDir = AppDomain.CurrentDomain.BaseDirectory;
                var bundledBrowsersPath = Path.Combine(appDir, ".playwright-browsers");
                var systemCachePath = GetPlaywrightBrowsersPath();

                // Prefer bundled browsers to avoid copying to cache
                if (Directory.Exists(bundledBrowsersPath))
                {
                    var bundledChromium = Directory.GetDirectories(bundledBrowsersPath, "chromium-*");
                    if (bundledChromium.Length > 0)
                    {
                        Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", bundledBrowsersPath);
                        _logger.Information("Using bundled Playwright browsers at: {Path}", bundledBrowsersPath);
                        return true;
                    }
                    else
                    {
                        _logger.Warning("Bundled browsers folder found but no chromium-* inside: {Path}", bundledBrowsersPath);
                    }
                }

                // Fallback to system cache if bundled browsers are missing
                if (!string.IsNullOrEmpty(systemCachePath) && HasChromiumInstalled(systemCachePath))
                {
                    Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", systemCachePath);
                    _logger.Information("Playwright browsers found in system cache: {Path}", systemCachePath);
                    return true;
                }

                _logger.Warning("Playwright browsers not found. Checked bundle: {BundlePath}, cache: {CachePath}", bundledBrowsersPath, systemCachePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking Playwright browsers installation");
                return false;
            }
        }

        private bool HasChromiumInstalled(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return false;
            }

            try
            {
                var hasChromium = Directory.GetDirectories(path, "chromium-*").Length > 0;
                var hasHeadlessShell = Directory.GetDirectories(path, "chromium_headless_shell-*").Length > 0;
                var hasFfmpeg = Directory.GetDirectories(path, "ffmpeg-*").Length > 0;

                if (!hasChromium || !hasHeadlessShell || !hasFfmpeg)
                {
                    _logger.Warning("Chromium installation incomplete at {Path}. chromium: {HasChromium}, headless_shell: {HasHeadless}, ffmpeg: {HasFfmpeg}",
                        path, hasChromium, hasHeadlessShell, hasFfmpeg);
                }

                return hasChromium && hasHeadlessShell && hasFfmpeg;
            }
            catch
            {
                return false;
            }
        }

    }
}
