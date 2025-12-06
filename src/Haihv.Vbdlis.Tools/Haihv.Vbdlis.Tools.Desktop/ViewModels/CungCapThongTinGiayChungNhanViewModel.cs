using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Haihv.Vbdlis.Tools.Desktop.Models;
using Haihv.Vbdlis.Tools.Desktop.Services;
using Microsoft.Playwright;
using Serilog;

namespace Haihv.Vbdlis.Tools.Desktop.ViewModels;

/// <summary>
/// ViewModel cho tìm kiếm nâng cao Giấy chứng nhận trên VBDLIS sử dụng Playwright
/// </summary>
public class CungCapThongTinGiayChungNhanViewModel
{
    private readonly ILogger _logger = Log.ForContext<CungCapThongTinGiayChungNhanViewModel>();
    private readonly IPlaywrightService _playwrightService;
    private readonly LoginSessionInfo _loginSessionInfo;
    private IPage? _page;

    public CungCapThongTinGiayChungNhanViewModel(
        IPlaywrightService playwrightService,
        LoginSessionInfo loginSessionInfo)
    {
        _playwrightService = playwrightService;
        _loginSessionInfo = loginSessionInfo;
    }

    /// <summary>
    /// Gọi API tìm kiếm nâng cao Giấy chứng nhận theo số phát hành hoặc số giấy tờ
    /// </summary>
    public async Task<AdvancedSearchGiayChungNhanResponse?> SearchAsync(
        string? soPhatHanh = null,
        string? soGiayTo = null)
    {
        if (string.IsNullOrWhiteSpace(soPhatHanh) && string.IsNullOrWhiteSpace(soGiayTo))
        {
            throw new ArgumentNullException(nameof(soPhatHanh),
                "Số phát hành hoặc Số giấy tờ phải được cung cấp để tìm kiếm.");
        }

        await EnsureCungCapThongTinPageAsync();
        var formData = CungCapThongTinGiayChungNhanPayload.GetAdvancedSearchGiayChungNhanPayload(
            soPhatHanh, soGiayTo);
        var json = await PostAdvancedSearchAsync(formData);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var response = AdvancedSearchGiayChungNhanResponse.FromJson(json);
            if (response == null)
            {
                _logger.Warning("Advanced search Giấy chứng nhận trả về null.");
            }
            return response;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.Error(ex, "Lỗi parse kết quả advanced search Giấy chứng nhận.");
            Debug.WriteLine($"AdvancedSearchGCN Parse Error: {ex.Message}");
            Debug.WriteLine(json);
            return null;
        }
    }

    /// <summary>
    /// Gọi API tìm kiếm nâng cao Giấy chứng nhận theo đơn vị hành chính và Tờ bản đồ, thửa đất
    /// </summary>
    public async Task<AdvancedSearchGiayChungNhanResponse?> SearchAsync(
        int thuTuThua,
        int toBanDo,
        int xaId,
        int tinhId = 24)
    {
        tinhId = tinhId <= 0 ? 24 : tinhId;
        if (xaId <= 0 || thuTuThua <= 0 || toBanDo <= 0)
        {
            throw new ArgumentNullException(nameof(xaId),
                "Đơn vị hành chính, Tờ bản đồ và Thửa đất phải được cung cấp để tìm kiếm.");
        }

        await EnsureCungCapThongTinPageAsync();
        var formData = CungCapThongTinGiayChungNhanPayload.GetAdvancedSearchGiayChungNhanPayload(
            thuTuThua, toBanDo, xaId, tinhId);
        var json = await PostAdvancedSearchAsync(formData);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var response = AdvancedSearchGiayChungNhanResponse.FromJson(json);
            if (response == null)
            {
                _logger.Warning("Advanced search Giấy chứng nhận trả về null.");
            }
            return response;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.Error(ex, "Lỗi parse kết quả advanced search Giấy chứng nhận.");
            Debug.WriteLine($"AdvancedSearchGCN Parse Error: {ex.Message}");
            Debug.WriteLine(json);
            return null;
        }
    }

    /// <summary>
    /// Gọi AJAX POST request sử dụng Playwright
    /// </summary>
    private async Task<string> PostAdvancedSearchAsync(string formData)
    {
        if (_page == null)
        {
            throw new InvalidOperationException("Page not initialized. Call EnsureCungCapThongTinPageAsync first.");
        }

        var sanitizedFormData = formData.Replace("\r", "").Replace("\n", "");

        const string script = """
            async ([url, payload]) => {
                if (typeof $ === 'undefined' || typeof $.ajax === 'undefined') {
                    return JSON.stringify({ error: 'jQuery not available on page' });
                }

                return new Promise((resolve) => {
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: payload,
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        timeout: 120000,
                        success: function(data) {
                            resolve(typeof data === 'object' ? JSON.stringify(data) : data);
                        },
                        error: function(xhr, status, error) {
                            resolve(JSON.stringify({
                                error: error,
                                status: xhr.status,
                                statusText: status,
                                responseText: xhr.responseText
                            }));
                        }
                    });
                });
            }
            """;

        try
        {
            var response = await _page.EvaluateAsync<string>(
                script,
                new object[] { _loginSessionInfo.AdvancedSearchGiayChungNhanUrl, sanitizedFormData });

            Debug.WriteLine($"AdvancedSearchGCN: Response length {response?.Length ?? 0}");
            return response ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Lỗi gọi AdvancedSearchGiayChungNhan");
            Debug.WriteLine($"AdvancedSearchGCN EvaluateAsync Error: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Đảm bảo đã mở trang cung cấp thông tin GCN
    /// </summary>
    public async Task EnsureCungCapThongTinPageAsync()
    {
        if (!_playwrightService.IsInitialized)
        {
            await _playwrightService.InitializeAsync();
        }

        // Tạo page mới nếu chưa có
        if (_page == null)
        {
            _page = await _playwrightService.NewPageAsync();
        }

        try
        {
            // Kiểm tra nếu page đã ở đúng URL
            var currentUrl = _page.Url;
            var targetUrl = _loginSessionInfo.CungCapThongTinGiayChungNhanPageUrl;

            if (!string.IsNullOrEmpty(currentUrl) && currentUrl.StartsWith(targetUrl))
            {
                _logger.Debug("Already on CungCapThongTin page");
                return;
            }

            // Navigate đến trang
            await _page.GotoAsync(targetUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 60000
            });

            _logger.Information("Navigated to CungCapThongTin page: {Url}", targetUrl);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to navigate to CungCapThongTin page");
            throw;
        }
    }

    /// <summary>
    /// Đóng page hiện tại
    /// </summary>
    public async Task ClosePageAsync()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
            _page = null;
        }
    }
}
